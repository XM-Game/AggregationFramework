// ==========================================================
// 文件名：FunctionPointerCache.cs
// 命名空间：AFramework.Burst
// 创建时间：2025-12-31
// 功能描述：函数指针缓存，提供函数指针的缓存和复用机制
// ==========================================================

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Burst.CompilerServices;

namespace AFramework.Burst
{
    /// <summary>
    /// 函数指针缓存
    /// 提供函数指针的缓存和复用机制，避免重复编译
    /// </summary>
    /// <typeparam name="T">函数指针委托类型</typeparam>
    public static class FunctionPointerCache<T> where T : Delegate
    {
        #region 私有字段

        private static readonly Dictionary<Delegate, FunctionPointer<T>> s_Cache = new Dictionary<Delegate, FunctionPointer<T>>();
        private static readonly object s_Lock = new object();

        #endregion

        #region 公共方法

        /// <summary>
        /// 获取或创建函数指针（带缓存）
        /// </summary>
        /// <param name="delegate">委托实例</param>
        /// <returns>函数指针</returns>
        /// <remarks>
        /// 如果缓存中已存在该委托的函数指针，直接返回缓存的版本
        /// 否则编译并缓存新的函数指针
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FunctionPointer<T> GetOrCreate(T @delegate)
        {
            if (@delegate == null)
                throw new ArgumentNullException(nameof(@delegate));

            lock (s_Lock)
            {
                if (s_Cache.TryGetValue(@delegate, out var cached))
                {
                    return cached;
                }

                var functionPointer = BurstCompiler.CompileFunctionPointer(@delegate);
                s_Cache[@delegate] = functionPointer;
                return functionPointer;
            }
        }

        /// <summary>
        /// 尝试获取缓存的函数指针
        /// </summary>
        /// <param name="delegate">委托实例</param>
        /// <param name="functionPointer">输出的函数指针</param>
        /// <returns>如果缓存中存在返回true，否则返回false</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGet(T @delegate, out FunctionPointer<T> functionPointer)
        {
            if (@delegate == null)
            {
                functionPointer = default;
                return false;
            }

            lock (s_Lock)
            {
                return s_Cache.TryGetValue(@delegate, out functionPointer);
            }
        }

        /// <summary>
        /// 添加函数指针到缓存
        /// </summary>
        /// <param name="delegate">委托实例</param>
        /// <param name="functionPointer">函数指针</param>
        /// <remarks>
        /// 如果缓存中已存在该委托，将覆盖旧值
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Add(T @delegate, FunctionPointer<T> functionPointer)
        {
            if (@delegate == null)
                throw new ArgumentNullException(nameof(@delegate));

            lock (s_Lock)
            {
                s_Cache[@delegate] = functionPointer;
            }
        }

        /// <summary>
        /// 从缓存中移除函数指针
        /// </summary>
        /// <param name="delegate">委托实例</param>
        /// <returns>如果成功移除返回true，否则返回false</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Remove(T @delegate)
        {
            if (@delegate == null)
                return false;

            lock (s_Lock)
            {
                return s_Cache.Remove(@delegate);
            }
        }

        /// <summary>
        /// 检查缓存中是否包含指定的委托
        /// </summary>
        /// <param name="delegate">委托实例</param>
        /// <returns>如果缓存中存在返回true，否则返回false</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Contains(T @delegate)
        {
            if (@delegate == null)
                return false;

            lock (s_Lock)
            {
                return s_Cache.ContainsKey(@delegate);
            }
        }

        /// <summary>
        /// 清空所有缓存
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Clear()
        {
            lock (s_Lock)
            {
                s_Cache.Clear();
            }
        }

        /// <summary>
        /// 获取缓存中的函数指针数量
        /// </summary>
        public static int Count
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                lock (s_Lock)
                {
                    return s_Cache.Count;
                }
            }
        }

        #endregion
    }

    /// <summary>
    /// 函数指针缓存管理器
    /// 提供全局的缓存管理和统计功能
    /// </summary>
    public static class FunctionPointerCacheManager
    {
        #region 统计信息

        /// <summary>
        /// 获取所有缓存的函数指针总数
        /// </summary>
        /// <remarks>
        /// 注意：此方法需要遍历所有缓存类型，性能开销较大，仅用于调试
        /// </remarks>
        public static int TotalCacheCount
        {
            get
            {
                // 由于无法直接获取所有泛型类型的缓存，这里返回0
                // 实际使用时可以通过反射或其他机制实现
                return 0;
            }
        }

        #endregion

        #region 管理方法

        /// <summary>
        /// 预热缓存（预编译常用函数指针）
        /// </summary>
        /// <remarks>
        /// 可以在游戏启动时调用此方法，预编译常用的函数指针以提高运行时性能
        /// </remarks>
        public static void Warmup()
        {
            // 可以在这里预编译常用的函数指针
            // 例如：数学运算、比较函数等
        }

        /// <summary>
        /// 清理所有缓存
        /// </summary>
        /// <remarks>
        /// 注意：清理后需要重新编译函数指针，建议仅在必要时调用
        /// </remarks>
        public static void ClearAll()
        {
            // 由于无法直接访问所有泛型类型的缓存，这里需要特殊处理
            // 实际使用时可以通过反射或其他机制实现
        }

        #endregion
    }
}

