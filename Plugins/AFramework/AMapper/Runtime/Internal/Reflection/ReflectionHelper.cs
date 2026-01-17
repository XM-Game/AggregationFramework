// ==========================================================
// 文件名：ReflectionHelper.cs
// 命名空间: AFramework.AMapper.Internal
// 依赖: System, System.Reflection
// 功能: 反射辅助类，提供常用反射操作
// ==========================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AFramework.AMapper.Internal
{
    /// <summary>
    /// 反射辅助类
    /// <para>提供常用反射操作的静态方法</para>
    /// <para>Reflection helper providing common reflection operations</para>
    /// </summary>
    public static class ReflectionHelper
    {
        #region 成员访问 / Member Access

        /// <summary>
        /// 获取成员类型
        /// <para>Get member type</para>
        /// </summary>
        /// <param name="member">成员信息 / Member info</param>
        /// <returns>成员类型 / Member type</returns>
        public static Type GetMemberType(MemberInfo member)
        {
            return member switch
            {
                PropertyInfo prop => prop.PropertyType,
                FieldInfo field => field.FieldType,
                MethodInfo method => method.ReturnType,
                _ => throw new ArgumentException($"不支持的成员类型 / Unsupported member type: {member?.MemberType}")
            };
        }

        /// <summary>
        /// 获取成员值
        /// <para>Get member value</para>
        /// </summary>
        /// <param name="member">成员信息 / Member info</param>
        /// <param name="obj">对象实例 / Object instance</param>
        /// <returns>成员值 / Member value</returns>
        public static object GetMemberValue(MemberInfo member, object obj)
        {
            return member switch
            {
                PropertyInfo prop => prop.GetValue(obj),
                FieldInfo field => field.GetValue(obj),
                _ => throw new ArgumentException($"不支持的成员类型 / Unsupported member type: {member?.MemberType}")
            };
        }

        /// <summary>
        /// 设置成员值
        /// <para>Set member value</para>
        /// </summary>
        /// <param name="member">成员信息 / Member info</param>
        /// <param name="obj">对象实例 / Object instance</param>
        /// <param name="value">值 / Value</param>
        public static void SetMemberValue(MemberInfo member, object obj, object value)
        {
            switch (member)
            {
                case PropertyInfo prop:
                    prop.SetValue(obj, value);
                    break;
                case FieldInfo field:
                    field.SetValue(obj, value);
                    break;
                default:
                    throw new ArgumentException($"不支持的成员类型 / Unsupported member type: {member?.MemberType}");
            }
        }

        /// <summary>
        /// 检查成员是否可读
        /// <para>Check if member is readable</para>
        /// </summary>
        /// <param name="member">成员信息 / Member info</param>
        /// <returns>是否可读 / Whether readable</returns>
        public static bool IsReadable(MemberInfo member)
        {
            return member switch
            {
                PropertyInfo prop => prop.CanRead,
                FieldInfo => true,
                _ => false
            };
        }

        /// <summary>
        /// 检查成员是否可写
        /// <para>Check if member is writable</para>
        /// </summary>
        /// <param name="member">成员信息 / Member info</param>
        /// <returns>是否可写 / Whether writable</returns>
        public static bool IsWritable(MemberInfo member)
        {
            return member switch
            {
                PropertyInfo prop => prop.CanWrite && prop.GetSetMethod() != null,
                FieldInfo field => !field.IsInitOnly,
                _ => false
            };
        }

        #endregion

        #region 类型检查 / Type Checking

        /// <summary>
        /// 检查是否是简单类型
        /// <para>Check if is simple type</para>
        /// </summary>
        /// <param name="type">类型 / Type</param>
        /// <returns>是否是简单类型 / Whether is simple type</returns>
        public static bool IsSimpleType(Type type)
        {
            var underlyingType = System.Nullable.GetUnderlyingType(type) ?? type;
            
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
        /// 检查是否是集合类型
        /// <para>Check if is collection type</para>
        /// </summary>
        /// <param name="type">类型 / Type</param>
        /// <returns>是否是集合类型 / Whether is collection type</returns>
        public static bool IsCollectionType(Type type)
        {
            return type != typeof(string) && 
                   typeof(System.Collections.IEnumerable).IsAssignableFrom(type);
        }

        /// <summary>
        /// 检查是否是字典类型
        /// <para>Check if is dictionary type</para>
        /// </summary>
        /// <param name="type">类型 / Type</param>
        /// <returns>是否是字典类型 / Whether is dictionary type</returns>
        public static bool IsDictionaryType(Type type)
        {
            return typeof(System.Collections.IDictionary).IsAssignableFrom(type) ||
                   (type.IsGenericType && 
                    (type.GetGenericTypeDefinition() == typeof(Dictionary<,>) ||
                     type.GetGenericTypeDefinition() == typeof(IDictionary<,>)));
        }

        /// <summary>
        /// 获取集合元素类型
        /// <para>Get collection element type</para>
        /// </summary>
        /// <param name="type">集合类型 / Collection type</param>
        /// <returns>元素类型 / Element type</returns>
        public static Type GetElementType(Type type)
        {
            if (type.IsArray)
                return type.GetElementType();

            if (type.IsGenericType)
            {
                var genericArgs = type.GetGenericArguments();
                if (genericArgs.Length == 1)
                    return genericArgs[0];
            }

            var enumerable = type.GetInterfaces()
                .FirstOrDefault(i => i.IsGenericType && 
                    i.GetGenericTypeDefinition() == typeof(IEnumerable<>));

            return enumerable?.GetGenericArguments()[0] ?? typeof(object);
        }

        /// <summary>
        /// 获取字典键值类型
        /// <para>Get dictionary key and value types</para>
        /// </summary>
        /// <param name="type">字典类型 / Dictionary type</param>
        /// <returns>键值类型元组 / Key-value type tuple</returns>
        public static (Type KeyType, Type ValueType) GetDictionaryTypes(Type type)
        {
            if (type.IsGenericType)
            {
                var genericArgs = type.GetGenericArguments();
                if (genericArgs.Length == 2)
                    return (genericArgs[0], genericArgs[1]);
            }

            var dictInterface = type.GetInterfaces()
                .FirstOrDefault(i => i.IsGenericType && 
                    i.GetGenericTypeDefinition() == typeof(IDictionary<,>));

            if (dictInterface != null)
            {
                var args = dictInterface.GetGenericArguments();
                return (args[0], args[1]);
            }

            return (typeof(object), typeof(object));
        }

        #endregion

        #region 构造函数 / Constructors

        /// <summary>
        /// 查找最佳构造函数
        /// <para>Find best constructor</para>
        /// </summary>
        /// <param name="type">类型 / Type</param>
        /// <param name="sourceType">源类型（用于参数匹配）/ Source type for parameter matching</param>
        /// <returns>构造函数信息 / Constructor info</returns>
        public static ConstructorInfo FindBestConstructor(Type type, Type sourceType = null)
        {
            var constructors = type.GetConstructors(BindingFlags.Public | BindingFlags.Instance)
                .OrderByDescending(c => c.GetParameters().Length)
                .ToList();

            if (constructors.Count == 0)
                return null;

            // 如果有源类型，尝试找到参数能匹配的构造函数
            if (sourceType != null)
            {
                var sourceDetails = TypeDetailsCache.GetDetails(sourceType);
                
                foreach (var ctor in constructors)
                {
                    var parameters = ctor.GetParameters();
                    var canResolve = parameters.All(p =>
                    {
                        var sourceMember = sourceDetails.FindReadableMember(p.Name);
                        return sourceMember != null || p.HasDefaultValue;
                    });

                    if (canResolve)
                        return ctor;
                }
            }

            // 返回默认构造函数或第一个构造函数
            return constructors.FirstOrDefault(c => c.GetParameters().Length == 0) ?? constructors[0];
        }

        #endregion

        #region 类型转换 / Type Conversion

        /// <summary>
        /// 检查是否可以隐式转换
        /// <para>Check if can implicitly convert</para>
        /// </summary>
        /// <param name="sourceType">源类型 / Source type</param>
        /// <param name="targetType">目标类型 / Target type</param>
        /// <returns>是否可以转换 / Whether can convert</returns>
        public static bool CanImplicitlyConvert(Type sourceType, Type targetType)
        {
            if (targetType.IsAssignableFrom(sourceType))
                return true;

            // 检查隐式转换运算符
            var implicitMethod = sourceType.GetMethods(BindingFlags.Public | BindingFlags.Static)
                .FirstOrDefault(m => m.Name == "op_Implicit" && 
                    m.ReturnType == targetType &&
                    m.GetParameters().Length == 1 &&
                    m.GetParameters()[0].ParameterType == sourceType);

            return implicitMethod != null;
        }

        /// <summary>
        /// 尝试转换值
        /// <para>Try to convert value</para>
        /// </summary>
        /// <param name="value">值 / Value</param>
        /// <param name="targetType">目标类型 / Target type</param>
        /// <param name="result">转换结果 / Conversion result</param>
        /// <returns>是否成功 / Whether successful</returns>
        public static bool TryConvert(object value, Type targetType, out object result)
        {
            result = null;

            if (value == null)
            {
                result = targetType.IsValueType ? Activator.CreateInstance(targetType) : null;
                return true;
            }

            var sourceType = value.GetType();

            if (targetType.IsAssignableFrom(sourceType))
            {
                result = value;
                return true;
            }

            try
            {
                result = Convert.ChangeType(value, targetType);
                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion
    }
}
