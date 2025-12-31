using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

#nullable enable
using MemoryPack.Internal;
using System.Runtime.CompilerServices;
using static System.Runtime.CompilerServices.Unsafe;

#if NET7_0_OR_GREATER
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
#endif

namespace MemoryPack.Compression {

[Preserve]
public sealed class BitPackFormatter : MemoryPackFormatter<bool[]>
{
    /// <summary>
    /// 默认实例
    /// </summary>
    public static readonly BitPackFormatter Default = new BitPackFormatter();

    /// <summary>
    /// 序列化布尔数组
    /// </summary>
    /// <param name="writer">MemoryPackWriter</param>
    /// <param name="value">布尔数组</param>
    /// <returns>void</returns>
    [Preserve]
    public override void Serialize(ref MemoryPackWriter writer, ref bool[]? value)
    {
        if (value == null)
        {
            writer.WriteNullCollectionHeader();
            return;
        }
        writer.WriteCollectionHeader(value.Length);
        if (value.Length == 0)
        {
            return;
        }

        var data = 0;
#if NET7_0_OR_GREATER
        ref var item = ref MemoryMarshal.GetArrayDataReference(value);
#else
        ref var item = ref value[0];
#endif
        ref var end = ref Unsafe.Add(ref item, value.Length);

#if NET7_0_OR_GREATER
        if (value.Length >= 32)
        {
            ref var loopEnd = ref Unsafe.Subtract(ref end, 32);
            if (Vector256.IsHardwareAccelerated)
            {
                while (!Unsafe.IsAddressGreaterThan(ref item, ref loopEnd))
                {
                    var vector = Vector256.LoadUnsafe(ref Unsafe.As<bool, byte>(ref item));
                    // false -> 1 true -> 0
                    data = (int)Vector256.Equals(vector, Vector256<byte>.Zero).ExtractMostSignificantBits();
                    writer.WriteUnmanaged(~data);
                    item = ref Unsafe.Add(ref item, 32);
                }
            }
            else if (Vector128.IsHardwareAccelerated)
            {
                while (!Unsafe.IsAddressGreaterThan(ref item, ref loopEnd))
                {
                    var bits0 = (ushort)Vector128.Equals(Vector128.LoadUnsafe(ref Unsafe.As<bool, byte>(ref item)), Vector128<byte>.Zero).ExtractMostSignificantBits();
                    var bits1 = (ushort)Vector128.Equals(Vector128.LoadUnsafe(ref Unsafe.As<bool, byte>(ref item), 16), Vector128<byte>.Zero).ExtractMostSignificantBits();
                    data = bits0 | (bits1 << 16);
                    writer.WriteUnmanaged(~data);
                    item = ref Unsafe.Add(ref item, 32);
                }
            }
            else if (Vector64.IsHardwareAccelerated)
            {
                while (!Unsafe.IsAddressGreaterThan(ref item, ref loopEnd))
                {
                    var bits0 = (byte)Vector64.Equals(Vector64.LoadUnsafe(ref Unsafe.As<bool, byte>(ref item)), Vector64<byte>.Zero).ExtractMostSignificantBits();
                    var bits1 = (byte)Vector64.Equals(Vector64.LoadUnsafe(ref Unsafe.As<bool, byte>(ref item), 8), Vector64<byte>.Zero).ExtractMostSignificantBits();
                    var bits2 = (byte)Vector64.Equals(Vector64.LoadUnsafe(ref Unsafe.As<bool, byte>(ref item), 16), Vector64<byte>.Zero).ExtractMostSignificantBits();
                    var bits3 = (byte)Vector64.Equals(Vector64.LoadUnsafe(ref Unsafe.As<bool, byte>(ref item), 24), Vector64<byte>.Zero).ExtractMostSignificantBits();
                    data = bits0 | (bits1 << 8) | (bits2 << 16) | (bits3 << 24);
                    writer.WriteUnmanaged(~data);
                    item = ref Unsafe.Add(ref item, 32);
                }
            }

            data = 0;
        }
#endif
        var bit = 0;
        while (Unsafe.IsAddressLessThan(ref item, ref end))
        {
            Set(ref data, bit, item);

            item = ref Unsafe.Add(ref item, 1);
            bit += 1;

            if (bit == 32)
            {
                writer.WriteUnmanaged(data);
                data = 0;
                bit = 0;
            }
        }

        if (bit != 0)
        {
            writer.WriteUnmanaged(data);
        }
    }

    /// <summary>
    /// 反序列化布尔数组
    /// </summary>
    /// <param name="reader">MemoryPackReader</param>
    /// <param name="value">布尔数组</param>
    /// <returns>void</returns>
    [Preserve]
    public override void Deserialize(ref MemoryPackReader reader, ref bool[]? value)
    {
        if (!reader.DangerousTryReadCollectionHeader(out var length))
        {
            value = null;
            return;
        }

        if (length == 0)
        {
            value = Array.Empty<bool>();
            return;
        }

        var readCount = ((length - 1) / 32) + 1;
        var requireSize = readCount * 4;
        if (reader.Remaining < requireSize)
        {
            MemoryPackSerializationException.ThrowInsufficientBufferUnless(length);
        }

        if (value == null || value.Length != length)
        {
            value = new bool[length];
        }

        var bit = 0;
        var data = 0;
        for (int i = 0; i < value.Length; i++)
        {
            if (bit == 0)
            {
                reader.ReadUnmanaged(out data);
            }

            value[i] = Get(data, bit);

            bit += 1;

            if (bit == 32)
            {
                data = 0;
                bit = 0;
            }
        }
    }

    /// <summary>
    /// 获取布尔值
    /// </summary>
    /// <param name="data">数据</param>
    /// <param name="index">索引</param>
    /// <returns>布尔值</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Get(int data, int index)
    {
        return (data & (1 << index)) != 0;
    }

    /// <summary>
    /// 设置布尔值
    /// </summary>
    /// <param name="data">数据</param>
    /// <param name="index">索引</param>
    /// <param name="value">布尔值</param>
    /// <returns>void</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Set(ref int data, int index, bool value)
    {
        int bitMask = 1 << index;
        if (value)
        {
            data |= bitMask;
        }
        else
        {
            data &= ~bitMask;
        }
    }
}

}