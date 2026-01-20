// ==========================================================
// 文件名：UnityDestroyPolicy.cs
// 命名空间: AFramework.Pool
// 依赖: UnityEngine, AFramework.Pool
// 功能: Unity Destroy 清理策略
// ==========================================================

using UnityEngine;


namespace AFramework.Pool
{
    /// <summary>
    /// Unity Destroy 清理策略
    /// Unity Destroy Cleanup Policy
    /// </summary>
    /// <typeparam name="T">组件类型 Component type</typeparam>
    /// <remarks>
    /// 使用 Unity Object.Destroy 销毁对象
    /// Uses Unity Object.Destroy to destroy objects
    /// </remarks>
    public class UnityDestroyPolicy<T> : PoolPolicyBase<T>, IPoolCleanupPolicy<T> where T : Component
    {
        #region 字段 Fields

        private readonly bool _destroyImmediate;

        #endregion

        #region 属性 Properties

        /// <summary>
        /// 是否立即销毁
        /// Whether to destroy immediately
        /// </summary>
        public bool DestroyImmediate => _destroyImmediate;

        #endregion

        #region 构造函数 Constructors

        /// <summary>
        /// 创建 Unity Destroy 策略
        /// Create Unity Destroy policy
        /// </summary>
        /// <param name="destroyImmediate">是否立即销毁 Whether to destroy immediately</param>
        public UnityDestroyPolicy(bool destroyImmediate = false)
        {
            _destroyImmediate = destroyImmediate;
        }

        #endregion

        #region IPoolCleanupPolicy 实现

        public void OnReturn(T obj)
        {
            if (obj == null) return;

            // 停用对象 Deactivate object
            obj.gameObject.SetActive(false);
        }

        public void OnDestroy(T obj)
        {
            if (obj == null) return;

            if (_destroyImmediate)
            {
                UnityEngine.Object.DestroyImmediate(obj.gameObject);
            }
            else
            {
                UnityEngine.Object.Destroy(obj.gameObject);
            }
        }

        public bool Validate()
        {
            return true;
        }

        #endregion
    }

    /// <summary>
    /// GameObject Destroy 清理策略
    /// GameObject Destroy Cleanup Policy
    /// </summary>
    public class GameObjectDestroyPolicy : PoolPolicyBase<GameObject>, IPoolCleanupPolicy<GameObject>
    {
        private readonly bool _destroyImmediate;

        public bool DestroyImmediate => _destroyImmediate;

        public GameObjectDestroyPolicy(bool destroyImmediate = false)
        {
            _destroyImmediate = destroyImmediate;
        }

        public void OnReturn(GameObject obj)
        {
            if (obj == null) return;
            obj.SetActive(false);
        }

        public void OnDestroy(GameObject obj)
        {
            if (obj == null) return;

            if (_destroyImmediate)
            {
                UnityEngine.Object.DestroyImmediate(obj);
            }
            else
            {
                UnityEngine.Object.Destroy(obj);
            }
        }

        public bool Validate()
        {
            return true;
        }
    }
}
