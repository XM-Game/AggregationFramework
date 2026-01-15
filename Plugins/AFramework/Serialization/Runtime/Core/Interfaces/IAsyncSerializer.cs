// ==========================================================
// 文件名：IAsyncSerializer.cs
// 命名空间: AFramework.Serialization
// 依赖: System, System.IO, System.Threading
// ==========================================================

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace AFramework.Serialization
{
    /// <summary>
    /// 异步序列化器接口
    /// <para>提供异步序列化和反序列化操作</para>
    /// <para>支持流式处理、取消令牌和进度报告</para>
    /// </summary>
    /// <remarks>
    /// 使用示例:
    /// <code>
    /// IAsyncSerializer serializer = new AsyncBinarySerializer();
    /// 
    /// // 异步序列化到流
    /// await serializer.SerializeAsync(stream, player, cancellationToken);
    /// 
    /// // 异步反序列化
    /// var player = await serializer.DeserializeAsync&lt;Player&gt;(stream);
    /// 
    /// // 带进度报告
    /// var progress = new Progress&lt;float&gt;(p => Console.WriteLine($"进度: {p:P0}"));
    /// await serializer.SerializeAsync(stream, largeData, progress: progress);
    /// </code>
    /// </remarks>
    public interface IAsyncSerializer
    {
        #region 异步序列化方法

        /// <summary>
        /// 异步序列化对象到流
        /// </summary>
        /// <param name="stream">目标流</param>
        /// <param name="value">要序列化的对象</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>异步任务</returns>
        Task SerializeAsync(Stream stream, object value, CancellationToken cancellationToken = default);

        /// <summary>
        /// 异步序列化对象到流 (带选项)
        /// </summary>
        /// <param name="stream">目标流</param>
        /// <param name="value">要序列化的对象</param>
        /// <param name="options">序列化选项</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>异步任务</returns>
        Task SerializeAsync(Stream stream, object value, SerializeOptions options, CancellationToken cancellationToken = default);

        /// <summary>
        /// 异步序列化对象到流 (带进度报告)
        /// </summary>
        /// <param name="stream">目标流</param>
        /// <param name="value">要序列化的对象</param>
        /// <param name="options">序列化选项</param>
        /// <param name="progress">进度报告器 (0.0 - 1.0)</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>异步任务</returns>
        Task SerializeAsync(Stream stream, object value, SerializeOptions options, IProgress<float> progress, CancellationToken cancellationToken = default);

        /// <summary>
        /// 异步序列化对象为字节数组
        /// </summary>
        /// <param name="value">要序列化的对象</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>序列化后的字节数组</returns>
        Task<byte[]> SerializeAsync(object value, CancellationToken cancellationToken = default);

        /// <summary>
        /// 异步序列化对象并返回结果
        /// </summary>
        /// <param name="value">要序列化的对象</param>
        /// <param name="options">序列化选项</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>序列化结果</returns>
        Task<SerializeResult> SerializeWithResultAsync(object value, SerializeOptions options = default, CancellationToken cancellationToken = default);

        #endregion

        #region 异步反序列化方法

        /// <summary>
        /// 异步从流反序列化对象
        /// </summary>
        /// <param name="stream">源流</param>
        /// <param name="type">目标类型</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>反序列化的对象</returns>
        Task<object> DeserializeAsync(Stream stream, Type type, CancellationToken cancellationToken = default);

        /// <summary>
        /// 异步从流反序列化对象 (带选项)
        /// </summary>
        /// <param name="stream">源流</param>
        /// <param name="type">目标类型</param>
        /// <param name="options">反序列化选项</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>反序列化的对象</returns>
        Task<object> DeserializeAsync(Stream stream, Type type, DeserializeOptions options, CancellationToken cancellationToken = default);

        /// <summary>
        /// 异步从流反序列化对象 (带进度报告)
        /// </summary>
        /// <param name="stream">源流</param>
        /// <param name="type">目标类型</param>
        /// <param name="options">反序列化选项</param>
        /// <param name="progress">进度报告器 (0.0 - 1.0)</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>反序列化的对象</returns>
        Task<object> DeserializeAsync(Stream stream, Type type, DeserializeOptions options, IProgress<float> progress, CancellationToken cancellationToken = default);

        /// <summary>
        /// 异步从字节数组反序列化对象
        /// </summary>
        /// <param name="data">序列化数据</param>
        /// <param name="type">目标类型</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>反序列化的对象</returns>
        Task<object> DeserializeAsync(byte[] data, Type type, CancellationToken cancellationToken = default);

        #endregion
    }

    /// <summary>
    /// 泛型异步序列化器接口
    /// <para>提供类型安全的异步序列化和反序列化操作</para>
    /// </summary>
    /// <typeparam name="T">要序列化的类型</typeparam>
    public interface IAsyncSerializer<T>
    {
        #region 异步序列化方法

        /// <summary>
        /// 异步序列化对象到流
        /// </summary>
        /// <param name="stream">目标流</param>
        /// <param name="value">要序列化的对象</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>异步任务</returns>
        Task SerializeAsync(Stream stream, T value, CancellationToken cancellationToken = default);

        /// <summary>
        /// 异步序列化对象到流 (带选项)
        /// </summary>
        /// <param name="stream">目标流</param>
        /// <param name="value">要序列化的对象</param>
        /// <param name="options">序列化选项</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>异步任务</returns>
        Task SerializeAsync(Stream stream, T value, SerializeOptions options, CancellationToken cancellationToken = default);

        /// <summary>
        /// 异步序列化对象为字节数组
        /// </summary>
        /// <param name="value">要序列化的对象</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>序列化后的字节数组</returns>
        Task<byte[]> SerializeAsync(T value, CancellationToken cancellationToken = default);

        /// <summary>
        /// 异步序列化对象并返回结果
        /// </summary>
        /// <param name="value">要序列化的对象</param>
        /// <param name="options">序列化选项</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>序列化结果</returns>
        Task<SerializeResult> SerializeWithResultAsync(T value, SerializeOptions options = default, CancellationToken cancellationToken = default);

        #endregion

        #region 异步反序列化方法

        /// <summary>
        /// 异步从流反序列化对象
        /// </summary>
        /// <param name="stream">源流</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>反序列化的对象</returns>
        Task<T> DeserializeAsync(Stream stream, CancellationToken cancellationToken = default);

        /// <summary>
        /// 异步从流反序列化对象 (带选项)
        /// </summary>
        /// <param name="stream">源流</param>
        /// <param name="options">反序列化选项</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>反序列化的对象</returns>
        Task<T> DeserializeAsync(Stream stream, DeserializeOptions options, CancellationToken cancellationToken = default);

        /// <summary>
        /// 异步从字节数组反序列化对象
        /// </summary>
        /// <param name="data">序列化数据</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>反序列化的对象</returns>
        Task<T> DeserializeAsync(byte[] data, CancellationToken cancellationToken = default);

        /// <summary>
        /// 异步反序列化并返回结果
        /// </summary>
        /// <param name="stream">源流</param>
        /// <param name="options">反序列化选项</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>反序列化结果</returns>
        Task<DeserializeResult<T>> DeserializeWithResultAsync(Stream stream, DeserializeOptions options = default, CancellationToken cancellationToken = default);

        #endregion
    }

    /// <summary>
    /// 异步序列化器扩展方法
    /// </summary>
    public static class AsyncSerializerExtensions
    {
        /// <summary>
        /// 异步序列化到文件
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="serializer">序列化器</param>
        /// <param name="filePath">文件路径</param>
        /// <param name="value">要序列化的对象</param>
        /// <param name="cancellationToken">取消令牌</param>
        public static async Task SerializeToFileAsync<T>(this IAsyncSerializer<T> serializer, string filePath, T value, CancellationToken cancellationToken = default)
        {
            using var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None, 4096, FileOptions.Asynchronous);
            await serializer.SerializeAsync(stream, value, cancellationToken);
        }

        /// <summary>
        /// 异步从文件反序列化
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <param name="serializer">序列化器</param>
        /// <param name="filePath">文件路径</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>反序列化的对象</returns>
        public static async Task<T> DeserializeFromFileAsync<T>(this IAsyncSerializer<T> serializer, string filePath, CancellationToken cancellationToken = default)
        {
            using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, FileOptions.Asynchronous);
            return await serializer.DeserializeAsync(stream, cancellationToken);
        }
    }
}
