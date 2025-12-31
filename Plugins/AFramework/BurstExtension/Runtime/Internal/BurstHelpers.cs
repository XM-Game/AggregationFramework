// ==========================================================
// 文件名：BurstHelpers.cs
// 命名空间：AFramework.Burst.Internal
// 创建时间：2026-01-01
// 功能描述：Burst辅助类，提供内部使用的Burst相关工具方法
// 依赖：Unity.Burst, Unity.Mathematics
// ==========================================================

using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Mathematics;

namespace AFramework.Burst.Internal
{
    /// <summary>
    /// Burst辅助类
    /// 提供内部使用的Burst相关工具方法
    /// </summary>
    [BurstCompile]
    internal static class BurstHelpers
    {
        #region 分支预测提示

        /// <summary>
        /// 提示编译器条件很可能为真
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Likely(bool condition)
        {
            return condition;
        }

        /// <summary>
        /// 提示编译器条件很可能为假
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Unlikely(bool condition)
        {
            return condition;
        }

        #endregion

        #region 内存预取

        /// <summary>
        /// 预取内存到缓存（读取）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void PrefetchRead(void* ptr)
        {
            // Burst会自动优化内存访问模式
            // 这里作为语义提示
        }

        /// <summary>
        /// 预取内存到缓存（写入）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void PrefetchWrite(void* ptr)
        {
            // Burst会自动优化内存访问模式
        }

        #endregion

        #region 循环优化提示

        /// <summary>
        /// 计算最优的循环展开因子
        /// </summary>
        /// <param name="totalIterations">总迭代次数</param>
        /// <param name="simdWidth">SIMD宽度</param>
        /// <returns>建议的展开因子</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetOptimalUnrollFactor(int totalIterations, int simdWidth = 4)
        {
            if (totalIterations < simdWidth)
                return 1;
            if (totalIterations < simdWidth * 2)
                return simdWidth;
            if (totalIterations < simdWidth * 4)
                return simdWidth * 2;
            return simdWidth * 4;
        }

        /// <summary>
        /// 计算向量化循环的迭代次数
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void GetVectorLoopBounds(int totalCount, int vectorWidth, out int vectorCount, out int remainder)
        {
            vectorCount = totalCount / vectorWidth;
            remainder = totalCount - vectorCount * vectorWidth;
        }

        #endregion

        #region 数值工具

        /// <summary>
        /// 快速整数除法（除数为2的幂）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int FastDivPow2(int value, int divisorPow2)
        {
            return value >> divisorPow2;
        }

        /// <summary>
        /// 快速整数取模（除数为2的幂）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int FastModPow2(int value, int divisorMask)
        {
            return value & divisorMask;
        }

        /// <summary>
        /// 计算log2（向下取整）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Log2Floor(int value)
        {
            return 31 - math.lzcnt(value);
        }

        /// <summary>
        /// 计算log2（向上取整）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Log2Ceil(int value)
        {
            int floor = Log2Floor(value);
            return (value & (value - 1)) == 0 ? floor : floor + 1;
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
        /// 向上取整到2的幂
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CeilToPowerOfTwo(int value)
        {
            value--;
            value |= value >> 1;
            value |= value >> 2;
            value |= value >> 4;
            value |= value >> 8;
            value |= value >> 16;
            return value + 1;
        }

        #endregion

        #region 位操作

        /// <summary>
        /// 交换两个整数（无临时变量）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Swap(ref int a, ref int b)
        {
            a ^= b;
            b ^= a;
            a ^= b;
        }

        /// <summary>
        /// 交换两个浮点数
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Swap(ref float a, ref float b)
        {
            float temp = a;
            a = b;
            b = temp;
        }

        /// <summary>
        /// 条件交换（如果a > b则交换）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ConditionalSwap(ref int a, ref int b)
        {
            int diff = a - b;
            int mask = diff >> 31;
            diff &= mask;
            a -= diff;
            b += diff;
        }

        /// <summary>
        /// 提取最低位的1
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ExtractLowestBit(int value)
        {
            return value & (-value);
        }

        /// <summary>
        /// 清除最低位的1
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ClearLowestBit(int value)
        {
            return value & (value - 1);
        }

        /// <summary>
        /// 设置指定位
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SetBit(int value, int bitIndex)
        {
            return value | (1 << bitIndex);
        }

        /// <summary>
        /// 清除指定位
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ClearBit(int value, int bitIndex)
        {
            return value & ~(1 << bitIndex);
        }

        /// <summary>
        /// 切换指定位
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ToggleBit(int value, int bitIndex)
        {
            return value ^ (1 << bitIndex);
        }

        /// <summary>
        /// 检查指定位是否设置
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsBitSet(int value, int bitIndex)
        {
            return (value & (1 << bitIndex)) != 0;
        }

        #endregion

        #region 哈希工具

        /// <summary>
        /// 组合两个哈希值
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CombineHash(int hash1, int hash2)
        {
            return hash1 ^ (hash2 + unchecked((int)0x9e3779b9) + (hash1 << 6) + (hash1 >> 2));
        }

        /// <summary>
        /// 计算整数的哈希值（Wang hash）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint WangHash(uint key)
        {
            key = ~key + (key << 15);
            key = key ^ (key >> 12);
            key = key + (key << 2);
            key = key ^ (key >> 4);
            key = key * 2057;
            key = key ^ (key >> 16);
            return key;
        }

        /// <summary>
        /// 计算float3的哈希值
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int HashFloat3(float3 v)
        {
            int hash = math.asint(v.x);
            hash = CombineHash(hash, math.asint(v.y));
            hash = CombineHash(hash, math.asint(v.z));
            return hash;
        }

        #endregion

        #region 索引计算

        /// <summary>
        /// 计算2D索引到1D索引
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Index2DTo1D(int x, int y, int width)
        {
            return y * width + x;
        }

        /// <summary>
        /// 计算1D索引到2D索引
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int2 Index1DTo2D(int index, int width)
        {
            return new int2(index % width, index / width);
        }

        /// <summary>
        /// 计算3D索引到1D索引
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Index3DTo1D(int x, int y, int z, int width, int height)
        {
            return z * width * height + y * width + x;
        }

        /// <summary>
        /// 计算1D索引到3D索引
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int3 Index1DTo3D(int index, int width, int height)
        {
            int z = index / (width * height);
            int remainder = index - z * width * height;
            int y = remainder / width;
            int x = remainder - y * width;
            return new int3(x, y, z);
        }

        /// <summary>
        /// 计算Morton码（Z-order curve）2D
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint MortonEncode2D(uint x, uint y)
        {
            x = (x | (x << 8)) & 0x00FF00FF;
            x = (x | (x << 4)) & 0x0F0F0F0F;
            x = (x | (x << 2)) & 0x33333333;
            x = (x | (x << 1)) & 0x55555555;

            y = (y | (y << 8)) & 0x00FF00FF;
            y = (y | (y << 4)) & 0x0F0F0F0F;
            y = (y | (y << 2)) & 0x33333333;
            y = (y | (y << 1)) & 0x55555555;

            return x | (y << 1);
        }

        #endregion
    }
}
