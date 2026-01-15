// ==========================================================
// 文件名：UnionBaseAttribute.cs
// 命名空间: AFramework.Serialization
// 依赖: System
// ==========================================================

using System;

namespace AFramework.Serialization
{
    /// <summary>
    /// 联合类型基类特性
    /// <para>标记类型为联合类型的基类型</para>
    /// <para>可选特性，用于提供额外的联合类型配置</para>
    /// </summary>
    /// <remarks>
    /// <para><b>使用示例：</b></para>
    /// <code>
    /// // 基础配置
    /// [UnionBase]
    /// [UnionType(0, typeof(TypeA))]
    /// [UnionType(1, typeof(TypeB))]
    /// public interface IMyUnion { }
    /// 
    /// // 自定义回退行为
    /// [UnionBase(FallbackBehavior = UnionFallbackBehavior.ReturnNull)]
    /// [UnionType(0, typeof(PlayerEntity))]
    /// [UnionType(1, typeof(EnemyEntity))]
    /// public abstract class EntityBase { }
    /// 
    /// // 使用字符串标签
    /// [UnionBase(UseStringTags = true)]
    /// [UnionType("player", typeof(PlayerData))]
    /// [UnionType("enemy", typeof(EnemyData))]
    /// public interface IGameEntity { }
    /// </code>
    /// 
    /// <para><b>配置选项：</b></para>
    /// <list type="bullet">
    ///   <item>FallbackBehavior - 未知类型的处理策略</item>
    ///   <item>AllowNull - 是否允许 null 值</item>
    ///   <item>UseStringTags - 是否使用字符串标签</item>
    ///   <item>EnableTypeCache - 是否启用类型缓存</item>
    /// </list>
    /// </remarks>
    [AttributeUsage(
        AttributeTargets.Interface | AttributeTargets.Class,
        AllowMultiple = false,
        Inherited = false)]
    public sealed class UnionBaseAttribute : Attribute
    {
        #region 属性

        /// <summary>
        /// 获取或设置未知类型的回退行为
        /// <para>默认值：<see cref="UnionFallbackBehavior.ThrowException"/></para>
        /// </summary>
        public UnionFallbackBehavior FallbackBehavior { get; set; } = UnionFallbackBehavior.ThrowException;

        /// <summary>
        /// 获取或设置是否允许 null 值
        /// <para>默认值：true</para>
        /// </summary>
        public bool AllowNull { get; set; } = true;

        /// <summary>
        /// 获取或设置是否使用字符串标签
        /// <para>默认值：false（使用整数 ID）</para>
        /// </summary>
        /// <remarks>
        /// 使用字符串标签可提高可读性，但会增加序列化数据大小。
        /// </remarks>
        public bool UseStringTags { get; set; }

        /// <summary>
        /// 获取或设置是否启用类型缓存
        /// <para>默认值：true</para>
        /// </summary>
        /// <remarks>
        /// 启用后将缓存类型查找结果，提升反序列化性能。
        /// </remarks>
        public bool EnableTypeCache { get; set; } = true;

        /// <summary>
        /// 获取或设置回退类型
        /// <para>仅当 FallbackBehavior 为 UseFallbackType 时有效</para>
        /// </summary>
        public Type FallbackType { get; set; }

        #endregion

        #region 构造函数

        /// <summary>
        /// 初始化 <see cref="UnionBaseAttribute"/> 的新实例
        /// </summary>
        public UnionBaseAttribute()
        {
        }

        /// <summary>
        /// 初始化 <see cref="UnionBaseAttribute"/> 的新实例
        /// </summary>
        /// <param name="fallbackBehavior">回退行为</param>
        public UnionBaseAttribute(UnionFallbackBehavior fallbackBehavior)
        {
            FallbackBehavior = fallbackBehavior;
        }

        #endregion

        #region 公共方法

        /// <summary>
        /// 检查是否有回退类型
        /// </summary>
        public bool HasFallbackType() => FallbackType != null;

        /// <summary>
        /// 验证配置是否有效
        /// </summary>
        public bool Validate(out string error)
        {
            if (FallbackBehavior == UnionFallbackBehavior.UseFallbackType && FallbackType == null)
            {
                error = "使用 UseFallbackType 行为时必须指定 FallbackType";
                return false;
            }

            error = null;
            return true;
        }

        /// <summary>
        /// 获取配置摘要信息
        /// </summary>
        public string GetSummary()
        {
            var tags = UseStringTags ? "StringTags" : "IntIds";
            return $"Fallback={FallbackBehavior}, {tags}, Cache={EnableTypeCache}";
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"[UnionBase(Fallback={FallbackBehavior})]";
        }

        #endregion
    }
}
