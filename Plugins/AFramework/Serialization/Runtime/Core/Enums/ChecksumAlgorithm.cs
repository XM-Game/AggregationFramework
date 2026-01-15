// ==========================================================
// 文件名：ChecksumAlgorithm.cs
// 命名空间: AFramework.Serialization
// 依赖: System
// ==========================================================

using System;
using System.Runtime.CompilerServices;

namespace AFramework.Serialization
{
    /// <summary>
    /// 校验算法枚举
    /// <para>定义序列化数据的完整性校验算法</para>
    /// </summary>
    /// <remarks>
    /// 使用示例:
    /// <code>
    /// // 使用 CRC32 校验
    /// var options = new SerializeOptions { Checksum = ChecksumAlgorithm.CRC32 };
    /// 
    /// // 获取校验值大小
    /// int size = ChecksumAlgorithm.XXHash64.GetChecksumSize(); // 返回 8
    /// </code>
    /// </remarks>
    public enum ChecksumAlgorithm : byte
    {
        /// <summary>
        /// 无校验
        /// <para>不进行完整性校验</para>
        /// </summary>
        None = 0,

        /// <summary>
        /// CRC32 校验
        /// <para>32位循环冗余校验</para>
        /// <para>特点：快速、4字节、广泛使用</para>
        /// <para>适用：一般数据完整性检查</para>
        /// </summary>
        CRC32 = 1,

        /// <summary>
        /// CRC64 校验
        /// <para>64位循环冗余校验</para>
        /// <para>特点：较快、8字节、更低碰撞率</para>
        /// <para>适用：大数据完整性检查</para>
        /// </summary>
        CRC64 = 2,

        /// <summary>
        /// XXHash32 校验
        /// <para>32位极速哈希</para>
        /// <para>特点：极快、4字节、非加密</para>
        /// <para>适用：高性能场景</para>
        /// </summary>
        XXHash32 = 3,

        /// <summary>
        /// XXHash64 校验
        /// <para>64位极速哈希</para>
        /// <para>特点：极快、8字节、非加密</para>
        /// <para>适用：高性能大数据场景</para>
        /// </summary>
        XXHash64 = 4,

        /// <summary>
        /// XXHash128 校验
        /// <para>128位极速哈希</para>
        /// <para>特点：极快、16字节、极低碰撞</para>
        /// <para>适用：需要极低碰撞率的场景</para>
        /// </summary>
        XXHash128 = 5,

        /// <summary>
        /// MD5 校验
        /// <para>128位消息摘要</para>
        /// <para>特点：中速、16字节、已不推荐用于安全</para>
        /// <para>适用：兼容性需求、非安全校验</para>
        /// </summary>
        MD5 = 6,

        /// <summary>
        /// SHA256 校验
        /// <para>256位安全哈希</para>
        /// <para>特点：安全、32字节、较慢</para>
        /// <para>适用：安全敏感场景</para>
        /// </summary>
        SHA256 = 7,

        /// <summary>
        /// SHA512 校验
        /// <para>512位安全哈希</para>
        /// <para>特点：高安全、64字节、较慢</para>
        /// <para>适用：最高安全要求</para>
        /// </summary>
        SHA512 = 8,

        /// <summary>
        /// Adler32 校验
        /// <para>32位校验和</para>
        /// <para>特点：极快、4字节、简单</para>
        /// <para>适用：快速校验、zlib兼容</para>
        /// </summary>
        Adler32 = 9
    }

    /// <summary>
    /// ChecksumAlgorithm 扩展方法
    /// </summary>
    public static class ChecksumAlgorithmExtensions
    {
        #region 特性检查方法

        /// <summary>
        /// 检查是否已启用校验
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsEnabled(this ChecksumAlgorithm algorithm)
        {
            return algorithm != ChecksumAlgorithm.None;
        }

        /// <summary>
        /// 检查是否为加密安全哈希
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsCryptographic(this ChecksumAlgorithm algorithm)
        {
            return algorithm == ChecksumAlgorithm.SHA256 ||
                   algorithm == ChecksumAlgorithm.SHA512;
        }

        /// <summary>
        /// 检查是否为高速哈希
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsHighSpeed(this ChecksumAlgorithm algorithm)
        {
            return algorithm == ChecksumAlgorithm.XXHash32 ||
                   algorithm == ChecksumAlgorithm.XXHash64 ||
                   algorithm == ChecksumAlgorithm.XXHash128 ||
                   algorithm == ChecksumAlgorithm.CRC32 ||
                   algorithm == ChecksumAlgorithm.Adler32;
        }

        #endregion

        #region 大小信息方法

        /// <summary>
        /// 获取校验值大小 (字节)
        /// </summary>
        public static int GetChecksumSize(this ChecksumAlgorithm algorithm)
        {
            return algorithm switch
            {
                ChecksumAlgorithm.None => 0,
                ChecksumAlgorithm.CRC32 => 4,
                ChecksumAlgorithm.CRC64 => 8,
                ChecksumAlgorithm.XXHash32 => 4,
                ChecksumAlgorithm.XXHash64 => 8,
                ChecksumAlgorithm.XXHash128 => 16,
                ChecksumAlgorithm.MD5 => 16,
                ChecksumAlgorithm.SHA256 => 32,
                ChecksumAlgorithm.SHA512 => 64,
                ChecksumAlgorithm.Adler32 => 4,
                _ => 0
            };
        }

        /// <summary>
        /// 获取校验值大小 (位)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetChecksumBits(this ChecksumAlgorithm algorithm)
        {
            return algorithm.GetChecksumSize() * 8;
        }

        #endregion

        #region 信息获取方法

        /// <summary>
        /// 获取算法的中文描述
        /// </summary>
        public static string GetDescription(this ChecksumAlgorithm algorithm)
        {
            return algorithm switch
            {
                ChecksumAlgorithm.None => "无校验",
                ChecksumAlgorithm.CRC32 => "CRC32",
                ChecksumAlgorithm.CRC64 => "CRC64",
                ChecksumAlgorithm.XXHash32 => "XXHash32 (极速)",
                ChecksumAlgorithm.XXHash64 => "XXHash64 (极速)",
                ChecksumAlgorithm.XXHash128 => "XXHash128 (极速)",
                ChecksumAlgorithm.MD5 => "MD5",
                ChecksumAlgorithm.SHA256 => "SHA-256 (安全)",
                ChecksumAlgorithm.SHA512 => "SHA-512 (安全)",
                ChecksumAlgorithm.Adler32 => "Adler32",
                _ => "未知算法"
            };
        }

        /// <summary>
        /// 获取速度等级 (1-5，5为最快)
        /// </summary>
        public static int GetSpeedLevel(this ChecksumAlgorithm algorithm)
        {
            return algorithm switch
            {
                ChecksumAlgorithm.None => 5,
                ChecksumAlgorithm.XXHash32 => 5,
                ChecksumAlgorithm.XXHash64 => 5,
                ChecksumAlgorithm.XXHash128 => 5,
                ChecksumAlgorithm.Adler32 => 5,
                ChecksumAlgorithm.CRC32 => 4,
                ChecksumAlgorithm.CRC64 => 4,
                ChecksumAlgorithm.MD5 => 3,
                ChecksumAlgorithm.SHA256 => 2,
                ChecksumAlgorithm.SHA512 => 1,
                _ => 3
            };
        }

        /// <summary>
        /// 获取推荐的校验算法
        /// </summary>
        public static ChecksumAlgorithm GetRecommended(bool needsSecurity, bool prioritizeSpeed)
        {
            if (needsSecurity) return ChecksumAlgorithm.SHA256;
            return prioritizeSpeed ? ChecksumAlgorithm.XXHash64 : ChecksumAlgorithm.CRC32;
        }

        #endregion
    }
}
