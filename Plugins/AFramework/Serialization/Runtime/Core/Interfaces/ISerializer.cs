// ==========================================================
// 文件名：ISerializer.cs
// 命名空间: AFramework.Serialization
// 依赖: System
// ==========================================================

using System;

namespace AFramework.Serialization
{
    /// <summary>
    /// 序列化器接口
    /// <para>定义序列化和反序列化的核心操作</para>
    /// <para>提供非泛型 API，适用于运行时类型处理</para>
    /// </summary>
    /// <remarks>
    /// 使用示例:
    /// <code>
    /// ISerializer serializer = new BinarySerializer();
    /// 
    /// // 序列化
    /// byte[] data = serializer.Serialize(player);
    /// 
    /// // 反序列化
    /// var player = (Player)serializer.Deserialize(data, typeof(Player));
    /// 
    /// // 使用选项
    /// var result = serializer.Serialize(player, SerializeOptions.ForStorage);
    /// </code>
    /// </remarks>
    public interface ISerializer
    {
        #region 序列化方法

        /// <summary>
        /// 序列化对象为字节数组
        /// </summary>
        /// <param name="value">要序列化的对象</param>
        /// <returns>序列化后的字节数组</returns>
        byte[] Serialize(object value);

        /// <summary>
        /// 序列化对象为字节数组 (带选项)
        /// </summary>
        /// <param name="value">要序列化的对象</param>
        /// <param name="options">序列化选项</param>
        /// <returns>序列化后的字节数组</returns>
        byte[] Serialize(object value, SerializeOptions options);

        /// <summary>
        /// 序列化对象到缓冲区
        /// </summary>
        /// <param name="value">要序列化的对象</param>
        /// <param name="buffer">目标缓冲区</param>
        /// <returns>写入的字节数</returns>
        int Serialize(object value, Span<byte> buffer);

        /// <summary>
        /// 序列化对象到缓冲区 (带选项)
        /// </summary>
        /// <param name="value">要序列化的对象</param>
        /// <param name="buffer">目标缓冲区</param>
        /// <param name="options">序列化选项</param>
        /// <returns>写入的字节数</returns>
        int Serialize(object value, Span<byte> buffer, SerializeOptions options);

        /// <summary>
        /// 序列化对象并返回结果
        /// </summary>
        /// <param name="value">要序列化的对象</param>
        /// <param name="options">序列化选项</param>
        /// <returns>序列化结果</returns>
        SerializeResult SerializeWithResult(object value, SerializeOptions options = default);

        #endregion

        #region 反序列化方法

        /// <summary>
        /// 反序列化字节数组为对象
        /// </summary>
        /// <param name="data">序列化数据</param>
        /// <param name="type">目标类型</param>
        /// <returns>反序列化的对象</returns>
        object Deserialize(ReadOnlySpan<byte> data, Type type);

        /// <summary>
        /// 反序列化字节数组为对象 (带选项)
        /// </summary>
        /// <param name="data">序列化数据</param>
        /// <param name="type">目标类型</param>
        /// <param name="options">反序列化选项</param>
        /// <returns>反序列化的对象</returns>
        object Deserialize(ReadOnlySpan<byte> data, Type type, DeserializeOptions options);

        /// <summary>
        /// 反序列化到现有对象
        /// </summary>
        /// <param name="data">序列化数据</param>
        /// <param name="target">目标对象</param>
        void DeserializeTo(ReadOnlySpan<byte> data, object target);

        /// <summary>
        /// 反序列化到现有对象 (带选项)
        /// </summary>
        /// <param name="data">序列化数据</param>
        /// <param name="target">目标对象</param>
        /// <param name="options">反序列化选项</param>
        void DeserializeTo(ReadOnlySpan<byte> data, object target, DeserializeOptions options);

        #endregion

        #region 辅助方法

        /// <summary>
        /// 获取序列化后的预估大小
        /// </summary>
        /// <param name="value">要序列化的对象</param>
        /// <returns>预估的字节数</returns>
        int GetSerializedSize(object value);

        /// <summary>
        /// 检查类型是否可序列化
        /// </summary>
        /// <param name="type">要检查的类型</param>
        /// <returns>如果可序列化返回 true</returns>
        bool CanSerialize(Type type);

        #endregion

        #region 属性

        /// <summary>
        /// 获取默认序列化选项
        /// </summary>
        SerializeOptions DefaultSerializeOptions { get; }

        /// <summary>
        /// 获取默认反序列化选项
        /// </summary>
        DeserializeOptions DefaultDeserializeOptions { get; }

        #endregion
    }
}
