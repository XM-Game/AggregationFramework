// ==========================================================
// 文件名：PolymorphicFormatterBase.cs
// 命名空间: AFramework.Serialization
// 依赖: System, System.Collections.Generic
// ==========================================================

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace AFramework.Serialization
{
    /// <summary>
    /// 多态格式化器基类
    /// <para>支持接口和抽象类的多态序列化</para>
    /// </summary>
    /// <remarks>
    /// 设计说明:
    /// 1. 自动写入/读取类型 ID
    /// 2. 支持派生类型注册
    /// 3. 支持运行时类型解析
    /// 
    /// 使用示例:
    /// <code>
    /// public class AnimalFormatter : PolymorphicFormatterBase
    /// {
    ///     public override Type TargetType => typeof(IAnimal);
    ///     
    ///     public AnimalFormatter()
    ///     {
    ///         RegisterType(typeof(Dog), 1);
    ///         RegisterType(typeof(Cat), 2);
    ///     }
    ///     
    ///     protected override void SerializePolymorphic(ISerializeWriter writer, object value, Type actualType, SerializeOptions options)
    ///     {
    ///         // 根据实际类型序列化
    ///     }
    /// }
    /// </code>
    /// </remarks>
    public abstract class PolymorphicFormatterBase : FormatterBase, IPolymorphicFormatter
    {
        #region 字段

        /// <summary>类型到 ID 的映射</summary>
        private readonly Dictionary<Type, int> _typeToId = new Dictionary<Type, int>();

        /// <summary>ID 到类型的映射</summary>
        private readonly Dictionary<int, Type> _idToType = new Dictionary<int, Type>();

        /// <summary>映射锁</summary>
        private readonly object _mapLock = new object();

        #endregion

        #region IPolymorphicFormatter 实现

        /// <inheritdoc/>
        public Type[] GetKnownTypes()
        {
            lock (_mapLock)
            {
                var types = new Type[_typeToId.Count];
                _typeToId.Keys.CopyTo(types, 0);
                return types;
            }
        }

        /// <inheritdoc/>
        public Type[] GetDerivedTypes()
        {
            return GetKnownTypes();
        }

        /// <inheritdoc/>
        public int GetTypeId(Type type)
        {
            if (type == null)
                return -1;

            lock (_mapLock)
            {
                return _typeToId.TryGetValue(type, out var id) ? id : -1;
            }
        }

        /// <inheritdoc/>
        public Type GetTypeById(int typeId)
        {
            lock (_mapLock)
            {
                return _idToType.TryGetValue(typeId, out var type) ? type : null;
            }
        }

        /// <inheritdoc/>
        public void RegisterType(Type type, int typeId)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            if (typeId < 0)
                throw new ArgumentOutOfRangeException(nameof(typeId), "类型 ID 必须大于等于 0");

            lock (_mapLock)
            {
                // 检查重复注册
                if (_typeToId.ContainsKey(type))
                    throw new InvalidOperationException($"类型 {type.Name} 已注册");

                if (_idToType.ContainsKey(typeId))
                    throw new InvalidOperationException($"类型 ID {typeId} 已被使用");

                _typeToId[type] = typeId;
                _idToType[typeId] = type;
            }
        }

        /// <inheritdoc/>
        public void RegisterDerivedType(Type derivedType, int typeId)
        {
            RegisterType(derivedType, typeId);
        }

        #endregion

        #region 重写方法

        /// <inheritdoc/>
        protected sealed override void SerializeCore(ISerializeWriter writer, object value, SerializeOptions options)
        {
            var actualType = value.GetType();
            var typeId = GetTypeId(actualType);

            if (typeId < 0)
            {
                throw new InvalidOperationException(
                    $"类型 {actualType.Name} 未注册，无法进行多态序列化");
            }

            // 写入类型 ID
            writer.WriteInt32(typeId);

            // 序列化实际类型的数据
            SerializePolymorphic(writer, value, actualType, options);
        }

        /// <inheritdoc/>
        protected sealed override object DeserializeCore(ISerializeReader reader, DeserializeOptions options)
        {
            // 读取类型 ID
            var typeId = reader.ReadInt32();
            var actualType = GetTypeById(typeId);

            if (actualType == null)
            {
                throw new InvalidOperationException(
                    $"类型 ID {typeId} 未注册，无法进行多态反序列化");
            }

            // 反序列化实际类型的数据
            return DeserializePolymorphic(reader, actualType, options);
        }

        #endregion

        #region 抽象方法

        /// <summary>
        /// 序列化多态类型
        /// </summary>
        /// <param name="writer">序列化写入器</param>
        /// <param name="value">要序列化的对象</param>
        /// <param name="actualType">实际类型</param>
        /// <param name="options">序列化选项</param>
        protected abstract void SerializePolymorphic(ISerializeWriter writer, object value, Type actualType, SerializeOptions options);

        /// <summary>
        /// 反序列化多态类型
        /// </summary>
        /// <param name="reader">序列化读取器</param>
        /// <param name="actualType">实际类型</param>
        /// <param name="options">反序列化选项</param>
        /// <returns>反序列化的对象</returns>
        protected abstract object DeserializePolymorphic(ISerializeReader reader, Type actualType, DeserializeOptions options);

        #endregion

        #region 辅助方法

        /// <summary>
        /// 注册类型 (泛型版本)
        /// </summary>
        /// <typeparam name="T">要注册的类型</typeparam>
        /// <param name="typeId">类型 ID</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void RegisterType<T>(int typeId)
        {
            RegisterType(typeof(T), typeId);
        }

        /// <summary>
        /// 尝试获取类型 ID
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="typeId">输出类型 ID</param>
        /// <returns>如果找到返回 true</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected bool TryGetTypeId(Type type, out int typeId)
        {
            lock (_mapLock)
            {
                return _typeToId.TryGetValue(type, out typeId);
            }
        }

        /// <summary>
        /// 获取已注册类型数量
        /// </summary>
        protected int RegisteredTypeCount
        {
            get
            {
                lock (_mapLock)
                {
                    return _typeToId.Count;
                }
            }
        }

        #endregion
    }
}
