// ==========================================================
// 文件名：BurstSerializableAttribute.cs
// 命名空间: AFramework.Serialization
// 依赖: System
// ==========================================================

using System;
using System.Runtime.CompilerServices;

namespace AFramework.Serialization
{
    /// <summary>
    /// Burst 兼容序列化特性
    /// <para>标记类型的序列化代码应兼容 Burst 编译器</para>
    /// <para>等效于 [GenerateMode(GenerateMode.Auto, EnableBurst = true)]</para>
    /// </summary>
    /// <remarks>
    /// <para><b>使用示例：</b></para>
    /// <code>
    /// // Burst 兼容的数据结构
    /// [Serializable]
    /// [BurstSerializable]
    /// public struct BurstData
    /// {
    ///     public int Value;
    ///     public float3 Position;
    /// }
    /// 
    /// // 高性能配置
    /// [Serializable]
    /// [BurstSerializable(SafetyLevel = BurstSafetyLevel.Minimal, EnableSimd = true)]
    /// public struct HighPerfData
    /// {
    ///     public float4 Vector;
    /// }
    /// </code>
    /// 
    /// <para><b>Burst 兼容要求：</b></para>
    /// <list type="bullet">
    ///   <item>类型必须是结构体（struct）</item>
    ///   <item>不能包含托管类型（string、class 等）</item>
    ///   <item>不能使用虚方法或接口</item>
    ///   <item>所有字段必须是 blittable 类型</item>
    /// </list>
    /// 
    /// <para><b>性能优化选项：</b></para>
    /// <list type="bullet">
    ///   <item>EnableSimd - 启用 SIMD 向量化优化</item>
    ///   <item>EnableVectorization - 启用自动向量化</item>
    ///   <item>SafetyLevel - 控制安全检查级别</item>
    /// </list>
    /// </remarks>
    [AttributeUsage(
        AttributeTargets.Struct,
        AllowMultiple = false,
        Inherited = false)]
    public sealed class BurstSerializableAttribute : Attribute
    {
        #region 属性

        /// <summary>
        /// 获取或设置是否启用 SIMD 优化
        /// <para>默认值：true</para>
        /// </summary>
        /// <remarks>
        /// 启用后生成的代码将使用 SIMD 指令进行批量数据处理。
        /// 适用于包含向量类型（float4、int4 等）的结构体。
        /// </remarks>
        public bool EnableSimd { get; set; } = true;

        /// <summary>
        /// 获取或设置是否启用向量化
        /// <para>默认值：true</para>
        /// </summary>
        /// <remarks>
        /// 启用后 Burst 编译器将尝试自动向量化循环操作。
        /// </remarks>
        public bool EnableVectorization { get; set; } = true;

        /// <summary>
        /// 获取或设置安全检查级别
        /// <para>默认值：<see cref="BurstSafetyLevel.Default"/></para>
        /// </summary>
        public BurstSafetyLevel SafetyLevel { get; set; } = BurstSafetyLevel.Default;

        /// <summary>
        /// 获取或设置是否启用快速数学运算
        /// <para>默认值：false</para>
        /// </summary>
        /// <remarks>
        /// 启用后使用更快但精度较低的数学运算。
        /// 警告：可能导致浮点运算结果略有差异。
        /// </remarks>
        public bool EnableFastMath { get; set; }

        /// <summary>
        /// 获取或设置是否启用内存对齐优化
        /// <para>默认值：true</para>
        /// </summary>
        public bool EnableAlignment { get; set; } = true;

        #endregion

        #region 构造函数

        /// <summary>
        /// 初始化 <see cref="BurstSerializableAttribute"/> 的新实例
        /// </summary>
        public BurstSerializableAttribute()
        {
        }

        /// <summary>
        /// 初始化 <see cref="BurstSerializableAttribute"/> 的新实例
        /// </summary>
        /// <param name="safetyLevel">安全检查级别</param>
        public BurstSerializableAttribute(BurstSafetyLevel safetyLevel)
        {
            SafetyLevel = safetyLevel;
        }

        #endregion

        #region 公共方法

        /// <summary>
        /// 检查是否启用任何优化
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool HasOptimizations()
        {
            return EnableSimd || EnableVectorization || EnableFastMath;
        }

        /// <summary>
        /// 检查是否为高性能配置
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsHighPerformance()
        {
            return EnableSimd &&
                   EnableVectorization &&
                   SafetyLevel.IsReleaseReady();
        }

        /// <summary>
        /// 获取配置摘要信息
        /// </summary>
        public string GetSummary()
        {
            var opts = new System.Text.StringBuilder();

            if (EnableSimd) opts.Append("SIMD,");
            if (EnableVectorization) opts.Append("Vec,");
            if (EnableFastMath) opts.Append("FastMath,");
            if (EnableAlignment) opts.Append("Align,");

            if (opts.Length > 0)
                opts.Length--; // 移除末尾逗号

            return $"Safety={SafetyLevel}, Opts=[{opts}]";
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"[BurstSerializable(Safety={SafetyLevel})]";
        }

        #endregion
    }
}
