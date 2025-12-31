using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

#nullable enable
using System.Buffers;

namespace MemoryPack {

#if NET7_0_OR_GREATER

/// <summary>
/// 固定大小可序列化接口
/// </summary>
public interface IFixedSizeMemoryPackable
{
    static abstract int Size { get; }
}

#endif

/// <summary>
/// 格式化器注册接口
/// </summary>
public interface IMemoryPackFormatterRegister
{
#if NET7_0_OR_GREATER
    static abstract void RegisterFormatter();
#endif
}

/// <summary>
/// 可序列化接口
/// </summary>
public interface IMemoryPackable<T> : IMemoryPackFormatterRegister
{
    // note: serialize parameter should be `ref readonly` but current lang spec can not.
    // see proposal https://github.com/dotnet/csharplang/issues/6010

#if NET7_0_OR_GREATER
    static abstract void Serialize(ref MemoryPackWriter writer, ref T? value)
        ;
    static abstract void Deserialize(ref MemoryPackReader reader, ref T? value);
#endif
}

}