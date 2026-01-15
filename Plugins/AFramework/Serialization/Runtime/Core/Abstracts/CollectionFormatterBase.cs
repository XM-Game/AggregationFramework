// ==========================================================
// 文件名：CollectionFormatterBase.cs
// 命名空间: AFramework.Serialization
// 依赖: System, System.Collections.Generic
// ==========================================================

using System;
using System.Collections.Generic;

namespace AFramework.Serialization
{
    /// <summary>
    /// 集合格式化器基类
    /// <para>为集合类型提供通用的序列化支持</para>
    /// </summary>
    /// <typeparam name="TCollection">集合类型</typeparam>
    /// <typeparam name="TElement">元素类型</typeparam>
    /// <remarks>
    /// 设计说明:
    /// 1. 提供集合序列化的通用框架
    /// 2. 支持元素格式化器注入
    /// 3. 子类只需实现集合操作方法
    /// 
    /// 使用示例:
    /// <code>
    /// public class ListFormatter&lt;T&gt; : CollectionFormatterBase&lt;List&lt;T&gt;, T&gt;
    /// {
    ///     protected override int GetCount(List&lt;T&gt; collection) => collection.Count;
    ///     protected override IEnumerable&lt;T&gt; GetElements(List&lt;T&gt; collection) => collection;
    ///     protected override List&lt;T&gt; CreateCollection(int count) => new List&lt;T&gt;(count);
    ///     protected override void AddElement(List&lt;T&gt; collection, T element) => collection.Add(element);
    /// }
    /// </code>
    /// </remarks>
    public abstract class CollectionFormatterBase<TCollection, TElement> : FormatterBase<TCollection>
    {
        #region 字段

        /// <summary>元素格式化器</summary>
        protected IFormatter<TElement> _elementFormatter;

        #endregion

        #region 属性

        /// <inheritdoc/>
        public override bool SupportsNull => true;

        /// <inheritdoc/>
        public override bool IsFixedSize => false;

        #endregion

        #region 构造函数

        /// <summary>
        /// 创建集合格式化器
        /// </summary>
        protected CollectionFormatterBase()
        {
        }

        /// <summary>
        /// 创建集合格式化器
        /// </summary>
        /// <param name="elementFormatter">元素格式化器</param>
        protected CollectionFormatterBase(IFormatter<TElement> elementFormatter)
        {
            _elementFormatter = elementFormatter;
        }

        #endregion

        #region 抽象方法

        /// <summary>
        /// 获取集合元素数量
        /// </summary>
        protected abstract int GetCount(TCollection collection);

        /// <summary>
        /// 获取集合元素枚举器
        /// </summary>
        protected abstract IEnumerable<TElement> GetElements(TCollection collection);

        /// <summary>
        /// 创建集合实例
        /// </summary>
        /// <param name="count">元素数量</param>
        protected abstract TCollection CreateCollection(int count);

        /// <summary>
        /// 添加元素到集合
        /// </summary>
        protected abstract void AddElement(TCollection collection, TElement element);

        #endregion

        #region 重写方法

        /// <inheritdoc/>
        protected override void SerializeCore(ISerializeWriter writer, TCollection value, SerializeOptions options)
        {
            var count = GetCount(value);
            writer.WriteCollectionHeader(count);

            foreach (var element in GetElements(value))
            {
                SerializeElement(writer, element, options);
            }
        }

        /// <inheritdoc/>
        protected override TCollection DeserializeCore(ISerializeReader reader, DeserializeOptions options)
        {
            var count = reader.ReadCollectionHeader();
            var collection = CreateCollection(count);

            for (var i = 0; i < count; i++)
            {
                var element = DeserializeElement(reader, options);
                AddElement(collection, element);
            }

            return collection;
        }

        #endregion

        #region 虚方法

        /// <summary>
        /// 序列化单个元素
        /// </summary>
        protected virtual void SerializeElement(ISerializeWriter writer, TElement element, SerializeOptions options)
        {
            if (_elementFormatter != null)
            {
                _elementFormatter.Serialize(writer, element, options);
            }
            else
            {
                writer.WriteObject(element);
            }
        }

        /// <summary>
        /// 反序列化单个元素
        /// </summary>
        protected virtual TElement DeserializeElement(ISerializeReader reader, DeserializeOptions options)
        {
            if (_elementFormatter != null)
            {
                return _elementFormatter.Deserialize(reader, options);
            }
            else
            {
                return reader.ReadObject<TElement>();
            }
        }

        #endregion

        #region 初始化

        /// <summary>
        /// 设置元素格式化器
        /// </summary>
        /// <param name="formatter">元素格式化器</param>
        public void SetElementFormatter(IFormatter<TElement> formatter)
        {
            _elementFormatter = formatter;
        }

        #endregion
    }

    /// <summary>
    /// 字典格式化器基类
    /// </summary>
    /// <typeparam name="TDictionary">字典类型</typeparam>
    /// <typeparam name="TKey">键类型</typeparam>
    /// <typeparam name="TValue">值类型</typeparam>
    public abstract class DictionaryFormatterBase<TDictionary, TKey, TValue> : FormatterBase<TDictionary>
    {
        #region 字段

        /// <summary>键格式化器</summary>
        protected IFormatter<TKey> _keyFormatter;

        /// <summary>值格式化器</summary>
        protected IFormatter<TValue> _valueFormatter;

        #endregion

        #region 属性

        /// <inheritdoc/>
        public override bool SupportsNull => true;

        /// <inheritdoc/>
        public override bool IsFixedSize => false;

        #endregion

        #region 构造函数

        /// <summary>
        /// 创建字典格式化器
        /// </summary>
        protected DictionaryFormatterBase()
        {
        }

        /// <summary>
        /// 创建字典格式化器
        /// </summary>
        /// <param name="keyFormatter">键格式化器</param>
        /// <param name="valueFormatter">值格式化器</param>
        protected DictionaryFormatterBase(IFormatter<TKey> keyFormatter, IFormatter<TValue> valueFormatter)
        {
            _keyFormatter = keyFormatter;
            _valueFormatter = valueFormatter;
        }

        #endregion

        #region 抽象方法

        /// <summary>
        /// 获取字典元素数量
        /// </summary>
        protected abstract int GetCount(TDictionary dictionary);

        /// <summary>
        /// 获取字典键值对枚举器
        /// </summary>
        protected abstract IEnumerable<KeyValuePair<TKey, TValue>> GetEntries(TDictionary dictionary);

        /// <summary>
        /// 创建字典实例
        /// </summary>
        /// <param name="count">元素数量</param>
        protected abstract TDictionary CreateDictionary(int count);

        /// <summary>
        /// 添加键值对到字典
        /// </summary>
        protected abstract void AddEntry(TDictionary dictionary, TKey key, TValue value);

        #endregion

        #region 重写方法

        /// <inheritdoc/>
        protected override void SerializeCore(ISerializeWriter writer, TDictionary value, SerializeOptions options)
        {
            var count = GetCount(value);
            writer.WriteMapHeader(count);

            foreach (var entry in GetEntries(value))
            {
                SerializeKey(writer, entry.Key, options);
                SerializeValue(writer, entry.Value, options);
            }
        }

        /// <inheritdoc/>
        protected override TDictionary DeserializeCore(ISerializeReader reader, DeserializeOptions options)
        {
            var count = reader.ReadMapHeader();
            var dictionary = CreateDictionary(count);

            for (var i = 0; i < count; i++)
            {
                var key = DeserializeKey(reader, options);
                var value = DeserializeValue(reader, options);
                AddEntry(dictionary, key, value);
            }

            return dictionary;
        }

        #endregion

        #region 虚方法

        /// <summary>
        /// 序列化键
        /// </summary>
        protected virtual void SerializeKey(ISerializeWriter writer, TKey key, SerializeOptions options)
        {
            if (_keyFormatter != null)
                _keyFormatter.Serialize(writer, key, options);
            else
                writer.WriteObject(key);
        }

        /// <summary>
        /// 序列化值
        /// </summary>
        protected virtual void SerializeValue(ISerializeWriter writer, TValue value, SerializeOptions options)
        {
            if (_valueFormatter != null)
                _valueFormatter.Serialize(writer, value, options);
            else
                writer.WriteObject(value);
        }

        /// <summary>
        /// 反序列化键
        /// </summary>
        protected virtual TKey DeserializeKey(ISerializeReader reader, DeserializeOptions options)
        {
            if (_keyFormatter != null)
                return _keyFormatter.Deserialize(reader, options);
            else
                return reader.ReadObject<TKey>();
        }

        /// <summary>
        /// 反序列化值
        /// </summary>
        protected virtual TValue DeserializeValue(ISerializeReader reader, DeserializeOptions options)
        {
            if (_valueFormatter != null)
                return _valueFormatter.Deserialize(reader, options);
            else
                return reader.ReadObject<TValue>();
        }

        #endregion

        #region 初始化

        /// <summary>
        /// 设置键格式化器
        /// </summary>
        public void SetKeyFormatter(IFormatter<TKey> formatter)
        {
            _keyFormatter = formatter;
        }

        /// <summary>
        /// 设置值格式化器
        /// </summary>
        public void SetValueFormatter(IFormatter<TValue> formatter)
        {
            _valueFormatter = formatter;
        }

        #endregion
    }
}
