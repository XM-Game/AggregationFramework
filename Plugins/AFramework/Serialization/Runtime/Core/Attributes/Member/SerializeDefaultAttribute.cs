// ==========================================================
// 文件名：SerializeDefaultAttribute.cs
// 命名空间: AFramework.Serialization
// 依赖: System
// ==========================================================

using System;
using System.Runtime.CompilerServices;

namespace AFramework.Serialization
{
    /// <summary>
    /// 序列化默认值特性
    /// <para>指定字段或属性的默认值，用于反序列化时缺失字段的处理</para>
    /// </summary>
    /// <remarks>
    /// <para><b>使用示例：</b></para>
    /// <code>
    /// [Serializable]
    /// public class PlayerData
    /// {
    ///     [SerializeDefault("Unknown")]
    ///     public string Name;
    ///     
    ///     [SerializeDefault(1)]
    ///     public int Level;
    ///     
    ///     [SerializeDefault(100.0f)]
    ///     public float Health;
    ///     
    ///     [SerializeDefault(FactoryMethod = nameof(GetDefaultItems))]
    ///     public List&lt;int&gt; Items;
    ///     
    ///     private static List&lt;int&gt; GetDefaultItems() => new List&lt;int&gt; { 1, 2, 3 };
    /// }
    /// </code>
    /// </remarks>
    [AttributeUsage(
        AttributeTargets.Field | AttributeTargets.Property,
        AllowMultiple = false,
        Inherited = true)]
    public sealed class SerializeDefaultAttribute : Attribute
    {
        #region 属性

        /// <summary>
        /// 获取默认值
        /// </summary>
        public object Value { get; }

        /// <summary>
        /// 获取默认值来源
        /// </summary>
        public DefaultValueSource Source { get; private set; } = DefaultValueSource.Explicit;

        /// <summary>
        /// 获取或设置工厂方法名称
        /// </summary>
        public string FactoryMethod
        {
            get => _factoryMethod;
            set
            {
                _factoryMethod = value;
                if (!string.IsNullOrEmpty(value))
                    Source = DefaultValueSource.FactoryMethod;
            }
        }
        private string _factoryMethod;

        /// <summary>
        /// 获取或设置是否使用默认构造函数
        /// <para>默认值：false</para>
        /// </summary>
        public bool UseDefaultConstructor
        {
            get => _useDefaultConstructor;
            set
            {
                _useDefaultConstructor = value;
                if (value)
                    Source = DefaultValueSource.DefaultConstructor;
            }
        }
        private bool _useDefaultConstructor;

        /// <summary>
        /// 获取或设置条件属性名称
        /// </summary>
        public string Condition { get; set; }

        /// <summary>
        /// 获取或设置是否在序列化时跳过默认值
        /// <para>默认值：false</para>
        /// </summary>
        public bool SkipOnSerialize { get; set; }

        /// <summary>
        /// 获取或设置是否缓存默认值
        /// <para>默认值：true</para>
        /// </summary>
        public bool CacheValue { get; set; } = true;

        /// <summary>
        /// 获取或设置描述信息
        /// </summary>
        public string Description { get; set; }

        #endregion

        #region 构造函数

        /// <summary>
        /// 初始化 <see cref="SerializeDefaultAttribute"/> 的新实例（使用类型默认值）
        /// </summary>
        public SerializeDefaultAttribute()
        {
            Value = null;
            Source = DefaultValueSource.TypeDefault;
        }

        /// <summary>
        /// 初始化 <see cref="SerializeDefaultAttribute"/> 的新实例
        /// </summary>
        public SerializeDefaultAttribute(object value)
        {
            Value = value;
            Source = DefaultValueSource.Explicit;
        }

        /// <summary>
        /// 初始化 <see cref="SerializeDefaultAttribute"/> 的新实例
        /// </summary>
        public SerializeDefaultAttribute(bool value)
        {
            Value = value;
            Source = DefaultValueSource.Explicit;
        }

        /// <summary>
        /// 初始化 <see cref="SerializeDefaultAttribute"/> 的新实例
        /// </summary>
        public SerializeDefaultAttribute(int value)
        {
            Value = value;
            Source = DefaultValueSource.Explicit;
        }

        /// <summary>
        /// 初始化 <see cref="SerializeDefaultAttribute"/> 的新实例
        /// </summary>
        public SerializeDefaultAttribute(long value)
        {
            Value = value;
            Source = DefaultValueSource.Explicit;
        }

        /// <summary>
        /// 初始化 <see cref="SerializeDefaultAttribute"/> 的新实例
        /// </summary>
        public SerializeDefaultAttribute(float value)
        {
            Value = value;
            Source = DefaultValueSource.Explicit;
        }

        /// <summary>
        /// 初始化 <see cref="SerializeDefaultAttribute"/> 的新实例
        /// </summary>
        public SerializeDefaultAttribute(double value)
        {
            Value = value;
            Source = DefaultValueSource.Explicit;
        }

        /// <summary>
        /// 初始化 <see cref="SerializeDefaultAttribute"/> 的新实例
        /// </summary>
        public SerializeDefaultAttribute(string value)
        {
            Value = value;
            Source = DefaultValueSource.Explicit;
        }

        /// <summary>
        /// 初始化 <see cref="SerializeDefaultAttribute"/> 的新实例
        /// </summary>
        public SerializeDefaultAttribute(char value)
        {
            Value = value;
            Source = DefaultValueSource.Explicit;
        }

        /// <summary>
        /// 初始化 <see cref="SerializeDefaultAttribute"/> 的新实例（枚举类型）
        /// </summary>
        /// <param name="enumType">枚举类型</param>
        /// <param name="enumValue">枚举值名称</param>
        public SerializeDefaultAttribute(Type enumType, string enumValue)
        {
            if (enumType == null)
                throw new ArgumentNullException(nameof(enumType));
            if (!enumType.IsEnum)
                throw new ArgumentException("类型必须是枚举类型", nameof(enumType));

            Value = Enum.Parse(enumType, enumValue);
            Source = DefaultValueSource.Explicit;
        }

        #endregion

        #region 公共方法

        /// <summary>
        /// 检查是否有显式默认值
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool HasExplicitValue()
        {
            return Source == DefaultValueSource.Explicit && Value != null;
        }

        /// <summary>
        /// 检查是否有工厂方法
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool HasFactoryMethod() => !string.IsNullOrEmpty(_factoryMethod);

        /// <summary>
        /// 检查是否为条件默认值
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsConditional() => !string.IsNullOrEmpty(Condition);

        /// <summary>
        /// 检查值是否等于默认值
        /// </summary>
        public bool IsDefaultValue(object value)
        {
            if (Value == null && value == null)
                return true;
            if (Value == null || value == null)
                return false;
            return Value.Equals(value);
        }

        /// <summary>
        /// 获取默认值（带类型转换）
        /// </summary>
        public T GetValue<T>()
        {
            if (Value == null)
                return default;

            if (Value is T typedValue)
                return typedValue;

            try
            {
                return (T)Convert.ChangeType(Value, typeof(T));
            }
            catch
            {
                return default;
            }
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return Source switch
            {
                DefaultValueSource.Explicit => $"[SerializeDefault({FormatValue(Value)})]",
                DefaultValueSource.FactoryMethod => $"[SerializeDefault(Factory = \"{_factoryMethod}\")]",
                DefaultValueSource.DefaultConstructor => "[SerializeDefault(UseDefaultConstructor = true)]",
                _ => "[SerializeDefault]"
            };
        }

        #endregion

        #region 私有方法

        private static string FormatValue(object value)
        {
            if (value == null) return "null";
            if (value is string str) return $"\"{str}\"";
            if (value is char c) return $"'{c}'";
            if (value is bool b) return b ? "true" : "false";
            return value.ToString();
        }

        #endregion
    }
}
