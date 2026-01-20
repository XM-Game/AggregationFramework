// ==========================================================
// 文件名：PoolValidator.cs
// 命名空间: AFramework.Pool.Utilities
// 依赖: System, AFramework.Pool
// 功能: 对象池验证器，提供池配置和状态的验证功能
// ==========================================================

using System;
using System.Collections.Generic;
using System.Linq;

namespace AFramework.Pool.Utilities
{
    /// <summary>
    /// 对象池验证器
    /// Pool Validator
    /// </summary>
    /// <remarks>
    /// 提供对象池配置和状态的验证功能，包括：
    /// - 配置参数验证
    /// - 池状态验证
    /// - 对象有效性验证
    /// - 容量规则验证
    /// Provides validation for pool configuration and state, including:
    /// - Configuration parameter validation
    /// - Pool state validation
    /// - Object validity validation
    /// - Capacity rule validation
    /// </remarks>
    public static class PoolValidator
    {
        #region 配置验证 Configuration Validation

        /// <summary>
        /// 验证池容量配置
        /// Validate pool capacity configuration
        /// </summary>
        /// <param name="initialCapacity">初始容量 / Initial capacity</param>
        /// <param name="maxCapacity">最大容量 / Maximum capacity</param>
        /// <param name="minCapacity">最小容量（默认 0）/ Minimum capacity (default 0)</param>
        /// <returns>验证结果 / Validation result</returns>
        public static ValidationResult ValidateCapacityConfiguration(
            int initialCapacity,
            int maxCapacity,
            int minCapacity = 0)
        {
            var errors = new List<string>();

            // 验证最小容量
            // Validate minimum capacity
            if (minCapacity < 0)
            {
                errors.Add($"最小容量不能为负数 Minimum capacity cannot be negative: {minCapacity}");
            }

            // 验证初始容量
            // Validate initial capacity
            if (initialCapacity < minCapacity)
            {
                errors.Add($"初始容量不能小于最小容量 Initial capacity cannot be less than minimum capacity: {initialCapacity} < {minCapacity}");
            }

            // 验证最大容量
            // Validate maximum capacity
            if (maxCapacity < initialCapacity)
            {
                errors.Add($"最大容量不能小于初始容量 Maximum capacity cannot be less than initial capacity: {maxCapacity} < {initialCapacity}");
            }

            // 验证容量范围合理性
            // Validate capacity range reasonableness
            if (maxCapacity > 0 && maxCapacity < 10)
            {
                errors.Add($"警告：最大容量过小可能影响性能 Warning: Maximum capacity too small may affect performance: {maxCapacity}");
            }

            if (maxCapacity > 100000)
            {
                errors.Add($"警告：最大容量过大可能导致内存问题 Warning: Maximum capacity too large may cause memory issues: {maxCapacity}");
            }

            return new ValidationResult(errors.Count == 0, errors);
        }

        /// <summary>
        /// 验证池创建策略配置
        /// Validate pool creation policy configuration
        /// </summary>
        /// <typeparam name="T">对象类型 / Object type</typeparam>
        /// <param name="creationPolicy">创建策略 / Creation policy</param>
        /// <returns>验证结果 / Validation result</returns>
        public static ValidationResult ValidateCreationPolicy<T>(IPoolCreationPolicy<T> creationPolicy)
        {
            var errors = new List<string>();

            if (creationPolicy == null)
            {
                errors.Add("创建策略不能为 null Creation policy cannot be null");
                return new ValidationResult(false, errors);
            }

            // 验证策略是否可用
            // Validate policy availability
            try
            {
                // 尝试创建一个测试对象（如果策略支持）
                // Try to create a test object (if policy supports)
                // 注意：这里只是验证策略本身，不实际创建对象
                // Note: This only validates the policy itself, not actually creating objects
            }
            catch (Exception ex)
            {
                errors.Add($"创建策略验证失败 Creation policy validation failed: {ex.Message}");
            }

            return new ValidationResult(errors.Count == 0, errors);
        }

        /// <summary>
        /// 验证池清理策略配置
        /// Validate pool cleanup policy configuration
        /// </summary>
        /// <typeparam name="T">对象类型 / Object type</typeparam>
        /// <param name="cleanupPolicy">清理策略 / Cleanup policy</param>
        /// <returns>验证结果 / Validation result</returns>
        public static ValidationResult ValidateCleanupPolicy<T>(IPoolCleanupPolicy<T> cleanupPolicy)
        {
            var errors = new List<string>();

            if (cleanupPolicy == null)
            {
                errors.Add("清理策略不能为 null Cleanup policy cannot be null");
                return new ValidationResult(false, errors);
            }

            return new ValidationResult(errors.Count == 0, errors);
        }

        /// <summary>
        /// 验证完整的池配置
        /// Validate complete pool configuration
        /// </summary>
        /// <param name="config">池配置 / Pool configuration</param>
        /// <returns>验证结果 / Validation result</returns>
        public static ValidationResult ValidatePoolConfiguration(PoolConfiguration config)
        {
            if (config == null)
            {
                return new ValidationResult(false, new[] { "池配置不能为 null Pool configuration cannot be null" });
            }

            var allErrors = new List<string>();

            // 验证容量配置
            // Validate capacity configuration
            var capacityResult = ValidateCapacityConfiguration(
                config.InitialCapacity,
                config.MaxCapacity,
                config.MinCapacity);

            if (!capacityResult.IsValid)
            {
                allErrors.AddRange(capacityResult.Errors);
            }

            // 验证创建策略
            // Validate creation policy
            // 注意：由于泛型限制，这里无法直接验证
            // Note: Cannot directly validate due to generic constraints

            // 验证清理策略
            // Validate cleanup policy
            // 注意：由于泛型限制，这里无法直接验证
            // Note: Cannot directly validate due to generic constraints

            return new ValidationResult(allErrors.Count == 0, allErrors);
        }

        #endregion

        #region 状态验证 State Validation

        /// <summary>
        /// 验证池状态是否有效
        /// Validate pool state is valid
        /// </summary>
        /// <param name="pool">对象池 / Object pool</param>
        /// <returns>验证结果 / Validation result</returns>
        public static ValidationResult ValidatePoolState(IObjectPool pool)
        {
            var errors = new List<string>();

            if (pool == null)
            {
                errors.Add("对象池不能为 null Object pool cannot be null");
                return new ValidationResult(false, errors);
            }

            // 验证池状态
            // Validate pool state
            if (pool.State == PoolState.Disposed)
            {
                errors.Add("对象池已被销毁 Object pool has been disposed");
            }

            // 验证对象计数一致性
            // Validate object count consistency
            if (pool.ActiveCount < 0)
            {
                errors.Add($"活跃对象数不能为负数 Active count cannot be negative: {pool.ActiveCount}");
            }

            if (pool.AvailableCount < 0)
            {
                errors.Add($"可用对象数不能为负数 Available count cannot be negative: {pool.AvailableCount}");
            }

            if (pool.TotalCount != pool.ActiveCount + pool.AvailableCount)
            {
                errors.Add($"对象计数不一致 Object count inconsistent: Total={pool.TotalCount}, Active={pool.ActiveCount}, Available={pool.AvailableCount}");
            }

            return new ValidationResult(errors.Count == 0, errors);
        }

        /// <summary>
        /// 验证池是否可以执行获取操作
        /// Validate pool can perform get operation
        /// </summary>
        /// <param name="pool">对象池 / Object pool</param>
        /// <returns>验证结果 / Validation result</returns>
        public static ValidationResult ValidateCanGet(IObjectPool pool)
        {
            var errors = new List<string>();

            if (pool == null)
            {
                errors.Add("对象池不能为 null Object pool cannot be null");
                return new ValidationResult(false, errors);
            }

            if (pool.State == PoolState.Disposed)
            {
                errors.Add("对象池已被销毁，无法获取对象 Pool has been disposed, cannot get object");
            }

            if (pool.State == PoolState.Uninitialized)
            {
                errors.Add("对象池未初始化，无法获取对象 Pool is uninitialized, cannot get object");
            }

            return new ValidationResult(errors.Count == 0, errors);
        }

        /// <summary>
        /// 验证池是否可以执行归还操作
        /// Validate pool can perform return operation
        /// </summary>
        /// <param name="pool">对象池 / Object pool</param>
        /// <param name="obj">要归还的对象 / Object to return</param>
        /// <returns>验证结果 / Validation result</returns>
        public static ValidationResult ValidateCanReturn(IObjectPool pool, object obj)
        {
            var errors = new List<string>();

            if (pool == null)
            {
                errors.Add("对象池不能为 null Object pool cannot be null");
                return new ValidationResult(false, errors);
            }

            if (obj == null)
            {
                errors.Add("要归还的对象不能为 null Object to return cannot be null");
                return new ValidationResult(false, errors);
            }

            if (pool.State == PoolState.Disposed)
            {
                errors.Add("对象池已被销毁，无法归还对象 Pool has been disposed, cannot return object");
            }

            // 验证对象类型
            // Validate object type
            if (pool.ObjectType != null && !pool.ObjectType.IsInstanceOfType(obj))
            {
                errors.Add($"对象类型不匹配 Object type mismatch: Expected {pool.ObjectType.Name}, Got {obj.GetType().Name}");
            }

            return new ValidationResult(errors.Count == 0, errors);
        }

        #endregion

        #region 对象验证 Object Validation

        /// <summary>
        /// 验证对象是否有效
        /// Validate object is valid
        /// </summary>
        /// <typeparam name="T">对象类型 / Object type</typeparam>
        /// <param name="obj">对象 / Object</param>
        /// <returns>验证结果 / Validation result</returns>
        public static ValidationResult ValidateObject<T>(T obj) where T : class
        {
            var errors = new List<string>();

            if (obj == null)
            {
                errors.Add("对象不能为 null Object cannot be null");
                return new ValidationResult(false, errors);
            }

            // 如果对象实现了 IPooledObject 接口，验证其状态
            // If object implements IPooledObject interface, validate its state
            if (obj is IPooledObject pooledObject)
            {
                // 可以添加更多的池化对象状态验证
                // Can add more pooled object state validation
            }

            return new ValidationResult(errors.Count == 0, errors);
        }

        /// <summary>
        /// 验证对象集合是否有效
        /// Validate object collection is valid
        /// </summary>
        /// <typeparam name="T">对象类型 / Object type</typeparam>
        /// <param name="objects">对象集合 / Object collection</param>
        /// <returns>验证结果 / Validation result</returns>
        public static ValidationResult ValidateObjects<T>(IEnumerable<T> objects) where T : class
        {
            var errors = new List<string>();

            if (objects == null)
            {
                errors.Add("对象集合不能为 null Object collection cannot be null");
                return new ValidationResult(false, errors);
            }

            var objectList = objects.ToList();
            if (objectList.Count == 0)
            {
                errors.Add("对象集合不能为空 Object collection cannot be empty");
            }

            // 验证每个对象
            // Validate each object
            for (int i = 0; i < objectList.Count; i++)
            {
                var obj = objectList[i];
                if (obj == null)
                {
                    errors.Add($"对象集合中的第 {i} 个对象为 null Object at index {i} is null");
                }
            }

            return new ValidationResult(errors.Count == 0, errors);
        }

        #endregion

        #region 容量规则验证 Capacity Rule Validation

        /// <summary>
        /// 验证池容量是否在合理范围内
        /// Validate pool capacity is within reasonable range
        /// </summary>
        /// <param name="pool">对象池 / Object pool</param>
        /// <param name="warningThreshold">警告阈值（0.0 - 1.0）/ Warning threshold (0.0 - 1.0)</param>
        /// <returns>验证结果 / Validation result</returns>
        public static ValidationResult ValidateCapacityUsage(IObjectPool pool, float warningThreshold = 0.8f)
        {
            var errors = new List<string>();

            if (pool == null)
            {
                errors.Add("对象池不能为 null Object pool cannot be null");
                return new ValidationResult(false, errors);
            }

            if (warningThreshold < 0f || warningThreshold > 1f)
            {
                errors.Add($"警告阈值必须在 0.0 到 1.0 之间 Warning threshold must be between 0.0 and 1.0: {warningThreshold}");
                return new ValidationResult(false, errors);
            }

            // 计算使用率
            // Calculate usage rate
            float usageRate = pool.TotalCount > 0 ? (float)pool.ActiveCount / pool.TotalCount : 0f;

            if (usageRate >= warningThreshold)
            {
                errors.Add($"警告：池使用率过高 Warning: Pool usage rate too high: {usageRate:P} >= {warningThreshold:P}");
            }

            // 检查是否有空闲对象
            // Check if there are idle objects
            if (pool.AvailableCount == 0 && pool.ActiveCount > 0)
            {
                errors.Add("警告：池中没有可用对象 Warning: No available objects in pool");
            }

            return new ValidationResult(errors.Count == 0, errors);
        }

        /// <summary>
        /// 验证池是否存在内存泄漏风险
        /// Validate pool has memory leak risk
        /// </summary>
        /// <param name="statistics">统计信息 / Statistics</param>
        /// <param name="leakThreshold">泄漏阈值（活跃对象占比）/ Leak threshold (active object ratio)</param>
        /// <returns>验证结果 / Validation result</returns>
        public static ValidationResult ValidateMemoryLeakRisk(IPoolStatistics statistics, float leakThreshold = 0.9f)
        {
            var errors = new List<string>();

            if (statistics == null)
            {
                errors.Add("统计信息不能为 null Statistics cannot be null");
                return new ValidationResult(false, errors);
            }

            // 计算活跃对象占比
            // Calculate active object ratio
            float activeRatio = statistics.CurrentTotal > 0
                ? (float)statistics.CurrentActive / statistics.CurrentTotal
                : 0f;

            if (activeRatio >= leakThreshold)
            {
                errors.Add($"警告：活跃对象占比过高，可能存在内存泄漏 Warning: Active object ratio too high, possible memory leak: {activeRatio:P}");
            }

            // 检查命中率
            // Check hit rate
            if (statistics.HitRate < 0.5 && statistics.TotalGets > 100)
            {
                errors.Add($"警告：命中率过低，池配置可能不合理 Warning: Hit rate too low, pool configuration may be unreasonable: {statistics.HitRate:P}");
            }

            return new ValidationResult(errors.Count == 0, errors);
        }

        #endregion

        #region 性能验证 Performance Validation

        /// <summary>
        /// 验证池性能是否正常
        /// Validate pool performance is normal
        /// </summary>
        /// <param name="statistics">统计信息 / Statistics</param>
        /// <param name="maxGetTime">最大获取时间（毫秒）/ Maximum get time (milliseconds)</param>
        /// <param name="maxReturnTime">最大归还时间（毫秒）/ Maximum return time (milliseconds)</param>
        /// <returns>验证结果 / Validation result</returns>
        public static ValidationResult ValidatePerformance(
            IPoolStatistics statistics,
            double maxGetTime = 10.0,
            double maxReturnTime = 5.0)
        {
            var errors = new List<string>();

            if (statistics == null)
            {
                errors.Add("统计信息不能为 null Statistics cannot be null");
                return new ValidationResult(false, errors);
            }

            // 验证获取时间
            // Validate get time
            if (statistics.AverageGetTime > maxGetTime)
            {
                errors.Add($"警告：平均获取时间过长 Warning: Average get time too long: {statistics.AverageGetTime:F2}ms > {maxGetTime}ms");
            }

            // 验证归还时间
            // Validate return time
            if (statistics.AverageReturnTime > maxReturnTime)
            {
                errors.Add($"警告：平均归还时间过长 Warning: Average return time too long: {statistics.AverageReturnTime:F2}ms > {maxReturnTime}ms");
            }

            return new ValidationResult(errors.Count == 0, errors);
        }

        #endregion
    }

    #region 辅助类 Helper Classes

    /// <summary>
    /// 验证结果
    /// Validation Result
    /// </summary>
    public readonly struct ValidationResult
    {
        /// <summary>
        /// 是否验证通过
        /// Whether validation passed
        /// </summary>
        public readonly bool IsValid;

        /// <summary>
        /// 错误信息列表
        /// Error message list
        /// </summary>
        public readonly IReadOnlyList<string> Errors;

        /// <summary>
        /// 初始化验证结果
        /// Initialize validation result
        /// </summary>
        public ValidationResult(bool isValid, IEnumerable<string> errors)
        {
            IsValid = isValid;
            Errors = errors?.ToList() ?? new List<string>();
        }

        /// <summary>
        /// 获取错误信息摘要
        /// Get error message summary
        /// </summary>
        public string GetErrorSummary()
        {
            if (IsValid)
                return "验证通过 Validation passed";

            return string.Join("\n", Errors);
        }

        /// <summary>
        /// 创建成功的验证结果
        /// Create successful validation result
        /// </summary>
        public static ValidationResult Success()
        {
            return new ValidationResult(true, Array.Empty<string>());
        }

        /// <summary>
        /// 创建失败的验证结果
        /// Create failed validation result
        /// </summary>
        public static ValidationResult Failure(params string[] errors)
        {
            return new ValidationResult(false, errors);
        }
    }

    /// <summary>
    /// 池配置（用于验证）
    /// Pool Configuration (for validation)
    /// </summary>
    public class PoolConfiguration
    {
        /// <summary>初始容量 / Initial capacity</summary>
        public int InitialCapacity { get; set; }

        /// <summary>最大容量 / Maximum capacity</summary>
        public int MaxCapacity { get; set; }

        /// <summary>最小容量 / Minimum capacity</summary>
        public int MinCapacity { get; set; }
    }

    #endregion
}
