// ==========================================================
// 文件名：MathConstants.cs
// 命名空间: AFramework.CSharpExtension
// 依赖: System
// ==========================================================

using System;
using System.Runtime.CompilerServices;

namespace AFramework.CSharpExtension
{
    /// <summary>
    /// 数学常量定义
    /// <para>提供常用数学常量和精度相关常量</para>
    /// <para>包含圆周率、自然对数底、黄金比例等数学常量</para>
    /// </summary>
    /// <remarks>
    /// 使用示例:
    /// <code>
    /// // 使用圆周率
    /// double circumference = 2 * MathConstants.PI * radius;
    /// 
    /// // 角度转弧度
    /// double radians = degrees * MathConstants.Deg2Rad;
    /// 
    /// // 浮点数近似比较
    /// bool isEqual = Math.Abs(a - b) &lt; MathConstants.Epsilon;
    /// </code>
    /// </remarks>
    public static class MathConstants
    {
        #region 基础数学常量

        /// <summary>圆周率 π (精确到 double 精度)</summary>
        public const double PI = 3.14159265358979323846;

        /// <summary>圆周率 π (float 精度)</summary>
        public const float PI_F = 3.14159265f;

        /// <summary>2π (一个完整圆的弧度)</summary>
        public const double TwoPI = 6.28318530717958647692;

        /// <summary>2π (float 精度)</summary>
        public const float TwoPI_F = 6.28318530f;

        /// <summary>π/2 (90度弧度)</summary>
        public const double HalfPI = 1.57079632679489661923;

        /// <summary>π/2 (float 精度)</summary>
        public const float HalfPI_F = 1.57079632f;

        /// <summary>π/4 (45度弧度)</summary>
        public const double QuarterPI = 0.78539816339744830962;

        /// <summary>π/4 (float 精度)</summary>
        public const float QuarterPI_F = 0.78539816f;

        /// <summary>自然对数的底 e</summary>
        public const double E = 2.71828182845904523536;

        /// <summary>自然对数的底 e (float 精度)</summary>
        public const float E_F = 2.71828182f;

        /// <summary>黄金比例 φ (Golden Ratio)</summary>
        public const double GoldenRatio = 1.61803398874989484820;

        /// <summary>黄金比例 φ (float 精度)</summary>
        public const float GoldenRatio_F = 1.61803398f;

        /// <summary>2 的平方根 √2</summary>
        public const double Sqrt2 = 1.41421356237309504880;

        /// <summary>2 的平方根 √2 (float 精度)</summary>
        public const float Sqrt2_F = 1.41421356f;

        /// <summary>3 的平方根 √3</summary>
        public const double Sqrt3 = 1.73205080756887729352;

        /// <summary>3 的平方根 √3 (float 精度)</summary>
        public const float Sqrt3_F = 1.73205080f;

        /// <summary>1/√2 (√2 的倒数)</summary>
        public const double InvSqrt2 = 0.70710678118654752440;

        /// <summary>1/√2 (float 精度)</summary>
        public const float InvSqrt2_F = 0.70710678f;

        #endregion

        #region 角度转换常量

        /// <summary>度转弧度的乘数 (π/180)</summary>
        public const double Deg2Rad = 0.01745329251994329576;

        /// <summary>度转弧度的乘数 (float 精度)</summary>
        public const float Deg2Rad_F = 0.01745329f;

        /// <summary>弧度转度的乘数 (180/π)</summary>
        public const double Rad2Deg = 57.29577951308232087679;

        /// <summary>弧度转度的乘数 (float 精度)</summary>
        public const float Rad2Deg_F = 57.29577951f;

        #endregion

        #region 精度常量

        /// <summary>float 类型的机器精度 (约 1.19e-7)</summary>
        public const float Epsilon_F = 1.175494351e-38f;

        /// <summary>double 类型的机器精度 (约 2.22e-16)</summary>
        public const double Epsilon_D = 2.2204460492503131e-16;

        /// <summary>通用浮点比较精度 (float)</summary>
        public const float Epsilon = 1e-6f;

        /// <summary>通用浮点比较精度 (double)</summary>
        public const double EpsilonDouble = 1e-15;

        /// <summary>宽松浮点比较精度 (用于视觉效果等)</summary>
        public const float EpsilonLoose = 1e-4f;

        /// <summary>严格浮点比较精度 (用于精确计算)</summary>
        public const float EpsilonStrict = 1e-8f;

        /// <summary>float 最大值</summary>
        public const float MaxFloat = float.MaxValue;

        /// <summary>float 最小正值</summary>
        public const float MinPositiveFloat = float.Epsilon;

        /// <summary>double 最大值</summary>
        public const double MaxDouble = double.MaxValue;

        /// <summary>double 最小正值</summary>
        public const double MinPositiveDouble = double.Epsilon;

        #endregion

        #region 常用数值常量

        /// <summary>一百</summary>
        public const int Hundred = 100;

        /// <summary>一千</summary>
        public const int Thousand = 1000;

        /// <summary>一万</summary>
        public const int TenThousand = 10000;

        /// <summary>十万</summary>
        public const int HundredThousand = 100000;

        /// <summary>一百万</summary>
        public const int Million = 1000000;

        /// <summary>十亿</summary>
        public const int Billion = 1000000000;

        /// <summary>字节单位: KB</summary>
        public const int KB = 1024;

        /// <summary>字节单位: MB</summary>
        public const int MB = 1024 * 1024;

        /// <summary>字节单位: GB</summary>
        public const long GB = 1024L * 1024L * 1024L;

        #endregion

        #region 百分比常量

        /// <summary>
        /// 百分比常量集合
        /// </summary>
        public static class Percent
        {
            /// <summary>0%</summary>
            public const float Zero = 0f;

            /// <summary>10%</summary>
            public const float Ten = 0.1f;

            /// <summary>25%</summary>
            public const float Quarter = 0.25f;

            /// <summary>33.33%</summary>
            public const float Third = 0.333333f;

            /// <summary>50%</summary>
            public const float Half = 0.5f;

            /// <summary>66.67%</summary>
            public const float TwoThirds = 0.666667f;

            /// <summary>75%</summary>
            public const float ThreeQuarters = 0.75f;

            /// <summary>90%</summary>
            public const float Ninety = 0.9f;

            /// <summary>100%</summary>
            public const float Full = 1f;
        }

        #endregion

        #region 工具方法

        /// <summary>
        /// 将角度转换为弧度
        /// </summary>
        /// <param name="degrees">角度值</param>
        /// <returns>弧度值</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float ToRadians(float degrees) => degrees * Deg2Rad_F;

        /// <summary>
        /// 将角度转换为弧度 (double 精度)
        /// </summary>
        /// <param name="degrees">角度值</param>
        /// <returns>弧度值</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double ToRadians(double degrees) => degrees * Deg2Rad;

        /// <summary>
        /// 将弧度转换为角度
        /// </summary>
        /// <param name="radians">弧度值</param>
        /// <returns>角度值</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float ToDegrees(float radians) => radians * Rad2Deg_F;

        /// <summary>
        /// 将弧度转换为角度 (double 精度)
        /// </summary>
        /// <param name="radians">弧度值</param>
        /// <returns>角度值</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double ToDegrees(double radians) => radians * Rad2Deg;

        /// <summary>
        /// 检查两个浮点数是否近似相等
        /// </summary>
        /// <param name="a">第一个值</param>
        /// <param name="b">第二个值</param>
        /// <param name="epsilon">精度阈值</param>
        /// <returns>如果近似相等返回 true</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Approximately(float a, float b, float epsilon = Epsilon)
        {
            return Math.Abs(a - b) < epsilon;
        }

        /// <summary>
        /// 检查两个双精度浮点数是否近似相等
        /// </summary>
        /// <param name="a">第一个值</param>
        /// <param name="b">第二个值</param>
        /// <param name="epsilon">精度阈值</param>
        /// <returns>如果近似相等返回 true</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Approximately(double a, double b, double epsilon = EpsilonDouble)
        {
            return Math.Abs(a - b) < epsilon;
        }

        /// <summary>
        /// 检查浮点数是否近似为零
        /// </summary>
        /// <param name="value">要检查的值</param>
        /// <param name="epsilon">精度阈值</param>
        /// <returns>如果近似为零返回 true</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsApproximatelyZero(float value, float epsilon = Epsilon)
        {
            return Math.Abs(value) < epsilon;
        }

        /// <summary>
        /// 检查双精度浮点数是否近似为零
        /// </summary>
        /// <param name="value">要检查的值</param>
        /// <param name="epsilon">精度阈值</param>
        /// <returns>如果近似为零返回 true</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsApproximatelyZero(double value, double epsilon = EpsilonDouble)
        {
            return Math.Abs(value) < epsilon;
        }

        #endregion
    }
}
