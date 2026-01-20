// ==========================================================
// 文件名：IPoolCreationPolicy.cs
// 命名空间: AFramework.Pool
// 依赖: 无
// 功能: 定义对象池创建策略接口，定义如何创建池化对象
// ==========================================================

namespace AFramework.Pool
{
    /// <summary>
    /// 对象池创建策略接口
    /// 定义如何创建池化对象
    /// </summary>
    /// <typeparam name="T">池化对象的类型</typeparam>
    /// <remarks>
    /// 设计原则：
    /// - 策略模式：封装对象创建算法
    /// - 工厂方法模式：延迟对象实例化到具体策略
    /// - 单一职责原则：仅负责对象创建
    /// 
    /// 内置策略：
    /// - DefaultCreationPolicy：使用 new T() 创建
    /// - FactoryCreationPolicy：使用工厂方法创建
    /// - ActivatorCreationPolicy：使用 Activator.CreateInstance 创建
    /// - UnityInstantiatePolicy：使用 Object.Instantiate 创建
    /// - AddressableCreationPolicy：使用 Addressables 异步创建
    /// 
    /// 使用场景：
    /// - 纯 C# 对象：DefaultCreationPolicy
    /// - 需要依赖注入：FactoryCreationPolicy
    /// - Unity GameObject：UnityInstantiatePolicy
    /// - 异步加载资源：AddressableCreationPolicy
    /// </remarks>
    public interface IPoolCreationPolicy<T> : IPoolPolicy
    {
        /// <summary>
        /// 创建一个新的对象实例
        /// </summary>
        /// <returns>新创建的对象实例</returns>
        /// <exception cref="PoolCreationException">对象创建失败</exception>
        /// <remarks>
        /// 实现要点：
        /// - 确保返回的对象处于初始状态
        /// - 如果对象实现了 IPooledObject，创建后会自动调用 OnCreate()
        /// - 创建失败应抛出 PoolCreationException 而非返回 null
        /// </remarks>
        T Create();

        /// <summary>
        /// 验证创建策略是否可用
        /// </summary>
        /// <returns>如果策略可用返回 true，否则返回 false</returns>
        /// <remarks>
        /// 用于在池初始化时验证策略配置是否正确
        /// 例如：
        /// - 检查工厂方法是否为 null
        /// - 检查 Prefab 引用是否有效
        /// - 检查 Addressables 资源路径是否存在
        /// </remarks>
        bool Validate();
    }
}
