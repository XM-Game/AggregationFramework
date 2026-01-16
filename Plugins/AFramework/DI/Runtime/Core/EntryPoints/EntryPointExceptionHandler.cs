// ==========================================================
// 文件名：EntryPointExceptionHandler.cs
// 命名空间: AFramework.DI
// 依赖: System, UnityEngine
// ==========================================================

using System;
using UnityEngine;

namespace AFramework.DI
{
    /// <summary>
    /// 入口点异常处理器
    /// <para>统一处理入口点执行过程中的异常</para>
    /// </summary>
    public sealed class EntryPointExceptionHandler
    {
        #region 单例模式

        private static EntryPointExceptionHandler _default;

        /// <summary>
        /// 默认异常处理器实例
        /// </summary>
        public static EntryPointExceptionHandler Default => _default ??= new EntryPointExceptionHandler();

        #endregion

        #region 事件

        /// <summary>
        /// 异常发生时触发的事件
        /// </summary>
        public event Action<EntryPointExceptionEventArgs> OnException;

        #endregion

        #region 配置

        /// <summary>
        /// 是否在异常时继续执行其他入口点
        /// <para>默认为 true，即一个入口点异常不影响其他入口点执行</para>
        /// </summary>
        public bool ContinueOnException { get; set; } = true;

        /// <summary>
        /// 是否将异常输出到 Unity 控制台
        /// <para>默认为 true</para>
        /// </summary>
        public bool LogToConsole { get; set; } = true;

        #endregion

        #region 公共方法

        /// <summary>
        /// 处理入口点异常
        /// </summary>
        /// <param name="entryPoint">发生异常的入口点对象</param>
        /// <param name="exception">异常信息</param>
        /// <param name="phase">执行阶段</param>
        public void HandleException(object entryPoint, Exception exception, EntryPointPhase phase)
        {
            var args = new EntryPointExceptionEventArgs(entryPoint, exception, phase);

            // 输出到控制台
            if (LogToConsole)
            {
                LogException(args);
            }

            // 触发事件
            try
            {
                OnException?.Invoke(args);
            }
            catch (Exception eventException)
            {
                Debug.LogError($"[AFramework.DI] 入口点异常事件处理器发生异常: {eventException}");
            }

            // 如果不继续执行，则重新抛出异常
            if (!ContinueOnException)
            {
                throw new EntryPointException(
                    $"入口点 '{entryPoint.GetType().Name}' 在 {phase} 阶段执行失败",
                    exception);
            }
        }

        /// <summary>
        /// 重置为默认配置
        /// </summary>
        public void Reset()
        {
            ContinueOnException = true;
            LogToConsole = true;
            OnException = null;
        }

        #endregion

        #region 私有方法

        private void LogException(EntryPointExceptionEventArgs args)
        {
            var typeName = args.EntryPoint?.GetType().Name ?? "Unknown";
            var phaseName = GetPhaseDisplayName(args.Phase);

            Debug.LogError(
                $"[AFramework.DI] 入口点执行异常\n" +
                $"类型: {typeName}\n" +
                $"阶段: {phaseName}\n" +
                $"异常: {args.Exception}");
        }

        private string GetPhaseDisplayName(EntryPointPhase phase)
        {
            return phase switch
            {
                EntryPointPhase.Initialize => "初始化 (Initialize)",
                EntryPointPhase.PostInitialize => "后初始化 (PostInitialize)",
                EntryPointPhase.AsyncStart => "异步启动 (AsyncStart)",
                EntryPointPhase.Start => "启动 (Start)",
                EntryPointPhase.PostStart => "后启动 (PostStart)",
                EntryPointPhase.Tick => "更新 (Tick)",
                EntryPointPhase.PostTick => "后更新 (PostTick)",
                EntryPointPhase.FixedTick => "固定更新 (FixedTick)",
                EntryPointPhase.PostFixedTick => "后固定更新 (PostFixedTick)",
                EntryPointPhase.LateTick => "延迟更新 (LateTick)",
                EntryPointPhase.PostLateTick => "后延迟更新 (PostLateTick)",
                _ => phase.ToString()
            };
        }

        #endregion
    }

    #region 辅助类型

    /// <summary>
    /// 入口点执行阶段枚举
    /// </summary>
    public enum EntryPointPhase
    {
        /// <summary>初始化阶段</summary>
        Initialize,
        /// <summary>后初始化阶段</summary>
        PostInitialize,
        /// <summary>异步启动阶段</summary>
        AsyncStart,
        /// <summary>启动阶段</summary>
        Start,
        /// <summary>后启动阶段</summary>
        PostStart,
        /// <summary>更新阶段</summary>
        Tick,
        /// <summary>后更新阶段</summary>
        PostTick,
        /// <summary>固定更新阶段</summary>
        FixedTick,
        /// <summary>后固定更新阶段</summary>
        PostFixedTick,
        /// <summary>延迟更新阶段</summary>
        LateTick,
        /// <summary>后延迟更新阶段</summary>
        PostLateTick
    }

    /// <summary>
    /// 入口点异常事件参数
    /// </summary>
    public sealed class EntryPointExceptionEventArgs : EventArgs
    {
        /// <summary>
        /// 发生异常的入口点对象
        /// </summary>
        public object EntryPoint { get; }

        /// <summary>
        /// 异常信息
        /// </summary>
        public Exception Exception { get; }

        /// <summary>
        /// 执行阶段
        /// </summary>
        public EntryPointPhase Phase { get; }

        /// <summary>
        /// 异常发生时间
        /// </summary>
        public DateTime Timestamp { get; }

        public EntryPointExceptionEventArgs(object entryPoint, Exception exception, EntryPointPhase phase)
        {
            EntryPoint = entryPoint;
            Exception = exception;
            Phase = phase;
            Timestamp = DateTime.Now;
        }
    }

    /// <summary>
    /// 入口点异常
    /// </summary>
    public sealed class EntryPointException : Exception
    {
        public EntryPointException(string message) : base(message) { }
        public EntryPointException(string message, Exception innerException) : base(message, innerException) { }
    }

    #endregion
}
