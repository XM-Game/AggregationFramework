// ==========================================================
// 文件名：ThrowHelper.cs
// 命名空间: AFramework.DI.Internal
// 依赖: System, System.Collections.Generic
// ==========================================================

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace AFramework.DI.Internal
{
    /// <summary>
    /// 异常抛出辅助工具
    /// <para>集中管理异常抛出，提高代码可读性和性能</para>
    /// </summary>
    internal static class ThrowHelper
    {
        #region 参数验证

        /// <summary>
        /// 如果参数为 null 则抛出异常
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ThrowIfNull(object argument, string paramName)
        {
            if (argument == null)
            {
                ThrowArgumentNullException(paramName);
            }
        }

        /// <summary>
        /// 如果字符串为空或 null 则抛出异常
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ThrowIfNullOrEmpty(string argument, string paramName)
        {
            if (string.IsNullOrEmpty(argument))
            {
                ThrowArgumentException("参数不能为空", paramName);
            }
        }

        /// <summary>
        /// 如果类型为 null 则抛出异常
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ThrowIfTypeNull(Type type, string paramName)
        {
            if (type == null)
            {
                ThrowArgumentNullException(paramName);
            }
        }

        #endregion

        #region 异常抛出方法

        [DebuggerHidden]
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void ThrowArgumentNullException(string paramName)
        {
            throw new ArgumentNullException(paramName);
        }

        [DebuggerHidden]
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void ThrowArgumentException(string message, string paramName)
        {
            throw new ArgumentException(message, paramName);
        }

        [DebuggerHidden]
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void ThrowInvalidOperationException(string message)
        {
            throw new InvalidOperationException(message);
        }

        [DebuggerHidden]
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void ThrowObjectDisposedException(string objectName)
        {
            throw new ObjectDisposedException(objectName);
        }

        #endregion

        #region DI 特定异常

        /// <summary>
        /// 抛出服务未注册异常
        /// </summary>
        [DebuggerHidden]
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void ThrowServiceNotRegistered(Type serviceType)
        {
            throw ResolutionException.ServiceNotRegistered(serviceType);
        }

        /// <summary>
        /// 抛出服务未注册异常（带键值）
        /// </summary>
        [DebuggerHidden]
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void ThrowServiceNotRegistered(Type serviceType, object key)
        {
            throw ResolutionException.ServiceNotRegistered(serviceType, key);
        }

        /// <summary>
        /// 抛出循环依赖异常
        /// </summary>
        [DebuggerHidden]
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void ThrowCircularDependency(Type type, IEnumerable<Type> chain)
        {
            throw CircularDependencyException.Create(type, chain);
        }

        /// <summary>
        /// 抛出无法实例化抽象类型异常
        /// </summary>
        [DebuggerHidden]
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void ThrowCannotInstantiateAbstract(Type type)
        {
            throw ResolutionException.CannotInstantiateAbstract(type);
        }

        /// <summary>
        /// 抛出无合适构造函数异常
        /// </summary>
        [DebuggerHidden]
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void ThrowNoSuitableConstructor(Type type)
        {
            throw ResolutionException.NoSuitableConstructor(type);
        }

        #endregion

        #region 条件抛出

        /// <summary>
        /// 如果条件为真则抛出异常
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ThrowIf(bool condition, string message)
        {
            if (condition)
            {
                ThrowInvalidOperationException(message);
            }
        }

        /// <summary>
        /// 如果对象已释放则抛出异常
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ThrowIfDisposed(bool isDisposed, string objectName)
        {
            if (isDisposed)
            {
                ThrowObjectDisposedException(objectName);
            }
        }

        #endregion
    }
}
