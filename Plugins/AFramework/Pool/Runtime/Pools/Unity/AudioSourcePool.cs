// ==========================================================
// 文件名：AudioSourcePool.cs
// 命名空间: AFramework.Pool
// 依赖: UnityEngine, AFramework.Pool
// 功能: AudioSource 对象池，支持音频播放管理
// ==========================================================

using System;
using System.Collections;
using UnityEngine;

namespace AFramework.Pool
{
    /// <summary>
    /// AudioSource 对象池
    /// AudioSource Object Pool
    /// </summary>
    public class AudioSourcePool : ComponentPool<AudioSource>
    {
        private readonly MonoBehaviour _coroutineRunner;
        private readonly bool _autoRecycle;

        public bool AutoRecycle => _autoRecycle;

        public AudioSourcePool(
            AudioSource prefab,
            MonoBehaviour coroutineRunner,
            Transform parent = null,
            int initialCapacity = 10,
            int maxCapacity = 50,
            bool autoRecycle = true)
            : base(prefab, parent, initialCapacity, maxCapacity)
        {
            _coroutineRunner = coroutineRunner ?? throw new ArgumentNullException(nameof(coroutineRunner));
            _autoRecycle = autoRecycle;
        }

        /// <summary>
        /// 播放音频
        /// Play audio
        /// </summary>
        public AudioSource Play(AudioClip clip, Vector3 position, float volume = 1f)
        {
            var audioSource = Get();
            audioSource.transform.position = position;
            audioSource.clip = clip;
            audioSource.volume = volume;
            audioSource.Play();

            if (_autoRecycle)
            {
                _coroutineRunner.StartCoroutine(AutoRecycleCoroutine(audioSource, clip.length));
            }

            return audioSource;
        }

        protected override void OnReturnToPool(AudioSource audioSource)
        {
            base.OnReturnToPool(audioSource);
            audioSource.Stop();
            audioSource.clip = null;
        }

        private IEnumerator AutoRecycleCoroutine(AudioSource audioSource, float duration)
        {
            yield return new WaitForSeconds(duration);

            if (audioSource != null && !audioSource.isPlaying)
            {
                Return(audioSource);
            }
        }
    }
}
