// ==========================================================
// 文件名：BufferConstants.cs
// 命名空间: AFramework.Serialization
// 依赖: System
// ==========================================================

using System;
using System.Runtime.CompilerServices;

namespace AFramework.Serialization
{
    /// <summary>
    /// 缓冲区管理常量定义
    /// <para>提供缓冲区池、内存管理相关的配置常量</para>
    /// <para>包含缓冲区大小、池化策略、内存对齐等常量</para>
    /// </summary>
    /// <remarks>
    /// 使用示例:
    /// <code>
    /// // 使用默认缓冲区大小
    /// var buffer = new byte[BufferConstants.DefaultBufferSize];
    /// 
    /// // 检查是否应该池化
    /// if (size &lt;= BufferConstants.Pool.MaxPooledSize)
    ///     return pool.Rent(size);
    /// 
    /// // 使用对齐常量
    /// int aligned = BufferConstants.AlignTo(size, BufferConstants.CacheLineSize);
    /// </code>
    /// </remarks>
    public static class BufferConstants
    {
        #region 基础缓冲区大小常量

        /// <summary>默认缓冲区大小 (4KB)</summary>
        public const int DefaultBufferSize = 4096;

        /// <summary>最小缓冲区大小 (256B)</summary>
        public const int MinBufferSize = 256;

        /// <summary>最大缓冲区大小 (1GB)</summary>
        public const int MaxBufferSize = 1024 * 1024 * 1024;

        /// <summary>小缓冲区大小 (1KB)</summary>
        public const int SmallBufferSize = 1024;

        /// <summary>中等缓冲区大小 (16KB)</summary>
        public const int MediumBufferSize = 16 * 1024;

        /// <summary>大缓冲区大小 (64KB)</summary>
        public const int LargeBufferSize = 64 * 1024;

        /// <summary>超大缓冲区大小 (1MB)</summary>
        public const int HugeBufferSize = 1024 * 1024;

        /// <summary>流式读写缓冲区大小 (8KB)</summary>
        public const int StreamBufferSize = 8 * 1024;

        /// <summary>网络传输缓冲区大小 (32KB)</summary>
        public const int NetworkBufferSize = 32 * 1024;

        /// <summary>文件 IO 缓冲区大小 (64KB)</summary>
        public const int FileBufferSize = 64 * 1024;

        #endregion

        #region 内存对齐常量

        /// <summary>CPU 缓存行大小 (64B)</summary>
        public const int CacheLineSize = 64;

        /// <summary>内存页大小 (4KB)</summary>
        public const int PageSize = 4096;

        /// <summary>大页大小 (2MB)</summary>
        public const int LargePageSize = 2 * 1024 * 1024;

        /// <summary>默认内存对齐 (8B)</summary>
        public const int DefaultAlignment = 8;

        /// <summary>SIMD 对齐 (16B - SSE)</summary>
        public const int SimdAlignment16 = 16;

        /// <summary>SIMD 对齐 (32B - AVX)</summary>
        public const int SimdAlignment32 = 32;

        /// <summary>SIMD 对齐 (64B - AVX-512)</summary>
        public const int SimdAlignment64 = 64;

        #endregion

        #region 缓冲区池常量

        /// <summary>
        /// 缓冲区池常量集合
        /// <para>定义缓冲区池的配置参数</para>
        /// </summary>
        public static class Pool
        {
            /// <summary>最大池化缓冲区大小 (1MB)</summary>
            public const int MaxPooledSize = 1024 * 1024;

            /// <summary>最小池化缓冲区大小 (16B)</summary>
            public const int MinPooledSize = 16;

            /// <summary>默认池容量</summary>
            public const int DefaultCapacity = 16;

            /// <summary>最大池容量</summary>
            public const int MaxCapacity = 256;

            /// <summary>每个桶的最大缓冲区数量</summary>
            public const int MaxBuffersPerBucket = 50;

            /// <summary>桶数量 (2^4 到 2^20)</summary>
            public const int BucketCount = 17;

            /// <summary>最小桶索引 (16B = 2^4)</summary>
            public const int MinBucketIndex = 4;

            /// <summary>最大桶索引 (1MB = 2^20)</summary>
            public const int MaxBucketIndex = 20;

            /// <summary>线程本地池大小</summary>
            public const int ThreadLocalPoolSize = 8;

            /// <summary>共享池大小</summary>
            public const int SharedPoolSize = 64;

            /// <summary>池清理间隔 (毫秒)</summary>
            public const int CleanupIntervalMs = 60000; // 1分钟

            /// <summary>池收缩阈值 (使用率低于此值时收缩)</summary>
            public const float ShrinkThreshold = 0.25f;

            /// <summary>池扩展阈值 (使用率高于此值时扩展)</summary>
            public const float GrowThreshold = 0.75f;
        }

        #endregion

        #region 分段缓冲区常量

        /// <summary>
        /// 分段缓冲区常量集合
        /// <para>定义分段缓冲区的配置参数</para>
        /// </summary>
        public static class Segment
        {
            /// <summary>默认段大小 (4KB)</summary>
            public const int DefaultSize = 4096;

            /// <summary>最小段大小 (256B)</summary>
            public const int MinSize = 256;

            /// <summary>最大段大小 (64KB)</summary>
            public const int MaxSize = 64 * 1024;

            /// <summary>最大段数量</summary>
            public const int MaxCount = 1024;

            /// <summary>段池大小</summary>
            public const int PoolSize = 32;

            /// <summary>初始段数量</summary>
            public const int InitialCount = 4;

            /// <summary>段增长因子</summary>
            public const float GrowthFactor = 2.0f;
        }

        #endregion

        #region 写入器常量

        /// <summary>
        /// 写入器常量集合
        /// <para>定义序列化写入器的配置参数</para>
        /// </summary>
        public static class Writer
        {
            /// <summary>初始缓冲区大小 (256B)</summary>
            public const int InitialBufferSize = 256;

            /// <summary>最大单次写入大小 (16MB)</summary>
            public const int MaxSingleWriteSize = 16 * 1024 * 1024;

            /// <summary>缓冲区增长因子</summary>
            public const float GrowthFactor = 2.0f;

            /// <summary>最大增长大小 (1MB)</summary>
            public const int MaxGrowthSize = 1024 * 1024;

            /// <summary>刷新阈值 (缓冲区使用率)</summary>
            public const float FlushThreshold = 0.9f;

            /// <summary>预分配阈值 (预估大小超过此值时预分配)</summary>
            public const int PreallocateThreshold = 1024;

            /// <summary>内联写入最大大小 (避免方法调用开销)</summary>
            public const int InlineWriteMaxSize = 64;
        }

        #endregion

        #region 读取器常量

        /// <summary>
        /// 读取器常量集合
        /// <para>定义序列化读取器的配置参数</para>
        /// </summary>
        public static class Reader
        {
            /// <summary>默认预读大小 (4KB)</summary>
            public const int DefaultPrefetchSize = 4096;

            /// <summary>最大单次读取大小 (16MB)</summary>
            public const int MaxSingleReadSize = 16 * 1024 * 1024;

            /// <summary>预读阈值 (剩余数据低于此值时预读)</summary>
            public const int PrefetchThreshold = 1024;

            /// <summary>跳过缓冲区大小 (用于跳过大块数据)</summary>
            public const int SkipBufferSize = 8192;

            /// <summary>内联读取最大大小</summary>
            public const int InlineReadMaxSize = 64;

            /// <summary>字符串读取缓冲区大小</summary>
            public const int StringBufferSize = 256;
        }

        #endregion

        #region 内存管理常量

        /// <summary>
        /// 内存管理常量集合
        /// <para>定义内存分配和管理的配置参数</para>
        /// </summary>
        public static class Memory
        {
            /// <summary>大对象堆阈值 (85KB)</summary>
            public const int LargeObjectHeapThreshold = 85000;

            /// <summary>栈分配最大大小 (1KB)</summary>
            public const int MaxStackAllocSize = 1024;

            /// <summary>固定内存最大大小 (用于 GCHandle.Alloc)</summary>
            public const int MaxPinnedSize = 1024 * 1024;

            /// <summary>非托管内存对齐</summary>
            public const int UnmanagedAlignment = 16;

            /// <summary>内存映射文件最小大小</summary>
            public const int MemoryMappedMinSize = 64 * 1024;

            /// <summary>内存映射文件最大大小 (2GB)</summary>
            public const long MemoryMappedMaxSize = 2L * 1024 * 1024 * 1024;

            /// <summary>零拷贝阈值 (超过此大小使用零拷贝)</summary>
            public const int ZeroCopyThreshold = 256;

            /// <summary>内存复制阈值 (超过此大小使用 Buffer.MemoryCopy)</summary>
            public const int MemoryCopyThreshold = 64;
        }

        #endregion

        #region 压缩缓冲区常量

        /// <summary>
        /// 压缩缓冲区常量集合
        /// <para>定义压缩操作的缓冲区配置</para>
        /// </summary>
        public static class Compression
        {
            /// <summary>LZ4 块大小 (64KB)</summary>
            public const int Lz4BlockSize = 64 * 1024;

            /// <summary>Brotli 缓冲区大小 (32KB)</summary>
            public const int BrotliBufferSize = 32 * 1024;

            /// <summary>Zstd 缓冲区大小 (128KB)</summary>
            public const int ZstdBufferSize = 128 * 1024;

            /// <summary>Gzip 缓冲区大小 (32KB)</summary>
            public const int GzipBufferSize = 32 * 1024;

            /// <summary>Deflate 缓冲区大小 (32KB)</summary>
            public const int DeflateBufferSize = 32 * 1024;

            /// <summary>压缩输出缓冲区预估倍数</summary>
            public const float OutputBufferMultiplier = 1.1f;

            /// <summary>最小压缩数据大小</summary>
            public const int MinCompressSize = 64;
        }

        #endregion

        #region 工具方法

        /// <summary>
        /// 将大小对齐到指定边界
        /// </summary>
        /// <param name="size">原始大小</param>
        /// <param name="alignment">对齐边界</param>
        /// <returns>对齐后的大小</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int AlignTo(int size, int alignment)
        {
            return (size + alignment - 1) & ~(alignment - 1);
        }

        /// <summary>
        /// 将大小对齐到指定边界 (long 版本)
        /// </summary>
        /// <param name="size">原始大小</param>
        /// <param name="alignment">对齐边界</param>
        /// <returns>对齐后的大小</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long AlignTo(long size, long alignment)
        {
            return (size + alignment - 1) & ~(alignment - 1);
        }

        /// <summary>
        /// 将大小对齐到缓存行边界
        /// </summary>
        /// <param name="size">原始大小</param>
        /// <returns>对齐后的大小</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int AlignToCacheLine(int size)
        {
            return AlignTo(size, CacheLineSize);
        }

        /// <summary>
        /// 将大小对齐到页边界
        /// </summary>
        /// <param name="size">原始大小</param>
        /// <returns>对齐后的大小</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int AlignToPage(int size)
        {
            return AlignTo(size, PageSize);
        }

        /// <summary>
        /// 检查大小是否已对齐
        /// </summary>
        /// <param name="size">要检查的大小</param>
        /// <param name="alignment">对齐边界</param>
        /// <returns>如果已对齐返回 true</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsAligned(int size, int alignment)
        {
            return (size & (alignment - 1)) == 0;
        }

        /// <summary>
        /// 获取适合指定大小的缓冲区池桶索引
        /// </summary>
        /// <param name="size">请求的大小</param>
        /// <returns>桶索引</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetBucketIndex(int size)
        {
            if (size <= Pool.MinPooledSize) return Pool.MinBucketIndex;
            if (size > Pool.MaxPooledSize) return -1;

            // 计算 log2(size) 向上取整
            int index = 32 - LeadingZeroCount((uint)(size - 1));
            return Math.Max(index, Pool.MinBucketIndex);
        }

        /// <summary>
        /// 计算前导零位数
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int LeadingZeroCount(uint value)
        {
            if (value == 0) return 32;
            int count = 0;
            if ((value & 0xFFFF0000) == 0) { count += 16; value <<= 16; }
            if ((value & 0xFF000000) == 0) { count += 8; value <<= 8; }
            if ((value & 0xF0000000) == 0) { count += 4; value <<= 4; }
            if ((value & 0xC0000000) == 0) { count += 2; value <<= 2; }
            if ((value & 0x80000000) == 0) { count += 1; }
            return count;
        }

        /// <summary>
        /// 获取指定桶索引的缓冲区大小
        /// </summary>
        /// <param name="bucketIndex">桶索引</param>
        /// <returns>缓冲区大小</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetBucketSize(int bucketIndex)
        {
            if (bucketIndex < Pool.MinBucketIndex) return Pool.MinPooledSize;
            if (bucketIndex > Pool.MaxBucketIndex) return Pool.MaxPooledSize;
            return 1 << bucketIndex;
        }

        /// <summary>
        /// 计算缓冲区增长后的新大小
        /// </summary>
        /// <param name="currentSize">当前大小</param>
        /// <param name="requiredSize">需要的最小大小</param>
        /// <returns>新的缓冲区大小</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CalculateGrowSize(int currentSize, int requiredSize)
        {
            // 使用增长因子计算新大小
            int newSize = (int)(currentSize * Writer.GrowthFactor);
            
            // 限制单次增长大小
            if (newSize - currentSize > Writer.MaxGrowthSize)
            {
                newSize = currentSize + Writer.MaxGrowthSize;
            }

            // 确保满足最小需求
            if (newSize < requiredSize)
            {
                newSize = requiredSize;
            }

            // 对齐到 2 的幂次
            return (int)RoundUpToPowerOf2((uint)newSize);
        }

        /// <summary>
        /// 向上取整到 2 的幂次
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static uint RoundUpToPowerOf2(uint value)
        {
            if (value == 0) return 1;
            value--;
            value |= value >> 1;
            value |= value >> 2;
            value |= value >> 4;
            value |= value >> 8;
            value |= value >> 16;
            return value + 1;
        }

        /// <summary>
        /// 检查是否应该使用栈分配
        /// </summary>
        /// <param name="size">请求的大小</param>
        /// <returns>如果应该使用栈分配返回 true</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ShouldStackAlloc(int size)
        {
            return size <= Memory.MaxStackAllocSize;
        }

        /// <summary>
        /// 检查是否会分配到大对象堆
        /// </summary>
        /// <param name="size">请求的大小</param>
        /// <returns>如果会分配到 LOH 返回 true</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool WillAllocateOnLOH(int size)
        {
            return size >= Memory.LargeObjectHeapThreshold;
        }

        /// <summary>
        /// 检查是否应该使用缓冲区池
        /// </summary>
        /// <param name="size">请求的大小</param>
        /// <returns>如果应该使用池返回 true</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ShouldUsePool(int size)
        {
            return size >= Pool.MinPooledSize && size <= Pool.MaxPooledSize;
        }

        /// <summary>
        /// 检查是否应该使用零拷贝
        /// </summary>
        /// <param name="size">数据大小</param>
        /// <returns>如果应该使用零拷贝返回 true</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ShouldUseZeroCopy(int size)
        {
            return size >= Memory.ZeroCopyThreshold;
        }

        /// <summary>
        /// 获取推荐的缓冲区大小
        /// </summary>
        /// <param name="estimatedSize">预估数据大小</param>
        /// <returns>推荐的缓冲区大小</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetRecommendedBufferSize(int estimatedSize)
        {
            if (estimatedSize <= 0) return DefaultBufferSize;
            if (estimatedSize <= SmallBufferSize) return SmallBufferSize;
            if (estimatedSize <= MediumBufferSize) return MediumBufferSize;
            if (estimatedSize <= LargeBufferSize) return LargeBufferSize;
            if (estimatedSize <= HugeBufferSize) return HugeBufferSize;
            
            // 对于更大的数据，返回对齐到页大小的值
            return AlignToPage(estimatedSize);
        }

        #endregion
    }
}
