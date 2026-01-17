// ==========================================================
// 文件名：TypeExtensions.cs
// 命名空间: AFramework.AMapper
// 依赖: System, System.Collections.Generic, System.Reflection
// 功能: Type 类型扩展方法，提供类型检查和反射辅助功能
// ==========================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AFramework.AMapper
{
    /// <summary>
    /// Type 类型扩展方法
    /// <para>提供类型检查、泛型处理、成员访问等辅助功能</para>
    /// <para>Type extension methods providing type checking, generic handling, and member access utilities</para>
    /// </summary>
    /// <remarks>
    /// 设计原则：
    /// <list type="bullet">
    /// <item>单一职责：每个扩展方法专注于单一功能</item>
    /// <item>性能优化：使用缓存减少反射开销</item>
    /// <item>空值安全：所有方法都进行空值检查</item>
    /// </list>
    /// </remarks>
    public static class TypeExtensions
    {
        #region 类型判断 / Type Checking

        /// <summary>
        /// 判断类型是否为可空值类型
        /// <para>Check if type is nullable value type</para>
        /// </summary>
        /// <param name="type">类型 / Type</param>
        /// <returns>是否为可空值类型 / Whether is nullable value type</returns>
        public static bool IsNullableType(this Type type)
        {
            if (type == null)
                return false;

            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        /// <summary>
        /// 判断类型是否为集合类型
        /// <para>Check if type is collection type</para>
        /// </summary>
        /// <param name="type">类型 / Type</param>
        /// <returns>是否为集合类型 / Whether is collection type</returns>
        public static bool IsCollectionType(this Type type)
        {
            if (type == null)
                return false;

            // 排除字符串（虽然实现了 IEnumerable<char>）
            if (type == typeof(string))
                return false;

            // 检查是否实现 IEnumerable<T>
            return type.GetInterfaces()
                .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumerable<>));
        }

        /// <summary>
        /// 判断类型是否为字典类型
        /// <para>Check if type is dictionary type</para>
        /// </summary>
        /// <param name="type">类型 / Type</param>
        /// <returns>是否为字典类型 / Whether is dictionary type</returns>
        public static bool IsDictionaryType(this Type type)
        {
            if (type == null)
                return false;

            return type.GetInterfaces()
                .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IDictionary<,>));
        }

        /// <summary>
        /// 判断类型是否为枚举类型或可空枚举类型
        /// <para>Check if type is enum or nullable enum type</para>
        /// </summary>
        /// <param name="type">类型 / Type</param>
        /// <returns>是否为枚举类型 / Whether is enum type</returns>
        public static bool IsEnumType(this Type type)
        {
            if (type == null)
                return false;

            return type.IsEnum || (type.IsNullableType() && System.Nullable.GetUnderlyingType(type).IsEnum);
        }

        /// <summary>
        /// 判断类型是否为基元类型（包括 string 和 decimal）
        /// <para>Check if type is primitive type (including string and decimal)</para>
        /// </summary>
        /// <param name="type">类型 / Type</param>
        /// <returns>是否为基元类型 / Whether is primitive type</returns>
        public static bool IsPrimitiveType(this Type type)
        {
            if (type == null)
                return false;

            return type.IsPrimitive || 
                   type == typeof(string) || 
                   type == typeof(decimal) ||
                   type == typeof(DateTime) ||
                   type == typeof(DateTimeOffset) ||
                   type == typeof(TimeSpan) ||
                   type == typeof(Guid);
        }

        /// <summary>
        /// 判断类型是否可以从另一个类型赋值
        /// <para>Check if type is assignable from another type</para>
        /// </summary>
        /// <param name="type">目标类型 / Target type</param>
        /// <param name="fromType">源类型 / Source type</param>
        /// <returns>是否可以赋值 / Whether is assignable</returns>
        public static bool IsAssignableFromType(this Type type, Type fromType)
        {
            if (type == null || fromType == null)
                return false;

            return type.IsAssignableFrom(fromType);
        }

        #endregion

        #region 泛型处理 / Generic Handling

        /// <summary>
        /// 获取可空类型的底层类型
        /// <para>Get underlying type of nullable type</para>
        /// </summary>
        /// <param name="type">可空类型 / Nullable type</param>
        /// <returns>底层类型 / Underlying type</returns>
        public static Type GetUnderlyingType(this Type type)
        {
            if (type == null)
                return null;

            return type.IsNullableType() ? System.Nullable.GetUnderlyingType(type) : type;
        }

        /// <summary>
        /// 获取集合的元素类型
        /// <para>Get element type of collection</para>
        /// </summary>
        /// <param name="type">集合类型 / Collection type</param>
        /// <returns>元素类型 / Element type</returns>
        public static Type GetCollectionElementType(this Type type)
        {
            if (type == null)
                return null;

            // 数组类型
            if (type.IsArray)
                return type.GetElementType();

            // IEnumerable<T> 类型
            var enumerableInterface = type.GetInterfaces()
                .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumerable<>));

            return enumerableInterface?.GetGenericArguments()[0];
        }

        /// <summary>
        /// 获取字典的键值类型
        /// <para>Get key and value types of dictionary</para>
        /// </summary>
        /// <param name="type">字典类型 / Dictionary type</param>
        /// <returns>键值类型元组 / Key-value type tuple</returns>
        public static (Type KeyType, Type ValueType) GetDictionaryTypes(this Type type)
        {
            if (type == null)
                return (null, null);

            var dictionaryInterface = type.GetInterfaces()
                .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IDictionary<,>));

            if (dictionaryInterface == null)
                return (null, null);

            var args = dictionaryInterface.GetGenericArguments();
            return (args[0], args[1]);
        }

        /// <summary>
        /// 获取泛型类型定义
        /// <para>Get generic type definition</para>
        /// </summary>
        /// <param name="type">泛型类型 / Generic type</param>
        /// <returns>泛型类型定义 / Generic type definition</returns>
        public static Type GetGenericTypeDefinitionSafe(this Type type)
        {
            if (type == null || !type.IsGenericType)
                return null;

            return type.GetGenericTypeDefinition();
        }

        #endregion

        #region 成员访问 / Member Access

        /// <summary>
        /// 获取所有公共实例属性
        /// <para>Get all public instance properties</para>
        /// </summary>
        /// <param name="type">类型 / Type</param>
        /// <returns>属性列表 / Property list</returns>
        public static IEnumerable<PropertyInfo> GetPublicProperties(this Type type)
        {
            if (type == null)
                return Enumerable.Empty<PropertyInfo>();

            return type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
        }

        /// <summary>
        /// 获取所有公共实例字段
        /// <para>Get all public instance fields</para>
        /// </summary>
        /// <param name="type">类型 / Type</param>
        /// <returns>字段列表 / Field list</returns>
        public static IEnumerable<FieldInfo> GetPublicFields(this Type type)
        {
            if (type == null)
                return Enumerable.Empty<FieldInfo>();

            return type.GetFields(BindingFlags.Public | BindingFlags.Instance);
        }

        /// <summary>
        /// 获取所有可读写的公共属性
        /// <para>Get all readable and writable public properties</para>
        /// </summary>
        /// <param name="type">类型 / Type</param>
        /// <returns>属性列表 / Property list</returns>
        public static IEnumerable<PropertyInfo> GetReadWriteProperties(this Type type)
        {
            if (type == null)
                return Enumerable.Empty<PropertyInfo>();

            return type.GetPublicProperties()
                .Where(p => p.CanRead && p.CanWrite);
        }

        /// <summary>
        /// 获取指定名称的成员（属性或字段）
        /// <para>Get member (property or field) by name</para>
        /// </summary>
        /// <param name="type">类型 / Type</param>
        /// <param name="memberName">成员名称 / Member name</param>
        /// <returns>成员信息 / Member info</returns>
        public static MemberInfo GetMember(this Type type, string memberName)
        {
            if (type == null || string.IsNullOrEmpty(memberName))
                return null;

            // 先查找属性
            var property = type.GetProperty(memberName, BindingFlags.Public | BindingFlags.Instance);
            if (property != null)
                return property;

            // 再查找字段
            var field = type.GetField(memberName, BindingFlags.Public | BindingFlags.Instance);
            return field;
        }

        /// <summary>
        /// 获取成员类型
        /// <para>Get member type</para>
        /// </summary>
        /// <param name="member">成员信息 / Member info</param>
        /// <returns>成员类型 / Member type</returns>
        public static Type GetMemberType(this MemberInfo member)
        {
            if (member == null)
                return null;

            switch (member)
            {
                case PropertyInfo property:
                    return property.PropertyType;
                case FieldInfo field:
                    return field.FieldType;
                default:
                    return null;
            }
        }

        #endregion

        #region 构造函数 / Constructor

        /// <summary>
        /// 获取默认构造函数
        /// <para>Get default constructor</para>
        /// </summary>
        /// <param name="type">类型 / Type</param>
        /// <returns>构造函数信息 / Constructor info</returns>
        public static ConstructorInfo GetDefaultConstructor(this Type type)
        {
            if (type == null)
                return null;

            return type.GetConstructor(
                BindingFlags.Public | BindingFlags.Instance,
                null,
                Type.EmptyTypes,
                null);
        }

        /// <summary>
        /// 判断类型是否有默认构造函数
        /// <para>Check if type has default constructor</para>
        /// </summary>
        /// <param name="type">类型 / Type</param>
        /// <returns>是否有默认构造函数 / Whether has default constructor</returns>
        public static bool HasDefaultConstructor(this Type type)
        {
            if (type == null)
                return false;

            return type.GetDefaultConstructor() != null;
        }

        /// <summary>
        /// 获取所有公共构造函数
        /// <para>Get all public constructors</para>
        /// </summary>
        /// <param name="type">类型 / Type</param>
        /// <returns>构造函数列表 / Constructor list</returns>
        public static IEnumerable<ConstructorInfo> GetPublicConstructors(this Type type)
        {
            if (type == null)
                return Enumerable.Empty<ConstructorInfo>();

            return type.GetConstructors(BindingFlags.Public | BindingFlags.Instance);
        }

        #endregion

        #region 类型名称 / Type Name

        /// <summary>
        /// 获取友好的类型名称（包括泛型参数）
        /// <para>Get friendly type name (including generic parameters)</para>
        /// </summary>
        /// <param name="type">类型 / Type</param>
        /// <returns>友好名称 / Friendly name</returns>
        public static string GetFriendlyName(this Type type)
        {
            if (type == null)
                return string.Empty;

            // 处理可空类型
            if (type.IsNullableType())
            {
                var underlyingType = System.Nullable.GetUnderlyingType(type);
                return $"{underlyingType.GetFriendlyName()}?";
            }

            // 处理泛型类型
            if (type.IsGenericType)
            {
                var genericTypeName = type.Name.Substring(0, type.Name.IndexOf('`'));
                var genericArgs = string.Join(", ", type.GetGenericArguments().Select(GetFriendlyName));
                return $"{genericTypeName}<{genericArgs}>";
            }

            // 处理数组类型
            if (type.IsArray)
            {
                var elementType = type.GetElementType();
                var rank = type.GetArrayRank();
                var brackets = rank == 1 ? "[]" : $"[{new string(',', rank - 1)}]";
                return $"{elementType.GetFriendlyName()}{brackets}";
            }

            return type.Name;
        }

        #endregion
    }
}
