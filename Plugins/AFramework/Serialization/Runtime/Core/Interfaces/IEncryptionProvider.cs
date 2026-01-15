// ==========================================================
// 文件名：IEncryptionProvider.cs
// 命名空间: AFramework.Serialization
// 依赖: System
// ==========================================================

using System;

namespace AFramework.Serialization
{
    /// <summary>
    /// 加密提供者接口
    /// <para>定义数据加密和解密的操作</para>
    /// <para>支持多种加密算法的统一抽象</para>
    /// </summary>
    /// <remarks>
    /// 加密提供者用于保护序列化数据的安全性。
    /// 
    /// 使用示例:
    /// <code>
    /// IEncryptionProvider encryptor = new AesEncryptionProvider(key);
    /// 
    /// // 加密数据
    /// byte[] encrypted = encryptor.Encrypt(plainData);
    /// 
    /// // 解密数据
    /// byte[] decrypted = encryptor.Decrypt(encrypted);
    /// 
    /// // 使用认证加密
    /// var result = encryptor.EncryptAuthenticated(data, associatedData);
    /// </code>
    /// </remarks>
    public interface IEncryptionProvider : IDisposable
    {
        /// <summary>
        /// 获取加密算法类型
        /// </summary>
        EncryptionAlgorithm Algorithm { get; }

        /// <summary>
        /// 获取密钥大小 (位)
        /// </summary>
        int KeySizeBits { get; }

        /// <summary>
        /// 获取块大小 (位)
        /// </summary>
        int BlockSizeBits { get; }

        /// <summary>
        /// 加密数据
        /// </summary>
        /// <param name="plaintext">明文数据</param>
        /// <returns>密文数据</returns>
        byte[] Encrypt(ReadOnlySpan<byte> plaintext);

        /// <summary>
        /// 加密数据到目标缓冲区
        /// </summary>
        /// <param name="plaintext">明文数据</param>
        /// <param name="ciphertext">密文缓冲区</param>
        /// <returns>写入的字节数</returns>
        int Encrypt(ReadOnlySpan<byte> plaintext, Span<byte> ciphertext);

        /// <summary>
        /// 尝试加密数据到目标缓冲区
        /// </summary>
        /// <param name="plaintext">明文数据</param>
        /// <param name="ciphertext">密文缓冲区</param>
        /// <param name="bytesWritten">写入的字节数</param>
        /// <returns>如果成功返回 true</returns>
        bool TryEncrypt(ReadOnlySpan<byte> plaintext, Span<byte> ciphertext, out int bytesWritten);

        /// <summary>
        /// 解密数据
        /// </summary>
        /// <param name="ciphertext">密文数据</param>
        /// <returns>明文数据</returns>
        byte[] Decrypt(ReadOnlySpan<byte> ciphertext);

        /// <summary>
        /// 解密数据到目标缓冲区
        /// </summary>
        /// <param name="ciphertext">密文数据</param>
        /// <param name="plaintext">明文缓冲区</param>
        /// <returns>写入的字节数</returns>
        int Decrypt(ReadOnlySpan<byte> ciphertext, Span<byte> plaintext);

        /// <summary>
        /// 尝试解密数据到目标缓冲区
        /// </summary>
        /// <param name="ciphertext">密文数据</param>
        /// <param name="plaintext">明文缓冲区</param>
        /// <param name="bytesWritten">写入的字节数</param>
        /// <returns>如果成功返回 true</returns>
        bool TryDecrypt(ReadOnlySpan<byte> ciphertext, Span<byte> plaintext, out int bytesWritten);

        /// <summary>
        /// 获取加密后的大小
        /// </summary>
        /// <param name="plaintextSize">明文大小</param>
        /// <returns>加密后的大小</returns>
        int GetEncryptedSize(int plaintextSize);

        /// <summary>
        /// 获取解密后的最大大小
        /// </summary>
        /// <param name="ciphertextSize">密文大小</param>
        /// <returns>解密后的最大大小</returns>
        int GetDecryptedMaxSize(int ciphertextSize);
    }

    /// <summary>
    /// 认证加密提供者接口
    /// <para>支持带认证标签的加密 (AEAD)</para>
    /// </summary>
    public interface IAuthenticatedEncryptionProvider : IEncryptionProvider
    {
        /// <summary>
        /// 获取认证标签大小 (字节)
        /// </summary>
        int TagSizeBytes { get; }

        /// <summary>
        /// 获取 Nonce 大小 (字节)
        /// </summary>
        int NonceSizeBytes { get; }

        /// <summary>
        /// 加密数据并生成认证标签
        /// </summary>
        /// <param name="plaintext">明文数据</param>
        /// <param name="associatedData">关联数据 (不加密但参与认证)</param>
        /// <returns>加密结果 (包含密文、Nonce 和标签)</returns>
        EncryptionResult EncryptAuthenticated(ReadOnlySpan<byte> plaintext, ReadOnlySpan<byte> associatedData = default);

        /// <summary>
        /// 解密数据并验证认证标签
        /// </summary>
        /// <param name="ciphertext">密文数据</param>
        /// <param name="nonce">Nonce</param>
        /// <param name="tag">认证标签</param>
        /// <param name="associatedData">关联数据</param>
        /// <returns>明文数据</returns>
        /// <exception cref="System.Security.Cryptography.CryptographicException">认证失败时抛出</exception>
        byte[] DecryptAuthenticated(ReadOnlySpan<byte> ciphertext, ReadOnlySpan<byte> nonce, ReadOnlySpan<byte> tag, ReadOnlySpan<byte> associatedData = default);

        /// <summary>
        /// 尝试解密数据并验证认证标签
        /// </summary>
        /// <param name="ciphertext">密文数据</param>
        /// <param name="nonce">Nonce</param>
        /// <param name="tag">认证标签</param>
        /// <param name="plaintext">明文缓冲区</param>
        /// <param name="bytesWritten">写入的字节数</param>
        /// <param name="associatedData">关联数据</param>
        /// <returns>如果成功返回 true</returns>
        bool TryDecryptAuthenticated(ReadOnlySpan<byte> ciphertext, ReadOnlySpan<byte> nonce, ReadOnlySpan<byte> tag, Span<byte> plaintext, out int bytesWritten, ReadOnlySpan<byte> associatedData = default);
    }

    /// <summary>
    /// 加密结果结构体
    /// </summary>
    public readonly struct EncryptionResult
    {
        /// <summary>密文数据</summary>
        public readonly byte[] Ciphertext;

        /// <summary>Nonce (初始化向量)</summary>
        public readonly byte[] Nonce;

        /// <summary>认证标签</summary>
        public readonly byte[] Tag;

        /// <summary>是否成功</summary>
        public bool IsSuccess => Ciphertext != null;

        /// <summary>
        /// 创建加密结果
        /// </summary>
        public EncryptionResult(byte[] ciphertext, byte[] nonce, byte[] tag)
        {
            Ciphertext = ciphertext;
            Nonce = nonce;
            Tag = tag;
        }

        /// <summary>
        /// 创建成功的加密结果
        /// </summary>
        public static EncryptionResult Success(byte[] ciphertext, byte[] nonce, byte[] tag)
        {
            return new EncryptionResult(ciphertext, nonce, tag);
        }

        /// <summary>
        /// 创建失败的加密结果
        /// </summary>
        public static EncryptionResult Failure => new EncryptionResult(null, null, null);

        /// <summary>
        /// 获取打包的数据 (Nonce + Ciphertext + Tag)
        /// </summary>
        /// <returns>打包的数据</returns>
        public byte[] ToPackedData()
        {
            if (!IsSuccess)
                return null;

            var result = new byte[Nonce.Length + Ciphertext.Length + Tag.Length];
            Buffer.BlockCopy(Nonce, 0, result, 0, Nonce.Length);
            Buffer.BlockCopy(Ciphertext, 0, result, Nonce.Length, Ciphertext.Length);
            Buffer.BlockCopy(Tag, 0, result, Nonce.Length + Ciphertext.Length, Tag.Length);
            return result;
        }

        /// <summary>
        /// 从打包的数据解析
        /// </summary>
        /// <param name="packedData">打包的数据</param>
        /// <param name="nonceSize">Nonce 大小</param>
        /// <param name="tagSize">标签大小</param>
        /// <returns>加密结果</returns>
        public static EncryptionResult FromPackedData(ReadOnlySpan<byte> packedData, int nonceSize, int tagSize)
        {
            if (packedData.Length < nonceSize + tagSize)
                return Failure;

            var nonce = packedData.Slice(0, nonceSize).ToArray();
            var ciphertext = packedData.Slice(nonceSize, packedData.Length - nonceSize - tagSize).ToArray();
            var tag = packedData.Slice(packedData.Length - tagSize, tagSize).ToArray();

            return new EncryptionResult(ciphertext, nonce, tag);
        }
    }
}
