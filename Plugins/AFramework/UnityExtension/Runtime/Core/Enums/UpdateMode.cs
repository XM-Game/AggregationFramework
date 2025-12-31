// ==========================================================
// 文件名：UpdateMode.cs
// 命名空间: AFramework.UnityExtension
// 依赖: UnityEngine
// ==========================================================

using System;

namespace AFramework.UnityExtension
{
    /// <summary>
    /// 更新模式枚举
    /// <para>定义组件或系统的更新时机</para>
    /// </summary>
    [Serializable]
    public enum UpdateMode
    {
        /// <summary>不更新</summary>
        None = 0,

        /// <summary>在 Update 中更新 (每帧)</summary>
        Update = 1,

        /// <summary>在 LateUpdate 中更新 (每帧，在 Update 之后)</summary>
        LateUpdate = 2,

        /// <summary>在 FixedUpdate 中更新 (固定时间步长)</summary>
        FixedUpdate = 3,

        /// <summary>手动更新 (由外部调用)</summary>
        Manual = 4
    }

    /// <summary>
    /// 时间缩放模式枚举
    /// <para>定义时间是否受 Time.timeScale 影响</para>
    /// </summary>
    [Serializable]
    public enum TimeScaleMode
    {
        /// <summary>受时间缩放影响 (使用 Time.deltaTime)</summary>
        Scaled = 0,

        /// <summary>不受时间缩放影响 (使用 Time.unscaledDeltaTime)</summary>
        Unscaled = 1
    }

    /// <summary>
    /// 插值模式枚举
    /// <para>定义值变化的插值方式</para>
    /// </summary>
    [Serializable]
    public enum InterpolationMode
    {
        /// <summary>无插值 (立即变化)</summary>
        None = 0,

        /// <summary>线性插值</summary>
        Linear = 1,

        /// <summary>平滑插值 (SmoothStep)</summary>
        Smooth = 2,

        /// <summary>平滑阻尼 (SmoothDamp)</summary>
        SmoothDamp = 3,

        /// <summary>弹簧效果</summary>
        Spring = 4,

        /// <summary>缓入</summary>
        EaseIn = 5,

        /// <summary>缓出</summary>
        EaseOut = 6,

        /// <summary>缓入缓出</summary>
        EaseInOut = 7
    }

    /// <summary>
    /// 循环模式枚举
    /// <para>定义动画或行为的循环方式</para>
    /// </summary>
    [Serializable]
    public enum LoopMode
    {
        /// <summary>不循环 (执行一次后停止)</summary>
        None = 0,

        /// <summary>循环 (从头开始重复)</summary>
        Loop = 1,

        /// <summary>乒乓循环 (来回往复)</summary>
        PingPong = 2,

        /// <summary>钳制 (到达终点后保持)</summary>
        Clamp = 3,

        /// <summary>钳制后销毁</summary>
        ClampAndDestroy = 4
    }

    /// <summary>
    /// 播放状态枚举
    /// </summary>
    [Serializable]
    public enum PlayState
    {
        /// <summary>停止</summary>
        Stopped = 0,

        /// <summary>播放中</summary>
        Playing = 1,

        /// <summary>暂停</summary>
        Paused = 2,

        /// <summary>已完成</summary>
        Completed = 3
    }

    /// <summary>
    /// UpdateMode 扩展方法
    /// </summary>
    public static class UpdateModeExtensions
    {
        /// <summary>
        /// 获取对应的 deltaTime
        /// </summary>
        /// <param name="mode">更新模式</param>
        /// <param name="timeScaleMode">时间缩放模式</param>
        /// <returns>deltaTime 值</returns>
        public static float GetDeltaTime(this UpdateMode mode, TimeScaleMode timeScaleMode = TimeScaleMode.Scaled)
        {
            bool unscaled = timeScaleMode == TimeScaleMode.Unscaled;

            return mode switch
            {
                UpdateMode.Update or UpdateMode.LateUpdate => 
                    unscaled ? UnityEngine.Time.unscaledDeltaTime : UnityEngine.Time.deltaTime,
                UpdateMode.FixedUpdate => 
                    unscaled ? UnityEngine.Time.fixedUnscaledDeltaTime : UnityEngine.Time.fixedDeltaTime,
                _ => 0f
            };
        }

        /// <summary>
        /// 检查是否为帧更新模式
        /// </summary>
        public static bool IsFrameUpdate(this UpdateMode mode)
        {
            return mode == UpdateMode.Update || mode == UpdateMode.LateUpdate;
        }

        /// <summary>
        /// 检查是否为物理更新模式
        /// </summary>
        public static bool IsPhysicsUpdate(this UpdateMode mode)
        {
            return mode == UpdateMode.FixedUpdate;
        }
    }

    /// <summary>
    /// TimeScaleMode 扩展方法
    /// </summary>
    public static class TimeScaleModeExtensions
    {
        /// <summary>
        /// 获取 deltaTime
        /// </summary>
        public static float GetDeltaTime(this TimeScaleMode mode)
        {
            return mode == TimeScaleMode.Unscaled 
                ? UnityEngine.Time.unscaledDeltaTime 
                : UnityEngine.Time.deltaTime;
        }

        /// <summary>
        /// 获取 fixedDeltaTime
        /// </summary>
        public static float GetFixedDeltaTime(this TimeScaleMode mode)
        {
            return mode == TimeScaleMode.Unscaled 
                ? UnityEngine.Time.fixedUnscaledDeltaTime 
                : UnityEngine.Time.fixedDeltaTime;
        }

        /// <summary>
        /// 获取当前时间
        /// </summary>
        public static float GetTime(this TimeScaleMode mode)
        {
            return mode == TimeScaleMode.Unscaled 
                ? UnityEngine.Time.unscaledTime 
                : UnityEngine.Time.time;
        }
    }

    /// <summary>
    /// LoopMode 扩展方法
    /// </summary>
    public static class LoopModeExtensions
    {
        /// <summary>
        /// 检查是否会循环
        /// </summary>
        public static bool IsLooping(this LoopMode mode)
        {
            return mode == LoopMode.Loop || mode == LoopMode.PingPong;
        }

        /// <summary>
        /// 计算循环后的进度值 (0-1)
        /// </summary>
        /// <param name="mode">循环模式</param>
        /// <param name="progress">原始进度值</param>
        /// <returns>处理后的进度值</returns>
        public static float ProcessProgress(this LoopMode mode, float progress)
        {
            return mode switch
            {
                LoopMode.None => UnityEngine.Mathf.Clamp01(progress),
                LoopMode.Loop => progress - UnityEngine.Mathf.Floor(progress),
                LoopMode.PingPong => UnityEngine.Mathf.PingPong(progress, 1f),
                LoopMode.Clamp or LoopMode.ClampAndDestroy => UnityEngine.Mathf.Clamp01(progress),
                _ => progress
            };
        }

        /// <summary>
        /// 检查是否已完成 (对于非循环模式)
        /// </summary>
        /// <param name="mode">循环模式</param>
        /// <param name="progress">当前进度</param>
        /// <returns>如果已完成返回 true</returns>
        public static bool IsCompleted(this LoopMode mode, float progress)
        {
            if (mode.IsLooping())
                return false;
            return progress >= 1f;
        }
    }
}
