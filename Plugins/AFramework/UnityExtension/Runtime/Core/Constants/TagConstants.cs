// ==========================================================
// 文件名：TagConstants.cs
// 命名空间: AFramework.UnityExtension
// 依赖: UnityEngine
// ==========================================================

using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace AFramework.UnityExtension
{
    /// <summary>
    /// Unity 标签常量定义
    /// <para>提供 Unity 内置标签和常用自定义标签的常量访问</para>
    /// <para>支持标签验证和比较操作</para>
    /// </summary>
    /// <remarks>
    /// 使用示例:
    /// <code>
    /// // 使用内置标签
    /// if (gameObject.CompareTag(TagConstants.Player))
    /// {
    ///     // 处理玩家逻辑
    /// }
    /// 
    /// // 验证标签是否存在
    /// bool exists = TagConstants.TagExists("CustomTag");
    /// 
    /// // 安全比较标签
    /// bool isPlayer = TagConstants.CompareTag(gameObject, TagConstants.Player);
    /// </code>
    /// </remarks>
    public static class TagConstants
    {
        #region Unity 内置标签

        /// <summary>未标记 (Untagged)</summary>
        public const string Untagged = "Untagged";

        /// <summary>可响应 (Respawn)</summary>
        public const string Respawn = "Respawn";

        /// <summary>完成 (Finish)</summary>
        public const string Finish = "Finish";

        /// <summary>编辑器专用 (EditorOnly)</summary>
        public const string EditorOnly = "EditorOnly";

        /// <summary>主摄像机 (MainCamera)</summary>
        public const string MainCamera = "MainCamera";

        /// <summary>玩家 (Player)</summary>
        public const string Player = "Player";

        /// <summary>游戏控制器 (GameController)</summary>
        public const string GameController = "GameController";

        #endregion

        #region 常用自定义标签 (建议在项目中定义)

        /// <summary>敌人标签</summary>
        public const string Enemy = "Enemy";

        /// <summary>NPC 标签</summary>
        public const string NPC = "NPC";

        /// <summary>可交互物体标签</summary>
        public const string Interactable = "Interactable";

        /// <summary>可拾取物品标签</summary>
        public const string Pickup = "Pickup";

        /// <summary>触发器标签</summary>
        public const string Trigger = "Trigger";

        /// <summary>检查点标签</summary>
        public const string Checkpoint = "Checkpoint";

        /// <summary>传送点标签</summary>
        public const string Teleporter = "Teleporter";

        /// <summary>可破坏物体标签</summary>
        public const string Destructible = "Destructible";

        /// <summary>障碍物标签</summary>
        public const string Obstacle = "Obstacle";

        /// <summary>地面标签</summary>
        public const string Ground = "Ground";

        /// <summary>水体标签</summary>
        public const string Water = "Water";

        /// <summary>投射物标签</summary>
        public const string Projectile = "Projectile";

        /// <summary>UI 元素标签</summary>
        public const string UIElement = "UIElement";

        /// <summary>音频源标签</summary>
        public const string AudioSource = "AudioSource";

        /// <summary>特效标签</summary>
        public const string VFX = "VFX";

        #endregion

        #region 工具方法

        /// <summary>
        /// 安全比较 GameObject 的标签
        /// </summary>
        /// <param name="gameObject">要比较的 GameObject</param>
        /// <param name="tag">目标标签</param>
        /// <returns>如果标签匹配返回 true，GameObject 为 null 时返回 false</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool CompareTag(GameObject gameObject, string tag)
        {
            return gameObject != null && gameObject.CompareTag(tag);
        }

        /// <summary>
        /// 安全比较 Component 所属 GameObject 的标签
        /// </summary>
        /// <param name="component">要比较的 Component</param>
        /// <param name="tag">目标标签</param>
        /// <returns>如果标签匹配返回 true，Component 为 null 时返回 false</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool CompareTag(Component component, string tag)
        {
            return component != null && component.CompareTag(tag);
        }

        /// <summary>
        /// 安全比较 Collider 所属 GameObject 的标签
        /// </summary>
        /// <param name="collider">要比较的 Collider</param>
        /// <param name="tag">目标标签</param>
        /// <returns>如果标签匹配返回 true，Collider 为 null 时返回 false</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool CompareTag(Collider collider, string tag)
        {
            return collider != null && collider.CompareTag(tag);
        }

        /// <summary>
        /// 安全比较 Collider2D 所属 GameObject 的标签
        /// </summary>
        /// <param name="collider">要比较的 Collider2D</param>
        /// <param name="tag">目标标签</param>
        /// <returns>如果标签匹配返回 true，Collider2D 为 null 时返回 false</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool CompareTag(Collider2D collider, string tag)
        {
            return collider != null && collider.CompareTag(tag);
        }

        /// <summary>
        /// 检查 GameObject 是否具有任意一个指定标签
        /// </summary>
        /// <param name="gameObject">要检查的 GameObject</param>
        /// <param name="tags">标签数组</param>
        /// <returns>如果匹配任意一个标签返回 true</returns>
        public static bool HasAnyTag(GameObject gameObject, params string[] tags)
        {
            if (gameObject == null || tags == null || tags.Length == 0)
                return false;

            for (int i = 0; i < tags.Length; i++)
            {
                if (gameObject.CompareTag(tags[i]))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// 检查 Component 所属 GameObject 是否具有任意一个指定标签
        /// </summary>
        /// <param name="component">要检查的 Component</param>
        /// <param name="tags">标签数组</param>
        /// <returns>如果匹配任意一个标签返回 true</returns>
        public static bool HasAnyTag(Component component, params string[] tags)
        {
            return component != null && HasAnyTag(component.gameObject, tags);
        }

        /// <summary>
        /// 检查标签是否为 Unity 内置标签
        /// </summary>
        /// <param name="tag">要检查的标签</param>
        /// <returns>如果是内置标签返回 true</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsBuiltInTag(string tag)
        {
            return tag == Untagged ||
                   tag == Respawn ||
                   tag == Finish ||
                   tag == EditorOnly ||
                   tag == MainCamera ||
                   tag == Player ||
                   tag == GameController;
        }

        /// <summary>
        /// 检查 GameObject 是否未标记
        /// </summary>
        /// <param name="gameObject">要检查的 GameObject</param>
        /// <returns>如果未标记返回 true</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsUntagged(GameObject gameObject)
        {
            return CompareTag(gameObject, Untagged);
        }

        /// <summary>
        /// 检查 GameObject 是否为玩家
        /// </summary>
        /// <param name="gameObject">要检查的 GameObject</param>
        /// <returns>如果是玩家返回 true</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsPlayer(GameObject gameObject)
        {
            return CompareTag(gameObject, Player);
        }

        /// <summary>
        /// 检查 GameObject 是否为敌人
        /// </summary>
        /// <param name="gameObject">要检查的 GameObject</param>
        /// <returns>如果是敌人返回 true</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsEnemy(GameObject gameObject)
        {
            return CompareTag(gameObject, Enemy);
        }

        /// <summary>
        /// 检查 GameObject 是否为主摄像机
        /// </summary>
        /// <param name="gameObject">要检查的 GameObject</param>
        /// <returns>如果是主摄像机返回 true</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsMainCamera(GameObject gameObject)
        {
            return CompareTag(gameObject, MainCamera);
        }

        /// <summary>
        /// 检查 GameObject 是否可交互
        /// </summary>
        /// <param name="gameObject">要检查的 GameObject</param>
        /// <returns>如果可交互返回 true</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsInteractable(GameObject gameObject)
        {
            return CompareTag(gameObject, Interactable);
        }

#if UNITY_EDITOR
        /// <summary>
        /// [仅编辑器] 检查标签是否存在于项目中
        /// </summary>
        /// <param name="tag">要检查的标签</param>
        /// <returns>如果标签存在返回 true</returns>
        public static bool TagExists(string tag)
        {
            try
            {
                // 尝试使用该标签，如果不存在会抛出异常
                var tempGo = new GameObject();
                tempGo.tag = tag;
                UnityEngine.Object.DestroyImmediate(tempGo);
                return true;
            }
            catch
            {
                return false;
            }
        }
#endif

        #endregion
    }
}
