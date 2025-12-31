// ==========================================================
// 文件名：StreamExtensions.cs
// 命名空间: AFramework.CSharpExtension
// 依赖: System, System.IO, System.Text
// ==========================================================

using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AFramework.CSharpExtension
{
    /// <summary>
    /// Stream 扩展方法
    /// <para>提供流的常用操作扩展，包括读写、复制、转换等功能</para>
    /// </summary>
    public static class StreamExtensions
    {
        #region 常量

        /// <summary>
        /// 默认缓冲区大小（81920 字节 = 80KB）
        /// </summary>
        public const int DefaultBufferSize = 81920;

        #endregion

        #region 读取操作

        /// <summary>
        /// 读取流的所有字节
        /// </summary>
        public static byte[] ReadAllBytes(this Stream stream)
        {
            if (stream == null) return Array.Empty<byte>();
            
            if (stream is MemoryStream ms)
                return ms.ToArray();
            
            using (var memoryStream = new MemoryStream())
            {
                stream.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }

        /// <summary>
        /// 异步读取流的所有字节
        /// </summary>
        public static async Task<byte[]> ReadAllBytesAsync(this Stream stream, CancellationToken cancellationToken = default)
        {
            if (stream == null) return Array.Empty<byte>();
            
            if (stream is MemoryStream ms)
                return ms.ToArray();
            
            using (var memoryStream = new MemoryStream())
            {
                await stream.CopyToAsync(memoryStream, DefaultBufferSize, cancellationToken);
                return memoryStream.ToArray();
            }
        }

        /// <summary>
        /// 读取流为字符串
        /// </summary>
        public static string ReadAsString(this Stream stream, Encoding encoding = null)
        {
            if (stream == null) return string.Empty;
            
            encoding = encoding ?? Encoding.UTF8;
            using (var reader = new StreamReader(stream, encoding, true, DefaultBufferSize, true))
            {
                return reader.ReadToEnd();
            }
        }

        /// <summary>
        /// 异步读取流为字符串
        /// </summary>
        public static async Task<string> ReadAsStringAsync(this Stream stream, Encoding encoding = null)
        {
            if (stream == null) return string.Empty;
            
            encoding = encoding ?? Encoding.UTF8;
            using (var reader = new StreamReader(stream, encoding, true, DefaultBufferSize, true))
            {
                return await reader.ReadToEndAsync();
            }
        }

        /// <summary>
        /// 读取指定长度的字节
        /// </summary>
        public static byte[] ReadBytes(this Stream stream, int count)
        {
            if (stream == null || count <= 0) return Array.Empty<byte>();
            
            var buffer = new byte[count];
            int totalRead = 0;
            
            while (totalRead < count)
            {
                int read = stream.Read(buffer, totalRead, count - totalRead);
                if (read == 0) break;
                totalRead += read;
            }
            
            if (totalRead < count)
                Array.Resize(ref buffer, totalRead);
            
            return buffer;
        }

        /// <summary>
        /// 读取一行文本
        /// </summary>
        public static string ReadLine(this Stream stream, Encoding encoding = null)
        {
            if (stream == null) return null;
            
            encoding = encoding ?? Encoding.UTF8;
            using (var reader = new StreamReader(stream, encoding, true, DefaultBufferSize, true))
            {
                return reader.ReadLine();
            }
        }

        #endregion

        #region 写入操作

        /// <summary>
        /// 写入字节数组
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteBytes(this Stream stream, byte[] data)
        {
            if (stream == null || data == null || data.Length == 0) return;
            stream.Write(data, 0, data.Length);
        }

        /// <summary>
        /// 异步写入字节数组
        /// </summary>
        public static async Task WriteBytesAsync(this Stream stream, byte[] data, CancellationToken cancellationToken = default)
        {
            if (stream == null || data == null || data.Length == 0) return;
            await stream.WriteAsync(data, 0, data.Length, cancellationToken);
        }

        /// <summary>
        /// 写入字符串
        /// </summary>
        public static void WriteString(this Stream stream, string text, Encoding encoding = null)
        {
            if (stream == null || string.IsNullOrEmpty(text)) return;
            
            encoding = encoding ?? Encoding.UTF8;
            var bytes = encoding.GetBytes(text);
            stream.Write(bytes, 0, bytes.Length);
        }

        /// <summary>
        /// 异步写入字符串
        /// </summary>
        public static async Task WriteStringAsync(this Stream stream, string text, Encoding encoding = null, CancellationToken cancellationToken = default)
        {
            if (stream == null || string.IsNullOrEmpty(text)) return;
            
            encoding = encoding ?? Encoding.UTF8;
            var bytes = encoding.GetBytes(text);
            await stream.WriteAsync(bytes, 0, bytes.Length, cancellationToken);
        }

        /// <summary>
        /// 写入一行文本
        /// </summary>
        public static void WriteLine(this Stream stream, string text, Encoding encoding = null)
        {
            stream.WriteString(text + Environment.NewLine, encoding);
        }

        #endregion

        #region 复制操作

        /// <summary>
        /// 复制流到另一个流（带进度回调）
        /// </summary>
        public static void CopyTo(this Stream source, Stream destination, int bufferSize, Action<long> progressCallback)
        {
            if (source == null || destination == null) return;
            
            var buffer = new byte[bufferSize];
            long totalCopied = 0;
            int read;
            
            while ((read = source.Read(buffer, 0, buffer.Length)) > 0)
            {
                destination.Write(buffer, 0, read);
                totalCopied += read;
                progressCallback?.Invoke(totalCopied);
            }
        }

        /// <summary>
        /// 异步复制流到另一个流（带进度回调）
        /// </summary>
        public static async Task CopyToAsync(this Stream source, Stream destination, int bufferSize, 
            IProgress<long> progress, CancellationToken cancellationToken = default)
        {
            if (source == null || destination == null) return;
            
            var buffer = new byte[bufferSize];
            long totalCopied = 0;
            int read;
            
            while ((read = await source.ReadAsync(buffer, 0, buffer.Length, cancellationToken)) > 0)
            {
                await destination.WriteAsync(buffer, 0, read, cancellationToken);
                totalCopied += read;
                progress?.Report(totalCopied);
            }
        }

        #endregion

        #region 位置操作

        /// <summary>
        /// 重置流位置到开头
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Stream Reset(this Stream stream)
        {
            if (stream != null && stream.CanSeek)
                stream.Position = 0;
            return stream;
        }

        /// <summary>
        /// 移动到流末尾
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Stream SeekToEnd(this Stream stream)
        {
            if (stream != null && stream.CanSeek)
                stream.Seek(0, SeekOrigin.End);
            return stream;
        }

        /// <summary>
        /// 获取剩余可读字节数
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long GetRemainingLength(this Stream stream)
        {
            if (stream == null || !stream.CanSeek) return -1;
            return stream.Length - stream.Position;
        }

        /// <summary>
        /// 检查是否已到达流末尾
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsAtEnd(this Stream stream)
        {
            if (stream == null) return true;
            if (!stream.CanSeek) return false;
            return stream.Position >= stream.Length;
        }

        #endregion

        #region 转换操作

        /// <summary>
        /// 转换为 MemoryStream
        /// </summary>
        public static MemoryStream ToMemoryStream(this Stream stream)
        {
            if (stream == null) return new MemoryStream();
            
            if (stream is MemoryStream ms)
                return ms;
            
            var memoryStream = new MemoryStream();
            stream.CopyTo(memoryStream);
            memoryStream.Position = 0;
            return memoryStream;
        }

        /// <summary>
        /// 转换为 Base64 字符串
        /// </summary>
        public static string ToBase64String(this Stream stream)
        {
            if (stream == null) return string.Empty;
            
            var bytes = stream.ReadAllBytes();
            return Convert.ToBase64String(bytes);
        }

        #endregion

        #region 状态检查

        /// <summary>
        /// 检查流是否可用（非空且可读）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsReadable(this Stream stream)
        {
            return stream != null && stream.CanRead;
        }

        /// <summary>
        /// 检查流是否可写
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsWritable(this Stream stream)
        {
            return stream != null && stream.CanWrite;
        }

        /// <summary>
        /// 检查流是否可定位
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsSeekable(this Stream stream)
        {
            return stream != null && stream.CanSeek;
        }

        /// <summary>
        /// 检查流是否为空
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsEmpty(this Stream stream)
        {
            return stream == null || (stream.CanSeek && stream.Length == 0);
        }

        #endregion

        #region 安全操作

        /// <summary>
        /// 安全关闭流
        /// </summary>
        public static void SafeClose(this Stream stream)
        {
            try { stream?.Close(); }
            catch { /* 忽略异常 */ }
        }

        /// <summary>
        /// 安全释放流
        /// </summary>
        public static void SafeDispose(this Stream stream)
        {
            try { stream?.Dispose(); }
            catch { /* 忽略异常 */ }
        }

        /// <summary>
        /// 安全刷新流
        /// </summary>
        public static void SafeFlush(this Stream stream)
        {
            try { stream?.Flush(); }
            catch { /* 忽略异常 */ }
        }

        #endregion

        #region 比较操作

        /// <summary>
        /// 比较两个流的内容是否相同
        /// </summary>
        public static bool ContentEquals(this Stream stream1, Stream stream2)
        {
            if (stream1 == null && stream2 == null) return true;
            if (stream1 == null || stream2 == null) return false;
            
            if (stream1.CanSeek && stream2.CanSeek && stream1.Length != stream2.Length)
                return false;
            
            var buffer1 = new byte[DefaultBufferSize];
            var buffer2 = new byte[DefaultBufferSize];
            
            int read1, read2;
            while ((read1 = stream1.Read(buffer1, 0, buffer1.Length)) > 0)
            {
                read2 = stream2.Read(buffer2, 0, buffer2.Length);
                if (read1 != read2) return false;
                
                for (int i = 0; i < read1; i++)
                {
                    if (buffer1[i] != buffer2[i]) return false;
                }
            }
            
            return stream2.Read(buffer2, 0, 1) == 0;
        }

        #endregion
    }
}
