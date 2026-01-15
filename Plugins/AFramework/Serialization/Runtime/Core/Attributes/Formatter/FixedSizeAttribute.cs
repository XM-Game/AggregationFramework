// ==========================================================
// 文件名：FixedSizeAttribute.cs
// 命名空间: AFramework.Serialization
// 依赖: System
// ==========================================================

using System;
using System.Runtime.CompilerServices;

namespace AFramework.Serialization
{
    /// <summary>
    /// 固定大小特性
    /// <para>指定字段或类型使用固定大小进行序列化</para>
    /// </summary>
    /// <remarks>
    /// <para><b>功能说明：</b></para>
    /// <list type="bullet">
    ///   <item>为可变长度类型指定固定大小</item>
    ///   <item>支持字符串、数组、集合等类型</item>
    ///   <item>支持填充和截断策略</item>
    ///   <item>适用于二进制协议和固定格式文件</item>
    /// </list>
    /// 
    /// <para><b>使用示例：</b></para>
    /// <code>
    /// [Serializable]
    /// public class BinaryRecord
    /// {
    ///     // 固定 32 字节的字符串
    ///     [FixedSize(32)]
    ///     public string Name;
    ///     
    ///     // 固定 16 字节，使用空格填充
    ///     [FixedSize(16, PaddingByte = 0x20)]
    ///     public string Code;
    ///     
    ///     // 固定 100 个元素的数组
    ///     [FixedSize(100)]
    ///     public int[] Scores;
    /// }
    /// </code>
    /// </remarks>
    [AttributeUsage(
        AttributeTargets.Field | AttributeTargets.Property |
        AttributeTargets.Class | AttributeTargets.Struct,
        AllowMultiple = false,
        Inherited = true)]
    public sealed class FixedSizeAttribute : Attribute
    {
        #region 常量

        /// <summary>默认填充字节</summary>
        public const byte DefaultPaddingByte = 0x00;

        /// <summary>空格填充字节</summary>
        public const byte SpacePaddingByte = 0x20;

        /// <summary>最大固定大小（16MB）</summary>
        public const int MaxFixedSize = 16 * 1024 * 1024;

        #endregion

        #region 字段

        private readonly int _size;
        private SizeUnit _sizeUnit = SizeUnit.Elements;
        private byte _paddingByte = DefaultPaddingByte;
        private PaddingPosition _paddingPosition = PaddingPosition.End;
        private bool _truncateIfExceeds = true;
        private bool _trimPadding = true;
        private StringEncoding _stringEncoding = StringEncoding.UTF8;
        private bool _nullTerminated;
        private int _alignment = 1;
        private bool _validateSize = true;
        private object _defaultValue;

        #endregion

        #region 属性

        /// <summary>获取固定大小</summary>
        public int Size => _size;

        /// <summary>
        /// 获取或设置大小单位
        /// <para>默认值：<see cref="SizeUnit.Elements"/></para>
        /// </summary>
        public SizeUnit SizeUnit
        {
            get => _sizeUnit;
            set => _sizeUnit = value;
        }

        /// <summary>
        /// 获取或设置填充字节
        /// <para>默认值：0x00</para>
        /// </summary>
        public byte PaddingByte
        {
            get => _paddingByte;
            set => _paddingByte = value;
        }

        /// <summary>
        /// 获取或设置填充位置
        /// <para>默认值：<see cref="PaddingPosition.End"/></para>
        /// </summary>
        public PaddingPosition PaddingPosition
        {
            get => _paddingPosition;
            set => _paddingPosition = value;
        }

        /// <summary>
        /// 获取或设置超出大小时是否截断
        /// <para>默认值：true</para>
        /// </summary>
        public bool TruncateIfExceeds
        {
            get => _truncateIfExceeds;
            set => _truncateIfExceeds = value;
        }

        /// <summary>
        /// 获取或设置是否移除尾部填充
        /// <para>默认值：true</para>
        /// </summary>
        public bool TrimPadding
        {
            get => _trimPadding;
            set => _trimPadding = value;
        }

        /// <summary>
        /// 获取或设置字符串编码
        /// <para>默认值：<see cref="StringEncoding.UTF8"/></para>
        /// </summary>
        public StringEncoding StringEncoding
        {
            get => _stringEncoding;
            set => _stringEncoding = value;
        }

        /// <summary>
        /// 获取或设置是否包含 null 终止符
        /// <para>默认值：false</para>
        /// </summary>
        public bool NullTerminated
        {
            get => _nullTerminated;
            set => _nullTerminated = value;
        }

        /// <summary>
        /// 获取或设置对齐边界
        /// <para>默认值：1（无对齐）</para>
        /// </summary>
        public int Alignment
        {
            get => _alignment;
            set
            {
                if (value <= 0 || (value & (value - 1)) != 0)
                    throw new ArgumentException("对齐边界必须是 2 的幂", nameof(value));
                _alignment = value;
            }
        }

        /// <summary>
        /// 获取或设置是否验证大小
        /// <para>默认值：true</para>
        /// </summary>
        public bool ValidateSize
        {
            get => _validateSize;
            set => _validateSize = value;
        }

        /// <summary>
        /// 获取或设置默认值（用于填充数组元素）
        /// </summary>
        public object DefaultValue
        {
            get => _defaultValue;
            set => _defaultValue = value;
        }

        #endregion

        #region 构造函数

        /// <summary>
        /// 初始化 <see cref="FixedSizeAttribute"/> 的新实例
        /// </summary>
        /// <param name="size">固定大小</param>
        /// <exception cref="ArgumentOutOfRangeException">大小超出范围</exception>
        public FixedSizeAttribute(int size)
        {
            if (size <= 0)
                throw new ArgumentOutOfRangeException(nameof(size), "大小必须大于 0");
            if (size > MaxFixedSize)
                throw new ArgumentOutOfRangeException(nameof(size), $"大小不能超过 {MaxFixedSize} 字节");

            _size = size;
        }

        /// <summary>
        /// 初始化 <see cref="FixedSizeAttribute"/> 的新实例
        /// </summary>
        /// <param name="size">固定大小</param>
        /// <param name="sizeUnit">大小单位</param>
        public FixedSizeAttribute(int size, SizeUnit sizeUnit) : this(size)
        {
            _sizeUnit = sizeUnit;
        }

        /// <summary>
        /// 初始化 <see cref="FixedSizeAttribute"/> 的新实例
        /// </summary>
        /// <param name="size">固定大小</param>
        /// <param name="paddingByte">填充字节</param>
        public FixedSizeAttribute(int size, byte paddingByte) : this(size)
        {
            _paddingByte = paddingByte;
        }

        #endregion

        #region 公共方法

        /// <summary>获取对齐后的大小</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetAlignedSize()
        {
            return _alignment <= 1 ? _size : (_size + _alignment - 1) & ~(_alignment - 1);
        }

        /// <summary>获取字符串的有效容量</summary>
        public int GetStringCapacity()
        {
            var capacity = _size;
            if (_nullTerminated) capacity -= 1;

            return _stringEncoding switch
            {
                StringEncoding.UTF8 => capacity,
                StringEncoding.UTF16LE or StringEncoding.UTF16BE => capacity / 2,
                StringEncoding.ASCII => capacity,
                _ => capacity
            };
        }

        /// <summary>计算需要的填充大小</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int CalculatePaddingSize(int actualSize)
        {
            return Math.Max(0, GetAlignedSize() - actualSize);
        }

        /// <summary>检查大小是否有效</summary>
        public bool IsValidSize(int actualSize)
        {
            if (!_validateSize) return true;
            if (_truncateIfExceeds) return true;
            return actualSize <= GetAlignedSize();
        }

        /// <summary>处理超出大小的情况</summary>
        public void HandleExceedsSize(int actualSize)
        {
            var targetSize = GetAlignedSize();
            if (actualSize > targetSize && !_truncateIfExceeds)
                throw new InvalidOperationException($"数据大小 ({actualSize}) 超过固定大小 ({targetSize})");
        }

        /// <summary>创建填充数组</summary>
        public byte[] CreatePadding(int length)
        {
            if (length <= 0) return Array.Empty<byte>();

            var padding = new byte[length];
            if (_paddingByte != 0)
            {
                for (int i = 0; i < length; i++)
                    padding[i] = _paddingByte;
            }
            return padding;
        }

        /// <summary>移除填充</summary>
        public int RemovePadding(byte[] data)
        {
            if (data == null || data.Length == 0 || !_trimPadding)
                return data?.Length ?? 0;

            var length = data.Length;

            if (_paddingPosition == PaddingPosition.End)
            {
                while (length > 0 && data[length - 1] == _paddingByte)
                    length--;
            }
            else
            {
                var start = 0;
                while (start < length && data[start] == _paddingByte)
                    start++;
                length -= start;
            }

            return length;
        }

        /// <summary>获取配置摘要信息</summary>
        public string GetSummary()
        {
            var parts = new System.Collections.Generic.List<string> { $"Size={_size}" };

            if (_sizeUnit != SizeUnit.Elements) parts.Add(_sizeUnit.ToString());
            if (_paddingByte != DefaultPaddingByte) parts.Add($"Pad=0x{_paddingByte:X2}");
            if (_paddingPosition != PaddingPosition.End) parts.Add($"PadPos={_paddingPosition}");
            if (!_truncateIfExceeds) parts.Add("NoTruncate");
            if (_nullTerminated) parts.Add("NullTerm");
            if (_alignment > 1) parts.Add($"Align={_alignment}");

            return string.Join(", ", parts);
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return _sizeUnit != SizeUnit.Elements
                ? $"[FixedSize({_size}, {_sizeUnit})]"
                : $"[FixedSize({_size})]";
        }

        #endregion
    }
}
