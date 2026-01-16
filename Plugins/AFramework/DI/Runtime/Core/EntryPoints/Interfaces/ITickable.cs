// ==========================================================
// 文件名：ITickable.cs
// 命名空间: AFramework.DI
// 依赖: 无
// ==========================================================

namespace AFramework.DI
{
    /// <summary>
    /// 更新接口
    /// <para>每帧调用，对应 Unity 的 Update 生命周期</para>
    /// <para>执行顺序：ITickable → IPostTickable</para>
    /// </summary>
    /// <remarks>
    /// 使用场景：
    /// - 游戏逻辑更新
    /// - 输入处理
    /// - 非物理相关的帧更新
    /// </remarks>
    public interface ITickable
    {
        /// <summary>
        /// 每帧更新方法
        /// <para>在 Unity Update 中调用</para>
        /// </summary>
        void Tick();
    }
}
