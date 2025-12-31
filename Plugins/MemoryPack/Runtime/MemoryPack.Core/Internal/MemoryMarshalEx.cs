using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

#nullable enable
#if !NET7_0_OR_GREATER

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace MemoryPack.Internal {

/// <summary>
/// MemoryMarshalEx 类
/// </summary>
internal static class MemoryMarshalEx
{
    /// <summary>
    /// 获取数组数据引用
    /// </summary>
    /// <typeparam name="T">类型</typeparam>
    /// <param name="array">数组</param>
    /// <returns>数据引用</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref T GetArrayDataReference<T>(T[] array)
    {
        return ref MemoryMarshal.GetReference(array.AsSpan());
    }

    /// <summary>
    /// 分配未初始化的数组
    /// </summary>
    /// <typeparam name="T">类型</typeparam>
    /// <param name="length">长度</param>
    /// <param name="pinned">是否固定</param>
    /// <returns>数组</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T[] AllocateUninitializedArray<T>(int length, bool pinned = false)
    {
        return new T[length];
    }
}

#endif

}