// ==========================================================
// 文件名：IPostInitializable.cs
// 命名空间: AFramework.DI
// 依赖: 无
// ==========================================================

namespace AFramework.DI
{
    /// <summary>
    /// 后初始化接口
    /// <para>在所有 IInitializable 执行完成后调用</para>
    /// <para>执行顺序：IInitializable → IPostInitializable → IStartable</para>
    /// </summary>
    /// <remarks>
    /// 使用场景：
    /// - 依赖其他服务初始化完成后的逻辑
    /// - 跨服务的初始化协调
    /// - 初始化验证
    /// </remarks>
    public interface IPostInitializable
    {
        /// <summary>
        /// 后初始化方法
        /// <para>在所有 IInitializable.Initialize() 执行完成后调用</para>
        /// </summary>
        void PostInitialize();
    }
}
