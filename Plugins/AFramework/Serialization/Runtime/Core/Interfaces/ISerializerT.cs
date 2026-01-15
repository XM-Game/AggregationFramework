// ==========================================================
// 文件名：ISerializerT.cs
// 命名空间: AFramework.Serialization
// 依赖: System
// ==========================================================

using System;

namespace AFramework.Serialization
{
    /// <summary>
    /// 泛型序列化器接口
    /// <para>提供类型安全的序列化和反序列化操作</para>
    /// <para>编译时类型检查，避免装箱拆箱开销</para>
    /// </summary>
    /// <typeparam name="T">要序列化的类型</typeparam>
    /// <remarks>
    /// 使用示例:
    /// <code>
    /// ISerializer&lt;Player&gt; serializer = new BinarySerializer&lt;Player&gt;();
    /// 
    /// // 序列化
    /// byte[] data = serializer.Serialize(player);
    /// 
    /// // 反序列化
    /// Player player = serializer.Deserialize(data);
    /// 
    /// // 使用结果类型
    /// var result = serializer.DeserializeWithResult(data);
    /// if (result.IsSuccess)
    ///     ProcessPlayer(result.Value);
    /// </code>
    /// </remarks>
    public interface ISerializer<T>
    {
        #region 序列化方法

        /// <summary>
        /// 序列化对象为字节数组
        /// </summary>
        /// <param name="value">要序列化的对象</param>
        /// <returns>序列化后的字节数组</returns>
        byte[] Serialize(T value);

        /// <summary>
        /// 序列化对象为字节数组 (带选项)
        /// </summary>
        /// <param name="value">要序列化的对象</param>
        /// <param name="options">序列化选项</param>
        /// <returns>序列化后的字节数组</returns>
        byte[] Serialize(T value, SerializeOptions options);

        /// <summary>
        /// 序列化对象到缓冲区
        /// </summary>
        /// <param name="value">要序列化的对象</param>
        /// <param name="buffer">目标缓冲区</param>
        /// <returns>写入的字节数</returns>
        int Serialize(T value, Span<byte> buffer);

        /// <summary>
        /// 序列化对象到缓冲区 (带选项)
        /// </summary>
        /// <param name="value">要序列化的对象</param>
        /// <param name="buffer">目标缓冲区</param>
        /// <param name="options">序列化选项</param>
        /// <returns>写入的字节数</returns>
        int Serialize(T value, Span<byte> buffer, SerializeOptions options);

        /// <summary>
        /// 序列化对象并返回结果
        /// </summary>
        /// <param name="value">要序列化的对象</param>
        /// <param name="options">序列化选项</param>
        /// <returns>序列化结果</returns>
        SerializeResult SerializeWithResult(T value, SerializeOptions options = default);

        #endregion

        #region 反序列化方法

        /// <summary>
        /// 反序列化字节数组为对象
        /// </summary>
        /// <param name="data">序列化数据</param>
        /// <returns>反序列化的对象</returns>
        T Deserialize(ReadOnlySpan<byte> data);

        /// <summary>
        /// 反序列化字节数组为对象 (带选项)
        /// </summary>
        /// <param name="data">序列化数据</param>
        /// <param name="options">反序列化选项</param>
        /// <returns>反序列化的对象</returns>
        T Deserialize(ReadOnlySpan<byte> data, DeserializeOptions options);

        /// <summary>
        /// 反序列化并返回结果
        /// </summary>
        /// <param name="data">序列化数据</param>
        /// <param name="options">反序列化选项</param>
        /// <returns>反序列化结果</returns>
        DeserializeResult<T> DeserializeWithResult(ReadOnlySpan<byte> data, DeserializeOptions options = default);

        /// <summary>
        /// 反序列化到现有对象
        /// </summary>
        /// <param name="data">序列化数据</param>
        /// <param name="target">目标对象</param>
        void DeserializeTo(ReadOnlySpan<byte> data, ref T target);

        /// <summary>
        /// 反序列化到现有对象 (带选项)
        /// </summary>
        /// <param name="data">序列化数据</param>
        /// <param name="target">目标对象</param>
        /// <param name="options">反序列化选项</param>
        void DeserializeTo(ReadOnlySpan<byte> data, ref T target, DeserializeOptions options);

        #endregion

        #region 辅助方法

        /// <summary>
        /// 获取序列化后的预估大小
        /// </summary>
        /// <param name="value">要序列化的对象</param>
        /// <returns>预估的字节数</returns>
        int GetSerializedSize(T value);

        /// <summary>
        /// 尝试序列化对象
        /// </summary>
        /// <param name="value">要序列化的对象</param>
        /// <param name="buffer">目标缓冲区</param>
        /// <param name="bytesWritten">写入的字节数</param>
        /// <returns>如果成功返回 true</returns>
        bool TrySerialize(T value, Span<byte> buffer, out int bytesWritten);

        /// <summary>
        /// 尝试反序列化数据
        /// </summary>
        /// <param name="data">序列化数据</param>
        /// <param name="value">输出值</param>
        /// <returns>如果成功返回 true</returns>
        bool TryDeserialize(ReadOnlySpan<byte> data, out T value);

        #endregion
    }

    /// <summary>
    /// 泛型序列化器扩展方法
    /// </summary>
    public static class SerializerExtensions
    {
        /// <summary>
        /// 从字节数组反序列化
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <param name="serializer">序列化器</param>
        /// <param name="data">字节数组</param>
        /// <returns>反序列化的对象</returns>
        public static T Deserialize<T>(this ISerializer<T> serializer, byte[] data)
        {
            return serializer.Deserialize(data.AsSpan());
        }

        /// <summary>
        /// 克隆对象 (通过序列化/反序列化)
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="serializer">序列化器</param>
        /// <param name="value">要克隆的对象</param>
        /// <returns>克隆的对象</returns>
        public static T Clone<T>(this ISerializer<T> serializer, T value)
        {
            var data = serializer.Serialize(value);
            return serializer.Deserialize(data);
        }

        /// <summary>
        /// 序列化到 Base64 字符串
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="serializer">序列化器</param>
        /// <param name="value">要序列化的对象</param>
        /// <returns>Base64 编码的字符串</returns>
        public static string SerializeToBase64<T>(this ISerializer<T> serializer, T value)
        {
            var data = serializer.Serialize(value);
            return Convert.ToBase64String(data);
        }

        /// <summary>
        /// 从 Base64 字符串反序列化
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <param name="serializer">序列化器</param>
        /// <param name="base64">Base64 编码的字符串</param>
        /// <returns>反序列化的对象</returns>
        public static T DeserializeFromBase64<T>(this ISerializer<T> serializer, string base64)
        {
            var data = Convert.FromBase64String(base64);
            return serializer.Deserialize(data);
        }
    }
}
