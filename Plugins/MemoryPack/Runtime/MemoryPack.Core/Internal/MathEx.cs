using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

#nullable enable
using System.Runtime.CompilerServices;

namespace MemoryPack.Internal {

/// <summary>
/// MathEx 类
/// </summary>
internal static class MathEx
{
    const int ArrayMexLength = 0x7FFFFFC7;

    /// <summary>
    /// 新数组容量
    /// </summary>
    /// <param name="size">大小</param>
    /// <returns>新数组容量</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int NewArrayCapacity(int size)
    {
        var newSize = unchecked(size * 2);
        if ((uint)newSize > ArrayMexLength)
        {
            newSize = ArrayMexLength;
        }
        return newSize;
    }
}

}