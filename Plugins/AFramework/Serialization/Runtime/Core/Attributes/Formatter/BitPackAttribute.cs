// ==========================================================
// 文件名：BitPackAttribute.cs
// 命名空间: AFramework.Serialization
// 依赖: System
// ==========================================================

using System;
using System.Runtime.CompilerServices;

namespace AFramework.Serialization
{
    /// <summary>
    /// 位打包特性
    /// <para>指定字段使用位级别的紧凑存储</para>
    /// </summary>
    /// <remarks>
    /// <para><b>功能说明：</b></para>
    /// <list type="bullet">
    ///   <item>将数值字段压缩到指定位数</item>
    ///   <item>支持有符号和无符号整数</item>
    ///   <item>支持浮点数量化</item>
    ///   <item>支持布尔值打包</item>
    /// </list>
    /// 
    /// <para><b>使用示例：</b></para>
    /// <code>
    /// [Serializable]
    /// public class NetworkPacket
    /// {
    ///     // 将 int 压缩到 10 位（0-1023）
    ///     [BitPack(10)]
    ///     public int Health;
    ///     
    ///     // 将 float 量化到 16 位
    ///     [BitPack(16, MinValue = 0f, MaxValue = 100f)]
    ///     public float Progress;
    ///     
    ///     // 有符号整数（-512 到 511）
    ///     [BitPack(10, Signed = true)]
    ///     public int Offset;
    ///     
    ///     // 布尔值打包（1 位）
    ///     [BitPack(1)]
    ///     public bool IsActive;
    /// }
    /// </code>
    /// 
    /// <para><b>位数与范围对照：</b></para>
    /// <list type="table">
    ///   <listheader>
    ///     <term>位数</term>
    ///     <description>无符号范围 | 有符号范围</description>
    ///   </listheader>
    ///   <item><term>1</term><description>0-1 | N/A</description></item>
    ///   <item><term>4</term><description>0-15 | -8 to 7</description></item>
    ///   <item><term>8</term><description>0-255 | -128 to 127</description></item>
    ///   <item><term>10</term><description>0-1023 | -512 to 511</description></item>
    ///   <item><term>16</term><description>0-65535 | -32768 to 32767</description></item>
    /// </list>
    /// </remarks>
    [AttributeUsage(
        AttributeTargets.Field | AttributeTargets.Property,
        AllowMultiple = false,
        Inherited = true)]
    public sealed class BitPackAttribute : Attribute
    {
        #region 常量

        /// <summary>最小位数</summary>
        public const int MinBits = 1;

        /// <summary>最大位数</summary>
        public const int MaxBits = 64;

        /// <summary>默认浮点精度位数</summary>
        public const int DefaultFloatBits = 16;

        #endregion

        #region 字段

        private readonly int _bits;
        private bool _signed;
        private float _minValue;
        private float _maxValue = 1f;
        private bool _enableRangeCheck = true;
        private OutOfRangeHandling _outOfRangeHandling = OutOfRangeHandling.Clamp;
        private int _bitOffset = -1;
        private bool _useZigZag;
        private float _quantizationStep;

        #endregion

        #region 属性

        /// <summary>获取位数</summary>
        public int Bits => _bits;

        /// <summary>
        /// 获取或设置是否有符号
        /// <para>默认值：false</para>
        /// </summary>
        public bool Signed
        {
            get => _signed;
            set => _signed = value;
        }

        /// <summary>
        /// 获取或设置最小值（用于浮点量化）
        /// <para>默认值：0</para>
        /// </summary>
        public float MinValue
        {
            get => _minValue;
            set => _minValue = value;
        }

        /// <summary>
        /// 获取或设置最大值（用于浮点量化）
        /// <para>默认值：1</para>
        /// </summary>
        public float MaxValue
        {
            get => _maxValue;
            set => _maxValue = value;
        }

        /// <summary>
        /// 获取或设置是否启用范围检查
        /// <para>默认值：true</para>
        /// </summary>
        public bool EnableRangeCheck
        {
            get => _enableRangeCheck;
            set => _enableRangeCheck = value;
        }

        /// <summary>
        /// 获取或设置超出范围时的处理方式
        /// <para>默认值：<see cref="OutOfRangeHandling.Clamp"/></para>
        /// </summary>
        public OutOfRangeHandling OutOfRangeHandling
        {
            get => _outOfRangeHandling;
            set => _outOfRangeHandling = value;
        }

        /// <summary>
        /// 获取或设置位偏移（用于手动布局）
        /// <para>默认值：-1（自动计算）</para>
        /// </summary>
        public int BitOffset
        {
            get => _bitOffset;
            set => _bitOffset = value;
        }

        /// <summary>
        /// 获取或设置是否使用 ZigZag 编码
        /// <para>默认值：false</para>
        /// </summary>
        public bool UseZigZag
        {
            get => _useZigZag;
            set => _useZigZag = value;
        }

        /// <summary>
        /// 获取或设置量化步长（用于浮点数）
        /// <para>默认值：0（自动计算）</para>
        /// </summary>
        public float QuantizationStep
        {
            get => _quantizationStep;
            set => _quantizationStep = value;
        }

        #endregion

        #region 构造函数

        /// <summary>
        /// 初始化 <see cref="BitPackAttribute"/> 的新实例
        /// </summary>
        /// <param name="bits">位数（1-64）</param>
        /// <exception cref="ArgumentOutOfRangeException">位数超出范围</exception>
        public BitPackAttribute(int bits)
        {
            if (bits < MinBits || bits > MaxBits)
                throw new ArgumentOutOfRangeException(nameof(bits),
                    $"位数必须在 {MinBits} 到 {MaxBits} 之间");

            _bits = bits;
        }

        /// <summary>
        /// 初始化 <see cref="BitPackAttribute"/> 的新实例
        /// </summary>
        /// <param name="bits">位数</param>
        /// <param name="signed">是否有符号</param>
        public BitPackAttribute(int bits, bool signed) : this(bits)
        {
            _signed = signed;
        }

        /// <summary>
        /// 初始化 <see cref="BitPackAttribute"/> 的新实例（用于浮点量化）
        /// </summary>
        /// <param name="bits">位数</param>
        /// <param name="minValue">最小值</param>
        /// <param name="maxValue">最大值</param>
        public BitPackAttribute(int bits, float minValue, float maxValue) : this(bits)
        {
            _minValue = minValue;
            _maxValue = maxValue;
        }

        #endregion

        #region 公共方法

        /// <summary>获取无符号整数的最大值</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ulong GetMaxUnsignedValue()
        {
            return _bits >= 64 ? ulong.MaxValue : (1UL << _bits) - 1;
        }

        /// <summary>获取有符号整数的范围</summary>
        public void GetSignedRange(out long min, out long max)
        {
            if (_bits >= 64)
            {
                min = long.MinValue;
                max = long.MaxValue;
            }
            else
            {
                var halfRange = 1L << (_bits - 1);
                min = -halfRange;
                max = halfRange - 1;
            }
        }

        /// <summary>获取浮点量化精度</summary>
        public float GetQuantizationPrecision()
        {
            if (_quantizationStep > 0)
                return _quantizationStep;

            var range = _maxValue - _minValue;
            var steps = GetMaxUnsignedValue();
            return range / steps;
        }

        /// <summary>量化浮点数为整数</summary>
        public ulong QuantizeFloat(float value)
        {
            if (_enableRangeCheck)
            {
                value = _outOfRangeHandling switch
                {
                    OutOfRangeHandling.Clamp => Math.Clamp(value, _minValue, _maxValue),
                    OutOfRangeHandling.Wrap => WrapValue(value),
                    OutOfRangeHandling.Throw when value < _minValue || value > _maxValue =>
                        throw new ArgumentOutOfRangeException(nameof(value),
                            $"值 {value} 超出范围 [{_minValue}, {_maxValue}]"),
                    _ => value
                };
            }

            var normalized = (value - _minValue) / (_maxValue - _minValue);
            var maxValue = GetMaxUnsignedValue();
            return (ulong)(normalized * maxValue + 0.5f);
        }

        /// <summary>反量化整数为浮点数</summary>
        public float DequantizeFloat(ulong quantized)
        {
            var maxValue = GetMaxUnsignedValue();
            var normalized = (float)quantized / maxValue;
            return _minValue + normalized * (_maxValue - _minValue);
        }

        /// <summary>编码有符号整数</summary>
        public ulong EncodeSignedInteger(long value)
        {
            if (_useZigZag)
                return (ulong)((value << 1) ^ (value >> 63));

            GetSignedRange(out var min, out _);
            return (ulong)(value - min);
        }

        /// <summary>解码有符号整数</summary>
        public long DecodeSignedInteger(ulong encoded)
        {
            if (_useZigZag)
                return (long)(encoded >> 1) ^ -((long)encoded & 1);

            GetSignedRange(out var min, out _);
            return (long)encoded + min;
        }

        /// <summary>检查值是否在有效范围内</summary>
        public bool IsInRange(long value)
        {
            if (_signed)
            {
                GetSignedRange(out var min, out var max);
                return value >= min && value <= max;
            }
            return value >= 0 && (ulong)value <= GetMaxUnsignedValue();
        }

        /// <summary>检查浮点值是否在有效范围内</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsInRange(float value) => value >= _minValue && value <= _maxValue;

        /// <summary>获取配置摘要信息</summary>
        public string GetSummary()
        {
            var parts = new System.Collections.Generic.List<string> { $"{_bits}bits" };

            if (_signed) parts.Add("Signed");
            if (_minValue != 0 || _maxValue != 1) parts.Add($"Range=[{_minValue},{_maxValue}]");
            if (_useZigZag) parts.Add("ZigZag");
            if (_bitOffset >= 0) parts.Add($"Offset={_bitOffset}");

            return string.Join(", ", parts);
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return _signed ? $"[BitPack({_bits}, Signed=true)]" : $"[BitPack({_bits})]";
        }

        #endregion

        #region 私有方法

        private float WrapValue(float value)
        {
            var range = _maxValue - _minValue;
            var normalized = (value - _minValue) % range;
            if (normalized < 0) normalized += range;
            return _minValue + normalized;
        }

        #endregion
    }
}
