// ==========================================================
// 文件名：IObjectPoolT.cs
// 命名空间: AFramework.Pool
// 依赖: System
// 功能: 定义泛型对象池接口，提供类型安全的对象池操作
// ==========================================================

using System;

namespace AFramework.Pool
{
    /// <summary>
    /// 泛型对象池接口
    /// 提供类型安全的对象池操作
    /// </summary>
    /// <typeparam name="T">池化对象的类型</typeparam>
    /// <remarks>
    /// 设计原则：
    /// - 类型安全：编译时类型检查，避免运行时类型转换
    /// - 协变支持：支持 IObjectPool&lt;out T&gt; 协变（仅获取操作）
    /// - 开闭原则（OCP）：通过泛型支持任意类型扩展
    /// 
    /// 使用场景：
    /// - 纯 C# 对象池（数据结构、业务对象）
    /// - Unity GameObject 池
    /// - 组件池（Component）
    /// </remarks>
    public interface IObjectPool<T> : IObjectPool
    {
        #region 泛型操作

        /// <summary>
        /// 从池中获取一个对象（泛型版本）
        /// </summary>
        /// <returns>类型为 T 的池化对象</returns>
        /// <exception cref="PoolDisposedException">池已被销毁</exception>
        /// <exception cref="PoolCapacityExceededException">池容量已满且无法扩容</exception>
        /// <exception cref="PoolCreationException">对象创建失败</exception>
        new T Get();

        /// <summary>
        /// 将对象归还到池中（泛型版本）
        /// </summary>
        /// <param name="obj">要归还的对象</param>
        /// <returns>如果成功归还返回 true，否则返回 false</returns>
        /// <exception cref="ArgumentNullException">obj 为 null</exception>
        /// <exception cref="PoolDisposedException">池已被销毁</exception>
        /// <exception cref="PoolReturnException">对象归还失败</exception>
        bool Return(T obj);

        #endregion

        #region 批量操作

        /// <summary>
        /// 批量获取多个对象
        /// </summary>
        /// <param name="count">要获取的对象数量</param>
        /// <returns>对象数组</returns>
        /// <exception cref="ArgumentOutOfRangeException">count 小于 0</exception>
        T[] GetMany(int count);

        /// <summary>
        /// 批量归还多个对象
        /// </summary>
        /// <param name="objects">要归还的对象集合</param>
        /// <returns>成功归还的对象数量</returns>
        /// <exception cref="ArgumentNullException">objects 为 null</exception>
        int ReturnMany(T[] objects);

        #endregion

        #region 高级操作

        /// <summary>
        /// 尝试从池中获取对象（不抛出异常）
        /// </summary>
        /// <param name="obj">输出参数，获取到的对象</param>
        /// <returns>如果成功获取返回 true，否则返回 false</returns>
        bool TryGet(out T obj);

        /// <summary>
        /// 租借对象（使用 using 语句自动归还）
        /// </summary>
        /// <returns>可释放的池化对象包装器</returns>
        /// <example>
        /// <code>
        /// using (var rental = pool.Rent())
        /// {
        ///     var obj = rental.Value;
        ///     // 使用对象
        /// } // 自动归还
        /// </code>
        /// </example>
        PooledObjectRental<T> Rent();

        #endregion
    }

    /// <summary>
    /// 池化对象租借包装器
    /// 实现 IDisposable 以支持 using 语句自动归还
    /// </summary>
    /// <typeparam name="T">池化对象的类型</typeparam>
    public readonly struct PooledObjectRental<T> : IDisposable
    {
        private readonly IObjectPool<T> _pool;
        private readonly T _value;

        /// <summary>
        /// 获取租借的对象
        /// </summary>
        public T Value => _value;

        /// <summary>
        /// 初始化 <see cref="PooledObjectRental{T}"/> 结构
        /// </summary>
        /// <param name="pool">对象池</param>
        /// <param name="value">租借的对象</param>
        public PooledObjectRental(IObjectPool<T> pool, T value)
        {
            _pool = pool;
            _value = value;
        }

        /// <summary>
        /// 释放资源，自动归还对象到池中
        /// </summary>
        public void Dispose()
        {
            if (_pool != null && _value != null)
            {
                _pool.Return(_value);
            }
        }
    }
}
