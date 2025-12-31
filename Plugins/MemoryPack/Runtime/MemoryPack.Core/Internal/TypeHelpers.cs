using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

#nullable enable
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace MemoryPack.Internal {

/// <summary>
/// TypeHelpers 类
/// </summary>
internal static class TypeHelpers
{
    static readonly MethodInfo isReferenceOrContainsReferences = typeof(RuntimeHelpers).GetMethod("IsReferenceOrContainsReferences")!;

    /// <summary>
    /// 是否引用或可空
    /// </summary>
    /// <typeparam name="T">类型</typeparam>
    /// <returns>是否引用或可空</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsReferenceOrNullable<T>()
    {
        return Cache<T>.IsReferenceOrNullable;
    }

    /// <summary>
    /// 尝试获取未管理SZ数组元素大小或内存打包固定大小
    /// </summary>
    /// <typeparam name="T">类型</typeparam>
    /// <param name="size">大小</param>
    /// <returns>类型种类</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TypeKind TryGetUnmanagedSZArrayElementSizeOrMemoryPackableFixedSize<T>(out int size)
    {
        if (Cache<T>.IsUnmanagedSZArray)
        {
            size = Cache<T>.UnmanagedSZArrayElementSize;
            return TypeKind.UnmanagedSZArray;
        }
        else
        {
            if (Cache<T>.IsFixedSizeMemoryPackable)
            {
                size = Cache<T>.MemoryPackableFixedSize;
                return TypeKind.FixedSizeMemoryPackable;
            }
        }

        size = 0;
        return TypeKind.None;
    }

    /// <summary>
    /// 是否匿名
    /// </summary>
    /// <param name="type">类型</param>
    /// <returns>是否匿名</returns>
    public static bool IsAnonymous(Type type)
    {
        return type.Namespace == null
               && type.IsSealed
               && (type.Name.StartsWith("<>f__AnonymousType", StringComparison.Ordinal)
                   || type.Name.StartsWith("<>__AnonType", StringComparison.Ordinal)
                   || type.Name.StartsWith("VB$AnonymousType_", StringComparison.Ordinal))
               && type.IsDefined(typeof(CompilerGeneratedAttribute), false);
    }

    /// <summary>
    /// Cache 类
    /// </summary>
    /// <typeparam name="T">类型</typeparam>
    static class Cache<T>
    {
        public static bool IsReferenceOrNullable;
        public static bool IsUnmanagedSZArray;
        public static int UnmanagedSZArrayElementSize;
        public static bool IsFixedSizeMemoryPackable = false;
        public static int MemoryPackableFixedSize = 0;

        /// <summary>
        /// 构造函数
        /// </summary>
        static Cache()
        {
            try
            {
                var type = typeof(T);
                IsReferenceOrNullable = !type.IsValueType || Nullable.GetUnderlyingType(type) != null;

                if (type.IsSZArray)
                {
                    var elementType = type.GetElementType();
                    bool containsReference = (bool)(isReferenceOrContainsReferences.MakeGenericMethod(elementType!).Invoke(null, null)!);
                    if (!containsReference)
                    {
                        IsUnmanagedSZArray = true;
                        UnmanagedSZArrayElementSize = Marshal.SizeOf(elementType!);
                    }
                }
#if NET7_0_OR_GREATER
                else
                {
                    if (typeof(IFixedSizeMemoryPackable).IsAssignableFrom(type))
                    {
                        var prop = type.GetProperty("global::MemoryPack.IFixedSizeMemoryPackable.Size", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
                        if (prop != null)
                        {
                            IsFixedSizeMemoryPackable = true;
                            MemoryPackableFixedSize = (int)prop.GetValue(null)!;
                        }
                    }
                }
#endif
            }
            catch
            {
                IsUnmanagedSZArray = false;
                IsFixedSizeMemoryPackable = false;
            }
        }
    }

    /// <summary>
    /// TypeKind 枚举
    /// </summary>
    internal enum TypeKind : byte
    {
        /// <summary>
        /// None 没有类型
        /// </summary>
        None,
        /// <summary>
        /// UnmanagedSZArray 未管理SZ数组
        /// </summary>
        UnmanagedSZArray,
        /// <summary>
        /// FixedSizeMemoryPackable 内存打包固定大小
        /// </summary>
        FixedSizeMemoryPackable
    }
}

}