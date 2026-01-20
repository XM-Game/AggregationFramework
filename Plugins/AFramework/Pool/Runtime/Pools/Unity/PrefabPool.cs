// ==========================================================
// 文件名：PrefabPool.cs
// 命名空间: AFramework.Pool
// 依赖: UnityEngine, AFramework.Pool
// 功能: Prefab 实例池，支持多种实例化策略
// ==========================================================

using System;
using UnityEngine;

namespace AFramework.Pool
{
    /// <summary>
    /// Prefab 实例池
    /// Prefab Instance Pool
    /// </summary>
    /// <remarks>
    /// 专为 Prefab 实例化设计的对象池
    /// Designed specifically for Prefab instantiation
    /// </remarks>
    public class PrefabPool : GameObjectPool
    {
        #region 字段 Fields

        private readonly string _poolName;
        private int _instanceCounter;

        #endregion

        #region 属性 Properties

        /// <summary>
        /// 池名称
        /// Pool name
        /// </summary>
        public string PoolName => _poolName;

        /// <summary>
        /// 实例计数器
        /// Instance counter
        /// </summary>
        public int InstanceCounter => _instanceCounter;

        #endregion

        #region 构造函数 Constructors

        /// <summary>
        /// 创建 Prefab 实例池
        /// Create Prefab instance pool
        /// </summary>
        public PrefabPool(
            GameObject prefab,
            string poolName = null,
            Transform parent = null,
            int initialCapacity = 10,
            int maxCapacity = 100,
            bool worldPositionStays = false)
            : base(prefab, parent, initialCapacity, maxCapacity, worldPositionStays)
        {
            _poolName = string.IsNullOrEmpty(poolName) ? prefab.name : poolName;
            _instanceCounter = 0;
        }

        #endregion

        #region 虚方法重写 Virtual Method Overrides

        protected override GameObject CreateNew()
        {
            var obj = base.CreateNew();
            obj.name = $"{_poolName}_{_instanceCounter++}";
            return obj;
        }

        #endregion
    }
}
