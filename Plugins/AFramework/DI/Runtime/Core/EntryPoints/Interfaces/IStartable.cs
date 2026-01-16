// ==========================================================
// 文件名：IStartable.cs
// 命名空间: AFramework.DI
// 依赖: 无
// ==========================================================

namespace AFramework.DI
{
    /// <summary>
    /// 启动接口
    /// <para>在初始化阶段完成后执行，用于启动服务逻辑</para>
    /// <para>执行顺序：IAsyncStartable → IStartable → IPostStartable</para>
    /// </summary>
    /// <remarks>
    /// 使用场景：
    /// - 启动游戏逻辑
    /// - 开始监听输入
    /// - 启动定时器
    /// - 激活游戏系统
    /// </remarks>
    public interface IStartable
    {
        /// <summary>
        /// 启动方法
        /// <para>在所有初始化完成后调用</para>
        /// </summary>
        void Start();
    }
}
