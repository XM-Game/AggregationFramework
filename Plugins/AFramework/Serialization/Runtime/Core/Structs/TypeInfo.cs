// ==========================================================
// 文件名：TypeInfo.cs
// 命名空间: AFramework.Serialization
// 依赖: System, System.Reflection
// ==========================================================

using System;
using System.Runtime.CompilerServices;

namespace AFramework.Serialization
{
    /// <summary>
    /// 序列化类型信息结构体
    /// <para>封装类型的序列化元数据，用于类型识别和格式化器查找</para>
    /// <para>支持泛型类型、数组类型、可空类型等复杂类型</para>
    /// </summary>
    /// <remarks>
    /// 使用示例:
    /// <code>
    /// // 从类型创建
    /// var typeInfo = SerializeTypeInfo.FromType(typeof(Player));
    /// 
    /// // 检查类型特性
    /// if (typeInfo.IsCollection)
    ///     // 处理集合类型
    /// 
    /// // 获取类型码
    /// byte typeCode = typeInfo.TypeCode;
    /// </code>
    /// </remarks>
    [Serializable]
    public readonly struct SerializeTypeInfo : IEquatable<SerializeTypeInfo>
    {
        #region 字段

        private readonly Type _type;
        private readonly int _typeId;
        private readonly byte _typeCode;
        private readonly TypeCategory _category;
        private readonly TypeFlags _flags;
        private readonly Type _elementType;
        private readonly Type[] _genericArguments;

        #endregion

        #region 构造函数

        /// <summary>
        /// 创建类型信息
        /// </summary>
        private SerializeTypeInfo(
            Type type,
            int typeId,
            byte typeCode,
            TypeCategory category,
            TypeFlags flags,
            Type elementType,
            Type[] genericArguments)
        {
            _type = type;
            _typeId = typeId;
            _typeCode = typeCode;
            _category = category;
            _flags = flags;
            _elementType = elementType;
            _genericArguments = genericArguments;
        }

        #endregion

        #region 属性

        /// <summary>原始类型</summary>
        public Type Type => _type;

        /// <summary>类型 ID (用于序列化)</summary>
        public int TypeId => _typeId;

        /// <summary>类型码 (内置类型快速识别)</summary>
        public byte TypeCode => _typeCode;

        /// <summary>类型分类</summary>
        public TypeCategory Category => _category;

        /// <summary>类型标志</summary>
        public TypeFlags Flags => _flags;

        /// <summary>元素类型 (数组/集合)</summary>
        public Type ElementType => _elementType;

        /// <summary>泛型参数</summary>
        public ReadOnlySpan<Type> GenericArguments => _genericArguments ?? Array.Empty<Type>();

        /// <summary>类型名称</summary>
        public string Name => _type?.Name ?? string.Empty;

        /// <summary>完整类型名称</summary>
        public string FullName => _type?.FullName ?? string.Empty;

        /// <summary>程序集限定名称</summary>
        public string AssemblyQualifiedName => _type?.AssemblyQualifiedName ?? string.Empty;

        #endregion

        #region 类型特性属性

        /// <summary>是否为基元类型</summary>
        public bool IsPrimitive => (_flags & TypeFlags.IsPrimitive) != 0;

        /// <summary>是否为值类型</summary>
        public bool IsValueType => (_flags & TypeFlags.IsValueType) != 0;

        /// <summary>是否为引用类型</summary>
        public bool IsReferenceType => !IsValueType;

        /// <summary>是否为枚举类型</summary>
        public bool IsEnum => (_flags & TypeFlags.IsEnum) != 0;

        /// <summary>是否为数组类型</summary>
        public bool IsArray => (_flags & TypeFlags.IsArray) != 0;

        /// <summary>是否为集合类型</summary>
        public bool IsCollection => (_flags & TypeFlags.IsCollection) != 0;

        /// <summary>是否为字典类型</summary>
        public bool IsDictionary => (_flags & TypeFlags.IsDictionary) != 0;

        /// <summary>是否为泛型类型</summary>
        public bool IsGeneric => (_flags & TypeFlags.IsGeneric) != 0;

        /// <summary>是否为可空类型</summary>
        public bool IsNullable => (_flags & TypeFlags.IsNullable) != 0;

        /// <summary>是否为字符串类型</summary>
        public bool IsString => _category == TypeCategory.String;

        /// <summary>是否为接口类型</summary>
        public bool IsInterface => (_flags & TypeFlags.IsInterface) != 0;

        /// <summary>是否为抽象类型</summary>
        public bool IsAbstract => (_flags & TypeFlags.IsAbstract) != 0;

        /// <summary>是否为密封类型</summary>
        public bool IsSealed => (_flags & TypeFlags.IsSealed) != 0;

        /// <summary>是否支持 Blittable (可直接内存复制)</summary>
        public bool IsBlittable => (_flags & TypeFlags.IsBlittable) != 0;

        /// <summary>是否为非托管类型</summary>
        public bool IsUnmanaged => (_flags & TypeFlags.IsUnmanaged) != 0;

        /// <summary>是否有无参构造函数</summary>
        public bool HasDefaultConstructor => (_flags & TypeFlags.HasDefaultConstructor) != 0;

        /// <summary>是否有自定义格式化器</summary>
        public bool HasCustomFormatter => (_flags & TypeFlags.HasCustomFormatter) != 0;

        #endregion

        #region 工厂方法

        /// <summary>
        /// 从类型创建类型信息
        /// </summary>
        /// <param name="type">类型</param>
        public static SerializeTypeInfo FromType(Type type)
        {
            if (type == null)
                return default;

            var category = GetTypeCategory(type);
            var flags = GetTypeFlags(type);
            var typeCode = GetTypeCode(type);
            var typeId = type.GetHashCode();
            var elementType = GetElementType(type);
            var genericArgs = type.IsGenericType ? type.GetGenericArguments() : null;

            return new SerializeTypeInfo(type, typeId, typeCode, category, flags, elementType, genericArgs);
        }

        /// <summary>
        /// 从类型创建类型信息 (泛型版本)
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SerializeTypeInfo FromType<T>()
        {
            return FromType(typeof(T));
        }

        /// <summary>
        /// 空类型信息
        /// </summary>
        public static SerializeTypeInfo Empty => default;

        #endregion

        #region 辅助方法

        /// <summary>
        /// 获取类型分类
        /// </summary>
        private static TypeCategory GetTypeCategory(Type type)
        {
            if (type == typeof(bool)) return TypeCategory.Boolean;
            if (type == typeof(byte) || type == typeof(sbyte)) return TypeCategory.Integer;
            if (type == typeof(short) || type == typeof(ushort)) return TypeCategory.Integer;
            if (type == typeof(int) || type == typeof(uint)) return TypeCategory.Integer;
            if (type == typeof(long) || type == typeof(ulong)) return TypeCategory.Integer;
            if (type == typeof(float) || type == typeof(double)) return TypeCategory.Float;
            if (type == typeof(decimal)) return TypeCategory.Decimal;
            if (type == typeof(char)) return TypeCategory.Char;
            if (type == typeof(string)) return TypeCategory.String;
            if (type == typeof(DateTime)) return TypeCategory.DateTime;
            if (type == typeof(DateTimeOffset)) return TypeCategory.DateTime;
            if (type == typeof(TimeSpan)) return TypeCategory.TimeSpan;
            if (type == typeof(Guid)) return TypeCategory.Guid;
            if (type == typeof(byte[])) return TypeCategory.ByteArray;
            if (type.IsEnum) return TypeCategory.Enum;
            if (type.IsArray) return TypeCategory.Array;
            if (IsCollectionType(type)) return TypeCategory.Collection;
            if (IsDictionaryType(type)) return TypeCategory.Dictionary;
            if (type.IsValueType) return TypeCategory.Struct;
            return TypeCategory.Object;
        }

        /// <summary>
        /// 获取类型标志
        /// </summary>
        private static TypeFlags GetTypeFlags(Type type)
        {
            var flags = TypeFlags.None;

            if (type.IsPrimitive) flags |= TypeFlags.IsPrimitive;
            if (type.IsValueType) flags |= TypeFlags.IsValueType;
            if (type.IsEnum) flags |= TypeFlags.IsEnum;
            if (type.IsArray) flags |= TypeFlags.IsArray;
            if (type.IsGenericType) flags |= TypeFlags.IsGeneric;
            if (type.IsInterface) flags |= TypeFlags.IsInterface;
            if (type.IsAbstract) flags |= TypeFlags.IsAbstract;
            if (type.IsSealed) flags |= TypeFlags.IsSealed;

            if (IsCollectionType(type)) flags |= TypeFlags.IsCollection;
            if (IsDictionaryType(type)) flags |= TypeFlags.IsDictionary;
            if (IsNullableType(type)) flags |= TypeFlags.IsNullable;
            if (CheckHasDefaultConstructor(type)) flags |= TypeFlags.HasDefaultConstructor;
            if (IsBlittableType(type)) flags |= TypeFlags.IsBlittable;

            return flags;
        }

        /// <summary>
        /// 获取类型码
        /// </summary>
        private static byte GetTypeCode(Type type)
        {
            if (type == typeof(bool)) return 1;
            if (type == typeof(byte)) return 2;
            if (type == typeof(sbyte)) return 3;
            if (type == typeof(short)) return 4;
            if (type == typeof(ushort)) return 5;
            if (type == typeof(int)) return 6;
            if (type == typeof(uint)) return 7;
            if (type == typeof(long)) return 8;
            if (type == typeof(ulong)) return 9;
            if (type == typeof(float)) return 10;
            if (type == typeof(double)) return 11;
            if (type == typeof(decimal)) return 12;
            if (type == typeof(char)) return 13;
            if (type == typeof(string)) return 14;
            if (type == typeof(DateTime)) return 15;
            if (type == typeof(DateTimeOffset)) return 16;
            if (type == typeof(TimeSpan)) return 17;
            if (type == typeof(Guid)) return 18;
            if (type == typeof(byte[])) return 19;
            return 0; // 自定义类型
        }

        /// <summary>
        /// 获取元素类型
        /// </summary>
        private static Type GetElementType(Type type)
        {
            if (type.IsArray)
                return type.GetElementType();

            if (type.IsGenericType)
            {
                var genericDef = type.GetGenericTypeDefinition();
                if (genericDef == typeof(Nullable<>))
                    return type.GetGenericArguments()[0];
            }

            return null;
        }

        /// <summary>
        /// 检查是否为集合类型
        /// </summary>
        private static bool IsCollectionType(Type type)
        {
            if (type.IsArray) return true;
            if (!type.IsGenericType) return false;

            var genericDef = type.GetGenericTypeDefinition();
            return genericDef == typeof(System.Collections.Generic.List<>) ||
                   genericDef == typeof(System.Collections.Generic.HashSet<>) ||
                   genericDef == typeof(System.Collections.Generic.Queue<>) ||
                   genericDef == typeof(System.Collections.Generic.Stack<>) ||
                   genericDef == typeof(System.Collections.Generic.LinkedList<>);
        }

        /// <summary>
        /// 检查是否为字典类型
        /// </summary>
        private static bool IsDictionaryType(Type type)
        {
            if (!type.IsGenericType) return false;

            var genericDef = type.GetGenericTypeDefinition();
            return genericDef == typeof(System.Collections.Generic.Dictionary<,>) ||
                   genericDef == typeof(System.Collections.Generic.SortedDictionary<,>);
        }

        /// <summary>
        /// 检查是否为可空类型
        /// </summary>
        private static bool IsNullableType(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        /// <summary>
        /// 检查是否有默认构造函数
        /// </summary>
        private static bool CheckHasDefaultConstructor(Type type)
        {
            if (type.IsValueType) return true;
            return type.GetConstructor(Type.EmptyTypes) != null;
        }

        /// <summary>
        /// 检查是否为 Blittable 类型
        /// </summary>
        private static bool IsBlittableType(Type type)
        {
            if (!type.IsValueType) return false;
            if (type.IsPrimitive) return true;
            if (type.IsEnum) return true;
            return false; // 简化实现，完整实现需要递归检查所有字段
        }

        #endregion

        #region IEquatable 实现

        /// <summary>判断是否相等</summary>
        public bool Equals(SerializeTypeInfo other)
        {
            return _type == other._type;
        }

        /// <summary>判断是否相等</summary>
        public override bool Equals(object obj) => obj is SerializeTypeInfo other && Equals(other);

        /// <summary>获取哈希码</summary>
        public override int GetHashCode() => _type?.GetHashCode() ?? 0;

        /// <summary>相等运算符</summary>
        public static bool operator ==(SerializeTypeInfo left, SerializeTypeInfo right) => left.Equals(right);

        /// <summary>不等运算符</summary>
        public static bool operator !=(SerializeTypeInfo left, SerializeTypeInfo right) => !left.Equals(right);

        #endregion

        #region 字符串表示

        /// <summary>获取字符串表示</summary>
        public override string ToString()
        {
            return $"TypeInfo({Name}, Category={_category}, Flags={_flags})";
        }

        #endregion
    }

    #region 类型分类枚举

    /// <summary>
    /// 类型分类枚举
    /// <para>定义序列化类型的主要分类</para>
    /// </summary>
    public enum TypeCategory : byte
    {
        /// <summary>未知类型</summary>
        Unknown = 0,

        /// <summary>布尔类型</summary>
        Boolean = 1,

        /// <summary>整数类型</summary>
        Integer = 2,

        /// <summary>浮点类型</summary>
        Float = 3,

        /// <summary>十进制类型</summary>
        Decimal = 4,

        /// <summary>字符类型</summary>
        Char = 5,

        /// <summary>字符串类型</summary>
        String = 6,

        /// <summary>日期时间类型</summary>
        DateTime = 7,

        /// <summary>时间跨度类型</summary>
        TimeSpan = 8,

        /// <summary>GUID 类型</summary>
        Guid = 9,

        /// <summary>字节数组类型</summary>
        ByteArray = 10,

        /// <summary>枚举类型</summary>
        Enum = 11,

        /// <summary>数组类型</summary>
        Array = 12,

        /// <summary>集合类型</summary>
        Collection = 13,

        /// <summary>字典类型</summary>
        Dictionary = 14,

        /// <summary>结构体类型</summary>
        Struct = 15,

        /// <summary>对象类型</summary>
        Object = 16
    }

    #endregion

    #region 类型标志枚举

    /// <summary>
    /// 类型标志枚举
    /// <para>定义类型的各种特性标志</para>
    /// </summary>
    [Flags]
    public enum TypeFlags : uint
    {
        /// <summary>无标志</summary>
        None = 0,

        /// <summary>基元类型</summary>
        IsPrimitive = 1 << 0,

        /// <summary>值类型</summary>
        IsValueType = 1 << 1,

        /// <summary>枚举类型</summary>
        IsEnum = 1 << 2,

        /// <summary>数组类型</summary>
        IsArray = 1 << 3,

        /// <summary>集合类型</summary>
        IsCollection = 1 << 4,

        /// <summary>字典类型</summary>
        IsDictionary = 1 << 5,

        /// <summary>泛型类型</summary>
        IsGeneric = 1 << 6,

        /// <summary>可空类型</summary>
        IsNullable = 1 << 7,

        /// <summary>接口类型</summary>
        IsInterface = 1 << 8,

        /// <summary>抽象类型</summary>
        IsAbstract = 1 << 9,

        /// <summary>密封类型</summary>
        IsSealed = 1 << 10,

        /// <summary>Blittable 类型</summary>
        IsBlittable = 1 << 11,

        /// <summary>非托管类型</summary>
        IsUnmanaged = 1 << 12,

        /// <summary>有默认构造函数</summary>
        HasDefaultConstructor = 1 << 13,

        /// <summary>有自定义格式化器</summary>
        HasCustomFormatter = 1 << 14,

        /// <summary>有序列化回调</summary>
        HasSerializationCallbacks = 1 << 15,

        /// <summary>支持多态</summary>
        SupportsPolymorphism = 1 << 16
    }

    #endregion
}
