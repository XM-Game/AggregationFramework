// ==========================================================
// 文件名：TransformExtensions.Search.cs
// 命名空间: AFramework.UnityExtension
// 依赖: UnityEngine, System.Collections.Generic
// ==========================================================

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace AFramework.UnityExtension
{
    /// <summary>
    /// Transform 查找操作扩展方法
    /// </summary>
    public static partial class TransformExtensions
    {
        #region 按名称查找

        /// <summary>
        /// 在直接子级中查找指定名称的 Transform
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <param name="name">要查找的名称</param>
        /// <returns>找到的 Transform，如果没找到返回 null</returns>
        public static Transform FindDirectChild(this Transform transform, string name)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                var child = transform.GetChild(i);
                if (child.name == name)
                {
                    return child;
                }
            }
            return null;
        }

        /// <summary>
        /// 递归查找指定名称的 Transform (深度优先)
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <param name="name">要查找的名称</param>
        /// <returns>找到的 Transform，如果没找到返回 null</returns>
        public static Transform FindDeep(this Transform transform, string name)
        {
            // 先检查直接子级
            for (int i = 0; i < transform.childCount; i++)
            {
                var child = transform.GetChild(i);
                if (child.name == name)
                {
                    return child;
                }
            }
            
            // 递归查找
            for (int i = 0; i < transform.childCount; i++)
            {
                var result = transform.GetChild(i).FindDeep(name);
                if (result != null)
                {
                    return result;
                }
            }
            
            return null;
        }

        /// <summary>
        /// 递归查找所有指定名称的 Transform
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <param name="name">要查找的名称</param>
        /// <returns>找到的所有 Transform 列表</returns>
        public static List<Transform> FindAllDeep(this Transform transform, string name)
        {
            var result = new List<Transform>();
            FindAllDeepInternal(transform, name, result);
            return result;
        }

        private static void FindAllDeepInternal(Transform transform, string name, List<Transform> result)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                var child = transform.GetChild(i);
                if (child.name == name)
                {
                    result.Add(child);
                }
                FindAllDeepInternal(child, name, result);
            }
        }

        /// <summary>
        /// 查找名称包含指定字符串的 Transform
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <param name="substring">要包含的子字符串</param>
        /// <param name="comparison">字符串比较方式</param>
        /// <returns>找到的 Transform，如果没找到返回 null</returns>
        public static Transform FindByNameContains(this Transform transform, string substring, 
            StringComparison comparison = StringComparison.Ordinal)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                var child = transform.GetChild(i);
                if (child.name.Contains(substring, comparison))
                {
                    return child;
                }
            }
            
            for (int i = 0; i < transform.childCount; i++)
            {
                var result = transform.GetChild(i).FindByNameContains(substring, comparison);
                if (result != null)
                {
                    return result;
                }
            }
            
            return null;
        }

        /// <summary>
        /// 查找名称以指定字符串开头的 Transform
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <param name="prefix">前缀字符串</param>
        /// <param name="comparison">字符串比较方式</param>
        /// <returns>找到的 Transform，如果没找到返回 null</returns>
        public static Transform FindByNameStartsWith(this Transform transform, string prefix,
            StringComparison comparison = StringComparison.Ordinal)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                var child = transform.GetChild(i);
                if (child.name.StartsWith(prefix, comparison))
                {
                    return child;
                }
            }
            
            for (int i = 0; i < transform.childCount; i++)
            {
                var result = transform.GetChild(i).FindByNameStartsWith(prefix, comparison);
                if (result != null)
                {
                    return result;
                }
            }
            
            return null;
        }

        /// <summary>
        /// 查找名称以指定字符串结尾的 Transform
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <param name="suffix">后缀字符串</param>
        /// <param name="comparison">字符串比较方式</param>
        /// <returns>找到的 Transform，如果没找到返回 null</returns>
        public static Transform FindByNameEndsWith(this Transform transform, string suffix,
            StringComparison comparison = StringComparison.Ordinal)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                var child = transform.GetChild(i);
                if (child.name.EndsWith(suffix, comparison))
                {
                    return child;
                }
            }
            
            for (int i = 0; i < transform.childCount; i++)
            {
                var result = transform.GetChild(i).FindByNameEndsWith(suffix, comparison);
                if (result != null)
                {
                    return result;
                }
            }
            
            return null;
        }

        #endregion

        #region 按条件查找

        /// <summary>
        /// 在直接子级中查找满足条件的 Transform
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <param name="predicate">条件谓词</param>
        /// <returns>找到的 Transform，如果没找到返回 null</returns>
        public static Transform FindChild(this Transform transform, Func<Transform, bool> predicate)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                var child = transform.GetChild(i);
                if (predicate(child))
                {
                    return child;
                }
            }
            return null;
        }

        /// <summary>
        /// 递归查找满足条件的 Transform
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <param name="predicate">条件谓词</param>
        /// <returns>找到的 Transform，如果没找到返回 null</returns>
        public static Transform FindChildDeep(this Transform transform, Func<Transform, bool> predicate)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                var child = transform.GetChild(i);
                if (predicate(child))
                {
                    return child;
                }
            }
            
            for (int i = 0; i < transform.childCount; i++)
            {
                var result = transform.GetChild(i).FindChildDeep(predicate);
                if (result != null)
                {
                    return result;
                }
            }
            
            return null;
        }

        /// <summary>
        /// 在直接子级中查找所有满足条件的 Transform
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <param name="predicate">条件谓词</param>
        /// <returns>找到的所有 Transform 列表</returns>
        public static List<Transform> FindChildren(this Transform transform, Func<Transform, bool> predicate)
        {
            var result = new List<Transform>();
            for (int i = 0; i < transform.childCount; i++)
            {
                var child = transform.GetChild(i);
                if (predicate(child))
                {
                    result.Add(child);
                }
            }
            return result;
        }

        /// <summary>
        /// 递归查找所有满足条件的 Transform
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <param name="predicate">条件谓词</param>
        /// <returns>找到的所有 Transform 列表</returns>
        public static List<Transform> FindChildrenDeep(this Transform transform, Func<Transform, bool> predicate)
        {
            var result = new List<Transform>();
            FindChildrenDeepInternal(transform, predicate, result);
            return result;
        }

        private static void FindChildrenDeepInternal(Transform transform, Func<Transform, bool> predicate, List<Transform> result)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                var child = transform.GetChild(i);
                if (predicate(child))
                {
                    result.Add(child);
                }
                FindChildrenDeepInternal(child, predicate, result);
            }
        }

        #endregion

        #region 按组件查找

        /// <summary>
        /// 在直接子级中查找包含指定组件的 Transform
        /// </summary>
        /// <typeparam name="T">组件类型</typeparam>
        /// <param name="transform">目标 Transform</param>
        /// <returns>找到的 Transform，如果没找到返回 null</returns>
        public static Transform FindChildWithComponent<T>(this Transform transform) where T : Component
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                var child = transform.GetChild(i);
                if (child.GetComponent<T>() != null)
                {
                    return child;
                }
            }
            return null;
        }

        /// <summary>
        /// 递归查找包含指定组件的 Transform
        /// </summary>
        /// <typeparam name="T">组件类型</typeparam>
        /// <param name="transform">目标 Transform</param>
        /// <returns>找到的 Transform，如果没找到返回 null</returns>
        public static Transform FindChildWithComponentDeep<T>(this Transform transform) where T : Component
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                var child = transform.GetChild(i);
                if (child.GetComponent<T>() != null)
                {
                    return child;
                }
            }
            
            for (int i = 0; i < transform.childCount; i++)
            {
                var result = transform.GetChild(i).FindChildWithComponentDeep<T>();
                if (result != null)
                {
                    return result;
                }
            }
            
            return null;
        }

        /// <summary>
        /// 在直接子级中查找所有包含指定组件的 Transform
        /// </summary>
        /// <typeparam name="T">组件类型</typeparam>
        /// <param name="transform">目标 Transform</param>
        /// <returns>找到的所有 Transform 列表</returns>
        public static List<Transform> FindChildrenWithComponent<T>(this Transform transform) where T : Component
        {
            var result = new List<Transform>();
            for (int i = 0; i < transform.childCount; i++)
            {
                var child = transform.GetChild(i);
                if (child.GetComponent<T>() != null)
                {
                    result.Add(child);
                }
            }
            return result;
        }

        /// <summary>
        /// 递归查找所有包含指定组件的 Transform
        /// </summary>
        /// <typeparam name="T">组件类型</typeparam>
        /// <param name="transform">目标 Transform</param>
        /// <returns>找到的所有 Transform 列表</returns>
        public static List<Transform> FindChildrenWithComponentDeep<T>(this Transform transform) where T : Component
        {
            var result = new List<Transform>();
            FindChildrenWithComponentDeepInternal<T>(transform, result);
            return result;
        }

        private static void FindChildrenWithComponentDeepInternal<T>(Transform transform, List<Transform> result) where T : Component
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                var child = transform.GetChild(i);
                if (child.GetComponent<T>() != null)
                {
                    result.Add(child);
                }
                FindChildrenWithComponentDeepInternal<T>(child, result);
            }
        }

        /// <summary>
        /// 在子级中获取指定组件 (递归)
        /// </summary>
        /// <typeparam name="T">组件类型</typeparam>
        /// <param name="transform">目标 Transform</param>
        /// <returns>找到的组件，如果没找到返回 null</returns>
        public static T GetComponentInChildrenDeep<T>(this Transform transform) where T : Component
        {
            var found = transform.FindChildWithComponentDeep<T>();
            return found != null ? found.GetComponent<T>() : null;
        }

        /// <summary>
        /// 在子级中获取所有指定组件 (递归)
        /// </summary>
        /// <typeparam name="T">组件类型</typeparam>
        /// <param name="transform">目标 Transform</param>
        /// <returns>找到的所有组件列表</returns>
        public static List<T> GetComponentsInChildrenDeep<T>(this Transform transform) where T : Component
        {
            var result = new List<T>();
            GetComponentsInChildrenDeepInternal<T>(transform, result);
            return result;
        }

        private static void GetComponentsInChildrenDeepInternal<T>(Transform transform, List<T> result) where T : Component
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                var child = transform.GetChild(i);
                var component = child.GetComponent<T>();
                if (component != null)
                {
                    result.Add(component);
                }
                GetComponentsInChildrenDeepInternal<T>(child, result);
            }
        }

        #endregion

        #region 按标签查找

        /// <summary>
        /// 在直接子级中查找指定标签的 Transform
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <param name="tag">标签名称</param>
        /// <returns>找到的 Transform，如果没找到返回 null</returns>
        public static Transform FindChildWithTag(this Transform transform, string tag)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                var child = transform.GetChild(i);
                if (child.CompareTag(tag))
                {
                    return child;
                }
            }
            return null;
        }

        /// <summary>
        /// 递归查找指定标签的 Transform
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <param name="tag">标签名称</param>
        /// <returns>找到的 Transform，如果没找到返回 null</returns>
        public static Transform FindChildWithTagDeep(this Transform transform, string tag)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                var child = transform.GetChild(i);
                if (child.CompareTag(tag))
                {
                    return child;
                }
            }
            
            for (int i = 0; i < transform.childCount; i++)
            {
                var result = transform.GetChild(i).FindChildWithTagDeep(tag);
                if (result != null)
                {
                    return result;
                }
            }
            
            return null;
        }

        /// <summary>
        /// 在直接子级中查找所有指定标签的 Transform
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <param name="tag">标签名称</param>
        /// <returns>找到的所有 Transform 列表</returns>
        public static List<Transform> FindChildrenWithTag(this Transform transform, string tag)
        {
            var result = new List<Transform>();
            for (int i = 0; i < transform.childCount; i++)
            {
                var child = transform.GetChild(i);
                if (child.CompareTag(tag))
                {
                    result.Add(child);
                }
            }
            return result;
        }

        /// <summary>
        /// 递归查找所有指定标签的 Transform
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <param name="tag">标签名称</param>
        /// <returns>找到的所有 Transform 列表</returns>
        public static List<Transform> FindChildrenWithTagDeep(this Transform transform, string tag)
        {
            var result = new List<Transform>();
            FindChildrenWithTagDeepInternal(transform, tag, result);
            return result;
        }

        private static void FindChildrenWithTagDeepInternal(Transform transform, string tag, List<Transform> result)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                var child = transform.GetChild(i);
                if (child.CompareTag(tag))
                {
                    result.Add(child);
                }
                FindChildrenWithTagDeepInternal(child, tag, result);
            }
        }

        #endregion

        #region 按层级查找

        /// <summary>
        /// 在直接子级中查找指定层级的 Transform
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <param name="layer">层级</param>
        /// <returns>找到的 Transform，如果没找到返回 null</returns>
        public static Transform FindChildInLayer(this Transform transform, int layer)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                var child = transform.GetChild(i);
                if (child.gameObject.layer == layer)
                {
                    return child;
                }
            }
            return null;
        }

        /// <summary>
        /// 递归查找指定层级的 Transform
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <param name="layer">层级</param>
        /// <returns>找到的 Transform，如果没找到返回 null</returns>
        public static Transform FindChildInLayerDeep(this Transform transform, int layer)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                var child = transform.GetChild(i);
                if (child.gameObject.layer == layer)
                {
                    return child;
                }
            }
            
            for (int i = 0; i < transform.childCount; i++)
            {
                var result = transform.GetChild(i).FindChildInLayerDeep(layer);
                if (result != null)
                {
                    return result;
                }
            }
            
            return null;
        }

        /// <summary>
        /// 在直接子级中查找所有指定层级的 Transform
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <param name="layer">层级</param>
        /// <returns>找到的所有 Transform 列表</returns>
        public static List<Transform> FindChildrenInLayer(this Transform transform, int layer)
        {
            var result = new List<Transform>();
            for (int i = 0; i < transform.childCount; i++)
            {
                var child = transform.GetChild(i);
                if (child.gameObject.layer == layer)
                {
                    result.Add(child);
                }
            }
            return result;
        }

        /// <summary>
        /// 递归查找所有指定层级的 Transform
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <param name="layer">层级</param>
        /// <returns>找到的所有 Transform 列表</returns>
        public static List<Transform> FindChildrenInLayerDeep(this Transform transform, int layer)
        {
            var result = new List<Transform>();
            FindChildrenInLayerDeepInternal(transform, layer, result);
            return result;
        }

        private static void FindChildrenInLayerDeepInternal(Transform transform, int layer, List<Transform> result)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                var child = transform.GetChild(i);
                if (child.gameObject.layer == layer)
                {
                    result.Add(child);
                }
                FindChildrenInLayerDeepInternal(child, layer, result);
            }
        }

        /// <summary>
        /// 在直接子级中查找指定层级名称的 Transform
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <param name="layerName">层级名称</param>
        /// <returns>找到的 Transform，如果没找到返回 null</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Transform FindChildInLayer(this Transform transform, string layerName)
        {
            return transform.FindChildInLayer(LayerMask.NameToLayer(layerName));
        }

        /// <summary>
        /// 递归查找指定层级名称的 Transform
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <param name="layerName">层级名称</param>
        /// <returns>找到的 Transform，如果没找到返回 null</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Transform FindChildInLayerDeep(this Transform transform, string layerName)
        {
            return transform.FindChildInLayerDeep(LayerMask.NameToLayer(layerName));
        }

        #endregion

        #region 最近/最远查找

        /// <summary>
        /// 在直接子级中查找距离指定位置最近的 Transform
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <param name="position">参考位置</param>
        /// <returns>最近的 Transform，如果没有子级返回 null</returns>
        public static Transform FindClosestChild(this Transform transform, Vector3 position)
        {
            if (transform.childCount == 0) return null;
            
            Transform closest = null;
            float closestSqrDistance = float.MaxValue;
            
            for (int i = 0; i < transform.childCount; i++)
            {
                var child = transform.GetChild(i);
                var sqrDistance = (child.position - position).sqrMagnitude;
                if (sqrDistance < closestSqrDistance)
                {
                    closestSqrDistance = sqrDistance;
                    closest = child;
                }
            }
            
            return closest;
        }

        /// <summary>
        /// 在直接子级中查找距离指定位置最远的 Transform
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <param name="position">参考位置</param>
        /// <returns>最远的 Transform，如果没有子级返回 null</returns>
        public static Transform FindFarthestChild(this Transform transform, Vector3 position)
        {
            if (transform.childCount == 0) return null;
            
            Transform farthest = null;
            float farthestSqrDistance = float.MinValue;
            
            for (int i = 0; i < transform.childCount; i++)
            {
                var child = transform.GetChild(i);
                var sqrDistance = (child.position - position).sqrMagnitude;
                if (sqrDistance > farthestSqrDistance)
                {
                    farthestSqrDistance = sqrDistance;
                    farthest = child;
                }
            }
            
            return farthest;
        }

        /// <summary>
        /// 递归查找距离指定位置最近的 Transform
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <param name="position">参考位置</param>
        /// <returns>最近的 Transform，如果没有子级返回 null</returns>
        public static Transform FindClosestChildDeep(this Transform transform, Vector3 position)
        {
            Transform closest = null;
            float closestSqrDistance = float.MaxValue;
            FindClosestChildDeepInternal(transform, position, ref closest, ref closestSqrDistance);
            return closest;
        }

        private static void FindClosestChildDeepInternal(Transform transform, Vector3 position, 
            ref Transform closest, ref float closestSqrDistance)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                var child = transform.GetChild(i);
                var sqrDistance = (child.position - position).sqrMagnitude;
                if (sqrDistance < closestSqrDistance)
                {
                    closestSqrDistance = sqrDistance;
                    closest = child;
                }
                FindClosestChildDeepInternal(child, position, ref closest, ref closestSqrDistance);
            }
        }

        /// <summary>
        /// 在直接子级中查找指定范围内的所有 Transform
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <param name="position">参考位置</param>
        /// <param name="range">范围半径</param>
        /// <returns>范围内的所有 Transform 列表</returns>
        public static List<Transform> FindChildrenInRange(this Transform transform, Vector3 position, float range)
        {
            var result = new List<Transform>();
            var sqrRange = range * range;
            
            for (int i = 0; i < transform.childCount; i++)
            {
                var child = transform.GetChild(i);
                if ((child.position - position).sqrMagnitude <= sqrRange)
                {
                    result.Add(child);
                }
            }
            
            return result;
        }

        /// <summary>
        /// 递归查找指定范围内的所有 Transform
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <param name="position">参考位置</param>
        /// <param name="range">范围半径</param>
        /// <returns>范围内的所有 Transform 列表</returns>
        public static List<Transform> FindChildrenInRangeDeep(this Transform transform, Vector3 position, float range)
        {
            var result = new List<Transform>();
            var sqrRange = range * range;
            FindChildrenInRangeDeepInternal(transform, position, sqrRange, result);
            return result;
        }

        private static void FindChildrenInRangeDeepInternal(Transform transform, Vector3 position, 
            float sqrRange, List<Transform> result)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                var child = transform.GetChild(i);
                if ((child.position - position).sqrMagnitude <= sqrRange)
                {
                    result.Add(child);
                }
                FindChildrenInRangeDeepInternal(child, position, sqrRange, result);
            }
        }

        #endregion

        #region 按路径查找

        /// <summary>
        /// 按路径查找 Transform
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <param name="path">路径 (使用 / 分隔)</param>
        /// <returns>找到的 Transform，如果没找到返回 null</returns>
        public static Transform FindByPath(this Transform transform, string path)
        {
            if (string.IsNullOrEmpty(path)) return transform;
            
            var parts = path.Split('/');
            var current = transform;
            
            foreach (var part in parts)
            {
                if (string.IsNullOrEmpty(part)) continue;
                
                current = current.Find(part);
                if (current == null) return null;
            }
            
            return current;
        }

        /// <summary>
        /// 尝试按路径查找 Transform
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <param name="path">路径 (使用 / 分隔)</param>
        /// <param name="result">找到的 Transform</param>
        /// <returns>如果找到返回 true</returns>
        public static bool TryFindByPath(this Transform transform, string path, out Transform result)
        {
            result = transform.FindByPath(path);
            return result != null;
        }

        #endregion
    }
}
