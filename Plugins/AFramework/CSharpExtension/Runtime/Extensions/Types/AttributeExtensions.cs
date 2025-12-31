// ==========================================================
// 文件名：AttributeExtensions.cs
// 命名空间: AFramework.CSharpExtension
// 依赖: System, System.Reflection, System.Linq
// ==========================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace AFramework.CSharpExtension
{
    /// <summary>
    /// Attribute 扩展方法
    /// <para>提供特性的常用操作扩展，包括特性获取、检查、缓存等功能</para>
    /// </summary>
    public static class AttributeExtensions
    {
        #region 缓存

        /// <summary>
        /// 特性缓存（提高反射性能）
        /// </summary>
        private static readonly Dictionary<(MemberInfo, Type), Attribute[]> _attributeCache = 
            new Dictionary<(MemberInfo, Type), Attribute[]>();

        private static readonly object _cacheLock = new object();

        /// <summary>
        /// 清除特性缓存
        /// </summary>
        public static void ClearAttributeCache()
        {
            lock (_cacheLock)
            {
                _attributeCache.Clear();
            }
        }

        #endregion

        #region MemberInfo 扩展

        /// <summary>
        /// 检查成员是否有指定特性
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasAttribute<TAttribute>(this MemberInfo member, bool inherit = true) 
            where TAttribute : Attribute
        {
            return member != null && Attribute.IsDefined(member, typeof(TAttribute), inherit);
        }

        /// <summary>
        /// 获取成员的指定特性
        /// </summary>
        public static TAttribute GetAttribute<TAttribute>(this MemberInfo member, bool inherit = true) 
            where TAttribute : Attribute
        {
            if (member == null) return null;
            
            var attributes = member.GetCustomAttributes(typeof(TAttribute), inherit);
            return attributes.Length > 0 ? attributes[0] as TAttribute : null;
        }

        /// <summary>
        /// 获取成员的所有指定特性
        /// </summary>
        public static TAttribute[] GetAttributes<TAttribute>(this MemberInfo member, bool inherit = true) 
            where TAttribute : Attribute
        {
            if (member == null) return Array.Empty<TAttribute>();
            
            var attrs = member.GetCustomAttributes(typeof(TAttribute), inherit);
            var result = new TAttribute[attrs.Length];
            for (int i = 0; i < attrs.Length; i++)
            {
                result[i] = (TAttribute)attrs[i];
            }
            return result;
        }

        /// <summary>
        /// 获取成员的指定特性（带缓存）
        /// </summary>
        public static TAttribute GetAttributeCached<TAttribute>(this MemberInfo member, bool inherit = true) 
            where TAttribute : Attribute
        {
            if (member == null) return null;
            
            var key = (member, typeof(TAttribute));
            
            lock (_cacheLock)
            {
                if (_attributeCache.TryGetValue(key, out var cached))
                    return cached.Length > 0 ? cached[0] as TAttribute : null;
                
                var attrs = member.GetCustomAttributes(typeof(TAttribute), inherit);
                var attributes = new Attribute[attrs.Length];
                for (int i = 0; i < attrs.Length; i++)
                {
                    attributes[i] = (Attribute)attrs[i];
                }
                _attributeCache[key] = attributes;
                return attributes.Length > 0 ? attributes[0] as TAttribute : null;
            }
        }

        /// <summary>
        /// 尝试获取成员的指定特性
        /// </summary>
        public static bool TryGetAttribute<TAttribute>(this MemberInfo member, out TAttribute attribute, bool inherit = true) 
            where TAttribute : Attribute
        {
            attribute = member.GetAttribute<TAttribute>(inherit);
            return attribute != null;
        }

        #endregion

        #region ParameterInfo 扩展

        /// <summary>
        /// 检查参数是否有指定特性
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasAttribute<TAttribute>(this ParameterInfo parameter, bool inherit = true) 
            where TAttribute : Attribute
        {
            return parameter != null && Attribute.IsDefined(parameter, typeof(TAttribute), inherit);
        }

        /// <summary>
        /// 获取参数的指定特性
        /// </summary>
        public static TAttribute GetAttribute<TAttribute>(this ParameterInfo parameter, bool inherit = true) 
            where TAttribute : Attribute
        {
            if (parameter == null) return null;
            
            var attributes = parameter.GetCustomAttributes(typeof(TAttribute), inherit);
            return attributes.Length > 0 ? attributes[0] as TAttribute : null;
        }

        #endregion

        #region Assembly 扩展

        /// <summary>
        /// 检查程序集是否有指定特性
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasAttribute<TAttribute>(this Assembly assembly) where TAttribute : Attribute
        {
            return assembly != null && Attribute.IsDefined(assembly, typeof(TAttribute));
        }

        /// <summary>
        /// 获取程序集的指定特性
        /// </summary>
        public static TAttribute GetAttribute<TAttribute>(this Assembly assembly) where TAttribute : Attribute
        {
            if (assembly == null) return null;
            
            var attributes = assembly.GetCustomAttributes(typeof(TAttribute), false);
            return attributes.Length > 0 ? attributes[0] as TAttribute : null;
        }

        #endregion

        #region 特性值提取

        /// <summary>
        /// 获取 Description 特性的值
        /// </summary>
        public static string GetDescription(this MemberInfo member)
        {
            var attr = member.GetAttribute<System.ComponentModel.DescriptionAttribute>();
            return attr?.Description ?? member?.Name ?? string.Empty;
        }

        /// <summary>
        /// 获取 DisplayName 特性的值
        /// </summary>
        public static string GetDisplayName(this MemberInfo member)
        {
            var attr = member.GetAttribute<System.ComponentModel.DisplayNameAttribute>();
            return attr?.DisplayName ?? member?.Name ?? string.Empty;
        }

        /// <summary>
        /// 获取 Category 特性的值
        /// </summary>
        public static string GetCategory(this MemberInfo member)
        {
            var attr = member.GetAttribute<System.ComponentModel.CategoryAttribute>();
            return attr?.Category ?? string.Empty;
        }

        /// <summary>
        /// 检查成员是否标记为 Obsolete
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsObsolete(this MemberInfo member)
        {
            return member.HasAttribute<ObsoleteAttribute>();
        }

        /// <summary>
        /// 获取 Obsolete 特性的消息
        /// </summary>
        public static string GetObsoleteMessage(this MemberInfo member)
        {
            var attr = member.GetAttribute<ObsoleteAttribute>();
            return attr?.Message ?? string.Empty;
        }

        #endregion

        #region 批量操作

        /// <summary>
        /// 获取类型中所有带指定特性的成员
        /// </summary>
        public static IEnumerable<MemberInfo> GetMembersWithAttribute<TAttribute>(this Type type, bool inherit = true) 
            where TAttribute : Attribute
        {
            if (type == null) yield break;
            
            foreach (var member in type.GetMembers(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static))
            {
                if (member.HasAttribute<TAttribute>(inherit))
                    yield return member;
            }
        }

        /// <summary>
        /// 获取类型中所有带指定特性的属性
        /// </summary>
        public static IEnumerable<PropertyInfo> GetPropertiesWithAttribute<TAttribute>(this Type type, bool inherit = true) 
            where TAttribute : Attribute
        {
            if (type == null) yield break;
            
            foreach (var property in type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static))
            {
                if (property.HasAttribute<TAttribute>(inherit))
                    yield return property;
            }
        }

        /// <summary>
        /// 获取类型中所有带指定特性的字段
        /// </summary>
        public static IEnumerable<FieldInfo> GetFieldsWithAttribute<TAttribute>(this Type type, bool inherit = true) 
            where TAttribute : Attribute
        {
            if (type == null) yield break;
            
            foreach (var field in type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static))
            {
                if (field.HasAttribute<TAttribute>(inherit))
                    yield return field;
            }
        }

        /// <summary>
        /// 获取类型中所有带指定特性的方法
        /// </summary>
        public static IEnumerable<MethodInfo> GetMethodsWithAttribute<TAttribute>(this Type type, bool inherit = true) 
            where TAttribute : Attribute
        {
            if (type == null) yield break;
            
            foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static))
            {
                if (method.HasAttribute<TAttribute>(inherit))
                    yield return method;
            }
        }

        #endregion

        #region 特性与值的映射

        /// <summary>
        /// 获取成员及其特性的映射
        /// </summary>
        public static Dictionary<MemberInfo, TAttribute> GetMemberAttributeMap<TAttribute>(this Type type, bool inherit = true) 
            where TAttribute : Attribute
        {
            var result = new Dictionary<MemberInfo, TAttribute>();
            if (type == null) return result;
            
            foreach (var member in type.GetMembersWithAttribute<TAttribute>(inherit))
            {
                var attr = member.GetAttribute<TAttribute>(inherit);
                if (attr != null)
                    result[member] = attr;
            }
            
            return result;
        }

        #endregion

        #region 条件检查

        /// <summary>
        /// 检查成员是否有任意一个指定特性
        /// </summary>
        public static bool HasAnyAttribute(this MemberInfo member, params Type[] attributeTypes)
        {
            if (member == null || attributeTypes == null) return false;
            
            foreach (var attrType in attributeTypes)
            {
                if (Attribute.IsDefined(member, attrType, true))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// 检查成员是否有所有指定特性
        /// </summary>
        public static bool HasAllAttributes(this MemberInfo member, params Type[] attributeTypes)
        {
            if (member == null || attributeTypes == null || attributeTypes.Length == 0) return false;
            
            foreach (var attrType in attributeTypes)
            {
                if (!Attribute.IsDefined(member, attrType, true))
                    return false;
            }
            return true;
        }

        #endregion
    }
}
