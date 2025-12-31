// ==========================================================
// 文件名：BinaryReaderExtensions.cs
// 命名空间: AFramework.CSharpExtension
// 依赖: System, System.IO, System.Text
// ==========================================================

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace AFramework.CSharpExtension
{
    /// <summary>
    /// BinaryReader 扩展方法
    /// <para>提供二进制读取器的常用操作扩展，包括安全读取、批量读取、类型转换等功能</para>
    /// </summary>
    public static class BinaryReaderExtensions
    {
        #region 安全读取

        /// <summary>
        /// 安全读取 Boolean（到达末尾返回默认值）
        /// </summary>
        public static bool ReadBooleanSafe(this BinaryReader reader, bool defaultValue = false)
        {
            try { return reader?.BaseStream.Position < reader?.BaseStream.Length ? reader.ReadBoolean() : defaultValue; }
            catch { return defaultValue; }
        }

        /// <summary>
        /// 安全读取 Byte（到达末尾返回默认值）
        /// </summary>
        public static byte ReadByteSafe(this BinaryReader reader, byte defaultValue = 0)
        {
            try { return reader?.BaseStream.Position < reader?.BaseStream.Length ? reader.ReadByte() : defaultValue; }
            catch { return defaultValue; }
        }

        /// <summary>
        /// 安全读取 Int16（到达末尾返回默认值）
        /// </summary>
        public static short ReadInt16Safe(this BinaryReader reader, short defaultValue = 0)
        {
            try { return reader != null && reader.BaseStream.Position + 2 <= reader.BaseStream.Length ? reader.ReadInt16() : defaultValue; }
            catch { return defaultValue; }
        }

        /// <summary>
        /// 安全读取 Int32（到达末尾返回默认值）
        /// </summary>
        public static int ReadInt32Safe(this BinaryReader reader, int defaultValue = 0)
        {
            try { return reader != null && reader.BaseStream.Position + 4 <= reader.BaseStream.Length ? reader.ReadInt32() : defaultValue; }
            catch { return defaultValue; }
        }

        /// <summary>
        /// 安全读取 Int64（到达末尾返回默认值）
        /// </summary>
        public static long ReadInt64Safe(this BinaryReader reader, long defaultValue = 0)
        {
            try { return reader != null && reader.BaseStream.Position + 8 <= reader.BaseStream.Length ? reader.ReadInt64() : defaultValue; }
            catch { return defaultValue; }
        }

        /// <summary>
        /// 安全读取 Single（到达末尾返回默认值）
        /// </summary>
        public static float ReadSingleSafe(this BinaryReader reader, float defaultValue = 0f)
        {
            try { return reader != null && reader.BaseStream.Position + 4 <= reader.BaseStream.Length ? reader.ReadSingle() : defaultValue; }
            catch { return defaultValue; }
        }

        /// <summary>
        /// 安全读取 Double（到达末尾返回默认值）
        /// </summary>
        public static double ReadDoubleSafe(this BinaryReader reader, double defaultValue = 0d)
        {
            try { return reader != null && reader.BaseStream.Position + 8 <= reader.BaseStream.Length ? reader.ReadDouble() : defaultValue; }
            catch { return defaultValue; }
        }

        /// <summary>
        /// 安全读取 String（到达末尾返回默认值）
        /// </summary>
        public static string ReadStringSafe(this BinaryReader reader, string defaultValue = "")
        {
            try { return reader?.BaseStream.Position < reader?.BaseStream.Length ? reader.ReadString() : defaultValue; }
            catch { return defaultValue; }
        }

        #endregion

        #region 批量读取

        /// <summary>
        /// 读取 Int32 数组
        /// </summary>
        public static int[] ReadInt32Array(this BinaryReader reader, int count)
        {
            if (reader == null || count <= 0) return Array.Empty<int>();
            
            var result = new int[count];
            for (int i = 0; i < count; i++)
                result[i] = reader.ReadInt32();
            return result;
        }

        /// <summary>
        /// 读取带长度前缀的 Int32 数组
        /// </summary>
        public static int[] ReadInt32ArrayWithLength(this BinaryReader reader)
        {
            if (reader == null) return Array.Empty<int>();
            
            int count = reader.ReadInt32();
            return reader.ReadInt32Array(count);
        }

        /// <summary>
        /// 读取 Single 数组
        /// </summary>
        public static float[] ReadSingleArray(this BinaryReader reader, int count)
        {
            if (reader == null || count <= 0) return Array.Empty<float>();
            
            var result = new float[count];
            for (int i = 0; i < count; i++)
                result[i] = reader.ReadSingle();
            return result;
        }

        /// <summary>
        /// 读取带长度前缀的 Single 数组
        /// </summary>
        public static float[] ReadSingleArrayWithLength(this BinaryReader reader)
        {
            if (reader == null) return Array.Empty<float>();
            
            int count = reader.ReadInt32();
            return reader.ReadSingleArray(count);
        }

        /// <summary>
        /// 读取 String 数组
        /// </summary>
        public static string[] ReadStringArray(this BinaryReader reader, int count)
        {
            if (reader == null || count <= 0) return Array.Empty<string>();
            
            var result = new string[count];
            for (int i = 0; i < count; i++)
                result[i] = reader.ReadString();
            return result;
        }

        /// <summary>
        /// 读取带长度前缀的 String 数组
        /// </summary>
        public static string[] ReadStringArrayWithLength(this BinaryReader reader)
        {
            if (reader == null) return Array.Empty<string>();
            
            int count = reader.ReadInt32();
            return reader.ReadStringArray(count);
        }

        #endregion

        #region 特殊类型读取

        /// <summary>
        /// 读取可空 Int32
        /// </summary>
        public static int? ReadNullableInt32(this BinaryReader reader)
        {
            if (reader == null) return null;
            
            bool hasValue = reader.ReadBoolean();
            return hasValue ? reader.ReadInt32() : (int?)null;
        }

        /// <summary>
        /// 读取可空 Single
        /// </summary>
        public static float? ReadNullableSingle(this BinaryReader reader)
        {
            if (reader == null) return null;
            
            bool hasValue = reader.ReadBoolean();
            return hasValue ? reader.ReadSingle() : (float?)null;
        }

        /// <summary>
        /// 读取 DateTime
        /// </summary>
        public static DateTime ReadDateTime(this BinaryReader reader)
        {
            if (reader == null) return DateTime.MinValue;
            return DateTime.FromBinary(reader.ReadInt64());
        }

        /// <summary>
        /// 读取 TimeSpan
        /// </summary>
        public static TimeSpan ReadTimeSpan(this BinaryReader reader)
        {
            if (reader == null) return TimeSpan.Zero;
            return TimeSpan.FromTicks(reader.ReadInt64());
        }

        /// <summary>
        /// 读取 Guid
        /// </summary>
        public static Guid ReadGuid(this BinaryReader reader)
        {
            if (reader == null) return Guid.Empty;
            return new Guid(reader.ReadBytes(16));
        }

        /// <summary>
        /// 读取 Decimal
        /// </summary>
        public static decimal ReadDecimalValue(this BinaryReader reader)
        {
            if (reader == null) return 0m;
            
            var bits = new int[4];
            for (int i = 0; i < 4; i++)
                bits[i] = reader.ReadInt32();
            return new decimal(bits);
        }

        #endregion

        #region 字符串读取

        /// <summary>
        /// 读取固定长度字符串
        /// </summary>
        public static string ReadFixedString(this BinaryReader reader, int length, Encoding encoding = null)
        {
            if (reader == null || length <= 0) return string.Empty;
            
            encoding = encoding ?? Encoding.UTF8;
            var bytes = reader.ReadBytes(length);
            var str = encoding.GetString(bytes);
            
            // 移除末尾的空字符
            int nullIndex = str.IndexOf('\0');
            return nullIndex >= 0 ? str.Substring(0, nullIndex) : str;
        }

        /// <summary>
        /// 读取以 null 结尾的字符串
        /// </summary>
        public static string ReadNullTerminatedString(this BinaryReader reader, Encoding encoding = null)
        {
            if (reader == null) return string.Empty;
            
            encoding = encoding ?? Encoding.UTF8;
            var bytes = new List<byte>();
            byte b;
            
            while ((b = reader.ReadByte()) != 0)
                bytes.Add(b);
            
            return encoding.GetString(bytes.ToArray());
        }

        /// <summary>
        /// 读取带长度前缀的字节数组作为字符串
        /// </summary>
        public static string ReadLengthPrefixedString(this BinaryReader reader, Encoding encoding = null)
        {
            if (reader == null) return string.Empty;
            
            encoding = encoding ?? Encoding.UTF8;
            int length = reader.ReadInt32();
            if (length <= 0) return string.Empty;
            
            var bytes = reader.ReadBytes(length);
            return encoding.GetString(bytes);
        }

        #endregion

        #region 位置操作

        /// <summary>
        /// 获取剩余可读字节数
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long GetRemainingBytes(this BinaryReader reader)
        {
            if (reader?.BaseStream == null) return 0;
            return reader.BaseStream.Length - reader.BaseStream.Position;
        }

        /// <summary>
        /// 检查是否已到达末尾
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsAtEnd(this BinaryReader reader)
        {
            if (reader?.BaseStream == null) return true;
            return reader.BaseStream.Position >= reader.BaseStream.Length;
        }

        /// <summary>
        /// 跳过指定字节数
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Skip(this BinaryReader reader, int count)
        {
            if (reader?.BaseStream != null && reader.BaseStream.CanSeek)
                reader.BaseStream.Position += count;
        }

        /// <summary>
        /// 定位到指定位置
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Seek(this BinaryReader reader, long position)
        {
            if (reader?.BaseStream != null && reader.BaseStream.CanSeek)
                reader.BaseStream.Position = position;
        }

        /// <summary>
        /// 重置到开头
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Reset(this BinaryReader reader)
        {
            reader.Seek(0);
        }

        #endregion

        #region 字典读取

        /// <summary>
        /// 读取字符串-字符串字典
        /// </summary>
        public static Dictionary<string, string> ReadStringDictionary(this BinaryReader reader)
        {
            if (reader == null) return new Dictionary<string, string>();
            
            int count = reader.ReadInt32();
            var result = new Dictionary<string, string>(count);
            
            for (int i = 0; i < count; i++)
            {
                string key = reader.ReadString();
                string value = reader.ReadString();
                result[key] = value;
            }
            
            return result;
        }

        /// <summary>
        /// 读取字符串-Int32 字典
        /// </summary>
        public static Dictionary<string, int> ReadStringIntDictionary(this BinaryReader reader)
        {
            if (reader == null) return new Dictionary<string, int>();
            
            int count = reader.ReadInt32();
            var result = new Dictionary<string, int>(count);
            
            for (int i = 0; i < count; i++)
            {
                string key = reader.ReadString();
                int value = reader.ReadInt32();
                result[key] = value;
            }
            
            return result;
        }

        #endregion
    }
}
