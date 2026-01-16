// ==========================================================
// 文件名：IPostTickable.cs
// 命名空间: AFramework.DI
// 依赖: 无
// ==========================================================

namespace AFramework.DI
{
    /// <summary>
    /// 后更新接口
    /// <para>在所有 ITickable 执行完成后调用</para>
    /// <para>执行顺序：ITickable → IPostTickable</para>
    /// </summary>
    /// <remarks>
    /// 使用场景：
    /// - 依赖其他系统更新完成后的逻辑
    /// - 状态同步
    /// - 更新后的验证
    /// </remarks>
    public interface IPostTickable
    {
        /// <summary>
        /// 后更新方法
        /// <para>在所有 ITickable.Tick() 执行完成后调用</para>
        /// </summary>
        void PostTick();
    }
}
