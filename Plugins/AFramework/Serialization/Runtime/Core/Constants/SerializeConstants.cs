// ==========================================================
// 文件名：SerializeConstants.cs
// 命名空间: AFramework.Serialization
// 依赖: System
// ==========================================================

using System;
using System.Runtime.CompilerServices;

namespace AFramework.Serialization
{
    /// <summary>
    /// 序列化系统核心常量定义
    /// <para>提供序列化系统的核心配置常量和限制值</para>
    /// <para>包含版本信息、深度限制、大小限制等关键常量</para>
    /// </summary>
    /// <remarks>
    /// 使用示例:
    /// <code>
    /// // 检查序列化深度
    /// if (currentDepth > SerializeConstants.MaxSerializeDepth)
    ///     throw new SerializationException("超出最大序列化深度");
    /// 
    /// // 使用版本号
    /// writer.WriteVersion(SerializeConstants.CurrentVersion);
    /// 
    /// // 检查数据大小
    /// if (dataSize > SerializeConstants.MaxDataSize)
    ///     throw new SerializationException("数据大小超出限制");
    /// </code>
    /// </remarks>
    public static class SerializeConstants
    {
        #region 版本信息常量

        /// <summary>
        /// 版本信息常量集合
        /// <para>定义序列化系统的版本号和兼容性信息</para>
        /// </summary>
        public static class Version
        {
            /// <summary>主版本号</summary>
            public const int Major = 1;

            /// <summary>次版本号</summary>
            public const int Minor = 0;

            /// <summary>修订版本号</summary>
            public const int Patch = 0;

            /// <summary>完整版本号 (用于数据头)</summary>
            public const ushort Current = (Major << 8) | Minor;

            /// <summary>最小兼容版本号</summary>
            public const ushort MinCompatible = 0x0100; // 1.0

            /// <summary>版本字符串</summary>
            public const string VersionString = "1.0.0";

            /// <summary>构建日期 (YYYYMMDD 格式)</summary>
            public const int BuildDate = 20260104;
        }

        #endregion

        #region 深度限制常量

        /// <summary>默认最大序列化深度</summary>
        public const int DefaultMaxDepth = 64;

        /// <summary>最大序列化深度 (硬限制)</summary>
        public const int MaxSerializeDepth = 256;

        /// <summary>最小序列化深度</summary>
        public const int MinSerializeDepth = 1;

        /// <summary>默认最大循环引用深度</summary>
        public const int DefaultMaxCircularDepth = 32;

        /// <summary>默认最大多态类型深度</summary>
        public const int DefaultMaxPolymorphicDepth = 16;

        #endregion

        #region 大小限制常量

        /// <summary>
        /// 大小限制常量集合
        /// <para>定义序列化数据的各种大小限制</para>
        /// </summary>
        public static class SizeLimit
        {
            /// <summary>最大数据大小 (默认 2GB)</summary>
            public const long MaxDataSize = 2L * 1024 * 1024 * 1024;

            /// <summary>最大字符串长度</summary>
            public const int MaxStringLength = 1024 * 1024 * 100; // 100MB

            /// <summary>最大集合元素数量</summary>
            public const int MaxCollectionCount = 1024 * 1024 * 10; // 1000万

            /// <summary>最大数组维度</summary>
            public const int MaxArrayRank = 32;

            /// <summary>最大对象成员数量</summary>
            public const int MaxMemberCount = 1024;

            /// <summary>最大类型名称长度</summary>
            public const int MaxTypeNameLength = 1024;

            /// <summary>最大字段名称长度</summary>
            public const int MaxFieldNameLength = 256;

            /// <summary>最大嵌套集合深度</summary>
            public const int MaxNestedCollectionDepth = 16;

            /// <summary>单次序列化最大对象数量 (循环引用模式)</summary>
            public const int MaxObjectCount = 1024 * 1024; // 100万
        }

        #endregion

        #region 默认配置常量

        /// <summary>
        /// 默认配置常量集合
        /// <para>定义序列化系统的默认行为配置</para>
        /// </summary>
        public static class Defaults
        {
            /// <summary>默认是否包含私有成员</summary>
            public const bool IncludePrivateMembers = false;

            /// <summary>默认是否忽略只读属性</summary>
            public const bool IgnoreReadOnlyProperties = true;

            /// <summary>默认是否保留空值</summary>
            public const bool PreserveNullValues = true;

            /// <summary>默认是否使用驼峰命名</summary>
            public const bool UseCamelCase = false;

            /// <summary>默认是否启用类型检查</summary>
            public const bool EnableTypeCheck = true;

            /// <summary>默认是否启用版本容错</summary>
            public const bool EnableVersionTolerance = false;

            /// <summary>默认是否启用循环引用检测</summary>
            public const bool EnableCircularReferenceDetection = false;

            /// <summary>默认是否启用压缩</summary>
            public const bool EnableCompression = false;

            /// <summary>默认是否启用加密</summary>
            public const bool EnableEncryption = false;

            /// <summary>默认是否启用校验和</summary>
            public const bool EnableChecksum = false;

            /// <summary>默认是否启用调试信息</summary>
            public const bool EnableDebugInfo = false;
        }

        #endregion

        #region 性能阈值常量

        /// <summary>
        /// 性能阈值常量集合
        /// <para>定义触发特定优化策略的阈值</para>
        /// </summary>
        public static class Threshold
        {
            /// <summary>启用 SIMD 优化的最小元素数量</summary>
            public const int SimdMinElements = 16;

            /// <summary>启用并行处理的最小元素数量</summary>
            public const int ParallelMinElements = 1000;

            /// <summary>启用字符串内化的最小重复次数</summary>
            public const int StringInternMinCount = 3;

            /// <summary>启用压缩的最小数据大小 (字节)</summary>
            public const int CompressionMinSize = 256;

            /// <summary>大对象阈值 (字节) - 超过此值使用特殊处理</summary>
            public const int LargeObjectSize = 85000;

            /// <summary>小字符串阈值 (字符数) - 使用栈分配</summary>
            public const int SmallStringLength = 128;

            /// <summary>内联数组最大长度</summary>
            public const int InlineArrayMaxLength = 8;
        }

        #endregion

        #region 超时常量

        /// <summary>
        /// 超时常量集合
        /// <para>定义序列化操作的超时时间</para>
        /// </summary>
        public static class Timeout
        {
            /// <summary>默认序列化超时 (毫秒)</summary>
            public const int DefaultSerializeMs = 30000; // 30秒

            /// <summary>默认反序列化超时 (毫秒)</summary>
            public const int DefaultDeserializeMs = 30000; // 30秒

            /// <summary>流式操作超时 (毫秒)</summary>
            public const int StreamOperationMs = 60000; // 60秒

            /// <summary>无限超时</summary>
            public const int Infinite = -1;

            /// <summary>快速操作超时 (毫秒)</summary>
            public const int FastOperationMs = 5000; // 5秒
        }

        #endregion

        #region 重试策略常量

        /// <summary>
        /// 重试策略常量集合
        /// <para>定义序列化操作失败时的重试策略</para>
        /// </summary>
        public static class Retry
        {
            /// <summary>默认最大重试次数</summary>
            public const int DefaultMaxRetries = 3;

            /// <summary>默认重试延迟 (毫秒)</summary>
            public const int DefaultDelayMs = 100;

            /// <summary>最大重试延迟 (毫秒)</summary>
            public const int MaxDelayMs = 5000;

            /// <summary>重试延迟倍数 (指数退避)</summary>
            public const float DelayMultiplier = 2.0f;
        }

        #endregion

        #region 特性标志常量

        /// <summary>
        /// 特性标志常量集合
        /// <para>定义序列化数据头中的特性标志位</para>
        /// </summary>
        [Flags]
        public enum FeatureFlags : ushort
        {
            /// <summary>无特性</summary>
            None = 0,

            /// <summary>启用压缩</summary>
            Compressed = 1 << 0,

            /// <summary>启用加密</summary>
            Encrypted = 1 << 1,

            /// <summary>启用校验和</summary>
            Checksum = 1 << 2,

            /// <summary>包含类型信息</summary>
            TypeInfo = 1 << 3,

            /// <summary>版本容错模式</summary>
            VersionTolerant = 1 << 4,

            /// <summary>循环引用模式</summary>
            CircularReference = 1 << 5,

            /// <summary>多态模式</summary>
            Polymorphic = 1 << 6,

            /// <summary>流式模式</summary>
            Streaming = 1 << 7,

            /// <summary>包含调试信息</summary>
            DebugInfo = 1 << 8,

            /// <summary>使用字符串内化</summary>
            StringIntern = 1 << 9,

            /// <summary>使用位打包</summary>
            BitPacked = 1 << 10,

            /// <summary>大端字节序</summary>
            BigEndian = 1 << 11,

            /// <summary>保留标志 1</summary>
            Reserved1 = 1 << 12,

            /// <summary>保留标志 2</summary>
            Reserved2 = 1 << 13,

            /// <summary>保留标志 3</summary>
            Reserved3 = 1 << 14,

            /// <summary>保留标志 4</summary>
            Reserved4 = 1 << 15
        }

        #endregion

        #region 工具方法

        /// <summary>
        /// 检查深度是否在有效范围内
        /// </summary>
        /// <param name="depth">要检查的深度值</param>
        /// <returns>如果深度有效返回 true</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsValidDepth(int depth)
        {
            return depth >= MinSerializeDepth && depth <= MaxSerializeDepth;
        }

        /// <summary>
        /// 检查数据大小是否在有效范围内
        /// </summary>
        /// <param name="size">要检查的大小</param>
        /// <returns>如果大小有效返回 true</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsValidDataSize(long size)
        {
            return size >= 0 && size <= SizeLimit.MaxDataSize;
        }

        /// <summary>
        /// 检查集合数量是否在有效范围内
        /// </summary>
        /// <param name="count">要检查的数量</param>
        /// <returns>如果数量有效返回 true</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsValidCollectionCount(int count)
        {
            return count >= 0 && count <= SizeLimit.MaxCollectionCount;
        }

        /// <summary>
        /// 检查字符串长度是否在有效范围内
        /// </summary>
        /// <param name="length">要检查的长度</param>
        /// <returns>如果长度有效返回 true</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsValidStringLength(int length)
        {
            return length >= 0 && length <= SizeLimit.MaxStringLength;
        }

        /// <summary>
        /// 检查版本是否兼容
        /// </summary>
        /// <param name="version">要检查的版本号</param>
        /// <returns>如果版本兼容返回 true</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsCompatibleVersion(ushort version)
        {
            return version >= Version.MinCompatible && version <= Version.Current;
        }

        /// <summary>
        /// 获取版本的主版本号
        /// </summary>
        /// <param name="version">完整版本号</param>
        /// <returns>主版本号</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetMajorVersion(ushort version)
        {
            return version >> 8;
        }

        /// <summary>
        /// 获取版本的次版本号
        /// </summary>
        /// <param name="version">完整版本号</param>
        /// <returns>次版本号</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetMinorVersion(ushort version)
        {
            return version & 0xFF;
        }

        /// <summary>
        /// 组合主次版本号为完整版本号
        /// </summary>
        /// <param name="major">主版本号</param>
        /// <param name="minor">次版本号</param>
        /// <returns>完整版本号</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort MakeVersion(int major, int minor)
        {
            return (ushort)((major << 8) | (minor & 0xFF));
        }

        /// <summary>
        /// 检查是否应该使用 SIMD 优化
        /// </summary>
        /// <param name="elementCount">元素数量</param>
        /// <returns>如果应该使用 SIMD 返回 true</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ShouldUseSimd(int elementCount)
        {
            return elementCount >= Threshold.SimdMinElements;
        }

        /// <summary>
        /// 检查是否应该使用并行处理
        /// </summary>
        /// <param name="elementCount">元素数量</param>
        /// <returns>如果应该使用并行处理返回 true</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ShouldUseParallel(int elementCount)
        {
            return elementCount >= Threshold.ParallelMinElements;
        }

        /// <summary>
        /// 检查是否应该启用压缩
        /// </summary>
        /// <param name="dataSize">数据大小</param>
        /// <returns>如果应该启用压缩返回 true</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ShouldCompress(int dataSize)
        {
            return dataSize >= Threshold.CompressionMinSize;
        }

        #endregion
    }
}
