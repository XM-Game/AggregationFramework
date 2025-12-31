// ==========================================================
// 文件名：TypeExtensions.cs
// 命名空间: AFramework.CSharpExtension
// 依赖: System, System.Reflection, System.Collections.Generic
// ==========================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace AFramework.CSharpExtension
{
    /// <summary>
    /// Type 扩展方法
    /// <para>提供类型的常用操作扩展，包括类型检查、泛型操作、继承关系等功能</para>
    /// </summary>
    public static class TypeExtensions
    {
        #region 类型检查

        /// <summary>
        /// 检查类型是否为数值类型
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNumeric(this Type type)
        {
            if (type == null) return false;
            
            var underlyingType = Nullable.GetUnderlyingType(type) ?? type;
            return underlyingType == typeof(byte) ||
                   underlyingType == typeof(sbyte) ||
                   underlyingType == typeof(short) ||
                   underlyingType == typeof(ushort) ||
                   underlyingType == typeof(int) ||
                   underlyingType == typeof(uint) ||
                   underlyingType == typeof(long) ||
                   underlyingType == typeof(ulong) ||
                   underlyingType == typeof(float) ||
                   underlyingType == typeof(double) ||
                   underlyingType == typeof(decimal);
        }

        /// <summary>
        /// 检查类型是否为整数类型
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsInteger(this Type type)
        {
            if (type == null) return false;
            
            var underlyingType = Nullable.GetUnderlyingType(type) ?? type;
            return underlyingType == typeof(byte) ||
                   underlyingType == typeof(sbyte) ||
                   underlyingType == typeof(short) ||
                   underlyingType == typeof(ushort) ||
                   underlyingType == typeof(int) ||
                   underlyingType == typeof(uint) ||
                   underlyingType == typeof(long) ||
                   underlyingType == typeof(ulong);
        }

        /// <summary>
        /// 检查类型是否为浮点类型
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsFloatingPoint(this Type type)
        {
            if (type == null) return false;
            
            var underlyingType = Nullable.GetUnderlyingType(type) ?? type;
            return underlyingType == typeof(float) ||
                   underlyingType == typeof(double) ||
                   underlyingType == typeof(decimal);
        }

        /// <summary>
        /// 检查类型是否为可空类型
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNullable(this Type type)
        {
            return type != null && type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        /// <summary>
        /// 检查类型是否为集合类型
        /// </summary>
        public static bool IsCollection(this Type type)
        {
            if (type == null) return false;
            if (type == typeof(string)) return false;
            
            return type.IsArray || 
                   typeof(System.Collections.IEnumerable).IsAssignableFrom(type);
        }

        /// <summary>
        /// 检查类型是否为字典类型
        /// </summary>
        public static bool IsDictionary(this Type type)
        {
            if (type == null) return false;
            
            return typeof(System.Collections.IDictionary).IsAssignableFrom(type) ||
                   (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Dictionary<,>)) ||
                   (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IDictionary<,>));
        }

        /// <summary>
        /// 检查类型是否为简单类型（基元类型、字符串、日期等）
        /// </summary>
        public static bool IsSimple(this Type type)
        {
            if (type == null) return false;
            
            var underlyingType = Nullable.GetUnderlyingType(type) ?? type;
            return underlyingType.IsPrimitive ||
                   underlyingType.IsEnum ||
                   underlyingType == typeof(string) ||
                   underlyingType == typeof(decimal) ||
                   underlyingType == typeof(DateTime) ||
                   underlyingType == typeof(DateTimeOffset) ||
                   underlyingType == typeof(TimeSpan) ||
                   underlyingType == typeof(Guid);
        }

        /// <summary>
        /// 检查类型是否为静态类
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsStatic(this Type type)
        {
            return type != null && type.IsAbstract && type.IsSealed;
        }

        /// <summary>
        /// 检查类型是否为匿名类型
        /// </summary>
        public static bool IsAnonymous(this Type type)
        {
            if (type == null) return false;
            
            return Attribute.IsDefined(type, typeof(CompilerGeneratedAttribute), false) &&
                   type.IsGenericType &&
                   type.Name.Contains("AnonymousType") &&
                   (type.Name.StartsWith("<>") || type.Name.StartsWith("VB$")) &&
                   type.Attributes.HasFlag(TypeAttributes.NotPublic);
        }

        #endregion

        #region 泛型操作

        /// <summary>
        /// 检查类型是否实现了指定的泛型接口
        /// </summary>
        public static bool ImplementsGenericInterface(this Type type, Type genericInterface)
        {
            if (type == null || genericInterface == null) return false;
            
            return type.GetInterfaces().Any(i => 
                i.IsGenericType && i.GetGenericTypeDefinition() == genericInterface);
        }

        /// <summary>
        /// 获取泛型类型参数
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Type[] GetGenericArguments(this Type type)
        {
            return type?.GetGenericArguments() ?? Array.Empty<Type>();
        }

        /// <summary>
        /// 获取泛型类型定义
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Type GetGenericDefinition(this Type type)
        {
            return type != null && type.IsGenericType ? type.GetGenericTypeDefinition() : null;
        }

        /// <summary>
        /// 创建泛型类型实例
        /// </summary>
        public static Type MakeGenericTypeSafe(this Type type, params Type[] typeArguments)
        {
            if (type == null || !type.IsGenericTypeDefinition) return null;
            
            try
            {
                return type.MakeGenericType(typeArguments);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 获取可空类型的基础类型
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Type GetUnderlyingType(this Type type)
        {
            return type != null ? Nullable.GetUnderlyingType(type) ?? type : null;
        }

        #endregion

        #region 继承关系

        /// <summary>
        /// 检查类型是否继承自指定类型
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool InheritsFrom(this Type type, Type baseType)
        {
            return type != null && baseType != null && baseType.IsAssignableFrom(type);
        }

        /// <summary>
        /// 检查类型是否继承自指定泛型类型
        /// </summary>
        public static bool InheritsFromGeneric(this Type type, Type genericType)
        {
            if (type == null || genericType == null) return false;
            
            var currentType = type;
            while (currentType != null && currentType != typeof(object))
            {
                if (currentType.IsGenericType && currentType.GetGenericTypeDefinition() == genericType)
                    return true;
                currentType = currentType.BaseType;
            }
            return false;
        }

        /// <summary>
        /// 获取类型的继承链
        /// </summary>
        public static IEnumerable<Type> GetInheritanceChain(this Type type)
        {
            if (type == null) yield break;
            
            var currentType = type;
            while (currentType != null)
            {
                yield return currentType;
                currentType = currentType.BaseType;
            }
        }

        /// <summary>
        /// 获取类型实现的所有接口（包括继承的）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Type[] GetAllInterfaces(this Type type)
        {
            return type?.GetInterfaces() ?? Array.Empty<Type>();
        }

        #endregion

        #region 实例创建

        /// <summary>
        /// 创建类型的默认实例
        /// </summary>
        public static object CreateInstance(this Type type)
        {
            if (type == null) return null;
            
            try
            {
                return Activator.CreateInstance(type);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 创建类型的实例（带参数）
        /// </summary>
        public static object CreateInstance(this Type type, params object[] args)
        {
            if (type == null) return null;
            
            try
            {
                return Activator.CreateInstance(type, args);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 创建类型的泛型实例
        /// </summary>
        public static T CreateInstance<T>(this Type type) where T : class
        {
            return type?.CreateInstance() as T;
        }

        /// <summary>
        /// 检查类型是否有无参构造函数
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasDefaultConstructor(this Type type)
        {
            return type != null && (type.IsValueType || type.GetConstructor(Type.EmptyTypes) != null);
        }

        #endregion

        #region 成员获取

        /// <summary>
        /// 获取类型的所有公共属性（包括继承的）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PropertyInfo[] GetAllPublicProperties(this Type type)
        {
            return type?.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy) 
                   ?? Array.Empty<PropertyInfo>();
        }

        /// <summary>
        /// 获取类型的所有属性（包括私有和继承的）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PropertyInfo[] GetAllProperties(this Type type)
        {
            return type?.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static) 
                   ?? Array.Empty<PropertyInfo>();
        }

        /// <summary>
        /// 获取类型的所有公共字段（包括继承的）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FieldInfo[] GetAllPublicFields(this Type type)
        {
            return type?.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy) 
                   ?? Array.Empty<FieldInfo>();
        }

        /// <summary>
        /// 获取类型的所有字段（包括私有和继承的）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FieldInfo[] GetAllFields(this Type type)
        {
            return type?.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static) 
                   ?? Array.Empty<FieldInfo>();
        }

        /// <summary>
        /// 获取类型的所有公共方法（包括继承的）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static MethodInfo[] GetAllPublicMethods(this Type type)
        {
            return type?.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy) 
                   ?? Array.Empty<MethodInfo>();
        }

        #endregion

        #region 特性操作

        /// <summary>
        /// 检查类型是否有指定特性
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasAttribute<TAttribute>(this Type type, bool inherit = true) where TAttribute : Attribute
        {
            return type != null && Attribute.IsDefined(type, typeof(TAttribute), inherit);
        }

        /// <summary>
        /// 获取类型的指定特性
        /// </summary>
        public static TAttribute GetAttribute<TAttribute>(this Type type, bool inherit = true) where TAttribute : Attribute
        {
            if (type == null) return null;
            
            var attributes = type.GetCustomAttributes(typeof(TAttribute), inherit);
            return attributes.Length > 0 ? attributes[0] as TAttribute : null;
        }

        /// <summary>
        /// 获取类型的所有指定特性
        /// </summary>
        public static TAttribute[] GetAttributes<TAttribute>(this Type type, bool inherit = true) where TAttribute : Attribute
        {
            if (type == null) return Array.Empty<TAttribute>();
            
            var attrs = type.GetCustomAttributes(typeof(TAttribute), inherit);
            var result = new TAttribute[attrs.Length];
            for (int i = 0; i < attrs.Length; i++)
            {
                result[i] = (TAttribute)attrs[i];
            }
            return result;
        }

        #endregion

        #region 名称操作

        /// <summary>
        /// 获取类型的友好名称（处理泛型）
        /// </summary>
        public static string GetFriendlyName(this Type type)
        {
            if (type == null) return "null";
            
            if (!type.IsGenericType)
                return type.Name;
            
            var genericTypeName = type.GetGenericTypeDefinition().Name;
            genericTypeName = genericTypeName.Substring(0, genericTypeName.IndexOf('`'));
            var genericArgs = string.Join(", ", type.GetGenericArguments().Select(GetFriendlyName));
            return $"{genericTypeName}<{genericArgs}>";
        }

        /// <summary>
        /// 获取类型的完整友好名称（包含命名空间）
        /// </summary>
        public static string GetFullFriendlyName(this Type type)
        {
            if (type == null) return "null";
            
            var friendlyName = type.GetFriendlyName();
            return string.IsNullOrEmpty(type.Namespace) ? friendlyName : $"{type.Namespace}.{friendlyName}";
        }

        #endregion

        #region 默认值

        /// <summary>
        /// 获取类型的默认值
        /// </summary>
        public static object GetDefaultValue(this Type type)
        {
            if (type == null) return null;
            return type.IsValueType ? Activator.CreateInstance(type) : null;
        }

        #endregion
    }
}
