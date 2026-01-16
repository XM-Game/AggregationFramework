// ==========================================================
// 文件名：IInitializable.cs
// 命名空间: AFramework.DI
// 依赖: 无
// ==========================================================

namespace AFramework.DI
{
    /// <summary>
    /// 初始化接口
    /// <para>在容器构建完成后立即执行，用于同步初始化逻辑</para>
    /// <para>执行顺序：IInitializable → IPostInitializable → IStartable</para>
    /// </summary>
    /// <remarks>
    /// 使用场景：
    /// - 初始化配置数据
    /// - 设置初始状态
    /// - 注册事件监听器
    /// </remarks>
    public interface IInitializable
    {
        /// <summary>
        /// 初始化方法
        /// <para>在所有依赖注入完成后调用</para>
        /// </summary>
        void Initialize();
    }
}
