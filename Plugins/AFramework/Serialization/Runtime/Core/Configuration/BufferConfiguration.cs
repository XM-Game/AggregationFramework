// ==========================================================
// 文件名：BufferConfiguration.cs
// 命名空间: AFramework.Serialization
// 依赖: System
// ==========================================================

using System;
using System.Runtime.CompilerServices;

namespace AFramework.Serialization
{
    /// <summary>
    /// 缓冲区配置
    /// <para>管理序列化缓冲区的分配和池化策略</para>
    /// </summary>
    /// <remarks>
    /// <para><b>功能说明：</b></para>
    /// <list type="bullet">
    ///   <item>配置缓冲区大小和增长策略</item>
    ///   <item>配置缓冲区池化参数</item>
    ///   <item>配置内存分配策略</item>
    /// </list>
    /// 
    /// <para><b>使用示例：</b></para>
    /// <code>
    /// // 使用预设配置
    /// var config = BufferConfiguration.Default;
    /// var smallConfig = BufferConfiguration.Small;
    /// 
    /// // 自定义配置
    /// var custom = new BufferConfiguration
    /// {
    ///     InitialSize = 4096,
    ///     MaxSize = 1024 * 1024,
    ///     GrowthFactor = 2.0,
    ///     UsePooling = true
    /// };
    /// </code>
    /// </remarks>
    public sealed class BufferConfiguration : ConfigurationBase<BufferConfiguration>
    {
        #region 常量

        /// <summary>最小缓冲区大小</summary>
        public const int MinBufferSize = 64;

        /// <summary>默认初始大小</summary>
        public const int DefaultInitialSize = 4096;

        /// <summary>默认最大大小</summary>
        public const int DefaultMaxSize = 1024 * 1024;

        /// <summary>默认增长因子</summary>
        public const double DefaultGrowthFactor = 2.0;

        #endregion

        #region 静态实例

        /// <summary>默认配置</summary>
        public static BufferConfiguration Default { get; } = new BufferConfiguration().AsReadOnly();

        /// <summary>小缓冲区配置（适合小数据）</summary>
        public static BufferConfiguration Small { get; } = new BufferConfiguration
        {
            _initialSize = 256,
            _maxSize = 64 * 1024,
            _poolMaxArrayLength = 16 * 1024
        }.AsReadOnly();

        /// <summary>大缓冲区配置（适合大数据）</summary>
        public static BufferConfiguration Large { get; } = new BufferConfiguration
        {
            _initialSize = 64 * 1024,
            _maxSize = 16 * 1024 * 1024,
            _poolMaxArrayLength = 1024 * 1024
        }.AsReadOnly();

        #endregion

        #region 字段 - 大小配置

        private int _initialSize = DefaultInitialSize;
        private int _maxSize = DefaultMaxSize;
        private int _segmentSize = 4096;

        #endregion

        #region 字段 - 增长策略

        private double _growthFactor = DefaultGrowthFactor;
        private int _linearGrowthThreshold = 64 * 1024;
        private int _linearGrowthIncrement = 32 * 1024;

        #endregion

        #region 字段 - 池化配置

        private bool _usePooling = true;
        private int _poolMaxArrayLength = 1024 * 1024;
        private int _poolMaxArraysPerBucket = 50;
        private bool _clearOnReturn;

        #endregion

        #region 字段 - 内存配置

        private bool _useUnmanagedMemory;
        private bool _pinMemory;
        private int _alignment = 8;

        #endregion

        #region 字段 - 性能配置

        private bool _enableStatistics;
        private bool _trackAllocations;

        #endregion

        #region 属性 - 大小配置

        /// <summary>
        /// 获取或设置初始缓冲区大小
        /// <para>默认值：4096</para>
        /// </summary>
        public int InitialSize
        {
            get => _initialSize;
            set { ThrowIfReadOnly(); _initialSize = Math.Max(MinBufferSize, value); }
        }

        /// <summary>
        /// 获取或设置最大缓冲区大小
        /// <para>默认值：1MB</para>
        /// </summary>
        public int MaxSize
        {
            get => _maxSize;
            set { ThrowIfReadOnly(); _maxSize = Math.Max(_initialSize, value); }
        }

        /// <summary>
        /// 获取或设置分段大小（用于分段缓冲区）
        /// <para>默认值：4096</para>
        /// </summary>
        public int SegmentSize
        {
            get => _segmentSize;
            set { ThrowIfReadOnly(); _segmentSize = Math.Max(MinBufferSize, value); }
        }

        #endregion

        #region 属性 - 增长策略

        /// <summary>
        /// 获取或设置增长因子
        /// <para>默认值：2.0，范围：1.1 ~ 4.0</para>
        /// </summary>
        public double GrowthFactor
        {
            get => _growthFactor;
            set { ThrowIfReadOnly(); _growthFactor = Math.Clamp(value, 1.1, 4.0); }
        }

        /// <summary>
        /// 获取或设置线性增长阈值（超过此大小后使用线性增长）
        /// <para>默认值：64KB</para>
        /// </summary>
        public int LinearGrowthThreshold
        {
            get => _linearGrowthThreshold;
            set { ThrowIfReadOnly(); _linearGrowthThreshold = Math.Max(0, value); }
        }

        /// <summary>
        /// 获取或设置线性增长增量
        /// <para>默认值：32KB</para>
        /// </summary>
        public int LinearGrowthIncrement
        {
            get => _linearGrowthIncrement;
            set { ThrowIfReadOnly(); _linearGrowthIncrement = Math.Max(1024, value); }
        }

        #endregion

        #region 属性 - 池化配置

        /// <summary>
        /// 获取或设置是否使用缓冲区池
        /// <para>默认值：true</para>
        /// </summary>
        public bool UsePooling
        {
            get => _usePooling;
            set { ThrowIfReadOnly(); _usePooling = value; }
        }

        /// <summary>
        /// 获取或设置池中最大数组长度
        /// <para>默认值：1MB</para>
        /// </summary>
        public int PoolMaxArrayLength
        {
            get => _poolMaxArrayLength;
            set { ThrowIfReadOnly(); _poolMaxArrayLength = Math.Max(MinBufferSize, value); }
        }

        /// <summary>
        /// 获取或设置每个桶的最大数组数
        /// <para>默认值：50</para>
        /// </summary>
        public int PoolMaxArraysPerBucket
        {
            get => _poolMaxArraysPerBucket;
            set { ThrowIfReadOnly(); _poolMaxArraysPerBucket = Math.Max(1, value); }
        }

        /// <summary>
        /// 获取或设置归还时是否清空数组
        /// <para>默认值：false</para>
        /// </summary>
        public bool ClearOnReturn
        {
            get => _clearOnReturn;
            set { ThrowIfReadOnly(); _clearOnReturn = value; }
        }

        #endregion

        #region 属性 - 内存配置

        /// <summary>
        /// 获取或设置是否使用非托管内存
        /// <para>默认值：false</para>
        /// </summary>
        public bool UseUnmanagedMemory
        {
            get => _useUnmanagedMemory;
            set { ThrowIfReadOnly(); _useUnmanagedMemory = value; }
        }

        /// <summary>
        /// 获取或设置是否固定内存
        /// <para>默认值：false</para>
        /// </summary>
        public bool PinMemory
        {
            get => _pinMemory;
            set { ThrowIfReadOnly(); _pinMemory = value; }
        }

        /// <summary>
        /// 获取或设置内存对齐边界（必须是 2 的幂）
        /// <para>默认值：8</para>
        /// </summary>
        public int Alignment
        {
            get => _alignment;
            set
            {
                ThrowIfReadOnly();
                if (value <= 0 || (value & (value - 1)) != 0)
                    throw new ArgumentException("对齐边界必须是 2 的幂 / Alignment must be power of 2", nameof(value));
                _alignment = value;
            }
        }

        #endregion

        #region 属性 - 性能配置

        /// <summary>
        /// 获取或设置是否启用统计
        /// <para>默认值：false</para>
        /// </summary>
        public bool EnableStatistics
        {
            get => _enableStatistics;
            set { ThrowIfReadOnly(); _enableStatistics = value; }
        }

        /// <summary>
        /// 获取或设置是否跟踪分配
        /// <para>默认值：false</para>
        /// </summary>
        public bool TrackAllocations
        {
            get => _trackAllocations;
            set { ThrowIfReadOnly(); _trackAllocations = value; }
        }

        #endregion

        #region 公共方法

        /// <summary>
        /// 计算下一个缓冲区大小
        /// </summary>
        /// <param name="currentSize">当前大小</param>
        /// <param name="requiredSize">需要的大小</param>
        /// <returns>新的缓冲区大小</returns>
        public int CalculateNextSize(int currentSize, int requiredSize)
        {
            if (requiredSize <= currentSize)
                return currentSize;

            int newSize;

            if (currentSize >= _linearGrowthThreshold)
            {
                // 线性增长
                newSize = currentSize + _linearGrowthIncrement;
                while (newSize < requiredSize)
                    newSize += _linearGrowthIncrement;
            }
            else
            {
                // 指数增长
                newSize = currentSize;
                while (newSize < requiredSize)
                {
                    newSize = (int)(newSize * _growthFactor);
                    if (newSize >= _linearGrowthThreshold)
                    {
                        while (newSize < requiredSize)
                            newSize += _linearGrowthIncrement;
                        break;
                    }
                }
            }

            return Math.Min(newSize, _maxSize);
        }

        /// <summary>
        /// 对齐大小
        /// </summary>
        /// <param name="size">原始大小</param>
        /// <returns>对齐后的大小</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int AlignSize(int size) => (size + _alignment - 1) & ~(_alignment - 1);

        /// <summary>
        /// 检查大小是否有效
        /// </summary>
        /// <param name="size">要检查的大小</param>
        /// <returns>如果有效返回 true</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsValidSize(int size) => size >= MinBufferSize && size <= _maxSize;

        /// <inheritdoc/>
        public override string GetSummary() =>
            $"Buffer[Init={_initialSize}, Max={_maxSize}, Pool={_usePooling}]";

        #endregion

        #region 保护方法

        /// <inheritdoc/>
        protected override bool OnValidate(ref string error)
        {
            if (_initialSize > _maxSize)
            {
                error = "初始大小不能大于最大大小 / InitialSize cannot exceed MaxSize";
                return false;
            }

            if (_segmentSize > _maxSize)
            {
                error = "分段大小不能大于最大大小 / SegmentSize cannot exceed MaxSize";
                return false;
            }

            return true;
        }

        /// <inheritdoc/>
        protected override void CopyTo(BufferConfiguration target)
        {
            target._initialSize = _initialSize;
            target._maxSize = _maxSize;
            target._segmentSize = _segmentSize;
            target._growthFactor = _growthFactor;
            target._linearGrowthThreshold = _linearGrowthThreshold;
            target._linearGrowthIncrement = _linearGrowthIncrement;
            target._usePooling = _usePooling;
            target._poolMaxArrayLength = _poolMaxArrayLength;
            target._poolMaxArraysPerBucket = _poolMaxArraysPerBucket;
            target._clearOnReturn = _clearOnReturn;
            target._useUnmanagedMemory = _useUnmanagedMemory;
            target._pinMemory = _pinMemory;
            target._alignment = _alignment;
            target._enableStatistics = _enableStatistics;
            target._trackAllocations = _trackAllocations;
        }

        #endregion
    }
}
