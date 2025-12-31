// ==========================================================
// 文件名：IntrinsicsWrapper.cs
// 命名空间：AFramework.Burst
// 创建时间：2026-01-01
// 功能描述：Burst Intrinsics封装，提供跨平台的SIMD指令抽象
// 依赖：Unity.Burst, Unity.Burst.Intrinsics
// ==========================================================

using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Mathematics;

namespace AFramework.Burst
{
    /// <summary>
    /// Burst Intrinsics封装类
    /// 提供跨平台的SIMD指令抽象，自动选择最优实现
    /// </summary>
    [BurstCompile]
    public static class IntrinsicsWrapper
    {
        #region v128 加载和存储

        /// <summary>
        /// 从内存加载128位向量（对齐加载）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe v128 LoadAligned(float* ptr)
        {
            if (X86.Sse.IsSseSupported)
                return X86.Sse.load_ps(ptr);
            if (Arm.Neon.IsNeonSupported)
                return Arm.Neon.vld1q_f32(ptr);
            return new v128(ptr[0], ptr[1], ptr[2], ptr[3]);
        }

        /// <summary>
        /// 从内存加载128位向量（非对齐加载）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe v128 LoadUnaligned(float* ptr)
        {
            if (X86.Sse.IsSseSupported)
                return X86.Sse.loadu_ps(ptr);
            if (Arm.Neon.IsNeonSupported)
                return Arm.Neon.vld1q_f32(ptr);
            return new v128(ptr[0], ptr[1], ptr[2], ptr[3]);
        }

        /// <summary>
        /// 存储128位向量到内存（对齐存储）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void StoreAligned(float* ptr, v128 value)
        {
            if (X86.Sse.IsSseSupported)
            {
                X86.Sse.store_ps(ptr, value);
                return;
            }
            if (Arm.Neon.IsNeonSupported)
            {
                Arm.Neon.vst1q_f32(ptr, value);
                return;
            }
            ptr[0] = value.Float0;
            ptr[1] = value.Float1;
            ptr[2] = value.Float2;
            ptr[3] = value.Float3;
        }

        /// <summary>
        /// 存储128位向量到内存（非对齐存储）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void StoreUnaligned(float* ptr, v128 value)
        {
            if (X86.Sse.IsSseSupported)
            {
                X86.Sse.storeu_ps(ptr, value);
                return;
            }
            if (Arm.Neon.IsNeonSupported)
            {
                Arm.Neon.vst1q_f32(ptr, value);
                return;
            }
            ptr[0] = value.Float0;
            ptr[1] = value.Float1;
            ptr[2] = value.Float2;
            ptr[3] = value.Float3;
        }

        #endregion

        #region v128 算术运算

        /// <summary>
        /// 128位浮点加法
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static v128 Add(v128 a, v128 b)
        {
            if (X86.Sse.IsSseSupported)
                return X86.Sse.add_ps(a, b);
            if (Arm.Neon.IsNeonSupported)
                return Arm.Neon.vaddq_f32(a, b);
            return new v128(
                a.Float0 + b.Float0,
                a.Float1 + b.Float1,
                a.Float2 + b.Float2,
                a.Float3 + b.Float3);
        }

        /// <summary>
        /// 128位浮点减法
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static v128 Sub(v128 a, v128 b)
        {
            if (X86.Sse.IsSseSupported)
                return X86.Sse.sub_ps(a, b);
            if (Arm.Neon.IsNeonSupported)
                return Arm.Neon.vsubq_f32(a, b);
            return new v128(
                a.Float0 - b.Float0,
                a.Float1 - b.Float1,
                a.Float2 - b.Float2,
                a.Float3 - b.Float3);
        }

        /// <summary>
        /// 128位浮点乘法
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static v128 Mul(v128 a, v128 b)
        {
            if (X86.Sse.IsSseSupported)
                return X86.Sse.mul_ps(a, b);
            if (Arm.Neon.IsNeonSupported)
                return Arm.Neon.vmulq_f32(a, b);
            return new v128(
                a.Float0 * b.Float0,
                a.Float1 * b.Float1,
                a.Float2 * b.Float2,
                a.Float3 * b.Float3);
        }

        /// <summary>
        /// 128位浮点除法
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static v128 Div(v128 a, v128 b)
        {
            if (X86.Sse.IsSseSupported)
                return X86.Sse.div_ps(a, b);
            // NEON没有直接的除法指令，使用倒数近似
            if (Arm.Neon.IsNeonSupported)
            {
                v128 rcp = Arm.Neon.vrecpeq_f32(b);
                rcp = Arm.Neon.vmulq_f32(rcp, Arm.Neon.vrecpsq_f32(b, rcp));
                return Arm.Neon.vmulq_f32(a, rcp);
            }
            return new v128(
                a.Float0 / b.Float0,
                a.Float1 / b.Float1,
                a.Float2 / b.Float2,
                a.Float3 / b.Float3);
        }

        /// <summary>
        /// 128位浮点乘加（FMA）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static v128 Fma(v128 a, v128 b, v128 c)
        {
            if (X86.Fma.IsFmaSupported)
                return X86.Fma.fmadd_ps(a, b, c);
            if (Arm.Neon.IsNeonSupported)
                return Arm.Neon.vfmaq_f32(c, a, b);
            return Add(Mul(a, b), c);
        }

        #endregion

        #region v128 数学函数

        /// <summary>
        /// 128位浮点平方根
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static v128 Sqrt(v128 a)
        {
            if (X86.Sse.IsSseSupported)
                return X86.Sse.sqrt_ps(a);
            if (Arm.Neon.IsNeonSupported)
                return Arm.Neon.vsqrtq_f32(a);
            return new v128(
                math.sqrt(a.Float0),
                math.sqrt(a.Float1),
                math.sqrt(a.Float2),
                math.sqrt(a.Float3));
        }

        /// <summary>
        /// 128位浮点倒数平方根（快速近似）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static v128 Rsqrt(v128 a)
        {
            if (X86.Sse.IsSseSupported)
                return X86.Sse.rsqrt_ps(a);
            if (Arm.Neon.IsNeonSupported)
                return Arm.Neon.vrsqrteq_f32(a);
            return new v128(
                math.rsqrt(a.Float0),
                math.rsqrt(a.Float1),
                math.rsqrt(a.Float2),
                math.rsqrt(a.Float3));
        }

        /// <summary>
        /// 128位浮点倒数（快速近似）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static v128 Rcp(v128 a)
        {
            if (X86.Sse.IsSseSupported)
                return X86.Sse.rcp_ps(a);
            if (Arm.Neon.IsNeonSupported)
                return Arm.Neon.vrecpeq_f32(a);
            return new v128(
                1f / a.Float0,
                1f / a.Float1,
                1f / a.Float2,
                1f / a.Float3);
        }

        /// <summary>
        /// 128位浮点最小值
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static v128 Min(v128 a, v128 b)
        {
            if (X86.Sse.IsSseSupported)
                return X86.Sse.min_ps(a, b);
            if (Arm.Neon.IsNeonSupported)
                return Arm.Neon.vminq_f32(a, b);
            return new v128(
                math.min(a.Float0, b.Float0),
                math.min(a.Float1, b.Float1),
                math.min(a.Float2, b.Float2),
                math.min(a.Float3, b.Float3));
        }

        /// <summary>
        /// 128位浮点最大值
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static v128 Max(v128 a, v128 b)
        {
            if (X86.Sse.IsSseSupported)
                return X86.Sse.max_ps(a, b);
            if (Arm.Neon.IsNeonSupported)
                return Arm.Neon.vmaxq_f32(a, b);
            return new v128(
                math.max(a.Float0, b.Float0),
                math.max(a.Float1, b.Float1),
                math.max(a.Float2, b.Float2),
                math.max(a.Float3, b.Float3));
        }

        #endregion

        #region v128 比较运算

        /// <summary>
        /// 128位浮点相等比较
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static v128 CmpEq(v128 a, v128 b)
        {
            if (X86.Sse.IsSseSupported)
                return X86.Sse.cmpeq_ps(a, b);
            if (Arm.Neon.IsNeonSupported)
                return Arm.Neon.vceqq_f32(a, b);
            return new v128(
                a.Float0 == b.Float0 ? -1 : 0,
                a.Float1 == b.Float1 ? -1 : 0,
                a.Float2 == b.Float2 ? -1 : 0,
                a.Float3 == b.Float3 ? -1 : 0);
        }

        /// <summary>
        /// 128位浮点小于比较
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static v128 CmpLt(v128 a, v128 b)
        {
            if (X86.Sse.IsSseSupported)
                return X86.Sse.cmplt_ps(a, b);
            if (Arm.Neon.IsNeonSupported)
                return Arm.Neon.vcltq_f32(a, b);
            return new v128(
                a.Float0 < b.Float0 ? -1 : 0,
                a.Float1 < b.Float1 ? -1 : 0,
                a.Float2 < b.Float2 ? -1 : 0,
                a.Float3 < b.Float3 ? -1 : 0);
        }

        /// <summary>
        /// 128位浮点小于等于比较
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static v128 CmpLe(v128 a, v128 b)
        {
            if (X86.Sse.IsSseSupported)
                return X86.Sse.cmple_ps(a, b);
            if (Arm.Neon.IsNeonSupported)
                return Arm.Neon.vcleq_f32(a, b);
            return new v128(
                a.Float0 <= b.Float0 ? -1 : 0,
                a.Float1 <= b.Float1 ? -1 : 0,
                a.Float2 <= b.Float2 ? -1 : 0,
                a.Float3 <= b.Float3 ? -1 : 0);
        }

        /// <summary>
        /// 128位浮点大于比较
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static v128 CmpGt(v128 a, v128 b)
        {
            if (X86.Sse.IsSseSupported)
                return X86.Sse.cmpgt_ps(a, b);
            if (Arm.Neon.IsNeonSupported)
                return Arm.Neon.vcgtq_f32(a, b);
            return new v128(
                a.Float0 > b.Float0 ? -1 : 0,
                a.Float1 > b.Float1 ? -1 : 0,
                a.Float2 > b.Float2 ? -1 : 0,
                a.Float3 > b.Float3 ? -1 : 0);
        }

        /// <summary>
        /// 128位浮点大于等于比较
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static v128 CmpGe(v128 a, v128 b)
        {
            if (X86.Sse.IsSseSupported)
                return X86.Sse.cmpge_ps(a, b);
            if (Arm.Neon.IsNeonSupported)
                return Arm.Neon.vcgeq_f32(a, b);
            return new v128(
                a.Float0 >= b.Float0 ? -1 : 0,
                a.Float1 >= b.Float1 ? -1 : 0,
                a.Float2 >= b.Float2 ? -1 : 0,
                a.Float3 >= b.Float3 ? -1 : 0);
        }

        #endregion

        #region v128 位运算

        /// <summary>
        /// 128位按位与
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static v128 And(v128 a, v128 b)
        {
            if (X86.Sse.IsSseSupported)
                return X86.Sse.and_ps(a, b);
            if (Arm.Neon.IsNeonSupported)
                return Arm.Neon.vandq_s32(a, b);
            return new v128(
                a.SInt0 & b.SInt0,
                a.SInt1 & b.SInt1,
                a.SInt2 & b.SInt2,
                a.SInt3 & b.SInt3);
        }

        /// <summary>
        /// 128位按位或
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static v128 Or(v128 a, v128 b)
        {
            if (X86.Sse.IsSseSupported)
                return X86.Sse.or_ps(a, b);
            if (Arm.Neon.IsNeonSupported)
                return Arm.Neon.vorrq_s32(a, b);
            return new v128(
                a.SInt0 | b.SInt0,
                a.SInt1 | b.SInt1,
                a.SInt2 | b.SInt2,
                a.SInt3 | b.SInt3);
        }

        /// <summary>
        /// 128位按位异或
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static v128 Xor(v128 a, v128 b)
        {
            if (X86.Sse.IsSseSupported)
                return X86.Sse.xor_ps(a, b);
            if (Arm.Neon.IsNeonSupported)
                return Arm.Neon.veorq_s32(a, b);
            return new v128(
                a.SInt0 ^ b.SInt0,
                a.SInt1 ^ b.SInt1,
                a.SInt2 ^ b.SInt2,
                a.SInt3 ^ b.SInt3);
        }

        /// <summary>
        /// 128位按位与非（a AND NOT b）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static v128 AndNot(v128 a, v128 b)
        {
            if (X86.Sse.IsSseSupported)
                return X86.Sse.andnot_ps(b, a);
            if (Arm.Neon.IsNeonSupported)
                return Arm.Neon.vbicq_s32(a, b);
            return new v128(
                a.SInt0 & ~b.SInt0,
                a.SInt1 & ~b.SInt1,
                a.SInt2 & ~b.SInt2,
                a.SInt3 & ~b.SInt3);
        }

        #endregion

        #region v128 混洗和选择

        /// <summary>
        /// 使用掩码混合两个向量
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static v128 Blend(v128 a, v128 b, v128 mask)
        {
            if (X86.Sse4_1.IsSse41Supported)
                return X86.Sse4_1.blendv_ps(a, b, mask);
            if (Arm.Neon.IsNeonSupported)
                return Arm.Neon.vbslq_f32(mask, b, a);
            // 回退实现：使用位运算
            return Or(AndNot(a, mask), And(b, mask));
        }

        /// <summary>
        /// 广播标量到所有通道
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static v128 Broadcast(float value)
        {
            if (X86.Sse.IsSseSupported)
                return X86.Sse.set1_ps(value);
            if (Arm.Neon.IsNeonSupported)
                return Arm.Neon.vdupq_n_f32(value);
            return new v128(value, value, value, value);
        }

        /// <summary>
        /// 创建零向量
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static v128 Zero()
        {
            if (X86.Sse.IsSseSupported)
                return X86.Sse.setzero_ps();
            return default;
        }

        #endregion

        #region v128 转换

        /// <summary>
        /// float4转v128
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static v128 FromFloat4(float4 value)
        {
            return new v128(value.x, value.y, value.z, value.w);
        }

        /// <summary>
        /// v128转float4
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4 ToFloat4(v128 value)
        {
            return new float4(value.Float0, value.Float1, value.Float2, value.Float3);
        }

        /// <summary>
        /// int4转v128
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static v128 FromInt4(int4 value)
        {
            return new v128(value.x, value.y, value.z, value.w);
        }

        /// <summary>
        /// v128转int4
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int4 ToInt4(v128 value)
        {
            return new int4(value.SInt0, value.SInt1, value.SInt2, value.SInt3);
        }

        #endregion

        #region 水平归约

        /// <summary>
        /// 水平求和（将4个float相加）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float HorizontalSum(v128 a)
        {
            if (X86.Sse3.IsSse3Supported)
            {
                v128 shuf = X86.Sse3.movehdup_ps(a);
                v128 sums = X86.Sse.add_ps(a, shuf);
                shuf = X86.Sse.movehl_ps(shuf, sums);
                sums = X86.Sse.add_ss(sums, shuf);
                return sums.Float0;
            }
            return a.Float0 + a.Float1 + a.Float2 + a.Float3;
        }

        /// <summary>
        /// 水平求最小值
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float HorizontalMin(v128 a)
        {
            float min = a.Float0;
            if (a.Float1 < min) min = a.Float1;
            if (a.Float2 < min) min = a.Float2;
            if (a.Float3 < min) min = a.Float3;
            return min;
        }

        /// <summary>
        /// 水平求最大值
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float HorizontalMax(v128 a)
        {
            float max = a.Float0;
            if (a.Float1 > max) max = a.Float1;
            if (a.Float2 > max) max = a.Float2;
            if (a.Float3 > max) max = a.Float3;
            return max;
        }

        #endregion
    }
}
