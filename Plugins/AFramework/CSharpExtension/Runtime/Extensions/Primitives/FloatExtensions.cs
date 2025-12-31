// ==========================================================
// 文件名：FloatExtensions.cs
// 命名空间: AFramework.CSharpExtension
// 依赖: System
// ==========================================================

using System;
using System.Runtime.CompilerServices;

namespace AFramework.CSharpExtension
{
    /// <summary>
    /// float 扩展方法
    /// <para>提供浮点数的常用操作扩展，包括范围检查、数学运算、近似比较等功能</para>
    /// </summary>
    public static class FloatExtensions
    {
        #region 范围检查

        /// <summary>
        /// 检查值是否在指定范围内（包含边界）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool InRange(this float value, float min, float max)
        {
            return value >= min && value <= max;
        }

        /// <summary>
        /// 检查值是否在指定范围内（不包含边界）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool InRangeExclusive(this float value, float min, float max)
        {
            return value > min && value < max;
        }

        /// <summary>
        /// 将值限制在指定范围内
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Clamp(this float value, float min, float max)
        {
            return Math.Max(min, Math.Min(max, value));
        }

        /// <summary>
        /// 将值限制在 0-1 范围内
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Clamp01(this float value)
        {
            return value < 0f ? 0f : (value > 1f ? 1f : value);
        }

        /// <summary>
        /// 检查是否为正数
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsPositive(this float value) => value > 0f;

        /// <summary>
        /// 检查是否为负数
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNegative(this float value) => value < 0f;

        /// <summary>
        /// 检查是否近似为零
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsApproximatelyZero(this float value, float epsilon = MathConstants.Epsilon)
        {
            return Math.Abs(value) < epsilon;
        }

        /// <summary>
        /// 检查是否为有效数字（非 NaN 且非无穷大）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsValid(this float value)
        {
            return !float.IsNaN(value) && !float.IsInfinity(value);
        }

        /// <summary>
        /// 检查是否为 NaN
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNaN(this float value) => float.IsNaN(value);

        /// <summary>
        /// 检查是否为无穷大
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsInfinity(this float value) => float.IsInfinity(value);

        #endregion

        #region 近似比较

        /// <summary>
        /// 检查两个浮点数是否近似相等
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Approximately(this float value, float other, float epsilon = MathConstants.Epsilon)
        {
            return Math.Abs(value - other) < epsilon;
        }

        /// <summary>
        /// 检查是否近似等于 1
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ApproximatelyOne(this float value, float epsilon = MathConstants.Epsilon)
        {
            return Math.Abs(value - 1f) < epsilon;
        }

        /// <summary>
        /// 检查是否大于另一个值（考虑精度）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool GreaterThan(this float value, float other, float epsilon = MathConstants.Epsilon)
        {
            return value - other > epsilon;
        }

        /// <summary>
        /// 检查是否小于另一个值（考虑精度）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool LessThan(this float value, float other, float epsilon = MathConstants.Epsilon)
        {
            return other - value > epsilon;
        }

        #endregion

        #region 数学运算

        /// <summary>
        /// 获取绝对值
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Abs(this float value) => Math.Abs(value);

        /// <summary>
        /// 获取符号（-1, 0, 1）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Sign(this float value) => Math.Sign(value);

        /// <summary>
        /// 平方
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Square(this float value) => value * value;

        /// <summary>
        /// 立方
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Cube(this float value) => value * value * value;

        /// <summary>
        /// 平方根
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Sqrt(this float value) => (float)Math.Sqrt(value);

        /// <summary>
        /// 幂运算
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Pow(this float value, float exponent)
        {
            return (float)Math.Pow(value, exponent);
        }

        /// <summary>
        /// 向上取整
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Ceil(this float value) => (float)Math.Ceiling(value);

        /// <summary>
        /// 向下取整
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Floor(this float value) => (float)Math.Floor(value);

        /// <summary>
        /// 四舍五入
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Round(this float value) => (float)Math.Round(value);

        /// <summary>
        /// 四舍五入到指定小数位数
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Round(this float value, int digits)
        {
            return (float)Math.Round(value, digits);
        }

        /// <summary>
        /// 向上取整为整数
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CeilToInt(this float value) => (int)Math.Ceiling(value);

        /// <summary>
        /// 向下取整为整数
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int FloorToInt(this float value) => (int)Math.Floor(value);

        /// <summary>
        /// 四舍五入为整数
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int RoundToInt(this float value) => (int)Math.Round(value);

        /// <summary>
        /// 最大值
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Max(this float value, float other) => Math.Max(value, other);

        /// <summary>
        /// 最小值
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Min(this float value, float other) => Math.Min(value, other);

        #endregion

        #region 插值和映射

        /// <summary>
        /// 线性插值
        /// </summary>
        /// <param name="from">起始值</param>
        /// <param name="to">目标值</param>
        /// <param name="t">插值参数（0-1）</param>
        /// <returns>插值结果</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Lerp(this float from, float to, float t)
        {
            return from + (to - from) * t.Clamp01();
        }

        /// <summary>
        /// 线性插值（不限制 t 的范围）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float LerpUnclamped(this float from, float to, float t)
        {
            return from + (to - from) * t;
        }

        /// <summary>
        /// 反向插值（获取 t 值）
        /// </summary>
        /// <param name="value">当前值</param>
        /// <param name="from">起始值</param>
        /// <param name="to">目标值</param>
        /// <returns>插值参数 t</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float InverseLerp(this float value, float from, float to)
        {
            if (Math.Abs(to - from) < MathConstants.Epsilon)
                return 0f;
            return ((value - from) / (to - from)).Clamp01();
        }

        /// <summary>
        /// 将值从一个范围映射到另一个范围
        /// </summary>
        /// <param name="value">当前值</param>
        /// <param name="fromMin">源范围最小值</param>
        /// <param name="fromMax">源范围最大值</param>
        /// <param name="toMin">目标范围最小值</param>
        /// <param name="toMax">目标范围最大值</param>
        /// <returns>映射后的值</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Remap(this float value, float fromMin, float fromMax, float toMin, float toMax)
        {
            float t = value.InverseLerp(fromMin, fromMax);
            return toMin.Lerp(toMax, t);
        }

        #endregion

        #region 角度相关

        /// <summary>
        /// 度转弧度
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float ToRadians(this float degrees)
        {
            return degrees * MathConstants.Deg2Rad_F;
        }

        /// <summary>
        /// 弧度转度
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float ToDegrees(this float radians)
        {
            return radians * MathConstants.Rad2Deg_F;
        }

        /// <summary>
        /// 将角度规范化到 0-360 范围
        /// </summary>
        public static float NormalizeAngle(this float angle)
        {
            angle %= 360f;
            if (angle < 0f)
                angle += 360f;
            return angle;
        }

        /// <summary>
        /// 将角度规范化到 -180 到 180 范围
        /// </summary>
        public static float NormalizeAngle180(this float angle)
        {
            angle = angle.NormalizeAngle();
            if (angle > 180f)
                angle -= 360f;
            return angle;
        }

        #endregion

        #region 百分比操作

        /// <summary>
        /// 转换为百分比字符串
        /// </summary>
        /// <param name="value">值（0-1）</param>
        /// <param name="decimals">小数位数</param>
        /// <returns>百分比字符串</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ToPercentString(this float value, int decimals = 0)
        {
            return (value * 100f).ToString($"F{decimals}") + "%";
        }

        /// <summary>
        /// 计算百分比
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="total">总数</param>
        /// <returns>百分比（0-1）</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float PercentOf(this float value, float total)
        {
            return Math.Abs(total) < MathConstants.Epsilon ? 0f : value / total;
        }

        /// <summary>
        /// 计算指定百分比的值
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="percent">百分比（0-1）</param>
        /// <returns>结果值</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Percent(this float value, float percent)
        {
            return value * percent;
        }

        #endregion

        #region 缓动函数

        /// <summary>
        /// 平滑阻尼（类似 Unity 的 SmoothDamp）
        /// </summary>
        public static float SmoothDamp(this float current, float target, ref float velocity, float smoothTime, float deltaTime, float maxSpeed = float.PositiveInfinity)
        {
            smoothTime = Math.Max(0.0001f, smoothTime);
            float omega = 2f / smoothTime;
            float x = omega * deltaTime;
            float exp = 1f / (1f + x + 0.48f * x * x + 0.235f * x * x * x);
            float change = current - target;
            float originalTo = target;

            float maxChange = maxSpeed * smoothTime;
            change = change.Clamp(-maxChange, maxChange);
            target = current - change;

            float temp = (velocity + omega * change) * deltaTime;
            velocity = (velocity - omega * temp) * exp;
            float output = target + (change + temp) * exp;

            if ((originalTo - current > 0f) == (output > originalTo))
            {
                output = originalTo;
                velocity = (output - originalTo) / deltaTime;
            }

            return output;
        }

        /// <summary>
        /// 平滑步进（Smoothstep）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float SmoothStep(this float t)
        {
            t = t.Clamp01();
            return t * t * (3f - 2f * t);
        }

        /// <summary>
        /// 更平滑的步进（Smootherstep）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float SmootherStep(this float t)
        {
            t = t.Clamp01();
            return t * t * t * (t * (t * 6f - 15f) + 10f);
        }

        #endregion

        #region 转换操作

        /// <summary>
        /// 转换为字节数组
        /// </summary>
        public static byte[] ToBytes(this float value)
        {
            return BitConverter.GetBytes(value);
        }

        /// <summary>
        /// 转换为格式化字符串
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ToFormattedString(this float value, int decimals = 2)
        {
            return value.ToString($"F{decimals}");
        }

        #endregion

        #region 特殊值处理

        /// <summary>
        /// 如果为 NaN 或无穷大，返回默认值
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float GetValidOrDefault(this float value, float defaultValue = 0f)
        {
            return value.IsValid() ? value : defaultValue;
        }

        #endregion
    }
}
