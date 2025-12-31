// ==========================================================
// 文件名：ArrayExtensions.cs
// 命名空间: AFramework.CSharpExtension
// 依赖: System, System.Linq
// ==========================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace AFramework.CSharpExtension
{
    /// <summary>
    /// 数组扩展方法
    /// <para>提供数组的常用操作扩展，包括查找、过滤、转换、随机等功能</para>
    /// </summary>
    /// <remarks>
    /// 使用示例:
    /// <code>
    /// int[] numbers = { 1, 2, 3, 4, 5 };
    /// 
    /// // 安全访问
    /// int value = numbers.GetOrDefault(10, -1); // 返回 -1
    /// 
    /// // 随机元素
    /// int random = numbers.Random();
    /// 
    /// // 填充
    /// numbers.Fill(0);
    /// </code>
    /// </remarks>
    public static class ArrayExtensions
    {
        #region 安全访问

        /// <summary>
        /// 安全获取数组元素，索引越界时返回默认值
        /// </summary>
        /// <typeparam name="T">元素类型</typeparam>
        /// <param name="array">数组</param>
        /// <param name="index">索引</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>元素或默认值</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T GetOrDefault<T>(this T[] array, int index, T defaultValue = default)
        {
            if (array == null || index < 0 || index >= array.Length)
                return defaultValue;
            return array[index];
        }

        /// <summary>
        /// 尝试获取数组元素
        /// </summary>
        /// <typeparam name="T">元素类型</typeparam>
        /// <param name="array">数组</param>
        /// <param name="index">索引</param>
        /// <param name="value">输出值</param>
        /// <returns>如果成功返回 true</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGet<T>(this T[] array, int index, out T value)
        {
            if (array != null && index >= 0 && index < array.Length)
            {
                value = array[index];
                return true;
            }
            value = default;
            return false;
        }

        /// <summary>
        /// 检查数组是否为空或 null
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNullOrEmpty<T>(this T[] array)
        {
            return array == null || array.Length == 0;
        }

        /// <summary>
        /// 检查数组是否有元素
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasElements<T>(this T[] array)
        {
            return array != null && array.Length > 0;
        }

        /// <summary>
        /// 检查索引是否在数组范围内
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsValidIndex<T>(this T[] array, int index)
        {
            return array != null && index >= 0 && index < array.Length;
        }

        #endregion

        #region 查找操作

        /// <summary>
        /// 查找第一个满足条件的元素索引
        /// </summary>
        /// <typeparam name="T">元素类型</typeparam>
        /// <param name="array">数组</param>
        /// <param name="predicate">条件</param>
        /// <returns>索引，未找到返回 -1</returns>
        public static int FindIndex<T>(this T[] array, Func<T, bool> predicate)
        {
            if (array == null || predicate == null)
                return -1;

            for (int i = 0; i < array.Length; i++)
            {
                if (predicate(array[i]))
                    return i;
            }
            return -1;
        }

        /// <summary>
        /// 查找最后一个满足条件的元素索引
        /// </summary>
        /// <typeparam name="T">元素类型</typeparam>
        /// <param name="array">数组</param>
        /// <param name="predicate">条件</param>
        /// <returns>索引，未找到返回 -1</returns>
        public static int FindLastIndex<T>(this T[] array, Func<T, bool> predicate)
        {
            if (array == null || predicate == null)
                return -1;

            for (int i = array.Length - 1; i >= 0; i--)
            {
                if (predicate(array[i]))
                    return i;
            }
            return -1;
        }

        /// <summary>
        /// 查找所有满足条件的元素索引
        /// </summary>
        /// <typeparam name="T">元素类型</typeparam>
        /// <param name="array">数组</param>
        /// <param name="predicate">条件</param>
        /// <returns>索引列表</returns>
        public static List<int> FindAllIndices<T>(this T[] array, Func<T, bool> predicate)
        {
            var indices = new List<int>();
            if (array == null || predicate == null)
                return indices;

            for (int i = 0; i < array.Length; i++)
            {
                if (predicate(array[i]))
                    indices.Add(i);
            }
            return indices;
        }

        /// <summary>
        /// 检查数组是否包含满足条件的元素
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Contains<T>(this T[] array, Func<T, bool> predicate)
        {
            return array.FindIndex(predicate) >= 0;
        }

        #endregion

        #region 填充和初始化

        /// <summary>
        /// 用指定值填充整个数组
        /// </summary>
        /// <typeparam name="T">元素类型</typeparam>
        /// <param name="array">数组</param>
        /// <param name="value">填充值</param>
        public static void Fill<T>(this T[] array, T value)
        {
            if (array == null)
                return;

            for (int i = 0; i < array.Length; i++)
            {
                array[i] = value;
            }
        }

        /// <summary>
        /// 用指定值填充数组的指定范围
        /// </summary>
        /// <typeparam name="T">元素类型</typeparam>
        /// <param name="array">数组</param>
        /// <param name="value">填充值</param>
        /// <param name="startIndex">起始索引</param>
        /// <param name="count">填充数量</param>
        public static void Fill<T>(this T[] array, T value, int startIndex, int count)
        {
            if (array == null || startIndex < 0 || count < 0)
                return;

            int endIndex = Math.Min(startIndex + count, array.Length);
            for (int i = startIndex; i < endIndex; i++)
            {
                array[i] = value;
            }
        }

        /// <summary>
        /// 用工厂方法初始化数组
        /// </summary>
        /// <typeparam name="T">元素类型</typeparam>
        /// <param name="array">数组</param>
        /// <param name="factory">工厂方法</param>
        public static void Initialize<T>(this T[] array, Func<T> factory)
        {
            if (array == null || factory == null)
                return;

            for (int i = 0; i < array.Length; i++)
            {
                array[i] = factory();
            }
        }

        /// <summary>
        /// 用工厂方法初始化数组（带索引参数）
        /// </summary>
        /// <typeparam name="T">元素类型</typeparam>
        /// <param name="array">数组</param>
        /// <param name="factory">工厂方法（参数为索引）</param>
        public static void Initialize<T>(this T[] array, Func<int, T> factory)
        {
            if (array == null || factory == null)
                return;

            for (int i = 0; i < array.Length; i++)
            {
                array[i] = factory(i);
            }
        }

        #endregion

        #region 随机操作

        private static readonly Random _random = new Random();

        /// <summary>
        /// 获取随机元素
        /// </summary>
        /// <typeparam name="T">元素类型</typeparam>
        /// <param name="array">数组</param>
        /// <returns>随机元素</returns>
        /// <exception cref="ArgumentException">数组为空时抛出</exception>
        public static T Random<T>(this T[] array)
        {
            if (array.IsNullOrEmpty())
                throw new ArgumentException("Array is null or empty.", nameof(array));

            return array[_random.Next(array.Length)];
        }

        /// <summary>
        /// 尝试获取随机元素
        /// </summary>
        /// <typeparam name="T">元素类型</typeparam>
        /// <param name="array">数组</param>
        /// <param name="value">输出值</param>
        /// <returns>如果成功返回 true</returns>
        public static bool TryRandom<T>(this T[] array, out T value)
        {
            if (array.HasElements())
            {
                value = array[_random.Next(array.Length)];
                return true;
            }
            value = default;
            return false;
        }

        /// <summary>
        /// 打乱数组顺序（Fisher-Yates 洗牌算法）
        /// </summary>
        /// <typeparam name="T">元素类型</typeparam>
        /// <param name="array">数组</param>
        public static void Shuffle<T>(this T[] array)
        {
            if (array == null || array.Length <= 1)
                return;

            for (int i = array.Length - 1; i > 0; i--)
            {
                int j = _random.Next(i + 1);
                T temp = array[i];
                array[i] = array[j];
                array[j] = temp;
            }
        }

        #endregion

        #region 转换操作

        /// <summary>
        /// 转换为 List
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static List<T> ToList<T>(this T[] array)
        {
            return array == null ? new List<T>() : new List<T>(array);
        }

        /// <summary>
        /// 转换为 HashSet
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static HashSet<T> ToHashSet<T>(this T[] array)
        {
            return array == null ? new HashSet<T>() : new HashSet<T>(array);
        }

        /// <summary>
        /// 映射为新数组
        /// </summary>
        /// <typeparam name="TSource">源类型</typeparam>
        /// <typeparam name="TResult">结果类型</typeparam>
        /// <param name="array">数组</param>
        /// <param name="selector">选择器</param>
        /// <returns>新数组</returns>
        public static TResult[] Map<TSource, TResult>(this TSource[] array, Func<TSource, TResult> selector)
        {
            if (array == null || selector == null)
                return Array.Empty<TResult>();

            var result = new TResult[array.Length];
            for (int i = 0; i < array.Length; i++)
            {
                result[i] = selector(array[i]);
            }
            return result;
        }

        #endregion

        #region 切片操作

        /// <summary>
        /// 获取数组切片
        /// </summary>
        /// <typeparam name="T">元素类型</typeparam>
        /// <param name="array">数组</param>
        /// <param name="startIndex">起始索引</param>
        /// <param name="count">元素数量</param>
        /// <returns>切片数组</returns>
        public static T[] Slice<T>(this T[] array, int startIndex, int count)
        {
            if (array == null || startIndex < 0 || count < 0)
                return Array.Empty<T>();

            startIndex = Math.Max(0, Math.Min(startIndex, array.Length));
            count = Math.Min(count, array.Length - startIndex);

            var result = new T[count];
            Array.Copy(array, startIndex, result, 0, count);
            return result;
        }

        /// <summary>
        /// 获取数组的前 N 个元素
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T[] Take<T>(this T[] array, int count)
        {
            return array.Slice(0, count);
        }

        /// <summary>
        /// 跳过前 N 个元素
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T[] Skip<T>(this T[] array, int count)
        {
            if (array == null)
                return Array.Empty<T>();
            return array.Slice(count, array.Length - count);
        }

        #endregion

        #region 比较操作

        /// <summary>
        /// 检查两个数组是否相等（元素顺序和值都相同）
        /// </summary>
        /// <typeparam name="T">元素类型</typeparam>
        /// <param name="array">数组</param>
        /// <param name="other">另一个数组</param>
        /// <returns>如果相等返回 true</returns>
        public static bool SequenceEqual<T>(this T[] array, T[] other)
        {
            if (array == null && other == null)
                return true;
            if (array == null || other == null)
                return false;
            if (array.Length != other.Length)
                return false;

            var comparer = EqualityComparer<T>.Default;
            for (int i = 0; i < array.Length; i++)
            {
                if (!comparer.Equals(array[i], other[i]))
                    return false;
            }
            return true;
        }

        #endregion

        #region 聚合操作

        /// <summary>
        /// 对数组元素执行聚合操作
        /// </summary>
        /// <typeparam name="T">元素类型</typeparam>
        /// <param name="array">数组</param>
        /// <param name="action">操作</param>
        public static void ForEach<T>(this T[] array, Action<T> action)
        {
            if (array == null || action == null)
                return;

            for (int i = 0; i < array.Length; i++)
            {
                action(array[i]);
            }
        }

        /// <summary>
        /// 对数组元素执行聚合操作（带索引）
        /// </summary>
        /// <typeparam name="T">元素类型</typeparam>
        /// <param name="array">数组</param>
        /// <param name="action">操作（参数为元素和索引）</param>
        public static void ForEach<T>(this T[] array, Action<T, int> action)
        {
            if (array == null || action == null)
                return;

            for (int i = 0; i < array.Length; i++)
            {
                action(array[i], i);
            }
        }

        #endregion

        #region 反转操作

        /// <summary>
        /// 反转数组（原地修改）
        /// </summary>
        /// <typeparam name="T">元素类型</typeparam>
        /// <param name="array">数组</param>
        public static void Reverse<T>(this T[] array)
        {
            if (array == null || array.Length <= 1)
                return;

            Array.Reverse(array);
        }

        /// <summary>
        /// 获取反转后的新数组
        /// </summary>
        /// <typeparam name="T">元素类型</typeparam>
        /// <param name="array">数组</param>
        /// <returns>反转后的新数组</returns>
        public static T[] Reversed<T>(this T[] array)
        {
            if (array == null)
                return Array.Empty<T>();

            var result = new T[array.Length];
            for (int i = 0; i < array.Length; i++)
            {
                result[i] = array[array.Length - 1 - i];
            }
            return result;
        }

        #endregion

        #region 清空操作

        /// <summary>
        /// 清空数组（所有元素设为默认值）
        /// </summary>
        /// <typeparam name="T">元素类型</typeparam>
        /// <param name="array">数组</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Clear<T>(this T[] array)
        {
            if (array != null)
                Array.Clear(array, 0, array.Length);
        }

        #endregion
    }
}
