// ==========================================================
// 文件名：GameObjectExtensions.cs
// 命名空间: AFramework.UnityExtension
// 依赖: UnityEngine
// ==========================================================

using System.Runtime.CompilerServices;
using UnityEngine;

namespace AFramework.UnityExtension
{
    /// <summary>
    /// GameObject 扩展方法
    /// <para>提供 GameObject 的基础操作扩展</para>
    /// </summary>
    public static partial class GameObjectExtensions
    {
        #region 空值检查

        /// <summary>
        /// 检查 GameObject 是否为 null 或已被销毁
        /// </summary>
        /// <param name="gameObject">目标 GameObject</param>
        /// <returns>如果为 null 或已销毁返回 true</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNullOrDestroyed(this GameObject gameObject)
        {
            return gameObject == null;
        }

        /// <summary>
        /// 检查 GameObject 是否有效 (非 null 且未被销毁)
        /// </summary>
        /// <param name="gameObject">目标 GameObject</param>
        /// <returns>如果有效返回 true</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsValid(this GameObject gameObject)
        {
            return gameObject != null;
        }

        #endregion

        #region 名称操作

        /// <summary>
        /// 设置 GameObject 名称
        /// </summary>
        /// <param name="gameObject">目标 GameObject</param>
        /// <param name="name">新名称</param>
        /// <returns>返回自身以支持链式调用</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static GameObject SetName(this GameObject gameObject, string name)
        {
            gameObject.name = name;
            return gameObject;
        }

        /// <summary>
        /// 添加名称前缀
        /// </summary>
        /// <param name="gameObject">目标 GameObject</param>
        /// <param name="prefix">前缀</param>
        /// <returns>返回自身以支持链式调用</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static GameObject AddNamePrefix(this GameObject gameObject, string prefix)
        {
            gameObject.name = prefix + gameObject.name;
            return gameObject;
        }

        /// <summary>
        /// 添加名称后缀
        /// </summary>
        /// <param name="gameObject">目标 GameObject</param>
        /// <param name="suffix">后缀</param>
        /// <returns>返回自身以支持链式调用</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static GameObject AddNameSuffix(this GameObject gameObject, string suffix)
        {
            gameObject.name = gameObject.name + suffix;
            return gameObject;
        }

        #endregion

        #region 标签操作

        /// <summary>
        /// 设置标签
        /// </summary>
        /// <param name="gameObject">目标 GameObject</param>
        /// <param name="tag">标签名称</param>
        /// <returns>返回自身以支持链式调用</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static GameObject SetTag(this GameObject gameObject, string tag)
        {
            gameObject.tag = tag;
            return gameObject;
        }

        /// <summary>
        /// 检查是否具有指定标签
        /// </summary>
        /// <param name="gameObject">目标 GameObject</param>
        /// <param name="tag">标签名称</param>
        /// <returns>如果具有指定标签返回 true</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasTag(this GameObject gameObject, string tag)
        {
            return gameObject.CompareTag(tag);
        }

        /// <summary>
        /// 检查是否具有任意一个指定标签
        /// </summary>
        /// <param name="gameObject">目标 GameObject</param>
        /// <param name="tags">标签数组</param>
        /// <returns>如果具有任意一个标签返回 true</returns>
        public static bool HasAnyTag(this GameObject gameObject, params string[] tags)
        {
            foreach (var tag in tags)
            {
                if (gameObject.CompareTag(tag))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 检查是否为未标记 (Untagged)
        /// </summary>
        /// <param name="gameObject">目标 GameObject</param>
        /// <returns>如果未标记返回 true</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsUntagged(this GameObject gameObject)
        {
            return gameObject.CompareTag("Untagged");
        }

        #endregion

        #region Transform 快捷访问

        /// <summary>
        /// 获取世界空间位置
        /// </summary>
        /// <param name="gameObject">目标 GameObject</param>
        /// <returns>世界空间位置</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 GetPosition(this GameObject gameObject)
        {
            return gameObject.transform.position;
        }

        /// <summary>
        /// 设置世界空间位置
        /// </summary>
        /// <param name="gameObject">目标 GameObject</param>
        /// <param name="position">新位置</param>
        /// <returns>返回自身以支持链式调用</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static GameObject SetPosition(this GameObject gameObject, Vector3 position)
        {
            gameObject.transform.position = position;
            return gameObject;
        }

        /// <summary>
        /// 设置世界空间位置
        /// </summary>
        /// <param name="gameObject">目标 GameObject</param>
        /// <param name="x">X 坐标</param>
        /// <param name="y">Y 坐标</param>
        /// <param name="z">Z 坐标</param>
        /// <returns>返回自身以支持链式调用</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static GameObject SetPosition(this GameObject gameObject, float x, float y, float z)
        {
            gameObject.transform.position = new Vector3(x, y, z);
            return gameObject;
        }

        /// <summary>
        /// 获取本地空间位置
        /// </summary>
        /// <param name="gameObject">目标 GameObject</param>
        /// <returns>本地空间位置</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 GetLocalPosition(this GameObject gameObject)
        {
            return gameObject.transform.localPosition;
        }

        /// <summary>
        /// 设置本地空间位置
        /// </summary>
        /// <param name="gameObject">目标 GameObject</param>
        /// <param name="position">新位置</param>
        /// <returns>返回自身以支持链式调用</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static GameObject SetLocalPosition(this GameObject gameObject, Vector3 position)
        {
            gameObject.transform.localPosition = position;
            return gameObject;
        }

        /// <summary>
        /// 获取世界空间旋转
        /// </summary>
        /// <param name="gameObject">目标 GameObject</param>
        /// <returns>世界空间旋转</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Quaternion GetRotation(this GameObject gameObject)
        {
            return gameObject.transform.rotation;
        }

        /// <summary>
        /// 设置世界空间旋转
        /// </summary>
        /// <param name="gameObject">目标 GameObject</param>
        /// <param name="rotation">新旋转</param>
        /// <returns>返回自身以支持链式调用</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static GameObject SetRotation(this GameObject gameObject, Quaternion rotation)
        {
            gameObject.transform.rotation = rotation;
            return gameObject;
        }

        /// <summary>
        /// 获取本地空间缩放
        /// </summary>
        /// <param name="gameObject">目标 GameObject</param>
        /// <returns>本地空间缩放</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 GetLocalScale(this GameObject gameObject)
        {
            return gameObject.transform.localScale;
        }

        /// <summary>
        /// 设置本地空间缩放
        /// </summary>
        /// <param name="gameObject">目标 GameObject</param>
        /// <param name="scale">新缩放</param>
        /// <returns>返回自身以支持链式调用</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static GameObject SetLocalScale(this GameObject gameObject, Vector3 scale)
        {
            gameObject.transform.localScale = scale;
            return gameObject;
        }

        /// <summary>
        /// 设置统一缩放
        /// </summary>
        /// <param name="gameObject">目标 GameObject</param>
        /// <param name="scale">统一缩放值</param>
        /// <returns>返回自身以支持链式调用</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static GameObject SetLocalScale(this GameObject gameObject, float scale)
        {
            gameObject.transform.localScale = new Vector3(scale, scale, scale);
            return gameObject;
        }

        #endregion

        #region 父级操作

        /// <summary>
        /// 设置父级
        /// </summary>
        /// <param name="gameObject">目标 GameObject</param>
        /// <param name="parent">父级 Transform</param>
        /// <param name="worldPositionStays">是否保持世界空间位置</param>
        /// <returns>返回自身以支持链式调用</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static GameObject SetParent(this GameObject gameObject, Transform parent, bool worldPositionStays = true)
        {
            gameObject.transform.SetParent(parent, worldPositionStays);
            return gameObject;
        }

        /// <summary>
        /// 设置父级
        /// </summary>
        /// <param name="gameObject">目标 GameObject</param>
        /// <param name="parent">父级 GameObject</param>
        /// <param name="worldPositionStays">是否保持世界空间位置</param>
        /// <returns>返回自身以支持链式调用</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static GameObject SetParent(this GameObject gameObject, GameObject parent, bool worldPositionStays = true)
        {
            gameObject.transform.SetParent(parent?.transform, worldPositionStays);
            return gameObject;
        }

        /// <summary>
        /// 移除父级 (设置为根级)
        /// </summary>
        /// <param name="gameObject">目标 GameObject</param>
        /// <param name="worldPositionStays">是否保持世界空间位置</param>
        /// <returns>返回自身以支持链式调用</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static GameObject Unparent(this GameObject gameObject, bool worldPositionStays = true)
        {
            gameObject.transform.SetParent(null, worldPositionStays);
            return gameObject;
        }

        /// <summary>
        /// 获取父级 GameObject
        /// </summary>
        /// <param name="gameObject">目标 GameObject</param>
        /// <returns>父级 GameObject，如果没有父级返回 null</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static GameObject GetParent(this GameObject gameObject)
        {
            var parent = gameObject.transform.parent;
            return parent != null ? parent.gameObject : null;
        }

        /// <summary>
        /// 获取根级 GameObject
        /// </summary>
        /// <param name="gameObject">目标 GameObject</param>
        /// <returns>根级 GameObject</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static GameObject GetRoot(this GameObject gameObject)
        {
            return gameObject.transform.root.gameObject;
        }

        #endregion

        #region 销毁操作

        /// <summary>
        /// 销毁 GameObject
        /// </summary>
        /// <param name="gameObject">目标 GameObject</param>
        /// <param name="delay">延迟时间 (秒)</param>
        public static void Destroy(this GameObject gameObject, float delay = 0f)
        {
            if (delay > 0f)
            {
                Object.Destroy(gameObject, delay);
            }
            else
            {
                Object.Destroy(gameObject);
            }
        }

        /// <summary>
        /// 立即销毁 GameObject (编辑器模式下使用)
        /// </summary>
        /// <param name="gameObject">目标 GameObject</param>
        public static void DestroyImmediate(this GameObject gameObject)
        {
            Object.DestroyImmediate(gameObject);
        }

        /// <summary>
        /// 安全销毁 GameObject (根据运行模式选择销毁方式)
        /// </summary>
        /// <param name="gameObject">目标 GameObject</param>
        public static void DestroySafe(this GameObject gameObject)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                Object.DestroyImmediate(gameObject);
                return;
            }
#endif
            Object.Destroy(gameObject);
        }

        #endregion

        #region 实例化

        /// <summary>
        /// 实例化 GameObject
        /// </summary>
        /// <param name="gameObject">原型 GameObject</param>
        /// <returns>实例化的 GameObject</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static GameObject Instantiate(this GameObject gameObject)
        {
            return Object.Instantiate(gameObject);
        }

        /// <summary>
        /// 实例化 GameObject 并设置父级
        /// </summary>
        /// <param name="gameObject">原型 GameObject</param>
        /// <param name="parent">父级 Transform</param>
        /// <param name="worldPositionStays">是否保持世界空间位置</param>
        /// <returns>实例化的 GameObject</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static GameObject Instantiate(this GameObject gameObject, Transform parent, bool worldPositionStays = false)
        {
            return Object.Instantiate(gameObject, parent, worldPositionStays);
        }

        /// <summary>
        /// 实例化 GameObject 并设置位置和旋转
        /// </summary>
        /// <param name="gameObject">原型 GameObject</param>
        /// <param name="position">位置</param>
        /// <param name="rotation">旋转</param>
        /// <returns>实例化的 GameObject</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static GameObject Instantiate(this GameObject gameObject, Vector3 position, Quaternion rotation)
        {
            return Object.Instantiate(gameObject, position, rotation);
        }

        /// <summary>
        /// 实例化 GameObject 并设置位置、旋转和父级
        /// </summary>
        /// <param name="gameObject">原型 GameObject</param>
        /// <param name="position">位置</param>
        /// <param name="rotation">旋转</param>
        /// <param name="parent">父级 Transform</param>
        /// <returns>实例化的 GameObject</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static GameObject Instantiate(this GameObject gameObject, Vector3 position, Quaternion rotation, Transform parent)
        {
            return Object.Instantiate(gameObject, position, rotation, parent);
        }

        #endregion

        #region DontDestroyOnLoad

        /// <summary>
        /// 标记为场景切换时不销毁
        /// </summary>
        /// <param name="gameObject">目标 GameObject</param>
        /// <returns>返回自身以支持链式调用</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static GameObject DontDestroyOnLoad(this GameObject gameObject)
        {
            Object.DontDestroyOnLoad(gameObject);
            return gameObject;
        }

        #endregion
    }
}
