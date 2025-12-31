#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable CS0436

#if UNITASK_NETCORE || UNITY_2022_3_OR_NEWER
#define SUPPORT_VALUETASK
#endif

using Cysharp.Threading.Tasks.CompilerServices;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;

namespace Cysharp.Threading.Tasks
{
    /// <summary>
    /// Awaiter 相关的公共委托封装工具类。
    /// 主要用于将通用的 <see cref="Action"/> 包装为 IUniTaskSource.OnCompleted 所需的回调形式，并复用单例委托以减少 GC 分配。
    /// </summary>
    internal static class AwaiterActions
    {
        /// <summary>
        /// 统一使用的延续调用委托，将装箱的 <see cref="Action"/> 对象强转后直接调用。
        /// 通过静态只读字段避免为每次 await 生成新的委托实例。
        /// </summary>
        internal static readonly Action<object> InvokeContinuationDelegate = Continuation;

        [DebuggerHidden]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void Continuation(object state)
        {
            // state 始终约定为 Action，这里只做强制转换然后执行，避免额外的包装开销。
            ((Action)state).Invoke();
        }
    }

    /// <summary>
    /// 轻量级、面向 Unity 的 Task 替代类型。
    /// 使用结构体 + <see cref="IUniTaskSource"/> 的组合来表示异步状态，相比标准 <see cref="System.Threading.Tasks.Task"/> 能大幅减少堆分配与 GC。
    /// </summary>
    [AsyncMethodBuilder(typeof(AsyncUniTaskMethodBuilder))]
    [StructLayout(LayoutKind.Auto)]
    public readonly partial struct UniTask
    {
        /// <summary>
        /// 实际承载异步状态机的源对象，实现 <see cref="IUniTaskSource"/> 接口。
        /// 当为 null 时表示任务已经成功完成且不再持有任何异步状态。
        /// </summary>
        readonly IUniTaskSource source;

        /// <summary>
        /// 与 <see cref="source"/> 绑定的版本号，用于防止重复消费同一异步源，保证 await 调用的安全性。
        /// </summary>
        readonly short token;

        [DebuggerHidden]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public UniTask(IUniTaskSource source, short token)
        {
            this.source = source;
            this.token = token;
        }

        /// <summary>
        /// 当前任务的执行状态。
        /// 如果内部 source 为 null，则直接认定为 <see cref="UniTaskStatus.Succeeded"/>。
        /// </summary>
        public UniTaskStatus Status
        {
            [DebuggerHidden]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                if (source == null) return UniTaskStatus.Succeeded;
                return source.GetStatus(token);
            }
        }

        [DebuggerHidden]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Awaiter GetAwaiter()
        {
            // 返回轻量级 Awaiter，用于支持 await UniTask; 编译器会基于该 Awaiter 生成状态机。
            return new Awaiter(this);
        }

        /// <summary>
        /// 在任务被取消时不抛出 <see cref="OperationCanceledException"/>，而是返回布尔值指示是否被取消。
        /// 适合使用返回值而非异常流程来处理取消逻辑的业务代码。
        /// </summary>
        public UniTask<bool> SuppressCancellationThrow()
        {
            var status = Status;
            if (status == UniTaskStatus.Succeeded) return CompletedTasks.False;
            if (status == UniTaskStatus.Canceled) return CompletedTasks.True;
            return new UniTask<bool>(new IsCanceledSource(source), token);
        }

#if SUPPORT_VALUETASK

        /// <summary>
        /// 将 <see cref="UniTask"/> 隐式转换为 <see cref="System.Threading.Tasks.ValueTask"/>。
        /// 在支持 ValueTask 的运行环境中，可与依赖 ValueTask 的第三方库无缝互操作。
        /// </summary>
        public static implicit operator System.Threading.Tasks.ValueTask(in UniTask self)
        {
            if (self.source == null)
            {
                return default;
            }

#if (UNITASK_NETCORE && NETSTANDARD2_0)
            return self.AsValueTask();
#else
            return new System.Threading.Tasks.ValueTask(self.source, self.token);
#endif
        }

#endif

        /// <summary>
        /// 返回当前任务的调试字符串，主要用于日志输出与调试观察。
        /// </summary>
        public override string ToString()
        {
            if (source == null) return "()";
            return "(" + source.UnsafeGetStatus() + ")";
        }

        /// <summary>
        /// 对内部 <see cref="IUniTaskSource"/> 结果进行缓存，使返回的 UniTask 可以被多次 await。
        /// 首次完成时会用 <see cref="MemoizeSource"/> 包装原始源，后续 await 只读取缓存结果或异常。
        /// </summary>
        public UniTask Preserve()
        {
            if (source == null)
            {
                return this;
            }
            else
            {
                return new UniTask(new MemoizeSource(source), token);
            }
        }

        /// <summary>
        /// 将当前无返回值的 UniTask 转换为返回 <see cref="AsyncUnit"/> 的 <see cref="UniTask{AsyncUnit}"/>。
        /// 便于在需要统一处理「有返回值/无返回值」任务时使用统一泛型形式。
        /// </summary>
        public UniTask<AsyncUnit> AsAsyncUnitUniTask()
        {
            if (this.source == null) return CompletedTasks.AsyncUnit;

            var status = this.source.GetStatus(this.token);
            if (status.IsCompletedSuccessfully())
            {
                this.source.GetResult(this.token);
                return CompletedTasks.AsyncUnit;
            }
            else if (this.source is IUniTaskSource<AsyncUnit> asyncUnitSource)
            {
                return new UniTask<AsyncUnit>(asyncUnitSource, this.token);
            }

            return new UniTask<AsyncUnit>(new AsyncUnitSource(this.source), this.token);
        }

        /// <summary>
        /// 适配器：将无返回值的 <see cref="IUniTaskSource"/> 包装为返回 <see cref="AsyncUnit"/> 的泛型源。
        /// 用于支持 <see cref="AsAsyncUnitUniTask"/> 等 API。
        /// </summary>
        sealed class AsyncUnitSource : IUniTaskSource<AsyncUnit>
        {
            readonly IUniTaskSource source;

            public AsyncUnitSource(IUniTaskSource source)
            {
                this.source = source;
            }

            public AsyncUnit GetResult(short token)
            {
                source.GetResult(token);
                return AsyncUnit.Default;
            }

            public UniTaskStatus GetStatus(short token)
            {
                return source.GetStatus(token);
            }

            public void OnCompleted(Action<object> continuation, object state, short token)
            {
                source.OnCompleted(continuation, state, token);
            }

            public UniTaskStatus UnsafeGetStatus()
            {
                return source.UnsafeGetStatus();
            }

            void IUniTaskSource.GetResult(short token)
            {
                GetResult(token);
            }
        }

        /// <summary>
        /// 适配器：用于实现 <see cref="SuppressCancellationThrow"/>，
        /// 将「是否被取消」转化为布尔结果，而不是直接抛出取消异常。
        /// </summary>
        sealed class IsCanceledSource : IUniTaskSource<bool>
        {
            readonly IUniTaskSource source;

            public IsCanceledSource(IUniTaskSource source)
            {
                this.source = source;
            }

            public bool GetResult(short token)
            {
                if (source.GetStatus(token) == UniTaskStatus.Canceled)
                {
                    return true;
                }

                source.GetResult(token);
                return false;
            }

            void IUniTaskSource.GetResult(short token)
            {
                GetResult(token);
            }

            public UniTaskStatus GetStatus(short token)
            {
                return source.GetStatus(token);
            }

            public UniTaskStatus UnsafeGetStatus()
            {
                return source.UnsafeGetStatus();
            }

            public void OnCompleted(Action<object> continuation, object state, short token)
            {
                source.OnCompleted(continuation, state, token);
            }
        }

        /// <summary>
        /// 结果缓存源：在第一次执行完成后缓存状态（成功/取消/异常），并释放原始源引用。
        /// 后续对同一任务的 await 将只读取缓存结果，实现「一次执行，多次等待」的效果。
        /// </summary>
        sealed class MemoizeSource : IUniTaskSource
        {
            IUniTaskSource source;
            ExceptionDispatchInfo exception;
            UniTaskStatus status;

            public MemoizeSource(IUniTaskSource source)
            {
                this.source = source;
            }

            public void GetResult(short token)
            {
                if (source == null)
                {
                    if (exception != null)
                    {
                        exception.Throw();
                    }
                }
                else
                {
                    try
                    {
                        source.GetResult(token);
                        status = UniTaskStatus.Succeeded;
                    }
                    catch (Exception ex)
                    {
                        exception = ExceptionDispatchInfo.Capture(ex);
                        if (ex is OperationCanceledException)
                        {
                            status = UniTaskStatus.Canceled;
                        }
                        else
                        {
                            status = UniTaskStatus.Faulted;
                        }
                        throw;
                    }
                    finally
                    {
                        source = null;
                    }
                }
            }

            public UniTaskStatus GetStatus(short token)
            {
                if (source == null)
                {
                    return status;
                }

                return source.GetStatus(token);
            }

            public void OnCompleted(Action<object> continuation, object state, short token)
            {
                if (source == null)
                {
                    continuation(state);
                }
                else
                {
                    source.OnCompleted(continuation, state, token);
                }
            }

            public UniTaskStatus UnsafeGetStatus()
            {
                if (source == null)
                {
                    return status;
                }

                return source.UnsafeGetStatus();
            }
        }

        /// <summary>
        /// 与 <see cref="UniTask"/> 配套的 Awaiter，实现 await 协议所需的接口。
        /// 内部通过引用原始 UniTask 实例来转发状态与结果获取。
        /// </summary>
        public readonly struct Awaiter : ICriticalNotifyCompletion
        {
            readonly UniTask task;

            [DebuggerHidden]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Awaiter(in UniTask task)
            {
                // 仅持有一个只读 UniTask 引用，不产生额外分配。
                this.task = task;
            }

            /// <summary>
            /// 指示任务当前是否已经完成（包含成功、失败与取消三种终态）。
            /// 编译器会基于该值决定是同步执行 GetResult 还是注册异步回调。
            /// </summary>
            public bool IsCompleted
            {
                [DebuggerHidden]
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get
                {
                    return task.Status.IsCompleted();
                }
            }

            [DebuggerHidden]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void GetResult()
            {
                // 无返回值任务仅用于触发异常/取消检查；source 为空则表示已成功完成，无需任何操作。
                if (task.source == null) return;
                task.source.GetResult(task.token);
            }

            [DebuggerHidden]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void OnCompleted(Action continuation)
            {
                if (task.source == null)
                {
                    continuation();
                }
                else
                {
                    task.source.OnCompleted(AwaiterActions.InvokeContinuationDelegate, continuation, task.token);
                }
            }

            [DebuggerHidden]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void UnsafeOnCompleted(Action continuation)
            {
                if (task.source == null)
                {
                    continuation();
                }
                else
                {
                    task.source.OnCompleted(AwaiterActions.InvokeContinuationDelegate, continuation, task.token);
                }
            }

            /// <summary>
            /// 手动注册 continuation 的辅助方法，可在自定义组合器中直接将回调转发给底层 IUniTaskSource。
            /// 当不希望使用编译器生成的 OnCompleted 调用路径时可以使用该方法。
            /// </summary>
            [DebuggerHidden]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void SourceOnCompleted(Action<object> continuation, object state)
            {
                if (task.source == null)
                {
                    continuation(state);
                }
                else
                {
                    task.source.OnCompleted(continuation, state, task.token);
                }
            }
        }
    }

    /// <summary>
    /// 带返回值的轻量级 Unity 异步任务类型。
    /// 通过将结果与异步源分离，大量同步完成的场景可以完全避免堆分配，提高性能并降低 GC 压力。
    /// </summary>
    [AsyncMethodBuilder(typeof(AsyncUniTaskMethodBuilder<>))]
    [StructLayout(LayoutKind.Auto)]
    public readonly struct UniTask<T>
    {
        /// <summary>
        /// 实际的异步源对象，实现 <see cref="IUniTaskSource{T}"/>。
        /// 当为 null 时表示该任务已同步完成，真实结果存储在 <see cref="result"/> 字段中。
        /// </summary>
        readonly IUniTaskSource<T> source;

        /// <summary>
        /// 同步完成时的结果值；当 <see cref="source"/> 为 null 时有效。
        /// </summary>
        readonly T result;

        /// <summary>
        /// 与异步源绑定的版本号，用于区分不同生命周期的 await 请求。
        /// </summary>
        readonly short token;

        [DebuggerHidden]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public UniTask(T result)
        {
            this.source = default;
            this.token = default;
            this.result = result;
        }

        [DebuggerHidden]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public UniTask(IUniTaskSource<T> source, short token)
        {
            this.source = source;
            this.token = token;
            this.result = default;
        }

        /// <summary>
        /// 当前任务的状态；若 source 为 null，则直接视为成功完成。
        /// </summary>
        public UniTaskStatus Status
        {
            [DebuggerHidden]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return (source == null) ? UniTaskStatus.Succeeded : source.GetStatus(token);
            }
        }

        [DebuggerHidden]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Awaiter GetAwaiter()
        {
            // 返回 Awaiter 以支持 await UniTask<T> 语法。
            return new Awaiter(this);
        }

        /// <summary>
        /// 对内部 IUniTaskSource 进行结果缓存，使该 UniTask{T} 可被多次 await。
        /// 实际通过包装为 <see cref="MemoizeSource"/> 实现。
        /// </summary>
        public UniTask<T> Preserve()
        {
            if (source == null)
            {
                return this;
            }
            else
            {
                return new UniTask<T>(new MemoizeSource(source), token);
            }
        }

        /// <summary>
        /// 将带返回值的 UniTask{T} 转换为不带返回值的 UniTask。
        /// 如果任务已经完成，则在转换过程中消费一次结果以进行异常/取消检查。
        /// </summary>
        public UniTask AsUniTask()
        {
            if (this.source == null) return UniTask.CompletedTask;

            var status = this.source.GetStatus(this.token);
            if (status.IsCompletedSuccessfully())
            {
                this.source.GetResult(this.token);
                return UniTask.CompletedTask;
            }

            // Converting UniTask<T> -> UniTask is zero overhead.
            return new UniTask(this.source, this.token);
        }

        /// <summary>
        /// 隐式转换为无返回值的 UniTask，语义等价于 <see cref="AsUniTask"/>。
        /// </summary>
        public static implicit operator UniTask(UniTask<T> self)
        {
            return self.AsUniTask();
        }

#if SUPPORT_VALUETASK

        /// <summary>
        /// 将 UniTask{T} 隐式转换为 ValueTask{T}，方便与基于 ValueTask 的 API 互操作。
        /// </summary>
        public static implicit operator System.Threading.Tasks.ValueTask<T>(in UniTask<T> self)
        {
            if (self.source == null)
            {
                return new System.Threading.Tasks.ValueTask<T>(self.result);
            }

#if (UNITASK_NETCORE && NETSTANDARD2_0)
            return self.AsValueTask();
#else
            return new System.Threading.Tasks.ValueTask<T>(self.source, self.token);
#endif
        }

#endif

        /// <summary>
        /// 在取消时不抛出 <see cref="OperationCanceledException"/>，而是返回 (IsCanceled, Result) 形式的元组。
        /// 取消时 IsCanceled 为 true，Result 为 default(T)。
        /// </summary>
        public UniTask<(bool IsCanceled, T Result)> SuppressCancellationThrow()
        {
            if (source == null)
            {
                return new UniTask<(bool IsCanceled, T Result)>((false, result));
            }

            return new UniTask<(bool, T)>(new IsCanceledSource(source), token);
        }

        /// <summary>
        /// 返回当前任务的调试字符串。
        /// 若任务为同步完成，则输出结果的 ToString；否则输出底层源的状态。
        /// </summary>
        public override string ToString()
        {
            return (this.source == null) ? result?.ToString()
                 : "(" + this.source.UnsafeGetStatus() + ")";
        }

        /// <summary>
        /// 适配器：配合 <see cref="SuppressCancellationThrow"/> 使用，
        /// 在取消时返回 (true, default) 而不是抛异常；正常完成时返回 (false, Result)。
        /// </summary>
        sealed class IsCanceledSource : IUniTaskSource<(bool, T)>
        {
            readonly IUniTaskSource<T> source;

            [DebuggerHidden]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public IsCanceledSource(IUniTaskSource<T> source)
            {
                this.source = source;
            }

            [DebuggerHidden]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public (bool, T) GetResult(short token)
            {
                if (source.GetStatus(token) == UniTaskStatus.Canceled)
                {
                    return (true, default);
                }

                var result = source.GetResult(token);
                return (false, result);
            }

            [DebuggerHidden]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            void IUniTaskSource.GetResult(short token)
            {
                GetResult(token);
            }

            [DebuggerHidden]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public UniTaskStatus GetStatus(short token)
            {
                return source.GetStatus(token);
            }

            [DebuggerHidden]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public UniTaskStatus UnsafeGetStatus()
            {
                return source.UnsafeGetStatus();
            }

            [DebuggerHidden]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void OnCompleted(Action<object> continuation, object state, short token)
            {
                source.OnCompleted(continuation, state, token);
            }
        }

        /// <summary>
        /// 结果缓存源：在首次获取结果时记录结果或异常并清空原始源引用，后续调用直接返回缓存状态。
        /// 用于支持多次 await 同一 UniTask{T} 而不重复执行异步逻辑。
        /// </summary>
        sealed class MemoizeSource : IUniTaskSource<T>
        {
            IUniTaskSource<T> source;
            T result;
            ExceptionDispatchInfo exception;
            UniTaskStatus status;

            public MemoizeSource(IUniTaskSource<T> source)
            {
                this.source = source;
            }

            public T GetResult(short token)
            {
                if (source == null)
                {
                    if (exception != null)
                    {
                        exception.Throw();
                    }
                    return result;
                }
                else
                {
                    try
                    {
                        result = source.GetResult(token);
                        status = UniTaskStatus.Succeeded;
                        return result;
                    }
                    catch (Exception ex)
                    {
                        exception = ExceptionDispatchInfo.Capture(ex);
                        if (ex is OperationCanceledException)
                        {
                            status = UniTaskStatus.Canceled;
                        }
                        else
                        {
                            status = UniTaskStatus.Faulted;
                        }
                        throw;
                    }
                    finally
                    {
                        source = null;
                    }
                }
            }

            void IUniTaskSource.GetResult(short token)
            {
                GetResult(token);
            }

            public UniTaskStatus GetStatus(short token)
            {
                if (source == null)
                {
                    return status;
                }

                return source.GetStatus(token);
            }

            public void OnCompleted(Action<object> continuation, object state, short token)
            {
                if (source == null)
                {
                    continuation(state);
                }
                else
                {
                    source.OnCompleted(continuation, state, token);
                }
            }

            public UniTaskStatus UnsafeGetStatus()
            {
                if (source == null)
                {
                    return status;
                }

                return source.UnsafeGetStatus();
            }
        }

        /// <summary>
        /// 与 UniTask{T} 搭配使用的 Awaiter，实现 await 协议。
        /// 持有一个只读任务实例引用，通过它访问状态、结果与回调注册。
        /// </summary>
        public readonly struct Awaiter : ICriticalNotifyCompletion
        {
            readonly UniTask<T> task;

            [DebuggerHidden]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Awaiter(in UniTask<T> task)
            {
                // 仅保存结构体副本，不会产生额外分配。
                this.task = task;
            }

            /// <summary>
            /// 指示任务是否已经到达终态（成功/失败/取消）。
            /// </summary>
            public bool IsCompleted
            {
                [DebuggerHidden]
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get
                {
                    return task.Status.IsCompleted();
                }
            }

            [DebuggerHidden]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public T GetResult()
            {
                var s = task.source;
                if (s == null)
                {
                    return task.result;
                }
                else
                {
                    return s.GetResult(task.token);
                }
            }

            [DebuggerHidden]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void OnCompleted(Action continuation)
            {
                var s = task.source;
                if (s == null)
                {
                    continuation();
                }
                else
                {
                    s.OnCompleted(AwaiterActions.InvokeContinuationDelegate, continuation, task.token);
                }
            }

            [DebuggerHidden]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void UnsafeOnCompleted(Action continuation)
            {
                var s = task.source;
                if (s == null)
                {
                    continuation();
                }
                else
                {
                    s.OnCompleted(AwaiterActions.InvokeContinuationDelegate, continuation, task.token);
                }
            }

            /// <summary>
            /// 为底层 IUniTaskSource 手动注册 continuation 的辅助方法，可在高级用法中直接控制回调与状态对象。
            /// </summary>
            [DebuggerHidden]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void SourceOnCompleted(Action<object> continuation, object state)
            {
                var s = task.source;
                if (s == null)
                {
                    continuation(state);
                }
                else
                {
                    s.OnCompleted(continuation, state, task.token);
                }
            }
        }
    }
}

