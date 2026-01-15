// ==========================================================
// 文件名：SerializerHelper.cs
// 命名空间: AFramework.Serialization
// 依赖: System
// ==========================================================

using System;
using System.Runtime.CompilerServices;

namespace AFramework.Serialization
{
    /// <summary>
    /// 序列化器辅助工具类
    /// <para>提供序列化器通用功能</para>
    /// </summary>
    internal static class SerializerHelper
    {
        #region 类型检查

        /// <summary>
        /// 检查是否为内置类型
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsBuiltInType(Type type)
        {
            return type.IsPrimitive ||
                   type == typeof(string) ||
                   type == typeof(decimal) ||
                   type == typeof(DateTime) ||
                   type == typeof(TimeSpan) ||
                   type == typeof(Guid) ||
                   type.IsEnum ||
                   type.IsArray;
        }

        #endregion

        #region 大小估算

        /// <summary>
        /// 估算序列化大小
        /// </summary>
        public static int EstimateSize(object value)
        {
            if (value == null)
                return 1;

            var type = value.GetType();

            // 基础类型大小估算
            if (type.IsPrimitive)
                return GetPrimitiveSize(type);

            if (type == typeof(string))
            {
                var str = (string)value;
                return 4 + (str?.Length ?? 0) * 3; // 长度前缀 + UTF-8 最大编码
            }

            if (type.IsArray)
            {
                var array = (Array)value;
                var elementType = type.GetElementType();
                return 4 + array.Length * EstimateSizeForType(elementType);
            }

            // 默认估算
            return BufferConstants.Writer.InitialBufferSize;
        }

        /// <summary>
        /// 获取基础类型大小
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetPrimitiveSize(Type type)
        {
            if (type == typeof(bool) || type == typeof(byte) || type == typeof(sbyte))
                return 1;
            if (type == typeof(short) || type == typeof(ushort) || type == typeof(char))
                return 2;
            if (type == typeof(int) || type == typeof(uint) || type == typeof(float))
                return 4;
            if (type == typeof(long) || type == typeof(ulong) || type == typeof(double))
                return 8;
            if (type == typeof(decimal))
                return 16;

            return 8; // 默认
        }

        /// <summary>
        /// 估算类型的序列化大小
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int EstimateSizeForType(Type type)
        {
            if (type.IsPrimitive)
                return GetPrimitiveSize(type);

            return 32; // 默认估算
        }

        #endregion

        #region 错误码转换

        /// <summary>
        /// 根据异常获取错误码
        /// </summary>
        public static SerializeErrorCode GetErrorCode(Exception ex)
        {
            return ex switch
            {
                ArgumentNullException => SerializeErrorCode.NullReference,
                ArgumentException => SerializeErrorCode.InvalidArgument,
                InvalidOperationException => SerializeErrorCode.InvalidOperation,
                NotSupportedException => SerializeErrorCode.TypeNotSupported,
                OutOfMemoryException => SerializeErrorCode.OutOfMemory,
                _ => SerializeErrorCode.Unknown
            };
        }

        #endregion
    }
}
