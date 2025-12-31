#pragma warning disable CS1591

using Cysharp.Threading.Tasks.Internal;
using System;
using System.Linq;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Cysharp.Threading.Tasks.CompilerServices
{
    // #ENABLE_IL2CPP 在本文件中用于规避 IL2CPP 虚拟机的一个已知 Bug。
    // 问题链接：https://issuetracker.unity3d.com/issues/il2cpp-incorrect-results-when-calling-a-method-from-outside-class-in-a-struct
    // 该问题目前被标记为 `Won't Fix`，因此需要在框架层通过特殊结构与回调方式绕开。

    /// <summary>
    /// 状态机运行器的基础接口。
    /// 用于抽象「驱动状态机执行一次 MoveNext」以及「生命周期结束后归还到对象池」两个核心操作。
    /// </summary>
    internal interface IStateMachineRunner
    {
        /// <summary>
        /// 推进状态机的委托入口，一般直接绑定到 stateMachine.MoveNext。
        /// 将 MoveNext 暴露为 <see cref="Action"/> 便于统一调度和缓存委托实例。
        /// </summary>
        Action MoveNext { get; }

        /// <summary>
        /// 状态机生命周期结束时调用，用于从任务跟踪中移除并将运行器放回对象池。
        /// </summary>
        void Return();

#if ENABLE_IL2CPP
        /// <summary>
        /// 在 IL2CPP 环境下，用于通过 PlayerLoop 延迟调用 Return 的委托。
        /// 这是为绕过 IL2CPP 在结构体方法调用上的 Bug 而设计的特殊入口。
        /// </summary>
        Action ReturnAction { get; }
#endif
    }

    /// <summary>
    /// 既是状态机运行器，又实现 UniTask 源（无返回值）的接口。
    /// async UniTask / async void 方法在编译后会使用该接口来驱动状态机并传递结果或异常。
    /// </summary>
    internal interface IStateMachineRunnerPromise : IUniTaskSource
    {
        /// <summary>
        /// 推进状态机的入口。
        /// </summary>
        Action MoveNext { get; }

        /// <summary>
        /// 与该状态机关联的 UniTask，用于外部 await。
        /// </summary>
        UniTask Task { get; }

        /// <summary>
        /// 标记任务成功完成。
        /// </summary>
        void SetResult();

        /// <summary>
        /// 标记任务失败并记录异常。
        /// </summary>
        void SetException(Exception exception);
    }

    /// <summary>
    /// 带返回值版本的状态机运行器 + UniTask 源接口。
    /// 用于 async UniTask&lt;T&gt; 方法的执行与结果传递。
    /// </summary>
    internal interface IStateMachineRunnerPromise<T> : IUniTaskSource<T>
    {
        /// <summary>
        /// 推进状态机的入口。
        /// </summary>
        Action MoveNext { get; }

        /// <summary>
        /// 与该状态机关联的 UniTask&lt;T&gt;，用于外部 await。
        /// </summary>
        UniTask<T> Task { get; }

        /// <summary>
        /// 标记任务成功完成并提供结果值。
        /// </summary>
        void SetResult(T result);

        /// <summary>
        /// 标记任务失败并记录异常。
        /// </summary>
        void SetException(Exception exception);
    }

    /// <summary>
    /// 状态机相关的辅助工具类。
    /// 目前主要用于通过反射读取编译器生成的 async 状态机内部状态字段，辅助调试或诊断 IL2CPP Bug。
    /// </summary>
    internal static class StateMachineUtility
    {
        /// <summary>
        /// 通过反射获取 IAsyncStateMachine 内部编译生成的状态字段（通常命名为 &lt;方法名&gt;d__X.__state）。
        /// </summary>
        /// <param name="stateMachine">编译器生成的 async 状态机实例。</param>
        /// <returns>当前状态机的状态整数：一般 -1 表示已完成，0/1/2... 表示中间状态。</returns>
        public static int GetState(IAsyncStateMachine stateMachine)
        {
            var info = stateMachine.GetType().GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .First(x => x.Name.EndsWith("__state"));
            return (int)info.GetValue(stateMachine);
        }
    }

    /// <summary>
    /// 用于 async UniTaskVoid / async void（以 UniTaskVoid 语义运行）的状态机运行器。
    /// 只负责推进状态机与对象池管理，不暴露结果（仅配合 TaskTracker 做调试追踪）。
    /// </summary>
    internal sealed class AsyncUniTaskVoid<TStateMachine> : IStateMachineRunner, ITaskPoolNode<AsyncUniTaskVoid<TStateMachine>>, IUniTaskSource
        where TStateMachine : IAsyncStateMachine
    {
        // 对象池：避免频繁分配运行器实例。
        static TaskPool<AsyncUniTaskVoid<TStateMachine>> pool;

#if ENABLE_IL2CPP
        public Action ReturnAction { get; }
#endif

        // 编译后生成的 async 状态机实例（结构体）。
        TStateMachine stateMachine;

        /// <summary>
        /// 推进状态机的委托，会被赋值为 Run。
        /// </summary>
        public Action MoveNext { get; }

        public AsyncUniTaskVoid()
        {
            MoveNext = Run;
#if ENABLE_IL2CPP
            ReturnAction = Return;
#endif
        }

        /// <summary>
        /// 将状态机与运行器绑定，并从对象池中获取/创建运行器实例。
        /// 该方法通常在 AsyncMethodBuilder 中被调用。
        /// </summary>
        /// <param name="stateMachine">编译器生成的状态机结构体（ref 传递）。</param>
        /// <param name="runnerFieldRef">状态机中保存运行器引用的字段（ref 赋值）。</param>
        public static void SetStateMachine(ref TStateMachine stateMachine, ref IStateMachineRunner runnerFieldRef)
        {
            if (!pool.TryPop(out var result))
            {
                result = new AsyncUniTaskVoid<TStateMachine>();
            }
            TaskTracker.TrackActiveTask(result, 3);

            runnerFieldRef = result; // set runner before copied.
            result.stateMachine = stateMachine; // copy struct StateMachine(in release build).
        }

        // 静态构造：向 TaskPool 注册池大小查询函数，方便调试查看当前池占用情况。
        static AsyncUniTaskVoid()
        {
            TaskPool.RegisterSizeGetter(typeof(AsyncUniTaskVoid<TStateMachine>), () => pool.Size);
        }

        // TaskPool 链表下一节点引用。
        AsyncUniTaskVoid<TStateMachine> nextNode;
        public ref AsyncUniTaskVoid<TStateMachine> NextNode => ref nextNode;

        /// <summary>
        /// 将当前运行器从 TaskTracker 中移除并归还到对象池。
        /// </summary>
        public void Return()
        {
            TaskTracker.RemoveTracking(this);
            stateMachine = default;
            pool.TryPush(this);
        }

        [DebuggerHidden]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        /// <summary>
        /// 推进一次状态机，实际由 AsyncUniTaskVoid.MoveNext 委托调用。
        /// </summary>
        void Run()
        {
            stateMachine.MoveNext();
        }

        // 以下 IUniTaskSource 实现仅用于满足接口要求，让 TaskTracker 能统一处理；
        // 对于 UniTaskVoid 来说，这些方法不会真正被业务代码调用。

        UniTaskStatus IUniTaskSource.GetStatus(short token)
        {
            return UniTaskStatus.Pending;
        }

        UniTaskStatus IUniTaskSource.UnsafeGetStatus()
        {
            return UniTaskStatus.Pending;
        }

        void IUniTaskSource.OnCompleted(Action<object> continuation, object state, short token)
        {
        }

        void IUniTaskSource.GetResult(short token)
        {
        }
    }

    /// <summary>
    /// async UniTask（无返回值）对应的状态机运行器 + Promise。
    /// 负责：推进状态机、维护 UniTaskCompletionSourceCore、与对象池交互，以及向外部暴露 UniTask 实例。
    /// </summary>
    internal sealed class AsyncUniTask<TStateMachine> : IStateMachineRunnerPromise, IUniTaskSource, ITaskPoolNode<AsyncUniTask<TStateMachine>>
        where TStateMachine : IAsyncStateMachine
    {
        // 对象池，复用运行器实例以减少 GC。
        static TaskPool<AsyncUniTask<TStateMachine>> pool;

#if ENABLE_IL2CPP
        // IL2CPP 下用于延迟归还的委托引用，避免直接在 GetResult 的 finally 中调用 Return 触发 VM Bug。
        readonly Action returnDelegate;  
#endif
        /// <summary>
        /// 推进状态机的入口委托。
        /// </summary>
        public Action MoveNext { get; }

        // 编译器生成的状态机结构体。
        TStateMachine stateMachine;
        // UniTask 的底层核心源，负责结果/异常/取消的存储与回调调度。
        UniTaskCompletionSourceCore<AsyncUnit> core;

        AsyncUniTask()
        {
            MoveNext = Run;
#if ENABLE_IL2CPP
            returnDelegate = Return;
#endif
        }

        /// <summary>
        /// 绑定状态机与运行器，从对象池获取/创建运行器并写入到状态机字段。
        /// 通常由 AsyncUniTaskMethodBuilder 在编译器生成代码中调用。
        /// </summary>
        public static void SetStateMachine(ref TStateMachine stateMachine, ref IStateMachineRunnerPromise runnerPromiseFieldRef)
        {
            if (!pool.TryPop(out var result))
            {
                result = new AsyncUniTask<TStateMachine>();
            }
            TaskTracker.TrackActiveTask(result, 3);

            runnerPromiseFieldRef = result; // set runner before copied.
            result.stateMachine = stateMachine; // copy struct StateMachine(in release build).
        }

        // TaskPool 链表节点指针。
        AsyncUniTask<TStateMachine> nextNode;
        public ref AsyncUniTask<TStateMachine> NextNode => ref nextNode;

        // 静态构造，注册池大小查询方法，方便调试查看当前池占用。
        static AsyncUniTask()
        {
            TaskPool.RegisterSizeGetter(typeof(AsyncUniTask<TStateMachine>), () => pool.Size);
        }

        /// <summary>
        /// 将当前运行器从 TaskTracker 移除、重置核心状态，并归还到对象池。
        /// </summary>
        void Return()
        {
            TaskTracker.RemoveTracking(this);
            core.Reset();
            stateMachine = default;
            pool.TryPush(this);
        }

        /// <summary>
        /// 与 <see cref="Return"/> 类似，但返回是否成功放入对象池（池已满时可能失败）。
        /// </summary>
        bool TryReturn()
        {
            TaskTracker.RemoveTracking(this);
            core.Reset();
            stateMachine = default;
            return pool.TryPush(this);
        }

        [DebuggerHidden]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        /// <summary>
        /// 推进状态机一次，由 MoveNext 委托调用。
        /// </summary>
        void Run()
        {
            stateMachine.MoveNext();
        }

        /// <summary>
        /// 暴露给外部 await 的 UniTask 视图。
        /// 内部通过 core.Version 区分不同生命周期的等待者，防止版本错乱。
        /// </summary>
        public UniTask Task
        {
            [DebuggerHidden]
            get
            {
                return new UniTask(this, core.Version);
            }
        }

        [DebuggerHidden]
        /// <summary>
        /// 将任务标记为成功完成（AsyncUnit.Default）。
        /// 通常由状态机在最终状态调用。
        /// </summary>
        public void SetResult()
        {
            core.TrySetResult(AsyncUnit.Default);
        }

        [DebuggerHidden]
        /// <summary>
        /// 将任务标记为失败并记录异常。
        /// </summary>
        public void SetException(Exception exception)
        {
            core.TrySetException(exception);
        }

        [DebuggerHidden]
        /// <summary>
        /// IUniTaskSource.GetResult 实现：在 await 完成后由 Awaiter 调用。
        /// 在 finally 中根据运行环境选择立即归还或延迟归还到对象池。
        /// </summary>
        public void GetResult(short token)
        {
            try
            {
                core.GetResult(token);
            }
            finally
            {
#if ENABLE_IL2CPP
                // workaround for IL2CPP bug.
                PlayerLoopHelper.AddContinuation(PlayerLoopTiming.LastPostLateUpdate, returnDelegate);
#else
                TryReturn();
#endif
            }
        }

        [DebuggerHidden]
        public UniTaskStatus GetStatus(short token)
        {
            return core.GetStatus(token);
        }

        [DebuggerHidden]
        public UniTaskStatus UnsafeGetStatus()
        {
            return core.UnsafeGetStatus();
        }

        [DebuggerHidden]
        public void OnCompleted(Action<object> continuation, object state, short token)
        {
            core.OnCompleted(continuation, state, token);
        }
    }

    /// <summary>
    /// async UniTask&lt;T&gt;（带返回值）的状态机运行器 + Promise。
    /// 与 <see cref="AsyncUniTask{TStateMachine}"/> 类似，但内部核心类型与对外 Task 都携带结果值 T。
    /// </summary>
    internal sealed class AsyncUniTask<TStateMachine, T> : IStateMachineRunnerPromise<T>, IUniTaskSource<T>, ITaskPoolNode<AsyncUniTask<TStateMachine, T>>
        where TStateMachine : IAsyncStateMachine
    {
        // 对象池，存放 AsyncUniTask 状态机运行器实例。
        static TaskPool<AsyncUniTask<TStateMachine, T>> pool;

#if ENABLE_IL2CPP
        // IL2CPP 下用于延迟归还的委托，解决直接在 finally 中调用 Return 触发的 IL2CPP Bug。
        readonly Action returnDelegate;  
#endif

        /// <summary>
        /// 推进状态机的委托，指向 Run 方法。
        /// </summary>
        public Action MoveNext { get; }

        // 编译器生成的异步状态机结构体。
        TStateMachine stateMachine;
        // 带结果值的 UniTask 核心源。
        UniTaskCompletionSourceCore<T> core;

        AsyncUniTask()
        {
            MoveNext = Run;
#if ENABLE_IL2CPP
            returnDelegate = Return;
#endif
        }

        /// <summary>
        /// 将给定状态机与新的运行器实例绑定，并写入到状态机字段中。
        /// 由 AsyncUniTaskMethodBuilder{T} 在编译器生成代码中调用。
        /// </summary>
        public static void SetStateMachine(ref TStateMachine stateMachine, ref IStateMachineRunnerPromise<T> runnerPromiseFieldRef)
        {
            if (!pool.TryPop(out var result))
            {
                result = new AsyncUniTask<TStateMachine, T>();
            }
            TaskTracker.TrackActiveTask(result, 3);

            runnerPromiseFieldRef = result; // set runner before copied.
            result.stateMachine = stateMachine; // copy struct StateMachine(in release build).
        }

        // TaskPool 链表节点指针。
        AsyncUniTask<TStateMachine, T> nextNode;
        public ref AsyncUniTask<TStateMachine, T> NextNode => ref nextNode;

        // 静态构造，注册池大小获取委托，便于调试观察池中元素数量。
        static AsyncUniTask()
        {
            TaskPool.RegisterSizeGetter(typeof(AsyncUniTask<TStateMachine, T>), () => pool.Size);
        }

        /// <summary>
        /// 将当前运行器从 TaskTracker 中移除、重置核心与状态机，并尝试放回对象池。
        /// </summary>
        void Return()
        {
            TaskTracker.RemoveTracking(this);
            core.Reset();
            stateMachine = default;
            pool.TryPush(this);
        }

        /// <summary>
        /// 与 <see cref="Return"/> 类似，但返回是否成功放入对象池。
        /// </summary>
        bool TryReturn()
        {
            TaskTracker.RemoveTracking(this);
            core.Reset();
            stateMachine = default;
            return pool.TryPush(this);
        }

        [DebuggerHidden]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        /// <summary>
        /// 推进状态机一次。
        /// 可以在此处插入调试日志来观察状态机内部状态（示例代码已注释）。
        /// </summary>
        void Run()
        {
            // UnityEngine.Debug.Log($"MoveNext State:" + StateMachineUtility.GetState(stateMachine));
            stateMachine.MoveNext();
        }

        /// <summary>
        /// 对外暴露的 UniTask&lt;T&gt;，供 await 使用。
        /// 内部通过 core.Version 标识当前生命周期，防止跨生命周期的 GetResult 调用。
        /// </summary>
        public UniTask<T> Task
        {
            [DebuggerHidden]
            get
            {
                return new UniTask<T>(this, core.Version);
            }
        }

        [DebuggerHidden]
        /// <summary>
        /// 将任务标记为成功并设置结果值。
        /// </summary>
        public void SetResult(T result)
        {
            core.TrySetResult(result);
        }

        [DebuggerHidden]
        /// <summary>
        /// 将任务标记为失败并记录异常。
        /// </summary>
        public void SetException(Exception exception)
        {
            core.TrySetException(exception);
        }

        [DebuggerHidden]
        /// <summary>
        /// IUniTaskSource&lt;T&gt;.GetResult 实现：在 await 完成时由 Awaiter 调用以获取结果。
        /// 在 finally 中负责将运行器归还对象池或通过 PlayerLoop 延迟归还。
        /// </summary>
        public T GetResult(short token)
        {
            try
            {
                return core.GetResult(token);
            }
            finally
            {
#if ENABLE_IL2CPP
                // workaround for IL2CPP bug.
                PlayerLoopHelper.AddContinuation(PlayerLoopTiming.LastPostLateUpdate, returnDelegate);
#else
                TryReturn();
#endif
            }
        }

        [DebuggerHidden]
        void IUniTaskSource.GetResult(short token)
        {
            GetResult(token);
        }

        [DebuggerHidden]
        public UniTaskStatus GetStatus(short token)
        {
            return core.GetStatus(token);
        }

        [DebuggerHidden]
        public UniTaskStatus UnsafeGetStatus()
        {
            return core.UnsafeGetStatus();
        }

        [DebuggerHidden]
        public void OnCompleted(Action<object> continuation, object state, short token)
        {
            core.OnCompleted(continuation, state, token);
        }
    }
}

