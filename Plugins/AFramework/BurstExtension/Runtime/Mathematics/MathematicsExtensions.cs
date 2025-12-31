// ==========================================================
// 文件名：MathematicsExtensions.cs
// 命名空间：AFramework.Burst
// 创建时间：2026-01-01
// 功能描述：Unity.Mathematics扩展，提供额外的数学工具方法
// 依赖：Unity.Burst, Unity.Mathematics
// ==========================================================

using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Mathematics;

namespace AFramework.Burst
{
    /// <summary>
    /// Unity.Mathematics扩展类
    /// 提供额外的数学工具方法和常用计算
    /// </summary>
    [BurstCompile]
    public static class MathematicsExtensions
    {
        #region 常量

        /// <summary>
        /// 极小值（用于浮点比较）
        /// </summary>
        public const float EPSILON = 1e-6f;

        /// <summary>
        /// 双精度极小值
        /// </summary>
        public const double EPSILON_DOUBLE = 1e-15;

        /// <summary>
        /// 弧度转角度系数
        /// </summary>
        public const float RAD2DEG = 180f / math.PI;

        /// <summary>
        /// 角度转弧度系数
        /// </summary>
        public const float DEG2RAD = math.PI / 180f;

        /// <summary>
        /// 黄金比例
        /// </summary>
        public const float GOLDEN_RATIO = 1.61803398875f;

        #endregion

        #region 角度转换

        /// <summary>
        /// 弧度转角度
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Degrees(float radians) => math.degrees(radians);

        /// <summary>
        /// 角度转弧度
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Radians(float degrees) => math.radians(degrees);

        /// <summary>
        /// 弧度转角度（向量版本）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4 Degrees(float4 radians) => math.degrees(radians);

        /// <summary>
        /// 角度转弧度（向量版本）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4 Radians(float4 degrees) => math.radians(degrees);

        #endregion

        #region 范围映射

        /// <summary>
        /// 将值从一个范围映射到另一个范围
        /// </summary>
        /// <param name="value">输入值</param>
        /// <param name="fromMin">源范围最小值</param>
        /// <param name="fromMax">源范围最大值</param>
        /// <param name="toMin">目标范围最小值</param>
        /// <param name="toMax">目标范围最大值</param>
        /// <returns>映射后的值</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Remap(float value, float fromMin, float fromMax, float toMin, float toMax)
        {
            float t = (value - fromMin) / (fromMax - fromMin);
            return math.lerp(toMin, toMax, t);
        }

        /// <summary>
        /// 将值从一个范围映射到另一个范围（带钳制）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float RemapClamped(float value, float fromMin, float fromMax, float toMin, float toMax)
        {
            float t = math.saturate((value - fromMin) / (fromMax - fromMin));
            return math.lerp(toMin, toMax, t);
        }

        /// <summary>
        /// 将值从0-1范围映射到指定范围
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Remap01(float t, float min, float max)
        {
            return math.lerp(min, max, t);
        }

        /// <summary>
        /// 将值从指定范围映射到0-1范围
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float InverseLerp(float min, float max, float value)
        {
            return (value - min) / (max - min);
        }

        /// <summary>
        /// 将值从指定范围映射到0-1范围（带钳制）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float InverseLerpClamped(float min, float max, float value)
        {
            return math.saturate((value - min) / (max - min));
        }

        #endregion

        #region 插值函数

        /// <summary>
        /// 平滑阶跃插值（Hermite插值）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Smoothstep(float t)
        {
            return t * t * (3f - 2f * t);
        }

        /// <summary>
        /// 更平滑的阶跃插值（Perlin改进版）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Smootherstep(float t)
        {
            return t * t * t * (t * (t * 6f - 15f) + 10f);
        }

        /// <summary>
        /// 指数衰减插值
        /// </summary>
        /// <param name="current">当前值</param>
        /// <param name="target">目标值</param>
        /// <param name="decay">衰减系数（越大越快）</param>
        /// <param name="deltaTime">时间增量</param>
        /// <returns>插值后的值</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float ExpDecay(float current, float target, float decay, float deltaTime)
        {
            return math.lerp(current, target, 1f - math.exp(-decay * deltaTime));
        }

        /// <summary>
        /// 指数衰减插值（float3版本）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3 ExpDecay(float3 current, float3 target, float decay, float deltaTime)
        {
            return math.lerp(current, target, 1f - math.exp(-decay * deltaTime));
        }

        /// <summary>
        /// 弹簧阻尼插值
        /// </summary>
        /// <param name="current">当前值</param>
        /// <param name="target">目标值</param>
        /// <param name="velocity">当前速度（引用，会被修改）</param>
        /// <param name="smoothTime">平滑时间</param>
        /// <param name="deltaTime">时间增量</param>
        /// <param name="maxSpeed">最大速度（可选）</param>
        /// <returns>插值后的值</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float SmoothDamp(float current, float target, ref float velocity,
            float smoothTime, float deltaTime, float maxSpeed = float.MaxValue)
        {
            smoothTime = math.max(0.0001f, smoothTime);
            float omega = 2f / smoothTime;
            float x = omega * deltaTime;
            float exp = 1f / (1f + x + 0.48f * x * x + 0.235f * x * x * x);
            float change = current - target;
            float maxChange = maxSpeed * smoothTime;
            change = math.clamp(change, -maxChange, maxChange);
            float temp = (velocity + omega * change) * deltaTime;
            velocity = (velocity - omega * temp) * exp;
            float result = target + (change + temp) * exp;
            if ((target - current > 0f) == (result > target))
            {
                result = target;
                velocity = 0f;
            }
            return result;
        }

        #endregion

        #region 周期函数

        /// <summary>
        /// 将角度规范化到-180到180度范围
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float NormalizeAngle180(float angle)
        {
            angle = math.fmod(angle + 180f, 360f);
            if (angle < 0f) angle += 360f;
            return angle - 180f;
        }

        /// <summary>
        /// 将角度规范化到0到360度范围
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float NormalizeAngle360(float angle)
        {
            angle = math.fmod(angle, 360f);
            if (angle < 0f) angle += 360f;
            return angle;
        }

        /// <summary>
        /// 将弧度规范化到-PI到PI范围
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float NormalizeRadians(float radians)
        {
            radians = math.fmod(radians + math.PI, math.PI * 2f);
            if (radians < 0f) radians += math.PI * 2f;
            return radians - math.PI;
        }

        /// <summary>
        /// 计算两个角度之间的最短差值（度）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float DeltaAngle(float current, float target)
        {
            float delta = NormalizeAngle180(target - current);
            return delta;
        }

        /// <summary>
        /// 乒乓函数（在0和length之间来回）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float PingPong(float t, float length)
        {
            t = math.fmod(t, length * 2f);
            return length - math.abs(t - length);
        }

        /// <summary>
        /// 重复函数（在0和length之间循环）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Repeat(float t, float length)
        {
            return math.clamp(t - math.floor(t / length) * length, 0f, length);
        }

        #endregion

        #region 近似比较

        /// <summary>
        /// 检查两个浮点数是否近似相等
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Approximately(float a, float b, float epsilon = EPSILON)
        {
            return math.abs(a - b) < epsilon;
        }

        /// <summary>
        /// 检查两个向量是否近似相等
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Approximately(float3 a, float3 b, float epsilon = EPSILON)
        {
            return math.lengthsq(a - b) < epsilon * epsilon;
        }

        /// <summary>
        /// 检查浮点数是否近似为零
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsZero(float value, float epsilon = EPSILON)
        {
            return math.abs(value) < epsilon;
        }

        /// <summary>
        /// 检查向量是否近似为零向量
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsZero(float3 value, float epsilon = EPSILON)
        {
            return math.lengthsq(value) < epsilon * epsilon;
        }

        #endregion

        #region 数值处理

        /// <summary>
        /// 将值对齐到指定步长
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Snap(float value, float step)
        {
            return math.round(value / step) * step;
        }

        /// <summary>
        /// 将向量对齐到指定步长
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3 Snap(float3 value, float step)
        {
            return math.round(value / step) * step;
        }

        /// <summary>
        /// 计算下一个2的幂
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int NextPowerOfTwo(int value)
        {
            value--;
            value |= value >> 1;
            value |= value >> 2;
            value |= value >> 4;
            value |= value >> 8;
            value |= value >> 16;
            return value + 1;
        }

        /// <summary>
        /// 检查是否为2的幂
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsPowerOfTwo(int value)
        {
            return value > 0 && (value & (value - 1)) == 0;
        }

        /// <summary>
        /// 安全除法（避免除以零）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float SafeDivide(float a, float b, float defaultValue = 0f)
        {
            return math.abs(b) > EPSILON ? a / b : defaultValue;
        }

        #endregion

        #region 随机数辅助

        /// <summary>
        /// 生成指定范围内的随机浮点数
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float RandomRange(ref Random random, float min, float max)
        {
            return random.NextFloat(min, max);
        }

        /// <summary>
        /// 生成单位圆内的随机点
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float2 RandomInsideUnitCircle(ref Random random)
        {
            float angle = random.NextFloat() * math.PI * 2f;
            float radius = math.sqrt(random.NextFloat());
            return new float2(math.cos(angle), math.sin(angle)) * radius;
        }

        /// <summary>
        /// 生成单位球内的随机点
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3 RandomInsideUnitSphere(ref Random random)
        {
            float theta = random.NextFloat() * math.PI * 2f;
            float phi = math.acos(2f * random.NextFloat() - 1f);
            float radius = math.pow(random.NextFloat(), 1f / 3f);
            float sinPhi = math.sin(phi);
            return new float3(
                radius * sinPhi * math.cos(theta),
                radius * sinPhi * math.sin(theta),
                radius * math.cos(phi));
        }

        /// <summary>
        /// 生成单位球面上的随机点
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3 RandomOnUnitSphere(ref Random random)
        {
            float theta = random.NextFloat() * math.PI * 2f;
            float phi = math.acos(2f * random.NextFloat() - 1f);
            float sinPhi = math.sin(phi);
            return new float3(
                sinPhi * math.cos(theta),
                sinPhi * math.sin(theta),
                math.cos(phi));
        }

        #endregion
    }
}
