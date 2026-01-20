// ==========================================================
// 文件名：IPooledObject.cs
// 命名空间: AFramework.Pool
// 依赖: 无
// 功能: 定义池化对象生命周期接口，提供对象池生命周期回调
// ==========================================================

namespace AFramework.Pool
{
    /// <summary>
    /// 池化对象生命周期接口
    /// 对象实现此接口可接收池化生命周期回调
    /// </summary>
    /// <remarks>
    /// 设计原则：
    /// - 单一职责原则（SRP）：仅负责生命周期通知
    /// - 接口隔离原则（ISP）：可选实现，不强制所有对象实现
    /// - 开闭原则（OCP）：通过接口扩展，不修改池核心逻辑
    /// 
    /// 生命周期流程：
    /// 1. OnCreate() - 对象首次创建时调用（仅一次）
    /// 2. OnGet() - 对象从池中获取时调用
    /// 3. [使用对象]
    /// 4. OnReturn() - 对象归还到池中时调用
    /// 5. OnDestroy() - 对象从池中移除销毁时调用（仅一次）
    /// 
    /// 使用场景：
    /// - 重置对象状态（清空数据、重置位置）
    /// - 启用/禁用组件
    /// - 注册/注销事件监听
    /// - 资源加载/卸载
    /// </remarks>
    public interface IPooledObject
    {
        /// <summary>
        /// 对象首次创建时调用
        /// </summary>
        /// <remarks>
        /// 调用时机：对象通过池的创建策略首次实例化后
        /// 调用次数：对象生命周期内仅调用一次
        /// 用途：初始化对象的不变状态（如缓存组件引用）
        /// </remarks>
        void OnCreate();

        /// <summary>
        /// 对象从池中获取时调用
        /// </summary>
        /// <remarks>
        /// 调用时机：每次调用 pool.Get() 成功后
        /// 调用次数：每次获取都会调用
        /// 用途：重置对象到初始状态、启用对象
        /// 
        /// Unity 示例：
        /// - gameObject.SetActive(true)
        /// - 重置 Transform 位置和旋转
        /// - 启用 Collider/Renderer
        /// </remarks>
        void OnGet();

        /// <summary>
        /// 对象归还到池中时调用
        /// </summary>
        /// <remarks>
        /// 调用时机：每次调用 pool.Return() 成功后
        /// 调用次数：每次归还都会调用
        /// 用途：清理对象状态、禁用对象
        /// 
        /// Unity 示例：
        /// - gameObject.SetActive(false)
        /// - 清空事件监听
        /// - 停止协程
        /// - 重置动画状态
        /// </remarks>
        void OnReturn();

        /// <summary>
        /// 对象从池中移除销毁时调用
        /// </summary>
        /// <remarks>
        /// 调用时机：
        /// - 池调用 Clear() 清空时
        /// - 池调用 Shrink() 收缩时
        /// - 池 Dispose() 销毁时
        /// 调用次数：对象生命周期内仅调用一次
        /// 用途：释放非托管资源、卸载资源
        /// 
        /// Unity 示例：
        /// - Object.Destroy(gameObject)
        /// - 卸载 Addressables 资源
        /// - 释放纹理/网格资源
        /// </remarks>
        void OnDestroy();
    }
}
