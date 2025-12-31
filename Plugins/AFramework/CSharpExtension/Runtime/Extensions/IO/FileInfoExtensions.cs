// ==========================================================
// 文件名：FileInfoExtensions.cs
// 命名空间: AFramework.CSharpExtension
// 依赖: System, System.IO, System.Text
// ==========================================================

using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AFramework.CSharpExtension
{
    /// <summary>
    /// FileInfo 扩展方法
    /// <para>提供文件信息的常用操作扩展，包括读写、复制、哈希计算等功能</para>
    /// </summary>
    public static class FileInfoExtensions
    {
        #region 读取操作

        /// <summary>
        /// 读取文件的所有文本
        /// </summary>
        public static string ReadAllText(this FileInfo file, Encoding encoding = null)
        {
            if (file == null || !file.Exists) return string.Empty;
            
            encoding = encoding ?? Encoding.UTF8;
            return File.ReadAllText(file.FullName, encoding);
        }

        /// <summary>
        /// 异步读取文件的所有文本
        /// </summary>
        public static async Task<string> ReadAllTextAsync(this FileInfo file, Encoding encoding = null, CancellationToken cancellationToken = default)
        {
            if (file == null || !file.Exists) return string.Empty;
            
            encoding = encoding ?? Encoding.UTF8;
            using (var reader = new StreamReader(file.FullName, encoding))
            {
                return await reader.ReadToEndAsync();
            }
        }

        /// <summary>
        /// 读取文件的所有字节
        /// </summary>
        public static byte[] ReadAllBytes(this FileInfo file)
        {
            if (file == null || !file.Exists) return Array.Empty<byte>();
            return File.ReadAllBytes(file.FullName);
        }

        /// <summary>
        /// 异步读取文件的所有字节
        /// </summary>
        public static async Task<byte[]> ReadAllBytesAsync(this FileInfo file, CancellationToken cancellationToken = default)
        {
            if (file == null || !file.Exists) return Array.Empty<byte>();
            
            using (var stream = file.OpenRead())
            {
                var buffer = new byte[stream.Length];
                await stream.ReadAsync(buffer, 0, buffer.Length, cancellationToken);
                return buffer;
            }
        }

        /// <summary>
        /// 读取文件的所有行
        /// </summary>
        public static string[] ReadAllLines(this FileInfo file, Encoding encoding = null)
        {
            if (file == null || !file.Exists) return Array.Empty<string>();
            
            encoding = encoding ?? Encoding.UTF8;
            return File.ReadAllLines(file.FullName, encoding);
        }

        #endregion

        #region 写入操作

        /// <summary>
        /// 写入所有文本
        /// </summary>
        public static void WriteAllText(this FileInfo file, string contents, Encoding encoding = null)
        {
            if (file == null) return;
            
            encoding = encoding ?? Encoding.UTF8;
            File.WriteAllText(file.FullName, contents ?? string.Empty, encoding);
            file.Refresh();
        }

        /// <summary>
        /// 异步写入所有文本
        /// </summary>
        public static async Task WriteAllTextAsync(this FileInfo file, string contents, Encoding encoding = null, CancellationToken cancellationToken = default)
        {
            if (file == null) return;
            
            encoding = encoding ?? Encoding.UTF8;
            using (var writer = new StreamWriter(file.FullName, false, encoding))
            {
                await writer.WriteAsync(contents ?? string.Empty);
            }
            file.Refresh();
        }

        /// <summary>
        /// 写入所有字节
        /// </summary>
        public static void WriteAllBytes(this FileInfo file, byte[] bytes)
        {
            if (file == null) return;
            
            File.WriteAllBytes(file.FullName, bytes ?? Array.Empty<byte>());
            file.Refresh();
        }

        /// <summary>
        /// 追加文本
        /// </summary>
        public static void AppendText(this FileInfo file, string contents, Encoding encoding = null)
        {
            if (file == null || string.IsNullOrEmpty(contents)) return;
            
            encoding = encoding ?? Encoding.UTF8;
            File.AppendAllText(file.FullName, contents, encoding);
            file.Refresh();
        }

        /// <summary>
        /// 追加一行文本
        /// </summary>
        public static void AppendLine(this FileInfo file, string line, Encoding encoding = null)
        {
            file.AppendText((line ?? string.Empty) + Environment.NewLine, encoding);
        }

        #endregion

        #region 文件操作

        /// <summary>
        /// 复制文件到指定路径
        /// </summary>
        public static FileInfo CopyToPath(this FileInfo file, string destPath, bool overwrite = false)
        {
            if (file == null || !file.Exists || string.IsNullOrEmpty(destPath))
                return null;
            
            var destDir = Path.GetDirectoryName(destPath);
            if (!string.IsNullOrEmpty(destDir) && !Directory.Exists(destDir))
                Directory.CreateDirectory(destDir);
            
            return file.CopyTo(destPath, overwrite);
        }

        /// <summary>
        /// 移动文件到指定路径
        /// </summary>
        public static void MoveToPath(this FileInfo file, string destPath, bool overwrite = false)
        {
            if (file == null || !file.Exists || string.IsNullOrEmpty(destPath))
                return;
            
            var destDir = Path.GetDirectoryName(destPath);
            if (!string.IsNullOrEmpty(destDir) && !Directory.Exists(destDir))
                Directory.CreateDirectory(destDir);
            
            if (overwrite && File.Exists(destPath))
                File.Delete(destPath);
            
            file.MoveTo(destPath);
        }

        /// <summary>
        /// 重命名文件
        /// </summary>
        public static FileInfo Rename(this FileInfo file, string newName)
        {
            if (file == null || !file.Exists || string.IsNullOrEmpty(newName))
                return file;
            
            var newPath = Path.Combine(file.DirectoryName ?? string.Empty, newName);
            file.MoveTo(newPath);
            return new FileInfo(newPath);
        }

        /// <summary>
        /// 安全删除文件
        /// </summary>
        public static bool SafeDelete(this FileInfo file)
        {
            if (file == null || !file.Exists) return false;
            
            try
            {
                file.Delete();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 确保目录存在
        /// </summary>
        public static FileInfo EnsureDirectoryExists(this FileInfo file)
        {
            if (file?.Directory != null && !file.Directory.Exists)
                file.Directory.Create();
            return file;
        }

        #endregion

        #region 文件信息

        /// <summary>
        /// 获取文件大小的友好显示
        /// </summary>
        public static string GetFileSizeString(this FileInfo file)
        {
            if (file == null || !file.Exists) return "0 B";
            
            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            double len = file.Length;
            int order = 0;
            
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len /= 1024;
            }
            
            return $"{len:0.##} {sizes[order]}";
        }

        /// <summary>
        /// 获取文件大小（KB）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double GetSizeInKB(this FileInfo file)
        {
            return file != null && file.Exists ? file.Length / 1024.0 : 0;
        }

        /// <summary>
        /// 获取文件大小（MB）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double GetSizeInMB(this FileInfo file)
        {
            return file != null && file.Exists ? file.Length / (1024.0 * 1024.0) : 0;
        }

        /// <summary>
        /// 检查文件是否为空
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsEmpty(this FileInfo file)
        {
            return file == null || !file.Exists || file.Length == 0;
        }

        /// <summary>
        /// 检查文件是否只读
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsReadOnlyFile(this FileInfo file)
        {
            return file != null && file.Exists && file.IsReadOnly;
        }

        /// <summary>
        /// 检查文件是否被锁定
        /// </summary>
        public static bool IsLocked(this FileInfo file)
        {
            if (file == null || !file.Exists) return false;
            
            try
            {
                using (file.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                {
                    return false;
                }
            }
            catch (IOException)
            {
                return true;
            }
        }

        #endregion

        #region 哈希计算

        /// <summary>
        /// 计算文件的 MD5 哈希值
        /// </summary>
        public static string ComputeMD5Hash(this FileInfo file)
        {
            if (file == null || !file.Exists) return string.Empty;
            
            using (var md5 = MD5.Create())
            using (var stream = file.OpenRead())
            {
                var hash = md5.ComputeHash(stream);
                return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
            }
        }

        /// <summary>
        /// 计算文件的 SHA256 哈希值
        /// </summary>
        public static string ComputeSHA256Hash(this FileInfo file)
        {
            if (file == null || !file.Exists) return string.Empty;
            
            using (var sha256 = SHA256.Create())
            using (var stream = file.OpenRead())
            {
                var hash = sha256.ComputeHash(stream);
                return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
            }
        }

        /// <summary>
        /// 比较两个文件内容是否相同
        /// </summary>
        public static bool ContentEquals(this FileInfo file1, FileInfo file2)
        {
            if (file1 == null || file2 == null) return false;
            if (!file1.Exists || !file2.Exists) return false;
            if (file1.Length != file2.Length) return false;
            
            return file1.ComputeMD5Hash() == file2.ComputeMD5Hash();
        }

        #endregion

        #region 扩展名检查

        /// <summary>
        /// 检查是否为指定扩展名
        /// </summary>
        public static bool HasExtension(this FileInfo file, string extension)
        {
            if (file == null || string.IsNullOrEmpty(extension)) return false;
            
            if (!extension.StartsWith("."))
                extension = "." + extension;
            
            return file.Extension.Equals(extension, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// 检查是否为图片文件
        /// </summary>
        public static bool IsImageFile(this FileInfo file)
        {
            return file != null && file.FullName.IsImageFile();
        }

        /// <summary>
        /// 检查是否为文本文件
        /// </summary>
        public static bool IsTextFile(this FileInfo file)
        {
            return file != null && file.FullName.IsTextFile();
        }

        #endregion

        #region 时间操作

        /// <summary>
        /// 获取文件年龄（从创建到现在的时间）
        /// </summary>
        public static TimeSpan GetAge(this FileInfo file)
        {
            if (file == null || !file.Exists) return TimeSpan.Zero;
            return DateTime.Now - file.CreationTime;
        }

        /// <summary>
        /// 获取自上次修改以来的时间
        /// </summary>
        public static TimeSpan GetTimeSinceLastModified(this FileInfo file)
        {
            if (file == null || !file.Exists) return TimeSpan.Zero;
            return DateTime.Now - file.LastWriteTime;
        }

        /// <summary>
        /// 检查文件是否在指定时间内被修改
        /// </summary>
        public static bool WasModifiedWithin(this FileInfo file, TimeSpan timeSpan)
        {
            if (file == null || !file.Exists) return false;
            return file.GetTimeSinceLastModified() <= timeSpan;
        }

        #endregion
    }
}
