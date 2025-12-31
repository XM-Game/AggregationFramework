// ==========================================================
// 文件名：FunctionPointerUtility.cs
// 命名空间：AFramework.Burst
// 创建时间：2025-12-31
// 功能描述：函数指针工具类，提供函数指针的创建、转换和验证功能
// ==========================================================

using System;
using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Burst.CompilerServices;

namespace AFramework.Burst
{
    /// <summary>
    /// 函数指针工具类
    /// 提供函数指针的创建、转换、验证等实用方法
    /// </summary>
    public static class FunctionPointerUtility
    {
        #region 函数指针创建

        /// <summary>
        /// 从委托创建函数指针
        /// </summary>
        /// <typeparam name="T">函数指针类型</typeparam>
        /// <param name="delegate">委托实例</param>
        /// <returns>函数指针</returns>
        /// <remarks>
        /// 注意：委托必须标记为 [BurstCompile] 才能转换为函数指针
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FunctionPointer<T> FromDelegate<T>(T @delegate) where T : Delegate
        {
            if (@delegate == null)
                throw new ArgumentNullException(nameof(@delegate));

            return BurstCompiler.CompileFunctionPointer(@delegate);
        }

        /// <summary>
        /// 从静态方法创建函数指针
        /// </summary>
        /// <typeparam name="T">函数指针类型</typeparam>
        /// <param name="method">静态方法委托</param>
        /// <returns>函数指针</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FunctionPointer<T> FromStaticMethod<T>(T method) where T : Delegate
        {
            return FromDelegate(method);
        }

        /// <summary>
        /// 创建空函数指针（用于可选参数）
        /// </summary>
        /// <typeparam name="T">函数指针类型</typeparam>
        /// <returns>空函数指针</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FunctionPointer<T> Empty<T>() where T : Delegate
        {
            return default;
        }

        #endregion

        #region 函数指针验证

        /// <summary>
        /// 检查函数指针是否有效
        /// </summary>
        /// <typeparam name="T">函数指针类型</typeparam>
        /// <param name="functionPointer">要检查的函数指针</param>
        /// <returns>如果函数指针有效返回true，否则返回false</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsValid<T>(FunctionPointer<T> functionPointer) where T : Delegate
        {
            return functionPointer.IsCreated;
        }

        /// <summary>
        /// 验证函数指针是否有效，如果无效则抛出异常
        /// </summary>
        /// <typeparam name="T">函数指针类型</typeparam>
        /// <param name="functionPointer">要验证的函数指针</param>
        /// <param name="paramName">参数名称（用于异常消息）</param>
        /// <exception cref="ArgumentNullException">函数指针无效时抛出</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Validate<T>(FunctionPointer<T> functionPointer, string paramName = "functionPointer") 
            where T : Delegate
        {
            if (!IsValid(functionPointer))
                throw new ArgumentNullException(paramName, "函数指针无效或未初始化");
        }

        #endregion

        #region 函数指针调用

        /// <summary>
        /// 安全调用函数指针（带空值检查）
        /// </summary>
        /// <typeparam name="T">函数指针类型</typeparam>
        /// <param name="functionPointer">函数指针</param>
        /// <param name="invoke">调用函数</param>
        /// <returns>如果函数指针有效并成功调用返回true，否则返回false</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryInvoke<T>(FunctionPointer<T> functionPointer, Action<FunctionPointer<T>> invoke) 
            where T : Delegate
        {
            if (!IsValid(functionPointer))
                return false;

            try
            {
                invoke(functionPointer);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 安全调用函数指针并返回值（带空值检查）
        /// </summary>
        /// <typeparam name="T">函数指针类型</typeparam>
        /// <typeparam name="TResult">返回值类型</typeparam>
        /// <param name="functionPointer">函数指针</param>
        /// <param name="invoke">调用函数</param>
        /// <param name="result">返回值</param>
        /// <returns>如果函数指针有效并成功调用返回true，否则返回false</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryInvoke<T, TResult>(
            FunctionPointer<T> functionPointer, 
            Func<FunctionPointer<T>, TResult> invoke, 
            out TResult result) 
            where T : Delegate
        {
            result = default;
            
            if (!IsValid(functionPointer))
                return false;

            try
            {
                result = invoke(functionPointer);
                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region 函数指针组合

        /// <summary>
        /// 组合两个函数指针（先执行第一个，再执行第二个）
        /// </summary>
        /// <typeparam name="T">函数指针类型</typeparam>
        /// <param name="first">第一个函数指针</param>
        /// <param name="second">第二个函数指针</param>
        /// <returns>组合后的函数指针包装器</returns>
        /// <remarks>
        /// 注意：此方法返回一个包装器，实际组合逻辑需要在调用时实现
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FunctionPointerPair<T> Combine<T>(
            FunctionPointer<T> first, 
            FunctionPointer<T> second) 
            where T : Delegate
        {
            return new FunctionPointerPair<T>(first, second);
        }

        #endregion

        #region 辅助类型

        /// <summary>
        /// 函数指针对（用于组合操作）
        /// </summary>
        /// <typeparam name="T">函数指针类型</typeparam>
        public struct FunctionPointerPair<T> where T : Delegate
        {
            /// <summary>第一个函数指针</summary>
            public FunctionPointer<T> First;
            
            /// <summary>第二个函数指针</summary>
            public FunctionPointer<T> Second;

            /// <summary>
            /// 构造函数
            /// </summary>
            public FunctionPointerPair(FunctionPointer<T> first, FunctionPointer<T> second)
            {
                First = first;
                Second = second;
            }

            /// <summary>
            /// 检查两个函数指针是否都有效
            /// </summary>
            public bool BothValid => First.IsCreated && Second.IsCreated;
        }

        #endregion
    }
}

