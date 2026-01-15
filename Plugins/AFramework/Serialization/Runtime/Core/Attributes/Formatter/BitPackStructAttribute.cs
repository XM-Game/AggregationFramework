// ==========================================================
// 文件名：BitPackStructAttribute.cs
// 命名空间: AFramework.Serialization
// 依赖: System
// ==========================================================

using System;

namespace AFramework.Serialization
{
    /// <summary>
    /// 位打包结构体特性
    /// <para>标记整个结构体使用位打包序列化</para>
    /// </summary>
    /// <remarks>
    /// <para><b>功能说明：</b></para>
    /// <list type="bullet">
    ///   <item>将结构体的所有字段紧凑打包到位流中</item>
    ///   <item>支持字节对齐配置</item>
    ///   <item>支持固定总位数配置</item>
    ///   <item>适用于网络协议和紧凑存储</item>
    /// </list>
    /// 
    /// <para><b>使用示例：</b></para>
    /// <code>
    /// // 位打包结构体
    /// [Serializable]
    /// [BitPackStruct]
    /// public struct CompactTransform
    /// {
    ///     [BitPack(16, MinValue = -1000f, MaxValue = 1000f)]
    ///     public float X;
    ///     
    ///     [BitPack(16, MinValue = -1000f, MaxValue = 1000f)]
    ///     public float Y;
    ///     
    ///     [BitPack(16, MinValue = -1000f, MaxValue = 1000f)]
    ///     public float Z;
    ///     
    ///     [BitPack(10, MinValue = 0f, MaxValue = 360f)]
    ///     public float Rotation;
    /// }
    /// 
    /// // 带长度前缀的位打包结构体
    /// [BitPackStruct(IncludeLengthPrefix = true)]
    /// public struct NetworkPacket
    /// {
    ///     [BitPack(8)]
    ///     public byte Type;
    ///     
    ///     [BitPack(16)]
    ///     public ushort Id;
    /// }
    /// </code>
    /// 
    /// <para><b>注意事项：</b></para>
    /// <list type="number">
    ///   <item>结构体内的字段应使用 BitPack 特性标记</item>
    ///   <item>未标记的字段将使用默认位数</item>
    ///   <item>字节对齐会在末尾填充零位</item>
    /// </list>
    /// </remarks>
    [AttributeUsage(
        AttributeTargets.Struct,
        AllowMultiple = false,
        Inherited = false)]
    public sealed class BitPackStructAttribute : Attribute
    {
        #region 字段

        private bool _byteAligned = true;
        private int _totalBits;
        private bool _includeLengthPrefix;
        private bool _validateOnSerialize = true;

        #endregion

        #region 属性

        /// <summary>
        /// 获取或设置是否按字节对齐
        /// <para>默认值：true</para>
        /// </summary>
        /// <remarks>
        /// 启用后，总位数会向上取整到 8 的倍数。
        /// </remarks>
        public bool ByteAligned
        {
            get => _byteAligned;
            set => _byteAligned = value;
        }

        /// <summary>
        /// 获取或设置总位数
        /// <para>默认值：0（自动计算）</para>
        /// </summary>
        /// <remarks>
        /// 设置为 0 时，总位数由所有字段的位数之和决定。
        /// 设置具体值时，如果字段位数不足会填充零位。
        /// </remarks>
        public int TotalBits
        {
            get => _totalBits;
            set => _totalBits = value;
        }

        /// <summary>
        /// 获取或设置是否包含长度前缀
        /// <para>默认值：false</para>
        /// </summary>
        /// <remarks>
        /// 启用后，序列化数据前会添加位数/字节数信息。
        /// </remarks>
        public bool IncludeLengthPrefix
        {
            get => _includeLengthPrefix;
            set => _includeLengthPrefix = value;
        }

        /// <summary>
        /// 获取或设置是否在序列化时验证
        /// <para>默认值：true</para>
        /// </summary>
        /// <remarks>
        /// 启用后，会验证所有字段值是否在有效范围内。
        /// </remarks>
        public bool ValidateOnSerialize
        {
            get => _validateOnSerialize;
            set => _validateOnSerialize = value;
        }

        #endregion

        #region 构造函数

        /// <summary>
        /// 初始化 <see cref="BitPackStructAttribute"/> 的新实例
        /// </summary>
        public BitPackStructAttribute()
        {
        }

        /// <summary>
        /// 初始化 <see cref="BitPackStructAttribute"/> 的新实例
        /// </summary>
        /// <param name="totalBits">总位数</param>
        public BitPackStructAttribute(int totalBits)
        {
            _totalBits = totalBits;
        }

        #endregion

        #region 公共方法

        /// <summary>
        /// 计算对齐后的字节数
        /// </summary>
        /// <param name="actualBits">实际位数</param>
        /// <returns>字节数</returns>
        public int CalculateByteCount(int actualBits)
        {
            var bits = _totalBits > 0 ? _totalBits : actualBits;
            return _byteAligned ? (bits + 7) / 8 : (bits + 7) / 8;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            if (_totalBits > 0)
                return $"[BitPackStruct(TotalBits={_totalBits})]";
            return "[BitPackStruct]";
        }

        #endregion
    }
}
