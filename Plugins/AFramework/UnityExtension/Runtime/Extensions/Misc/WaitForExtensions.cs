// ==========================================================
// 文件名：WaitForExtensions.cs
// 命名空间: AFramework.UnityExtension
// 依赖: UnityEngine, System
// ==========================================================

using System;
using System.Collections;
using UnityEngine;

namespace AFramework.UnityExtension
{
    /// <summary>
    /// 等待指令工厂类
    /// <para>提供常用等待指令的缓存和创建方法</para>
    /// </summary>
    public static class WaitFor
    {
        #region 缓存的等待指令

        /// <summary>
        /// 等待一帧 (EndOfFrame)
        /// </summary>
        public static readonly WaitForEndOfFrame EndOfFrame = new WaitForEndOfFrame();

        /// <summary>
        /// 等待固定更新
        /// </summary>
        public static readonly WaitForFixedUpdate FixedUpdate = new WaitForFixedUpdate();

        /// <summary>
        /// 等待 0.1 秒
        /// </summary>
        public static readonly WaitForSeconds Seconds01 = new WaitForSeconds(0.1f);

        /// <summary>
        /// 等待 0.25 秒
        /// </summary>
        public static readonly WaitForSeconds Seconds025 = new WaitForSeconds(0.25f);

        /// <summary>
        /// 等待 0.5 秒
        /// </summary>
        public static readonly WaitForSeconds Seconds05 = new WaitForSeconds(0.5f);

        /// <summary>
        /// 等待 1 秒
        /// </summary>
        public static readonly WaitForSeconds Seconds1 = new WaitForSeconds(1f);

        /// <summary>
        /// 等待 2 秒
        /// </summary>
        public static readonly WaitForSeconds Seconds2 = new WaitForSeconds(2f);

        /// <summary>
        /// 等待 5 秒
        /// </summary>
        public static readonly WaitForSeconds Seconds5 = new WaitForSeconds(5f);

        #endregion

        #region 创建方法

        /// <summary>
        /// 创建等待指定秒数的指令
        /// </summary>
        public static WaitForSeconds Seconds(float seconds)
        {
            return new WaitForSeconds(seconds);
        }

        /// <summary>
        /// 创建等待指定秒数的指令 (不受 Time.timeScale 影响)
        /// </summary>
        public static WaitForSecondsRealtime SecondsRealtime(float seconds)
        {
            return new WaitForSecondsRealtime(seconds);
        }

        /// <summary>
        /// 创建等待条件满足的指令
        /// </summary>
        public static WaitUntil Until(Func<bool> predicate)
        {
            return new WaitUntil(predicate);
        }

        /// <summary>
        /// 创建等待条件不满足的指令
        /// </summary>
        public static WaitWhile While(Func<bool> predicate)
        {
            return new WaitWhile(predicate);
        }

        #endregion

        #region 帧等待

        /// <summary>
        /// 等待指定帧数
        /// </summary>
        public static IEnumerator Frames(int frameCount)
        {
            for (int i = 0; i < frameCount; i++)
            {
                yield return null;
            }
        }

        /// <summary>
        /// 等待下一帧
        /// </summary>
        public static IEnumerator NextFrame()
        {
            yield return null;
        }

        #endregion

        #region 条件等待

        /// <summary>
        /// 等待直到对象不为空
        /// </summary>
        public static WaitUntil UntilNotNull<T>(Func<T> getter) where T : class
        {
            return new WaitUntil(() => getter() != null);
        }

        /// <summary>
        /// 等待直到对象为空
        /// </summary>
        public static WaitUntil UntilNull<T>(Func<T> getter) where T : class
        {
            return new WaitUntil(() => getter() == null);
        }

        /// <summary>
        /// 等待直到值为真
        /// </summary>
        public static WaitUntil UntilTrue(Func<bool> predicate)
        {
            return new WaitUntil(predicate);
        }

        /// <summary>
        /// 等待直到值为假
        /// </summary>
        public static WaitUntil UntilFalse(Func<bool> predicate)
        {
            return new WaitUntil(() => !predicate());
        }

        #endregion

        #region 异步操作等待

        /// <summary>
        /// 等待异步操作完成
        /// </summary>
        public static IEnumerator AsyncOperation(AsyncOperation operation)
        {
            while (!operation.isDone)
            {
                yield return null;
            }
        }

        /// <summary>
        /// 等待异步操作完成 (带进度回调)
        /// </summary>
        public static IEnumerator AsyncOperation(AsyncOperation operation, Action<float> onProgress)
        {
            while (!operation.isDone)
            {
                onProgress?.Invoke(operation.progress);
                yield return null;
            }
            onProgress?.Invoke(1f);
        }

        #endregion

        #region 组合等待

        /// <summary>
        /// 等待所有条件满足
        /// </summary>
        public static WaitUntil All(params Func<bool>[] predicates)
        {
            return new WaitUntil(() =>
            {
                foreach (var predicate in predicates)
                {
                    if (!predicate()) return false;
                }
                return true;
            });
        }

        /// <summary>
        /// 等待任一条件满足
        /// </summary>
        public static WaitUntil Any(params Func<bool>[] predicates)
        {
            return new WaitUntil(() =>
            {
                foreach (var predicate in predicates)
                {
                    if (predicate()) return true;
                }
                return false;
            });
        }

        #endregion

        #region 超时等待

        /// <summary>
        /// 等待条件满足或超时
        /// </summary>
        public static IEnumerator UntilOrTimeout(Func<bool> predicate, float timeout)
        {
            float elapsed = 0f;
            while (!predicate() && elapsed < timeout)
            {
                elapsed += Time.deltaTime;
                yield return null;
            }
        }

        /// <summary>
        /// 等待条件满足或超时 (带结果回调)
        /// </summary>
        public static IEnumerator UntilOrTimeout(Func<bool> predicate, float timeout, Action<bool> onComplete)
        {
            float elapsed = 0f;
            while (!predicate() && elapsed < timeout)
            {
                elapsed += Time.deltaTime;
                yield return null;
            }
            onComplete?.Invoke(predicate());
        }

        #endregion
    }

    /// <summary>
    /// 自定义等待指令：等待指定帧数
    /// </summary>
    public class WaitForFrames : CustomYieldInstruction
    {
        private int _remainingFrames;

        public WaitForFrames(int frameCount)
        {
            _remainingFrames = frameCount;
        }

        public override bool keepWaiting
        {
            get
            {
                _remainingFrames--;
                return _remainingFrames > 0;
            }
        }
    }

    /// <summary>
    /// 自定义等待指令：等待条件满足或超时
    /// </summary>
    public class WaitUntilOrTimeout : CustomYieldInstruction
    {
        private readonly Func<bool> _predicate;
        private readonly float _timeout;
        private float _elapsed;

        /// <summary>
        /// 是否因超时而结束
        /// </summary>
        public bool TimedOut { get; private set; }

        public WaitUntilOrTimeout(Func<bool> predicate, float timeout)
        {
            _predicate = predicate;
            _timeout = timeout;
            _elapsed = 0f;
            TimedOut = false;
        }

        public override bool keepWaiting
        {
            get
            {
                if (_predicate())
                {
                    TimedOut = false;
                    return false;
                }

                _elapsed += Time.deltaTime;
                if (_elapsed >= _timeout)
                {
                    TimedOut = true;
                    return false;
                }

                return true;
            }
        }
    }

    /// <summary>
    /// 自定义等待指令：等待动画完成
    /// </summary>
    public class WaitForAnimation : CustomYieldInstruction
    {
        private readonly Animator _animator;
        private readonly int _layerIndex;
        private readonly int _stateHash;

        public WaitForAnimation(Animator animator, string stateName, int layerIndex = 0)
        {
            _animator = animator;
            _layerIndex = layerIndex;
            _stateHash = Animator.StringToHash(stateName);
        }

        public override bool keepWaiting
        {
            get
            {
                if (_animator == null) return false;

                var stateInfo = _animator.GetCurrentAnimatorStateInfo(_layerIndex);
                return stateInfo.shortNameHash == _stateHash && stateInfo.normalizedTime < 1f;
            }
        }
    }

    /// <summary>
    /// 自定义等待指令：等待音频播放完成
    /// </summary>
    public class WaitForAudio : CustomYieldInstruction
    {
        private readonly AudioSource _audioSource;

        public WaitForAudio(AudioSource audioSource)
        {
            _audioSource = audioSource;
        }

        public override bool keepWaiting
        {
            get
            {
                return _audioSource != null && _audioSource.isPlaying;
            }
        }
    }
}
