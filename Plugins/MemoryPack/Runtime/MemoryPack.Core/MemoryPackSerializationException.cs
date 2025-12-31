using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

#nullable enable
using System.Buffers;
using System.Diagnostics.CodeAnalysis;

namespace MemoryPack {

/// <summary>
/// MemoryPack序列化异常类
/// </summary>
public class MemoryPackSerializationException : Exception
{
    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="message">消息</param>
    public MemoryPackSerializationException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="message">消息</param>
    /// <param name="innerException">内部异常</param>
    public MemoryPackSerializationException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    /// <summary>
    /// 抛出消息
    /// </summary>
    /// <param name="message">消息</param>
    [DoesNotReturn]
    public static void ThrowMessage(string message)
    {
        throw new MemoryPackSerializationException(message);
    }
    /// <summary>
    /// 抛出无效属性计数
    /// </summary>
    /// <param name="expected">期望值</param>
    /// <param name="actual">实际值</param>
    [DoesNotReturn]
    public static void ThrowInvalidPropertyCount(byte expected, byte actual)
        {
            throw new MemoryPackSerializationException($"对象属性计数不匹配，期望值: {expected}，实际值: {actual}，无法反序列化关于版本容错的信息。");
        }
    /// <summary>
    /// 抛出无效属性计数
    /// </summary>
    /// <param name="type">类型</param>
    /// <param name="expected">期望值</param>
    /// <param name="actual">实际值</param>
    [DoesNotReturn]
    public static void ThrowInvalidPropertyCount(Type type, byte expected, byte actual)
        {
            throw new MemoryPackSerializationException($"{type.FullName} 对象属性计数不匹配，期望值: {expected}，实际值: {actual}，无法反序列化关于版本容错的信息。");
        }
    /// <summary>
    /// 抛出无效集合
    /// </summary>
    [DoesNotReturn]
    public static void ThrowInvalidCollection()
        {
            throw new MemoryPackSerializationException($"当前读取到集合，缓冲区头不是集合。");
        }
    /// <summary>
    /// 抛出无效范围
    /// </summary>
    /// <param name="expected">期望值</param>
    /// <param name="actual">实际值</param>
    [DoesNotReturn]
    public static void ThrowInvalidRange(int expected, int actual)
        {
            throw new MemoryPackSerializationException($"所需大小是 {expected}，但缓冲区长度是 {actual}。");
        }
    /// <summary>
    /// 抛出无效前进
    /// </summary>
    [DoesNotReturn]
    public static void ThrowInvalidAdvance()
    {
        throw new MemoryPackSerializationException($"无法前进到缓冲区的末尾。");
    }
    /// <summary>
    /// 抛出序列到达末尾
    /// </summary>
    [DoesNotReturn]
    public static void ThrowSequenceReachedEnd()
        {
            throw new MemoryPackSerializationException($"序列到达末尾，读取器无法提供更多缓冲区。");
        }
    /// <summary>
    /// 抛出无效成员计数
    /// </summary>
    /// <param name="memberCount">成员计数</param>
    [DoesNotReturn]
    public static void ThrowWriteInvalidMemberCount(byte memberCount)
        {
            throw new MemoryPackSerializationException($"成员计数/标签允许 < 250，但尝试写入 {memberCount}。");
        }
    /// <summary>
    /// 抛出无效缓冲区长度
    /// </summary>
    /// <param name="length">长度</param>
    [DoesNotReturn]
    public static void ThrowInsufficientBufferUnless(int length)
        {
            throw new MemoryPackSerializationException($"长度头大小大于缓冲区大小，长度: {length}。");
        }
    /// <summary>
    /// 抛出未注册在提供者中
    /// </summary>
    /// <param name="type">类型</param>
    [DoesNotReturn]
    public static void ThrowNotRegisteredInProvider(Type type)
        {
            throw new MemoryPackSerializationException($"{type.FullName} 未注册在此提供者中。");
        }
    /// <summary>
    /// 抛出注册在提供者中失败
    /// </summary>
    /// <param name="type">类型</param>
    /// <param name="innerException">内部异常</param>
    [DoesNotReturn]
    public static void ThrowRegisterInProviderFailed(Type type, Exception innerException)
    {
        throw new MemoryPackSerializationException($"{type.FullName} 在提供者中创建格式化器失败。", innerException);
    }
    /// <summary>
    /// 抛出未找到在联合类型中
    /// </summary>
    /// <param name="actualType">实际类型</param>
    /// <param name="baseType">基类型</param>
    [DoesNotReturn]
    public static void ThrowNotFoundInUnionType(Type actualType, Type baseType)
        {
            throw new MemoryPackSerializationException($"类型 {actualType.FullName} 未注解在 {baseType.FullName} MemoryPackUnion 中。");
        }
    /// <summary>
    /// 抛出无效标签
    /// </summary>
    /// <param name="tag">标签</param>
    /// <param name="baseType">基类型</param>
    [DoesNotReturn]
    public static void ThrowInvalidTag(ushort tag, Type baseType)
    {
        throw new MemoryPackSerializationException($"读取到的标签: {tag} 未找到在 {baseType.FullName} MemoryPackUnion 注解中。");
    }
    /// <summary>
    /// 抛出到达深度限制
    /// </summary>
    /// <param name="type">类型</param>
    [DoesNotReturn]
    public static void ThrowReachedDepthLimit(Type type)
    {
        throw new MemoryPackSerializationException($"序列化类型 '{type}' 到达深度限制，可能检测到循环引用。");
    }
    /// <summary>
    /// 抛出无效并发集合操作
    /// </summary>
    [DoesNotReturn]
    public static void ThrowInvalidConcurrrentCollectionOperation()
        {
            throw new MemoryPackSerializationException($"并发集合在序列化期间添加/删除，但序列化并发集合不是线程安全的。");
        }
    /// <summary>
    /// 抛出反序列化对象为空
    /// </summary>
    /// <param name="target">目标</param>
    [DoesNotReturn]
    public static void ThrowDeserializeObjectIsNull(string target)
        {
            throw new MemoryPackSerializationException($"反序列化 {target} 为空。");
        }
    /// <summary>
    /// 抛出编码失败
    /// </summary>
    /// <param name="status">状态</param>
    [DoesNotReturn]
    public static void ThrowFailedEncoding(OperationStatus status)
        {
            throw new MemoryPackSerializationException($"Utf8 编码/解码过程失败，状态: {status}。");
        }
    /// <summary>
    /// 抛出压缩失败
    /// </summary>
    /// <param name="status">状态</param>
    [DoesNotReturn]
    public static void ThrowCompressionFailed(OperationStatus status)
        {
            throw new MemoryPackSerializationException($"Brotli 压缩/解压缩过程失败，状态: {status}。");
        }
    /// <summary>
    /// 抛出压缩失败
    /// </summary>
    [DoesNotReturn]
    public static void ThrowCompressionFailed()
    {
        throw new MemoryPackSerializationException($"Brotli 压缩/解压缩过程失败。");
    }
    /// <summary>
    /// 抛出已解压缩
    /// </summary>
    [DoesNotReturn]
    public static void ThrowAlreadyDecompressed()
    {
        throw new MemoryPackSerializationException($"BrotliDecompressor 不能重复调用 Decompress，已经调用过。");
    }
    /// <summary>
    /// 抛出解压缩大小限制超出
    /// </summary>
    /// <param name="limit">限制</param>
    /// <param name="size">大小</param>
    [DoesNotReturn]
    public static void ThrowDecompressionSizeLimitExceeded(int limit, int size)
    {
        throw new MemoryPackSerializationException($"在解压缩过程中，限制是 {limit}，但目标大小是 {size}。");
    }
}

}