using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

#nullable enable
using MemoryPack.Compression;
using MemoryPack.Formatters;

namespace MemoryPack {

#if !UNITY_2021_2_OR_NEWER
/// <summary>
/// UTF-8 字符串格式化器属性
/// </summary>
public sealed class Utf8StringFormatterAttribute : MemoryPackCustomFormatterAttribute<Utf8StringFormatter, string>
{
    public override Utf8StringFormatter GetFormatter()
    {
        return Utf8StringFormatter.Default;
    }
}

/// <summary>
/// UTF-16 字符串格式化器属性
/// </summary>
public sealed class Utf16StringFormatterAttribute : MemoryPackCustomFormatterAttribute<Utf16StringFormatter, string>
{
    public override Utf16StringFormatter GetFormatter()
    {
        return Utf16StringFormatter.Default;
    }
}

/// <summary>
/// 忽略大小写字符串字典格式化器属性
/// </summary>
public sealed class OrdinalIgnoreCaseStringDictionaryFormatter<TValue> : MemoryPackCustomFormatterAttribute<DictionaryFormatter<string, TValue?>, Dictionary<string, TValue?>>
{
    static readonly DictionaryFormatter<string, TValue?> formatter = new DictionaryFormatter<string, TValue?>(StringComparer.OrdinalIgnoreCase);

    public override DictionaryFormatter<string, TValue?> GetFormatter()
    {
        return formatter;
    }
}

/// <summary>
/// 内化字符串格式化器属性
/// </summary>
public sealed class InternStringFormatterAttribute : MemoryPackCustomFormatterAttribute<InternStringFormatter, string>
{
    public override InternStringFormatter GetFormatter()
    {
        return InternStringFormatter.Default;
    }
}

/// <summary>
/// 位打包格式化器属性
/// </summary>
public sealed class BitPackFormatterAttribute : MemoryPackCustomFormatterAttribute<BitPackFormatter, bool[]>
{
    public override BitPackFormatter GetFormatter()
    {
        return BitPackFormatter.Default;
    }
}

/// <summary>
/// Brotli 压缩格式化器属性
/// </summary>
public sealed class BrotliFormatterAttribute : MemoryPackCustomFormatterAttribute<BrotliFormatter, byte[]>
{
    public System.IO.Compression.CompressionLevel CompressionLevel { get; }
    public int Window { get; }
    public int DecompressionSizeLimit { get; }

    public BrotliFormatterAttribute(System.IO.Compression.CompressionLevel compressionLevel = System.IO.Compression.CompressionLevel.Fastest, int window = BrotliUtils.WindowBits_Default, int decompressionSizeLimit = BrotliFormatter.DefaultDecompssionSizeLimit)
    {
        this.CompressionLevel = compressionLevel;
        this.Window = window;
        this.DecompressionSizeLimit = decompressionSizeLimit;
    }

    public override BrotliFormatter GetFormatter()
    {
        return new BrotliFormatter(CompressionLevel, Window, DecompressionSizeLimit);
    }
}

/// <summary>
/// Brotli 压缩格式化器属性
/// </summary>
public sealed class BrotliFormatterAttribute<T> : MemoryPackCustomFormatterAttribute<BrotliFormatter<T>, T>
{
    public System.IO.Compression.CompressionLevel CompressionLevel { get; }
    public int Window { get; }

    public BrotliFormatterAttribute(System.IO.Compression.CompressionLevel compressionLevel = System.IO.Compression.CompressionLevel.Fastest, int window = BrotliUtils.WindowBits_Default)
    {
        this.CompressionLevel = compressionLevel;
        this.Window = window;
    }

    public override BrotliFormatter<T> GetFormatter()
    {
        return new BrotliFormatter<T>(CompressionLevel, Window);
    }
}

/// <summary>
/// Brotli 压缩字符串格式化器属性
/// </summary>
public sealed class BrotliStringFormatterAttribute : MemoryPackCustomFormatterAttribute<BrotliStringFormatter, string>
{
    public System.IO.Compression.CompressionLevel CompressionLevel { get; }
    public int Window { get; }
    public int DecompressionSizeLimit { get; }

    public BrotliStringFormatterAttribute(System.IO.Compression.CompressionLevel compressionLevel = System.IO.Compression.CompressionLevel.Fastest, int window = BrotliUtils.WindowBits_Default, int decompressionSizeLimit = BrotliFormatter.DefaultDecompssionSizeLimit)
    {
        this.CompressionLevel = compressionLevel;
        this.Window = window;
        this.DecompressionSizeLimit = decompressionSizeLimit;
    }

    public override BrotliStringFormatter GetFormatter()
    {
        return new BrotliStringFormatter(CompressionLevel, Window, DecompressionSizeLimit);
    }
}

/// <summary>
/// 内存池格式化器属性
/// </summary>
public sealed class MemoryPoolFormatterAttribute<T> : MemoryPackCustomFormatterAttribute<MemoryPoolFormatter<T>, Memory<T?>>
{
    static readonly MemoryPoolFormatter<T> formatter = new MemoryPoolFormatter<T>();

    public override MemoryPoolFormatter<T> GetFormatter()
    {
        return formatter;
    }
}

/// <summary>
/// 只读内存池格式化器属性
/// </summary>
public sealed class ReadOnlyMemoryPoolFormatterAttribute<T> : MemoryPackCustomFormatterAttribute<ReadOnlyMemoryPoolFormatter<T>, ReadOnlyMemory<T?>>
{
    static readonly ReadOnlyMemoryPoolFormatter<T> formatter = new ReadOnlyMemoryPoolFormatter<T>();

    public override ReadOnlyMemoryPoolFormatter<T> GetFormatter()
    {
        return formatter;
    }
}

#endif

}