// ==========================================================
// 文件名：SimdVector.cs
// 命名空间：AFramework.Burst
// 创建时间：2026-01-01
// 功能描述：SIMD向量操作，提供高性能的向量运算功能
// 依赖：Unity.Burst, Unity.Mathematics
// ==========================================================

using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Mathematics;

namespace AFramework.Burst
{
    /// <summary>
    /// SIMD向量操作类
    /// 提供高性能的向量运算功能，包括点积、叉积、归一化等
    /// </summary>
    [BurstCompile]
    public static class SimdVector
    {
        #region 点积运算

        /// <summary>
        /// 计算两个float2向量的点积
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Dot(float2 a, float2 b) => math.dot(a, b);

        /// <summary>
        /// 计算两个float3向量的点积
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Dot(float3 a, float3 b) => math.dot(a, b);

        /// <summary>
        /// 计算两个float4向量的点积
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Dot(float4 a, float4 b) => math.dot(a, b);

        /// <summary>
        /// 批量计算点积（4组float3向量）
        /// 使用SOA布局提高SIMD效率
        /// </summary>
        /// <param name="ax">第一组向量的x分量</param>
        /// <param name="ay">第一组向量的y分量</param>
        /// <param name="az">第一组向量的z分量</param>
        /// <param name="bx">第二组向量的x分量</param>
        /// <param name="by">第二组向量的y分量</param>
        /// <param name="bz">第二组向量的z分量</param>
        /// <returns>4个点积结果</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4 DotBatch(float4 ax, float4 ay, float4 az,
                                       float4 bx, float4 by, float4 bz)
        {
            return math.mad(ax, bx, math.mad(ay, by, az * bz));
        }

        #endregion

        #region 叉积运算

        /// <summary>
        /// 计算两个float3向量的叉积
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3 Cross(float3 a, float3 b) => math.cross(a, b);

        /// <summary>
        /// 计算2D向量的叉积（返回标量，表示z分量）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Cross2D(float2 a, float2 b)
        {
            return a.x * b.y - a.y * b.x;
        }

        /// <summary>
        /// 批量计算叉积（4组float3向量，SOA布局）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CrossBatch(
            float4 ax, float4 ay, float4 az,
            float4 bx, float4 by, float4 bz,
            out float4 rx, out float4 ry, out float4 rz)
        {
            rx = math.mad(ay, bz, -az * by);
            ry = math.mad(az, bx, -ax * bz);
            rz = math.mad(ax, by, -ay * bx);
        }

        #endregion

        #region 长度和距离

        /// <summary>
        /// 计算向量长度
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Length(float2 v) => math.length(v);

        /// <summary>
        /// 计算向量长度
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Length(float3 v) => math.length(v);

        /// <summary>
        /// 计算向量长度
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Length(float4 v) => math.length(v);

        /// <summary>
        /// 计算向量长度的平方（避免开方运算）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float LengthSq(float2 v) => math.lengthsq(v);

        /// <summary>
        /// 计算向量长度的平方
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float LengthSq(float3 v) => math.lengthsq(v);

        /// <summary>
        /// 计算向量长度的平方
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float LengthSq(float4 v) => math.lengthsq(v);

        /// <summary>
        /// 计算两点之间的距离
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Distance(float2 a, float2 b) => math.distance(a, b);

        /// <summary>
        /// 计算两点之间的距离
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Distance(float3 a, float3 b) => math.distance(a, b);

        /// <summary>
        /// 计算两点之间的距离的平方
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float DistanceSq(float2 a, float2 b) => math.distancesq(a, b);

        /// <summary>
        /// 计算两点之间的距离的平方
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float DistanceSq(float3 a, float3 b) => math.distancesq(a, b);

        /// <summary>
        /// 批量计算长度平方（4组float3向量，SOA布局）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4 LengthSqBatch(float4 x, float4 y, float4 z)
        {
            return math.mad(x, x, math.mad(y, y, z * z));
        }

        /// <summary>
        /// 批量计算长度（4组float3向量，SOA布局）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4 LengthBatch(float4 x, float4 y, float4 z)
        {
            return math.sqrt(LengthSqBatch(x, y, z));
        }

        #endregion

        #region 归一化

        /// <summary>
        /// 归一化向量
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float2 Normalize(float2 v) => math.normalize(v);

        /// <summary>
        /// 归一化向量
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3 Normalize(float3 v) => math.normalize(v);

        /// <summary>
        /// 归一化向量
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4 Normalize(float4 v) => math.normalize(v);

        /// <summary>
        /// 安全归一化（零向量返回零向量）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float2 NormalizeSafe(float2 v, float2 defaultValue = default)
        {
            return math.normalizesafe(v, defaultValue);
        }

        /// <summary>
        /// 安全归一化（零向量返回零向量）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3 NormalizeSafe(float3 v, float3 defaultValue = default)
        {
            return math.normalizesafe(v, defaultValue);
        }

        /// <summary>
        /// 安全归一化（零向量返回零向量）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4 NormalizeSafe(float4 v, float4 defaultValue = default)
        {
            return math.normalizesafe(v, defaultValue);
        }

        /// <summary>
        /// 批量归一化（4组float3向量，SOA布局）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void NormalizeBatch(
            ref float4 x, ref float4 y, ref float4 z)
        {
            float4 invLen = math.rsqrt(LengthSqBatch(x, y, z));
            x *= invLen;
            y *= invLen;
            z *= invLen;
        }

        #endregion

        #region 反射和折射

        /// <summary>
        /// 计算反射向量
        /// </summary>
        /// <param name="incident">入射向量</param>
        /// <param name="normal">法线向量（必须归一化）</param>
        /// <returns>反射向量</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float2 Reflect(float2 incident, float2 normal)
        {
            return math.reflect(incident, normal);
        }

        /// <summary>
        /// 计算反射向量
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3 Reflect(float3 incident, float3 normal)
        {
            return math.reflect(incident, normal);
        }

        /// <summary>
        /// 计算折射向量
        /// </summary>
        /// <param name="incident">入射向量（必须归一化）</param>
        /// <param name="normal">法线向量（必须归一化）</param>
        /// <param name="eta">折射率比值（入射介质/折射介质）</param>
        /// <returns>折射向量</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float2 Refract(float2 incident, float2 normal, float eta)
        {
            return math.refract(incident, normal, eta);
        }

        /// <summary>
        /// 计算折射向量
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3 Refract(float3 incident, float3 normal, float eta)
        {
            return math.refract(incident, normal, eta);
        }

        #endregion

        #region 投影

        /// <summary>
        /// 计算向量在另一向量上的投影
        /// </summary>
        /// <param name="v">要投影的向量</param>
        /// <param name="onto">投影目标向量（不需要归一化）</param>
        /// <returns>投影向量</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float2 Project(float2 v, float2 onto)
        {
            return onto * (math.dot(v, onto) / math.lengthsq(onto));
        }

        /// <summary>
        /// 计算向量在另一向量上的投影
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3 Project(float3 v, float3 onto)
        {
            return onto * (math.dot(v, onto) / math.lengthsq(onto));
        }

        /// <summary>
        /// 计算向量在归一化向量上的投影（更高效）
        /// </summary>
        /// <param name="v">要投影的向量</param>
        /// <param name="ontoNormalized">投影目标向量（必须归一化）</param>
        /// <returns>投影向量</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3 ProjectOnNormal(float3 v, float3 ontoNormalized)
        {
            return ontoNormalized * math.dot(v, ontoNormalized);
        }

        /// <summary>
        /// 计算向量在平面上的投影
        /// </summary>
        /// <param name="v">要投影的向量</param>
        /// <param name="planeNormal">平面法线（必须归一化）</param>
        /// <returns>投影向量</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3 ProjectOnPlane(float3 v, float3 planeNormal)
        {
            return v - ProjectOnNormal(v, planeNormal);
        }

        #endregion

        #region 角度计算

        /// <summary>
        /// 计算两个向量之间的夹角（弧度）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Angle(float2 a, float2 b)
        {
            float denominator = math.sqrt(math.lengthsq(a) * math.lengthsq(b));
            if (denominator < 1e-15f)
                return 0f;
            float dot = math.clamp(math.dot(a, b) / denominator, -1f, 1f);
            return math.acos(dot);
        }

        /// <summary>
        /// 计算两个向量之间的夹角（弧度）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Angle(float3 a, float3 b)
        {
            float denominator = math.sqrt(math.lengthsq(a) * math.lengthsq(b));
            if (denominator < 1e-15f)
                return 0f;
            float dot = math.clamp(math.dot(a, b) / denominator, -1f, 1f);
            return math.acos(dot);
        }

        /// <summary>
        /// 计算两个归一化向量之间的夹角（更高效）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float AngleNormalized(float3 aNormalized, float3 bNormalized)
        {
            float dot = math.clamp(math.dot(aNormalized, bNormalized), -1f, 1f);
            return math.acos(dot);
        }

        /// <summary>
        /// 计算2D向量的有符号角度（-PI到PI）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float SignedAngle2D(float2 from, float2 to)
        {
            return math.atan2(Cross2D(from, to), math.dot(from, to));
        }

        #endregion

        #region 插值

        /// <summary>
        /// 向量线性插值
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float2 Lerp(float2 a, float2 b, float t) => math.lerp(a, b, t);

        /// <summary>
        /// 向量线性插值
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3 Lerp(float3 a, float3 b, float t) => math.lerp(a, b, t);

        /// <summary>
        /// 向量线性插值
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4 Lerp(float4 a, float4 b, float t) => math.lerp(a, b, t);

        /// <summary>
        /// 球面线性插值（用于方向向量）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3 Slerp(float3 a, float3 b, float t)
        {
            float dot = math.clamp(math.dot(a, b), -1f, 1f);
            float theta = math.acos(dot) * t;
            float3 relativeVec = math.normalize(b - a * dot);
            return a * math.cos(theta) + relativeVec * math.sin(theta);
        }

        /// <summary>
        /// 向量朝目标移动（限制最大距离）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3 MoveTowards(float3 current, float3 target, float maxDistanceDelta)
        {
            float3 toTarget = target - current;
            float dist = math.length(toTarget);
            if (dist <= maxDistanceDelta || dist < 1e-10f)
                return target;
            return current + toTarget / dist * maxDistanceDelta;
        }

        #endregion
    }
}
