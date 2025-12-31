using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

#nullable enable
namespace MemoryPack {

/// <summary>
/// MemoryPack 代码
/// </summary>
public static class MemoryPackCode
{
    /// <summary>
    /// Null Collection 空集合
    /// </summary>
    // 0~* is length, -1 is null
    public const int NullCollection = -1;

    /// <summary>
    /// Object/Union Header 对象/联合头
    /// </summary>
    // 0~249 is member count or tag, 250~254 is unused, 255 is null
    public const byte WideTag = 250; // for Union, 250 is wide tag
    /// <summary>
    /// ReferenceId 引用ID
    /// </summary>
    public const byte ReferenceId = 250; // for CircularReference, 250 is referenceId marker, next VarInt id reference.

    /// <summary>
    /// Reserved1 保留1
    /// </summary>
    public const byte Reserved1 = 250;
    /// <summary>
    /// Reserved2 保留2
    /// </summary>
    public const byte Reserved2 = 251;
    /// <summary>
    /// Reserved3 保留3
    /// </summary>
    public const byte Reserved3 = 252;
    /// <summary>
    /// Reserved4 保留4
    /// </summary>
    public const byte Reserved4 = 253;
    /// <summary>
    /// Reserved5 保留5
    /// </summary>
    public const byte Reserved5 = 254;
    /// <summary>
    /// NullObject 空对象
    /// </summary>
    public const byte NullObject = 255;

    // predefined, C# compiler optimize byte[] as ReadOnlySpan<byte> property
    internal static ReadOnlySpan<byte> NullCollectionData => new byte[] { 255, 255, 255, 255 }; // -1
    internal static ReadOnlySpan<byte> ZeroCollectionData => new byte[] { 0, 0, 0, 0 }; // 0
}

}