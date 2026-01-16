// ==========================================================
// 文件名：IFixedTickable.cs
// 命名空间: AFramework.DI
// 依赖: 无
// ==========================================================

namespace AFramework.DI
{
    /// <summary>
    /// 固定更新接口
    /// <para>固定时间间隔调用，对应 Unity 的 FixedUpdate 生命周期</para>
    /// <para>执行顺序：IFixedTickable → IPostFixedTickable</para>
    /// </summary>
    /// <remarks>
    /// 使用场景：
    /// - 物理计算
    /// - 固定时间步长的逻辑
    /// - 网络同步
    /// </remarks>
    public interface IFixedTickable
    {
        /// <summary>
        /// 固定更新方法
        /// <para>在 Unity FixedUpdate 中调用</para>
        /// </summary>
        void FixedTick();
    }
}
