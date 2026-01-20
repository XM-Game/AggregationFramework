// ==========================================================
// 文件名：ObjectPoolExtensions.cs
// 命名空间: AFramework.Pool
// 依赖: System
// 功能: 对象池扩展方法，提供便捷操作
// ==========================================================

using System;

namespace AFramework.Pool
{
    /// <summary>
    /// 对象池扩展方法
    /// Object Pool Extension Methods
    /// 
    /// <para>为核心接口提供便捷的扩展方法</para>
    /// <para>Provides convenient extension methods for core interface</para>
    /// </summary>
    /// <remarks>
    /// 改进点：
    /// - 将便捷方法从接口移至扩展方法
    /// - 降低接口实现复杂度
    /// - 保持功能完整性
    /// </remarks>
    public static class ObjectPoolExtensions
    {
        #region 批量操作扩展

        /// <summary>
        /// 批量获取多个对象
        /// Get multiple objects in batch
        /// </summary>
        /// <typeparam name="T">对象类型 / Object type</typeparam>
        /// <param name="pool">对象池 / Object pool</param>
        /// <param name="count">数量 / Count</param>
        /// <returns>对象数组 / Object array</returns>
        public static T[] GetMany<T>(this IObjectPoolCore<T> pool, int count)
        {
            if (pool == null)
                throw new ArgumentNullException(nameof(pool));

            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count), "Count must be non-negative.");

            T[] objects = new T[count];
            for (int i = 0; i < count; i++)
            {
                objects[i] = pool.Get();
            }
            return objects;
        }

        /// <summary>
        /// 批量归还多个对象
        /// Return multiple objects in batch
        /// </summary>
        /// <typeparam name="T">对象类型 / Object type</typeparam>
        /// <param name="pool">对象池 / Object pool</param>
        /// <param name="objects">对象集合 / Object collection</param>
        /// <returns>成功归还的数量 / Successfully returned count</returns>
        public static int ReturnMany<T>(this IObjectPoolCore<T> pool, T[] objects)
        {
            if (pool == null)
                throw new ArgumentNullException(nameof(pool));

            if (objects == null)
                throw new ArgumentNullException(nameof(objects));

            int returnedCount = 0;
            foreach (var obj in objects)
            {
                if (obj != null && pool.Return(obj))
                {
                    returnedCount++;
                }
            }
            return returnedCount;
        }

        #endregion

        #region 安全操作扩展

        /// <summary>
        /// 尝试获取对象（不抛出异常）
        /// Try to get object (no exception)
        /// </summary>
        /// <typeparam name="T">对象类型 / Object type</typeparam>
        /// <param name="pool">对象池 / Object pool</param>
        /// <param name="obj">输出对象 / Output object</param>
        /// <returns>是否成功 / Whether successful</returns>
        public static bool TryGet<T>(this IObjectPoolCore<T> pool, out T obj)
        {
            if (pool == null)
                throw new ArgumentNullException(nameof(pool));

            try
            {
                obj = pool.Get();
                return true;
            }
            catch
            {
                obj = default;
                return false;
            }
        }

        /// <summary>
        /// 安全归还对象（忽略异常）
        /// Safely return object (ignore exceptions)
        /// </summary>
        /// <typeparam name="T">对象类型 / Object type</typeparam>
        /// <param name="pool">对象池 / Object pool</param>
        /// <param name="obj">要归还的对象 / Object to return</param>
        /// <returns>是否成功 / Whether successful</returns>
        public static bool TryReturn<T>(this IObjectPoolCore<T> pool, T obj)
        {
            if (pool == null || obj == null)
                return false;

            try
            {
                return pool.Return(obj);
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region 租借模式扩展



        #endregion

        #region 条件操作扩展

        /// <summary>
        /// 获取对象并执行操作
        /// Get object and execute action
        /// </summary>
        /// <typeparam name="T">对象类型 / Object type</typeparam>
        /// <param name="pool">对象池 / Object pool</param>
        /// <param name="action">要执行的操作 / Action to execute</param>
        public static void Use<T>(this IObjectPoolCore<T> pool, Action<T> action)
        {
            if (pool == null)
                throw new ArgumentNullException(nameof(pool));

            if (action == null)
                throw new ArgumentNullException(nameof(action));

            T obj = pool.Get();
            try
            {
                action(obj);
            }
            finally
            {
                pool.Return(obj);
            }
        }

        /// <summary>
        /// 获取对象并执行函数
        /// Get object and execute function
        /// </summary>
        /// <typeparam name="T">对象类型 / Object type</typeparam>
        /// <typeparam name="TResult">返回值类型 / Return type</typeparam>
        /// <param name="pool">对象池 / Object pool</param>
        /// <param name="func">要执行的函数 / Function to execute</param>
        /// <returns>函数返回值 / Function return value</returns>
        public static TResult Use<T, TResult>(this IObjectPoolCore<T> pool, Func<T, TResult> func)
        {
            if (pool == null)
                throw new ArgumentNullException(nameof(pool));

            if (func == null)
                throw new ArgumentNullException(nameof(func));

            T obj = pool.Get();
            try
            {
                return func(obj);
            }
            finally
            {
                pool.Return(obj);
            }
        }

        #endregion
    }


}
