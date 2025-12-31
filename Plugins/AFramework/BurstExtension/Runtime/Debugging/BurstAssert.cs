// ==========================================================
// 文件名：BurstAssert.cs
// 命名空间：AFramework.Burst
// 创建时间：2026-01-01
// 功能描述：Burst兼容断言，提供编译时可移除的断言检查
// 依赖：Unity.Burst, Unity.Mathematics
// ==========================================================

using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;

namespace AFramework.Burst
{
    /// <summary>
    /// Burst兼容断言类
    /// 提供在Burst编译代码中可用的断言功能
    /// 在Release构建中自动移除以保证性能
    /// </summary>
    [BurstCompile]
    public static class BurstAssert
    {
        #region 基础断言

        /// <summary>
        /// 断言条件为真
        /// </summary>
        /// <param name="condition">条件</param>
        /// <param name="message">失败时的消息</param>
        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD"), Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsTrue(bool condition, in FixedString128Bytes message = default)
        {
            if (!condition)
                ThrowAssertionFailed(message);
        }

        /// <summary>
        /// 断言条件为假
        /// </summary>
        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD"), Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsFalse(bool condition, in FixedString128Bytes message = default)
        {
            if (condition)
                ThrowAssertionFailed(message);
        }

        /// <summary>
        /// 断言引用不为null（仅用于托管代码）
        /// </summary>
        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD"), Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
        [BurstDiscard]
        public static void IsNotNull<T>(T obj, in FixedString128Bytes message = default) where T : class
        {
            if (obj == null)
                ThrowAssertionFailed(message);
        }

        /// <summary>
        /// 断言指针不为null
        /// </summary>
        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD"), Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void IsNotNull(void* ptr, in FixedString128Bytes message = default)
        {
            if (ptr == null)
                ThrowAssertionFailed(message);
        }

        #endregion

        #region 相等性断言

        /// <summary>
        /// 断言两个整数相等
        /// </summary>
        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD"), Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AreEqual(int expected, int actual, in FixedString128Bytes message = default)
        {
            if (expected != actual)
                ThrowNotEqual(expected, actual, message);
        }

        /// <summary>
        /// 断言两个浮点数近似相等
        /// </summary>
        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD"), Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AreApproximatelyEqual(float expected, float actual, float epsilon = 1e-6f, in FixedString128Bytes message = default)
        {
            if (math.abs(expected - actual) > epsilon)
                ThrowNotApproximatelyEqual(expected, actual, epsilon, message);
        }

        /// <summary>
        /// 断言两个float3近似相等
        /// </summary>
        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD"), Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AreApproximatelyEqual(float3 expected, float3 actual, float epsilon = 1e-6f, in FixedString128Bytes message = default)
        {
            if (math.lengthsq(expected - actual) > epsilon * epsilon)
                ThrowNotApproximatelyEqualVector(message);
        }

        /// <summary>
        /// 断言两个整数不相等
        /// </summary>
        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD"), Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AreNotEqual(int a, int b, in FixedString128Bytes message = default)
        {
            if (a == b)
                ThrowAreEqual(a, message);
        }

        #endregion

        #region 范围断言

        /// <summary>
        /// 断言值大于指定值
        /// </summary>
        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD"), Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsGreater(int value, int threshold, in FixedString128Bytes message = default)
        {
            if (value <= threshold)
                ThrowNotGreater(value, threshold, message);
        }

        /// <summary>
        /// 断言值大于等于指定值
        /// </summary>
        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD"), Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsGreaterOrEqual(int value, int threshold, in FixedString128Bytes message = default)
        {
            if (value < threshold)
                ThrowNotGreaterOrEqual(value, threshold, message);
        }

        /// <summary>
        /// 断言值小于指定值
        /// </summary>
        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD"), Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsLess(int value, int threshold, in FixedString128Bytes message = default)
        {
            if (value >= threshold)
                ThrowNotLess(value, threshold, message);
        }

        /// <summary>
        /// 断言值小于等于指定值
        /// </summary>
        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD"), Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsLessOrEqual(int value, int threshold, in FixedString128Bytes message = default)
        {
            if (value > threshold)
                ThrowNotLessOrEqual(value, threshold, message);
        }

        /// <summary>
        /// 断言值在指定范围内（包含边界）
        /// </summary>
        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD"), Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsInRange(int value, int min, int max, in FixedString128Bytes message = default)
        {
            if (value < min || value > max)
                ThrowOutOfRange(value, min, max, message);
        }

        /// <summary>
        /// 断言浮点值在指定范围内
        /// </summary>
        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD"), Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsInRange(float value, float min, float max, in FixedString128Bytes message = default)
        {
            if (value < min || value > max)
                ThrowOutOfRangeFloat(value, min, max, message);
        }

        #endregion

        #region 索引断言

        /// <summary>
        /// 断言索引有效
        /// </summary>
        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD"), Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsValidIndex(int index, int length, in FixedString128Bytes message = default)
        {
            if (index < 0 || index >= length)
                ThrowIndexOutOfRange(index, length, message);
        }

        /// <summary>
        /// 断言索引范围有效
        /// </summary>
        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD"), Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsValidRange(int startIndex, int count, int length, in FixedString128Bytes message = default)
        {
            if (startIndex < 0 || count < 0 || startIndex + count > length)
                ThrowRangeOutOfBounds(startIndex, count, length, message);
        }

        #endregion

        #region 数值断言

        /// <summary>
        /// 断言值为正数
        /// </summary>
        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD"), Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsPositive(int value, in FixedString128Bytes message = default)
        {
            if (value <= 0)
                ThrowNotPositive(value, message);
        }

        /// <summary>
        /// 断言值为正数（浮点）
        /// </summary>
        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD"), Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsPositive(float value, in FixedString128Bytes message = default)
        {
            if (value <= 0f)
                ThrowNotPositiveFloat(value, message);
        }

        /// <summary>
        /// 断言值为非负数
        /// </summary>
        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD"), Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsNonNegative(int value, in FixedString128Bytes message = default)
        {
            if (value < 0)
                ThrowNegative(value, message);
        }

        /// <summary>
        /// 断言值不为零
        /// </summary>
        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD"), Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsNotZero(int value, in FixedString128Bytes message = default)
        {
            if (value == 0)
                ThrowIsZero(message);
        }

        /// <summary>
        /// 断言值不为零（浮点）
        /// </summary>
        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD"), Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsNotZero(float value, float epsilon = 1e-6f, in FixedString128Bytes message = default)
        {
            if (math.abs(value) < epsilon)
                ThrowIsZero(message);
        }

        /// <summary>
        /// 断言值不是NaN
        /// </summary>
        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD"), Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsNotNaN(float value, in FixedString128Bytes message = default)
        {
            if (math.isnan(value))
                ThrowIsNaN(message);
        }

        /// <summary>
        /// 断言值是有限数
        /// </summary>
        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD"), Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsFinite(float value, in FixedString128Bytes message = default)
        {
            if (!math.isfinite(value))
                ThrowNotFinite(message);
        }

        /// <summary>
        /// 断言向量已归一化
        /// </summary>
        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD"), Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsNormalized(float3 value, float epsilon = 1e-4f, in FixedString128Bytes message = default)
        {
            float lenSq = math.lengthsq(value);
            if (math.abs(lenSq - 1f) > epsilon)
                ThrowNotNormalized(message);
        }

        #endregion

        #region 异常抛出辅助方法

        [BurstDiscard]
        private static void ThrowAssertionFailed(in FixedString128Bytes message)
        {
            string msg = message.Length > 0 ? message.ToString() : "Assertion failed";
            throw new InvalidOperationException($"[BurstAssert] {msg}");
        }

        [BurstDiscard]
        private static void ThrowNotEqual(int expected, int actual, in FixedString128Bytes message)
        {
            string msg = message.Length > 0 ? message.ToString() : $"Expected {expected}, but was {actual}";
            throw new InvalidOperationException($"[BurstAssert] {msg}");
        }

        [BurstDiscard]
        private static void ThrowNotApproximatelyEqual(float expected, float actual, float epsilon, in FixedString128Bytes message)
        {
            string msg = message.Length > 0 ? message.ToString() : $"Expected ~{expected}, but was {actual} (epsilon={epsilon})";
            throw new InvalidOperationException($"[BurstAssert] {msg}");
        }

        [BurstDiscard]
        private static void ThrowNotApproximatelyEqualVector(in FixedString128Bytes message)
        {
            string msg = message.Length > 0 ? message.ToString() : "Vectors are not approximately equal";
            throw new InvalidOperationException($"[BurstAssert] {msg}");
        }

        [BurstDiscard]
        private static void ThrowAreEqual(int value, in FixedString128Bytes message)
        {
            string msg = message.Length > 0 ? message.ToString() : $"Values should not be equal: {value}";
            throw new InvalidOperationException($"[BurstAssert] {msg}");
        }

        [BurstDiscard]
        private static void ThrowNotGreater(int value, int threshold, in FixedString128Bytes message)
        {
            string msg = message.Length > 0 ? message.ToString() : $"Expected {value} > {threshold}";
            throw new InvalidOperationException($"[BurstAssert] {msg}");
        }

        [BurstDiscard]
        private static void ThrowNotGreaterOrEqual(int value, int threshold, in FixedString128Bytes message)
        {
            string msg = message.Length > 0 ? message.ToString() : $"Expected {value} >= {threshold}";
            throw new InvalidOperationException($"[BurstAssert] {msg}");
        }

        [BurstDiscard]
        private static void ThrowNotLess(int value, int threshold, in FixedString128Bytes message)
        {
            string msg = message.Length > 0 ? message.ToString() : $"Expected {value} < {threshold}";
            throw new InvalidOperationException($"[BurstAssert] {msg}");
        }

        [BurstDiscard]
        private static void ThrowNotLessOrEqual(int value, int threshold, in FixedString128Bytes message)
        {
            string msg = message.Length > 0 ? message.ToString() : $"Expected {value} <= {threshold}";
            throw new InvalidOperationException($"[BurstAssert] {msg}");
        }

        [BurstDiscard]
        private static void ThrowOutOfRange(int value, int min, int max, in FixedString128Bytes message)
        {
            string msg = message.Length > 0 ? message.ToString() : $"Value {value} out of range [{min}, {max}]";
            throw new InvalidOperationException($"[BurstAssert] {msg}");
        }

        [BurstDiscard]
        private static void ThrowOutOfRangeFloat(float value, float min, float max, in FixedString128Bytes message)
        {
            string msg = message.Length > 0 ? message.ToString() : $"Value {value} out of range [{min}, {max}]";
            throw new InvalidOperationException($"[BurstAssert] {msg}");
        }

        [BurstDiscard]
        private static void ThrowIndexOutOfRange(int index, int length, in FixedString128Bytes message)
        {
            string msg = message.Length > 0 ? message.ToString() : $"Index {index} out of range [0, {length})";
            throw new IndexOutOfRangeException($"[BurstAssert] {msg}");
        }

        [BurstDiscard]
        private static void ThrowRangeOutOfBounds(int start, int count, int length, in FixedString128Bytes message)
        {
            string msg = message.Length > 0 ? message.ToString() : $"Range [{start}, {start + count}) out of bounds [0, {length})";
            throw new ArgumentOutOfRangeException($"[BurstAssert] {msg}");
        }

        [BurstDiscard]
        private static void ThrowNotPositive(int value, in FixedString128Bytes message)
        {
            string msg = message.Length > 0 ? message.ToString() : $"Expected positive value, but was {value}";
            throw new InvalidOperationException($"[BurstAssert] {msg}");
        }

        [BurstDiscard]
        private static void ThrowNotPositiveFloat(float value, in FixedString128Bytes message)
        {
            string msg = message.Length > 0 ? message.ToString() : $"Expected positive value, but was {value}";
            throw new InvalidOperationException($"[BurstAssert] {msg}");
        }

        [BurstDiscard]
        private static void ThrowNegative(int value, in FixedString128Bytes message)
        {
            string msg = message.Length > 0 ? message.ToString() : $"Expected non-negative value, but was {value}";
            throw new InvalidOperationException($"[BurstAssert] {msg}");
        }

        [BurstDiscard]
        private static void ThrowIsZero(in FixedString128Bytes message)
        {
            string msg = message.Length > 0 ? message.ToString() : "Value should not be zero";
            throw new InvalidOperationException($"[BurstAssert] {msg}");
        }

        [BurstDiscard]
        private static void ThrowIsNaN(in FixedString128Bytes message)
        {
            string msg = message.Length > 0 ? message.ToString() : "Value is NaN";
            throw new InvalidOperationException($"[BurstAssert] {msg}");
        }

        [BurstDiscard]
        private static void ThrowNotFinite(in FixedString128Bytes message)
        {
            string msg = message.Length > 0 ? message.ToString() : "Value is not finite";
            throw new InvalidOperationException($"[BurstAssert] {msg}");
        }

        [BurstDiscard]
        private static void ThrowNotNormalized(in FixedString128Bytes message)
        {
            string msg = message.Length > 0 ? message.ToString() : "Vector is not normalized";
            throw new InvalidOperationException($"[BurstAssert] {msg}");
        }

        #endregion
    }
}
