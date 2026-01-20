// ==========================================================
// 文件名：ParticleSystemPool.cs
// 命名空间: AFramework.Pool
// 依赖: UnityEngine, AFramework.Pool
// 功能: 粒子系统对象池，支持自动回收和生命周期管理
// ==========================================================

using System;
using System.Collections;
using UnityEngine;

namespace AFramework.Pool
{
    /// <summary>
    /// 粒子系统对象池
    /// Particle System Object Pool
    /// </summary>
    /// <remarks>
    /// 专为 Unity 粒子系统设计的对象池，支持：
    /// - 自动回收（播放完成后）
    /// - 生命周期管理
    /// - 停止行为配置
    /// - 性能优化
    /// Designed specifically for Unity particle systems, supports:
    /// - Automatic recycling (after playback completion)
    /// - Lifecycle management
    /// - Stop behavior configuration
    /// - Performance optimization
    /// </remarks>
    public class ParticleSystemPool : ComponentPool<ParticleSystem>
    {
        #region 字段 Fields

        private readonly bool _autoRecycle;
        private readonly ParticleSystemStopBehavior _stopBehavior;
        private readonly MonoBehaviour _coroutineRunner;

        #endregion

        #region 属性 Properties

        /// <summary>
        /// 是否自动回收
        /// Whether to auto recycle
        /// </summary>
        public bool AutoRecycle => _autoRecycle;

        /// <summary>
        /// 停止行为
        /// Stop behavior
        /// </summary>
        public ParticleSystemStopBehavior StopBehavior => _stopBehavior;

        #endregion

        #region 构造函数 Constructors

        /// <summary>
        /// 创建粒子系统对象池
        /// Create particle system object pool
        /// </summary>
        /// <param name="prefab">粒子系统预制体 Particle system prefab</param>
        /// <param name="coroutineRunner">协程运行器 Coroutine runner</param>
        /// <param name="parent">父级变换（可选）Parent transform (optional)</param>
        /// <param name="initialCapacity">初始容量 Initial capacity</param>
        /// <param name="maxCapacity">最大容量 Maximum capacity</param>
        /// <param name="autoRecycle">是否自动回收 Whether to auto recycle</param>
        /// <param name="stopBehavior">停止行为 Stop behavior</param>
        public ParticleSystemPool(
            ParticleSystem prefab,
            MonoBehaviour coroutineRunner,
            Transform parent = null,
            int initialCapacity = 10,
            int maxCapacity = 50,
            bool autoRecycle = true,
            ParticleSystemStopBehavior stopBehavior = ParticleSystemStopBehavior.StopEmittingAndClear)
            : base(prefab, parent, initialCapacity, maxCapacity)
        {
            _coroutineRunner = coroutineRunner ?? throw new ArgumentNullException(nameof(coroutineRunner));
            _autoRecycle = autoRecycle;
            _stopBehavior = stopBehavior;
        }

        #endregion

        #region 核心方法 Core Methods

        /// <summary>
        /// 播放粒子效果
        /// Play particle effect
        /// </summary>
        /// <param name="position">位置 Position</param>
        /// <param name="rotation">旋转 Rotation</param>
        /// <param name="parent">父级变换（可选）Parent transform (optional)</param>
        /// <returns>粒子系统实例 Particle system instance</returns>
        public ParticleSystem Play(Vector3 position, Quaternion rotation, Transform parent = null)
        {
            var ps = Get();
            var transform = ps.transform;
            transform.SetParent(parent);
            transform.position = position;
            transform.rotation = rotation;

            ps.Play(true);

            if (_autoRecycle)
            {
                _coroutineRunner.StartCoroutine(AutoRecycleCoroutine(ps));
            }

            return ps;
        }

        /// <summary>
        /// 播放粒子效果（重载）
        /// Play particle effect (overload)
        /// </summary>
        public ParticleSystem Play(Vector3 position)
        {
            return Play(position, Quaternion.identity, null);
        }

        /// <summary>
        /// 停止并归还粒子系统
        /// Stop and return particle system
        /// </summary>
        public void StopAndReturn(ParticleSystem ps)
        {
            if (ps == null) return;

            ps.Stop(true, _stopBehavior);
            Return(ps);
        }

        #endregion

        #region 虚方法重写 Virtual Method Overrides

        protected override void OnGetFromPool(ParticleSystem ps)
        {
            base.OnGetFromPool(ps);
            ps.Clear(true);
        }

        protected override void OnReturnToPool(ParticleSystem ps)
        {
            base.OnReturnToPool(ps);
            ps.Stop(true, _stopBehavior);
            ps.Clear(true);
        }

        #endregion

        #region 协程 Coroutines

        /// <summary>
        /// 自动回收协程
        /// Auto recycle coroutine
        /// </summary>
        private IEnumerator AutoRecycleCoroutine(ParticleSystem ps)
        {
            // 等待粒子系统播放完成 Wait for particle system to finish playing
            while (ps != null && ps.isPlaying)
            {
                yield return null;
            }

            // 归还到池 Return to pool
            if (ps != null)
            {
                Return(ps);
            }
        }

        #endregion
    }
}
