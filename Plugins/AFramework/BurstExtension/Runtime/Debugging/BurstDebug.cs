// ==========================================================
// 文件名：BurstDebug.cs
// 命名空间：AFramework.Burst
// 创建时间：2026-01-01
// 功能描述：Burst调试工具，提供Burst兼容的调试功能
// 依赖：Unity.Burst, Unity.Collections
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
    /// Burst调试工具类
    /// 提供Burst兼容的调试输出和诊断功能
    /// </summary>
    [BurstCompile]
    public static class BurstDebug
    {
        #region 调试输出

        /// <summary>
        /// 在Burst中输出调试信息（仅在编辑器和开发构建中有效）
        /// </summary>
        /// <param name="message">调试消息</param>
        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
        [BurstDiscard]
        public static void Log(in FixedString128Bytes message)
        {
            UnityEngine.Debug.Log(message.ToString());
        }

        /// <summary>
        /// 在Burst中输出警告信息
        /// </summary>
        /// <param name="message">警告消息</param>
        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
        [BurstDiscard]
        public static void LogWarning(in FixedString128Bytes message)
        {
            UnityEngine.Debug.LogWarning(message.ToString());
        }

        /// <summary>
        /// 在Burst中输出错误信息
        /// </summary>
        /// <param name="message">错误消息</param>
        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
        [BurstDiscard]
        public static void LogError(in FixedString128Bytes message)
        {
            UnityEngine.Debug.LogError(message.ToString());
        }

        /// <summary>
        /// 输出整数值
        /// </summary>
        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
        [BurstDiscard]
        public static void LogValue(in FixedString64Bytes label, int value)
        {
            UnityEngine.Debug.Log($"{label}: {value}");
        }

        /// <summary>
        /// 输出浮点值
        /// </summary>
        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
        [BurstDiscard]
        public static void LogValue(in FixedString64Bytes label, float value)
        {
            UnityEngine.Debug.Log($"{label}: {value:F4}");
        }

        /// <summary>
        /// 输出float3值
        /// </summary>
        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
        [BurstDiscard]
        public static void LogValue(in FixedString64Bytes label, float3 value)
        {
            UnityEngine.Debug.Log($"{label}: ({value.x:F4}, {value.y:F4}, {value.z:F4})");
        }

        /// <summary>
        /// 输出float4值
        /// </summary>
        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
        [BurstDiscard]
        public static void LogValue(in FixedString64Bytes label, float4 value)
        {
            UnityEngine.Debug.Log($"{label}: ({value.x:F4}, {value.y:F4}, {value.z:F4}, {value.w:F4})");
        }

        /// <summary>
        /// 输出布尔值
        /// </summary>
        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
        [BurstDiscard]
        public static void LogValue(in FixedString64Bytes label, bool value)
        {
            UnityEngine.Debug.Log($"{label}: {value}");
        }

        #endregion

        #region 条件日志

        /// <summary>
        /// 条件日志输出
        /// </summary>
        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
        [BurstDiscard]
        public static void LogIf(bool condition, in FixedString128Bytes message)
        {
            if (condition)
                UnityEngine.Debug.Log(message.ToString());
        }

        /// <summary>
        /// 条件警告输出
        /// </summary>
        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
        [BurstDiscard]
        public static void LogWarningIf(bool condition, in FixedString128Bytes message)
        {
            if (condition)
                UnityEngine.Debug.LogWarning(message.ToString());
        }

        /// <summary>
        /// 条件错误输出
        /// </summary>
        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
        [BurstDiscard]
        public static void LogErrorIf(bool condition, in FixedString128Bytes message)
        {
            if (condition)
                UnityEngine.Debug.LogError(message.ToString());
        }

        #endregion

        #region 数组调试

        /// <summary>
        /// 输出NativeArray的内容（前N个元素）
        /// </summary>
        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
        [BurstDiscard]
        public static void LogArray<T>(in FixedString64Bytes label, NativeArray<T> array, int maxElements = 10) 
            where T : unmanaged
        {
            int count = math.min(array.Length, maxElements);
            var sb = new System.Text.StringBuilder();
            sb.Append(label.ToString());
            sb.Append($" (Length={array.Length}): [");
            
            for (int i = 0; i < count; i++)
            {
                if (i > 0) sb.Append(", ");
                sb.Append(array[i].ToString());
            }
            
            if (array.Length > maxElements)
                sb.Append($", ... ({array.Length - maxElements} more)");
            
            sb.Append("]");
            UnityEngine.Debug.Log(sb.ToString());
        }

        /// <summary>
        /// 输出NativeArray的统计信息
        /// </summary>
        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
        [BurstDiscard]
        public static void LogArrayStats(in FixedString64Bytes label, NativeArray<float> array)
        {
            if (array.Length == 0)
            {
                UnityEngine.Debug.Log($"{label}: Empty array");
                return;
            }

            float min = float.MaxValue;
            float max = float.MinValue;
            float sum = 0f;
            
            for (int i = 0; i < array.Length; i++)
            {
                float v = array[i];
                if (v < min) min = v;
                if (v > max) max = v;
                sum += v;
            }
            
            float avg = sum / array.Length;
            UnityEngine.Debug.Log($"{label}: Length={array.Length}, Min={min:F4}, Max={max:F4}, Avg={avg:F4}, Sum={sum:F4}");
        }

        #endregion

        #region 性能标记

        /// <summary>
        /// 开始性能采样区域（仅在Profiler中可见）
        /// </summary>
        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
        [BurstDiscard]
        public static void BeginSample(string name)
        {
            UnityEngine.Profiling.Profiler.BeginSample(name);
        }

        /// <summary>
        /// 结束性能采样区域
        /// </summary>
        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
        [BurstDiscard]
        public static void EndSample()
        {
            UnityEngine.Profiling.Profiler.EndSample();
        }

        #endregion

        #region 断点和暂停

        /// <summary>
        /// 在编辑器中触发断点
        /// </summary>
        [Conditional("UNITY_EDITOR")]
        [BurstDiscard]
        public static void Break()
        {
            UnityEngine.Debug.Break();
        }

        /// <summary>
        /// 条件断点
        /// </summary>
        [Conditional("UNITY_EDITOR")]
        [BurstDiscard]
        public static void BreakIf(bool condition)
        {
            if (condition)
                UnityEngine.Debug.Break();
        }

        #endregion

        #region 检查工具

        /// <summary>
        /// 检查NaN值
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool CheckNaN(float value, in FixedString64Bytes context = default)
        {
            bool isNaN = math.isnan(value);
            LogErrorIf(isNaN, CreateNaNMessage(context));
            return isNaN;
        }

        /// <summary>
        /// 检查NaN值（float3）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool CheckNaN(float3 value, in FixedString64Bytes context = default)
        {
            bool hasNaN = math.any(math.isnan(value));
            LogErrorIf(hasNaN, CreateNaNMessage(context));
            return hasNaN;
        }

        /// <summary>
        /// 检查无穷大值
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool CheckInfinity(float value, in FixedString64Bytes context = default)
        {
            bool isInf = math.isinf(value);
            LogErrorIf(isInf, CreateInfinityMessage(context));
            return isInf;
        }

        /// <summary>
        /// 检查索引范围
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool CheckIndexRange(int index, int length, in FixedString64Bytes context = default)
        {
            bool outOfRange = index < 0 || index >= length;
            LogErrorIf(outOfRange, CreateIndexOutOfRangeMessage(context, index, length));
            return outOfRange;
        }

        [BurstDiscard]
        private static FixedString128Bytes CreateNaNMessage(in FixedString64Bytes context)
        {
            var msg = new FixedString128Bytes();
            msg.Append((FixedString32Bytes)"NaN detected");
            if (context.Length > 0)
            {
                msg.Append((FixedString32Bytes)" in ");
                msg.Append(context);
            }
            return msg;
        }

        [BurstDiscard]
        private static FixedString128Bytes CreateInfinityMessage(in FixedString64Bytes context)
        {
            var msg = new FixedString128Bytes();
            msg.Append((FixedString32Bytes)"Infinity detected");
            if (context.Length > 0)
            {
                msg.Append((FixedString32Bytes)" in ");
                msg.Append(context);
            }
            return msg;
        }

        [BurstDiscard]
        private static FixedString128Bytes CreateIndexOutOfRangeMessage(in FixedString64Bytes context, int index, int length)
        {
            var msg = new FixedString128Bytes();
            msg.Append((FixedString64Bytes)"Index out of range: ");
            msg.Append(index);
            msg.Append((FixedString32Bytes)" (length=");
            msg.Append(length);
            msg.Append((FixedString32Bytes)")");
            if (context.Length > 0)
            {
                msg.Append((FixedString32Bytes)" in ");
                msg.Append(context);
            }
            return msg;
        }

        #endregion
    }
}
