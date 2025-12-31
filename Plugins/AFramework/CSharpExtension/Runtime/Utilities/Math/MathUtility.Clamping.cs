// ==========================================================
// 文件名：MathUtility.Clamping.cs
// 命名空间: AFramework.CSharpExtension
// 依赖: System, System.Runtime.CompilerServices
// ==========================================================

using System;
using System.Runtime.CompilerServices;

namespace AFramework.CSharpExtension
{
    public static partial class MathUtility
    {
        #region 基础范围限制

        /// <summary>
        /// 将值限制在指定范围内
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Clamp(int value, int min, int max)
        {
            if (value < min) return min;
            if (value > max) return max;
            return value;
        }

        /// <summary>
        /// 将值限制在指定范围内
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Clamp(float value, float min, float max)
        {
            if (value < min) return min;
            if (value > max) return max;
            return value;
        }

        /// <summary>
        /// 将值限制在指定范围内
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Clamp(double value, double min, double max)
        {
            if (value < min) return min;
            if (value > max) return max;
            return value;
        }

        /// <summary>
        /// 将值限制在 [0, 1] 范围内
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Clamp01(float value)
        {
            if (value < 0f) return 0f;
            if (value > 1f) return 1f;
            return value;
        }

        /// <summary>
        /// 将值限制在 [0, 1] 范围内
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Clamp01(double value)
        {
            if (value < 0.0) return 0.0;
            if (value > 1.0) return 1.0;
            return value;
        }

        #endregion

        #region 特殊范围限制

        /// <summary>
        /// 将值限制为非负数
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ClampPositive(int value) => value < 0 ? 0 : value;

        /// <summary>
        /// 将值限制为非负数
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float ClampPositive(float value) => value < 0f ? 0f : value;

        /// <summary>
        /// 将值限制为非正数
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ClampNegative(int value) => value > 0 ? 0 : value;

        /// <summary>
        /// 将值限制为非正数
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float ClampNegative(float value) => value > 0f ? 0f : value;

        /// <summary>
        /// 将值限制在最小值以上
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ClampMin(int value, int min) => value < min ? min : value;

        /// <summary>
        /// 将值限制在最小值以上
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float ClampMin(float value, float min) => value < min ? min : value;

        /// <summary>
        /// 将值限制在最大值以下
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ClampMax(int value, int max) => value > max ? max : value;

        /// <summary>
        /// 将值限制在最大值以下
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float ClampMax(float value, float max) => value > max ? max : value;

        #endregion

        #region 角度范围限制

        /// <summary>
        /// 将角度限制在 [0, 360) 范围内
        /// </summary>
        public static float ClampAngle360(float angle)
        {
            angle %= 360f;
            return angle < 0f ? angle + 360f : angle;
        }

        /// <summary>
        /// 将角度限制在 [-180, 180) 范围内
        /// </summary>
        public static float ClampAngle180(float angle)
        {
            angle = ClampAngle360(angle);
            return angle >= 180f ? angle - 360f : angle;
        }

        /// <summary>
        /// 将角度限制在指定范围内
        /// </summary>
        public static float ClampAngle(float angle, float min, float max)
        {
            angle = ClampAngle180(angle);
            min = ClampAngle180(min);
            max = ClampAngle180(max);
            return Clamp(angle, min, max);
        }

        #endregion

        #region 范围检查

        /// <summary>
        /// 检查值是否在指定范围内（包含边界）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool InRange(int value, int min, int max)
        {
            return value >= min && value <= max;
        }

        /// <summary>
        /// 检查值是否在指定范围内（包含边界）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool InRange(float value, float min, float max)
        {
            return value >= min && value <= max;
        }

        /// <summary>
        /// 检查值是否在指定范围内（不包含边界）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool InRangeExclusive(int value, int min, int max)
        {
            return value > min && value < max;
        }

        /// <summary>
        /// 检查值是否在指定范围内（不包含边界）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool InRangeExclusive(float value, float min, float max)
        {
            return value > min && value < max;
        }

        /// <summary>
        /// 检查值是否在 [0, 1] 范围内
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNormalized(float value)
        {
            return value >= 0f && value <= 1f;
        }

        #endregion

        #region 死区处理

        /// <summary>
        /// 应用死区（小于阈值的值归零）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float ApplyDeadzone(float value, float deadzone)
        {
            return Abs(value) < deadzone ? 0f : value;
        }

        /// <summary>
        /// 应用死区并重新映射（保持连续性）
        /// </summary>
        public static float ApplyDeadzoneRemapped(float value, float deadzone)
        {
            float absValue = Abs(value);
            if (absValue < deadzone) return 0f;
            float sign = Sign(value);
            return sign * (absValue - deadzone) / (1f - deadzone);
        }

        /// <summary>
        /// 应用径向死区（用于摇杆输入）
        /// </summary>
        public static void ApplyRadialDeadzone(ref float x, ref float y, float deadzone)
        {
            float magnitude = (float)Math.Sqrt(x * x + y * y);
            if (magnitude < deadzone)
            {
                x = 0f;
                y = 0f;
            }
            else
            {
                float scale = (magnitude - deadzone) / (1f - deadzone) / magnitude;
                x *= scale;
                y *= scale;
            }
        }

        #endregion

        #region 饱和处理

        /// <summary>
        /// 应用饱和限制（超过阈值的值归一）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float ApplySaturation(float value, float saturation)
        {
            if (value > saturation) return 1f;
            if (value < -saturation) return -1f;
            return value;
        }

        /// <summary>
        /// 软限制（使用双曲正切函数平滑限制）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float SoftClamp(float value, float limit)
        {
            return (float)Math.Tanh(value / limit) * limit;
        }

        #endregion
    }
}
