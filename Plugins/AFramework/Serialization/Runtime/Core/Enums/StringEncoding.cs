// ==========================================================
// 文件名：StringEncoding.cs
// 命名空间: AFramework.Serialization
// 依赖: System, System.Text
// ==========================================================

using System;
using System.Runtime.CompilerServices;
using System.Text;

namespace AFramework.Serialization
{
    /// <summary>
    /// 字符串编码枚举
    /// <para>定义序列化时字符串的编码方式</para>
    /// </summary>
    /// <remarks>
    /// 使用示例:
    /// <code>
    /// // 使用 UTF-8 编码
    /// var options = new SerializeOptions { StringEncoding = StringEncoding.UTF8 };
    /// 
    /// // 获取 .NET Encoding 对象
    /// var encoding = StringEncoding.UTF8.ToEncoding();
    /// </code>
    /// </remarks>
    public enum StringEncoding : byte
    {
        /// <summary>
        /// UTF-8 编码 (默认)
        /// <para>可变长度编码，ASCII兼容</para>
        /// <para>特点：紧凑、通用、网络友好</para>
        /// <para>适用：大多数场景、网络传输、存储</para>
        /// </summary>
        UTF8 = 0,

        /// <summary>
        /// UTF-16 LE 编码
        /// <para>固定2/4字节编码，小端序</para>
        /// <para>特点：.NET内部格式、Unicode完整支持</para>
        /// <para>适用：Unicode密集、本地处理</para>
        /// </summary>
        UTF16LE = 1,

        /// <summary>
        /// UTF-16 BE 编码
        /// <para>固定2/4字节编码，大端序</para>
        /// <para>特点：网络字节序、跨平台</para>
        /// <para>适用：跨平台Unicode交换</para>
        /// </summary>
        UTF16BE = 2,

        /// <summary>
        /// UTF-32 LE 编码
        /// <para>固定4字节编码，小端序</para>
        /// <para>特点：定长、简单索引、空间大</para>
        /// <para>适用：需要O(1)字符访问</para>
        /// </summary>
        UTF32LE = 3,

        /// <summary>
        /// UTF-32 BE 编码
        /// <para>固定4字节编码，大端序</para>
        /// </summary>
        UTF32BE = 4,

        /// <summary>
        /// ASCII 编码
        /// <para>7位编码，仅支持基本ASCII字符</para>
        /// <para>特点：最紧凑、仅英文、兼容性好</para>
        /// <para>适用：纯英文数据、标识符</para>
        /// </summary>
        ASCII = 5,

        /// <summary>
        /// Latin1 (ISO-8859-1) 编码
        /// <para>8位编码，支持西欧字符</para>
        /// <para>特点：单字节、西欧语言支持</para>
        /// <para>适用：西欧语言文本</para>
        /// </summary>
        Latin1 = 6,

        /// <summary>
        /// 自动检测
        /// <para>根据字符串内容自动选择最佳编码</para>
        /// </summary>
        Auto = 255
    }

    /// <summary>
    /// StringEncoding 扩展方法
    /// </summary>
    public static class StringEncodingExtensions
    {
        #region 编码转换方法

        /// <summary>
        /// 转换为 .NET Encoding 对象
        /// </summary>
        public static Encoding ToEncoding(this StringEncoding encoding)
        {
            return encoding switch
            {
                StringEncoding.UTF8 => Encoding.UTF8,
                StringEncoding.UTF16LE => Encoding.Unicode,
                StringEncoding.UTF16BE => Encoding.BigEndianUnicode,
                StringEncoding.UTF32LE => Encoding.UTF32,
                StringEncoding.UTF32BE => new UTF32Encoding(bigEndian: true, byteOrderMark: false),
                StringEncoding.ASCII => Encoding.ASCII,
                StringEncoding.Latin1 => Encoding.GetEncoding("ISO-8859-1"),
                StringEncoding.Auto => Encoding.UTF8,
                _ => Encoding.UTF8
            };
        }

        /// <summary>
        /// 从 .NET Encoding 转换
        /// </summary>
        public static StringEncoding FromEncoding(Encoding encoding)
        {
            if (encoding == null) return StringEncoding.UTF8;
            
            var name = encoding.WebName.ToUpperInvariant();
            return name switch
            {
                "UTF-8" => StringEncoding.UTF8,
                "UTF-16" or "UTF-16LE" => StringEncoding.UTF16LE,
                "UTF-16BE" or "UNICODEFFFE" => StringEncoding.UTF16BE,
                "UTF-32" or "UTF-32LE" => StringEncoding.UTF32LE,
                "UTF-32BE" => StringEncoding.UTF32BE,
                "US-ASCII" or "ASCII" => StringEncoding.ASCII,
                "ISO-8859-1" => StringEncoding.Latin1,
                _ => StringEncoding.UTF8
            };
        }

        #endregion

        #region 特性检查方法

        /// <summary>
        /// 检查是否为变长编码
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsVariableLength(this StringEncoding encoding)
        {
            return encoding == StringEncoding.UTF8 ||
                   encoding == StringEncoding.UTF16LE ||
                   encoding == StringEncoding.UTF16BE;
        }

        /// <summary>
        /// 检查是否为定长编码
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsFixedLength(this StringEncoding encoding)
        {
            return encoding == StringEncoding.UTF32LE ||
                   encoding == StringEncoding.UTF32BE ||
                   encoding == StringEncoding.ASCII ||
                   encoding == StringEncoding.Latin1;
        }

        /// <summary>
        /// 检查是否支持完整 Unicode
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SupportsFullUnicode(this StringEncoding encoding)
        {
            return encoding == StringEncoding.UTF8 ||
                   encoding == StringEncoding.UTF16LE ||
                   encoding == StringEncoding.UTF16BE ||
                   encoding == StringEncoding.UTF32LE ||
                   encoding == StringEncoding.UTF32BE;
        }

        /// <summary>
        /// 检查是否为大端序
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsBigEndian(this StringEncoding encoding)
        {
            return encoding == StringEncoding.UTF16BE ||
                   encoding == StringEncoding.UTF32BE;
        }

        #endregion

        #region 大小计算方法

        /// <summary>
        /// 获取每字符最小字节数
        /// </summary>
        public static int GetMinBytesPerChar(this StringEncoding encoding)
        {
            return encoding switch
            {
                StringEncoding.UTF8 => 1,
                StringEncoding.UTF16LE => 2,
                StringEncoding.UTF16BE => 2,
                StringEncoding.UTF32LE => 4,
                StringEncoding.UTF32BE => 4,
                StringEncoding.ASCII => 1,
                StringEncoding.Latin1 => 1,
                _ => 1
            };
        }

        /// <summary>
        /// 获取每字符最大字节数
        /// </summary>
        public static int GetMaxBytesPerChar(this StringEncoding encoding)
        {
            return encoding switch
            {
                StringEncoding.UTF8 => 4,
                StringEncoding.UTF16LE => 4,
                StringEncoding.UTF16BE => 4,
                StringEncoding.UTF32LE => 4,
                StringEncoding.UTF32BE => 4,
                StringEncoding.ASCII => 1,
                StringEncoding.Latin1 => 1,
                _ => 4
            };
        }

        /// <summary>
        /// 计算字符串编码后的最大字节数
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetMaxByteCount(this StringEncoding encoding, int charCount)
        {
            return charCount * encoding.GetMaxBytesPerChar();
        }

        /// <summary>
        /// 计算字符串编码后的精确字节数
        /// </summary>
        public static int GetByteCount(this StringEncoding encoding, string value)
        {
            if (string.IsNullOrEmpty(value)) return 0;
            return encoding.ToEncoding().GetByteCount(value);
        }

        #endregion

        #region 信息获取方法

        /// <summary>
        /// 获取编码的中文描述
        /// </summary>
        public static string GetDescription(this StringEncoding encoding)
        {
            return encoding switch
            {
                StringEncoding.UTF8 => "UTF-8",
                StringEncoding.UTF16LE => "UTF-16 (小端)",
                StringEncoding.UTF16BE => "UTF-16 (大端)",
                StringEncoding.UTF32LE => "UTF-32 (小端)",
                StringEncoding.UTF32BE => "UTF-32 (大端)",
                StringEncoding.ASCII => "ASCII",
                StringEncoding.Latin1 => "Latin1 (ISO-8859-1)",
                StringEncoding.Auto => "自动检测",
                _ => "未知编码"
            };
        }

        /// <summary>
        /// 获取推荐的字符串编码
        /// </summary>
        public static StringEncoding GetRecommended(bool asciiOnly, bool needsCompactness)
        {
            if (asciiOnly) return StringEncoding.ASCII;
            return needsCompactness ? StringEncoding.UTF8 : StringEncoding.UTF16LE;
        }

        /// <summary>
        /// 自动选择最佳编码
        /// </summary>
        public static StringEncoding AutoSelect(string value)
        {
            if (string.IsNullOrEmpty(value)) return StringEncoding.UTF8;

            bool hasNonAscii = false;
            foreach (char c in value)
            {
                if (c > 127)
                {
                    hasNonAscii = true;
                    break;
                }
            }

            return hasNonAscii ? StringEncoding.UTF8 : StringEncoding.ASCII;
        }

        #endregion
    }
}
