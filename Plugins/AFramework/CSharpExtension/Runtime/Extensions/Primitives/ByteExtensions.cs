// ==========================================================
// 文件名：ByteExtensions.cs
// 命名空间: AFramework.CSharpExtension
// 依赖: System
// ==========================================================

using System;
using System.Text;

namespace AFramework.CSharpExtension
{
    /// <summary>
    /// byte/sbyte 扩展方法集合
    /// </summary>
    public static class ByteExtensions
    {
        #region byte 扩展

        /// <summary>
        /// 转换为十六进制字符串
        /// </summary>
        public static string ToHexString(this byte value, bool uppercase = true)
        {
            return value.ToString(uppercase ? "X2" : "x2");
        }

        /// <summary>
        /// 转换为二进制字符串
        /// </summary>
        public static string ToBinaryString(this byte value)
        {
            return Convert.ToString(value, 2).PadLeft(8, '0');
        }

        /// <summary>
        /// 判断指定位是否为 1
        /// </summary>
        public static bool IsBitSet(this byte value, int bitIndex)
        {
            if (bitIndex < 0 || bitIndex > 7)
                throw new ArgumentOutOfRangeException(nameof(bitIndex), "位索引必须在 0-7 之间");

            return (value & (1 << bitIndex)) != 0;
        }

        /// <summary>
        /// 设置指定位为 1
        /// </summary>
        public static byte SetBit(this byte value, int bitIndex)
        {
            if (bitIndex < 0 || bitIndex > 7)
                throw new ArgumentOutOfRangeException(nameof(bitIndex), "位索引必须在 0-7 之间");

            return (byte)(value | (1 << bitIndex));
        }

        /// <summary>
        /// 清除指定位（设置为 0）
        /// </summary>
        public static byte ClearBit(this byte value, int bitIndex)
        {
            if (bitIndex < 0 || bitIndex > 7)
                throw new ArgumentOutOfRangeException(nameof(bitIndex), "位索引必须在 0-7 之间");

            return (byte)(value & ~(1 << bitIndex));
        }

        /// <summary>
        /// 切换指定位
        /// </summary>
        public static byte ToggleBit(this byte value, int bitIndex)
        {
            if (bitIndex < 0 || bitIndex > 7)
                throw new ArgumentOutOfRangeException(nameof(bitIndex), "位索引必须在 0-7 之间");

            return (byte)(value ^ (1 << bitIndex));
        }

        /// <summary>
        /// 判断是否在指定范围内
        /// </summary>
        public static bool IsInRange(this byte value, byte min, byte max)
        {
            return value >= min && value <= max;
        }

        /// <summary>
        /// 限制在指定范围内
        /// </summary>
        public static byte Clamp(this byte value, byte min, byte max)
        {
            if (min > max)
                throw new ArgumentException("最小值不能大于最大值");

            if (value < min) return min;
            if (value > max) return max;
            return value;
        }

        #endregion

        #region byte[] 扩展

        /// <summary>
        /// 字节数组转换为十六进制字符串
        /// </summary>
        public static string ToHexString(this byte[] bytes, bool uppercase = true, string separator = "")
        {
            if (bytes == null)
                throw new ArgumentNullException(nameof(bytes));

            if (bytes.Length == 0)
                return string.Empty;

            var format = uppercase ? "X2" : "x2";
            var sb = new StringBuilder(bytes.Length * (2 + separator.Length));

            for (int i = 0; i < bytes.Length; i++)
            {
                if (i > 0 && !string.IsNullOrEmpty(separator))
                    sb.Append(separator);

                sb.Append(bytes[i].ToString(format));
            }

            return sb.ToString();
        }

        /// <summary>
        /// 字节数组转换为 Base64 字符串
        /// </summary>
        public static string ToBase64String(this byte[] bytes)
        {
            if (bytes == null)
                throw new ArgumentNullException(nameof(bytes));

            return Convert.ToBase64String(bytes);
        }

        /// <summary>
        /// 判断字节数组是否为空或 null
        /// </summary>
        public static bool IsNullOrEmpty(this byte[] bytes)
        {
            return bytes == null || bytes.Length == 0;
        }

        /// <summary>
        /// 比较两个字节数组是否相等
        /// </summary>
        public static bool SequenceEqual(this byte[] bytes, byte[] other)
        {
            if (bytes == null || other == null)
                return bytes == other;

            if (bytes.Length != other.Length)
                return false;

            for (int i = 0; i < bytes.Length; i++)
            {
                if (bytes[i] != other[i])
                    return false;
            }

            return true;
        }

        #endregion

        #region sbyte 扩展

        /// <summary>
        /// 判断是否在指定范围内
        /// </summary>
        public static bool IsInRange(this sbyte value, sbyte min, sbyte max)
        {
            return value >= min && value <= max;
        }

        /// <summary>
        /// 限制在指定范围内
        /// </summary>
        public static sbyte Clamp(this sbyte value, sbyte min, sbyte max)
        {
            if (min > max)
                throw new ArgumentException("最小值不能大于最大值");

            if (value < min) return min;
            if (value > max) return max;
            return value;
        }

        /// <summary>
        /// 转换为绝对值
        /// </summary>
        public static sbyte Abs(this sbyte value)
        {
            return (sbyte)Math.Abs(value);
        }

        #endregion
    }
}
