// ==========================================================
// 文件名：ReflectionHelper.cs
// 命名空间: AFramework.DI.Internal
// 依赖: System, System.Reflection, System.Collections.Generic
// ==========================================================

using System;
using System.Collections.Generic;
using System.Reflection;

namespace AFramework.DI.Internal
{
    /// <summary>
    /// 反射辅助工具
    /// <para>提供反射相关的实用方法，用于依赖注入的成员发现和分析</para>
    /// </summary>
    internal static class ReflectionHelper
    {
        #region 常量定义

        /// <summary>
        /// 实例成员绑定标志
        /// </summary>
        private const BindingFlags InstanceBindingFlags =
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        /// <summary>
        /// 公共实例成员绑定标志
        /// </summary>
        private const BindingFlags PublicInstanceBindingFlags =
            BindingFlags.Instance | BindingFlags.Public;

        #endregion

        #region 构造函数

        /// <summary>
        /// 获取最佳构造函数
        /// <para>优先选择标记了 [Inject] 的构造函数，否则选择参数最多的公共构造函数</para>
        /// </summary>
        /// <param name="type">目标类型</param>
        /// <returns>最佳构造函数，如果没有找到则返回 null</returns>
        public static ConstructorInfo GetBestConstructor(Type type)
        {
            if (type == null) return null;

            var constructors = type.GetConstructors(InstanceBindingFlags);

            // 优先查找标记了 [Inject] 的构造函数
            foreach (var ctor in constructors)
            {
                if (ctor.GetCustomAttribute<InjectAttribute>() != null)
                {
                    return ctor;
                }
            }

            // 选择参数最多的公共构造函数
            ConstructorInfo best = null;
            int maxParams = -1;

            foreach (var ctor in constructors)
            {
                if (!ctor.IsPublic) continue;

                var paramCount = ctor.GetParameters().Length;
                if (paramCount > maxParams)
                {
                    maxParams = paramCount;
                    best = ctor;
                }
            }

            return best;
        }

        /// <summary>
        /// 获取所有公共构造函数
        /// </summary>
        /// <param name="type">目标类型</param>
        /// <returns>公共构造函数数组</returns>
        public static ConstructorInfo[] GetPublicConstructors(Type type)
        {
            return type?.GetConstructors(PublicInstanceBindingFlags)
                   ?? Array.Empty<ConstructorInfo>();
        }

        /// <summary>
        /// 获取标记了 [Inject] 的构造函数
        /// </summary>
        /// <param name="type">目标类型</param>
        /// <returns>标记了 [Inject] 的构造函数，如果没有则返回 null</returns>
        public static ConstructorInfo GetInjectConstructor(Type type)
        {
            if (type == null) return null;

            var constructors = type.GetConstructors(InstanceBindingFlags);
            foreach (var ctor in constructors)
            {
                if (ctor.GetCustomAttribute<InjectAttribute>() != null)
                {
                    return ctor;
                }
            }
            return null;
        }

        #endregion

        #region 字段注入

        /// <summary>
        /// 获取可注入的字段
        /// <para>返回标记了 [Inject] 特性的字段</para>
        /// </summary>
        /// <param name="type">目标类型</param>
        /// <returns>可注入字段列表</returns>
        public static List<FieldInfo> GetInjectableFields(Type type)
        {
            var result = new List<FieldInfo>();
            if (type == null) return result;

            // 遍历类型层次结构（包括基类）
            var currentType = type;
            while (currentType != null && currentType != typeof(object))
            {
                var fields = currentType.GetFields(InstanceBindingFlags | BindingFlags.DeclaredOnly);
                foreach (var field in fields)
                {
                    if (field.GetCustomAttribute<InjectAttribute>() != null)
                    {
                        result.Add(field);
                    }
                }
                currentType = currentType.BaseType;
            }

            return result;
        }

        /// <summary>
        /// 检查字段是否可注入
        /// </summary>
        /// <param name="field">字段信息</param>
        /// <returns>是否可注入</returns>
        public static bool IsInjectableField(FieldInfo field)
        {
            if (field == null) return false;
            if (field.IsInitOnly) return false; // readonly 字段不可注入
            if (field.IsLiteral) return false;  // const 字段不可注入
            return field.GetCustomAttribute<InjectAttribute>() != null;
        }

        #endregion

        #region 属性注入

        /// <summary>
        /// 获取可注入的属性
        /// <para>返回标记了 [Inject] 特性且有 setter 的属性</para>
        /// </summary>
        /// <param name="type">目标类型</param>
        /// <returns>可注入属性列表</returns>
        public static List<PropertyInfo> GetInjectableProperties(Type type)
        {
            var result = new List<PropertyInfo>();
            if (type == null) return result;

            // 遍历类型层次结构（包括基类）
            var currentType = type;
            while (currentType != null && currentType != typeof(object))
            {
                var properties = currentType.GetProperties(InstanceBindingFlags | BindingFlags.DeclaredOnly);
                foreach (var prop in properties)
                {
                    if (IsInjectableProperty(prop))
                    {
                        result.Add(prop);
                    }
                }
                currentType = currentType.BaseType;
            }

            return result;
        }

        /// <summary>
        /// 检查属性是否可注入
        /// </summary>
        /// <param name="property">属性信息</param>
        /// <returns>是否可注入</returns>
        public static bool IsInjectableProperty(PropertyInfo property)
        {
            if (property == null) return false;
            if (!property.CanWrite) return false; // 必须有 setter
            if (property.GetIndexParameters().Length > 0) return false; // 排除索引器
            return property.GetCustomAttribute<InjectAttribute>() != null;
        }

        #endregion

        #region 方法注入

        /// <summary>
        /// 获取可注入的方法
        /// <para>返回标记了 [Inject] 特性的方法</para>
        /// </summary>
        /// <param name="type">目标类型</param>
        /// <returns>可注入方法列表</returns>
        public static List<MethodInfo> GetInjectableMethods(Type type)
        {
            var result = new List<MethodInfo>();
            if (type == null) return result;

            // 遍历类型层次结构（包括基类）
            var currentType = type;
            while (currentType != null && currentType != typeof(object))
            {
                var methods = currentType.GetMethods(InstanceBindingFlags | BindingFlags.DeclaredOnly);
                foreach (var method in methods)
                {
                    if (IsInjectableMethod(method))
                    {
                        result.Add(method);
                    }
                }
                currentType = currentType.BaseType;
            }

            return result;
        }

        /// <summary>
        /// 检查方法是否可注入
        /// </summary>
        /// <param name="method">方法信息</param>
        /// <returns>是否可注入</returns>
        public static bool IsInjectableMethod(MethodInfo method)
        {
            if (method == null) return false;
            if (method.IsAbstract) return false;
            if (method.IsGenericMethodDefinition) return false; // 排除开放泛型方法
            if (method.IsSpecialName) return false; // 排除属性访问器等特殊方法
            return method.GetCustomAttribute<InjectAttribute>() != null;
        }

        #endregion


        #region 参数信息

        /// <summary>
        /// 获取构造函数参数类型数组
        /// </summary>
        /// <param name="constructor">构造函数</param>
        /// <returns>参数类型数组</returns>
        public static Type[] GetParameterTypes(ConstructorInfo constructor)
        {
            if (constructor == null) return Array.Empty<Type>();

            var parameters = constructor.GetParameters();
            var types = new Type[parameters.Length];
            for (int i = 0; i < parameters.Length; i++)
            {
                types[i] = parameters[i].ParameterType;
            }
            return types;
        }

        /// <summary>
        /// 获取方法参数类型数组
        /// </summary>
        /// <param name="method">方法</param>
        /// <returns>参数类型数组</returns>
        public static Type[] GetParameterTypes(MethodInfo method)
        {
            if (method == null) return Array.Empty<Type>();

            var parameters = method.GetParameters();
            var types = new Type[parameters.Length];
            for (int i = 0; i < parameters.Length; i++)
            {
                types[i] = parameters[i].ParameterType;
            }
            return types;
        }

        /// <summary>
        /// 检查参数是否有默认值
        /// </summary>
        /// <param name="parameter">参数信息</param>
        /// <returns>是否有默认值</returns>
        public static bool HasDefaultValue(ParameterInfo parameter)
        {
            return parameter != null && parameter.HasDefaultValue;
        }

        /// <summary>
        /// 获取参数的默认值
        /// </summary>
        /// <param name="parameter">参数信息</param>
        /// <returns>默认值，如果没有则返回 null</returns>
        public static object GetDefaultValue(ParameterInfo parameter)
        {
            if (parameter == null || !parameter.HasDefaultValue)
                return null;
            return parameter.DefaultValue;
        }

        #endregion

        #region 特性检查

        /// <summary>
        /// 检查成员是否标记了指定特性
        /// </summary>
        /// <typeparam name="TAttribute">特性类型</typeparam>
        /// <param name="member">成员信息</param>
        /// <returns>是否标记了特性</returns>
        public static bool HasAttribute<TAttribute>(MemberInfo member) where TAttribute : Attribute
        {
            return member?.GetCustomAttribute<TAttribute>() != null;
        }

        /// <summary>
        /// 获取成员上的指定特性
        /// </summary>
        /// <typeparam name="TAttribute">特性类型</typeparam>
        /// <param name="member">成员信息</param>
        /// <returns>特性实例，如果没有则返回 null</returns>
        public static TAttribute GetAttribute<TAttribute>(MemberInfo member) where TAttribute : Attribute
        {
            return member?.GetCustomAttribute<TAttribute>();
        }

        /// <summary>
        /// 获取参数上的指定特性
        /// </summary>
        /// <typeparam name="TAttribute">特性类型</typeparam>
        /// <param name="parameter">参数信息</param>
        /// <returns>特性实例，如果没有则返回 null</returns>
        public static TAttribute GetAttribute<TAttribute>(ParameterInfo parameter) where TAttribute : Attribute
        {
            return parameter?.GetCustomAttribute<TAttribute>();
        }

        #endregion

        #region 类型成员获取

        /// <summary>
        /// 获取类型的所有实例字段（包括基类）
        /// </summary>
        /// <param name="type">目标类型</param>
        /// <returns>字段列表</returns>
        public static List<FieldInfo> GetAllInstanceFields(Type type)
        {
            var result = new List<FieldInfo>();
            if (type == null) return result;

            var currentType = type;
            while (currentType != null && currentType != typeof(object))
            {
                var fields = currentType.GetFields(InstanceBindingFlags | BindingFlags.DeclaredOnly);
                result.AddRange(fields);
                currentType = currentType.BaseType;
            }

            return result;
        }

        /// <summary>
        /// 获取类型的所有实例属性（包括基类）
        /// </summary>
        /// <param name="type">目标类型</param>
        /// <returns>属性列表</returns>
        public static List<PropertyInfo> GetAllInstanceProperties(Type type)
        {
            var result = new List<PropertyInfo>();
            if (type == null) return result;

            var currentType = type;
            while (currentType != null && currentType != typeof(object))
            {
                var properties = currentType.GetProperties(InstanceBindingFlags | BindingFlags.DeclaredOnly);
                result.AddRange(properties);
                currentType = currentType.BaseType;
            }

            return result;
        }

        /// <summary>
        /// 获取类型的所有实例方法（包括基类）
        /// </summary>
        /// <param name="type">目标类型</param>
        /// <returns>方法列表</returns>
        public static List<MethodInfo> GetAllInstanceMethods(Type type)
        {
            var result = new List<MethodInfo>();
            if (type == null) return result;

            var currentType = type;
            while (currentType != null && currentType != typeof(object))
            {
                var methods = currentType.GetMethods(InstanceBindingFlags | BindingFlags.DeclaredOnly);
                foreach (var method in methods)
                {
                    if (!method.IsSpecialName) // 排除属性访问器等
                    {
                        result.Add(method);
                    }
                }
                currentType = currentType.BaseType;
            }

            return result;
        }

        #endregion

        #region 委托创建

        /// <summary>
        /// 创建字段设置委托
        /// </summary>
        /// <param name="field">字段信息</param>
        /// <returns>设置委托</returns>
        public static Action<object, object> CreateFieldSetter(FieldInfo field)
        {
            if (field == null) return null;
            return (target, value) => field.SetValue(target, value);
        }

        /// <summary>
        /// 创建属性设置委托
        /// </summary>
        /// <param name="property">属性信息</param>
        /// <returns>设置委托</returns>
        public static Action<object, object> CreatePropertySetter(PropertyInfo property)
        {
            if (property == null || !property.CanWrite) return null;
            var setter = property.GetSetMethod(true);
            if (setter == null) return null;
            return (target, value) => setter.Invoke(target, new[] { value });
        }

        #endregion
    }
}
