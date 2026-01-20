// ==========================================================
// 文件名：GameObjectPoolExtensions.cs
// 命名空间: AFramework.Pool.Extensions
// 依赖: UnityEngine, AFramework.Pool
// 功能: GameObject 对象池扩展方法
// ==========================================================

using System;
using UnityEngine;

namespace AFramework.Pool.Extensions
{
    /// <summary>
    /// GameObject 对象池扩展方法
    /// GameObject Pool Extension Methods
    /// </summary>
    /// <remarks>
    /// 提供便捷的 GameObject 池操作扩展方法，包括：
    /// - 获取/归还扩展
    /// - 激活/停用扩展
    /// - Transform 操作扩展
    /// - 父对象设置扩展
    /// Provides convenient extension methods for GameObject pool operations, including:
    /// - Get/Return extensions
    /// - Activation/Deactivation extensions
    /// - Transform operation extensions
    /// - Parent setting extensions
    /// </remarks>
    public static class GameObjectPoolExtensions
    {
        #region 获取与归还扩展 Get & Return Extensions

        /// <summary>
        /// 获取 GameObject 并设置位置
        /// Get GameObject and set position
        /// </summary>
        /// <param name="pool">GameObject 池 / GameObject pool</param>
        /// <param name="position">世界坐标位置 / World position</param>
        /// <returns>GameObject 实例 / GameObject instance</returns>
        public static GameObject Get(this GameObjectPool pool, Vector3 position)
        {
            if (pool == null)
                throw new ArgumentNullException(nameof(pool));

            var obj = pool.GetGameObject();
            obj.transform.position = position;
            return obj;
        }

        /// <summary>
        /// 获取 GameObject 并设置位置和旋转
        /// Get GameObject and set position and rotation
        /// </summary>
        /// <param name="pool">GameObject 池 / GameObject pool</param>
        /// <param name="position">世界坐标位置 / World position</param>
        /// <param name="rotation">世界坐标旋转 / World rotation</param>
        /// <returns>GameObject 实例 / GameObject instance</returns>
        public static GameObject Get(this GameObjectPool pool, Vector3 position, Quaternion rotation)
        {
            if (pool == null)
                throw new ArgumentNullException(nameof(pool));

            var obj = pool.GetGameObject();
            var transform = obj.transform;
            transform.position = position;
            transform.rotation = rotation;
            return obj;
        }

        /// <summary>
        /// 获取 GameObject 并设置父对象
        /// Get GameObject and set parent
        /// </summary>
        /// <param name="pool">GameObject 池 / GameObject pool</param>
        /// <param name="parent">父级 Transform / Parent transform</param>
        /// <param name="worldPositionStays">是否保持世界坐标 / Whether to keep world position</param>
        /// <returns>GameObject 实例 / GameObject instance</returns>
        public static GameObject Get(this GameObjectPool pool, Transform parent, bool worldPositionStays = false)
        {
            if (pool == null)
                throw new ArgumentNullException(nameof(pool));

            var obj = pool.GetGameObject();
            obj.transform.SetParent(parent, worldPositionStays);
            return obj;
        }

        /// <summary>
        /// 获取 GameObject 并设置完整 Transform 信息
        /// Get GameObject and set complete Transform information
        /// </summary>
        /// <param name="pool">GameObject 池 / GameObject pool</param>
        /// <param name="position">世界坐标位置 / World position</param>
        /// <param name="rotation">世界坐标旋转 / World rotation</param>
        /// <param name="parent">父级 Transform / Parent transform</param>
        /// <param name="worldPositionStays">是否保持世界坐标 / Whether to keep world position</param>
        /// <returns>GameObject 实例 / GameObject instance</returns>
        public static GameObject Get(
            this GameObjectPool pool,
            Vector3 position,
            Quaternion rotation,
            Transform parent,
            bool worldPositionStays = false)
        {
            if (pool == null)
                throw new ArgumentNullException(nameof(pool));

            var obj = pool.GetGameObject();
            var transform = obj.transform;
            transform.SetParent(parent, worldPositionStays);
            transform.position = position;
            transform.rotation = rotation;
            return obj;
        }

        /// <summary>
        /// 延迟归还 GameObject（指定秒数后自动归还）
        /// Return GameObject after delay (automatically return after specified seconds)
        /// </summary>
        /// <param name="pool">GameObject 池 / GameObject pool</param>
        /// <param name="obj">GameObject 实例 / GameObject instance</param>
        /// <param name="delay">延迟秒数 / Delay in seconds</param>
        public static void ReturnAfter(this GameObjectPool pool, GameObject obj, float delay)
        {
            if (pool == null)
                throw new ArgumentNullException(nameof(pool));

            if (obj == null)
                throw new ArgumentNullException(nameof(obj));

            if (delay <= 0)
            {
                pool.Return(obj);
                return;
            }

            // 使用 MonoBehaviour 协程实现延迟归还
            // Use MonoBehaviour coroutine for delayed return
            var helper = obj.GetComponent<PoolReturnHelper>() ?? obj.AddComponent<PoolReturnHelper>();
            helper.ReturnAfter(pool, obj, delay);
        }

        #endregion

        #region 激活与停用扩展 Activation & Deactivation Extensions

        /// <summary>
        /// 获取并激活 GameObject
        /// Get and activate GameObject
        /// </summary>
        /// <param name="pool">GameObject 池 / GameObject pool</param>
        /// <param name="active">是否激活 / Whether to activate</param>
        /// <returns>GameObject 实例 / GameObject instance</returns>
        public static GameObject GetAndSetActive(this GameObjectPool pool, bool active = true)
        {
            if (pool == null)
                throw new ArgumentNullException(nameof(pool));

            var obj = pool.GetGameObject();
            obj.SetActive(active);
            return obj;
        }

        /// <summary>
        /// 停用并归还 GameObject
        /// Deactivate and return GameObject
        /// </summary>
        /// <param name="pool">GameObject 池 / GameObject pool</param>
        /// <param name="obj">GameObject 实例 / GameObject instance</param>
        public static void DeactivateAndReturn(this GameObjectPool pool, GameObject obj)
        {
            if (pool == null)
                throw new ArgumentNullException(nameof(pool));

            if (obj == null)
                return;

            obj.SetActive(false);
            pool.ReturnGameObject(obj);
        }

        #endregion

        #region Transform 操作扩展 Transform Operation Extensions

        /// <summary>
        /// 获取 GameObject 并重置 Transform
        /// Get GameObject and reset Transform
        /// </summary>
        /// <param name="pool">GameObject 池 / GameObject pool</param>
        /// <param name="resetPosition">是否重置位置 / Whether to reset position</param>
        /// <param name="resetRotation">是否重置旋转 / Whether to reset rotation</param>
        /// <param name="resetScale">是否重置缩放 / Whether to reset scale</param>
        /// <returns>GameObject 实例 / GameObject instance</returns>
        public static GameObject GetAndResetTransform(
            this GameObjectPool pool,
            bool resetPosition = true,
            bool resetRotation = true,
            bool resetScale = true)
        {
            if (pool == null)
                throw new ArgumentNullException(nameof(pool));

            var obj = pool.GetGameObject();
            var transform = obj.transform;

            if (resetPosition)
                transform.localPosition = Vector3.zero;

            if (resetRotation)
                transform.localRotation = Quaternion.identity;

            if (resetScale)
                transform.localScale = Vector3.one;

            return obj;
        }

        /// <summary>
        /// 获取 GameObject 并设置本地 Transform
        /// Get GameObject and set local Transform
        /// </summary>
        /// <param name="pool">GameObject 池 / GameObject pool</param>
        /// <param name="localPosition">本地位置 / Local position</param>
        /// <param name="localRotation">本地旋转 / Local rotation</param>
        /// <param name="localScale">本地缩放 / Local scale</param>
        /// <returns>GameObject 实例 / GameObject instance</returns>
        public static GameObject GetWithLocalTransform(
            this GameObjectPool pool,
            Vector3 localPosition,
            Quaternion localRotation,
            Vector3 localScale)
        {
            if (pool == null)
                throw new ArgumentNullException(nameof(pool));

            var obj = pool.GetGameObject();
            var transform = obj.transform;
            transform.localPosition = localPosition;
            transform.localRotation = localRotation;
            transform.localScale = localScale;
            return obj;
        }

        #endregion

        #region 批量操作扩展 Batch Operation Extensions

        /// <summary>
        /// 批量获取 GameObject
        /// Get GameObjects in batch
        /// </summary>
        /// <param name="pool">GameObject 池 / GameObject pool</param>
        /// <param name="count">数量 / Count</param>
        /// <param name="parent">父级 Transform（可选）/ Parent transform (optional)</param>
        /// <returns>GameObject 数组 / GameObject array</returns>
        public static GameObject[] GetMany(this GameObjectPool pool, int count, Transform parent = null)
        {
            if (pool == null)
                throw new ArgumentNullException(nameof(pool));

            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count));

            var objects = new GameObject[count];
            for (int i = 0; i < count; i++)
            {
                objects[i] = pool.GetGameObject();
                if (parent != null)
                {
                    objects[i].transform.SetParent(parent, false);
                }
            }

            return objects;
        }

        /// <summary>
        /// 批量归还 GameObject
        /// Return GameObjects in batch
        /// </summary>
        /// <param name="pool">GameObject 池 / GameObject pool</param>
        /// <param name="objects">GameObject 数组 / GameObject array</param>
        public static void ReturnMany(this GameObjectPool pool, params GameObject[] objects)
        {
            if (pool == null)
                throw new ArgumentNullException(nameof(pool));

            if (objects == null || objects.Length == 0)
                return;

            foreach (var obj in objects)
            {
                if (obj != null)
                {
                    pool.ReturnGameObject(obj);
                }
            }
        }

        #endregion

        #region 条件操作扩展 Conditional Operation Extensions

        /// <summary>
        /// 尝试获取 GameObject
        /// Try to get GameObject
        /// </summary>
        /// <param name="pool">GameObject 池 / GameObject pool</param>
        /// <param name="obj">输出 GameObject / Output GameObject</param>
        /// <returns>是否成功获取 / Whether successfully got</returns>
        public static bool TryGet(this GameObjectPool pool, out GameObject obj)
        {
            obj = null;

            if (pool == null)
                return false;

            try
            {
                obj = pool.GetGameObject();
                return obj != null;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 安全归还 GameObject（忽略异常）
        /// Safely return GameObject (ignore exceptions)
        /// </summary>
        /// <param name="pool">GameObject 池 / GameObject pool</param>
        /// <param name="obj">GameObject 实例 / GameObject instance</param>
        /// <returns>是否成功归还 / Whether successfully returned</returns>
        public static bool TryReturn(this GameObjectPool pool, GameObject obj)
        {
            if (pool == null || obj == null)
                return false;

            try
            {
                pool.ReturnGameObject(obj);
                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region 查询扩展 Query Extensions

        /// <summary>
        /// 检查 GameObject 是否属于此池
        /// Check if GameObject belongs to this pool
        /// </summary>
        /// <param name="pool">GameObject 池 / GameObject pool</param>
        /// <param name="obj">GameObject 实例 / GameObject instance</param>
        /// <returns>是否属于此池 / Whether belongs to this pool</returns>
        public static bool Contains(this GameObjectPool pool, GameObject obj)
        {
            if (pool == null || obj == null)
                return false;

            // 通过名称判断（简单实现）
            // Judge by name (simple implementation)
            return obj.name.Contains("(Pooled)");
        }

        #endregion
    }

    #region 辅助类 Helper Classes

    /// <summary>
    /// 池归还辅助组件（用于延迟归还）
    /// Pool Return Helper Component (for delayed return)
    /// </summary>
    internal class PoolReturnHelper : MonoBehaviour
    {
        private System.Collections.IEnumerator _returnCoroutine;

        /// <summary>
        /// 延迟归还
        /// Return after delay
        /// </summary>
        public void ReturnAfter(GameObjectPool pool, GameObject obj, float delay)
        {
            if (_returnCoroutine != null)
            {
                StopCoroutine(_returnCoroutine);
            }

            _returnCoroutine = ReturnCoroutine(pool, obj, delay);
            StartCoroutine(_returnCoroutine);
        }

        private System.Collections.IEnumerator ReturnCoroutine(GameObjectPool pool, GameObject obj, float delay)
        {
            yield return new WaitForSeconds(delay);

            if (pool != null && obj != null)
            {
                pool.ReturnGameObject(obj);
            }

            _returnCoroutine = null;
        }

        private void OnDestroy()
        {
            if (_returnCoroutine != null)
            {
                StopCoroutine(_returnCoroutine);
                _returnCoroutine = null;
            }
        }
    }

    #endregion
}
