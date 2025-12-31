// ==========================================================
// 文件名：IntExtensions.cs
// 命名空间: AFramework.CSharpExtension
// 依赖: System
// ==========================================================

using System;
using System.Runtime.CompilerServices;

namespace AFramework.CSharpExtension
{
    /// <summary>
    /// int/uint 扩展方法
    /// <para>提供整数的常用操作扩展，包括范围检查、数学运算、转换等功能</para>
    /// </summary>
    public static class IntExtensions
    {
        #region 范围检查

        /// <summary>
        /// 检查值是否在指定范围内（包含边界）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool InRange(this int value, int min, int max)
        {
            return value >= min && value <= max;
        }

        /// <summary>
        /// 检查值是否在指定范围内（不包含边界）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool InRangeExclusive(this int value, int min, int max)
        {
            return value > min && value < max;
        }

        /// <summary>
        /// 将值限制在指定范围内
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Clamp(this int value, int min, int max)
        {
            return Math.Max(min, Math.Min(max, value));
        }

        /// <summary>
        /// 检查是否为正数
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsPositive(this int value) => value > 0;

        /// <summary>
        /// 检查是否为负数
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNegative(this int value) => value < 0;

        /// <summary>
        /// 检查是否为零
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsZero(this int value) => value == 0;

        /// <summary>
        /// 检查是否为偶数
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsEven(this int value) => (value & 1) == 0;

        /// <summary>
        /// 检查是否为奇数
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsOdd(this int value) => (value & 1) != 0;

        #endregion

        #region 数学运算

        /// <summary>
        /// 获取绝对值
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Abs(this int value) => Math.Abs(value);

        /// <summary>
        /// 获取符号（-1, 0, 1）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Sign(this int value) => Math.Sign(value);

        /// <summary>
        /// 平方
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Square(this int value) => value * value;

        /// <summary>
        /// 立方
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Cube(this int value) => value * value * value;

        /// <summary>
        /// 幂运算
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Pow(this int value, int exponent)
        {
            return (int)Math.Pow(value, exponent);
        }

        /// <summary>
        /// 取模（确保结果为正数）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Mod(this int value, int divisor)
        {
            int result = value % divisor;
            return result < 0 ? result + divisor : result;
        }

        /// <summary>
        /// 最大值
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Max(this int value, int other) => Math.Max(value, other);

        /// <summary>
        /// 最小值
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Min(this int value, int other) => Math.Min(value, other);

        #endregion

        #region 循环操作

        /// <summary>
        /// 执行指定次数的操作
        /// </summary>
        /// <param name="count">次数</param>
        /// <param name="action">操作</param>
        public static void Times(this int count, Action action)
        {
            if (action == null || count <= 0)
                return;

            for (int i = 0; i < count; i++)
            {
                action();
            }
        }

        /// <summary>
        /// 执行指定次数的操作（带索引）
        /// </summary>
        /// <param name="count">次数</param>
        /// <param name="action">操作（参数为索引）</param>
        public static void Times(this int count, Action<int> action)
        {
            if (action == null || count <= 0)
                return;

            for (int i = 0; i < count; i++)
            {
                action(i);
            }
        }

        /// <summary>
        /// 从当前值到目标值的范围迭代
        /// </summary>
        /// <param name="from">起始值</param>
        /// <param name="to">结束值（包含）</param>
        /// <param name="action">操作</param>
        public static void To(this int from, int to, Action<int> action)
        {
            if (action == null)
                return;

            if (from <= to)
            {
                for (int i = from; i <= to; i++)
                    action(i);
            }
            else
            {
                for (int i = from; i >= to; i--)
                    action(i);
            }
        }

        /// <summary>
        /// 从当前值到目标值的范围迭代（指定步长）
        /// </summary>
        /// <param name="from">起始值</param>
        /// <param name="to">结束值（包含）</param>
        /// <param name="step">步长</param>
        /// <param name="action">操作</param>
        public static void To(this int from, int to, int step, Action<int> action)
        {
            if (action == null || step == 0)
                return;

            if (step > 0 && from <= to)
            {
                for (int i = from; i <= to; i += step)
                    action(i);
            }
            else if (step < 0 && from >= to)
            {
                for (int i = from; i >= to; i += step)
                    action(i);
            }
        }

        #endregion

        #region 转换操作

        /// <summary>
        /// 转换为字节数组（大端序）
        /// </summary>
        public static byte[] ToBytes(this int value)
        {
            return BitConverter.GetBytes(value);
        }

        /// <summary>
        /// 转换为十六进制字符串
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ToHex(this int value) => value.ToString("X");

        /// <summary>
        /// 转换为十六进制字符串（指定位数）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ToHex(this int value, int digits) => value.ToString($"X{digits}");

        /// <summary>
        /// 转换为二进制字符串
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ToBinary(this int value) => Convert.ToString(value, 2);

        /// <summary>
        /// 转换为带千位分隔符的字符串
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ToFormattedString(this int value) => value.ToString("N0");

        /// <summary>
        /// 转换为文件大小字符串（KB, MB, GB）
        /// </summary>
        public static string ToFileSizeString(this int bytes)
        {
            if (bytes < MathConstants.KB)
                return $"{bytes} B";
            if (bytes < MathConstants.MB)
                return $"{bytes / (double)MathConstants.KB:F2} KB";
            if (bytes < MathConstants.GB)
                return $"{bytes / (double)MathConstants.MB:F2} MB";
            return $"{bytes / (double)MathConstants.GB:F2} GB";
        }

        #endregion

        #region 位操作

        /// <summary>
        /// 检查指定位是否为 1
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsBitSet(this int value, int bitIndex)
        {
            return (value & (1 << bitIndex)) != 0;
        }

        /// <summary>
        /// 设置指定位为 1
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SetBit(this int value, int bitIndex)
        {
            return value | (1 << bitIndex);
        }

        /// <summary>
        /// 清除指定位（设为 0）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ClearBit(this int value, int bitIndex)
        {
            return value & ~(1 << bitIndex);
        }

        /// <summary>
        /// 切换指定位
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ToggleBit(this int value, int bitIndex)
        {
            return value ^ (1 << bitIndex);
        }

        /// <summary>
        /// 计算二进制中 1 的个数
        /// </summary>
        public static int CountBits(this int value)
        {
            int count = 0;
            while (value != 0)
            {
                count++;
                value &= value - 1; // 清除最低位的 1
            }
            return count;
        }

        #endregion

        #region 时间相关

        /// <summary>
        /// 转换为 TimeSpan（秒）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TimeSpan Seconds(this int value) => TimeSpan.FromSeconds(value);

        /// <summary>
        /// 转换为 TimeSpan（分钟）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TimeSpan Minutes(this int value) => TimeSpan.FromMinutes(value);

        /// <summary>
        /// 转换为 TimeSpan（小时）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TimeSpan Hours(this int value) => TimeSpan.FromHours(value);

        /// <summary>
        /// 转换为 TimeSpan（天）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TimeSpan Days(this int value) => TimeSpan.FromDays(value);

        /// <summary>
        /// 转换为 TimeSpan（毫秒）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TimeSpan Milliseconds(this int value) => TimeSpan.FromMilliseconds(value);

        #endregion

        #region 百分比操作

        /// <summary>
        /// 计算百分比
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="total">总数</param>
        /// <returns>百分比（0-100）</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float PercentOf(this int value, int total)
        {
            return total == 0 ? 0f : (value / (float)total) * 100f;
        }

        /// <summary>
        /// 计算指定百分比的值
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="percent">百分比（0-100）</param>
        /// <returns>结果值</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Percent(this int value, float percent)
        {
            return (int)(value * (percent / 100f));
        }

        #endregion

        #region uint 扩展

        /// <summary>
        /// 检查值是否在指定范围内（包含边界）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool InRange(this uint value, uint min, uint max)
        {
            return value >= min && value <= max;
        }

        /// <summary>
        /// 将值限制在指定范围内
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint Clamp(this uint value, uint min, uint max)
        {
            return Math.Max(min, Math.Min(max, value));
        }

        /// <summary>
        /// 检查是否为偶数
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsEven(this uint value) => (value & 1) == 0;

        /// <summary>
        /// 检查是否为奇数
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsOdd(this uint value) => (value & 1) != 0;

        #endregion
    }
}
