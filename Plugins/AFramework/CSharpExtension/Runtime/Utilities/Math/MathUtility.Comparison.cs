// ==========================================================
// 文件名：MathUtility.Comparison.cs
// 命名空间: AFramework.CSharpExtension
// 依赖: System, System.Runtime.CompilerServices
// ==========================================================

using System;
using System.Runtime.CompilerServices;

namespace AFramework.CSharpExtension
{
    public static partial class MathUtility
    {
        #region 浮点数近似比较

        /// <summary>
        /// 检查两个浮点数是否近似相等
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Approximately(float a, float b)
        {
            return Abs(a - b) < DefaultEpsilon;
        }

        /// <summary>
        /// 检查两个浮点数是否近似相等（自定义容差）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Approximately(float a, float b, float epsilon)
        {
            return Abs(a - b) < epsilon;
        }

        /// <summary>
        /// 检查两个双精度浮点数是否近似相等
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Approximately(double a, double b)
        {
            return Math.Abs(a - b) < DefaultEpsilonDouble;
        }

        /// <summary>
        /// 检查两个双精度浮点数是否近似相等（自定义容差）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Approximately(double a, double b, double epsilon)
        {
            return Math.Abs(a - b) < epsilon;
        }

        #endregion

        #region 零值检查

        /// <summary>
        /// 检查浮点数是否近似为零
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsZero(float value)
        {
            return Abs(value) < DefaultEpsilon;
        }

        /// <summary>
        /// 检查浮点数是否近似为零（自定义容差）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsZero(float value, float epsilon)
        {
            return Abs(value) < epsilon;
        }

        /// <summary>
        /// 检查双精度浮点数是否近似为零
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsZero(double value)
        {
            return Math.Abs(value) < DefaultEpsilonDouble;
        }

        #endregion

        #region 特殊值检查

        /// <summary>
        /// 检查浮点数是否为有效数值（非 NaN 且非无穷大）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsFinite(float value)
        {
            return !float.IsNaN(value) && !float.IsInfinity(value);
        }

        /// <summary>
        /// 检查双精度浮点数是否为有效数值
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsFinite(double value)
        {
            return !double.IsNaN(value) && !double.IsInfinity(value);
        }

        /// <summary>
        /// 如果值为 NaN 或无穷大，返回默认值
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float FiniteOrDefault(float value, float defaultValue = 0f)
        {
            return IsFinite(value) ? value : defaultValue;
        }

        #endregion

        #region 符号比较

        /// <summary>
        /// 检查两个值是否同号
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SameSign(float a, float b)
        {
            return (a >= 0f && b >= 0f) || (a < 0f && b < 0f);
        }

        /// <summary>
        /// 检查两个值是否异号
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool OppositeSign(float a, float b)
        {
            return (a > 0f && b < 0f) || (a < 0f && b > 0f);
        }

        /// <summary>
        /// 检查值是否为正数
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsPositive(float value) => value > 0f;

        /// <summary>
        /// 检查值是否为负数
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNegative(float value) => value < 0f;

        #endregion

        #region 大小比较

        /// <summary>
        /// 检查 a 是否大于 b（考虑浮点误差）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool GreaterThan(float a, float b)
        {
            return a - b > DefaultEpsilon;
        }

        /// <summary>
        /// 检查 a 是否小于 b（考虑浮点误差）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool LessThan(float a, float b)
        {
            return b - a > DefaultEpsilon;
        }

        /// <summary>
        /// 检查 a 是否大于等于 b（考虑浮点误差）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool GreaterThanOrEqual(float a, float b)
        {
            return a - b > -DefaultEpsilon;
        }

        /// <summary>
        /// 检查 a 是否小于等于 b（考虑浮点误差）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool LessThanOrEqual(float a, float b)
        {
            return b - a > -DefaultEpsilon;
        }

        #endregion

        #region 整数比较

        /// <summary>
        /// 检查整数是否为偶数
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsEven(int value) => (value & 1) == 0;

        /// <summary>
        /// 检查整数是否为奇数
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsOdd(int value) => (value & 1) == 1;

        /// <summary>
        /// 检查整数是否能被另一个整数整除
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsDivisibleBy(int value, int divisor)
        {
            return divisor != 0 && value % divisor == 0;
        }

        #endregion

        #region 差值计算

        /// <summary>
        /// 计算两个值的差的绝对值
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Difference(int a, int b) => Abs(a - b);

        /// <summary>
        /// 计算两个值的差的绝对值
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Difference(float a, float b) => Abs(a - b);

        /// <summary>
        /// 计算相对误差
        /// </summary>
        public static float RelativeError(float actual, float expected)
        {
            if (IsZero(expected)) return IsZero(actual) ? 0f : float.MaxValue;
            return Abs((actual - expected) / expected);
        }

        #endregion
    }
}
