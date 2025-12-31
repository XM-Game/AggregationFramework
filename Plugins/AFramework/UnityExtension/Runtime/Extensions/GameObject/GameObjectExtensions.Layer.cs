// ==========================================================
// 文件名：GameObjectExtensions.Layer.cs
// 命名空间: AFramework.UnityExtension
// 依赖: UnityEngine, System.Collections.Generic
// ==========================================================

using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace AFramework.UnityExtension
{
    /// <summary>
    /// GameObject 层级设置扩展方法
    /// </summary>
    public static partial class GameObjectExtensions
    {
        #region 层级设置

        /// <summary>
        /// 设置层级
        /// </summary>
        /// <param name="gameObject">目标 GameObject</param>
        /// <param name="layer">层级索引</param>
        /// <returns>返回自身以支持链式调用</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static GameObject SetLayer(this GameObject gameObject, int layer)
        {
            gameObject.layer = layer;
            return gameObject;
        }

        /// <summary>
        /// 通过层级名称设置层级
        /// </summary>
        /// <param name="gameObject">目标 GameObject</param>
        /// <param name="layerName">层级名称</param>
        /// <returns>返回自身以支持链式调用</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static GameObject SetLayer(this GameObject gameObject, string layerName)
        {
            gameObject.layer = LayerMask.NameToLayer(layerName);
            return gameObject;
        }

        /// <summary>
        /// 递归设置层级 (包含所有子级)
        /// </summary>
        /// <param name="gameObject">目标 GameObject</param>
        /// <param name="layer">层级索引</param>
        /// <returns>返回自身以支持链式调用</returns>
        public static GameObject SetLayerRecursive(this GameObject gameObject, int layer)
        {
            gameObject.layer = layer;
            SetLayerRecursiveInternal(gameObject.transform, layer);
            return gameObject;
        }

        /// <summary>
        /// 递归设置层级 (包含所有子级)
        /// </summary>
        /// <param name="gameObject">目标 GameObject</param>
        /// <param name="layerName">层级名称</param>
        /// <returns>返回自身以支持链式调用</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static GameObject SetLayerRecursive(this GameObject gameObject, string layerName)
        {
            return gameObject.SetLayerRecursive(LayerMask.NameToLayer(layerName));
        }

        private static void SetLayerRecursiveInternal(Transform transform, int layer)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                var child = transform.GetChild(i);
                child.gameObject.layer = layer;
                SetLayerRecursiveInternal(child, layer);
            }
        }

        #endregion

        #region 层级检查

        /// <summary>
        /// 检查是否在指定层级
        /// </summary>
        /// <param name="gameObject">目标 GameObject</param>
        /// <param name="layer">层级索引</param>
        /// <returns>如果在指定层级返回 true</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsInLayer(this GameObject gameObject, int layer)
        {
            return gameObject.layer == layer;
        }

        /// <summary>
        /// 检查是否在指定层级
        /// </summary>
        /// <param name="gameObject">目标 GameObject</param>
        /// <param name="layerName">层级名称</param>
        /// <returns>如果在指定层级返回 true</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsInLayer(this GameObject gameObject, string layerName)
        {
            return gameObject.layer == LayerMask.NameToLayer(layerName);
        }

        /// <summary>
        /// 检查是否在指定层级掩码中
        /// </summary>
        /// <param name="gameObject">目标 GameObject</param>
        /// <param name="layerMask">层级掩码</param>
        /// <returns>如果在层级掩码中返回 true</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsInLayerMask(this GameObject gameObject, LayerMask layerMask)
        {
            return ((1 << gameObject.layer) & layerMask.value) != 0;
        }

        /// <summary>
        /// 检查是否在任意一个指定层级中
        /// </summary>
        /// <param name="gameObject">目标 GameObject</param>
        /// <param name="layers">层级索引数组</param>
        /// <returns>如果在任意一个层级中返回 true</returns>
        public static bool IsInAnyLayer(this GameObject gameObject, params int[] layers)
        {
            var currentLayer = gameObject.layer;
            foreach (var layer in layers)
            {
                if (currentLayer == layer)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 检查是否在任意一个指定层级中
        /// </summary>
        /// <param name="gameObject">目标 GameObject</param>
        /// <param name="layerNames">层级名称数组</param>
        /// <returns>如果在任意一个层级中返回 true</returns>
        public static bool IsInAnyLayer(this GameObject gameObject, params string[] layerNames)
        {
            var currentLayer = gameObject.layer;
            foreach (var layerName in layerNames)
            {
                if (currentLayer == LayerMask.NameToLayer(layerName))
                {
                    return true;
                }
            }
            return false;
        }

        #endregion

        #region 层级信息

        /// <summary>
        /// 获取层级名称
        /// </summary>
        /// <param name="gameObject">目标 GameObject</param>
        /// <returns>层级名称</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetLayerName(this GameObject gameObject)
        {
            return LayerMask.LayerToName(gameObject.layer);
        }

        /// <summary>
        /// 获取层级掩码
        /// </summary>
        /// <param name="gameObject">目标 GameObject</param>
        /// <returns>层级掩码</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetLayerMask(this GameObject gameObject)
        {
            return 1 << gameObject.layer;
        }

        #endregion

        #region 层级查找

        /// <summary>
        /// 在子级中查找指定层级的 GameObject
        /// </summary>
        /// <param name="gameObject">目标 GameObject</param>
        /// <param name="layer">层级索引</param>
        /// <returns>找到的 GameObject 列表</returns>
        public static List<GameObject> FindChildrenInLayer(this GameObject gameObject, int layer)
        {
            var result = new List<GameObject>();
            FindChildrenInLayerInternal(gameObject.transform, layer, result);
            return result;
        }

        /// <summary>
        /// 在子级中查找指定层级的 GameObject
        /// </summary>
        /// <param name="gameObject">目标 GameObject</param>
        /// <param name="layerName">层级名称</param>
        /// <returns>找到的 GameObject 列表</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static List<GameObject> FindChildrenInLayer(this GameObject gameObject, string layerName)
        {
            return gameObject.FindChildrenInLayer(LayerMask.NameToLayer(layerName));
        }

        private static void FindChildrenInLayerInternal(Transform transform, int layer, List<GameObject> result)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                var child = transform.GetChild(i);
                if (child.gameObject.layer == layer)
                {
                    result.Add(child.gameObject);
                }
                FindChildrenInLayerInternal(child, layer, result);
            }
        }

        /// <summary>
        /// 在子级中查找指定层级掩码的 GameObject
        /// </summary>
        /// <param name="gameObject">目标 GameObject</param>
        /// <param name="layerMask">层级掩码</param>
        /// <returns>找到的 GameObject 列表</returns>
        public static List<GameObject> FindChildrenInLayerMask(this GameObject gameObject, LayerMask layerMask)
        {
            var result = new List<GameObject>();
            FindChildrenInLayerMaskInternal(gameObject.transform, layerMask, result);
            return result;
        }

        private static void FindChildrenInLayerMaskInternal(Transform transform, LayerMask layerMask, List<GameObject> result)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                var child = transform.GetChild(i);
                if (((1 << child.gameObject.layer) & layerMask.value) != 0)
                {
                    result.Add(child.gameObject);
                }
                FindChildrenInLayerMaskInternal(child, layerMask, result);
            }
        }

        #endregion

        #region 层级统计

        /// <summary>
        /// 统计子级中各层级的 GameObject 数量
        /// </summary>
        /// <param name="gameObject">目标 GameObject</param>
        /// <param name="includeSelf">是否包含自身</param>
        /// <returns>层级索引到数量的字典</returns>
        public static Dictionary<int, int> CountChildrenByLayer(this GameObject gameObject, bool includeSelf = false)
        {
            var result = new Dictionary<int, int>();
            if (includeSelf)
            {
                AddLayerCount(result, gameObject.layer);
            }
            CountChildrenByLayerInternal(gameObject.transform, result);
            return result;
        }

        private static void CountChildrenByLayerInternal(Transform transform, Dictionary<int, int> result)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                var child = transform.GetChild(i);
                AddLayerCount(result, child.gameObject.layer);
                CountChildrenByLayerInternal(child, result);
            }
        }

        private static void AddLayerCount(Dictionary<int, int> dict, int layer)
        {
            if (dict.ContainsKey(layer))
            {
                dict[layer]++;
            }
            else
            {
                dict[layer] = 1;
            }
        }

        #endregion
    }
}
