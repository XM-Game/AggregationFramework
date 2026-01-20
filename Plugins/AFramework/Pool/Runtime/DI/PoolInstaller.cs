// ==========================================================
// 文件名：PoolInstaller.cs
// 命名空间: AFramework.Pool.DI
// 依赖: AFramework.DI, AFramework.Pool
// 功能: 对象池安装器
// ==========================================================

using AFramework.DI;

namespace AFramework.Pool.DI
{
    /// <summary>
    /// 对象池安装器基类
    /// Pool Installer Base Class
    /// </summary>
    /// <remarks>
    /// 继承此类以创建自定义的对象池安装器
    /// Inherit from this class to create custom pool installers
    /// </remarks>
    public abstract class PoolInstaller : InstallerBase
    {
        /// <summary>
        /// 安装对象池
        /// Install object pools
        /// </summary>
        public override void Install(IContainerBuilder builder)
        {
            InstallPools(builder);
        }

        /// <summary>
        /// 安装对象池（由子类实现）
        /// Install object pools (implemented by subclass)
        /// </summary>
        /// <param name="builder">容器构建器 / Container builder</param>
        protected abstract void InstallPools(IContainerBuilder builder);
    }
}
