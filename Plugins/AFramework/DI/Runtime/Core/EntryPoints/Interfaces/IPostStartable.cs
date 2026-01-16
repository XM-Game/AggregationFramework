// ==========================================================
// 文件名：IPostStartable.cs
// 命名空间: AFramework.DI
// 依赖: 无
// ==========================================================

namespace AFramework.DI
{
    /// <summary>
    /// 后启动接口
    /// <para>在所有 IStartable 执行完成后调用</para>
    /// <para>执行顺序：IStartable → IPostStartable → 进入更新循环</para>
    /// </summary>
    /// <remarks>
    /// 使用场景：
    /// - 依赖其他服务启动完成后的逻辑
    /// - 最终的启动验证
    /// - 启动完成通知
    /// </remarks>
    public interface IPostStartable
    {
        /// <summary>
        /// 后启动方法
        /// <para>在所有 IStartable.Start() 执行完成后调用</para>
        /// </summary>
        void PostStart();
    }
}
