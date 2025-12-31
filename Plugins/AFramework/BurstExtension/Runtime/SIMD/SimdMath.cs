// ==========================================================
// 文件名：SimdMath.cs
// 命名空间：AFramework.Burst
// 创建时间：2026-01-01
// 功能描述：SIMD数学运算，提供高性能的向量化数学函数
// 依赖：Unity.Burst, Unity.Mathematics
// ==========================================================

using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Mathematics;

namespace AFramework.Burst
{
    /// <summary>
    /// SIMD数学运算类
    /// 提供高性能的向量化数学函数，充分利用SIMD指令集
    /// </summary>
    [BurstCompile]
    public static class SimdMath
    {
        #region 基础算术运算

        /// <summary>
        /// 向量化加法
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4 Add(float4 a, float4 b) => a + b;

        /// <summary>
        /// 向量化减法
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4 Sub(float4 a, float4 b) => a - b;

        /// <summary>
        /// 向量化乘法
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4 Mul(float4 a, float4 b) => a * b;

        /// <summary>
        /// 向量化除法
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4 Div(float4 a, float4 b) => a / b;

        /// <summary>
        /// 向量化乘加运算（Fused Multiply-Add）
        /// 计算 a * b + c，在支持FMA的硬件上只需一条指令
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4 Fma(float4 a, float4 b, float4 c)
        {
            return math.mad(a, b, c);
        }

        /// <summary>
        /// 向量化乘减运算
        /// 计算 a * b - c
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4 Fms(float4 a, float4 b, float4 c)
        {
            return math.mad(a, b, -c);
        }

        /// <summary>
        /// 向量化负乘加运算
        /// 计算 -a * b + c
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4 Fnma(float4 a, float4 b, float4 c)
        {
            return math.mad(-a, b, c);
        }

        #endregion

        #region 数学函数

        /// <summary>
        /// 向量化平方根
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4 Sqrt(float4 x) => math.sqrt(x);

        /// <summary>
        /// 向量化倒数平方根（快速近似）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4 Rsqrt(float4 x) => math.rsqrt(x);

        /// <summary>
        /// 向量化倒数（快速近似）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4 Rcp(float4 x) => math.rcp(x);

        /// <summary>
        /// 向量化绝对值
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4 Abs(float4 x) => math.abs(x);

        /// <summary>
        /// 向量化取负
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4 Neg(float4 x) => -x;

        /// <summary>
        /// 向量化符号函数
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4 Sign(float4 x) => math.sign(x);

        /// <summary>
        /// 向量化向下取整
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4 Floor(float4 x) => math.floor(x);

        /// <summary>
        /// 向量化向上取整
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4 Ceil(float4 x) => math.ceil(x);

        /// <summary>
        /// 向量化四舍五入
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4 Round(float4 x) => math.round(x);

        /// <summary>
        /// 向量化截断
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4 Trunc(float4 x) => math.trunc(x);

        /// <summary>
        /// 向量化取小数部分
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4 Frac(float4 x) => math.frac(x);

        #endregion

        #region 三角函数

        /// <summary>
        /// 向量化正弦
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4 Sin(float4 x) => math.sin(x);

        /// <summary>
        /// 向量化余弦
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4 Cos(float4 x) => math.cos(x);

        /// <summary>
        /// 向量化正切
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4 Tan(float4 x) => math.tan(x);

        /// <summary>
        /// 同时计算正弦和余弦（比分别调用更高效）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SinCos(float4 x, out float4 sin, out float4 cos)
        {
            math.sincos(x, out sin, out cos);
        }

        /// <summary>
        /// 向量化反正弦
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4 Asin(float4 x) => math.asin(x);

        /// <summary>
        /// 向量化反余弦
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4 Acos(float4 x) => math.acos(x);

        /// <summary>
        /// 向量化反正切
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4 Atan(float4 x) => math.atan(x);

        /// <summary>
        /// 向量化双参数反正切
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4 Atan2(float4 y, float4 x) => math.atan2(y, x);

        #endregion

        #region 指数和对数

        /// <summary>
        /// 向量化指数函数（e^x）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4 Exp(float4 x) => math.exp(x);

        /// <summary>
        /// 向量化2的幂（2^x）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4 Exp2(float4 x) => math.exp2(x);

        /// <summary>
        /// 向量化自然对数
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4 Log(float4 x) => math.log(x);

        /// <summary>
        /// 向量化以2为底的对数
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4 Log2(float4 x) => math.log2(x);

        /// <summary>
        /// 向量化以10为底的对数
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4 Log10(float4 x) => math.log10(x);

        /// <summary>
        /// 向量化幂运算
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4 Pow(float4 x, float4 y) => math.pow(x, y);

        #endregion

        #region 比较和选择

        /// <summary>
        /// 向量化最小值
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4 Min(float4 a, float4 b) => math.min(a, b);

        /// <summary>
        /// 向量化最大值
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4 Max(float4 a, float4 b) => math.max(a, b);

        /// <summary>
        /// 向量化钳制
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4 Clamp(float4 x, float4 min, float4 max)
        {
            return math.clamp(x, min, max);
        }

        /// <summary>
        /// 向量化饱和（钳制到0-1范围）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4 Saturate(float4 x) => math.saturate(x);

        /// <summary>
        /// 向量化线性插值
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4 Lerp(float4 a, float4 b, float4 t)
        {
            return math.lerp(a, b, t);
        }

        /// <summary>
        /// 向量化条件选择
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4 Select(float4 a, float4 b, bool4 condition)
        {
            return math.select(a, b, condition);
        }

        /// <summary>
        /// 向量化阶跃函数
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4 Step(float4 edge, float4 x)
        {
            return math.step(edge, x);
        }

        /// <summary>
        /// 向量化平滑阶跃（Hermite插值）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4 Smoothstep(float4 edge0, float4 edge1, float4 x)
        {
            return math.smoothstep(edge0, edge1, x);
        }

        #endregion

        #region 归约操作

        /// <summary>
        /// 水平求和（将float4的4个分量相加）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float HorizontalSum(float4 x)
        {
            return math.csum(x);
        }

        /// <summary>
        /// 水平求最小值
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float HorizontalMin(float4 x)
        {
            return math.cmin(x);
        }

        /// <summary>
        /// 水平求最大值
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float HorizontalMax(float4 x)
        {
            return math.cmax(x);
        }

        /// <summary>
        /// 水平求乘积
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float HorizontalProduct(float4 x)
        {
            return x.x * x.y * x.z * x.w;
        }

        #endregion

        #region 特殊运算

        /// <summary>
        /// 向量化取模
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4 Mod(float4 x, float4 y) => math.fmod(x, y);

        /// <summary>
        /// 向量化复制符号
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4 CopySign(float4 magnitude, float4 sign)
        {
            return math.abs(magnitude) * math.sign(sign);
        }

        /// <summary>
        /// 检查是否为NaN
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool4 IsNaN(float4 x) => math.isnan(x);

        /// <summary>
        /// 检查是否为无穷大
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool4 IsInf(float4 x) => math.isinf(x);

        /// <summary>
        /// 检查是否为有限数
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool4 IsFinite(float4 x) => math.isfinite(x);

        #endregion
    }
}
