using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

#nullable enable
using MemoryPack.Internal;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace MemoryPack.Formatters {

[Preserve]
public sealed class BigIntegerFormatter : MemoryPackFormatter<BigInteger>
{
    /// <summary>
    /// 序列化BigInteger
    /// </summary>
    /// <param name="writer">MemoryPackWriter</param>
    /// <param name="value">BigInteger</param>
    /// <returns>void</returns>
    [Preserve]
    public override void Serialize(ref MemoryPackWriter writer, ref BigInteger value)
    {
        // 如果未在Unity 2021.2或更高版本中编译
#if !UNITY_2021_2_OR_NEWER
        // 分配内存
        Span<byte> temp = stackalloc byte[255];
        if (value.TryWriteBytes(temp, out var written))
        {
            writer.WriteUnmanagedSpan(temp.Slice(written));
            return;
        }
        else
        // 否则，将BigInteger转换为字节数组
#endif
        {
            var byteArray = value.ToByteArray();
            // 写入字节数组
            writer.WriteUnmanagedArray(byteArray);
        }
    }

    /// <summary>
    /// 反序列化BigInteger
    /// </summary>
    /// <param name="reader">MemoryPackReader</param>
    /// <param name="value">BigInteger</param>
    /// <returns>void</returns>
    [Preserve]
    public override void Deserialize(ref MemoryPackReader reader, ref BigInteger value)
    {
        // 读取长度
        if (!reader.TryReadCollectionHeader(out var length))
        {
            value = default;
            return;
        }

        // 创建只读Span
        ref var src = ref reader.GetSpanReference(length);
        // 创建BigInteger
        value = new BigInteger(MemoryMarshal.CreateReadOnlySpan(ref src, length));

        // 前进
        reader.Advance(length);
    }
}

}