// ==========================================================
// 文件名：FieldInfoExtensions.cs
// 命名空间: AFramework.CSharpExtension
// 依赖: System, System.Reflection
// ==========================================================

using System;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace AFramework.CSharpExtension
{
    /// <summary>
    /// FieldInfo 扩展方法
    /// <para>提供字段信息的常用操作扩展，包括值操作、访问性检查等功能</para>
    /// </summary>
    public static class FieldInfoExtensions
    {
        #region 值操作

        /// <summary>
        /// 安全获取字段值
        /// </summary>
        public static object GetValueSafe(this FieldInfo field, object obj)
        {
            if (field == null) return null;
            
            try
            {
                return field.GetValue(obj);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 获取字段值并转换为指定类型
        /// </summary>
        public static T GetValue<T>(this FieldInfo field, object obj, T defaultValue = default)
        {
            if (field == null) return defaultValue;
            
            try
            {
                var value = field.GetValue(obj);
                return value is T typed ? typed : defaultValue;
            }
            catch
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// 安全设置字段值
        /// </summary>
        public static bool SetValueSafe(this FieldInfo field, object obj, object value)
        {
            if (field == null || field.IsInitOnly || field.IsLiteral) return false;
            
            try
            {
                field.SetValue(obj, value);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 尝试获取字段值
        /// </summary>
        public static bool TryGetValue(this FieldInfo field, object obj, out object value)
        {
            value = null;
            if (field == null) return false;
            
            try
            {
                value = field.GetValue(obj);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 尝试设置字段值
        /// </summary>
        public static bool TrySetValue(this FieldInfo field, object obj, object value)
        {
            if (field == null || field.IsInitOnly || field.IsLiteral) return false;
            
            try
            {
                field.SetValue(obj, value);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 复制字段值到另一个对象
        /// </summary>
        public static bool CopyValueTo(this FieldInfo field, object source, object target)
        {
            if (field == null || field.IsInitOnly || field.IsLiteral)
                return false;
            
            try
            {
                var value = field.GetValue(source);
                field.SetValue(target, value);
                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region 访问性检查

        /// <summary>
        /// 检查是否为只读字段
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsReadOnly(this FieldInfo field)
        {
            return field?.IsInitOnly ?? false;
        }

        /// <summary>
        /// 检查是否为常量字段
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsConstant(this FieldInfo field)
        {
            return field?.IsLiteral ?? false;
        }

        /// <summary>
        /// 检查是否可写
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsWritable(this FieldInfo field)
        {
            return field != null && !field.IsInitOnly && !field.IsLiteral;
        }

        /// <summary>
        /// 检查是否为静态字段
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsStaticField(this FieldInfo field)
        {
            return field?.IsStatic ?? false;
        }

        /// <summary>
        /// 检查是否为实例字段
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsInstanceField(this FieldInfo field)
        {
            return field != null && !field.IsStatic;
        }

        /// <summary>
        /// 检查是否为私有字段
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsPrivateField(this FieldInfo field)
        {
            return field?.IsPrivate ?? false;
        }

        /// <summary>
        /// 检查是否为公共字段
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsPublicField(this FieldInfo field)
        {
            return field?.IsPublic ?? false;
        }

        /// <summary>
        /// 检查是否为受保护字段
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsProtectedField(this FieldInfo field)
        {
            return field?.IsFamily ?? false;
        }

        /// <summary>
        /// 检查是否为内部字段
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsInternalField(this FieldInfo field)
        {
            return field?.IsAssembly ?? false;
        }

        #endregion

        #region 类型检查

        /// <summary>
        /// 检查字段类型是否为指定类型
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsOfType<T>(this FieldInfo field)
        {
            return field?.FieldType == typeof(T);
        }

        /// <summary>
        /// 检查字段类型是否可赋值给指定类型
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsAssignableTo<T>(this FieldInfo field)
        {
            return field != null && typeof(T).IsAssignableFrom(field.FieldType);
        }

        /// <summary>
        /// 检查字段类型是否为值类型
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsValueType(this FieldInfo field)
        {
            return field?.FieldType.IsValueType ?? false;
        }

        /// <summary>
        /// 检查字段类型是否为引用类型
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsReferenceType(this FieldInfo field)
        {
            return field != null && !field.FieldType.IsValueType;
        }

        /// <summary>
        /// 检查字段类型是否为可空类型
        /// </summary>
        public static bool IsNullable(this FieldInfo field)
        {
            if (field == null) return false;
            var type = field.FieldType;
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        /// <summary>
        /// 获取可空类型的基础类型
        /// </summary>
        public static Type GetUnderlyingType(this FieldInfo field)
        {
            if (field == null) return null;
            return Nullable.GetUnderlyingType(field.FieldType) ?? field.FieldType;
        }

        #endregion

        #region 特殊字段检查

        /// <summary>
        /// 检查是否为编译器生成的字段
        /// </summary>
        public static bool IsCompilerGenerated(this FieldInfo field)
        {
            return field?.IsDefined(typeof(CompilerGeneratedAttribute), false) ?? false;
        }

        /// <summary>
        /// 检查是否为后备字段（自动属性的后备字段）
        /// </summary>
        public static bool IsBackingField(this FieldInfo field)
        {
            if (field == null) return false;
            return field.Name.Contains("<") && field.Name.Contains(">k__BackingField");
        }

        /// <summary>
        /// 获取后备字段对应的属性名
        /// </summary>
        public static string GetBackingPropertyName(this FieldInfo field)
        {
            if (field == null || !field.IsBackingField()) return null;
            
            var name = field.Name;
            var start = name.IndexOf('<') + 1;
            var end = name.IndexOf('>');
            
            return start > 0 && end > start ? name.Substring(start, end - start) : null;
        }

        /// <summary>
        /// 检查是否为序列化字段
        /// </summary>
        public static bool IsSerializable(this FieldInfo field)
        {
            if (field == null) return false;
            
            // 非公共字段需要 SerializeField 特性（Unity）或 NonSerialized 特性检查
            if (!field.IsPublic && !field.IsDefined(typeof(System.Runtime.Serialization.DataMemberAttribute), false))
                return false;
            
            // 检查是否标记为不序列化
            return !field.IsDefined(typeof(NonSerializedAttribute), false);
        }

        #endregion

        #region 签名

        /// <summary>
        /// 获取字段签名
        /// </summary>
        public static string GetSignature(this FieldInfo field)
        {
            if (field == null) return string.Empty;
            
            var modifiers = "";
            if (field.IsStatic) modifiers += "static ";
            if (field.IsInitOnly) modifiers += "readonly ";
            if (field.IsLiteral) modifiers += "const ";
            
            return $"{modifiers}{field.FieldType.Name} {field.Name}";
        }

        /// <summary>
        /// 获取完整字段签名
        /// </summary>
        public static string GetFullSignature(this FieldInfo field)
        {
            if (field == null) return string.Empty;
            
            var declaringType = field.DeclaringType?.FullName ?? "Unknown";
            var modifiers = "";
            if (field.IsStatic) modifiers += "static ";
            if (field.IsInitOnly) modifiers += "readonly ";
            if (field.IsLiteral) modifiers += "const ";
            
            return $"{modifiers}{field.FieldType.FullName} {declaringType}.{field.Name}";
        }

        #endregion

        #region 默认值

        /// <summary>
        /// 获取字段类型的默认值
        /// </summary>
        public static object GetDefaultValue(this FieldInfo field)
        {
            if (field == null) return null;
            
            // 常量字段返回其值
            if (field.IsLiteral)
                return field.GetRawConstantValue();
            
            var type = field.FieldType;
            return type.IsValueType ? Activator.CreateInstance(type) : null;
        }

        /// <summary>
        /// 将字段设置为默认值
        /// </summary>
        public static bool SetToDefault(this FieldInfo field, object obj)
        {
            if (field == null || field.IsInitOnly || field.IsLiteral) return false;
            
            try
            {
                var defaultValue = field.FieldType.IsValueType 
                    ? Activator.CreateInstance(field.FieldType) 
                    : null;
                field.SetValue(obj, defaultValue);
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
        public static Func<object, T> CreateGetter<T>(this FieldInfo field)
        {
            if (field == null) return null;
            return obj => (T)field.GetValue(obj);
        }

        /// <summary>
        /// 创建 setter 委托
        /// </summary>
        public static Action<object, T> CreateSetter<T>(this FieldInfo field)
        {
            if (field == null || field.IsInitOnly || field.IsLiteral) return null;
            return (obj, value) => field.SetValue(obj, value);
        }

        #endregion
    }
}
