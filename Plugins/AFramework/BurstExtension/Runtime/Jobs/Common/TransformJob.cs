// ==========================================================
// 文件名：TransformJob.cs
// 命名空间：AFramework.Burst
// 创建时间：2025-12-31
// 功能描述：数据转换Job，提供高性能的数据映射和转换功能
// ==========================================================

using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Burst.Intrinsics;

namespace AFramework.Burst
{
    /// <summary>
    /// 数据转换委托（Burst兼容）
    /// </summary>
    public interface ITransformOperator<TSource, TResult>
        where TSource : struct
        where TResult : struct
    {
        /// <summary>
        /// 执行转换
        /// </summary>
        TResult Transform(TSource value);
    }

    /// <summary>
    /// 通用数据转换Job
    /// </summary>
    /// <typeparam name="TSource">源类型</typeparam>
    /// <typeparam name="TResult">结果类型</typeparam>
    /// <typeparam name="TOperator">转换操作符</typeparam>
    [BurstCompile]
    public struct TransformJob<TSource, TResult, TOperator> : IJobParallelFor
        where TSource : struct
        where TResult : struct
        where TOperator : struct, ITransformOperator<TSource, TResult>
    {
        /// <summary>源数据</summary>
        [ReadOnly] public NativeArray<TSource> Source;
        
        /// <summary>结果数据</summary>
        [WriteOnly] public NativeArray<TResult> Result;
        
        /// <summary>转换操作符</summary>
        public TOperator Operator;

        [BurstCompile]
        public void Execute(int index)
        {
            Result[index] = Operator.Transform(Source[index]);
        }
    }

    #region 常用转换操作符

    /// <summary>
    /// int转float操作符
    /// </summary>
    [BurstCompile]
    public struct IntToFloatOperator : ITransformOperator<int, float>
    {
        [BurstCompile]
        public float Transform(int value) => value;
    }

    /// <summary>
    /// float转int操作符
    /// </summary>
    [BurstCompile]
    public struct FloatToIntOperator : ITransformOperator<float, int>
    {
        [BurstCompile]
        public int Transform(float value) => (int)value;
    }

    /// <summary>
    /// 缩放操作符
    /// </summary>
    [BurstCompile]
    public struct ScaleFloatOperator : ITransformOperator<float, float>
    {
        public float Scale;

        [BurstCompile]
        public float Transform(float value) => value * Scale;
    }

    /// <summary>
    /// 偏移操作符
    /// </summary>
    [BurstCompile]
    public struct OffsetFloatOperator : ITransformOperator<float, float>
    {
        public float Offset;

        [BurstCompile]
        public float Transform(float value) => value + Offset;
    }

    /// <summary>
    /// 缩放并偏移操作符
    /// </summary>
    [BurstCompile]
    public struct ScaleOffsetFloatOperator : ITransformOperator<float, float>
    {
        public float Scale;
        public float Offset;

        [BurstCompile]
        public float Transform(float value) => value * Scale + Offset;
    }

    /// <summary>
    /// 钳制操作符
    /// </summary>
    [BurstCompile]
    public struct ClampFloatOperator : ITransformOperator<float, float>
    {
        public float Min;
        public float Max;

        [BurstCompile]
        public float Transform(float value)
        {
            return value < Min ? Min : (value > Max ? Max : value);
        }
    }

    #endregion
}
