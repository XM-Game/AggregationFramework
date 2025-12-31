// ==========================================================
// 文件名：MathUtility.Rounding.cs
// 命名空间: AFramework.CSharpExtension
// 依赖: System, System.Runtime.CompilerServices
// ==========================================================

using System;
using System.Runtime.CompilerServices;

namespace AFramework.CSharpExtension
{
    public static partial class MathUtility
    {
        #region 基础舍入

        /// <summary>
        /// 向下取整
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int FloorToInt(float value) => (int)Math.Floor(value);

        /// <summary>
        /// 向上取整
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CeilToInt(float value) => (int)Math.Ceiling(value);

        /// <summary>
        /// 四舍五入取整
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int RoundToInt(float value) => (int)Math.Round(value);

        /// <summary>
        /// 向零取整（截断）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int TruncateToInt(float value) => (int)value;

        #endregion

        #region 指定精度舍入

        /// <summary>
        /// 舍入到指定小数位数
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Round(float value, int decimals)
        {
            return (float)Math.Round(value, decimals);
        }

        /// <summary>
        /// 舍入到指定小数位数（指定舍入模式）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Round(float value, int decimals, MidpointRounding mode)
        {
            return (float)Math.Round(value, decimals, mode);
        }

        /// <summary>
        /// 向下舍入到指定小数位数
        /// </summary>
        public static float FloorToDecimals(float value, int decimals)
        {
            float multiplier = (float)Math.Pow(10, decimals);
            return (float)Math.Floor(value * multiplier) / multiplier;
        }

        /// <summary>
        /// 向上舍入到指定小数位数
        /// </summary>
        public static float CeilToDecimals(float value, int decimals)
        {
            float multiplier = (float)Math.Pow(10, decimals);
            return (float)Math.Ceiling(value * multiplier) / multiplier;
        }

        #endregion

        #region 步进舍入

        /// <summary>
        /// 舍入到最近的步进值
        /// </summary>
        public static float RoundToStep(float value, float step)
        {
            if (step <= 0f) return value;
            return (float)Math.Round(value / step) * step;
        }

        /// <summary>
        /// 向下舍入到步进值
        /// </summary>
        public static float FloorToStep(float value, float step)
        {
            if (step <= 0f) return value;
            return (float)Math.Floor(value / step) * step;
        }

        /// <summary>
        /// 向上舍入到步进值
        /// </summary>
        public static float CeilToStep(float value, float step)
        {
            if (step <= 0f) return value;
            return (float)Math.Ceiling(value / step) * step;
        }

        /// <summary>
        /// 舍入到最近的整数倍
        /// </summary>
        public static int RoundToMultiple(int value, int multiple)
        {
            if (multiple <= 0) return value;
            int remainder = value % multiple;
            if (remainder == 0) return value;
            if (remainder < multiple / 2)
                return value - remainder;
            return value + (multiple - remainder);
        }

        /// <summary>
        /// 向下舍入到整数倍
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int FloorToMultiple(int value, int multiple)
        {
            if (multiple <= 0) return value;
            return (value / multiple) * multiple;
        }

        /// <summary>
        /// 向上舍入到整数倍
        /// </summary>
        public static int CeilToMultiple(int value, int multiple)
        {
            if (multiple <= 0) return value;
            int remainder = value % multiple;
            return remainder == 0 ? value : value + (multiple - remainder);
        }

        #endregion

        #region 有效数字舍入

        /// <summary>
        /// 舍入到指定有效数字位数
        /// </summary>
        public static float RoundToSignificantDigits(float value, int digits)
        {
            if (value == 0f || digits <= 0) return 0f;
            
            double scale = Math.Pow(10, Math.Floor(Math.Log10(Math.Abs(value))) + 1);
            return (float)(scale * Math.Round(value / scale, digits));
        }

        #endregion

        #region 量化

        /// <summary>
        /// 将值量化到指定级别数
        /// </summary>
        /// <param name="value">输入值 [0, 1]</param>
        /// <param name="levels">级别数</param>
        public static float Quantize(float value, int levels)
        {
            if (levels <= 1) return 0f;
            return (float)Math.Round(value * (levels - 1)) / (levels - 1);
        }

        /// <summary>
        /// 将值量化到指定范围内的级别
        /// </summary>
        public static float Quantize(float value, float min, float max, int levels)
        {
            if (levels <= 1) return min;
            float normalized = (value - min) / (max - min);
            float quantized = Quantize(Clamp01(normalized), levels);
            return min + quantized * (max - min);
        }

        #endregion

        #region 对齐

        /// <summary>
        /// 将值对齐到指定边界（向上）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int AlignUp(int value, int alignment)
        {
            if (alignment <= 0) return value;
            return ((value + alignment - 1) / alignment) * alignment;
        }

        /// <summary>
        /// 将值对齐到指定边界（向下）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int AlignDown(int value, int alignment)
        {
            if (alignment <= 0) return value;
            return (value / alignment) * alignment;
        }

        /// <summary>
        /// 检查值是否已对齐
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsAligned(int value, int alignment)
        {
            return alignment > 0 && value % alignment == 0;
        }

        #endregion

        #region 分数舍入

        /// <summary>
        /// 获取小数部分
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Frac(float value)
        {
            return value - (float)Math.Floor(value);
        }

        /// <summary>
        /// 获取整数部分
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Trunc(float value)
        {
            return (float)Math.Truncate(value);
        }

        /// <summary>
        /// 分离整数和小数部分
        /// </summary>
        public static void SplitIntegerFraction(float value, out int integer, out float fraction)
        {
            integer = (int)Math.Floor(value);
            fraction = value - integer;
        }

        #endregion
    }
}
