// ==========================================================
// 文件名：LayerConstants.cs
// 命名空间: AFramework.UnityExtension
// 依赖: UnityEngine
// ==========================================================

using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace AFramework.UnityExtension
{
    /// <summary>
    /// Unity 层级常量定义
    /// <para>提供 Unity 内置层级和自定义层级的常量访问</para>
    /// <para>支持层级掩码的快速构建和验证</para>
    /// </summary>
    /// <remarks>
    /// 使用示例:
    /// <code>
    /// // 获取层级索引
    /// int layer = LayerConstants.Default;
    /// 
    /// // 获取层级掩码
    /// int mask = LayerConstants.Masks.Default;
    /// 
    /// // 组合多个层级掩码
    /// int combinedMask = LayerConstants.Masks.Default | LayerConstants.Masks.UI;
    /// 
    /// // 验证层级是否有效
    /// bool isValid = LayerConstants.IsValidLayer(5);
    /// </code>
    /// </remarks>
    public static class LayerConstants
    {
        #region Unity 内置层级索引

        /// <summary>默认层级 (Layer 0)</summary>
        public const int Default = 0;

        /// <summary>透明特效层级 (Layer 1)</summary>
        public const int TransparentFX = 1;

        /// <summary>忽略射线检测层级 (Layer 2)</summary>
        public const int IgnoreRaycast = 2;

        /// <summary>水体层级 (Layer 4)</summary>
        public const int Water = 4;

        /// <summary>UI 层级 (Layer 5)</summary>
        public const int UI = 5;

        #endregion

        #region 常用自定义层级索引 (建议范围: 6-31)

        /// <summary>玩家层级 (建议 Layer 6)</summary>
        public const int Player = 6;

        /// <summary>敌人层级 (建议 Layer 7)</summary>
        public const int Enemy = 7;

        /// <summary>NPC 层级 (建议 Layer 8)</summary>
        public const int NPC = 8;

        /// <summary>可交互物体层级 (建议 Layer 9)</summary>
        public const int Interactable = 9;

        /// <summary>地面层级 (建议 Layer 10)</summary>
        public const int Ground = 10;

        /// <summary>障碍物层级 (建议 Layer 11)</summary>
        public const int Obstacle = 11;

        /// <summary>触发器层级 (建议 Layer 12)</summary>
        public const int Trigger = 12;

        /// <summary>投射物层级 (建议 Layer 13)</summary>
        public const int Projectile = 13;

        /// <summary>特效层级 (建议 Layer 14)</summary>
        public const int VFX = 14;

        /// <summary>可拾取物品层级 (建议 Layer 15)</summary>
        public const int Pickup = 15;

        #endregion

        #region 层级范围常量

        /// <summary>最小有效层级索引</summary>
        public const int MinLayer = 0;

        /// <summary>最大有效层级索引</summary>
        public const int MaxLayer = 31;

        /// <summary>总层级数量</summary>
        public const int LayerCount = 32;

        /// <summary>用户自定义层级起始索引</summary>
        public const int UserLayerStart = 6;

        #endregion

        #region 层级掩码常量

        /// <summary>
        /// 层级掩码常量集合
        /// <para>提供预计算的层级掩码，避免运行时位运算</para>
        /// </summary>
        public static class Masks
        {
            /// <summary>默认层级掩码</summary>
            public const int Default = 1 << LayerConstants.Default;

            /// <summary>透明特效层级掩码</summary>
            public const int TransparentFX = 1 << LayerConstants.TransparentFX;

            /// <summary>忽略射线检测层级掩码</summary>
            public const int IgnoreRaycast = 1 << LayerConstants.IgnoreRaycast;

            /// <summary>水体层级掩码</summary>
            public const int Water = 1 << LayerConstants.Water;

            /// <summary>UI 层级掩码</summary>
            public const int UI = 1 << LayerConstants.UI;

            /// <summary>玩家层级掩码</summary>
            public const int Player = 1 << LayerConstants.Player;

            /// <summary>敌人层级掩码</summary>
            public const int Enemy = 1 << LayerConstants.Enemy;

            /// <summary>NPC 层级掩码</summary>
            public const int NPC = 1 << LayerConstants.NPC;

            /// <summary>可交互物体层级掩码</summary>
            public const int Interactable = 1 << LayerConstants.Interactable;

            /// <summary>地面层级掩码</summary>
            public const int Ground = 1 << LayerConstants.Ground;

            /// <summary>障碍物层级掩码</summary>
            public const int Obstacle = 1 << LayerConstants.Obstacle;

            /// <summary>触发器层级掩码</summary>
            public const int Trigger = 1 << LayerConstants.Trigger;

            /// <summary>投射物层级掩码</summary>
            public const int Projectile = 1 << LayerConstants.Projectile;

            /// <summary>特效层级掩码</summary>
            public const int VFX = 1 << LayerConstants.VFX;

            /// <summary>可拾取物品层级掩码</summary>
            public const int Pickup = 1 << LayerConstants.Pickup;

            /// <summary>所有层级掩码 (全选)</summary>
            public const int Everything = ~0;

            /// <summary>无层级掩码 (全不选)</summary>
            public const int Nothing = 0;

            /// <summary>所有角色层级掩码 (玩家 + 敌人 + NPC)</summary>
            public const int AllCharacters = Player | Enemy | NPC;

            /// <summary>所有可碰撞层级掩码 (地面 + 障碍物)</summary>
            public const int AllCollidable = Ground | Obstacle;

            /// <summary>默认射线检测掩码 (排除 IgnoreRaycast)</summary>
            public const int DefaultRaycast = Everything & ~IgnoreRaycast;
        }

        #endregion

        #region 工具方法

        /// <summary>
        /// 验证层级索引是否有效
        /// </summary>
        /// <param name="layer">层级索引</param>
        /// <returns>如果层级在有效范围内返回 true</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsValidLayer(int layer)
        {
            return layer >= MinLayer && layer <= MaxLayer;
        }

        /// <summary>
        /// 将层级索引转换为层级掩码
        /// </summary>
        /// <param name="layer">层级索引 (0-31)</param>
        /// <returns>对应的层级掩码</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ToMask(int layer)
        {
            return 1 << layer;
        }

        /// <summary>
        /// 组合多个层级索引为层级掩码
        /// </summary>
        /// <param name="layers">层级索引数组</param>
        /// <returns>组合后的层级掩码</returns>
        public static int CombineMask(params int[] layers)
        {
            int mask = 0;
            for (int i = 0; i < layers.Length; i++)
            {
                mask |= 1 << layers[i];
            }
            return mask;
        }

        /// <summary>
        /// 检查层级掩码是否包含指定层级
        /// </summary>
        /// <param name="mask">层级掩码</param>
        /// <param name="layer">层级索引</param>
        /// <returns>如果掩码包含该层级返回 true</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ContainsLayer(int mask, int layer)
        {
            return (mask & (1 << layer)) != 0;
        }

        /// <summary>
        /// 向层级掩码添加层级
        /// </summary>
        /// <param name="mask">原始层级掩码</param>
        /// <param name="layer">要添加的层级索引</param>
        /// <returns>添加后的层级掩码</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int AddLayer(int mask, int layer)
        {
            return mask | (1 << layer);
        }

        /// <summary>
        /// 从层级掩码移除层级
        /// </summary>
        /// <param name="mask">原始层级掩码</param>
        /// <param name="layer">要移除的层级索引</param>
        /// <returns>移除后的层级掩码</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int RemoveLayer(int mask, int layer)
        {
            return mask & ~(1 << layer);
        }

        /// <summary>
        /// 切换层级掩码中的层级状态
        /// </summary>
        /// <param name="mask">原始层级掩码</param>
        /// <param name="layer">要切换的层级索引</param>
        /// <returns>切换后的层级掩码</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ToggleLayer(int mask, int layer)
        {
            return mask ^ (1 << layer);
        }

        /// <summary>
        /// 获取层级名称
        /// </summary>
        /// <param name="layer">层级索引</param>
        /// <returns>层级名称，如果层级无效返回空字符串</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetLayerName(int layer)
        {
            return LayerMask.LayerToName(layer);
        }

        /// <summary>
        /// 根据名称获取层级索引
        /// </summary>
        /// <param name="layerName">层级名称</param>
        /// <returns>层级索引，如果名称无效返回 -1</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetLayerIndex(string layerName)
        {
            return LayerMask.NameToLayer(layerName);
        }

        /// <summary>
        /// 根据名称获取层级掩码
        /// </summary>
        /// <param name="layerName">层级名称</param>
        /// <returns>层级掩码</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetMask(string layerName)
        {
            return LayerMask.GetMask(layerName);
        }

        /// <summary>
        /// 根据多个名称获取组合层级掩码
        /// </summary>
        /// <param name="layerNames">层级名称数组</param>
        /// <returns>组合后的层级掩码</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetMask(params string[] layerNames)
        {
            return LayerMask.GetMask(layerNames);
        }

        #endregion
    }
}
