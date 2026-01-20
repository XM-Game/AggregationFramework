// ==========================================================
// 文件名：IPoolCleanupPolicy.cs
// 命名空间: AFramework.Pool
// 依赖: 无
// 功能: 定义对象池清理策略接口，定义如何清理和销毁池化对象
// ==========================================================

namespace AFramework.Pool
{
    /// <summary>
    /// 对象池清理策略接口
    /// 定义如何清理和销毁池化对象
    /// </summary>
    /// <typeparam name="T">池化对象的类型</typeparam>
    /// <remarks>
    /// 设计原则：
    /// - 策略模式：封装对象清理算法
    /// - 单一职责原则：仅负责对象清理和销毁
    /// - 资源管理：确保资源正确释放
    /// 
    /// 内置策略：
    /// - DefaultCleanupPolicy：无操作（适用于简单值类型）
    /// - ResetCleanupPolicy：调用对象的 Reset() 方法
    /// - DisposeCleanupPolicy：调用 IDisposable.Dispose()
    /// - UnityDestroyPolicy：调用 Object.Destroy()
    /// - TimedCleanupPolicy：定时清理空闲对象
    /// - IdleCleanupPolicy：清理长时间未使用的对象（LRU）
    /// 
    /// 清理时机：
    /// - OnReturn：对象归还时清理状态
    /// - OnDestroy：对象从池中移除时销毁
    /// - OnShrink：池收缩时清理多余对象
    /// - OnClear：池清空时清理所有对象
    /// </remarks>
    public interface IPoolCleanupPolicy<T> : IPoolPolicy
    {
        /// <summary>
        /// 对象归还到池中时的清理操作
        /// </summary>
        /// <param name="obj">要清理的对象</param>
        /// <remarks>
        /// 调用时机：每次调用 pool.Return() 成功后
        /// 用途：重置对象状态，使其可以被再次使用
        /// 
        /// 实现示例：
        /// - 清空集合（List.Clear()）
        /// - 重置字段为默认值
        /// - 禁用 GameObject（SetActive(false)）
        /// - 停止协程和动画
        /// 
        /// 注意：
        /// - 此方法不应销毁对象，仅重置状态
        /// - 如果对象实现了 IPooledObject，会先调用 OnReturn()
        /// </remarks>
        void OnReturn(T obj);

        /// <summary>
        /// 对象从池中移除销毁时的清理操作
        /// </summary>
        /// <param name="obj">要销毁的对象</param>
        /// <remarks>
        /// 调用时机：
        /// - 池调用 Clear() 清空时
        /// - 池调用 Shrink() 收缩时
        /// - 池 Dispose() 销毁时
        /// 
        /// 用途：释放非托管资源，彻底销毁对象
        /// 
        /// 实现示例：
        /// - 调用 IDisposable.Dispose()
        /// - 调用 Object.Destroy()（Unity）
        /// - 卸载 Addressables 资源
        /// - 释放纹理/网格资源
        /// 
        /// 注意：
        /// - 此方法应彻底销毁对象
        /// - 如果对象实现了 IPooledObject，会先调用 OnDestroy()
        /// </remarks>
        void OnDestroy(T obj);

        /// <summary>
        /// 验证清理策略是否可用
        /// </summary>
        /// <returns>如果策略可用返回 true，否则返回 false</returns>
        bool Validate();
    }
}
