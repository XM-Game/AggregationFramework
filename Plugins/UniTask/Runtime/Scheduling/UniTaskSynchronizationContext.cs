using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace Cysharp.Threading.Tasks
{
    /// <summary>
    /// UniTaskSynchronizationContext 类
    /// </summary>
    /// <remarks>
    /// 继承自 SynchronizationContext，用于在 Unity 主线程中执行任务
    /// </remarks>
    public class UniTaskSynchronizationContext : SynchronizationContext
    {
        /// <summary>
        /// 最大数组长度
        /// </summary>
        const int MaxArrayLength = 0X7FEFFFFF;
        const int InitialSize = 16;

        /// <summary>
        /// 锁
        /// </summary>
        static SpinLock gate = new SpinLock(false);
        static bool dequing = false;

        /// <summary>
        /// 动作列表计数
        /// </summary>
        static int actionListCount = 0;
        static Callback[] actionList = new Callback[InitialSize];

        /// <summary>
        /// 等待列表计数
        /// </summary>
        static int waitingListCount = 0;
        static Callback[] waitingList = new Callback[InitialSize];

        /// <summary>
        /// 操作计数
        /// </summary>
        static int opCount;

        /// <summary>
        /// 发送
        /// </summary>
        /// <param name="d">委托</param>
        /// <param name="state">状态</param>
        public override void Send(SendOrPostCallback d, object state)
        {
            d(state);
        }

        /// <summary>
        /// 发布
        /// </summary>
        /// <param name="d">委托</param>
        /// <param name="state">状态</param>
        public override void Post(SendOrPostCallback d, object state)
        {
            bool lockTaken = false;
            try
            {
                gate.Enter(ref lockTaken);

                if (dequing)
                {
                    // Ensure Capacity
                    if (waitingList.Length == waitingListCount)
                    {
                        var newLength = waitingListCount * 2;
                        if ((uint)newLength > MaxArrayLength) newLength = MaxArrayLength;

                        var newArray = new Callback[newLength];
                        Array.Copy(waitingList, newArray, waitingListCount);
                        waitingList = newArray;
                    }
                    waitingList[waitingListCount] = new Callback(d, state);
                    waitingListCount++;
                }
                else
                {
                    // Ensure Capacity
                    if (actionList.Length == actionListCount)
                    {
                        var newLength = actionListCount * 2;
                        if ((uint)newLength > MaxArrayLength) newLength = MaxArrayLength;

                        var newArray = new Callback[newLength];
                        Array.Copy(actionList, newArray, actionListCount);
                        actionList = newArray;
                    }
                    actionList[actionListCount] = new Callback(d, state);
                    actionListCount++;
                }
            }
            finally
            {
                if (lockTaken) gate.Exit(false);
            }
        }

        /// <summary>
        /// 操作开始
        /// </summary>
        public override void OperationStarted()
        {
            Interlocked.Increment(ref opCount);
        }

        /// <summary>
        /// 操作完成
        /// </summary>
        public override void OperationCompleted()
        {
            Interlocked.Decrement(ref opCount);
        }

        /// <summary>
        /// 创建副本
        /// </summary>
        /// <returns>SynchronizationContext</returns>
        public override SynchronizationContext CreateCopy()
        {
            return this;
        }

        /// <summary>
        /// 运行
        /// </summary>
        internal static void Run()
        {
            {
                bool lockTaken = false;
                try
                {
                    gate.Enter(ref lockTaken);
                    if (actionListCount == 0) return;
                    dequing = true;
                }
                finally
                {
                    if (lockTaken) gate.Exit(false);
                }
            }

            for (int i = 0; i < actionListCount; i++)
            {
                var action = actionList[i];
                actionList[i] = default;
                action.Invoke();
            }

            {
                bool lockTaken = false;
                try
                {
                    gate.Enter(ref lockTaken);
                    dequing = false;

                    var swapTempActionList = actionList;

                    actionListCount = waitingListCount;
                    actionList = waitingList;

                    waitingListCount = 0;
                    waitingList = swapTempActionList;
                }
                finally
                {
                    if (lockTaken) gate.Exit(false);
                }
            }
        }

        /// <summary>
        /// Callback 结构体
        /// </summary>
        /// <remarks>
        /// 用于存储委托和状态
        /// </remarks>
        [StructLayout(LayoutKind.Auto)]
        readonly struct Callback
        {
            readonly SendOrPostCallback callback;
            readonly object state;

            /// <summary>
            /// Callback 构造函数
            /// </summary>
            /// <param name="callback">委托</param>
            /// <param name="state">状态</param>
            public Callback(SendOrPostCallback callback, object state)
            {
                this.callback = callback;
                this.state = state;
            }

            /// <summary>
            /// 调用
            /// </summary>
            public void Invoke()
            {
                try
                {
                    callback(state);
                }
                catch (Exception ex)
                {
                    UnityEngine.Debug.LogException(ex);
                }
            }
        }
    }
}