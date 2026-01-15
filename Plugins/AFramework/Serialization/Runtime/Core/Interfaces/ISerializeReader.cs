// ==========================================================
// 文件名：ISerializeReader.cs
// 命名空间: AFramework.Serialization
// 依赖: System
// ==========================================================

using System;

namespace AFramework.Serialization
{
    /// <summary>
    /// 序列化读取器接口
    /// <para>提供从二进制流读取各种数据类型的方法</para>
    /// <para>支持基元类型、字符串、数组和复杂对象的读取</para>
    /// </summary>
    /// <remarks>
    /// 读取器是格式化器的核心依赖，负责从二进制格式还原数据。
    /// 
    /// 使用示例:
    /// <code>
    /// public Player Deserialize(ISerializeReader reader, DeserializeOptions options)
    /// {
    ///     var player = new Player
    ///     {
    ///         Name = reader.ReadString(),
    ///         Level = reader.ReadInt32(),
    ///         Experience = reader.ReadInt64()
    ///     };
    ///     
    ///     // 读取数组
    ///     int count = reader.ReadArrayHeader();
    ///     player.Items = new Item[count];
    ///     for (int i = 0; i &lt; count; i++)
    ///         player.Items[i] = reader.ReadObject&lt;Item&gt;(itemFormatter);
    ///     
    ///     return player;
    /// }
    /// </code>
    /// </remarks>
    public interface ISerializeReader : IDisposable
    {
        #region 基元类型读取

        /// <summary>读取布尔值</summary>
        bool ReadBool();

        /// <summary>读取字节</summary>
        byte ReadByte();

        /// <summary>读取有符号字节</summary>
        sbyte ReadSByte();

        /// <summary>读取短整数</summary>
        short ReadInt16();

        /// <summary>读取无符号短整数</summary>
        ushort ReadUInt16();

        /// <summary>读取整数</summary>
        int ReadInt32();

        /// <summary>读取无符号整数</summary>
        uint ReadUInt32();

        /// <summary>读取长整数</summary>
        long ReadInt64();

        /// <summary>读取无符号长整数</summary>
        ulong ReadUInt64();

        /// <summary>读取单精度浮点数</summary>
        float ReadSingle();

        /// <summary>读取双精度浮点数</summary>
        double ReadDouble();

        /// <summary>读取十进制数</summary>
        decimal ReadDecimal();

        /// <summary>读取字符</summary>
        char ReadChar();

        #endregion

        #region 变长整数读取

        /// <summary>读取变长整数 (VarInt)</summary>
        int ReadVarInt32();

        /// <summary>读取变长无符号整数 (VarUInt)</summary>
        uint ReadVarUInt32();

        /// <summary>读取变长长整数 (VarInt64)</summary>
        long ReadVarInt64();

        /// <summary>读取变长无符号长整数 (VarUInt64)</summary>
        ulong ReadVarUInt64();

        #endregion

        #region 字符串和字节数组读取

        /// <summary>读取字符串</summary>
        string ReadString();

        /// <summary>读取字符串 (可为 null)</summary>
        string ReadNullableString();

        /// <summary>读取字节数组</summary>
        byte[] ReadBytes();

        /// <summary>读取字节数组到指定缓冲区</summary>
        int ReadBytes(Span<byte> buffer);

        /// <summary>读取字节数组 (可为 null)</summary>
        byte[] ReadNullableBytes();

        #endregion

        #region 特殊类型读取

        /// <summary>读取 GUID</summary>
        Guid ReadGuid();

        /// <summary>读取日期时间</summary>
        DateTime ReadDateTime();

        /// <summary>读取日期时间偏移</summary>
        DateTimeOffset ReadDateTimeOffset();

        /// <summary>读取时间跨度</summary>
        TimeSpan ReadTimeSpan();

        #endregion

        #region 集合读取

        /// <summary>
        /// 读取数组头 (元素数量)
        /// </summary>
        /// <returns>元素数量，-1 表示 null</returns>
        int ReadArrayHeader();

        /// <summary>
        /// 读取字典头 (键值对数量)
        /// </summary>
        /// <returns>键值对数量，-1 表示 null</returns>
        int ReadMapHeader();

        /// <summary>
        /// 读取集合头 (元素数量)
        /// </summary>
        /// <returns>元素数量，-1 表示 null</returns>
        int ReadCollectionHeader();

        #endregion

        #region 对象读取

        /// <summary>
        /// 检查下一个值是否为 null
        /// </summary>
        /// <returns>如果是 null 返回 true</returns>
        bool TryReadNull();

        /// <summary>
        /// 尝试读取 null 对象头标记
        /// </summary>
        /// <returns>如果是 null 对象返回 true</returns>
        bool TryReadNullObjectHeader();

        /// <summary>
        /// 读取对象 (使用指定格式化器)
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="formatter">格式化器</param>
        /// <returns>读取的对象</returns>
        T ReadObject<T>(IFormatter<T> formatter);

        /// <summary>
        /// 读取对象 (自动解析格式化器)
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <returns>读取的对象</returns>
        T ReadObject<T>();

        /// <summary>
        /// 读取类型信息
        /// </summary>
        /// <returns>类型</returns>
        Type ReadType();

        #endregion

        #region 原始数据读取

        /// <summary>
        /// 直接读取原始字节
        /// </summary>
        /// <param name="count">要读取的字节数</param>
        /// <returns>读取的字节数组</returns>
        byte[] ReadRaw(int count);

        /// <summary>
        /// 直接读取原始字节到缓冲区
        /// </summary>
        /// <param name="buffer">目标缓冲区</param>
        void ReadRaw(Span<byte> buffer);

        /// <summary>
        /// 获取可读取的 Span
        /// </summary>
        /// <param name="size">需要的大小</param>
        /// <returns>可读取的 Span</returns>
        ReadOnlySpan<byte> GetSpan(int size);

        /// <summary>
        /// 推进读取位置
        /// </summary>
        /// <param name="count">推进的字节数</param>
        void Advance(int count);

        /// <summary>
        /// 读取非托管类型值 (零拷贝)
        /// </summary>
        /// <typeparam name="T">非托管类型</typeparam>
        /// <returns>读取的值</returns>
        T ReadUnmanaged<T>() where T : unmanaged;

        /// <summary>
        /// 读取非托管类型数组 (零拷贝)
        /// </summary>
        /// <typeparam name="T">非托管类型</typeparam>
        /// <param name="destination">目标缓冲区</param>
        void ReadUnmanagedArray<T>(Span<T> destination) where T : unmanaged;

        #endregion

        #region 跳过操作

        /// <summary>
        /// 跳过指定字节数
        /// </summary>
        /// <param name="count">要跳过的字节数</param>
        void Skip(int count);

        /// <summary>
        /// 跳过下一个值 (自动检测类型)
        /// </summary>
        void SkipValue();

        #endregion

        #region 状态和属性

        /// <summary>获取当前读取位置</summary>
        long Position { get; }

        /// <summary>获取剩余可读字节数</summary>
        long Remaining { get; }

        /// <summary>获取总字节数</summary>
        long Length { get; }

        /// <summary>检查是否已到达末尾</summary>
        bool IsEnd { get; }

        /// <summary>获取当前深度</summary>
        int Depth { get; }

        /// <summary>获取反序列化选项</summary>
        DeserializeOptions Options { get; }

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

        #region 预览操作

        /// <summary>
        /// 预览下一个字节 (不移动位置)
        /// </summary>
        /// <returns>下一个字节</returns>
        byte PeekByte();

        /// <summary>
        /// 预览指定数量的字节 (不移动位置)
        /// </summary>
        /// <param name="count">要预览的字节数</param>
        /// <returns>预览的字节数组</returns>
        ReadOnlySpan<byte> Peek(int count);

        #endregion
    }
}
