#if ENABLE_MANAGED_JOBS
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System;
using System.Threading;
using Unity.Jobs;
using UnityEngine;

namespace Cysharp.Threading.Tasks
{
    /// <summary>
    /// UnityAsyncExtensions 类
    /// </summary>
    /// <remarks>
    /// 提供 WaitAsync 方法，用于等待 JobHandle 完成
    /// </remarks>
    public static partial class UnityAsyncExtensions
    {
        /// <summary>
        /// 等待 JobHandle 完成
        /// </summary>
        /// <param name="jobHandle">JobHandle</param>
        /// <param name="waitTiming">等待时机</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>UniTask</returns>
        public static async UniTask WaitAsync(this JobHandle jobHandle, PlayerLoopTiming waitTiming, CancellationToken cancellationToken = default)
        {
            await UniTask.Yield(waitTiming);
            jobHandle.Complete();
            cancellationToken.ThrowIfCancellationRequested(); // call cancel after Complete.
        }

        /// <summary>
        /// 获取 JobHandle 的异步等待器
        /// </summary>
        /// <param name="jobHandle">JobHandle</param>
        /// <returns>UniTask.Awaiter</returns>
        public static UniTask.Awaiter GetAwaiter(this JobHandle jobHandle)
        {
            var handler = JobHandlePromise.Create(jobHandle, out var token);
            {
                PlayerLoopHelper.AddAction(PlayerLoopTiming.EarlyUpdate, handler);
                PlayerLoopHelper.AddAction(PlayerLoopTiming.PreUpdate, handler);
                PlayerLoopHelper.AddAction(PlayerLoopTiming.Update, handler);
                PlayerLoopHelper.AddAction(PlayerLoopTiming.PreLateUpdate, handler);
                PlayerLoopHelper.AddAction(PlayerLoopTiming.PostLateUpdate, handler);
            }

            return new UniTask(handler, token).GetAwaiter();
        }

        // can not pass CancellationToken because can't handle JobHandle's Complete and NativeArray.Dispose.
        /// <summary>
        /// 将 JobHandle 转换为 UniTask
        /// </summary>
        /// <param name="jobHandle">JobHandle</param>
        /// <param name="waitTiming">等待时机</param>
        /// <returns>UniTask</returns>
        public static UniTask ToUniTask(this JobHandle jobHandle, PlayerLoopTiming waitTiming)
        {
            var handler = JobHandlePromise.Create(jobHandle, out var token);
            {
                PlayerLoopHelper.AddAction(waitTiming, handler);
            }

            return new UniTask(handler, token);
        }

        /// <summary>
        /// JobHandlePromise 类
        /// </summary>
        /// <remarks>
        /// 实现 IUniTaskSource 和 IPlayerLoopItem 接口
        /// </remarks>
        sealed class JobHandlePromise : IUniTaskSource, IPlayerLoopItem
        {
            JobHandle jobHandle; // JobHandle

            UniTaskCompletionSourceCore<AsyncUnit> core; // UniTaskCompletionSourceCore<AsyncUnit>

            // Cancellation is not supported.
            /// <summary>
            /// 创建 JobHandlePromise
            /// </summary>
            /// <param name="jobHandle">JobHandle</param>
            /// <param name="token">令牌</param>
            /// <returns>JobHandlePromise</returns>
            public static JobHandlePromise Create(JobHandle jobHandle, out short token)
            {
                // not use pool.
                var result = new JobHandlePromise();

                result.jobHandle = jobHandle;

                TaskTracker.TrackActiveTask(result, 3);

                token = result.core.Version;
                return result;
            }

            /// <summary>
            /// 获取结果
            /// </summary>
            /// <param name="token">令牌</param>
            public void GetResult(short token)
            {
                TaskTracker.RemoveTracking(this);
                core.GetResult(token);
            }

            /// <summary>
            /// 获取状态
            /// </summary>
            /// <param name="token">令牌</param>
            /// <returns>UniTaskStatus</returns>
            public UniTaskStatus GetStatus(short token)
            {
                return core.GetStatus(token);
            }

            /// <summary>
            /// 获取状态
            /// </summary>
            /// <returns>UniTaskStatus</returns>
            public UniTaskStatus UnsafeGetStatus()
            {
                return core.UnsafeGetStatus();
            }

            /// <summary>
            /// 完成
            /// </summary>
            /// <param name="continuation">延续</param>
            /// <param name="state">状态</param>
            /// <param name="token">令牌</param>
            public void OnCompleted(Action<object> continuation, object state, short token)
            {
                core.OnCompleted(continuation, state, token);
            }

            /// <summary>
            /// 移动到下一个
            /// </summary>
            /// <returns>bool</returns>
            public bool MoveNext()
            {
                if (jobHandle.IsCompleted | PlayerLoopHelper.IsEditorApplicationQuitting)
                {
                    jobHandle.Complete();
                    core.TrySetResult(AsyncUnit.Default);
                    return false;
                }

                return true;
            }
        }
    }
}

#endif