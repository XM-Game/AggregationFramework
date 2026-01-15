// ==========================================================
// 文件名：MemberInfo.cs
// 命名空间: AFramework.Serialization
// 依赖: System, System.Reflection
// ==========================================================

using System;
using System.Runtime.CompilerServices;

namespace AFramework.Serialization
{
    /// <summary>
    /// 序列化成员信息结构体
    /// <para>封装类型成员（字段/属性）的序列化元数据</para>
    /// <para>用于序列化器确定如何读写成员数据</para>
    /// </summary>
    /// <remarks>
    /// 使用示例:
    /// <code>
    /// // 从字段创建
    /// var memberInfo = SerializeMemberInfo.FromField(fieldInfo);
    /// 
    /// // 检查成员特性
    /// if (memberInfo.IsRequired)
    ///     // 必需字段处理
    /// 
    /// // 获取成员值
    /// object value = memberInfo.GetValue(instance);
    /// </code>
    /// </remarks>
    [Serializable]
    public readonly struct SerializeMemberInfo : IEquatable<SerializeMemberInfo>, IComparable<SerializeMemberInfo>
    {
        #region 字段

        private readonly string _name;
        private readonly string _serializedName;
        private readonly Type _memberType;
        private readonly Type _declaringType;
        private readonly int _order;
        private readonly int _key;
        private readonly MemberKind _kind;
        private readonly MemberFlags _flags;
        private readonly object _defaultValue;
        private readonly System.Reflection.MemberInfo _reflectionInfo;

        #endregion

        #region 构造函数

        /// <summary>
        /// 创建成员信息
        /// </summary>
        private SerializeMemberInfo(
            string name,
            string serializedName,
            Type memberType,
            Type declaringType,
            int order,
            int key,
            MemberKind kind,
            MemberFlags flags,
            object defaultValue,
            System.Reflection.MemberInfo reflectionInfo)
        {
            _name = name;
            _serializedName = serializedName ?? name;
            _memberType = memberType;
            _declaringType = declaringType;
            _order = order;
            _key = key;
            _kind = kind;
            _flags = flags;
            _defaultValue = defaultValue;
            _reflectionInfo = reflectionInfo;
        }

        #endregion

        #region 属性

        /// <summary>成员名称</summary>
        public string Name => _name;

        /// <summary>序列化名称 (可能与成员名称不同)</summary>
        public string SerializedName => _serializedName;

        /// <summary>成员类型</summary>
        public Type MemberType => _memberType;

        /// <summary>声明类型</summary>
        public Type DeclaringType => _declaringType;

        /// <summary>序列化顺序</summary>
        public int Order => _order;

        /// <summary>成员键 (用于版本容错模式)</summary>
        public int Key => _key;

        /// <summary>成员种类</summary>
        public MemberKind Kind => _kind;

        /// <summary>成员标志</summary>
        public MemberFlags Flags => _flags;

        /// <summary>默认值</summary>
        public object DefaultValue => _defaultValue;

        /// <summary>反射信息</summary>
        public System.Reflection.MemberInfo ReflectionInfo => _reflectionInfo;

        /// <summary>是否有效</summary>
        public bool IsValid => _name != null && _memberType != null;

        #endregion

        #region 标志属性

        /// <summary>是否为字段</summary>
        public bool IsField => _kind == MemberKind.Field;

        /// <summary>是否为属性</summary>
        public bool IsProperty => _kind == MemberKind.Property;

        /// <summary>是否可读</summary>
        public bool CanRead => (_flags & MemberFlags.CanRead) != 0;

        /// <summary>是否可写</summary>
        public bool CanWrite => (_flags & MemberFlags.CanWrite) != 0;

        /// <summary>是否为必需成员</summary>
        public bool IsRequired => (_flags & MemberFlags.IsRequired) != 0;

        /// <summary>是否忽略</summary>
        public bool IsIgnored => (_flags & MemberFlags.IsIgnored) != 0;

        /// <summary>是否为公共成员</summary>
        public bool IsPublic => (_flags & MemberFlags.IsPublic) != 0;

        /// <summary>是否为私有成员</summary>
        public bool IsPrivate => (_flags & MemberFlags.IsPrivate) != 0;

        /// <summary>是否为只读成员</summary>
        public bool IsReadOnly => (_flags & MemberFlags.IsReadOnly) != 0;

        /// <summary>是否有默认值</summary>
        public bool HasDefaultValue => (_flags & MemberFlags.HasDefaultValue) != 0;

        /// <summary>是否为可空类型</summary>
        public bool IsNullable => (_flags & MemberFlags.IsNullable) != 0;

        /// <summary>是否为集合类型</summary>
        public bool IsCollection => (_flags & MemberFlags.IsCollection) != 0;

        #endregion

        #region 工厂方法

        /// <summary>
        /// 从字段信息创建
        /// </summary>
        /// <param name="fieldInfo">字段信息</param>
        /// <param name="order">序列化顺序</param>
        /// <param name="key">成员键</param>
        public static SerializeMemberInfo FromField(System.Reflection.FieldInfo fieldInfo, int order = 0, int key = -1)
        {
            if (fieldInfo == null)
                return default;

            var flags = MemberFlags.CanRead | MemberFlags.CanWrite;
            if (fieldInfo.IsPublic) flags |= MemberFlags.IsPublic;
            if (fieldInfo.IsPrivate) flags |= MemberFlags.IsPrivate;
            if (fieldInfo.IsInitOnly) flags |= MemberFlags.IsReadOnly;
            if (IsNullableType(fieldInfo.FieldType)) flags |= MemberFlags.IsNullable;
            if (IsCollectionType(fieldInfo.FieldType)) flags |= MemberFlags.IsCollection;

            return new SerializeMemberInfo(
                name: fieldInfo.Name,
                serializedName: null,
                memberType: fieldInfo.FieldType,
                declaringType: fieldInfo.DeclaringType,
                order: order,
                key: key >= 0 ? key : order,
                kind: MemberKind.Field,
                flags: flags,
                defaultValue: GetDefaultValue(fieldInfo.FieldType),
                reflectionInfo: fieldInfo
            );
        }

        /// <summary>
        /// 从属性信息创建
        /// </summary>
        /// <param name="propertyInfo">属性信息</param>
        /// <param name="order">序列化顺序</param>
        /// <param name="key">成员键</param>
        public static SerializeMemberInfo FromProperty(System.Reflection.PropertyInfo propertyInfo, int order = 0, int key = -1)
        {
            if (propertyInfo == null)
                return default;

            var flags = MemberFlags.None;
            if (propertyInfo.CanRead) flags |= MemberFlags.CanRead;
            if (propertyInfo.CanWrite) flags |= MemberFlags.CanWrite;
            else flags |= MemberFlags.IsReadOnly;

            var getMethod = propertyInfo.GetGetMethod(true);
            if (getMethod != null)
            {
                if (getMethod.IsPublic) flags |= MemberFlags.IsPublic;
                if (getMethod.IsPrivate) flags |= MemberFlags.IsPrivate;
            }

            if (IsNullableType(propertyInfo.PropertyType)) flags |= MemberFlags.IsNullable;
            if (IsCollectionType(propertyInfo.PropertyType)) flags |= MemberFlags.IsCollection;

            return new SerializeMemberInfo(
                name: propertyInfo.Name,
                serializedName: null,
                memberType: propertyInfo.PropertyType,
                declaringType: propertyInfo.DeclaringType,
                order: order,
                key: key >= 0 ? key : order,
                kind: MemberKind.Property,
                flags: flags,
                defaultValue: GetDefaultValue(propertyInfo.PropertyType),
                reflectionInfo: propertyInfo
            );
        }

        /// <summary>
        /// 创建自定义成员信息
        /// </summary>
        public static SerializeMemberInfo Create(
            string name,
            Type memberType,
            int order = 0,
            int key = -1,
            string serializedName = null,
            MemberFlags flags = MemberFlags.CanRead | MemberFlags.CanWrite,
            object defaultValue = null)
        {
            return new SerializeMemberInfo(
                name: name,
                serializedName: serializedName,
                memberType: memberType,
                declaringType: null,
                order: order,
                key: key >= 0 ? key : order,
                kind: MemberKind.Custom,
                flags: flags,
                defaultValue: defaultValue ?? GetDefaultValue(memberType),
                reflectionInfo: null
            );
        }

        /// <summary>空成员信息</summary>
        public static SerializeMemberInfo Empty => default;

        #endregion

        #region 值操作方法

        /// <summary>
        /// 获取成员值
        /// </summary>
        /// <param name="instance">对象实例</param>
        public object GetValue(object instance)
        {
            if (instance == null || _reflectionInfo == null)
                return _defaultValue;

            return _kind switch
            {
                MemberKind.Field => ((System.Reflection.FieldInfo)_reflectionInfo).GetValue(instance),
                MemberKind.Property => ((System.Reflection.PropertyInfo)_reflectionInfo).GetValue(instance),
                _ => _defaultValue
            };
        }

        /// <summary>
        /// 设置成员值
        /// </summary>
        /// <param name="instance">对象实例</param>
        /// <param name="value">要设置的值</param>
        public void SetValue(object instance, object value)
        {
            if (instance == null || _reflectionInfo == null || !CanWrite)
                return;

            switch (_kind)
            {
                case MemberKind.Field:
                    ((System.Reflection.FieldInfo)_reflectionInfo).SetValue(instance, value);
                    break;
                case MemberKind.Property:
                    ((System.Reflection.PropertyInfo)_reflectionInfo).SetValue(instance, value);
                    break;
            }
        }

        /// <summary>
        /// 检查值是否为默认值
        /// </summary>
        /// <param name="value">要检查的值</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsDefaultValue(object value)
        {
            if (value == null)
                return _defaultValue == null;
            return value.Equals(_defaultValue);
        }

        #endregion

        #region With 方法

        /// <summary>修改序列化名称</summary>
        public SerializeMemberInfo WithSerializedName(string serializedName) =>
            new SerializeMemberInfo(_name, serializedName, _memberType, _declaringType, _order, _key, _kind, _flags, _defaultValue, _reflectionInfo);

        /// <summary>修改顺序</summary>
        public SerializeMemberInfo WithOrder(int order) =>
            new SerializeMemberInfo(_name, _serializedName, _memberType, _declaringType, order, _key, _kind, _flags, _defaultValue, _reflectionInfo);

        /// <summary>修改键</summary>
        public SerializeMemberInfo WithKey(int key) =>
            new SerializeMemberInfo(_name, _serializedName, _memberType, _declaringType, _order, key, _kind, _flags, _defaultValue, _reflectionInfo);

        /// <summary>添加标志</summary>
        public SerializeMemberInfo AddFlags(MemberFlags flags) =>
            new SerializeMemberInfo(_name, _serializedName, _memberType, _declaringType, _order, _key, _kind, _flags | flags, _defaultValue, _reflectionInfo);

        /// <summary>移除标志</summary>
        public SerializeMemberInfo RemoveFlags(MemberFlags flags) =>
            new SerializeMemberInfo(_name, _serializedName, _memberType, _declaringType, _order, _key, _kind, _flags & ~flags, _defaultValue, _reflectionInfo);

        /// <summary>设置为必需</summary>
        public SerializeMemberInfo AsRequired() => AddFlags(MemberFlags.IsRequired);

        /// <summary>设置为忽略</summary>
        public SerializeMemberInfo AsIgnored() => AddFlags(MemberFlags.IsIgnored);

        #endregion

        #region 辅助方法

        private static bool IsNullableType(Type type)
        {
            return !type.IsValueType || (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>));
        }

        private static bool IsCollectionType(Type type)
        {
            if (type.IsArray) return true;
            if (!type.IsGenericType) return false;
            var genericDef = type.GetGenericTypeDefinition();
            return genericDef == typeof(System.Collections.Generic.List<>) ||
                   genericDef == typeof(System.Collections.Generic.Dictionary<,>) ||
                   genericDef == typeof(System.Collections.Generic.HashSet<>);
        }

        private static object GetDefaultValue(Type type)
        {
            if (type == null) return null;
            if (!type.IsValueType) return null;
            return Activator.CreateInstance(type);
        }

        #endregion

        #region IComparable 实现

        /// <summary>比较顺序</summary>
        public int CompareTo(SerializeMemberInfo other)
        {
            int orderCompare = _order.CompareTo(other._order);
            if (orderCompare != 0) return orderCompare;
            return string.Compare(_name, other._name, StringComparison.Ordinal);
        }

        #endregion

        #region IEquatable 实现

        /// <summary>判断是否相等</summary>
        public bool Equals(SerializeMemberInfo other)
        {
            return _name == other._name &&
                   _memberType == other._memberType &&
                   _declaringType == other._declaringType;
        }

        /// <summary>判断是否相等</summary>
        public override bool Equals(object obj) => obj is SerializeMemberInfo other && Equals(other);

        /// <summary>获取哈希码</summary>
        public override int GetHashCode() => HashCode.Combine(_name, _memberType, _declaringType);

        /// <summary>相等运算符</summary>
        public static bool operator ==(SerializeMemberInfo left, SerializeMemberInfo right) => left.Equals(right);

        /// <summary>不等运算符</summary>
        public static bool operator !=(SerializeMemberInfo left, SerializeMemberInfo right) => !left.Equals(right);

        #endregion

        #region 字符串表示

        /// <summary>获取字符串表示</summary>
        public override string ToString()
        {
            return $"Member({_name}: {_memberType?.Name}, Order={_order}, Key={_key})";
        }

        #endregion
    }

    #region 成员种类枚举

    /// <summary>
    /// 成员种类枚举
    /// </summary>
    public enum MemberKind : byte
    {
        /// <summary>未知</summary>
        Unknown = 0,

        /// <summary>字段</summary>
        Field = 1,

        /// <summary>属性</summary>
        Property = 2,

        /// <summary>自定义</summary>
        Custom = 3
    }

    #endregion

    #region 成员标志枚举

    /// <summary>
    /// 成员标志枚举
    /// </summary>
    [Flags]
    public enum MemberFlags : uint
    {
        /// <summary>无标志</summary>
        None = 0,

        /// <summary>可读</summary>
        CanRead = 1 << 0,

        /// <summary>可写</summary>
        CanWrite = 1 << 1,

        /// <summary>必需</summary>
        IsRequired = 1 << 2,

        /// <summary>忽略</summary>
        IsIgnored = 1 << 3,

        /// <summary>公共</summary>
        IsPublic = 1 << 4,

        /// <summary>私有</summary>
        IsPrivate = 1 << 5,

        /// <summary>只读</summary>
        IsReadOnly = 1 << 6,

        /// <summary>有默认值</summary>
        HasDefaultValue = 1 << 7,

        /// <summary>可空</summary>
        IsNullable = 1 << 8,

        /// <summary>集合</summary>
        IsCollection = 1 << 9,

        /// <summary>已弃用</summary>
        IsDeprecated = 1 << 10,

        /// <summary>内部使用</summary>
        IsInternal = 1 << 11
    }

    #endregion
}
