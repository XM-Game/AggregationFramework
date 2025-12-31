using System;
using System.Threading;

namespace Cysharp.Threading.Tasks
{
    /// <summary>
    /// UnityAsyncExtensions 类
    /// </summary>
    /// <remarks>
    /// 提供 StartAsyncCoroutine 方法，用于启动异步协程
    /// </remarks>
    public static partial class UnityAsyncExtensions
    {
        /// <summary>
        /// 启动异步协程
        /// </summary>
        /// <param name="monoBehaviour">MonoBehaviour</param>
        /// <param name="asyncCoroutine">异步协程</param>
        /// <returns>UniTask</returns>
        public static UniTask StartAsyncCoroutine(this UnityEngine.MonoBehaviour monoBehaviour, Func<CancellationToken, UniTask> asyncCoroutine)
        {
            var token = monoBehaviour.GetCancellationTokenOnDestroy();
            return asyncCoroutine(token);
        }
    }
}