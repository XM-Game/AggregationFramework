// ==========================================================
// 文件名：EncryptionAlgorithm.cs
// 命名空间: AFramework.Serialization
// 依赖: System
// ==========================================================

using System;
using System.Runtime.CompilerServices;

namespace AFramework.Serialization
{
    /// <summary>
    /// 加密算法枚举
    /// <para>定义序列化数据的加密算法类型</para>
    /// </summary>
    /// <remarks>
    /// 使用示例:
    /// <code>
    /// // 使用 AES 加密
    /// var options = new SerializeOptions 
    /// { 
    ///     Encryption = EncryptionAlgorithm.AES256,
    ///     EncryptionKey = key 
    /// };
    /// 
    /// // 检查安全级别
    /// if (algorithm.IsSecure())
    ///     // 适合敏感数据
    /// </code>
    /// </remarks>
    public enum EncryptionAlgorithm : byte
    {
        /// <summary>
        /// 无加密
        /// <para>不对数据进行加密处理</para>
        /// </summary>
        None = 0,

        /// <summary>
        /// XOR 加密
        /// <para>简单异或加密，仅用于混淆</para>
        /// <para>特点：极快、不安全、仅防止直接查看</para>
        /// <para>适用：非敏感数据混淆、防止简单篡改</para>
        /// </summary>
        XOR = 1,

        /// <summary>
        /// AES-128 加密
        /// <para>128位密钥的 AES 加密</para>
        /// <para>特点：安全、快速、硬件加速支持</para>
        /// <para>适用：一般敏感数据</para>
        /// </summary>
        AES128 = 2,

        /// <summary>
        /// AES-256 加密
        /// <para>256位密钥的 AES 加密</para>
        /// <para>特点：高安全性、较快、硬件加速支持</para>
        /// <para>适用：高敏感数据、金融数据</para>
        /// </summary>
        AES256 = 3,

        /// <summary>
        /// ChaCha20 加密
        /// <para>现代流密码算法</para>
        /// <para>特点：高安全性、软件实现快、移动友好</para>
        /// <para>适用：移动设备、无硬件AES加速场景</para>
        /// </summary>
        ChaCha20 = 4,

        /// <summary>
        /// ChaCha20-Poly1305 加密
        /// <para>带认证的 ChaCha20 加密</para>
        /// <para>特点：加密+认证、防篡改</para>
        /// <para>适用：需要完整性验证的场景</para>
        /// </summary>
        ChaCha20Poly1305 = 5,

        /// <summary>
        /// AES-GCM 加密
        /// <para>带认证的 AES 加密</para>
        /// <para>特点：加密+认证、硬件加速、防篡改</para>
        /// <para>适用：需要完整性验证的高安全场景</para>
        /// </summary>
        AesGcm = 6
    }

    /// <summary>
    /// EncryptionAlgorithm 扩展方法
    /// </summary>
    public static class EncryptionAlgorithmExtensions
    {
        #region 特性检查方法

        /// <summary>
        /// 检查是否已启用加密
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsEnabled(this EncryptionAlgorithm algorithm)
        {
            return algorithm != EncryptionAlgorithm.None;
        }

        /// <summary>
        /// 检查是否为安全加密算法
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsSecure(this EncryptionAlgorithm algorithm)
        {
            return algorithm >= EncryptionAlgorithm.AES128;
        }

        /// <summary>
        /// 检查是否支持认证 (AEAD)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SupportsAuthentication(this EncryptionAlgorithm algorithm)
        {
            return algorithm == EncryptionAlgorithm.ChaCha20Poly1305 ||
                   algorithm == EncryptionAlgorithm.AesGcm;
        }

        /// <summary>
        /// 检查是否支持硬件加速
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SupportsHardwareAcceleration(this EncryptionAlgorithm algorithm)
        {
            return algorithm == EncryptionAlgorithm.AES128 ||
                   algorithm == EncryptionAlgorithm.AES256 ||
                   algorithm == EncryptionAlgorithm.AesGcm;
        }

        /// <summary>
        /// 检查是否为流密码
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsStreamCipher(this EncryptionAlgorithm algorithm)
        {
            return algorithm == EncryptionAlgorithm.XOR ||
                   algorithm == EncryptionAlgorithm.ChaCha20 ||
                   algorithm == EncryptionAlgorithm.ChaCha20Poly1305;
        }

        #endregion

        #region 密钥信息方法

        /// <summary>
        /// 获取所需的密钥长度 (字节)
        /// </summary>
        public static int GetKeySize(this EncryptionAlgorithm algorithm)
        {
            return algorithm switch
            {
                EncryptionAlgorithm.None => 0,
                EncryptionAlgorithm.XOR => 0, // 可变长度
                EncryptionAlgorithm.AES128 => 16,
                EncryptionAlgorithm.AES256 => 32,
                EncryptionAlgorithm.ChaCha20 => 32,
                EncryptionAlgorithm.ChaCha20Poly1305 => 32,
                EncryptionAlgorithm.AesGcm => 32,
                _ => 0
            };
        }

        /// <summary>
        /// 获取所需的 IV/Nonce 长度 (字节)
        /// </summary>
        public static int GetIVSize(this EncryptionAlgorithm algorithm)
        {
            return algorithm switch
            {
                EncryptionAlgorithm.None => 0,
                EncryptionAlgorithm.XOR => 0,
                EncryptionAlgorithm.AES128 => 16,
                EncryptionAlgorithm.AES256 => 16,
                EncryptionAlgorithm.ChaCha20 => 12,
                EncryptionAlgorithm.ChaCha20Poly1305 => 12,
                EncryptionAlgorithm.AesGcm => 12,
                _ => 0
            };
        }

        /// <summary>
        /// 获取认证标签长度 (字节)
        /// </summary>
        public static int GetTagSize(this EncryptionAlgorithm algorithm)
        {
            return algorithm switch
            {
                EncryptionAlgorithm.ChaCha20Poly1305 => 16,
                EncryptionAlgorithm.AesGcm => 16,
                _ => 0
            };
        }

        /// <summary>
        /// 获取块大小 (字节)
        /// </summary>
        public static int GetBlockSize(this EncryptionAlgorithm algorithm)
        {
            return algorithm switch
            {
                EncryptionAlgorithm.AES128 => 16,
                EncryptionAlgorithm.AES256 => 16,
                EncryptionAlgorithm.AesGcm => 16,
                _ => 1 // 流密码无块大小限制
            };
        }

        #endregion

        #region 信息获取方法

        /// <summary>
        /// 获取算法的中文描述
        /// </summary>
        public static string GetDescription(this EncryptionAlgorithm algorithm)
        {
            return algorithm switch
            {
                EncryptionAlgorithm.None => "无加密",
                EncryptionAlgorithm.XOR => "XOR 混淆",
                EncryptionAlgorithm.AES128 => "AES-128",
                EncryptionAlgorithm.AES256 => "AES-256",
                EncryptionAlgorithm.ChaCha20 => "ChaCha20",
                EncryptionAlgorithm.ChaCha20Poly1305 => "ChaCha20-Poly1305 (认证)",
                EncryptionAlgorithm.AesGcm => "AES-GCM (认证)",
                _ => "未知算法"
            };
        }

        /// <summary>
        /// 获取安全等级 (0-5，5为最高)
        /// </summary>
        public static int GetSecurityLevel(this EncryptionAlgorithm algorithm)
        {
            return algorithm switch
            {
                EncryptionAlgorithm.None => 0,
                EncryptionAlgorithm.XOR => 1,
                EncryptionAlgorithm.AES128 => 4,
                EncryptionAlgorithm.AES256 => 5,
                EncryptionAlgorithm.ChaCha20 => 5,
                EncryptionAlgorithm.ChaCha20Poly1305 => 5,
                EncryptionAlgorithm.AesGcm => 5,
                _ => 0
            };
        }

        /// <summary>
        /// 获取推荐的加密算法
        /// </summary>
        public static EncryptionAlgorithm GetRecommended(bool needsAuthentication, bool preferHardwareAcceleration)
        {
            if (needsAuthentication)
            {
                return preferHardwareAcceleration ? EncryptionAlgorithm.AesGcm : EncryptionAlgorithm.ChaCha20Poly1305;
            }
            return preferHardwareAcceleration ? EncryptionAlgorithm.AES256 : EncryptionAlgorithm.ChaCha20;
        }

        #endregion
    }
}
