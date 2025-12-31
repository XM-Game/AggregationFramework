// ==========================================================
// 文件名：MemberInfoExtensions.cs
// 命名空间: AFramework.CSharpExtension
// 依赖: System, System.Reflection
// ==========================================================

using System;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace AFramework.CSharpExtension
{
    /// <summary>
    /// MemberInfo 扩展方法
    /// <para>提供成员信息的常用操作扩展，包括类型获取、访问性检查等功能</para>
    /// </summary>
    public static class MemberInfoExtensions
    {
        #region 类型获取

        /// <summary>
        /// 获取成员的类型（属性返回属性类型，字段返回字段类型，方法返回返回类型）
        /// </summary>
        public static Type GetMemberType(this MemberInfo member)
        {
            if (member == null) return null;
            
            switch (member.MemberType)
            {
                case MemberTypes.Property:
                    return ((PropertyInfo)member).PropertyType;
                case MemberTypes.Field:
                    return ((FieldInfo)member).FieldType;
                case MemberTypes.Method:
                    return ((MethodInfo)member).ReturnType;
                case MemberTypes.Event:
                    return ((EventInfo)member).EventHandlerType;
                case MemberTypes.TypeInfo:
                case MemberTypes.NestedType:
                    return (Type)member;
                default:
                    return null;
            }
        }

        /// <summary>
        /// 获取成员的声明类型
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Type GetDeclaringTypeSafe(this MemberInfo member)
        {
            return member?.DeclaringType;
        }

        /// <summary>
        /// 获取成员的反射类型
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Type GetReflectedTypeSafe(this MemberInfo member)
        {
            return member?.ReflectedType;
        }

        #endregion

        #region 成员类型检查

        /// <summary>
        /// 检查是否为属性
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsProperty(this MemberInfo member)
        {
            return member?.MemberType == MemberTypes.Property;
        }

        /// <summary>
        /// 检查是否为字段
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsField(this MemberInfo member)
        {
            return member?.MemberType == MemberTypes.Field;
        }

        /// <summary>
        /// 检查是否为方法
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsMethod(this MemberInfo member)
        {
            return member?.MemberType == MemberTypes.Method;
        }

        /// <summary>
        /// 检查是否为事件
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsEvent(this MemberInfo member)
        {
            return member?.MemberType == MemberTypes.Event;
        }

        /// <summary>
        /// 检查是否为构造函数
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsConstructor(this MemberInfo member)
        {
            return member?.MemberType == MemberTypes.Constructor;
        }

        /// <summary>
        /// 检查是否为嵌套类型
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNestedType(this MemberInfo member)
        {
            return member?.MemberType == MemberTypes.NestedType;
        }

        #endregion

        #region 访问性检查

        /// <summary>
        /// 检查成员是否为公共成员
        /// </summary>
        public static bool IsPublic(this MemberInfo member)
        {
            if (member == null) return false;
            
            switch (member.MemberType)
            {
                case MemberTypes.Property:
                    var prop = (PropertyInfo)member;
                    return (prop.GetMethod?.IsPublic ?? false) || (prop.SetMethod?.IsPublic ?? false);
                case MemberTypes.Field:
                    return ((FieldInfo)member).IsPublic;
                case MemberTypes.Method:
                    return ((MethodInfo)member).IsPublic;
                case MemberTypes.Constructor:
                    return ((ConstructorInfo)member).IsPublic;
                case MemberTypes.Event:
                    var evt = (EventInfo)member;
                    return evt.AddMethod?.IsPublic ?? false;
                case MemberTypes.TypeInfo:
                case MemberTypes.NestedType:
                    return ((Type)member).IsPublic || ((Type)member).IsNestedPublic;
                default:
                    return false;
            }
        }

        /// <summary>
        /// 检查成员是否为私有成员
        /// </summary>
        public static bool IsPrivate(this MemberInfo member)
        {
            if (member == null) return false;
            
            switch (member.MemberType)
            {
                case MemberTypes.Property:
                    var prop = (PropertyInfo)member;
                    return (prop.GetMethod?.IsPrivate ?? true) && (prop.SetMethod?.IsPrivate ?? true);
                case MemberTypes.Field:
                    return ((FieldInfo)member).IsPrivate;
                case MemberTypes.Method:
                    return ((MethodInfo)member).IsPrivate;
                case MemberTypes.Constructor:
                    return ((ConstructorInfo)member).IsPrivate;
                case MemberTypes.TypeInfo:
                case MemberTypes.NestedType:
                    return ((Type)member).IsNestedPrivate;
                default:
                    return false;
            }
        }

        /// <summary>
        /// 检查成员是否为静态成员
        /// </summary>
        public static bool IsStatic(this MemberInfo member)
        {
            if (member == null) return false;
            
            switch (member.MemberType)
            {
                case MemberTypes.Property:
                    var prop = (PropertyInfo)member;
                    return (prop.GetMethod?.IsStatic ?? false) || (prop.SetMethod?.IsStatic ?? false);
                case MemberTypes.Field:
                    return ((FieldInfo)member).IsStatic;
                case MemberTypes.Method:
                    return ((MethodInfo)member).IsStatic;
                case MemberTypes.Constructor:
                    return ((ConstructorInfo)member).IsStatic;
                case MemberTypes.Event:
                    var evt = (EventInfo)member;
                    return evt.AddMethod?.IsStatic ?? false;
                default:
                    return false;
            }
        }

        /// <summary>
        /// 检查成员是否为实例成员
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsInstance(this MemberInfo member)
        {
            return member != null && !member.IsStatic();
        }

        #endregion

        #region 值操作

        /// <summary>
        /// 获取成员的值
        /// </summary>
        public static object GetValue(this MemberInfo member, object obj)
        {
            if (member == null) return null;
            
            switch (member.MemberType)
            {
                case MemberTypes.Property:
                    var prop = (PropertyInfo)member;
                    return prop.CanRead ? prop.GetValue(obj) : null;
                case MemberTypes.Field:
                    return ((FieldInfo)member).GetValue(obj);
                default:
                    return null;
            }
        }

        /// <summary>
        /// 设置成员的值
        /// </summary>
        public static bool SetValue(this MemberInfo member, object obj, object value)
        {
            if (member == null) return false;
            
            try
            {
                switch (member.MemberType)
                {
                    case MemberTypes.Property:
                        var prop = (PropertyInfo)member;
                        if (prop.CanWrite)
                        {
                            prop.SetValue(obj, value);
                            return true;
                        }
                        return false;
                    case MemberTypes.Field:
                        ((FieldInfo)member).SetValue(obj, value);
                        return true;
                    default:
                        return false;
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 尝试获取成员的值
        /// </summary>
        public static bool TryGetValue(this MemberInfo member, object obj, out object value)
        {
            value = null;
            if (member == null) return false;
            
            try
            {
                value = member.GetValue(obj);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 尝试设置成员的值
        /// </summary>
        public static bool TrySetValue(this MemberInfo member, object obj, object value)
        {
            try
            {
                return member.SetValue(obj, value);
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region 可读写检查

        /// <summary>
        /// 检查成员是否可读
        /// </summary>
        public static bool CanRead(this MemberInfo member)
        {
            if (member == null) return false;
            
            switch (member.MemberType)
            {
                case MemberTypes.Property:
                    return ((PropertyInfo)member).CanRead;
                case MemberTypes.Field:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// 检查成员是否可写
        /// </summary>
        public static bool CanWrite(this MemberInfo member)
        {
            if (member == null) return false;
            
            switch (member.MemberType)
            {
                case MemberTypes.Property:
                    return ((PropertyInfo)member).CanWrite;
                case MemberTypes.Field:
                    var field = (FieldInfo)member;
                    return !field.IsInitOnly && !field.IsLiteral;
                default:
                    return false;
            }
        }

        #endregion

        #region 名称操作

        /// <summary>
        /// 获取成员的完整名称（包含声明类型）
        /// </summary>
        public static string GetFullName(this MemberInfo member)
        {
            if (member == null) return string.Empty;
            
            var declaringType = member.DeclaringType;
            return declaringType != null 
                ? $"{declaringType.FullName}.{member.Name}" 
                : member.Name;
        }

        /// <summary>
        /// 获取成员的签名字符串
        /// </summary>
        public static string GetSignature(this MemberInfo member)
        {
            if (member == null) return string.Empty;
            
            switch (member.MemberType)
            {
                case MemberTypes.Property:
                    var prop = (PropertyInfo)member;
                    return $"{prop.PropertyType.Name} {prop.Name}";
                case MemberTypes.Field:
                    var field = (FieldInfo)member;
                    return $"{field.FieldType.Name} {field.Name}";
                case MemberTypes.Method:
                    var method = (MethodInfo)member;
                    var parameters = string.Join(", ", Array.ConvertAll(method.GetParameters(), p => $"{p.ParameterType.Name} {p.Name}"));
                    return $"{method.ReturnType.Name} {method.Name}({parameters})";
                default:
                    return member.Name;
            }
        }

        #endregion

        #region 转换操作

        /// <summary>
        /// 转换为 PropertyInfo（如果是属性）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PropertyInfo AsProperty(this MemberInfo member)
        {
            return member as PropertyInfo;
        }

        /// <summary>
        /// 转换为 FieldInfo（如果是字段）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FieldInfo AsField(this MemberInfo member)
        {
            return member as FieldInfo;
        }

        /// <summary>
        /// 转换为 MethodInfo（如果是方法）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static MethodInfo AsMethod(this MemberInfo member)
        {
            return member as MethodInfo;
        }

        /// <summary>
        /// 转换为 EventInfo（如果是事件）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static EventInfo AsEvent(this MemberInfo member)
        {
            return member as EventInfo;
        }

        #endregion
    }
}
