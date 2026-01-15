// ==========================================================
// 文件名：SerializeConstructorAttribute.cs
// 命名空间: AFramework.Serialization
// 依赖: System
// ==========================================================

using System;
using System.Runtime.CompilerServices;

namespace AFramework.Serialization
{
    /// <summary>
    /// 序列化构造函数特性
    /// <para>标记用于反序列化的构造函数</para>
    /// </summary>
    /// <remarks>
    /// <para><b>使用示例：</b></para>
    /// <code>
    /// [Serializable]
    /// public class PlayerData
    /// {
    ///     public int Id { get; }
    ///     public string Name { get; }
    ///     
    ///     [SerializeConstructor]
    ///     public PlayerData(int id, string name)
    ///     {
    ///         Id = id;
    ///         Name = name;
    ///     }
    /// }
    /// 
    /// // 参数名称映射
    /// [Serializable]
    /// public class ConfigData
    /// {
    ///     public int Version { get; }
    ///     
    ///     [SerializeConstructor(ParameterNames = new[] { "ver" })]
    ///     public ConfigData(int ver)
    ///     {
    ///         Version = ver;
    ///     }
    /// }
    /// </code>
    /// </remarks>
    [AttributeUsage(
        AttributeTargets.Constructor,
        AllowMultiple = false,
        Inherited = false)]
    public sealed class SerializeConstructorAttribute : Attribute
    {
        #region 属性

        /// <summary>
        /// 获取或设置参数名称映射
        /// </summary>
        public string[] ParameterNames { get; set; }

        /// <summary>
        /// 获取或设置是否允许使用默认值
        /// <para>默认值：true</para>
        /// </summary>
        public bool AllowDefaults { get; set; } = true;

        /// <summary>
        /// 获取或设置是否为主构造函数
        /// <para>默认值：true</para>
        /// </summary>
        public bool IsPrimary { get; set; } = true;

        /// <summary>
        /// 获取或设置优先级（值越大优先级越高）
        /// <para>默认值：0</para>
        /// </summary>
        public int Priority { get; set; }

        /// <summary>
        /// 获取或设置是否严格匹配参数（区分大小写）
        /// <para>默认值：false</para>
        /// </summary>
        public bool StrictMatching { get; set; }

        /// <summary>
        /// 获取或设置描述信息
        /// </summary>
        public string Description { get; set; }

        #endregion

        #region 构造函数

        /// <summary>
        /// 初始化 <see cref="SerializeConstructorAttribute"/> 的新实例
        /// </summary>
        public SerializeConstructorAttribute() { }

        /// <summary>
        /// 初始化 <see cref="SerializeConstructorAttribute"/> 的新实例
        /// </summary>
        /// <param name="parameterNames">参数名称映射</param>
        public SerializeConstructorAttribute(params string[] parameterNames)
        {
            ParameterNames = parameterNames;
        }

        #endregion

        #region 公共方法

        /// <summary>
        /// 检查是否有参数名称映射
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool HasParameterNames()
        {
            return ParameterNames != null && ParameterNames.Length > 0;
        }

        /// <summary>
        /// 获取指定索引的参数名称
        /// </summary>
        /// <param name="index">参数索引</param>
        /// <param name="defaultName">默认名称</param>
        public string GetParameterName(int index, string defaultName)
        {
            if (ParameterNames == null || index < 0 || index >= ParameterNames.Length)
                return defaultName;

            var name = ParameterNames[index];
            return string.IsNullOrEmpty(name) ? defaultName : name;
        }

        /// <summary>
        /// 检查参数名称是否匹配
        /// </summary>
        /// <param name="parameterName">参数名称</param>
        /// <param name="memberName">成员名称</param>
        public bool MatchesParameter(string parameterName, string memberName)
        {
            if (string.IsNullOrEmpty(parameterName) || string.IsNullOrEmpty(memberName))
                return false;

            var comparison = StrictMatching
                ? StringComparison.Ordinal
                : StringComparison.OrdinalIgnoreCase;

            return string.Equals(parameterName, memberName, comparison);
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return HasParameterNames()
                ? $"[SerializeConstructor({string.Join(", ", ParameterNames)})]"
                : "[SerializeConstructor]";
        }

        #endregion

        #region 静态方法

        /// <summary>
        /// 创建带参数映射的构造函数特性
        /// </summary>
        public static SerializeConstructorAttribute WithParameters(params string[] parameterNames)
        {
            return new SerializeConstructorAttribute(parameterNames);
        }

        /// <summary>
        /// 创建严格匹配的构造函数特性
        /// </summary>
        public static SerializeConstructorAttribute Strict()
        {
            return new SerializeConstructorAttribute { StrictMatching = true };
        }

        /// <summary>
        /// 创建不允许默认值的构造函数特性
        /// </summary>
        public static SerializeConstructorAttribute NoDefaults()
        {
            return new SerializeConstructorAttribute { AllowDefaults = false };
        }

        #endregion
    }
}
