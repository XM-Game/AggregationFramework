// ==========================================================
// 文件名：IdleCleanupPolicy.cs
// 命名空间: AFramework.Pool
// 依赖: System, System.Collections.Generic
// 功能: 空闲清理策略，基于 LRU（最近最少使用）算法
// ==========================================================

using System;
using System.Collections.Generic;

namespace AFramework.Pool
{
    /// <summary>
    /// 空闲清理策略（LRU）
    /// Idle Cleanup Policy (LRU - Least Recently Used)
    /// 
    /// <para>基于 LRU 算法的空闲清理策略，优先清理最久未使用的对象</para>
    /// <para>Idle cleanup policy based on LRU algorithm, prioritizes cleaning up least recently used objects</para>
    /// </summary>
    /// <typeparam name="T">对象类型 / Object type</typeparam>
    /// <remarks>
    /// 使用场景：
    /// - 需要智能清理空闲对象
    /// - 优先保留最近使用的对象
    /// - 内存受限环境
    /// 
    /// 特性：
    /// - LRU 算法实现
    /// - 记录对象访问顺序
    /// - 支持最大空闲数量限制
    /// 
    /// 算法：
    /// - 每次归还时更新访问时间
    /// - 超过最大空闲数量时清理最久未使用的对象
    /// - 保持最近使用的对象在池中
    /// </remarks>
    public class IdleCleanupPolicy<T> : PoolPolicyBase<T>, IPoolCleanupPolicy<T>
    {
        #region Fields

        /// <summary>
        /// 访问顺序链表（LRU）
        /// Access order linked list (LRU)
        /// </summary>
        private readonly LinkedList<T> _accessOrder;

        /// <summary>
        /// 对象到节点的映射
        /// Object to node mapping
        /// </summary>
        private readonly Dictionary<T, LinkedListNode<T>> _objectToNode;

        /// <summary>
        /// 最大空闲数量
        /// Maximum idle count
        /// </summary>
        private readonly int _maxIdleCount;

        #endregion

        #region Properties

        /// <summary>
        /// 最大空闲数量
        /// Maximum idle count
        /// </summary>
        public int MaxIdleCount => _maxIdleCount;

        /// <summary>
        /// 当前空闲数量
        /// Current idle count
        /// </summary>
        public int CurrentIdleCount => _accessOrder.Count;

        #endregion

        #region Constructors

        /// <summary>
        /// 构造函数
        /// Constructor
        /// </summary>
        /// <param name="maxIdleCount">最大空闲数量（默认 50）/ Maximum idle count (default 50)</param>
        /// <param name="name">策略名称 / Policy name</param>
        public IdleCleanupPolicy(int maxIdleCount = 50, string name = null)
            : base(name ?? "IdleCleanupPolicy")
        {
            if (maxIdleCount <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maxIdleCount), "Max idle count must be greater than 0.");
            }

            _maxIdleCount = maxIdleCount;
            _accessOrder = new LinkedList<T>();
            _objectToNode = new Dictionary<T, LinkedListNode<T>>();
        }

        #endregion

        #region IPoolCleanupPolicy Implementation

        /// <inheritdoc />
        public void OnReturn(T obj)
        {
            ThrowIfDisposed();
            ThrowIfInvalidObject(obj);

            // 更新访问顺序（移到链表头部）
            // Update access order (move to head of list)
            UpdateAccessOrder(obj);

            // 检查是否超过最大空闲数量
            // Check if exceeded maximum idle count
            if (_accessOrder.Count > _maxIdleCount)
            {
                // 移除最久未使用的对象（链表尾部）
                // Remove least recently used object (tail of list)
                RemoveLeastRecentlyUsed();
            }
        }

        /// <inheritdoc />
        public void OnDestroy(T obj)
        {
            ThrowIfDisposed();
            ThrowIfInvalidObject(obj);

            // 从跟踪列表中移除
            // Remove from tracking list
            RemoveObject(obj);

            // 清理对象
            // Cleanup object
            CleanupObject(obj);
        }

        /// <inheritdoc />
        public bool Validate()
        {
            // 验证最大空闲数量
            // Validate maximum idle count
            return _maxIdleCount > 0;
        }

        #endregion

        #region LRU Methods

        /// <summary>
        /// 更新访问顺序
        /// Update access order
        /// </summary>
        /// <param name="obj">对象 / Object</param>
        private void UpdateAccessOrder(T obj)
        {
            // 如果对象已存在，先移除
            // If object exists, remove it first
            if (_objectToNode.TryGetValue(obj, out LinkedListNode<T> existingNode))
            {
                _accessOrder.Remove(existingNode);
            }

            // 添加到链表头部（最近使用）
            // Add to head of list (most recently used)
            LinkedListNode<T> newNode = _accessOrder.AddFirst(obj);
            _objectToNode[obj] = newNode;
        }

        /// <summary>
        /// 移除最久未使用的对象
        /// Remove least recently used object
        /// </summary>
        private void RemoveLeastRecentlyUsed()
        {
            if (_accessOrder.Count == 0)
            {
                return;
            }

            // 获取链表尾部对象（最久未使用）
            // Get tail object (least recently used)
            LinkedListNode<T> lruNode = _accessOrder.Last;
            T lruObject = lruNode.Value;

            // 从链表和字典中移除
            // Remove from list and dictionary
            _accessOrder.RemoveLast();
            _objectToNode.Remove(lruObject);

            // 清理对象
            // Cleanup object
            CleanupObject(lruObject);
        }

        /// <summary>
        /// 清理对象
        /// Cleanup object
        /// </summary>
        /// <param name="obj">要清理的对象 / Object to cleanup</param>
        protected virtual void CleanupObject(T obj)
        {
            // 子类可重写此方法执行自定义清理逻辑
            // Subclasses can override this method to perform custom cleanup logic

            // 如果对象实现了 IDisposable，调用 Dispose
            // If object implements IDisposable, call Dispose
            if (obj is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }

        /// <summary>
        /// 移除对象
        /// Remove object
        /// </summary>
        /// <param name="obj">要移除的对象 / Object to remove</param>
        /// <returns>是否成功移除 / Whether successfully removed</returns>
        public bool RemoveObject(T obj)
        {
            ThrowIfDisposed();

            if (_objectToNode.TryGetValue(obj, out LinkedListNode<T> node))
            {
                _accessOrder.Remove(node);
                _objectToNode.Remove(obj);
                return true;
            }

            return false;
        }

        /// <summary>
        /// 获取最久未使用的对象
        /// Get least recently used object
        /// </summary>
        /// <returns>最久未使用的对象，如果为空返回 default / Least recently used object, returns default if empty</returns>
        public T GetLeastRecentlyUsed()
        {
            ThrowIfDisposed();

            if (_accessOrder.Count > 0)
            {
                return _accessOrder.Last.Value;
            }

            return default;
        }

        /// <summary>
        /// 获取最近使用的对象
        /// Get most recently used object
        /// </summary>
        /// <returns>最近使用的对象，如果为空返回 default / Most recently used object, returns default if empty</returns>
        public T GetMostRecentlyUsed()
        {
            ThrowIfDisposed();

            if (_accessOrder.Count > 0)
            {
                return _accessOrder.First.Value;
            }

            return default;
        }

        #endregion

        #region Dispose Override

        /// <inheritdoc />
        protected override void OnDispose()
        {
            _accessOrder.Clear();
            _objectToNode.Clear();
            base.OnDispose();
        }

        #endregion
    }
}
