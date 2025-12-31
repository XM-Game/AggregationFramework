// ==========================================================
// 文件名：BinaryWriterExtensions.cs
// 命名空间: AFramework.CSharpExtension
// 依赖: System, System.IO, System.Text, System.Collections.Generic
// ==========================================================

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace AFramework.CSharpExtension
{
    /// <summary>
    /// BinaryWriter 扩展方法
    /// <para>提供二进制写入器的常用操作扩展，包括批量写入、类型转换、安全写入等功能</para>
    /// </summary>
    public static class BinaryWriterExtensions
    {
        #region 批量写入

        /// <summary>
        /// 写入 Int32 数组
        /// </summary>
        public static void WriteArray(this BinaryWriter writer, int[] array)
        {
            if (writer == null || array == null) return;
            
            foreach (var item in array)
                writer.Write(item);
        }

        /// <summary>
        /// 写入带长度前缀的 Int32 数组
        /// </summary>
        public static void WriteArrayWithLength(this BinaryWriter writer, int[] array)
        {
            if (writer == null) return;
            
            if (array == null)
            {
                writer.Write(0);
                return;
            }
            
            writer.Write(array.Length);
            writer.WriteArray(array);
        }

        /// <summary>
        /// 写入 Single 数组
        /// </summary>
        public static void WriteArray(this BinaryWriter writer, float[] array)
        {
            if (writer == null || array == null) return;
            
            foreach (var item in array)
                writer.Write(item);
        }

        /// <summary>
        /// 写入带长度前缀的 Single 数组
        /// </summary>
        public static void WriteArrayWithLength(this BinaryWriter writer, float[] array)
        {
            if (writer == null) return;
            
            if (array == null)
            {
                writer.Write(0);
                return;
            }
            
            writer.Write(array.Length);
            writer.WriteArray(array);
        }

        /// <summary>
        /// 写入 Double 数组
        /// </summary>
        public static void WriteArray(this BinaryWriter writer, double[] array)
        {
            if (writer == null || array == null) return;
            
            foreach (var item in array)
                writer.Write(item);
        }

        /// <summary>
        /// 写入 String 数组
        /// </summary>
        public static void WriteArray(this BinaryWriter writer, string[] array)
        {
            if (writer == null || array == null) return;
            
            foreach (var item in array)
                writer.Write(item ?? string.Empty);
        }

        /// <summary>
        /// 写入带长度前缀的 String 数组
        /// </summary>
        public static void WriteArrayWithLength(this BinaryWriter writer, string[] array)
        {
            if (writer == null) return;
            
            if (array == null)
            {
                writer.Write(0);
                return;
            }
            
            writer.Write(array.Length);
            writer.WriteArray(array);
        }

        #endregion

        #region 特殊类型写入

        /// <summary>
        /// 写入可空 Int32
        /// </summary>
        public static void WriteNullable(this BinaryWriter writer, int? value)
        {
            if (writer == null) return;
            
            writer.Write(value.HasValue);
            if (value.HasValue)
                writer.Write(value.Value);
        }

        /// <summary>
        /// 写入可空 Single
        /// </summary>
        public static void WriteNullable(this BinaryWriter writer, float? value)
        {
            if (writer == null) return;
            
            writer.Write(value.HasValue);
            if (value.HasValue)
                writer.Write(value.Value);
        }

        /// <summary>
        /// 写入可空 Double
        /// </summary>
        public static void WriteNullable(this BinaryWriter writer, double? value)
        {
            if (writer == null) return;
            
            writer.Write(value.HasValue);
            if (value.HasValue)
                writer.Write(value.Value);
        }

        /// <summary>
        /// 写入 DateTime
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Write(this BinaryWriter writer, DateTime value)
        {
            writer?.Write(value.ToBinary());
        }

        /// <summary>
        /// 写入 TimeSpan
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Write(this BinaryWriter writer, TimeSpan value)
        {
            writer?.Write(value.Ticks);
        }

        /// <summary>
        /// 写入 Guid
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Write(this BinaryWriter writer, Guid value)
        {
            writer?.Write(value.ToByteArray());
        }

        /// <summary>
        /// 写入 Decimal
        /// </summary>
        public static void WriteDecimal(this BinaryWriter writer, decimal value)
        {
            if (writer == null) return;
            
            var bits = decimal.GetBits(value);
            foreach (var bit in bits)
                writer.Write(bit);
        }

        #endregion

        #region 字符串写入

        /// <summary>
        /// 写入固定长度字符串（不足补零，超出截断）
        /// </summary>
        public static void WriteFixedString(this BinaryWriter writer, string value, int length, Encoding encoding = null)
        {
            if (writer == null || length <= 0) return;
            
            encoding = encoding ?? Encoding.UTF8;
            value = value ?? string.Empty;
            
            var bytes = encoding.GetBytes(value);
            var buffer = new byte[length];
            
            int copyLength = Math.Min(bytes.Length, length);
            Array.Copy(bytes, buffer, copyLength);
            
            writer.Write(buffer);
        }

        /// <summary>
        /// 写入以 null 结尾的字符串
        /// </summary>
        public static void WriteNullTerminatedString(this BinaryWriter writer, string value, Encoding encoding = null)
        {
            if (writer == null) return;
            
            encoding = encoding ?? Encoding.UTF8;
            value = value ?? string.Empty;
            
            var bytes = encoding.GetBytes(value);
            writer.Write(bytes);
            writer.Write((byte)0);
        }

        /// <summary>
        /// 写入带长度前缀的字符串（字节数组形式）
        /// </summary>
        public static void WriteLengthPrefixedString(this BinaryWriter writer, string value, Encoding encoding = null)
        {
            if (writer == null) return;
            
            encoding = encoding ?? Encoding.UTF8;
            
            if (string.IsNullOrEmpty(value))
            {
                writer.Write(0);
                return;
            }
            
            var bytes = encoding.GetBytes(value);
            writer.Write(bytes.Length);
            writer.Write(bytes);
        }

        #endregion

        #region 字典写入

        /// <summary>
        /// 写入字符串-字符串字典
        /// </summary>
        public static void WriteDictionary(this BinaryWriter writer, Dictionary<string, string> dictionary)
        {
            if (writer == null) return;
            
            if (dictionary == null)
            {
                writer.Write(0);
                return;
            }
            
            writer.Write(dictionary.Count);
            foreach (var kvp in dictionary)
            {
                writer.Write(kvp.Key ?? string.Empty);
                writer.Write(kvp.Value ?? string.Empty);
            }
        }

        /// <summary>
        /// 写入字符串-Int32 字典
        /// </summary>
        public static void WriteDictionary(this BinaryWriter writer, Dictionary<string, int> dictionary)
        {
            if (writer == null) return;
            
            if (dictionary == null)
            {
                writer.Write(0);
                return;
            }
            
            writer.Write(dictionary.Count);
            foreach (var kvp in dictionary)
            {
                writer.Write(kvp.Key ?? string.Empty);
                writer.Write(kvp.Value);
            }
        }

        /// <summary>
        /// 写入 Int32-字符串 字典
        /// </summary>
        public static void WriteDictionary(this BinaryWriter writer, Dictionary<int, string> dictionary)
        {
            if (writer == null) return;
            
            if (dictionary == null)
            {
                writer.Write(0);
                return;
            }
            
            writer.Write(dictionary.Count);
            foreach (var kvp in dictionary)
            {
                writer.Write(kvp.Key);
                writer.Write(kvp.Value ?? string.Empty);
            }
        }

        #endregion

        #region 列表写入

        /// <summary>
        /// 写入 Int32 列表
        /// </summary>
        public static void WriteList(this BinaryWriter writer, List<int> list)
        {
            if (writer == null) return;
            
            if (list == null)
            {
                writer.Write(0);
                return;
            }
            
            writer.Write(list.Count);
            foreach (var item in list)
                writer.Write(item);
        }

        /// <summary>
        /// 写入 String 列表
        /// </summary>
        public static void WriteList(this BinaryWriter writer, List<string> list)
        {
            if (writer == null) return;
            
            if (list == null)
            {
                writer.Write(0);
                return;
            }
            
            writer.Write(list.Count);
            foreach (var item in list)
                writer.Write(item ?? string.Empty);
        }

        #endregion

        #region 位置操作

        /// <summary>
        /// 获取当前位置
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long GetPosition(this BinaryWriter writer)
        {
            return writer?.BaseStream?.Position ?? 0;
        }

        /// <summary>
        /// 定位到指定位置
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Seek(this BinaryWriter writer, long position)
        {
            if (writer?.BaseStream != null && writer.BaseStream.CanSeek)
                writer.BaseStream.Position = position;
        }

        /// <summary>
        /// 重置到开头
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Reset(this BinaryWriter writer)
        {
            writer.Seek(0);
        }

        #endregion

        #region 安全操作

        /// <summary>
        /// 安全刷新
        /// </summary>
        public static void SafeFlush(this BinaryWriter writer)
        {
            try { writer?.Flush(); }
            catch { /* 忽略异常 */ }
        }

        /// <summary>
        /// 安全关闭
        /// </summary>
        public static void SafeClose(this BinaryWriter writer)
        {
            try { writer?.Close(); }
            catch { /* 忽略异常 */ }
        }

        #endregion

        #region 填充操作

        /// <summary>
        /// 写入填充字节
        /// </summary>
        public static void WritePadding(this BinaryWriter writer, int count, byte value = 0)
        {
            if (writer == null || count <= 0) return;
            
            for (int i = 0; i < count; i++)
                writer.Write(value);
        }

        /// <summary>
        /// 对齐到指定边界
        /// </summary>
        public static void AlignTo(this BinaryWriter writer, int alignment, byte paddingValue = 0)
        {
            if (writer?.BaseStream == null || alignment <= 1) return;
            
            long position = writer.BaseStream.Position;
            int remainder = (int)(position % alignment);
            
            if (remainder > 0)
                writer.WritePadding(alignment - remainder, paddingValue);
        }

        #endregion
    }
}
