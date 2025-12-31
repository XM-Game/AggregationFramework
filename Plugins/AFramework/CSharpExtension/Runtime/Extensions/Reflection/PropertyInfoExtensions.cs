// ==========================================================
// 文件名：PropertyInfoExtensions.cs
// 命名空间: AFramework.CSharpExtension
// 依赖: System, System.Reflection
// ==========================================================

using System;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace AFramework.CSharpExtension
{
    /// <summary>
    /// PropertyInfo 扩展方法
    /// <para>提供属性信息的常用操作扩展，包括值操作、访问器检查等功能</para>
    /// </summary>
    public static class PropertyInfoExtensions
    {
        #region 值操作

        /// <summary>
        /// 安全获取属性值
        /// </summary>
        public static object GetValueSafe(this PropertyInfo property, object obj)
        {
            if (property == null || !property.CanRead) return null;
            
            try
            {
                return property.GetValue(obj);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 获取属性值并转换为指定类型
        /// </summary>
        public static T GetValue<T>(this PropertyInfo property, object obj, T defaultValue = default)
        {
            if (property == null || !property.CanRead) return defaultValue;
            
            try
            {
                var value = property.GetValue(obj);
                return value is T typed ? typed : defaultValue;
            }
            catch
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// 安全设置属性值
        /// </summary>
        public static bool SetValueSafe(this PropertyInfo property, object obj, object value)
        {
            if (property == null || !property.CanWrite) return false;
            
            try
            {
                property.SetValue(obj, value);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 尝试获取属性值
        /// </summary>
        public static bool TryGetValue(this PropertyInfo property, object obj, out object value)
        {
            value = null;
            if (property == null || !property.CanRead) return false;
            
            try
            {
                value = property.GetValue(obj);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 尝试设置属性值
        /// </summary>
        public static bool TrySetValue(this PropertyInfo property, object obj, object value)
        {
            if (property == null || !property.CanWrite) return false;
            
            try
            {
                property.SetValue(obj, value);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 复制属性值到另一个对象
        /// </summary>
        public static bool CopyValueTo(this PropertyInfo property, object source, object target)
        {
            if (property == null || !property.CanRead || !property.CanWrite)
                return false;
            
            try
            {
                var value = property.GetValue(source);
                property.SetValue(target, value);
                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region 访问器检查

        /// <summary>
        /// 检查是否有公共 getter
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasPublicGetter(this PropertyInfo property)
        {
            return property?.GetMethod?.IsPublic ?? false;
        }

        /// <summary>
        /// 检查是否有公共 setter
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasPublicSetter(this PropertyInfo property)
        {
            return property?.SetMethod?.IsPublic ?? false;
        }

        /// <summary>
        /// 检查是否为只读属性
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsReadOnly(this PropertyInfo property)
        {
            return property != null && property.CanRead && !property.CanWrite;
        }

        /// <summary>
        /// 检查是否为只写属性
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsWriteOnly(this PropertyInfo property)
        {
            return property != null && !property.CanRead && property.CanWrite;
        }

        /// <summary>
        /// 检查是否为读写属性
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsReadWrite(this PropertyInfo property)
        {
            return property != null && property.CanRead && property.CanWrite;
        }

        /// <summary>
        /// 检查是否为静态属性
        /// </summary>
        public static bool IsStatic(this PropertyInfo property)
        {
            if (property == null) return false;
            return (property.GetMethod?.IsStatic ?? false) || (property.SetMethod?.IsStatic ?? false);
        }

        /// <summary>
        /// 检查是否为虚属性
        /// </summary>
        public static bool IsVirtual(this PropertyInfo property)
        {
            if (property == null) return false;
            var getter = property.GetMethod;
            var setter = property.SetMethod;
            return (getter?.IsVirtual ?? false) || (setter?.IsVirtual ?? false);
        }

        /// <summary>
        /// 检查是否为抽象属性
        /// </summary>
        public static bool IsAbstract(this PropertyInfo property)
        {
            if (property == null) return false;
            var getter = property.GetMethod;
            var setter = property.SetMethod;
            return (getter?.IsAbstract ?? false) || (setter?.IsAbstract ?? false);
        }

        #endregion

        #region 类型检查

        /// <summary>
        /// 检查属性类型是否为指定类型
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsOfType<T>(this PropertyInfo property)
        {
            return property?.PropertyType == typeof(T);
        }

        /// <summary>
        /// 检查属性类型是否可赋值给指定类型
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsAssignableTo<T>(this PropertyInfo property)
        {
            return property != null && typeof(T).IsAssignableFrom(property.PropertyType);
        }

        /// <summary>
        /// 检查属性类型是否为值类型
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsValueType(this PropertyInfo property)
        {
            return property?.PropertyType.IsValueType ?? false;
        }

        /// <summary>
        /// 检查属性类型是否为引用类型
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsReferenceType(this PropertyInfo property)
        {
            return property != null && !property.PropertyType.IsValueType;
        }

        /// <summary>
        /// 检查属性类型是否为可空类型
        /// </summary>
        public static bool IsNullable(this PropertyInfo property)
        {
            if (property == null) return false;
            var type = property.PropertyType;
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        /// <summary>
        /// 获取可空类型的基础类型
        /// </summary>
        public static Type GetUnderlyingType(this PropertyInfo property)
        {
            if (property == null) return null;
            return Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
        }

        #endregion

        #region 索引器

        /// <summary>
        /// 检查是否为索引器
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsIndexer(this PropertyInfo property)
        {
            return property != null && property.GetIndexParameters().Length > 0;
        }

        /// <summary>
        /// 获取索引器参数类型
        /// </summary>
        public static Type[] GetIndexParameterTypes(this PropertyInfo property)
        {
            if (property == null) return Array.Empty<Type>();
            return Array.ConvertAll(property.GetIndexParameters(), p => p.ParameterType);
        }

        #endregion

        #region 签名

        /// <summary>
        /// 获取属性签名
        /// </summary>
        public static string GetSignature(this PropertyInfo property)
        {
            if (property == null) return string.Empty;
            
            var accessors = "";
            if (property.CanRead) accessors += "get; ";
            if (property.CanWrite) accessors += "set; ";
            
            return $"{property.PropertyType.Name} {property.Name} {{ {accessors.TrimEnd()}}}";
        }

        /// <summary>
        /// 获取完整属性签名
        /// </summary>
        public static string GetFullSignature(this PropertyInfo property)
        {
            if (property == null) return string.Empty;
            
            var declaringType = property.DeclaringType?.FullName ?? "Unknown";
            var accessors = "";
            if (property.CanRead) accessors += "get; ";
            if (property.CanWrite) accessors += "set; ";
            
            return $"{property.PropertyType.FullName} {declaringType}.{property.Name} {{ {accessors.TrimEnd()}}}";
        }

        #endregion

        #region 默认值

        /// <summary>
        /// 获取属性类型的默认值
        /// </summary>
        public static object GetDefaultValue(this PropertyInfo property)
        {
            if (property == null) return null;
            
            var type = property.PropertyType;
            return type.IsValueType ? Activator.CreateInstance(type) : null;
        }

        /// <summary>
        /// 将属性设置为默认值
        /// </summary>
        public static bool SetToDefault(this PropertyInfo property, object obj)
        {
            if (property == null || !property.CanWrite) return false;
            
            try
            {
                property.SetValue(obj, property.GetDefaultValue());
                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region 委托创建

        /// <summary>
        /// 创建 getter 委托
        /// </summary>
        public static Func<object, T> CreateGetter<T>(this PropertyInfo property)
        {
            if (property == null || !property.CanRead) return null;
            
            return obj => (T)property.GetValue(obj);
        }

        /// <summary>
        /// 创建 setter 委托
        /// </summary>
        public static Action<object, T> CreateSetter<T>(this PropertyInfo property)
        {
            if (property == null || !property.CanWrite) return null;
            
            return (obj, value) => property.SetValue(obj, value);
        }

        #endregion
    }
}
