using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

#nullable enable
using MemoryPack.Internal;
using System.Text.RegularExpressions;

namespace MemoryPack.Formatters {

[Preserve]
public sealed partial class TypeFormatter : MemoryPackFormatter<Type>
{
    // Remove Version, Culture, PublicKeyToken from AssemblyQualifiedName.
    // Result will be "TypeName, Assembly"
    // see:http://msdn.microsoft.com/en-us/library/w3f99sx1.aspx

#if NET7_0_OR_GREATER

    [GeneratedRegex(@", Version=\d+.\d+.\d+.\d+, Culture=[\w-]+, PublicKeyToken=(?:null|[a-f0-9]{16})")]
    private static partial Regex ShortTypeNameRegex();

#else

    static readonly Regex _shortTypeNameRegex = new Regex(@", Version=\d+.\d+.\d+.\d+, Culture=[\w-]+, PublicKeyToken=(?:null|[a-f0-9]{16})", RegexOptions.Compiled);
    static Regex ShortTypeNameRegex() => _shortTypeNameRegex;

#endif
    /// <summary>
    /// 序列化Type
    /// </summary>
    /// <param name="writer">MemoryPackWriter</param>
    /// <param name="value">Type</param>
    /// <returns>void</returns>
    [Preserve]
    public override void Serialize(ref MemoryPackWriter writer, ref Type? value)
    {
        var full = value?.AssemblyQualifiedName;
        if (full == null)
        {
            writer.WriteNullCollectionHeader();
            return;
        }

        // 替换版本号、文化、公钥令牌
        var shortName = ShortTypeNameRegex().Replace(full, "");
        // 写入字符串
        writer.WriteString(shortName);
    }

    /// <summary>
    /// 反序列化Type
    /// </summary>
    /// <param name="reader">MemoryPackReader</param>
    /// <param name="value">Type</param>
    /// <returns>void</returns>
    [Preserve]
    public override void Deserialize(ref MemoryPackReader reader, ref Type? value)
    {
        // 读取字符串
        var typeName = reader.ReadString();
        // 如果字符串为空，则设置为空
        if (typeName == null)
        {
            value = null;
            return;
        }

        value = Type.GetType(typeName, throwOnError: true);
    }
}

}