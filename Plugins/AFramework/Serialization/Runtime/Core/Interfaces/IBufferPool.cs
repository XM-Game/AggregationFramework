// ==========================================================
// 文件名：IBufferPool.cs
// 命名空间: AFramework.Serialization
// 依赖: System
// ==========================================================

using System;
using System.Buffers;

namespace AFramework.Serialization
{
    /// <summary>
    /// 缓冲区池接口
    /// <para>提供高效的缓冲区租借和归还机制</para>
    /// <para>减少内存分配和 GC 压力</para>
    /// </summary>
    /// <remarks>
    /// 缓冲区池是序列化系统的性能关键组件，用于：
    /// 1. 复用临时缓冲区，减少内存分配
    /// 2. 管理大型缓冲区，避免 LOH 碎片
    /// 3. 提供线程安全的缓冲区访问
    /// 
    /// 使用示例:
    /// <code>
    /// // 租借缓冲区
    /// byte[] buffer = pool.Rent(1024);
    /// try
    /// {
    ///     // 使用缓冲区
    ///     ProcessData(buffer);
    /// }
    /// finally
    /// {
    ///     // 归还缓冲区
    ///     pool.Return(buffer);
    /// }
    /// 
    /// // 使用 using 模式
    /// using var rental = pool.RentDisposable(1024);
    /// ProcessData(rental.Buffer);
    /// </code>
    /// </remarks>
    public interface IBufferPool
    {
        /// <summary>
        /// 租借指定最小大小的缓冲区
        /// </summary>
        /// <param name="minimumSize">最小大小</param>
        /// <returns>缓冲区 (实际大小可能大于请求大小)</returns>
        byte[] Rent(int minimumSize);

        /// <summary>
        /// 归还缓冲区
        /// </summary>
        /// <param name="buffer">要归还的缓冲区</param>
        /// <param name="clearArray">是否清除数组内容</param>
        void Return(byte[] buffer, bool clearArray = false);

        /// <summary>
        /// 租借可释放的缓冲区
        /// </summary>
        /// <param name="minimumSize">最小大小</param>
        /// <returns>可释放的缓冲区租借</returns>
        BufferRental RentDisposable(int minimumSize);

        /// <summary>
        /// 获取池中可用的缓冲区数量
        /// </summary>
        int AvailableCount { get; }

        /// <summary>
        /// 获取池的最大容量
        /// </summary>
        int MaxPoolSize { get; }

        /// <summary>
        /// 获取单个缓冲区的最大大小
        /// </summary>
        int MaxBufferSize { get; }

        /// <summary>
        /// 清空池中所有缓冲区
        /// </summary>
        void Clear();
    }

    /// <summary>
    /// 可释放的缓冲区租借
    /// <para>使用 using 语句自动归还缓冲区</para>
    /// </summary>
    public readonly struct BufferRental : IDisposable
    {
        private readonly IBufferPool _pool;
        private readonly byte[] _buffer;
        private readonly bool _clearOnReturn;

        /// <summary>
        /// 创建缓冲区租借
        /// </summary>
        /// <param name="pool">缓冲区池</param>
        /// <param name="buffer">租借的缓冲区</param>
        /// <param name="clearOnReturn">归还时是否清除</param>
        public BufferRental(IBufferPool pool, byte[] buffer, bool clearOnReturn = false)
        {
            _pool = pool;
            _buffer = buffer;
            _clearOnReturn = clearOnReturn;
        }

        /// <summary>获取缓冲区</summary>
        public byte[] Buffer => _buffer;

        /// <summary>获取缓冲区长度</summary>
        public int Length => _buffer?.Length ?? 0;

        /// <summary>获取缓冲区的 Span</summary>
        public Span<byte> Span => _buffer.AsSpan();

        /// <summary>获取缓冲区的 Memory</summary>
        public Memory<byte> Memory => _buffer.AsMemory();

        /// <summary>
        /// 释放缓冲区 (归还到池)
        /// </summary>
        public void Dispose()
        {
            if (_buffer != null && _pool != null)
            {
                _pool.Return(_buffer, _clearOnReturn);
            }
        }
    }

    /// <summary>
    /// 缓冲区池扩展方法
    /// </summary>
    public static class BufferPoolExtensions
    {
        /// <summary>
        /// 租借并清零的缓冲区
        /// </summary>
        /// <param name="pool">缓冲区池</param>
        /// <param name="minimumSize">最小大小</param>
        /// <returns>已清零的缓冲区</returns>
        public static byte[] RentCleared(this IBufferPool pool, int minimumSize)
        {
            var buffer = pool.Rent(minimumSize);
            Array.Clear(buffer, 0, buffer.Length);
            return buffer;
        }

        /// <summary>
        /// 租借指定大小的缓冲区并复制数据
        /// </summary>
        /// <param name="pool">缓冲区池</param>
        /// <param name="source">源数据</param>
        /// <returns>包含源数据的缓冲区</returns>
        public static byte[] RentAndCopy(this IBufferPool pool, ReadOnlySpan<byte> source)
        {
            var buffer = pool.Rent(source.Length);
            source.CopyTo(buffer);
            return buffer;
        }

        /// <summary>
        /// 安全归还缓冲区 (检查 null)
        /// </summary>
        /// <param name="pool">缓冲区池</param>
        /// <param name="buffer">要归还的缓冲区</param>
        /// <param name="clearArray">是否清除数组内容</param>
        public static void ReturnSafe(this IBufferPool pool, byte[] buffer, bool clearArray = false)
        {
            if (buffer != null)
            {
                pool.Return(buffer, clearArray);
            }
        }
    }

    /// <summary>
    /// 缓冲区写入器接口
    /// <para>提供可增长的缓冲区写入功能</para>
    /// </summary>
    public interface IBufferWriter : IBufferWriter<byte>, IDisposable
    {
        /// <summary>获取已写入的字节数</summary>
        int WrittenCount { get; }

        /// <summary>获取总容量</summary>
        int Capacity { get; }

        /// <summary>获取剩余可用空间</summary>
        int FreeCapacity { get; }

        /// <summary>获取已写入的数据</summary>
        ReadOnlySpan<byte> WrittenSpan { get; }

        /// <summary>获取已写入的数据 (Memory)</summary>
        ReadOnlyMemory<byte> WrittenMemory { get; }

        /// <summary>
        /// 将已写入的数据复制到数组
        /// </summary>
        /// <returns>包含已写入数据的新数组</returns>
        byte[] ToArray();

        /// <summary>
        /// 重置写入器 (清除已写入数据)
        /// </summary>
        void Reset();

        /// <summary>
        /// 确保有足够的容量
        /// </summary>
        /// <param name="size">需要的额外容量</param>
        void EnsureCapacity(int size);
    }
}
