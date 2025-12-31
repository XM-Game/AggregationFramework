// ==========================================================
// 文件名：DelegateCompiler.cs
// 命名空间：AFramework.Burst
// 创建时间：2025-12-31
// 功能描述：委托编译为函数指针，提供委托到函数指针的转换工具
// ==========================================================

using System;
using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Burst.CompilerServices;

namespace AFramework.Burst
{
    /// <summary>
    /// 委托编译器
    /// 提供将委托编译为Burst函数指针的工具方法
    /// </summary>
    public static class DelegateCompiler
    {
        #region 编译方法

        /// <summary>
        /// 编译委托为函数指针
        /// </summary>
        /// <typeparam name="T">函数指针委托类型</typeparam>
        /// <param name="delegate">要编译的委托</param>
        /// <returns>编译后的函数指针</returns>
        /// <exception cref="ArgumentNullException">委托为null时抛出</exception>
        /// <exception cref="InvalidOperationException">委托未标记[BurstCompile]时抛出</exception>
        /// <remarks>
        /// 委托必须标记为 [BurstCompile] 特性才能成功编译
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FunctionPointer<T> Compile<T>(T @delegate) where T : Delegate
        {
            if (@delegate == null)
                throw new ArgumentNullException(nameof(@delegate), "委托不能为null");

            try
            {
                return BurstCompiler.CompileFunctionPointer(@delegate);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    $"无法编译委托为函数指针。请确保委托已标记 [BurstCompile] 特性。", ex);
            }
        }

        /// <summary>
        /// 尝试编译委托为函数指针
        /// </summary>
        /// <typeparam name="T">函数指针委托类型</typeparam>
        /// <param name="delegate">要编译的委托</param>
        /// <param name="functionPointer">输出的函数指针</param>
        /// <returns>如果编译成功返回true，否则返回false</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryCompile<T>(T @delegate, out FunctionPointer<T> functionPointer) where T : Delegate
        {
            functionPointer = default;

            if (@delegate == null)
                return false;

            try
            {
                functionPointer = BurstCompiler.CompileFunctionPointer(@delegate);
                return functionPointer.IsCreated;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 编译委托为函数指针（带缓存）
        /// </summary>
        /// <typeparam name="T">函数指针委托类型</typeparam>
        /// <param name="delegate">要编译的委托</param>
        /// <returns>编译后的函数指针（可能来自缓存）</returns>
        /// <remarks>
        /// 使用缓存机制，相同委托的多次调用将复用已编译的函数指针
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FunctionPointer<T> CompileWithCache<T>(T @delegate) where T : Delegate
        {
            return FunctionPointerCache<T>.GetOrCreate(@delegate);
        }

        #endregion

        #region 验证方法

        /// <summary>
        /// 检查委托是否可以编译为函数指针
        /// </summary>
        /// <typeparam name="T">函数指针委托类型</typeparam>
        /// <param name="delegate">要检查的委托</param>
        /// <returns>如果可以编译返回true，否则返回false</returns>
        /// <remarks>
        /// 此方法会尝试编译委托，但不抛出异常
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool CanCompile<T>(T @delegate) where T : Delegate
        {
            return TryCompile(@delegate, out _);
        }

        /// <summary>
        /// 验证委托是否已标记 [BurstCompile] 特性
        /// </summary>
        /// <param name="delegate">要验证的委托</param>
        /// <returns>如果已标记返回true，否则返回false</returns>
        /// <remarks>
        /// 注意：此方法通过反射检查特性，性能开销较大，仅用于调试
        /// </remarks>
        public static bool HasBurstCompileAttribute(Delegate @delegate)
        {
            if (@delegate == null)
                return false;

            var method = @delegate.Method;
            if (method == null)
                return false;

            // 检查方法上的特性
            var attributes = method.GetCustomAttributes(typeof(BurstCompileAttribute), false);
            if (attributes != null && attributes.Length > 0)
                return true;

            // 检查类型上的特性
            var type = method.DeclaringType;
            if (type != null)
            {
                attributes = type.GetCustomAttributes(typeof(BurstCompileAttribute), false);
                if (attributes != null && attributes.Length > 0)
                    return true;
            }

            return false;
        }

        #endregion

        #region 批量编译

        /// <summary>
        /// 批量编译多个委托为函数指针
        /// </summary>
        /// <typeparam name="T">函数指针委托类型</typeparam>
        /// <param name="delegates">要编译的委托数组</param>
        /// <returns>编译后的函数指针数组</returns>
        /// <exception cref="ArgumentNullException">委托数组为null时抛出</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FunctionPointer<T>[] CompileBatch<T>(T[] delegates) where T : Delegate
        {
            if (delegates == null)
                throw new ArgumentNullException(nameof(delegates));

            var results = new FunctionPointer<T>[delegates.Length];
            for (int i = 0; i < delegates.Length; i++)
            {
                results[i] = Compile(delegates[i]);
            }
            return results;
        }

        /// <summary>
        /// 尝试批量编译多个委托为函数指针
        /// </summary>
        /// <typeparam name="T">函数指针委托类型</typeparam>
        /// <param name="delegates">要编译的委托数组</param>
        /// <param name="functionPointers">输出的函数指针数组</param>
        /// <returns>成功编译的数量</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int TryCompileBatch<T>(T[] delegates, out FunctionPointer<T>[] functionPointers) where T : Delegate
        {
            functionPointers = null;

            if (delegates == null || delegates.Length == 0)
                return 0;

            functionPointers = new FunctionPointer<T>[delegates.Length];
            int successCount = 0;

            for (int i = 0; i < delegates.Length; i++)
            {
                if (TryCompile(delegates[i], out functionPointers[i]))
                {
                    successCount++;
                }
            }

            return successCount;
        }

        #endregion

        #region 辅助方法

        /// <summary>
        /// 获取函数指针的编译信息
        /// </summary>
        /// <typeparam name="T">函数指针委托类型</typeparam>
        /// <param name="functionPointer">函数指针</param>
        /// <returns>编译信息字符串</returns>
        public static string GetCompileInfo<T>(FunctionPointer<T> functionPointer) where T : Delegate
        {
            if (!functionPointer.IsCreated)
                return "函数指针未创建";

            return $"函数指针类型: {typeof(T).Name}, 已创建: {functionPointer.IsCreated}";
        }

        #endregion
    }
}

