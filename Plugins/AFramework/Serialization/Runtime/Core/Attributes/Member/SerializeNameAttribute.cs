// ==========================================================
// 文件名：SerializeNameAttribute.cs
// 命名空间: AFramework.Serialization
// 依赖: System
// ==========================================================

using System;
using System.Runtime.CompilerServices;

namespace AFramework.Serialization
{
    /// <summary>
    /// 序列化名称特性
    /// <para>指定字段或属性在序列化时使用的名称</para>
    /// </summary>
    /// <remarks>
    /// <para><b>使用示例：</b></para>
    /// <code>
    /// [Serializable]
    /// public class PlayerData
    /// {
    ///     [SerializeName("id")]
    ///     public int PlayerId;
    ///     
    ///     [SerializeName("n")]
    ///     public string Name;
    ///     
    ///     [SerializeName("hp", Aliases = new[] { "health", "hitPoints" })]
    ///     public float Health;
    ///     
    ///     [SerializeName(Index = 0)]
    ///     public int Level;
    /// }
    /// </code>
    /// </remarks>
    [AttributeUsage(
        AttributeTargets.Field | AttributeTargets.Property,
        AllowMultiple = false,
        Inherited = true)]
    public sealed class SerializeNameAttribute : Attribute
    {
        #region 常量

        /// <summary>
        /// 未指定索引
        /// </summary>
        public const int UnspecifiedIndex = -1;

        #endregion

        #region 属性

        /// <summary>
        /// 获取序列化名称
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// 获取或设置数字索引
        /// <para>默认值：-1（未指定）</para>
        /// </summary>
        public int Index { get; set; } = UnspecifiedIndex;

        /// <summary>
        /// 获取或设置别名列表（用于版本迁移）
        /// </summary>
        public string[] Aliases { get; set; }

        /// <summary>
        /// 获取或设置是否区分大小写
        /// <para>默认值：true</para>
        /// </summary>
        public bool CaseSensitive { get; set; } = true;

        /// <summary>
        /// 获取或设置是否为已弃用名称
        /// <para>默认值：false</para>
        /// </summary>
        public bool IsDeprecated { get; set; }

        /// <summary>
        /// 获取或设置弃用消息
        /// </summary>
        public string DeprecationMessage { get; set; }

        /// <summary>
        /// 获取或设置命名策略
        /// <para>默认值：<see cref="NamingStrategy.None"/></para>
        /// </summary>
        public NamingStrategy NamingStrategy { get; set; } = NamingStrategy.None;

        #endregion

        #region 构造函数

        /// <summary>
        /// 初始化 <see cref="SerializeNameAttribute"/> 的新实例
        /// </summary>
        /// <param name="name">序列化名称</param>
        /// <exception cref="ArgumentNullException">名称为 null</exception>
        /// <exception cref="ArgumentException">名称为空字符串</exception>
        public SerializeNameAttribute(string name)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("序列化名称不能为空", nameof(name));

            Name = name;
        }

        /// <summary>
        /// 初始化 <see cref="SerializeNameAttribute"/> 的新实例
        /// </summary>
        /// <param name="name">序列化名称</param>
        /// <param name="index">数字索引</param>
        public SerializeNameAttribute(string name, int index) : this(name)
        {
            Index = index;
        }

        #endregion

        #region 公共方法

        /// <summary>
        /// 检查是否有数字索引
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool HasIndex() => Index != UnspecifiedIndex;

        /// <summary>
        /// 检查是否有别名
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool HasAliases() => Aliases != null && Aliases.Length > 0;

        /// <summary>
        /// 检查名称是否匹配
        /// </summary>
        /// <param name="name">要检查的名称</param>
        public bool Matches(string name)
        {
            if (string.IsNullOrEmpty(name))
                return false;

            var comparison = CaseSensitive
                ? StringComparison.Ordinal
                : StringComparison.OrdinalIgnoreCase;

            // 检查主名称
            if (string.Equals(Name, name, comparison))
                return true;

            // 检查别名
            if (Aliases != null)
            {
                foreach (var alias in Aliases)
                {
                    if (string.Equals(alias, name, comparison))
                        return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 检查索引是否匹配
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool MatchesIndex(int index)
        {
            return Index != UnspecifiedIndex && Index == index;
        }

        /// <summary>
        /// 获取所有可能的名称（包括别名）
        /// </summary>
        public string[] GetAllNames()
        {
            if (!HasAliases())
                return new[] { Name };

            var result = new string[1 + Aliases.Length];
            result[0] = Name;
            Array.Copy(Aliases, 0, result, 1, Aliases.Length);
            return result;
        }

        /// <summary>
        /// 应用命名策略转换名称
        /// </summary>
        /// <param name="originalName">原始名称</param>
        public string ApplyNamingStrategy(string originalName)
        {
            if (!string.IsNullOrEmpty(Name))
                return Name;

            return NamingStrategy.Transform(originalName);
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return HasIndex()
                ? $"[SerializeName(\"{Name}\", {Index})]"
                : $"[SerializeName(\"{Name}\")]";
        }

        #endregion
    }
}
