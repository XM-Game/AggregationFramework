using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

#nullable enable
namespace MemoryPack
{   
    /// <summary>
    /// MemoryPackSerializerOptions 类
    /// </summary>
    public record MemoryPackSerializerOptions
    {
        // Default is Utf8
        public static readonly MemoryPackSerializerOptions Default = new MemoryPackSerializerOptions { StringEncoding = StringEncoding.Utf8 };
        /// <summary>
        /// Utf8
        /// </summary>
        public static readonly MemoryPackSerializerOptions Utf8 = Default with { StringEncoding = StringEncoding.Utf8 };
        /// <summary>
        /// Utf16
        /// </summary>
        public static readonly MemoryPackSerializerOptions Utf16 = Default with { StringEncoding = StringEncoding.Utf16 };
        /// <summary>
        /// 字符串编码
        /// </summary>
        public StringEncoding StringEncoding { get; init; }
        /// <summary>
        /// 服务提供者
        /// </summary>
        public IServiceProvider? ServiceProvider { get; init; }
    }

    /// <summary>
    /// StringEncoding 枚举
    /// </summary>
    public enum StringEncoding : byte
    {
        /// <summary>
        /// Utf16
        /// </summary>
        Utf16,
        /// <summary>
        /// Utf8
        /// </summary>
        Utf8,
    }
}

#if !NET5_0_OR_GREATER

namespace System.Runtime.CompilerServices
{
    internal sealed class IsExternalInit
    {
    }
}

#endif
