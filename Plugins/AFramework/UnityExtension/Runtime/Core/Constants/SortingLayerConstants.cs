// ==========================================================
// 文件名：SortingLayerConstants.cs
// 命名空间: AFramework.UnityExtension
// 依赖: UnityEngine
// ==========================================================

using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace AFramework.UnityExtension
{
    /// <summary>
    /// Unity 排序层常量定义
    /// <para>提供 2D 渲染排序层的常量访问和工具方法</para>
    /// <para>用于 SpriteRenderer、Canvas 等组件的渲染顺序控制</para>
    /// </summary>
    /// <remarks>
    /// 使用示例:
    /// <code>
    /// // 设置 SpriteRenderer 的排序层
    /// spriteRenderer.sortingLayerName = SortingLayerConstants.UI;
    /// spriteRenderer.sortingOrder = SortingLayerConstants.Orders.Front;
    /// 
    /// // 获取排序层 ID
    /// int layerId = SortingLayerConstants.GetLayerId(SortingLayerConstants.Default);
    /// 
    /// // 比较排序层顺序
    /// bool isAbove = SortingLayerConstants.IsAbove("UI", "Background");
    /// </code>
    /// </remarks>
    public static class SortingLayerConstants
    {
        #region 内置排序层名称

        /// <summary>默认排序层</summary>
        public const string Default = "Default";

        #endregion

        #region 常用自定义排序层名称 (建议在项目中定义)

        /// <summary>背景层 (最底层)</summary>
        public const string Background = "Background";

        /// <summary>远景层</summary>
        public const string FarBackground = "FarBackground";

        /// <summary>中景层</summary>
        public const string Midground = "Midground";

        /// <summary>地面层</summary>
        public const string Ground = "Ground";

        /// <summary>角色层</summary>
        public const string Characters = "Characters";

        /// <summary>前景层</summary>
        public const string Foreground = "Foreground";

        /// <summary>特效层</summary>
        public const string Effects = "Effects";

        /// <summary>UI 层 (最顶层)</summary>
        public const string UI = "UI";

        /// <summary>覆盖层 (高于 UI)</summary>
        public const string Overlay = "Overlay";

        #endregion

        #region 排序顺序常量

        /// <summary>
        /// 排序顺序常量集合
        /// <para>提供常用的 sortingOrder 值</para>
        /// </summary>
        public static class Orders
        {
            /// <summary>最底层</summary>
            public const int Bottom = -32768;

            /// <summary>非常靠后</summary>
            public const int VeryBack = -1000;

            /// <summary>靠后</summary>
            public const int Back = -100;

            /// <summary>稍后</summary>
            public const int BehindDefault = -10;

            /// <summary>默认顺序</summary>
            public const int Default = 0;

            /// <summary>稍前</summary>
            public const int AboveDefault = 10;

            /// <summary>靠前</summary>
            public const int Front = 100;

            /// <summary>非常靠前</summary>
            public const int VeryFront = 1000;

            /// <summary>最顶层</summary>
            public const int Top = 32767;

            /// <summary>UI 背景</summary>
            public const int UIBackground = -100;

            /// <summary>UI 默认</summary>
            public const int UIDefault = 0;

            /// <summary>UI 前景</summary>
            public const int UIForeground = 100;

            /// <summary>UI 弹窗</summary>
            public const int UIPopup = 500;

            /// <summary>UI 提示</summary>
            public const int UITooltip = 1000;

            /// <summary>UI 最顶层</summary>
            public const int UITop = 2000;
        }

        #endregion

        #region 工具方法

        /// <summary>
        /// 根据排序层名称获取排序层 ID
        /// </summary>
        /// <param name="layerName">排序层名称</param>
        /// <returns>排序层 ID</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetLayerId(string layerName)
        {
            return SortingLayer.NameToID(layerName);
        }

        /// <summary>
        /// 根据排序层 ID 获取排序层名称
        /// </summary>
        /// <param name="layerId">排序层 ID</param>
        /// <returns>排序层名称</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetLayerName(int layerId)
        {
            return SortingLayer.IDToName(layerId);
        }

        /// <summary>
        /// 检查排序层是否有效
        /// </summary>
        /// <param name="layerId">排序层 ID</param>
        /// <returns>如果排序层有效返回 true</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsValidLayer(int layerId)
        {
            return SortingLayer.IsValid(layerId);
        }

        /// <summary>
        /// 检查排序层名称是否有效
        /// </summary>
        /// <param name="layerName">排序层名称</param>
        /// <returns>如果排序层名称有效返回 true</returns>
        public static bool IsValidLayerName(string layerName)
        {
            if (string.IsNullOrEmpty(layerName))
                return false;

            int id = SortingLayer.NameToID(layerName);
            return SortingLayer.IsValid(id);
        }

        /// <summary>
        /// 获取排序层的渲染顺序值
        /// </summary>
        /// <param name="layerId">排序层 ID</param>
        /// <returns>渲染顺序值</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetLayerValue(int layerId)
        {
            return SortingLayer.GetLayerValueFromID(layerId);
        }

        /// <summary>
        /// 获取排序层的渲染顺序值
        /// </summary>
        /// <param name="layerName">排序层名称</param>
        /// <returns>渲染顺序值</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetLayerValue(string layerName)
        {
            int id = SortingLayer.NameToID(layerName);
            return SortingLayer.GetLayerValueFromID(id);
        }

        /// <summary>
        /// 比较两个排序层，判断第一个是否在第二个之上
        /// </summary>
        /// <param name="layerName1">第一个排序层名称</param>
        /// <param name="layerName2">第二个排序层名称</param>
        /// <returns>如果第一个排序层在第二个之上返回 true</returns>
        public static bool IsAbove(string layerName1, string layerName2)
        {
            int value1 = GetLayerValue(layerName1);
            int value2 = GetLayerValue(layerName2);
            return value1 > value2;
        }

        /// <summary>
        /// 比较两个排序层，判断第一个是否在第二个之下
        /// </summary>
        /// <param name="layerName1">第一个排序层名称</param>
        /// <param name="layerName2">第二个排序层名称</param>
        /// <returns>如果第一个排序层在第二个之下返回 true</returns>
        public static bool IsBelow(string layerName1, string layerName2)
        {
            int value1 = GetLayerValue(layerName1);
            int value2 = GetLayerValue(layerName2);
            return value1 < value2;
        }

        /// <summary>
        /// 获取所有排序层
        /// </summary>
        /// <returns>排序层数组</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SortingLayer[] GetAllLayers()
        {
            return SortingLayer.layers;
        }

        /// <summary>
        /// 获取所有排序层名称
        /// </summary>
        /// <returns>排序层名称数组</returns>
        public static string[] GetAllLayerNames()
        {
            var layers = SortingLayer.layers;
            var names = new string[layers.Length];
            for (int i = 0; i < layers.Length; i++)
            {
                names[i] = layers[i].name;
            }
            return names;
        }

        /// <summary>
        /// 设置 Renderer 的排序层
        /// </summary>
        /// <param name="renderer">渲染器</param>
        /// <param name="layerName">排序层名称</param>
        /// <param name="order">排序顺序 (可选)</param>
        public static void SetSortingLayer(Renderer renderer, string layerName, int? order = null)
        {
            if (renderer == null)
                return;

            renderer.sortingLayerName = layerName;
            if (order.HasValue)
            {
                renderer.sortingOrder = order.Value;
            }
        }

        /// <summary>
        /// 设置 Canvas 的排序层
        /// </summary>
        /// <param name="canvas">Canvas</param>
        /// <param name="layerName">排序层名称</param>
        /// <param name="order">排序顺序 (可选)</param>
        public static void SetSortingLayer(Canvas canvas, string layerName, int? order = null)
        {
            if (canvas == null)
                return;

            canvas.sortingLayerName = layerName;
            if (order.HasValue)
            {
                canvas.sortingOrder = order.Value;
            }
        }

        #endregion
    }
}
