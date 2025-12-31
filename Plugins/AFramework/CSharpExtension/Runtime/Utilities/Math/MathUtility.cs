// ==========================================================
// 文件名：MathUtility.cs
// 命名空间: AFramework.CSharpExtension
// 依赖: System, System.Runtime.CompilerServices
// ==========================================================

using System;
using System.Runtime.CompilerServices;

namespace AFramework.CSharpExtension
{
    /// <summary>
    /// 数学工具类
    /// <para>提供常用数学计算功能，包括插值、范围限制、数值比较、舍入等</para>
    /// </summary>
    public static partial class MathUtility
    {
        #region 常量

        /// <summary>
        /// 浮点数比较的默认容差
        /// </summary>
        public const float DefaultEpsilon = 1e-6f;

        /// <summary>
        /// 双精度浮点数比较的默认容差
        /// </summary>
        public const double DefaultEpsilonDouble = 1e-10;

        /// <summary>
        /// 圆周率
        /// </summary>
        public const float PI = 3.14159265358979f;

        /// <summary>
        /// 2π
        /// </summary>
        public const float TwoPI = PI * 2f;

        /// <summary>
        /// π/2
        /// </summary>
        public const float HalfPI = PI * 0.5f;

        /// <summary>
        /// 角度转弧度系数
        /// </summary>
        public const float Deg2Rad = PI / 180f;

        /// <summary>
        /// 弧度转角度系数
        /// </summary>
        public const float Rad2Deg = 180f / PI;

        #endregion

        #region 基础数学运算

        /// <summary>
        /// 返回两个值中的较小值
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Min(int a, int b) => a < b ? a : b;

        /// <summary>
        /// 返回两个值中的较小值
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Min(float a, float b) => a < b ? a : b;

        /// <summary>
        /// 返回两个值中的较大值
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Max(int a, int b) => a > b ? a : b;

        /// <summary>
        /// 返回两个值中的较大值
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Max(float a, float b) => a > b ? a : b;

        /// <summary>
        /// 返回绝对值
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Abs(int value) => value >= 0 ? value : -value;

        /// <summary>
        /// 返回绝对值
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Abs(float value) => value >= 0f ? value : -value;

        /// <summary>
        /// 返回值的符号（-1, 0, 1）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Sign(int value) => value > 0 ? 1 : (value < 0 ? -1 : 0);

        /// <summary>
        /// 返回值的符号（-1, 0, 1）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Sign(float value) => value > 0f ? 1 : (value < 0f ? -1 : 0);

        #endregion

        #region 角度与弧度转换

        /// <summary>
        /// 角度转弧度
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float ToRadians(float degrees) => degrees * Deg2Rad;

        /// <summary>
        /// 弧度转角度
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float ToDegrees(float radians) => radians * Rad2Deg;

        /// <summary>
        /// 将角度规范化到 [0, 360) 范围
        /// </summary>
        public static float NormalizeAngle(float angle)
        {
            angle %= 360f;
            if (angle < 0f) angle += 360f;
            return angle;
        }

        /// <summary>
        /// 将角度规范化到 [-180, 180) 范围
        /// </summary>
        public static float NormalizeAngleSigned(float angle)
        {
            angle = NormalizeAngle(angle);
            if (angle >= 180f) angle -= 360f;
            return angle;
        }

        #endregion

        #region 百分比与比例

        /// <summary>
        /// 计算百分比 (value / total * 100)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Percentage(float value, float total)
        {
            return total == 0f ? 0f : (value / total) * 100f;
        }

        /// <summary>
        /// 计算比例 (value / total)，范围 [0, 1]
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Ratio(float value, float total)
        {
            return total == 0f ? 0f : Clamp01(value / total);
        }

        /// <summary>
        /// 将值从一个范围映射到另一个范围
        /// </summary>
        public static float Map(float value, float fromMin, float fromMax, float toMin, float toMax)
        {
            if (Approximately(fromMin, fromMax)) return toMin;
            float t = (value - fromMin) / (fromMax - fromMin);
            return toMin + (toMax - toMin) * t;
        }

        /// <summary>
        /// 将值从一个范围映射到另一个范围（带限制）
        /// </summary>
        public static float MapClamped(float value, float fromMin, float fromMax, float toMin, float toMax)
        {
            if (Approximately(fromMin, fromMax)) return toMin;
            float t = Clamp01((value - fromMin) / (fromMax - fromMin));
            return toMin + (toMax - toMin) * t;
        }

        #endregion

        #region 幂运算与对数

        /// <summary>
        /// 计算平方
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Square(float value) => value * value;

        /// <summary>
        /// 计算立方
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Cube(float value) => value * value * value;

        /// <summary>
        /// 检查是否为2的幂
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsPowerOfTwo(int value)
        {
            return value > 0 && (value & (value - 1)) == 0;
        }

        /// <summary>
        /// 获取大于等于给定值的最小2的幂
        /// </summary>
        public static int NextPowerOfTwo(int value)
        {
            if (value <= 0) return 1;
            value--;
            value |= value >> 1;
            value |= value >> 2;
            value |= value >> 4;
            value |= value >> 8;
            value |= value >> 16;
            return value + 1;
        }

        /// <summary>
        /// 获取小于等于给定值的最大2的幂
        /// </summary>
        public static int PreviousPowerOfTwo(int value)
        {
            if (value <= 0) return 0;
            int next = NextPowerOfTwo(value);
            return next == value ? value : next >> 1;
        }

        #endregion

        #region 循环与包装

        /// <summary>
        /// 循环值到指定范围 [0, length)
        /// </summary>
        public static int Repeat(int value, int length)
        {
            if (length <= 0) return 0;
            int result = value % length;
            return result < 0 ? result + length : result;
        }

        /// <summary>
        /// 循环值到指定范围 [0, length)
        /// </summary>
        public static float Repeat(float value, float length)
        {
            if (length <= 0f) return 0f;
            return value - (float)Math.Floor(value / length) * length;
        }

        /// <summary>
        /// 乒乓循环值（来回反弹）
        /// </summary>
        public static float PingPong(float value, float length)
        {
            if (length <= 0f) return 0f;
            float t = Repeat(value, length * 2f);
            return length - Abs(t - length);
        }

        #endregion

        #region 距离计算

        /// <summary>
        /// 计算两点间的曼哈顿距离
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ManhattanDistance(int x1, int y1, int x2, int y2)
        {
            return Abs(x2 - x1) + Abs(y2 - y1);
        }

        /// <summary>
        /// 计算两点间的切比雪夫距离
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ChebyshevDistance(int x1, int y1, int x2, int y2)
        {
            return Max(Abs(x2 - x1), Abs(y2 - y1));
        }

        /// <summary>
        /// 计算两点间的欧几里得距离的平方（避免开方运算）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float DistanceSquared(float x1, float y1, float x2, float y2)
        {
            float dx = x2 - x1;
            float dy = y2 - y1;
            return dx * dx + dy * dy;
        }

        #endregion
    }
}
