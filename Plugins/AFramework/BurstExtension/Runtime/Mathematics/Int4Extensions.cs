// ==========================================================
// 文件名：Int4Extensions.cs
// 命名空间：AFramework.Burst
// 创建时间：2026-01-01
// 功能描述：int4扩展方法，提供便捷的整数向量操作
// 依赖：Unity.Burst, Unity.Mathematics
// ==========================================================

using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Mathematics;

namespace AFramework.Burst
{
    /// <summary>
    /// int4扩展方法类
    /// 提供便捷的整数向量操作和常用计算
    /// </summary>
    [BurstCompile]
    public static class Int4Extensions
    {
        #region 分量操作

        /// <summary>
        /// 获取向量的xyz分量作为int3
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int3 XYZ(this int4 v) => v.xyz;

        /// <summary>
        /// 获取向量的xy分量作为int2
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int2 XY(this int4 v) => v.xy;

        /// <summary>
        /// 获取向量的xz分量作为int2
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int2 XZ(this int4 v) => v.xz;

        /// <summary>
        /// 获取向量的yz分量作为int2
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int2 YZ(this int4 v) => v.yz;

        /// <summary>
        /// 设置x分量
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int4 WithX(this int4 v, int x) => new int4(x, v.y, v.z, v.w);

        /// <summary>
        /// 设置y分量
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int4 WithY(this int4 v, int y) => new int4(v.x, y, v.z, v.w);

        /// <summary>
        /// 设置z分量
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int4 WithZ(this int4 v, int z) => new int4(v.x, v.y, z, v.w);

        /// <summary>
        /// 设置w分量
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int4 WithW(this int4 v, int w) => new int4(v.x, v.y, v.z, w);

        #endregion

        #region 数学运算

        /// <summary>
        /// 计算绝对值
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int4 Abs(this int4 v) => math.abs(v);

        /// <summary>
        /// 取负
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int4 Negated(this int4 v) => -v;

        /// <summary>
        /// 计算符号
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int4 Sign(this int4 v) => (int4)math.sign(v);

        #endregion

        #region 归约操作

        /// <summary>
        /// 计算所有分量的和
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Sum(this int4 v) => math.csum(v);

        /// <summary>
        /// 计算所有分量的最小值
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Min(this int4 v) => math.cmin(v);

        /// <summary>
        /// 计算所有分量的最大值
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Max(this int4 v) => math.cmax(v);

        /// <summary>
        /// 计算所有分量的乘积
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Product(this int4 v) => v.x * v.y * v.z * v.w;

        #endregion

        #region 钳制

        /// <summary>
        /// 钳制到指定范围
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int4 Clamped(this int4 v, int min, int max)
        {
            return math.clamp(v, min, max);
        }

        /// <summary>
        /// 钳制到指定范围（向量版本）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int4 Clamped(this int4 v, int4 min, int4 max)
        {
            return math.clamp(v, min, max);
        }

        /// <summary>
        /// 钳制到非负范围
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int4 ClampedPositive(this int4 v)
        {
            return math.max(v, 0);
        }

        #endregion

        #region 位运算

        /// <summary>
        /// 按位与
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int4 And(this int4 v, int4 other) => v & other;

        /// <summary>
        /// 按位或
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int4 Or(this int4 v, int4 other) => v | other;

        /// <summary>
        /// 按位异或
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int4 Xor(this int4 v, int4 other) => v ^ other;

        /// <summary>
        /// 按位取反
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int4 Not(this int4 v) => ~v;

        /// <summary>
        /// 左移
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int4 ShiftLeft(this int4 v, int shift) => v << shift;

        /// <summary>
        /// 右移
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int4 ShiftRight(this int4 v, int shift) => v >> shift;

        /// <summary>
        /// 计算前导零的数量
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int4 LeadingZeroCount(this int4 v) => math.lzcnt(v);

        /// <summary>
        /// 计算尾随零的数量
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int4 TrailingZeroCount(this int4 v) => math.tzcnt(v);

        /// <summary>
        /// 计算置位数量（popcount）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int4 PopCount(this int4 v) => math.countbits(v);

        /// <summary>
        /// 位反转
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int4 ReverseBits(this int4 v) => (int4)math.reversebits((uint4)v);

        #endregion

        #region 类型转换

        /// <summary>
        /// 转换为float4
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4 ToFloat4(this int4 v) => (float4)v;

        /// <summary>
        /// 转换为uint4
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint4 ToUInt4(this int4 v) => (uint4)v;

        /// <summary>
        /// 重新解释为float4（位模式不变）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4 AsFloat4(this int4 v) => math.asfloat(v);

        #endregion

        #region 比较操作

        /// <summary>
        /// 检查是否相等
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Equals(this int4 v, int4 other)
        {
            return math.all(v == other);
        }

        /// <summary>
        /// 检查是否为零向量
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsZero(this int4 v)
        {
            return math.all(v == 0);
        }

        /// <summary>
        /// 检查是否所有分量都为正
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AllPositive(this int4 v)
        {
            return math.all(v > 0);
        }

        /// <summary>
        /// 检查是否所有分量都为非负
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AllNonNegative(this int4 v)
        {
            return math.all(v >= 0);
        }

        /// <summary>
        /// 检查是否所有分量都为负
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AllNegative(this int4 v)
        {
            return math.all(v < 0);
        }

        #endregion

        #region 索引操作

        /// <summary>
        /// 计算一维索引（用于多维数组访问）
        /// 假设布局为 [x, y, z, w]，维度为 [dimX, dimY, dimZ]
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ToLinearIndex(this int4 v, int dimX, int dimY, int dimZ)
        {
            return v.x + v.y * dimX + v.z * dimX * dimY + v.w * dimX * dimY * dimZ;
        }

        /// <summary>
        /// 从一维索引计算多维坐标
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int4 FromLinearIndex(int index, int dimX, int dimY, int dimZ)
        {
            int w = index / (dimX * dimY * dimZ);
            index -= w * dimX * dimY * dimZ;
            int z = index / (dimX * dimY);
            index -= z * dimX * dimY;
            int y = index / dimX;
            int x = index - y * dimX;
            return new int4(x, y, z, w);
        }

        #endregion

        #region 特殊操作

        /// <summary>
        /// 分量相乘
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int4 Scale(this int4 v, int4 scale) => v * scale;

        /// <summary>
        /// 计算曼哈顿距离（L1范数）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ManhattanLength(this int4 v)
        {
            return math.csum(math.abs(v));
        }

        /// <summary>
        /// 计算到另一个向量的曼哈顿距离
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ManhattanDistanceTo(this int4 v, int4 other)
        {
            return math.csum(math.abs(v - other));
        }

        /// <summary>
        /// 计算切比雪夫距离（L∞范数）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ChebyshevLength(this int4 v)
        {
            return math.cmax(math.abs(v));
        }

        /// <summary>
        /// 计算到另一个向量的切比雪夫距离
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ChebyshevDistanceTo(this int4 v, int4 other)
        {
            return math.cmax(math.abs(v - other));
        }

        #endregion
    }
}
