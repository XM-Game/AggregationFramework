// ==========================================================
// 文件名：IFormatter.cs
// 命名空间: AFramework.Serialization
// 依赖: System
// ==========================================================

using System;

namespace AFramework.Serialization
{
    /// <summary>
    /// 格式化器接口
    /// <para>定义特定类型的序列化和反序列化逻辑</para>
    /// <para>非泛型版本，用于运行时类型处理</para>
    /// </summary>
    /// <remarks>
    /// 格式化器负责将特定类型转换为二进制格式，是序列化系统的核心组件。
    /// 每种类型都有对应的格式化器，序列化器通过格式化器解析器获取合适的格式化器。
    /// 
    /// 使用示例:
    /// <code>
    /// public class PlayerFormatter : IFormatter
    /// {
    ///     public Type TargetType => typeof(Player);
    ///     
    ///     public void Serialize(ISerializeWriter writer, object value, SerializeOptions options)
    ///     {
    ///         var player = (Player)value;
    ///         writer.WriteString(player.Name);
    ///         writer.WriteInt32(player.Level);
    ///     }
    ///     
    ///     public object Deserialize(ISerializeReader reader, DeserializeOptions options)
    ///     {
    ///         return new Player
    ///         {
    ///             Name = reader.ReadString(),
    ///             Level = reader.ReadInt32()
    ///         };
    ///     }
    /// }
    /// </code>
    /// </remarks>
    public interface IFormatter
    {
        /// <summary>
        /// 获取目标类型
        /// </summary>
        Type TargetType { get; }

        /// <summary>
        /// 序列化对象
        /// </summary>
        /// <param name="writer">序列化写入器</param>
        /// <param name="value">要序列化的对象</param>
        /// <param name="options">序列化选项</param>
        void Serialize(ISerializeWriter writer, object value, SerializeOptions options);

        /// <summary>
        /// 反序列化对象
        /// </summary>
        /// <param name="reader">序列化读取器</param>
        /// <param name="options">反序列化选项</param>
        /// <returns>反序列化的对象</returns>
        object Deserialize(ISerializeReader reader, DeserializeOptions options);

        /// <summary>
        /// 反序列化到现有对象
        /// </summary>
        /// <param name="reader">序列化读取器</param>
        /// <param name="target">目标对象</param>
        /// <param name="options">反序列化选项</param>
        void DeserializeTo(ISerializeReader reader, object target, DeserializeOptions options);

        /// <summary>
        /// 获取序列化后的预估大小
        /// </summary>
        /// <param name="value">要序列化的对象</param>
        /// <returns>预估的字节数，-1 表示无法预估</returns>
        int GetSerializedSize(object value);
    }

    /// <summary>
    /// 可初始化的格式化器接口
    /// <para>支持在序列化前进行初始化操作</para>
    /// </summary>
    public interface IInitializableFormatter : IFormatter
    {
        /// <summary>
        /// 初始化格式化器
        /// </summary>
        /// <param name="resolver">格式化器解析器</param>
        void Initialize(IFormatterResolver resolver);

        /// <summary>
        /// 是否已初始化
        /// </summary>
        bool IsInitialized { get; }
    }

    /// <summary>
    /// 支持版本迁移的格式化器接口
    /// <para>用于处理数据版本变更</para>
    /// </summary>
    public interface IVersionedFormatter : IFormatter
    {
        /// <summary>
        /// 获取当前版本
        /// </summary>
        int CurrentVersion { get; }

        /// <summary>
        /// 获取最小支持版本
        /// </summary>
        int MinSupportedVersion { get; }

        /// <summary>
        /// 检查版本是否支持
        /// </summary>
        /// <param name="version">数据版本</param>
        /// <returns>如果支持返回 true</returns>
        bool IsVersionSupported(int version);

        /// <summary>
        /// 检查是否可以从源版本迁移到目标版本
        /// </summary>
        /// <param name="fromVersion">源版本</param>
        /// <param name="toVersion">目标版本</param>
        /// <returns>如果可以迁移返回 true</returns>
        bool CanMigrate(int fromVersion, int toVersion);

        /// <summary>
        /// 序列化对象 (带版本)
        /// </summary>
        /// <param name="writer">序列化写入器</param>
        /// <param name="value">要序列化的对象</param>
        /// <param name="version">版本号</param>
        /// <param name="options">序列化选项</param>
        void SerializeVersion(ISerializeWriter writer, object value, int version, SerializeOptions options);

        /// <summary>
        /// 反序列化指定版本的数据
        /// </summary>
        /// <param name="reader">序列化读取器</param>
        /// <param name="version">数据版本</param>
        /// <param name="options">反序列化选项</param>
        /// <returns>反序列化的对象</returns>
        object DeserializeVersion(ISerializeReader reader, int version, DeserializeOptions options);
    }

    /// <summary>
    /// 多态格式化器接口
    /// <para>支持接口和抽象类的多态序列化</para>
    /// </summary>
    public interface IPolymorphicFormatter : IFormatter
    {
        /// <summary>
        /// 获取支持的派生类型
        /// </summary>
        /// <returns>派生类型数组</returns>
        Type[] GetDerivedTypes();

        /// <summary>
        /// 注册派生类型
        /// </summary>
        /// <param name="derivedType">派生类型</param>
        /// <param name="typeId">类型 ID</param>
        void RegisterDerivedType(Type derivedType, int typeId);

        /// <summary>
        /// 获取类型 ID
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns>类型 ID，-1 表示未注册</returns>
        int GetTypeId(Type type);

        /// <summary>
        /// 根据类型 ID 获取类型
        /// </summary>
        /// <param name="typeId">类型 ID</param>
        /// <returns>类型，null 表示未找到</returns>
        Type GetTypeById(int typeId);
    }
}
