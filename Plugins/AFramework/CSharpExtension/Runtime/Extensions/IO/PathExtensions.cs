// ==========================================================
// 文件名：PathExtensions.cs
// 命名空间: AFramework.CSharpExtension
// 依赖: System, System.IO
// ==========================================================

using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace AFramework.CSharpExtension
{
    /// <summary>
    /// 路径字符串扩展方法
    /// <para>提供路径的常用操作扩展，包括路径组合、规范化、验证等功能</para>
    /// </summary>
    public static class PathExtensions
    {
        #region 路径组合

        /// <summary>
        /// 组合路径
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string CombinePath(this string path, string path2)
        {
            if (string.IsNullOrEmpty(path)) return path2 ?? string.Empty;
            if (string.IsNullOrEmpty(path2)) return path;
            return Path.Combine(path, path2);
        }

        /// <summary>
        /// 组合多个路径
        /// </summary>
        public static string CombinePath(this string path, params string[] paths)
        {
            if (string.IsNullOrEmpty(path)) return string.Empty;
            if (paths == null || paths.Length == 0) return path;
            
            var result = path;
            foreach (var p in paths)
            {
                if (!string.IsNullOrEmpty(p))
                    result = Path.Combine(result, p);
            }
            return result;
        }

        #endregion

        #region 路径信息获取

        /// <summary>
        /// 获取文件名（包含扩展名）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetFileName(this string path)
        {
            return string.IsNullOrEmpty(path) ? string.Empty : Path.GetFileName(path);
        }

        /// <summary>
        /// 获取文件名（不包含扩展名）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetFileNameWithoutExtension(this string path)
        {
            return string.IsNullOrEmpty(path) ? string.Empty : Path.GetFileNameWithoutExtension(path);
        }

        /// <summary>
        /// 获取扩展名（包含点号）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetExtension(this string path)
        {
            return string.IsNullOrEmpty(path) ? string.Empty : Path.GetExtension(path);
        }

        /// <summary>
        /// 获取扩展名（不包含点号）
        /// </summary>
        public static string GetExtensionWithoutDot(this string path)
        {
            var ext = path.GetExtension();
            return ext.StartsWith(".") ? ext.Substring(1) : ext;
        }

        /// <summary>
        /// 获取目录路径
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetDirectoryPath(this string path)
        {
            return string.IsNullOrEmpty(path) ? string.Empty : Path.GetDirectoryName(path) ?? string.Empty;
        }

        /// <summary>
        /// 获取目录名称
        /// </summary>
        public static string GetDirectoryName(this string path)
        {
            var dirPath = path.GetDirectoryPath();
            return string.IsNullOrEmpty(dirPath) ? string.Empty : Path.GetFileName(dirPath);
        }

        /// <summary>
        /// 获取根目录
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetPathRoot(this string path)
        {
            return string.IsNullOrEmpty(path) ? string.Empty : Path.GetPathRoot(path) ?? string.Empty;
        }

        #endregion

        #region 路径修改

        /// <summary>
        /// 更改扩展名
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ChangeExtension(this string path, string extension)
        {
            return string.IsNullOrEmpty(path) ? string.Empty : Path.ChangeExtension(path, extension);
        }

        /// <summary>
        /// 添加扩展名（如果没有）
        /// </summary>
        public static string EnsureExtension(this string path, string extension)
        {
            if (string.IsNullOrEmpty(path)) return string.Empty;
            if (string.IsNullOrEmpty(extension)) return path;
            
            if (!extension.StartsWith("."))
                extension = "." + extension;
            
            return path.EndsWith(extension, StringComparison.OrdinalIgnoreCase) ? path : path + extension;
        }

        /// <summary>
        /// 移除扩展名
        /// </summary>
        public static string RemoveExtension(this string path)
        {
            if (string.IsNullOrEmpty(path)) return string.Empty;
            
            var ext = Path.GetExtension(path);
            return string.IsNullOrEmpty(ext) ? path : path.Substring(0, path.Length - ext.Length);
        }

        /// <summary>
        /// 添加后缀到文件名（扩展名之前）
        /// </summary>
        public static string AddFileNameSuffix(this string path, string suffix)
        {
            if (string.IsNullOrEmpty(path) || string.IsNullOrEmpty(suffix)) return path ?? string.Empty;
            
            var dir = Path.GetDirectoryName(path) ?? string.Empty;
            var name = Path.GetFileNameWithoutExtension(path);
            var ext = Path.GetExtension(path);
            
            var newName = name + suffix + ext;
            return string.IsNullOrEmpty(dir) ? newName : Path.Combine(dir, newName);
        }

        /// <summary>
        /// 添加前缀到文件名
        /// </summary>
        public static string AddFileNamePrefix(this string path, string prefix)
        {
            if (string.IsNullOrEmpty(path) || string.IsNullOrEmpty(prefix)) return path ?? string.Empty;
            
            var dir = Path.GetDirectoryName(path) ?? string.Empty;
            var name = Path.GetFileName(path);
            
            var newName = prefix + name;
            return string.IsNullOrEmpty(dir) ? newName : Path.Combine(dir, newName);
        }

        #endregion

        #region 路径规范化

        /// <summary>
        /// 规范化路径分隔符（使用系统默认分隔符）
        /// </summary>
        public static string NormalizeSeparators(this string path)
        {
            if (string.IsNullOrEmpty(path)) return string.Empty;
            return path.Replace('/', Path.DirectorySeparatorChar)
                       .Replace('\\', Path.DirectorySeparatorChar);
        }

        /// <summary>
        /// 转换为 Unix 风格路径（使用正斜杠）
        /// </summary>
        public static string ToUnixPath(this string path)
        {
            return string.IsNullOrEmpty(path) ? string.Empty : path.Replace('\\', '/');
        }

        /// <summary>
        /// 转换为 Windows 风格路径（使用反斜杠）
        /// </summary>
        public static string ToWindowsPath(this string path)
        {
            return string.IsNullOrEmpty(path) ? string.Empty : path.Replace('/', '\\');
        }

        /// <summary>
        /// 移除末尾的路径分隔符
        /// </summary>
        public static string TrimEndSeparator(this string path)
        {
            if (string.IsNullOrEmpty(path)) return string.Empty;
            return path.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        }

        /// <summary>
        /// 确保末尾有路径分隔符
        /// </summary>
        public static string EnsureEndSeparator(this string path)
        {
            if (string.IsNullOrEmpty(path)) return string.Empty;
            
            if (!path.EndsWith(Path.DirectorySeparatorChar.ToString()) && 
                !path.EndsWith(Path.AltDirectorySeparatorChar.ToString()))
                return path + Path.DirectorySeparatorChar;
            
            return path;
        }

        /// <summary>
        /// 获取完整路径
        /// </summary>
        public static string GetFullPath(this string path)
        {
            if (string.IsNullOrEmpty(path)) return string.Empty;
            
            try { return Path.GetFullPath(path); }
            catch { return path; }
        }

        #endregion

        #region 路径验证

        /// <summary>
        /// 检查是否为有效路径
        /// </summary>
        public static bool IsValidPath(this string path)
        {
            if (string.IsNullOrWhiteSpace(path)) return false;
            
            try
            {
                var invalidChars = Path.GetInvalidPathChars();
                foreach (var c in invalidChars)
                {
                    if (path.Contains(c.ToString()))
                        return false;
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 检查是否为有效文件名
        /// </summary>
        public static bool IsValidFileName(this string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName)) return false;
            
            try
            {
                var invalidChars = Path.GetInvalidFileNameChars();
                foreach (var c in invalidChars)
                {
                    if (fileName.Contains(c.ToString()))
                        return false;
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 检查是否为绝对路径
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsAbsolutePath(this string path)
        {
            return !string.IsNullOrEmpty(path) && Path.IsPathRooted(path);
        }

        /// <summary>
        /// 检查是否为相对路径
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsRelativePath(this string path)
        {
            return !string.IsNullOrEmpty(path) && !Path.IsPathRooted(path);
        }

        /// <summary>
        /// 检查路径是否存在（文件或目录）
        /// </summary>
        public static bool PathExists(this string path)
        {
            if (string.IsNullOrEmpty(path)) return false;
            return File.Exists(path) || Directory.Exists(path);
        }

        /// <summary>
        /// 检查文件是否存在
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool FileExists(this string path)
        {
            return !string.IsNullOrEmpty(path) && File.Exists(path);
        }

        /// <summary>
        /// 检查目录是否存在
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool DirectoryExists(this string path)
        {
            return !string.IsNullOrEmpty(path) && Directory.Exists(path);
        }

        #endregion

        #region 路径清理

        /// <summary>
        /// 移除路径中的非法字符
        /// </summary>
        public static string RemoveInvalidPathChars(this string path)
        {
            if (string.IsNullOrEmpty(path)) return string.Empty;
            
            var invalidChars = Path.GetInvalidPathChars();
            foreach (var c in invalidChars)
                path = path.Replace(c.ToString(), string.Empty);
            
            return path;
        }

        /// <summary>
        /// 移除文件名中的非法字符
        /// </summary>
        public static string RemoveInvalidFileNameChars(this string fileName)
        {
            if (string.IsNullOrEmpty(fileName)) return string.Empty;
            
            var invalidChars = Path.GetInvalidFileNameChars();
            foreach (var c in invalidChars)
                fileName = fileName.Replace(c.ToString(), string.Empty);
            
            return fileName;
        }

        /// <summary>
        /// 替换文件名中的非法字符
        /// </summary>
        public static string ReplaceInvalidFileNameChars(this string fileName, char replacement = '_')
        {
            if (string.IsNullOrEmpty(fileName)) return string.Empty;
            
            var invalidChars = Path.GetInvalidFileNameChars();
            foreach (var c in invalidChars)
                fileName = fileName.Replace(c, replacement);
            
            return fileName;
        }

        #endregion

        #region 相对路径

        /// <summary>
        /// 获取相对路径
        /// </summary>
        public static string GetRelativePath(this string path, string basePath)
        {
            if (string.IsNullOrEmpty(path) || string.IsNullOrEmpty(basePath))
                return path ?? string.Empty;
            
            try
            {
                var pathUri = new Uri(Path.GetFullPath(path));
                var baseUri = new Uri(Path.GetFullPath(basePath).EnsureEndSeparator());
                
                var relativeUri = baseUri.MakeRelativeUri(pathUri);
                return Uri.UnescapeDataString(relativeUri.ToString()).Replace('/', Path.DirectorySeparatorChar);
            }
            catch
            {
                return path;
            }
        }

        /// <summary>
        /// 转换为绝对路径
        /// </summary>
        public static string ToAbsolutePath(this string relativePath, string basePath)
        {
            if (string.IsNullOrEmpty(relativePath)) return string.Empty;
            if (relativePath.IsAbsolutePath()) return relativePath;
            if (string.IsNullOrEmpty(basePath)) return relativePath;
            
            return Path.GetFullPath(Path.Combine(basePath, relativePath));
        }

        #endregion

        #region 扩展名检查

        /// <summary>
        /// 检查是否具有指定扩展名
        /// </summary>
        public static bool HasExtension(this string path, string extension)
        {
            if (string.IsNullOrEmpty(path) || string.IsNullOrEmpty(extension))
                return false;
            
            if (!extension.StartsWith("."))
                extension = "." + extension;
            
            return path.EndsWith(extension, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// 检查是否具有任意指定扩展名
        /// </summary>
        public static bool HasAnyExtension(this string path, params string[] extensions)
        {
            if (string.IsNullOrEmpty(path) || extensions == null) return false;
            
            foreach (var ext in extensions)
            {
                if (path.HasExtension(ext))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// 检查是否为图片文件
        /// </summary>
        public static bool IsImageFile(this string path)
        {
            return path.HasAnyExtension(".png", ".jpg", ".jpeg", ".gif", ".bmp", ".tiff", ".webp", ".ico");
        }

        /// <summary>
        /// 检查是否为文本文件
        /// </summary>
        public static bool IsTextFile(this string path)
        {
            return path.HasAnyExtension(".txt", ".md", ".json", ".xml", ".csv", ".log", ".ini", ".cfg");
        }

        /// <summary>
        /// 检查是否为代码文件
        /// </summary>
        public static bool IsCodeFile(this string path)
        {
            return path.HasAnyExtension(".cs", ".js", ".ts", ".py", ".java", ".cpp", ".c", ".h", ".hpp");
        }

        #endregion
    }
}
