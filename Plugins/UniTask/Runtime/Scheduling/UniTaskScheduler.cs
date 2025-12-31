#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System;
using System.Threading;

namespace Cysharp.Threading.Tasks
{
    /// <summary>
    /// UniTask 没有像 TaskScheduler 那样的调度器。
    /// </summary>
    /// <remarks>
    /// 只处理未观察到的异常
    /// </remarks>
    public static class UniTaskScheduler
    {
        /// <summary>
        /// 未观察到的异常事件
        /// </summary>
        public static event Action<Exception> UnobservedTaskException;

        /// <summary>
        /// 传播 OperationCanceledException 到 UnobservedTaskException 时为 true。默认是 false。
        /// </summary>
        public static bool PropagateOperationCanceledException = false;

#if UNITY_2018_3_OR_NEWER

        /// <summary>
        /// 捕获未观察到的异常并且未注册 UnobservedTaskException 时写入日志的类型。默认是 Exception。
        /// </summary>
        public static UnityEngine.LogType UnobservedExceptionWriteLogType = UnityEngine.LogType.Exception;

        /// <summary>
        /// 将异常事件分派到 Unity 主线程。默认是 true。
        /// </summary>
        public static bool DispatchUnityMainThread = true;
        
        /// <summary>
        /// 缓存委托。
        /// </summary>
        static readonly SendOrPostCallback handleExceptionInvoke = InvokeUnobservedTaskException;

        /// <summary>
        /// 调用未观察到的异常事件
        /// </summary>
        /// <param name="state">状态</param>
        static void InvokeUnobservedTaskException(object state)
        {
            UnobservedTaskException((Exception)state);
        }
#endif

        /// <summary>
        /// 发布未观察到的异常
        /// </summary>
        /// <param name="ex">异常</param>
        internal static void PublishUnobservedTaskException(Exception ex)
        {
            if (ex != null)
            {
                if (!PropagateOperationCanceledException && ex is OperationCanceledException)
                {
                    return;
                }

                if (UnobservedTaskException != null)
                {
#if UNITY_2018_3_OR_NEWER
                    if (!DispatchUnityMainThread || Thread.CurrentThread.ManagedThreadId == PlayerLoopHelper.MainThreadId)
                    {
                        // 允许内联调用。
                        UnobservedTaskException.Invoke(ex);
                    }
                    else
                    {
                        // 发送到主线程。
                        PlayerLoopHelper.UnitySynchronizationContext.Post(handleExceptionInvoke, ex);
                    }
#else
                    UnobservedTaskException.Invoke(ex);
#endif
                }
                else
                {
#if UNITY_2018_3_OR_NEWER
                    string msg = null;
                    if (UnobservedExceptionWriteLogType != UnityEngine.LogType.Exception)
                    {
                        msg = "未观察到的异常: " + ex.ToString();
                    }
                    switch (UnobservedExceptionWriteLogType)
                    {
                        case UnityEngine.LogType.Error:
                            UnityEngine.Debug.LogError(msg);
                            break;
                        case UnityEngine.LogType.Assert:
                            UnityEngine.Debug.LogAssertion(msg);
                            break;
                        case UnityEngine.LogType.Warning:
                            UnityEngine.Debug.LogWarning(msg);
                            break;
                        case UnityEngine.LogType.Log:
                            UnityEngine.Debug.Log(msg);
                            break;
                        case UnityEngine.LogType.Exception:
                            UnityEngine.Debug.LogException(ex);
                            break;
                        default:
                            break;
                    }
#else
                    Console.WriteLine("未观察到的异常: " + ex.ToString());
#endif
                }
            }
        }
    }
}

