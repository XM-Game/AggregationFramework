// ==========================================================
// 文件名：IPoolComponent.cs
// 命名空间: AFramework.Pool
// 依赖: System
// 功能: 对象池组件接口，支持组合式功能扩展
// ==========================================================

using System;

namespace AFramework.Pool
{
    /// <summary>
    /// 对象池组件接口
    /// Object Pool Component Interface
    /// 
    /// <para>定义可组合的池功能组件契约</para>
    /// <para>Defines the contract for composable pool functionality components</para>
    /// </summary>
    /// <remarks>
    /// 设计模式：组合模式 (Composite Pattern)
    /// - 将功能模块化为独立组件
    /// - 支持动态添加/移除功能
    /// - 降低继承层次复杂度
    /// </remarks>
    public interface IPoolComponent : IDisposable
    {
        /// <summary>
        /// 组件名称
        /// Component name
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 组件是否已启用
        /// Whether the component is enabled
        /// </summary>
        bool IsEnabled { get; set; }

        /// <summary>
        /// 初始化组件
        /// Initialize the component
        /// </summary>
        void Initialize();

        /// <summary>
        /// 重置组件状态
        /// Reset component state
        /// </summary>
        void Reset();
    }
}
