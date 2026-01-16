// ==========================================================
// 文件名：IPostLateTickable.cs
// 命名空间: AFramework.DI
// 依赖: 无
// ==========================================================

namespace AFramework.DI
{
    /// <summary>
    /// 后延迟更新接口
    /// <para>在所有 ILateTickable 执行完成后调用</para>
    /// <para>执行顺序：ILateTickable → IPostLateTickable</para>
    /// </summary>
    /// <remarks>
    /// 使用场景：
    /// - 最终的帧处理
    /// - 渲染前的最后调整
    /// - 帧结束时的清理
    /// </remarks>
    public interface IPostLateTickable
    {
        /// <summary>
        /// 后延迟更新方法
        /// <para>在所有 ILateTickable.LateTick() 执行完成后调用</para>
        /// </summary>
        void PostLateTick();
    }
}
