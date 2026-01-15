// ==========================================================
// 文件名：ObjectTracker.cs
// 命名空间: AFramework.Serialization
// 依赖: System, System.Collections.Generic
// ==========================================================

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace AFramework.Serialization.Internal
{
    /// <summary>
    /// 对象跟踪器
    /// <para>用于检测和处理循环引用</para>
    /// <para>支持对象引用的序列化和反序列化</para>
    /// </summary>
    /// <remarks>
    /// 设计说明:
    /// 1. 序列化时记录已序列化的对象，检测循环引用
    /// 2. 反序列化时记录已创建的对象，支持引用还原
    /// 3. 使用对象 ID 标识引用关系
    /// 
    /// 使用示例:
    /// <code>
    /// // 序列化时
    /// using var tracker = new ObjectTracker();
    /// if (tracker.TryGetObjectId(obj, out int id))
    ///     WriteObjectReference(id);
    /// else
    ///     tracker.TrackObject(obj);
    /// 
    /// // 反序列化时
    /// using var tracker = new ObjectTracker();
    /// tracker.RegisterObject(id, obj);
    /// var referenced = tracker.GetObject(refId);
    /// </code>
    /// </remarks>
    internal sealed class ObjectTracker : IDisposable
    {
        #region 常量

        /// <summary>默认初始容量</summary>
        private const int DefaultCapacity = 64;

        /// <summary>最大跟踪对象数量</summary>
        private const int MaxTrackedObjects = 100000;

        /// <summary>无效对象 ID</summary>
        public const int InvalidId = -1;

        #endregion

        #region 字段

        /// <summary>对象到 ID 的映射 (序列化用)</summary>
        private Dictionary<object, int> _objectToId;

        /// <summary>ID 到对象的映射 (反序列化用)</summary>
        private Dictionary<int, object> _idToObject;

        /// <summary>下一个可用的对象 ID</summary>
        private int _nextId;

        /// <summary>是否已释放</summary>
        private bool _disposed;

        /// <summary>是否启用循环引用检测</summary>
        private readonly bool _enableCircularReference;

        /// <summary>最大对象数量</summary>
        private readonly int _maxObjects;

        #endregion

        #region 构造函数

        /// <summary>
        /// 创建对象跟踪器
        /// </summary>
        /// <param name="enableCircularReference">是否启用循环引用检测</param>
        /// <param name="maxObjects">最大对象数量</param>
        public ObjectTracker(bool enableCircularReference = true, int maxObjects = MaxTrackedObjects)
        {
            _enableCircularReference = enableCircularReference;
            _maxObjects = maxObjects;
            _nextId = 0;

            if (enableCircularReference)
            {
                _objectToId = new Dictionary<object, int>(DefaultCapacity, ReferenceEqualityComparer.Instance);
                _idToObject = new Dictionary<int, object>(DefaultCapacity);
            }
        }

        #endregion

        #region 属性

        /// <summary>已跟踪的对象数量</summary>
        public int Count => _objectToId?.Count ?? 0;

        /// <summary>是否启用循环引用检测</summary>
        public bool IsEnabled => _enableCircularReference;

        /// <summary>是否已达到最大容量</summary>
        public bool IsFull => Count >= _maxObjects;

        #endregion

        #region 序列化方法

        /// <summary>
        /// 尝试获取对象的 ID (如果已跟踪)
        /// </summary>
        /// <param name="obj">对象</param>
        /// <param name="id">输出对象 ID</param>
        /// <returns>如果对象已被跟踪返回 true</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryGetObjectId(object obj, out int id)
        {
            id = InvalidId;

            if (!_enableCircularReference || obj == null)
                return false;

            return _objectToId.TryGetValue(obj, out id);
        }

        /// <summary>
        /// 跟踪对象并返回分配的 ID
        /// </summary>
        /// <param name="obj">要跟踪的对象</param>
        /// <returns>分配的对象 ID</returns>
        /// <exception cref="InvalidOperationException">超出最大对象数量时抛出</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int TrackObject(object obj)
        {
            if (!_enableCircularReference || obj == null)
                return InvalidId;

            if (_objectToId.Count >= _maxObjects)
                throw new InvalidOperationException($"对象跟踪器已达到最大容量 ({_maxObjects})");

            int id = _nextId++;
            _objectToId[obj] = id;
            _idToObject[id] = obj;
            return id;
        }

        /// <summary>
        /// 检查对象是否已被跟踪
        /// </summary>
        /// <param name="obj">对象</param>
        /// <returns>是否已被跟踪</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsTracked(object obj)
        {
            if (!_enableCircularReference || obj == null)
                return false;

            return _objectToId.ContainsKey(obj);
        }

        /// <summary>
        /// 尝试跟踪对象
        /// </summary>
        /// <param name="obj">要跟踪的对象</param>
        /// <param name="id">输出对象 ID</param>
        /// <param name="isNew">是否为新跟踪的对象</param>
        /// <returns>是否成功</returns>
        public bool TryTrackObject(object obj, out int id, out bool isNew)
        {
            id = InvalidId;
            isNew = false;

            if (!_enableCircularReference || obj == null)
                return false;

            if (_objectToId.TryGetValue(obj, out id))
            {
                isNew = false;
                return true;
            }

            if (_objectToId.Count >= _maxObjects)
                return false;

            id = _nextId++;
            _objectToId[obj] = id;
            _idToObject[id] = obj;
            isNew = true;
            return true;
        }

        #endregion

        #region 反序列化方法

        /// <summary>
        /// 注册对象 (反序列化时使用)
        /// </summary>
        /// <param name="id">对象 ID</param>
        /// <param name="obj">对象实例</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void RegisterObject(int id, object obj)
        {
            if (!_enableCircularReference || obj == null)
                return;

            _idToObject[id] = obj;
            _objectToId[obj] = id;
        }

        /// <summary>
        /// 预注册对象 ID (用于延迟初始化)
        /// </summary>
        /// <returns>预分配的对象 ID</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int ReserveId()
        {
            if (!_enableCircularReference)
                return InvalidId;

            return _nextId++;
        }

        /// <summary>
        /// 获取已注册的对象
        /// </summary>
        /// <param name="id">对象 ID</param>
        /// <returns>对象实例，未找到返回 null</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object GetObject(int id)
        {
            if (!_enableCircularReference || id == InvalidId)
                return null;

            return _idToObject.TryGetValue(id, out object obj) ? obj : null;
        }

        /// <summary>
        /// 尝试获取已注册的对象
        /// </summary>
        /// <param name="id">对象 ID</param>
        /// <param name="obj">输出对象实例</param>
        /// <returns>是否成功</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryGetObject(int id, out object obj)
        {
            obj = null;

            if (!_enableCircularReference || id == InvalidId)
                return false;

            return _idToObject.TryGetValue(id, out obj);
        }

        /// <summary>
        /// 获取已注册的对象 (泛型版本)
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="id">对象 ID</param>
        /// <returns>对象实例</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T GetObject<T>(int id) where T : class
        {
            return GetObject(id) as T;
        }

        /// <summary>
        /// 检查 ID 是否已注册
        /// </summary>
        /// <param name="id">对象 ID</param>
        /// <returns>是否已注册</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool HasObject(int id)
        {
            if (!_enableCircularReference || id == InvalidId)
                return false;

            return _idToObject.ContainsKey(id);
        }

        #endregion

        #region 管理方法

        /// <summary>
        /// 清空所有跟踪数据
        /// </summary>
        public void Clear()
        {
            _objectToId?.Clear();
            _idToObject?.Clear();
            _nextId = 0;
        }

        /// <summary>
        /// 重置跟踪器 (保留容量)
        /// </summary>
        public void Reset()
        {
            Clear();
        }

        #endregion

        #region IDisposable 实现

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            if (_disposed)
                return;

            Clear();
            _objectToId = null;
            _idToObject = null;
            _disposed = true;
        }

        #endregion

        #region 引用相等比较器

        /// <summary>
        /// 引用相等比较器
        /// <para>使用 ReferenceEquals 进行比较，避免值类型装箱问题</para>
        /// </summary>
        private sealed class ReferenceEqualityComparer : IEqualityComparer<object>
        {
            /// <summary>单例实例</summary>
            public static readonly ReferenceEqualityComparer Instance = new ReferenceEqualityComparer();

            private ReferenceEqualityComparer() { }

            /// <summary>比较两个对象是否引用相等</summary>
            public new bool Equals(object x, object y)
            {
                return ReferenceEquals(x, y);
            }

            /// <summary>获取对象的哈希码</summary>
            public int GetHashCode(object obj)
            {
                return RuntimeHelpers.GetHashCode(obj);
            }
        }

        #endregion
    }

    #region 字符串内化器

    /// <summary>
    /// 字符串内化器
    /// <para>用于字符串去重和复用</para>
    /// <para>减少重复字符串的内存占用</para>
    /// </summary>
    internal sealed class StringInternPool : IDisposable
    {
        #region 常量

        /// <summary>默认初始容量</summary>
        private const int DefaultCapacity = 128;

        /// <summary>最大字符串数量</summary>
        private const int MaxStrings = 50000;

        #endregion

        #region 字段

        /// <summary>字符串到 ID 的映射</summary>
        private Dictionary<string, int> _stringToId;

        /// <summary>ID 到字符串的映射</summary>
        private List<string> _idToString;

        /// <summary>是否已释放</summary>
        private bool _disposed;

        #endregion

        #region 构造函数

        /// <summary>
        /// 创建字符串内化器
        /// </summary>
        public StringInternPool()
        {
            _stringToId = new Dictionary<string, int>(DefaultCapacity);
            _idToString = new List<string>(DefaultCapacity);
        }

        #endregion

        #region 属性

        /// <summary>已内化的字符串数量</summary>
        public int Count => _idToString.Count;

        /// <summary>是否已达到最大容量</summary>
        public bool IsFull => Count >= MaxStrings;

        #endregion

        #region 序列化方法

        /// <summary>
        /// 尝试获取字符串的 ID
        /// </summary>
        /// <param name="str">字符串</param>
        /// <param name="id">输出字符串 ID</param>
        /// <returns>如果字符串已内化返回 true</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryGetStringId(string str, out int id)
        {
            id = -1;
            if (string.IsNullOrEmpty(str))
                return false;

            return _stringToId.TryGetValue(str, out id);
        }

        /// <summary>
        /// 内化字符串并返回 ID
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns>字符串 ID</returns>
        public int InternString(string str)
        {
            if (string.IsNullOrEmpty(str))
                return -1;

            if (_stringToId.TryGetValue(str, out int id))
                return id;

            if (_idToString.Count >= MaxStrings)
                return -1;

            id = _idToString.Count;
            _stringToId[str] = id;
            _idToString.Add(str);
            return id;
        }

        /// <summary>
        /// 尝试内化字符串
        /// </summary>
        /// <param name="str">字符串</param>
        /// <param name="id">输出字符串 ID</param>
        /// <param name="isNew">是否为新内化的字符串</param>
        /// <returns>是否成功</returns>
        public bool TryInternString(string str, out int id, out bool isNew)
        {
            id = -1;
            isNew = false;

            if (string.IsNullOrEmpty(str))
                return false;

            if (_stringToId.TryGetValue(str, out id))
            {
                isNew = false;
                return true;
            }

            if (_idToString.Count >= MaxStrings)
                return false;

            id = _idToString.Count;
            _stringToId[str] = id;
            _idToString.Add(str);
            isNew = true;
            return true;
        }

        #endregion

        #region 反序列化方法

        /// <summary>
        /// 注册字符串 (反序列化时使用)
        /// </summary>
        /// <param name="id">字符串 ID</param>
        /// <param name="str">字符串</param>
        public void RegisterString(int id, string str)
        {
            // 确保列表有足够容量
            while (_idToString.Count <= id)
                _idToString.Add(null);

            _idToString[id] = str;
            if (!string.IsNullOrEmpty(str))
                _stringToId[str] = id;
        }

        /// <summary>
        /// 获取已注册的字符串
        /// </summary>
        /// <param name="id">字符串 ID</param>
        /// <returns>字符串，未找到返回 null</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string GetString(int id)
        {
            if (id < 0 || id >= _idToString.Count)
                return null;

            return _idToString[id];
        }

        #endregion

        #region 管理方法

        /// <summary>
        /// 清空所有数据
        /// </summary>
        public void Clear()
        {
            _stringToId.Clear();
            _idToString.Clear();
        }

        #endregion

        #region IDisposable 实现

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            if (_disposed)
                return;

            Clear();
            _stringToId = null;
            _idToString = null;
            _disposed = true;
        }

        #endregion
    }

    #endregion
}
