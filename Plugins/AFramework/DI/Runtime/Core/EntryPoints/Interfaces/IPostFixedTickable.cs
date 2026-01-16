// ==========================================================
// 文件名：IPostFixedTickable.cs
// 命名空间: AFramework.DI
// 依赖: 无
// ==========================================================

namespace AFramework.DI
{
    /// <summary>
    /// 后固定更新接口
    /// <para>在所有 IFixedTickable 执行完成后调用</para>
    /// <para>执行顺序：IFixedTickable → IPostFixedTickable</para>
    /// </summary>
    /// <remarks>
    /// 使用场景：
    /// - 物理计算后的处理
    /// - 碰撞响应
    /// - 物理状态同步
    /// </remarks>
    public interface IPostFixedTickable
    {
        /// <summary>
        /// 后固定更新方法
        /// <para>在所有 IFixedTickable.FixedTick() 执行完成后调用</para>
        /// </summary>
        void PostFixedTick();
    }
}
