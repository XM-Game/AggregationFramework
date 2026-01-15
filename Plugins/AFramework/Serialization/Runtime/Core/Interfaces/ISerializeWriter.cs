// ==========================================================
// 文件名：ISerializeWriter.cs
// 命名空间: AFramework.Serialization
// 依赖: System
// ==========================================================

using System;

namespace AFramework.Serialization
{
    /// <summary>
    /// 序列化写入器接口
    /// <para>提供将各种数据类型写入二进制流的方法</para>
    /// <para>支持基元类型、字符串、数组和复杂对象的写入</para>
    /// </summary>
    /// <remarks>
    /// 写入器是格式化器的核心依赖，负责将数据转换为二进制格式。
    /// 
    /// 使用示例:
    /// <code>
    /// public void Serialize(ISerializeWriter writer, Player value, SerializeOptions options)
    /// {
    ///     writer.WriteString(value.Name);
    ///     writer.WriteInt32(value.Level);
    ///     writer.WriteInt64(value.Experience);
    ///     
    ///     // 写入数组
    ///     writer.WriteArrayHeader(value.Items.Length);
    ///     foreach (var item in value.Items)
    ///         writer.WriteObject(item, itemFormatter);
    /// }
    /// </code>
    /// </remarks>
    public interface ISerializeWriter : IDisposable
    {
        #region 基元类型写入

        /// <summary>写入布尔值</summary>
        void WriteBool(bool value);

        /// <summary>写入字节</summary>
        void WriteByte(byte value);

        /// <summary>写入有符号字节</summary>
        void WriteSByte(sbyte value);

        /// <summary>写入短整数</summary>
        void WriteInt16(short value);

        /// <summary>写入无符号短整数</summary>
        void WriteUInt16(ushort value);

        /// <summary>写入整数</summary>
        void WriteInt32(int value);

        /// <summary>写入无符号整数</summary>
        void WriteUInt32(uint value);

        /// <summary>写入长整数</summary>
        void WriteInt64(long value);

        /// <summary>写入无符号长整数</summary>
        void WriteUInt64(ulong value);

        /// <summary>写入单精度浮点数</summary>
        void WriteSingle(float value);

        /// <summary>写入双精度浮点数</summary>
        void WriteDouble(double value);

        /// <summary>写入十进制数</summary>
        void WriteDecimal(decimal value);

        /// <summary>写入字符</summary>
        void WriteChar(char value);

        #endregion

        #region 变长整数写入

        /// <summary>写入变长整数 (VarInt)</summary>
        void WriteVarInt32(int value);

        /// <summary>写入变长无符号整数 (VarUInt)</summary>
        void WriteVarUInt32(uint value);

        /// <summary>写入变长长整数 (VarInt64)</summary>
        void WriteVarInt64(long value);

        /// <summary>写入变长无符号长整数 (VarUInt64)</summary>
        void WriteVarUInt64(ulong value);

        #endregion

        #region 字符串和字节数组写入

        /// <summary>写入字符串</summary>
        void WriteString(string value);

        /// <summary>写入字符串 (可为 null)</summary>
        void WriteNullableString(string value);

        /// <summary>写入字节数组</summary>
        void WriteBytes(ReadOnlySpan<byte> value);

        /// <summary>写入字节数组 (可为 null)</summary>
        void WriteNullableBytes(byte[] value);

        #endregion

        #region 特殊类型写入

        /// <summary>写入 GUID</summary>
        void WriteGuid(Guid value);

        /// <summary>写入日期时间</summary>
        void WriteDateTime(DateTime value);

        /// <summary>写入日期时间偏移</summary>
        void WriteDateTimeOffset(DateTimeOffset value);

        /// <summary>写入时间跨度</summary>
        void WriteTimeSpan(TimeSpan value);

        #endregion

        #region 集合写入

        /// <summary>
        /// 写入数组头 (元素数量)
        /// </summary>
        /// <param name="count">元素数量，-1 表示 null</param>
        void WriteArrayHeader(int count);

        /// <summary>
        /// 写入字典头 (键值对数量)
        /// </summary>
        /// <param name="count">键值对数量，-1 表示 null</param>
        void WriteMapHeader(int count);

        /// <summary>
        /// 写入集合头 (元素数量)
        /// </summary>
        /// <param name="count">元素数量，-1 表示 null</param>
        void WriteCollectionHeader(int count);

        #endregion

        #region 对象写入

        /// <summary>
        /// 写入 null 标记
        /// </summary>
        void WriteNull();

        /// <summary>
        /// 写入 null 对象头标记
        /// </summary>
        void WriteNullObjectHeader();

        /// <summary>
        /// 写入对象 (使用指定格式化器)
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="value">要写入的对象</param>
        /// <param name="formatter">格式化器</param>
        void WriteObject<T>(T value, IFormatter<T> formatter);

        /// <summary>
        /// 写入对象 (自动解析格式化器)
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="value">要写入的对象</param>
        void WriteObject<T>(T value);

        /// <summary>
        /// 写入类型信息
        /// </summary>
        /// <param name="type">类型</param>
        void WriteType(Type type);

        #endregion

        #region 原始数据写入

        /// <summary>
        /// 直接写入原始字节
        /// </summary>
        /// <param name="data">原始数据</param>
        void WriteRaw(ReadOnlySpan<byte> data);

        /// <summary>
        /// 获取可写入的 Span
        /// </summary>
        /// <param name="size">需要的大小</param>
        /// <returns>可写入的 Span</returns>
        Span<byte> GetSpan(int size);

        /// <summary>
        /// 推进写入位置
        /// </summary>
        /// <param name="count">推进的字节数</param>
        void Advance(int count);

        /// <summary>
        /// 写入非托管类型值 (零拷贝)
        /// </summary>
        /// <typeparam name="T">非托管类型</typeparam>
        /// <param name="value">要写入的值</param>
        void WriteUnmanaged<T>(T value) where T : unmanaged;

        /// <summary>
        /// 写入非托管类型数组 (零拷贝)
        /// </summary>
        /// <typeparam name="T">非托管类型</typeparam>
        /// <param name="values">要写入的数组</param>
        void WriteUnmanagedArray<T>(ReadOnlySpan<T> values) where T : unmanaged;

        #endregion

        #region 状态和属性

        /// <summary>获取当前写入位置</summary>
        long Position { get; }

        /// <summary>获取已写入的字节数</summary>
        long BytesWritten { get; }

        /// <summary>获取当前深度</summary>
        int Depth { get; }

        /// <summary>获取序列化选项</summary>
        SerializeOptions Options { get; }

        /// <summary>获取格式化器解析器</summary>
        IFormatterResolver Resolver { get; }

        #endregion

        #region 深度控制

        /// <summary>
        /// 进入嵌套层级
        /// </summary>
        /// <exception cref="InvalidOperationException">超出最大深度时抛出</exception>
        void EnterDepth();

        /// <summary>
        /// 离开嵌套层级
        /// </summary>
        void ExitDepth();

        #endregion

        #region 刷新和完成

        /// <summary>
        /// 刷新缓冲区
        /// </summary>
        void Flush();

        /// <summary>
        /// 获取已写入的数据
        /// </summary>
        /// <returns>已写入的字节数组</returns>
        byte[] ToArray();

        /// <summary>
        /// 获取已写入的数据 (只读)
        /// </summary>
        /// <returns>已写入数据的只读内存</returns>
        ReadOnlyMemory<byte> GetWrittenMemory();

        #endregion
    }
}
