// ==========================================================
// 文件名：LayerMaskExtensions.cs
// 命名空间: AFramework.UnityExtension
// 依赖: UnityEngine, System
// ==========================================================

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace AFramework.UnityExtension
{
    /// <summary>
    /// LayerMask 扩展方法
    /// <para>提供 LayerMask 的位操作和实用功能扩展</para>
    /// </summary>
    public static class LayerMaskExtensions
    {
        #region 层检查

        /// <summary>
        /// 检查层掩码是否包含指定层
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Contains(this LayerMask mask, int layer)
        {
            return (mask.value & (1 << layer)) != 0;
        }

        /// <summary>
        /// 检查层掩码是否包含指定层 (按名称)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Contains(this LayerMask mask, string layerName)
        {
            int layer = LayerMask.NameToLayer(layerName);
            return layer >= 0 && (mask.value & (1 << layer)) != 0;
        }

        /// <summary>
        /// 检查层掩码是否包含 GameObject 的层
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Contains(this LayerMask mask, GameObject gameObject)
        {
            return gameObject != null && (mask.value & (1 << gameObject.layer)) != 0;
        }

        /// <summary>
        /// 检查层掩码是否为空
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsEmpty(this LayerMask mask)
        {
            return mask.value == 0;
        }

        /// <summary>
        /// 检查层掩码是否包含所有层
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsAll(this LayerMask mask)
        {
            return mask.value == -1;
        }

        #endregion

        #region 层操作

        /// <summary>
        /// 添加层到掩码
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static LayerMask Add(this LayerMask mask, int layer)
        {
            return mask.value | (1 << layer);
        }

        /// <summary>
        /// 添加层到掩码 (按名称)
        /// </summary>
        public static LayerMask Add(this LayerMask mask, string layerName)
        {
            int layer = LayerMask.NameToLayer(layerName);
            if (layer < 0) return mask;
            return mask.value | (1 << layer);
        }

        /// <summary>
        /// 添加多个层到掩码
        /// </summary>
        public static LayerMask Add(this LayerMask mask, params int[] layers)
        {
            int result = mask.value;
            foreach (int layer in layers)
            {
                result |= (1 << layer);
            }
            return result;
        }

        /// <summary>
        /// 添加多个层到掩码 (按名称)
        /// </summary>
        public static LayerMask Add(this LayerMask mask, params string[] layerNames)
        {
            int result = mask.value;
            foreach (string name in layerNames)
            {
                int layer = LayerMask.NameToLayer(name);
                if (layer >= 0)
                {
                    result |= (1 << layer);
                }
            }
            return result;
        }

        /// <summary>
        /// 从掩码移除层
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static LayerMask Remove(this LayerMask mask, int layer)
        {
            return mask.value & ~(1 << layer);
        }

        /// <summary>
        /// 从掩码移除层 (按名称)
        /// </summary>
        public static LayerMask Remove(this LayerMask mask, string layerName)
        {
            int layer = LayerMask.NameToLayer(layerName);
            if (layer < 0) return mask;
            return mask.value & ~(1 << layer);
        }

        /// <summary>
        /// 从掩码移除多个层
        /// </summary>
        public static LayerMask Remove(this LayerMask mask, params int[] layers)
        {
            int result = mask.value;
            foreach (int layer in layers)
            {
                result &= ~(1 << layer);
            }
            return result;
        }

        /// <summary>
        /// 切换层的包含状态
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static LayerMask Toggle(this LayerMask mask, int layer)
        {
            return mask.value ^ (1 << layer);
        }

        /// <summary>
        /// 切换层的包含状态 (按名称)
        /// </summary>
        public static LayerMask Toggle(this LayerMask mask, string layerName)
        {
            int layer = LayerMask.NameToLayer(layerName);
            if (layer < 0) return mask;
            return mask.value ^ (1 << layer);
        }

        /// <summary>
        /// 反转层掩码
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static LayerMask Invert(this LayerMask mask)
        {
            return ~mask.value;
        }

        #endregion

        #region 集合操作

        /// <summary>
        /// 与另一个掩码进行并集
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static LayerMask Union(this LayerMask a, LayerMask b)
        {
            return a.value | b.value;
        }

        /// <summary>
        /// 与另一个掩码进行交集
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static LayerMask Intersection(this LayerMask a, LayerMask b)
        {
            return a.value & b.value;
        }

        /// <summary>
        /// 与另一个掩码进行差集
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static LayerMask Difference(this LayerMask a, LayerMask b)
        {
            return a.value & ~b.value;
        }

        #endregion

        #region 转换

        /// <summary>
        /// 获取掩码中包含的所有层索引
        /// </summary>
        public static int[] GetLayers(this LayerMask mask)
        {
            var layers = new List<int>();
            for (int i = 0; i < 32; i++)
            {
                if ((mask.value & (1 << i)) != 0)
                {
                    layers.Add(i);
                }
            }
            return layers.ToArray();
        }

        /// <summary>
        /// 获取掩码中包含的所有层名称
        /// </summary>
        public static string[] GetLayerNames(this LayerMask mask)
        {
            var names = new List<string>();
            for (int i = 0; i < 32; i++)
            {
                if ((mask.value & (1 << i)) != 0)
                {
                    string name = LayerMask.LayerToName(i);
                    if (!string.IsNullOrEmpty(name))
                    {
                        names.Add(name);
                    }
                }
            }
            return names.ToArray();
        }

        /// <summary>
        /// 获取掩码中包含的层数量
        /// </summary>
        public static int GetLayerCount(this LayerMask mask)
        {
            int count = 0;
            int value = mask.value;
            while (value != 0)
            {
                count += value & 1;
                value >>= 1;
            }
            return count;
        }

        /// <summary>
        /// 获取第一个包含的层
        /// </summary>
        public static int GetFirstLayer(this LayerMask mask)
        {
            for (int i = 0; i < 32; i++)
            {
                if ((mask.value & (1 << i)) != 0)
                {
                    return i;
                }
            }
            return -1;
        }

        #endregion

        #region 创建

        /// <summary>
        /// 从层索引创建掩码
        /// </summary>
        public static LayerMask FromLayers(params int[] layers)
        {
            int mask = 0;
            foreach (int layer in layers)
            {
                mask |= (1 << layer);
            }
            return mask;
        }

        /// <summary>
        /// 从层名称创建掩码
        /// </summary>
        public static LayerMask FromLayerNames(params string[] layerNames)
        {
            return LayerMask.GetMask(layerNames);
        }

        /// <summary>
        /// 创建包含所有层的掩码
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static LayerMask All()
        {
            return -1;
        }

        /// <summary>
        /// 创建空掩码
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static LayerMask None()
        {
            return 0;
        }

        #endregion
    }
}
