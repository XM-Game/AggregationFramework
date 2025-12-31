// ==========================================================
// 文件名：FilterJob.cs
// 命名空间：AFramework.Burst
// 创建时间：2025-12-31
// 功能描述：数据过滤Job，提供高性能的条件过滤功能
// ==========================================================

using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;

namespace AFramework.Burst
{
    /// <summary>
    /// 过滤条件接口（Burst兼容）
    /// </summary>
    public interface IFilterPredicate<T> where T : unmanaged
    {
        /// <summary>
        /// 判断元素是否满足条件
        /// </summary>
        bool Evaluate(T value);
    }

    /// <summary>
    /// 过滤标记Job（第一阶段：标记满足条件的元素）
    /// </summary>
    [BurstCompile]
    public struct FilterMarkJob<T, TPredicate> : IJobParallelFor
        where T : unmanaged
        where TPredicate : struct, IFilterPredicate<T>
    {
        /// <summary>源数据</summary>
        [ReadOnly] public NativeArray<T> Source;
        
        /// <summary>标记数组（1=满足条件，0=不满足）</summary>
        [WriteOnly] public NativeArray<int> Marks;
        
        /// <summary>过滤条件</summary>
        public TPredicate Predicate;

        [BurstCompile]
        public void Execute(int index)
        {
            Marks[index] = Predicate.Evaluate(Source[index]) ? 1 : 0;
        }
    }

    /// <summary>
    /// 过滤计数Job（统计满足条件的元素数量）
    /// </summary>
    [BurstCompile]
    public struct FilterCountJob<T, TPredicate> : IJob
        where T : unmanaged
        where TPredicate : struct, IFilterPredicate<T>
    {
        /// <summary>源数据</summary>
        [ReadOnly] public NativeArray<T> Source;
        
        /// <summary>计数结果</summary>
        public NativeReference<int> Count;
        
        /// <summary>过滤条件</summary>
        public TPredicate Predicate;

        [BurstCompile]
        public void Execute()
        {
            int count = 0;
            for (int i = 0; i < Source.Length; i++)
            {
                if (Predicate.Evaluate(Source[i]))
                    count++;
            }
            Count.Value = count;
        }
    }

    #region 常用过滤条件

    /// <summary>
    /// 大于条件
    /// </summary>
    [BurstCompile]
    public struct GreaterThanFloat : IFilterPredicate<float>
    {
        public float Threshold;

        [BurstCompile]
        public bool Evaluate(float value) => value > Threshold;
    }

    /// <summary>
    /// 小于条件
    /// </summary>
    [BurstCompile]
    public struct LessThanFloat : IFilterPredicate<float>
    {
        public float Threshold;

        [BurstCompile]
        public bool Evaluate(float value) => value < Threshold;
    }

    /// <summary>
    /// 范围条件
    /// </summary>
    [BurstCompile]
    public struct InRangeFloat : IFilterPredicate<float>
    {
        public float Min;
        public float Max;

        [BurstCompile]
        public bool Evaluate(float value) => value >= Min && value <= Max;
    }

    /// <summary>
    /// 非零条件（int）
    /// </summary>
    [BurstCompile]
    public struct NonZeroInt : IFilterPredicate<int>
    {
        [BurstCompile]
        public bool Evaluate(int value) => value != 0;
    }

    /// <summary>
    /// 正数条件（int）
    /// </summary>
    [BurstCompile]
    public struct PositiveInt : IFilterPredicate<int>
    {
        [BurstCompile]
        public bool Evaluate(int value) => value > 0;
    }

    /// <summary>
    /// 偶数条件
    /// </summary>
    [BurstCompile]
    public struct EvenInt : IFilterPredicate<int>
    {
        [BurstCompile]
        public bool Evaluate(int value) => (value & 1) == 0;
    }

    #endregion
}
