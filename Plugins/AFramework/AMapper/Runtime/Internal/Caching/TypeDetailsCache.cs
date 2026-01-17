// ==========================================================
// 文件名：TypeDetailsCache.cs
// 命名空间: AFramework.AMapper.Internal
// 依赖: System, System.Collections.Generic, System.Reflection
// 功能: 类型详情缓存，缓存类型的反射信息
// ==========================================================

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AFramework.AMapper.Internal
{
    /// <summary>
    /// 类型详情缓存
    /// <para>缓存类型的反射信息以提高性能</para>
    /// <para>Type details cache for caching reflection information to improve performance</para>
    /// </summary>
    public static class TypeDetailsCache
    {
        #region 私有字段 / Private Fields

        private static readonly ConcurrentDictionary<Type, TypeDetails> _cache = new();

        #endregion

        #region 公共方法 / Public Methods

        /// <summary>
        /// 获取类型详情
        /// <para>Get type details</para>
        /// </summary>
        /// <param name="type">类型 / Type</param>
        /// <returns>类型详情 / Type details</returns>
        public static TypeDetails GetDetails(Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            return _cache.GetOrAdd(type, t => new TypeDetails(t));
        }

        /// <summary>
        /// 清除缓存
        /// <para>Clear cache</para>
        /// </summary>
        public static void Clear()
        {
            _cache.Clear();
        }

        #endregion
    }

    /// <summary>
    /// 类型详情
    /// <para>包含类型的反射信息</para>
    /// <para>Type details containing reflection information</para>
    /// </summary>
    public sealed class TypeDetails
    {
        #region 属性 / Properties

        /// <summary>
        /// 获取类型
        /// <para>Get the type</para>
        /// </summary>
        public Type Type { get; }

        /// <summary>
        /// 获取公共属性
        /// <para>Get public properties</para>
        /// </summary>
        public IReadOnlyList<PropertyInfo> PublicProperties { get; }

        /// <summary>
        /// 获取公共字段
        /// <para>Get public fields</para>
        /// </summary>
        public IReadOnlyList<FieldInfo> PublicFields { get; }

        /// <summary>
        /// 获取可读成员
        /// <para>Get readable members</para>
        /// </summary>
        public IReadOnlyList<MemberInfo> ReadableMembers { get; }

        /// <summary>
        /// 获取可写成员
        /// <para>Get writable members</para>
        /// </summary>
        public IReadOnlyList<MemberInfo> WritableMembers { get; }

        /// <summary>
        /// 获取公共构造函数
        /// <para>Get public constructors</para>
        /// </summary>
        public IReadOnlyList<ConstructorInfo> PublicConstructors { get; }

        /// <summary>
        /// 获取是否是值类型
        /// <para>Get whether is value type</para>
        /// </summary>
        public bool IsValueType { get; }

        /// <summary>
        /// 获取是否是可空类型
        /// <para>Get whether is nullable type</para>
        /// </summary>
        public bool IsNullable { get; }

        /// <summary>
        /// 获取可空类型的基础类型
        /// <para>Get underlying type of nullable</para>
        /// </summary>
        public Type UnderlyingType { get; }

        /// <summary>
        /// 获取是否是集合类型
        /// <para>Get whether is collection type</para>
        /// </summary>
        public bool IsCollection { get; }

        /// <summary>
        /// 获取是否是字典类型
        /// <para>Get whether is dictionary type</para>
        /// </summary>
        public bool IsDictionary { get; }

        /// <summary>
        /// 获取是否是枚举类型
        /// <para>Get whether is enum type</para>
        /// </summary>
        public bool IsEnum { get; }

        /// <summary>
        /// 获取元素类型（集合/数组）
        /// <para>Get element type (collection/array)</para>
        /// </summary>
        public Type ElementType { get; }

        #endregion

        #region 构造函数 / Constructor

        /// <summary>
        /// 创建类型详情
        /// </summary>
        /// <param name="type">类型 / Type</param>
        internal TypeDetails(Type type)
        {
            Type = type;
            IsValueType = type.IsValueType;
            IsEnum = type.IsEnum;
            IsNullable = System.Nullable.GetUnderlyingType(type) != null;
            UnderlyingType = System.Nullable.GetUnderlyingType(type) ?? type;

            // 获取公共属性
            PublicProperties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .ToList();

            // 获取公共字段
            PublicFields = type.GetFields(BindingFlags.Public | BindingFlags.Instance)
                .ToList();

            // 获取可读成员
            ReadableMembers = PublicProperties
                .Where(p => p.CanRead)
                .Cast<MemberInfo>()
                .Concat(PublicFields)
                .ToList();

            // 获取可写成员
            WritableMembers = PublicProperties
                .Where(p => p.CanWrite && p.GetSetMethod() != null)
                .Cast<MemberInfo>()
                .Concat(PublicFields.Where(f => !f.IsInitOnly))
                .ToList();

            // 获取公共构造函数
            PublicConstructors = type.GetConstructors(BindingFlags.Public | BindingFlags.Instance)
                .ToList();

            // 检查集合类型
            IsCollection = typeof(System.Collections.IEnumerable).IsAssignableFrom(type) && type != typeof(string);
            IsDictionary = typeof(System.Collections.IDictionary).IsAssignableFrom(type) ||
                          (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Dictionary<,>));

            // 获取元素类型
            ElementType = GetElementType(type);
        }

        #endregion

        #region 公共方法 / Public Methods

        /// <summary>
        /// 查找成员
        /// <para>Find member</para>
        /// </summary>
        /// <param name="name">成员名称 / Member name</param>
        /// <param name="ignoreCase">是否忽略大小写 / Whether to ignore case</param>
        /// <returns>成员信息或 null / Member info or null</returns>
        public MemberInfo FindMember(string name, bool ignoreCase = true)
        {
            var comparison = ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;

            // 先查找属性
            var property = PublicProperties.FirstOrDefault(p => 
                string.Equals(p.Name, name, comparison));
            if (property != null)
                return property;

            // 再查找字段
            return PublicFields.FirstOrDefault(f => 
                string.Equals(f.Name, name, comparison));
        }

        /// <summary>
        /// 查找可读成员
        /// <para>Find readable member</para>
        /// </summary>
        /// <param name="name">成员名称 / Member name</param>
        /// <param name="ignoreCase">是否忽略大小写 / Whether to ignore case</param>
        /// <returns>成员信息或 null / Member info or null</returns>
        public MemberInfo FindReadableMember(string name, bool ignoreCase = true)
        {
            var comparison = ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
            return ReadableMembers.FirstOrDefault(m => 
                string.Equals(m.Name, name, comparison));
        }

        /// <summary>
        /// 查找可写成员
        /// <para>Find writable member</para>
        /// </summary>
        /// <param name="name">成员名称 / Member name</param>
        /// <param name="ignoreCase">是否忽略大小写 / Whether to ignore case</param>
        /// <returns>成员信息或 null / Member info or null</returns>
        public MemberInfo FindWritableMember(string name, bool ignoreCase = true)
        {
            var comparison = ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
            return WritableMembers.FirstOrDefault(m => 
                string.Equals(m.Name, name, comparison));
        }

        #endregion

        #region 私有方法 / Private Methods

        private static Type GetElementType(Type type)
        {
            if (type.IsArray)
                return type.GetElementType();

            if (type.IsGenericType)
            {
                var genericArgs = type.GetGenericArguments();
                if (genericArgs.Length == 1)
                    return genericArgs[0];
            }

            // 查找 IEnumerable<T>
            var enumerable = type.GetInterfaces()
                .FirstOrDefault(i => i.IsGenericType && 
                    i.GetGenericTypeDefinition() == typeof(IEnumerable<>));

            return enumerable?.GetGenericArguments()[0];
        }

        #endregion
    }
}
