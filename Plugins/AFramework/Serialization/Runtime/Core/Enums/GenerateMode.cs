// ==========================================================
// 文件名：GenerateMode.cs
// 命名空间: AFramework.Serialization
// 依赖: System
// ==========================================================

using System;
using System.Runtime.CompilerServices;

namespace AFramework.Serialization
{
    /// <summary>
    /// 代码生成模式枚举
    /// <para>定义源代码生成器的生成策略</para>
    /// </summary>
    /// <remarks>
    /// 使用示例:
    /// <code>
    /// // 使用自动生成模式
    /// [Serializable(GenerateMode = GenerateMode.Auto)]
    /// public class PlayerData { }
    /// 
    /// // 使用手动实现模式
    /// [Serializable(GenerateMode = GenerateMode.Manual)]
    /// public class CustomData : ISerializable { }
    /// </code>
    /// </remarks>
    public enum GenerateMode : byte
    {
        /// <summary>
        /// 自动生成模式 (默认)
        /// <para>源代码生成器自动生成序列化代码</para>
        /// <para>特点：零配置、编译时生成、类型安全</para>
        /// <para>适用：大多数标准数据类型</para>
        /// </summary>
        Auto = 0,

        /// <summary>
        /// 手动实现模式
        /// <para>开发者手动实现序列化逻辑</para>
        /// <para>特点：完全控制、自定义逻辑、灵活性高</para>
        /// <para>适用：特殊序列化需求、复杂类型</para>
        /// </summary>
        Manual = 1,

        /// <summary>
        /// 混合模式
        /// <para>自动生成基础代码，允许部分手动覆盖</para>
        /// <para>特点：基础自动化、关键部分可定制</para>
        /// <para>适用：需要部分自定义的类型</para>
        /// </summary>
        Hybrid = 2,

        /// <summary>
        /// 反射模式
        /// <para>运行时使用反射进行序列化</para>
        /// <para>特点：无需生成代码、运行时开销、灵活</para>
        /// <para>适用：动态类型、插件系统、AOT不支持场景</para>
        /// </summary>
        Reflection = 3,

        /// <summary>
        /// 表达式树模式
        /// <para>运行时编译表达式树</para>
        /// <para>特点：首次调用编译、后续调用快速</para>
        /// <para>适用：需要运行时生成但追求性能</para>
        /// </summary>
        Expression = 4,

        /// <summary>
        /// IL 生成模式
        /// <para>运行时动态生成 IL 代码</para>
        /// <para>特点：高性能、运行时生成、AOT不兼容</para>
        /// <para>适用：需要最高运行时性能</para>
        /// </summary>
        ILEmit = 5,

        /// <summary>
        /// 禁用生成
        /// <para>不生成任何序列化代码</para>
        /// <para>特点：类型标记但不参与序列化</para>
        /// <para>适用：占位符、未来扩展</para>
        /// </summary>
        Disabled = 6
    }

    /// <summary>
    /// GenerateMode 扩展方法
    /// </summary>
    public static class GenerateModeExtensions
    {
        #region 特性检查方法

        /// <summary>
        /// 检查是否为编译时生成
        /// </summary>
        /// <param name="mode">生成模式</param>
        /// <returns>如果是编译时生成返回 true</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsCompileTime(this GenerateMode mode)
        {
            return mode == GenerateMode.Auto || mode == GenerateMode.Hybrid;
        }

        /// <summary>
        /// 检查是否为运行时生成
        /// </summary>
        /// <param name="mode">生成模式</param>
        /// <returns>如果是运行时生成返回 true</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsRuntime(this GenerateMode mode)
        {
            return mode == GenerateMode.Reflection ||
                   mode == GenerateMode.Expression ||
                   mode == GenerateMode.ILEmit;
        }

        /// <summary>
        /// 检查是否支持 AOT 编译
        /// </summary>
        /// <param name="mode">生成模式</param>
        /// <returns>如果支持 AOT 返回 true</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SupportsAOT(this GenerateMode mode)
        {
            return mode == GenerateMode.Auto ||
                   mode == GenerateMode.Manual ||
                   mode == GenerateMode.Hybrid ||
                   mode == GenerateMode.Disabled;
        }

        /// <summary>
        /// 检查是否需要源代码生成器
        /// </summary>
        /// <param name="mode">生成模式</param>
        /// <returns>如果需要源代码生成器返回 true</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool RequiresSourceGenerator(this GenerateMode mode)
        {
            return mode == GenerateMode.Auto || mode == GenerateMode.Hybrid;
        }

        /// <summary>
        /// 检查是否允许自定义实现
        /// </summary>
        /// <param name="mode">生成模式</param>
        /// <returns>如果允许自定义实现返回 true</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AllowsCustomImplementation(this GenerateMode mode)
        {
            return mode == GenerateMode.Manual || mode == GenerateMode.Hybrid;
        }

        /// <summary>
        /// 检查是否已禁用
        /// </summary>
        /// <param name="mode">生成模式</param>
        /// <returns>如果已禁用返回 true</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsDisabled(this GenerateMode mode)
        {
            return mode == GenerateMode.Disabled;
        }

        #endregion

        #region 信息获取方法

        /// <summary>
        /// 获取模式的中文描述
        /// </summary>
        /// <param name="mode">生成模式</param>
        /// <returns>中文描述</returns>
        public static string GetDescription(this GenerateMode mode)
        {
            return mode switch
            {
                GenerateMode.Auto => "自动生成",
                GenerateMode.Manual => "手动实现",
                GenerateMode.Hybrid => "混合模式",
                GenerateMode.Reflection => "反射模式",
                GenerateMode.Expression => "表达式树模式",
                GenerateMode.ILEmit => "IL生成模式",
                GenerateMode.Disabled => "已禁用",
                _ => "未知模式"
            };
        }

        /// <summary>
        /// 获取预估的性能等级 (1-5，5为最高)
        /// </summary>
        /// <param name="mode">生成模式</param>
        /// <returns>性能等级</returns>
        public static int GetPerformanceLevel(this GenerateMode mode)
        {
            return mode switch
            {
                GenerateMode.Auto => 5,
                GenerateMode.Manual => 5,
                GenerateMode.Hybrid => 5,
                GenerateMode.ILEmit => 4,
                GenerateMode.Expression => 3,
                GenerateMode.Reflection => 1,
                GenerateMode.Disabled => 0,
                _ => 0
            };
        }

        /// <summary>
        /// 获取预估的灵活性等级 (1-5，5为最高)
        /// </summary>
        /// <param name="mode">生成模式</param>
        /// <returns>灵活性等级</returns>
        public static int GetFlexibilityLevel(this GenerateMode mode)
        {
            return mode switch
            {
                GenerateMode.Manual => 5,
                GenerateMode.Reflection => 5,
                GenerateMode.Hybrid => 4,
                GenerateMode.Expression => 4,
                GenerateMode.ILEmit => 3,
                GenerateMode.Auto => 2,
                GenerateMode.Disabled => 0,
                _ => 0
            };
        }

        #endregion

        #region 推荐方法

        /// <summary>
        /// 获取推荐的生成模式
        /// </summary>
        /// <param name="requiresAOT">是否需要 AOT 支持</param>
        /// <param name="needsCustomization">是否需要自定义</param>
        /// <returns>推荐的生成模式</returns>
        public static GenerateMode GetRecommended(bool requiresAOT = true, bool needsCustomization = false)
        {
            if (needsCustomization)
            {
                return requiresAOT ? GenerateMode.Hybrid : GenerateMode.Manual;
            }
            return GenerateMode.Auto;
        }

        #endregion
    }
}
