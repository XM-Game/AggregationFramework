// ==========================================================
// 文件名：MethodInfoExtensions.cs
// 命名空间: AFramework.CSharpExtension
// 依赖: System, System.Reflection, System.Linq
// ==========================================================

using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace AFramework.CSharpExtension
{
    /// <summary>
    /// MethodInfo 扩展方法
    /// <para>提供方法信息的常用操作扩展，包括调用、参数检查、签名获取等功能</para>
    /// </summary>
    public static class MethodInfoExtensions
    {
        #region 方法调用

        /// <summary>
        /// 安全调用方法（捕获异常）
        /// </summary>
        public static object InvokeSafe(this MethodInfo method, object obj, params object[] parameters)
        {
            if (method == null) return null;
            
            try
            {
                return method.Invoke(obj, parameters);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 调用方法并返回指定类型的结果
        /// </summary>
        public static T Invoke<T>(this MethodInfo method, object obj, params object[] parameters)
        {
            if (method == null) return default;
            
            var result = method.Invoke(obj, parameters);
            return result is T typed ? typed : default;
        }

        /// <summary>
        /// 安全调用方法并返回指定类型的结果
        /// </summary>
        public static T InvokeSafe<T>(this MethodInfo method, object obj, params object[] parameters)
        {
            if (method == null) return default;
            
            try
            {
                var result = method.Invoke(obj, parameters);
                return result is T typed ? typed : default;
            }
            catch
            {
                return default;
            }
        }

        /// <summary>
        /// 尝试调用方法
        /// </summary>
        public static bool TryInvoke(this MethodInfo method, object obj, out object result, params object[] parameters)
        {
            result = null;
            if (method == null) return false;
            
            try
            {
                result = method.Invoke(obj, parameters);
                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region 参数信息

        /// <summary>
        /// 获取参数数量
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetParameterCount(this MethodInfo method)
        {
            return method?.GetParameters().Length ?? 0;
        }

        /// <summary>
        /// 获取参数类型数组
        /// </summary>
        public static Type[] GetParameterTypes(this MethodInfo method)
        {
            if (method == null) return Array.Empty<Type>();
            return Array.ConvertAll(method.GetParameters(), p => p.ParameterType);
        }

        /// <summary>
        /// 获取参数名称数组
        /// </summary>
        public static string[] GetParameterNames(this MethodInfo method)
        {
            if (method == null) return Array.Empty<string>();
            return Array.ConvertAll(method.GetParameters(), p => p.Name);
        }

        /// <summary>
        /// 检查是否有参数
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasParameters(this MethodInfo method)
        {
            return method != null && method.GetParameters().Length > 0;
        }

        /// <summary>
        /// 检查是否为无参方法
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsParameterless(this MethodInfo method)
        {
            return method != null && method.GetParameters().Length == 0;
        }

        /// <summary>
        /// 检查参数是否匹配指定类型
        /// </summary>
        public static bool ParametersMatch(this MethodInfo method, params Type[] types)
        {
            if (method == null) return false;
            
            var parameters = method.GetParameters();
            if (parameters.Length != (types?.Length ?? 0)) return false;
            
            for (int i = 0; i < parameters.Length; i++)
            {
                if (parameters[i].ParameterType != types[i])
                    return false;
            }
            return true;
        }

        #endregion

        #region 返回类型

        /// <summary>
        /// 检查是否有返回值
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasReturnValue(this MethodInfo method)
        {
            return method != null && method.ReturnType != typeof(void);
        }

        /// <summary>
        /// 检查返回类型是否为指定类型
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ReturnsType<T>(this MethodInfo method)
        {
            return method != null && method.ReturnType == typeof(T);
        }

        /// <summary>
        /// 检查返回类型是否可赋值给指定类型
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ReturnsAssignableTo<T>(this MethodInfo method)
        {
            return method != null && typeof(T).IsAssignableFrom(method.ReturnType);
        }

        #endregion

        #region 方法特性检查

        /// <summary>
        /// 检查是否为虚方法
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsVirtualMethod(this MethodInfo method)
        {
            return method != null && method.IsVirtual && !method.IsFinal;
        }

        /// <summary>
        /// 检查是否为抽象方法
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsAbstractMethod(this MethodInfo method)
        {
            return method?.IsAbstract ?? false;
        }

        /// <summary>
        /// 检查是否为重写方法
        /// </summary>
        public static bool IsOverride(this MethodInfo method)
        {
            if (method == null) return false;
            return method.GetBaseDefinition().DeclaringType != method.DeclaringType;
        }

        /// <summary>
        /// 检查是否为扩展方法
        /// </summary>
        public static bool IsExtensionMethod(this MethodInfo method)
        {
            if (method == null) return false;
            return method.IsDefined(typeof(System.Runtime.CompilerServices.ExtensionAttribute), false);
        }

        /// <summary>
        /// 检查是否为异步方法
        /// </summary>
        public static bool IsAsyncMethod(this MethodInfo method)
        {
            if (method == null) return false;
            return method.IsDefined(typeof(AsyncStateMachineAttribute), false);
        }

        /// <summary>
        /// 检查是否为泛型方法
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsGenericMethod(this MethodInfo method)
        {
            return method?.IsGenericMethod ?? false;
        }

        /// <summary>
        /// 检查是否为泛型方法定义
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsGenericMethodDefinition(this MethodInfo method)
        {
            return method?.IsGenericMethodDefinition ?? false;
        }

        #endregion

        #region 签名操作

        /// <summary>
        /// 获取方法签名字符串
        /// </summary>
        public static string GetSignature(this MethodInfo method)
        {
            if (method == null) return string.Empty;
            
            var parameters = string.Join(", ", method.GetParameters()
                .Select(p => $"{p.ParameterType.Name} {p.Name}"));
            
            return $"{method.ReturnType.Name} {method.Name}({parameters})";
        }

        /// <summary>
        /// 获取完整方法签名（包含声明类型）
        /// </summary>
        public static string GetFullSignature(this MethodInfo method)
        {
            if (method == null) return string.Empty;
            
            var declaringType = method.DeclaringType?.FullName ?? "Unknown";
            var parameters = string.Join(", ", method.GetParameters()
                .Select(p => $"{p.ParameterType.FullName} {p.Name}"));
            
            return $"{method.ReturnType.FullName} {declaringType}.{method.Name}({parameters})";
        }

        #endregion

        #region 泛型操作

        /// <summary>
        /// 安全创建泛型方法
        /// </summary>
        public static MethodInfo MakeGenericMethodSafe(this MethodInfo method, params Type[] typeArguments)
        {
            if (method == null || !method.IsGenericMethodDefinition) return null;
            
            try
            {
                return method.MakeGenericMethod(typeArguments);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 获取泛型方法定义
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static MethodInfo GetGenericDefinition(this MethodInfo method)
        {
            return method != null && method.IsGenericMethod ? method.GetGenericMethodDefinition() : null;
        }

        /// <summary>
        /// 获取泛型参数类型
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Type[] GetGenericArgumentTypes(this MethodInfo method)
        {
            return method?.GetGenericArguments() ?? Array.Empty<Type>();
        }

        #endregion

        #region 委托创建

        /// <summary>
        /// 创建 Action 委托（无参无返回值方法）
        /// </summary>
        public static Action CreateAction(this MethodInfo method, object target = null)
        {
            if (method == null) return null;
            
            try
            {
                return (Action)Delegate.CreateDelegate(typeof(Action), target, method);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 创建 Action&lt;T&gt; 委托
        /// </summary>
        public static Action<T> CreateAction<T>(this MethodInfo method, object target = null)
        {
            if (method == null) return null;
            
            try
            {
                return (Action<T>)Delegate.CreateDelegate(typeof(Action<T>), target, method);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 创建 Func&lt;TResult&gt; 委托
        /// </summary>
        public static Func<TResult> CreateFunc<TResult>(this MethodInfo method, object target = null)
        {
            if (method == null) return null;
            
            try
            {
                return (Func<TResult>)Delegate.CreateDelegate(typeof(Func<TResult>), target, method);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 创建 Func&lt;T, TResult&gt; 委托
        /// </summary>
        public static Func<T, TResult> CreateFunc<T, TResult>(this MethodInfo method, object target = null)
        {
            if (method == null) return null;
            
            try
            {
                return (Func<T, TResult>)Delegate.CreateDelegate(typeof(Func<T, TResult>), target, method);
            }
            catch
            {
                return null;
            }
        }

        #endregion

        #region 基方法

        /// <summary>
        /// 获取基类中的方法定义
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static MethodInfo GetBaseMethod(this MethodInfo method)
        {
            return method?.GetBaseDefinition();
        }

        /// <summary>
        /// 检查是否重写了基类方法
        /// </summary>
        public static bool OverridesBaseMethod(this MethodInfo method)
        {
            if (method == null) return false;
            var baseMethod = method.GetBaseDefinition();
            return baseMethod != null && baseMethod.DeclaringType != method.DeclaringType;
        }

        #endregion
    }
}
