// ==========================================================
// 文件名：CompressionLevel.cs
// 命名空间: AFramework.Serialization
// 依赖: System
// ==========================================================

using System;
using System.Runtime.CompilerServices;

namespace AFramework.Serialization
{
    /// <summary>
    /// 压缩级别枚举
    /// <para>定义压缩算法的压缩强度</para>
    /// </summary>
    /// <remarks>
    /// 使用示例:
    /// <code>
    /// // 使用最快压缩
    /// var options = new SerializeOptions 
    /// { 
    ///     Compression = CompressionAlgorithm.LZ4,
    ///     CompressionLevel = CompressionLevel.Fastest 
    /// };
    /// 
    /// // 获取数值级别
    /// int level = CompressionLevel.Optimal.GetNumericLevel(); // 返回 6
    /// </code>
    /// </remarks>
    public enum CompressionLevel : byte
    {
        /// <summary>
        /// 最快速度
        /// <para>最低压缩率，最快压缩速度</para>
        /// <para>适用：实时场景、CPU敏感、临时数据</para>
        /// </summary>
        Fastest = 0,

        /// <summary>
        /// 快速
        /// <para>较低压缩率，快速压缩</para>
        /// <para>适用：网络通信、游戏数据</para>
        /// </summary>
        Fast = 1,

        /// <summary>
        /// 平衡 (默认)
        /// <para>平衡压缩率和速度</para>
        /// <para>适用：通用场景</para>
        /// </summary>
        Optimal = 2,

        /// <summary>
        /// 高压缩
        /// <para>较高压缩率，较慢速度</para>
        /// <para>适用：存储优化、带宽受限</para>
        /// </summary>
        High = 3,

        /// <summary>
        /// 最小体积
        /// <para>最高压缩率，最慢速度</para>
        /// <para>适用：归档、分发、长期存储</para>
        /// </summary>
        SmallestSize = 4,

        /// <summary>
        /// 无压缩
        /// <para>仅存储，不压缩</para>
        /// </summary>
        NoCompression = 5
    }

    /// <summary>
    /// CompressionLevel 扩展方法
    /// </summary>
    public static class CompressionLevelExtensions
    {
        #region 数值转换方法

        /// <summary>
        /// 获取数值级别 (0-9 或 0-11，取决于算法)
        /// </summary>
        /// <param name="level">压缩级别</param>
        /// <param name="maxLevel">算法支持的最大级别</param>
        /// <returns>数值级别</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetNumericLevel(this CompressionLevel level, int maxLevel = 9)
        {
            return level switch
            {
                CompressionLevel.NoCompression => 0,
                CompressionLevel.Fastest => 1,
                CompressionLevel.Fast => Math.Max(1, maxLevel / 4),
                CompressionLevel.Optimal => Math.Max(1, maxLevel / 2),
                CompressionLevel.High => Math.Max(1, maxLevel * 3 / 4),
                CompressionLevel.SmallestSize => maxLevel,
                _ => maxLevel / 2
            };
        }

        /// <summary>
        /// 获取 LZ4 压缩级别
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetLZ4Level(this CompressionLevel level)
        {
            return level switch
            {
                CompressionLevel.NoCompression => 0,
                CompressionLevel.Fastest => 1,
                CompressionLevel.Fast => 3,
                CompressionLevel.Optimal => 6,
                CompressionLevel.High => 9,
                CompressionLevel.SmallestSize => 12,
                _ => 6
            };
        }

        /// <summary>
        /// 获取 Brotli 压缩级别 (0-11)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetBrotliLevel(this CompressionLevel level)
        {
            return level switch
            {
                CompressionLevel.NoCompression => 0,
                CompressionLevel.Fastest => 1,
                CompressionLevel.Fast => 4,
                CompressionLevel.Optimal => 6,
                CompressionLevel.High => 9,
                CompressionLevel.SmallestSize => 11,
                _ => 6
            };
        }

        /// <summary>
        /// 获取 Zstd 压缩级别 (1-22)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetZstdLevel(this CompressionLevel level)
        {
            return level switch
            {
                CompressionLevel.NoCompression => 0,
                CompressionLevel.Fastest => 1,
                CompressionLevel.Fast => 3,
                CompressionLevel.Optimal => 6,
                CompressionLevel.High => 12,
                CompressionLevel.SmallestSize => 19,
                _ => 6
            };
        }

        /// <summary>
        /// 获取 .NET System.IO.Compression.CompressionLevel
        /// </summary>
        public static System.IO.Compression.CompressionLevel ToSystemLevel(this CompressionLevel level)
        {
            return level switch
            {
                CompressionLevel.NoCompression => System.IO.Compression.CompressionLevel.NoCompression,
                CompressionLevel.Fastest => System.IO.Compression.CompressionLevel.Fastest,
                CompressionLevel.Fast => System.IO.Compression.CompressionLevel.Fastest,
                CompressionLevel.Optimal => System.IO.Compression.CompressionLevel.Optimal,
                CompressionLevel.High => System.IO.Compression.CompressionLevel.Optimal,
                CompressionLevel.SmallestSize => System.IO.Compression.CompressionLevel.Optimal,
                _ => System.IO.Compression.CompressionLevel.Optimal
            };
        }

        #endregion

        #region 信息获取方法

        /// <summary>
        /// 获取级别的中文描述
        /// </summary>
        public static string GetDescription(this CompressionLevel level)
        {
            return level switch
            {
                CompressionLevel.Fastest => "最快速度",
                CompressionLevel.Fast => "快速",
                CompressionLevel.Optimal => "平衡",
                CompressionLevel.High => "高压缩",
                CompressionLevel.SmallestSize => "最小体积",
                CompressionLevel.NoCompression => "无压缩",
                _ => "未知级别"
            };
        }

        /// <summary>
        /// 检查是否启用压缩
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsCompressionEnabled(this CompressionLevel level)
        {
            return level != CompressionLevel.NoCompression;
        }

        /// <summary>
        /// 检查是否为速度优先
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsSpeedPriority(this CompressionLevel level)
        {
            return level == CompressionLevel.Fastest || level == CompressionLevel.Fast;
        }

        /// <summary>
        /// 检查是否为大小优先
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsSizePriority(this CompressionLevel level)
        {
            return level == CompressionLevel.High || level == CompressionLevel.SmallestSize;
        }

        #endregion
    }
}
