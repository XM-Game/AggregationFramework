// ==========================================================
// 文件名：Float4Extensions.cs
// 命名空间：AFramework.Burst
// 创建时间：2026-01-01
// 功能描述：float4扩展方法，提供便捷的向量操作
// 依赖：Unity.Burst, Unity.Mathematics
// ==========================================================

using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Mathematics;

namespace AFramework.Burst
{
    /// <summary>
    /// float4扩展方法类
    /// 提供便捷的向量操作和常用计算
    /// </summary>
    [BurstCompile]
    public static class Float4Extensions
    {
        #region 分量操作

        /// <summary>
        /// 获取向量的xyz分量作为float3
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3 XYZ(this float4 v) => v.xyz;

        /// <summary>
        /// 获取向量的xy分量作为float2
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float2 XY(this float4 v) => v.xy;

        /// <summary>
        /// 获取向量的xz分量作为float2
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float2 XZ(this float4 v) => v.xz;

        /// <summary>
        /// 获取向量的yz分量作为float2
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float2 YZ(this float4 v) => v.yz;

        /// <summary>
        /// 设置x分量
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4 WithX(this float4 v, float x) => new float4(x, v.y, v.z, v.w);

        /// <summary>
        /// 设置y分量
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4 WithY(this float4 v, float y) => new float4(v.x, y, v.z, v.w);

        /// <summary>
        /// 设置z分量
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4 WithZ(this float4 v, float z) => new float4(v.x, v.y, z, v.w);

        /// <summary>
        /// 设置w分量
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4 WithW(this float4 v, float w) => new float4(v.x, v.y, v.z, w);

        #endregion

        #region 数学运算

        /// <summary>
        /// 计算向量长度
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Length(this float4 v) => math.length(v);

        /// <summary>
        /// 计算向量长度的平方
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float LengthSq(this float4 v) => math.lengthsq(v);

        /// <summary>
        /// 归一化向量
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4 Normalized(this float4 v) => math.normalize(v);

        /// <summary>
        /// 安全归一化（零向量返回零向量）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4 NormalizedSafe(this float4 v) => math.normalizesafe(v);

        /// <summary>
        /// 计算绝对值
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4 Abs(this float4 v) => math.abs(v);

        /// <summary>
        /// 取负
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4 Negated(this float4 v) => -v;

        /// <summary>
        /// 计算倒数
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4 Reciprocal(this float4 v) => math.rcp(v);

        #endregion

        #region 归约操作

        /// <summary>
        /// 计算所有分量的和
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Sum(this float4 v) => math.csum(v);

        /// <summary>
        /// 计算所有分量的最小值
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Min(this float4 v) => math.cmin(v);

        /// <summary>
        /// 计算所有分量的最大值
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Max(this float4 v) => math.cmax(v);

        /// <summary>
        /// 计算所有分量的乘积
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Product(this float4 v) => v.x * v.y * v.z * v.w;

        /// <summary>
        /// 计算所有分量的平均值
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Average(this float4 v) => math.csum(v) * 0.25f;

        #endregion

        #region 钳制和饱和

        /// <summary>
        /// 钳制到指定范围
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4 Clamped(this float4 v, float min, float max)
        {
            return math.clamp(v, min, max);
        }

        /// <summary>
        /// 钳制到指定范围（向量版本）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4 Clamped(this float4 v, float4 min, float4 max)
        {
            return math.clamp(v, min, max);
        }

        /// <summary>
        /// 饱和（钳制到0-1范围）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4 Saturated(this float4 v) => math.saturate(v);

        /// <summary>
        /// 钳制向量长度
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4 ClampedLength(this float4 v, float maxLength)
        {
            float len = math.length(v);
            if (len > maxLength && len > 0f)
                return v * (maxLength / len);
            return v;
        }

        #endregion

        #region 取整操作

        /// <summary>
        /// 向下取整
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4 Floor(this float4 v) => math.floor(v);

        /// <summary>
        /// 向上取整
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4 Ceil(this float4 v) => math.ceil(v);

        /// <summary>
        /// 四舍五入
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4 Round(this float4 v) => math.round(v);

        /// <summary>
        /// 截断（向零取整）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4 Trunc(this float4 v) => math.trunc(v);

        /// <summary>
        /// 取小数部分
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4 Frac(this float4 v) => math.frac(v);

        /// <summary>
        /// 转换为int4
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int4 ToInt4(this float4 v) => (int4)v;

        /// <summary>
        /// 向下取整并转换为int4
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int4 FloorToInt(this float4 v) => (int4)math.floor(v);

        /// <summary>
        /// 向上取整并转换为int4
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int4 CeilToInt(this float4 v) => (int4)math.ceil(v);

        /// <summary>
        /// 四舍五入并转换为int4
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int4 RoundToInt(this float4 v) => (int4)math.round(v);

        #endregion

        #region 插值

        /// <summary>
        /// 线性插值到目标向量
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4 LerpTo(this float4 v, float4 target, float t)
        {
            return math.lerp(v, target, t);
        }

        /// <summary>
        /// 向目标移动（限制最大距离）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4 MoveTowards(this float4 current, float4 target, float maxDelta)
        {
            float4 diff = target - current;
            float dist = math.length(diff);
            if (dist <= maxDelta || dist < 1e-10f)
                return target;
            return current + diff / dist * maxDelta;
        }

        #endregion

        #region 比较操作

        /// <summary>
        /// 检查是否近似相等
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Approximately(this float4 v, float4 other, float epsilon = 1e-6f)
        {
            return math.lengthsq(v - other) < epsilon * epsilon;
        }

        /// <summary>
        /// 检查是否为零向量
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsZero(this float4 v, float epsilon = 1e-6f)
        {
            return math.lengthsq(v) < epsilon * epsilon;
        }

        /// <summary>
        /// 检查是否包含NaN
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasNaN(this float4 v)
        {
            return math.any(math.isnan(v));
        }

        /// <summary>
        /// 检查是否包含无穷大
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasInfinity(this float4 v)
        {
            return math.any(math.isinf(v));
        }

        /// <summary>
        /// 检查是否所有分量都是有限数
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsFinite(this float4 v)
        {
            return math.all(math.isfinite(v));
        }

        #endregion

        #region 特殊操作

        /// <summary>
        /// 计算点积
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Dot(this float4 v, float4 other) => math.dot(v, other);

        /// <summary>
        /// 计算到另一个向量的距离
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float DistanceTo(this float4 v, float4 other) => math.distance(v, other);

        /// <summary>
        /// 计算到另一个向量的距离的平方
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float DistanceSqTo(this float4 v, float4 other) => math.distancesq(v, other);

        /// <summary>
        /// 分量相乘
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4 Scale(this float4 v, float4 scale) => v * scale;

        /// <summary>
        /// 将NaN替换为指定值
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4 ReplaceNaN(this float4 v, float replacement = 0f)
        {
            return math.select(v, replacement, math.isnan(v));
        }

        #endregion
    }
}
