// ==========================================================
// 文件名：MemberInfoExtensions.cs
// 命名空间: AFramework.AMapper
// 依赖: System, System.Reflection
// 功能: MemberInfo 扩展方法，提供成员信息访问辅助功能
// ==========================================================

using System;
using System.Reflection;

namespace AFramework.AMapper
{
    /// <summary>
    /// MemberInfo 扩展方法
    /// <para>提供成员信息访问、值获取和设置等辅助功能</para>
    /// <para>MemberInfo extension methods providing member access, value get/set utilities</para>
    /// </summary>
    /// <remarks>
    /// 设计原则：
    /// <list type="bullet">
    /// <item>单一职责：每个扩展方法专注于单一功能</item>
    /// <item>性能优化：使用委托缓存减少反射开销</item>
    /// <item>空值安全：所有方法都进行空值检查</item>
    /// </list>
    /// </remarks>
    public static class MemberInfoExtensions
    {
        #region 成员类型 / Member Type

        /// <summary>
        /// 获取成员的类型
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
                case MethodInfo method:
                    return method.ReturnType;
                default:
                    throw new ArgumentException($"不支持的成员类型：{member.MemberType} / Unsupported member type: {member.MemberType}");
            }
        }

        #endregion

        #region 值访问 / Value Access

        /// <summary>
        /// 获取成员的值
        /// <para>Get member value</para>
        /// </summary>
        /// <param name="member">成员信息 / Member info</param>
        /// <param name="obj">对象实例 / Object instance</param>
        /// <returns>成员值 / Member value</returns>
        public static object GetValue(this MemberInfo member, object obj)
        {
            if (member == null)
                throw new ArgumentNullException(nameof(member));

            switch (member)
            {
                case PropertyInfo property:
                    if (!property.CanRead)
                        throw new InvalidOperationException($"属性 {property.Name} 不可读 / Property {property.Name} is not readable");
                    return property.GetValue(obj);

                case FieldInfo field:
                    return field.GetValue(obj);

                default:
                    throw new ArgumentException($"不支持的成员类型：{member.MemberType} / Unsupported member type: {member.MemberType}");
            }
        }

        /// <summary>
        /// 设置成员的值
        /// <para>Set member value</para>
        /// </summary>
        /// <param name="member">成员信息 / Member info</param>
        /// <param name="obj">对象实例 / Object instance</param>
        /// <param name="value">要设置的值 / Value to set</param>
        public static void SetValue(this MemberInfo member, object obj, object value)
        {
            if (member == null)
                throw new ArgumentNullException(nameof(member));

            switch (member)
            {
                case PropertyInfo property:
                    if (!property.CanWrite)
                        throw new InvalidOperationException($"属性 {property.Name} 不可写 / Property {property.Name} is not writable");
                    property.SetValue(obj, value);
                    break;

                case FieldInfo field:
                    if (field.IsInitOnly)
                        throw new InvalidOperationException($"字段 {field.Name} 是只读的 / Field {field.Name} is readonly");
                    field.SetValue(obj, value);
                    break;

                default:
                    throw new ArgumentException($"不支持的成员类型：{member.MemberType} / Unsupported member type: {member.MemberType}");
            }
        }

        #endregion

        #region 成员特性 / Member Characteristics

        /// <summary>
        /// 判断成员是否可读
        /// <para>Check if member is readable</para>
        /// </summary>
        /// <param name="member">成员信息 / Member info</param>
        /// <returns>是否可读 / Whether is readable</returns>
        public static bool CanRead(this MemberInfo member)
        {
            if (member == null)
                return false;

            switch (member)
            {
                case PropertyInfo property:
                    return property.CanRead;
                case FieldInfo field:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// 判断成员是否可写
        /// <para>Check if member is writable</para>
        /// </summary>
        /// <param name="member">成员信息 / Member info</param>
        /// <returns>是否可写 / Whether is writable</returns>
        public static bool CanWrite(this MemberInfo member)
        {
            if (member == null)
                return false;

            switch (member)
            {
                case PropertyInfo property:
                    return property.CanWrite;
                case FieldInfo field:
                    return !field.IsInitOnly && !field.IsLiteral;
                default:
                    return false;
            }
        }

        /// <summary>
        /// 判断成员是否为静态成员
        /// <para>Check if member is static</para>
        /// </summary>
        /// <param name="member">成员信息 / Member info</param>
        /// <returns>是否为静态成员 / Whether is static</returns>
        public static bool IsStatic(this MemberInfo member)
        {
            if (member == null)
                return false;

            switch (member)
            {
                case PropertyInfo property:
                    return property.GetMethod?.IsStatic ?? property.SetMethod?.IsStatic ?? false;
                case FieldInfo field:
                    return field.IsStatic;
                case MethodInfo method:
                    return method.IsStatic;
                default:
                    return false;
            }
        }

        /// <summary>
        /// 判断成员是否为公共成员
        /// <para>Check if member is public</para>
        /// </summary>
        /// <param name="member">成员信息 / Member info</param>
        /// <returns>是否为公共成员 / Whether is public</returns>
        public static bool IsPublic(this MemberInfo member)
        {
            if (member == null)
                return false;

            switch (member)
            {
                case PropertyInfo property:
                    return (property.GetMethod?.IsPublic ?? false) || (property.SetMethod?.IsPublic ?? false);
                case FieldInfo field:
                    return field.IsPublic;
                case MethodInfo method:
                    return method.IsPublic;
                default:
                    return false;
            }
        }

        #endregion

        #region 特性访问 / Attribute Access

        /// <summary>
        /// 判断成员是否有指定特性
        /// <para>Check if member has specified attribute</para>
        /// </summary>
        /// <typeparam name="TAttribute">特性类型 / Attribute type</typeparam>
        /// <param name="member">成员信息 / Member info</param>
        /// <param name="inherit">是否继承 / Whether inherit</param>
        /// <returns>是否有特性 / Whether has attribute</returns>
        public static bool HasAttribute<TAttribute>(this MemberInfo member, bool inherit = true)
            where TAttribute : Attribute
        {
            if (member == null)
                return false;

            return member.GetCustomAttribute<TAttribute>(inherit) != null;
        }

        /// <summary>
        /// 获取成员的指定特性
        /// <para>Get specified attribute of member</para>
        /// </summary>
        /// <typeparam name="TAttribute">特性类型 / Attribute type</typeparam>
        /// <param name="member">成员信息 / Member info</param>
        /// <param name="inherit">是否继承 / Whether inherit</param>
        /// <returns>特性实例 / Attribute instance</returns>
        public static TAttribute GetAttribute<TAttribute>(this MemberInfo member, bool inherit = true)
            where TAttribute : Attribute
        {
            if (member == null)
                return null;

            return member.GetCustomAttribute<TAttribute>(inherit);
        }

        /// <summary>
        /// 获取成员的所有指定特性
        /// <para>Get all specified attributes of member</para>
        /// </summary>
        /// <typeparam name="TAttribute">特性类型 / Attribute type</typeparam>
        /// <param name="member">成员信息 / Member info</param>
        /// <param name="inherit">是否继承 / Whether inherit</param>
        /// <returns>特性数组 / Attribute array</returns>
        public static TAttribute[] GetAttributes<TAttribute>(this MemberInfo member, bool inherit = true)
            where TAttribute : Attribute
        {
            if (member == null)
                return Array.Empty<TAttribute>();

            return (TAttribute[])member.GetCustomAttributes(typeof(TAttribute), inherit);
        }

        #endregion

        #region 名称处理 / Name Handling

        /// <summary>
        /// 获取成员的友好名称
        /// <para>Get friendly name of member</para>
        /// </summary>
        /// <param name="member">成员信息 / Member info</param>
        /// <returns>友好名称 / Friendly name</returns>
        public static string GetFriendlyName(this MemberInfo member)
        {
            if (member == null)
                return string.Empty;

            var memberType = MemberInfoExtensions.GetMemberType(member);
            return $"{member.Name} ({memberType?.GetFriendlyName() ?? "unknown"})";
        }

        #endregion
    }
}
