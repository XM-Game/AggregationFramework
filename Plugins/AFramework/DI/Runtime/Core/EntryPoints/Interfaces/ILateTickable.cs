// ==========================================================
// 文件名：ILateTickable.cs
// 命名空间: AFramework.DI
// 依赖: 无
// ==========================================================

namespace AFramework.DI
{
    /// <summary>
    /// 延迟更新接口
    /// <para>在所有 Update 完成后调用，对应 Unity 的 LateUpdate 生命周期</para>
    /// <para>执行顺序：ILateTickable → IPostLateTickable</para>
    /// </summary>
    /// <remarks>
    /// 使用场景：
    /// - 相机跟随
    /// - 动画后处理
    /// - 依赖其他对象位置更新后的逻辑
    /// </remarks>
    public interface ILateTickable
    {
        /// <summary>
        /// 延迟更新方法
        /// <para>在 Unity LateUpdate 中调用</para>
        /// </summary>
        void LateTick();
    }
}
