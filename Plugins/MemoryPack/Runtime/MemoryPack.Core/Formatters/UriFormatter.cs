using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

#nullable enable
using MemoryPack.Internal;
using System.Buffers;

namespace MemoryPack.Formatters {

/// <summary>
/// UriFormatter 类
/// </summary>
[Preserve]
public sealed class UriFormatter : MemoryPackFormatter<Uri>
{
    /// <summary>
    /// 序列化
    /// </summary>
    /// <param name="writer">写入器</param>
    /// <param name="value">值</param>
    /// <returns>void</returns>
    [Preserve]
    public override void Serialize(ref MemoryPackWriter writer, ref Uri? value)
    {
        writer.WriteString(value?.OriginalString);
    }

    [Preserve]
    public override void Deserialize(ref MemoryPackReader reader, ref Uri? value)
    {
        var str = reader.ReadString();
        if (str == null)
        {
            value = null;
        }
        else
        {
            value = new Uri(str, UriKind.RelativeOrAbsolute);
        }
    }
}

}