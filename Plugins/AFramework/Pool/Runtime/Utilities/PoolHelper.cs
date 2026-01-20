// ==========================================================
// 文件名：PoolHelper.cs
// 命名空间: AFramework.Pool.Utilities
// 依赖: System, AFramework.Pool
// 功能: 对象池辅助工具类
// ==========================================================

using System;
using System.Collections.Generic;

namespace AFramework.Pool.Utilities
{
    /// <summary>
    /// 对象池辅助工具
    /// Pool Helper Utilities
    /// </summary>
    public static class PoolHelper
    {
        #region 容量计算 Capacity Calculation

        /// <summary>
        /// 计算推荐的初始容量
        /// Calculate recommended initial capacity
        /// </summary>
        /// <param name="expectedUsage">预期使用量 Expected usage</param>
        /// <param name="safetyFactor">安全系数（默认 1.5）Safety factor (default 1.5)</param>
        public static int CalculateInitialCapacity(int expectedUsage, float safetyFactor = 1.5f)
        {
            if (expectedUsage < 0)
                throw new ArgumentOutOfRangeException(nameof(expectedUsage));
            if (safetyFactor < 1.0f)
                throw new ArgumentOutOfRangeException(nameof(safetyFactor));

            return Math.Max(1, (int)(expectedUsage * safetyFactor));
        }

        /// <summary>
        /// 计算推荐的最大容量
        /// Calculate recommended maximum capacity
        /// </summary>
        public static int CalculateMaxCapacity(int initialCapacity, float growthFactor = 2.0f)
        {
            if (initialCapacity < 0)
                throw new ArgumentOutOfRangeException(nameof(initialCapacity));
            if (growthFactor < 1.0f)
                throw new ArgumentOutOfRangeException(nameof(growthFactor));

            return Math.Max(initialCapacity, (int)(initialCapacity * growthFactor));
        }

        #endregion

        #region 性能分析 Performance Analysis

        /// <summary>
        /// 计算命中率
        /// Calculate hit rate
        /// </summary>
        public static float CalculateHitRate(int hits, int misses)
        {
            int total = hits + misses;
            return total > 0 ? (float)hits / total : 0f;
        }

        /// <summary>
        /// 计算利用率
        /// Calculate utilization rate
        /// </summary>
        public static float CalculateUtilization(int activeCount, int totalCapacity)
        {
            return totalCapacity > 0 ? (float)activeCount / totalCapacity : 0f;
        }

        #endregion

        #region 验证 Validation

        /// <summary>
        /// 验证容量参数
        /// Validate capacity parameters
        /// </summary>
        public static void ValidateCapacity(int initialCapacity, int maxCapacity)
        {
            if (initialCapacity < 0)
                throw new ArgumentOutOfRangeException(nameof(initialCapacity), "初始容量不能为负数 Initial capacity cannot be negative");

            if (maxCapacity < initialCapacity)
                throw new ArgumentOutOfRangeException(nameof(maxCapacity), "最大容量不能小于初始容量 Maximum capacity cannot be less than initial capacity");
        }

        /// <summary>
        /// 验证对象是否为空
        /// Validate object is not null
        /// </summary>
        public static void ValidateNotNull<T>(T obj, string paramName) where T : class
        {
            if (obj == null)
                throw new ArgumentNullException(paramName);
        }

        #endregion
    }
}
