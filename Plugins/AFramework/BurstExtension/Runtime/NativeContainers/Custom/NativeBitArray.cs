// ==========================================================
// 文件名：NativeBitArray.cs
// 命名空间：AFramework.Burst
// 创建时间：2025-12-31
// 功能描述：原生位数组，提供高效的位操作功能
// ==========================================================

using System;
using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace AFramework.Burst
{
    /// <summary>
    /// 原生位数组
    /// 提供高效的位操作功能，每个位占用1个bit
    /// </summary>
    [GenerateTestsForBurstCompatibility]
    public struct NativeBitArray : IDisposable
    {
        #region 字段

        private NativeArray<ulong> m_Bits;
        private int m_Length;
        private Allocator m_Allocator;

        #endregion

        #region 常量

        private const int BitsPerLong = 64;
        private const int BitsPerLongLog2 = 6;

        #endregion

        #region 属性

        /// <summary>
        /// 获取位数组长度
        /// </summary>
        public int Length
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => m_Length;
        }

        /// <summary>
        /// 检查位数组是否已创建
        /// </summary>
        public bool IsCreated
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => m_Bits.IsCreated;
        }

        #endregion

        #region 构造函数

        /// <summary>
        /// 创建位数组
        /// </summary>
        /// <param name="length">位数组长度</param>
        /// <param name="allocator">分配器</param>
        public NativeBitArray(int length, Allocator allocator)
        {
            if (length < 0)
                throw new ArgumentException("长度不能为负数", nameof(length));

            m_Length = length;
            m_Allocator = allocator;
            int longCount = (length + BitsPerLong - 1) >> BitsPerLongLog2;
            m_Bits = new NativeArray<ulong>(longCount, allocator);
        }

        #endregion

        #region 公共方法

        /// <summary>
        /// 设置指定位的值
        /// </summary>
        /// <param name="index">位索引</param>
        /// <param name="value">要设置的值</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Set(int index, bool value)
        {
            if (index < 0 || index >= m_Length)
                throw new ArgumentOutOfRangeException(nameof(index));

            int longIndex = index >> BitsPerLongLog2;
            int bitIndex = index & (BitsPerLong - 1);

            if (value)
            {
                m_Bits[longIndex] |= 1UL << bitIndex;
            }
            else
            {
                m_Bits[longIndex] &= ~(1UL << bitIndex);
            }
        }

        /// <summary>
        /// 获取指定位的值
        /// </summary>
        /// <param name="index">位索引</param>
        /// <returns>位的值</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Get(int index)
        {
            if (index < 0 || index >= m_Length)
                throw new ArgumentOutOfRangeException(nameof(index));

            int longIndex = index >> BitsPerLongLog2;
            int bitIndex = index & (BitsPerLong - 1);

            return (m_Bits[longIndex] & (1UL << bitIndex)) != 0;
        }

        /// <summary>
        /// 切换指定位的值
        /// </summary>
        /// <param name="index">位索引</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Toggle(int index)
        {
            Set(index, !Get(index));
        }

        /// <summary>
        /// 设置所有位为指定值
        /// </summary>
        /// <param name="value">要设置的值</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetAll(bool value)
        {
            ulong fillValue = value ? ulong.MaxValue : 0UL;
            for (int i = 0; i < m_Bits.Length; i++)
            {
                m_Bits[i] = fillValue;
            }
        }

        /// <summary>
        /// 清空所有位（设置为false）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear()
        {
            SetAll(false);
        }

        /// <summary>
        /// 统计设置为true的位的数量
        /// </summary>
        /// <returns>设置为true的位的数量</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int CountBits()
        {
            int count = 0;
            for (int i = 0; i < m_Length; i++)
            {
                if (Get(i))
                    count++;
            }
            return count;
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Dispose()
        {
            if (m_Bits.IsCreated)
            {
                m_Bits.Dispose();
            }
        }

        #endregion
    }
}

