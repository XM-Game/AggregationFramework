// ==========================================================
// 文件名：SimdUtility.cs
// 命名空间：AFramework.Burst
// 创建时间：2026-01-01
// 功能描述：SIMD工具类，提供SIMD操作的通用工具方法
// 依赖：Unity.Burst, Unity.Mathematics
// ==========================================================

using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Mathematics;

namespace AFramework.Burst
{
    /// <summary>
    /// SIMD工具类
    /// 提供SIMD操作的通用工具方法和辅助功能
    /// </summary>
    [BurstCompile]
    public static class SimdUtility
    {
        #region 常量定义

        /// <summary>
        /// SSE寄存器宽度（字节）
        /// </summary>
        public const int SSE_WIDTH = 16;

        /// <summary>
        /// AVX寄存器宽度（字节）
        /// </summary>
        public const int AVX_WIDTH = 32;

        /// <summary>
        /// AVX-512寄存器宽度（字节）
        /// </summary>
        public const int AVX512_WIDTH = 64;

        /// <summary>
        /// float4向量中的元素数量
        /// </summary>
        public const int FLOAT4_COUNT = 4;

        /// <summary>
        /// int4向量中的元素数量
        /// </summary>
        public const int INT4_COUNT = 4;

        #endregion

        #region 硬件检测

        /// <summary>
        /// 检查是否支持SSE指令集
        /// </summary>
        /// <returns>如果支持返回true</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsSseSupported()
        {
            return X86.Sse.IsSseSupported;
        }

        /// <summary>
        /// 检查是否支持SSE2指令集
        /// </summary>
        /// <returns>如果支持返回true</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsSse2Supported()
        {
            return X86.Sse2.IsSse2Supported;
        }

        /// <summary>
        /// 检查是否支持SSE3指令集
        /// </summary>
        /// <returns>如果支持返回true</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsSse3Supported()
        {
            return X86.Sse3.IsSse3Supported;
        }

        /// <summary>
        /// 检查是否支持SSE4.1指令集
        /// </summary>
        /// <returns>如果支持返回true</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsSse41Supported()
        {
            return X86.Sse4_1.IsSse41Supported;
        }

        /// <summary>
        /// 检查是否支持SSE4.2指令集
        /// </summary>
        /// <returns>如果支持返回true</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsSse42Supported()
        {
            return X86.Sse4_2.IsSse42Supported;
        }

        /// <summary>
        /// 检查是否支持AVX指令集
        /// </summary>
        /// <returns>如果支持返回true</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsAvxSupported()
        {
            return X86.Avx.IsAvxSupported;
        }

        /// <summary>
        /// 检查是否支持AVX2指令集
        /// </summary>
        /// <returns>如果支持返回true</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsAvx2Supported()
        {
            return X86.Avx2.IsAvx2Supported;
        }

        /// <summary>
        /// 检查是否支持FMA指令集
        /// </summary>
        /// <returns>如果支持返回true</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsFmaSupported()
        {
            return X86.Fma.IsFmaSupported;
        }

        /// <summary>
        /// 检查是否支持ARM NEON指令集
        /// </summary>
        /// <returns>如果支持返回true</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNeonSupported()
        {
            return Arm.Neon.IsNeonSupported;
        }

        #endregion

        #region 对齐工具

        /// <summary>
        /// 检查地址是否按指定字节对齐
        /// </summary>
        /// <param name="address">内存地址</param>
        /// <param name="alignment">对齐字节数（必须是2的幂）</param>
        /// <returns>如果对齐返回true</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe bool IsAligned(void* address, int alignment)
        {
            return ((long)address & (alignment - 1)) == 0;
        }

        /// <summary>
        /// 检查地址是否按16字节对齐（SSE要求）
        /// </summary>
        /// <param name="address">内存地址</param>
        /// <returns>如果对齐返回true</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe bool IsAligned16(void* address)
        {
            return IsAligned(address, SSE_WIDTH);
        }

        /// <summary>
        /// 检查地址是否按32字节对齐（AVX要求）
        /// </summary>
        /// <param name="address">内存地址</param>
        /// <returns>如果对齐返回true</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe bool IsAligned32(void* address)
        {
            return IsAligned(address, AVX_WIDTH);
        }

        /// <summary>
        /// 计算向上对齐后的值
        /// </summary>
        /// <param name="value">原始值</param>
        /// <param name="alignment">对齐字节数（必须是2的幂）</param>
        /// <returns>对齐后的值</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int AlignUp(int value, int alignment)
        {
            return (value + alignment - 1) & ~(alignment - 1);
        }

        /// <summary>
        /// 计算向下对齐后的值
        /// </summary>
        /// <param name="value">原始值</param>
        /// <param name="alignment">对齐字节数（必须是2的幂）</param>
        /// <returns>对齐后的值</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int AlignDown(int value, int alignment)
        {
            return value & ~(alignment - 1);
        }

        #endregion

        #region 向量化计算辅助

        /// <summary>
        /// 计算可以进行SIMD处理的元素数量（float4）
        /// </summary>
        /// <param name="totalCount">总元素数量</param>
        /// <returns>可SIMD处理的元素数量（4的倍数）</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetSimdCount(int totalCount)
        {
            return totalCount & ~(FLOAT4_COUNT - 1);
        }

        /// <summary>
        /// 计算剩余需要标量处理的元素数量
        /// </summary>
        /// <param name="totalCount">总元素数量</param>
        /// <returns>剩余元素数量</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetRemainderCount(int totalCount)
        {
            return totalCount & (FLOAT4_COUNT - 1);
        }

        /// <summary>
        /// 计算需要的float4向量数量
        /// </summary>
        /// <param name="floatCount">float元素数量</param>
        /// <returns>float4向量数量</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetFloat4Count(int floatCount)
        {
            return (floatCount + FLOAT4_COUNT - 1) / FLOAT4_COUNT;
        }

        #endregion

        #region 掩码操作

        /// <summary>
        /// 创建float4掩码（用于条件选择）
        /// </summary>
        /// <param name="mask">布尔掩码（每个分量true为全1，false为全0）</param>
        /// <returns>float4掩码</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4 CreateMask(bool4 mask)
        {
            return math.select(float4.zero, math.asfloat(new int4(-1)), mask);
        }

        /// <summary>
        /// 创建int4掩码
        /// </summary>
        /// <param name="mask">布尔掩码</param>
        /// <returns>int4掩码（true为-1，false为0）</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int4 CreateIntMask(bool4 mask)
        {
            return math.select(int4.zero, new int4(-1), mask);
        }

        /// <summary>
        /// 使用掩码混合两个向量
        /// </summary>
        /// <param name="a">第一个向量</param>
        /// <param name="b">第二个向量</param>
        /// <param name="mask">掩码（true选择b，false选择a）</param>
        /// <returns>混合后的向量</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4 Blend(float4 a, float4 b, bool4 mask)
        {
            return math.select(a, b, mask);
        }

        #endregion
    }
}
