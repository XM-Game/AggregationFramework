// ==========================================================
// 文件名：AllocatorUtility.cs
// 命名空间：AFramework.Burst
// 创建时间：2025-12-31
// 功能描述：分配器工具，提供分配器相关的实用方法和扩展
// ==========================================================

using System;
using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Collections;

namespace AFramework.Burst
{
    /// <summary>
    /// 分配器工具
    /// 提供分配器相关的实用方法和扩展
    /// </summary>
    public static class AllocatorUtility
    {
        #region 分配器检查

        /// <summary>
        /// 检查分配器是否有效
        /// </summary>
        /// <param name="allocator">分配器</param>
        /// <returns>如果分配器有效返回true</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsValid(Allocator allocator)
        {
            return allocator > Allocator.None && allocator <= Allocator.Persistent;
        }

        /// <summary>
        /// 检查分配器是否为临时分配器
        /// </summary>
        /// <param name="allocator">分配器</param>
        /// <returns>如果是临时分配器返回true</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsTemporary(Allocator allocator)
        {
            return allocator == Allocator.Temp || allocator == Allocator.TempJob;
        }

        /// <summary>
        /// 检查分配器是否为持久分配器
        /// </summary>
        /// <param name="allocator">分配器</param>
        /// <returns>如果是持久分配器返回true</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsPersistent(Allocator allocator)
        {
            return allocator == Allocator.Persistent;
        }

        /// <summary>
        /// 验证分配器，如果无效则抛出异常
        /// </summary>
        /// <param name="allocator">分配器</param>
        /// <param name="paramName">参数名称</param>
        /// <exception cref="ArgumentException">分配器无效时抛出</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Validate(Allocator allocator, string paramName = "allocator")
        {
            if (!IsValid(allocator))
                throw new ArgumentException($"无效的分配器: {allocator}", paramName);
        }

        #endregion

        #region 分配器选择

        /// <summary>
        /// 根据生命周期选择分配器
        /// </summary>
        /// <param name="isPersistent">是否持久化</param>
        /// <param name="isJob">是否用于Job</param>
        /// <returns>合适的分配器</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Allocator SelectAllocator(bool isPersistent, bool isJob = false)
        {
            if (isPersistent)
                return Allocator.Persistent;
            
            return isJob ? Allocator.TempJob : Allocator.Temp;
        }

        /// <summary>
        /// 获取推荐的分配器（基于使用场景）
        /// </summary>
        /// <param name="lifetime">生命周期类型</param>
        /// <returns>推荐的分配器</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Allocator GetRecommendedAllocator(AllocatorLifetime lifetime)
        {
            return lifetime switch
            {
                AllocatorLifetime.Temp => Allocator.Temp,
                AllocatorLifetime.TempJob => Allocator.TempJob,
                AllocatorLifetime.Persistent => Allocator.Persistent,
                _ => Allocator.TempJob
            };
        }

        #endregion

        #region 分配器信息

        /// <summary>
        /// 获取分配器的友好名称
        /// </summary>
        /// <param name="allocator">分配器</param>
        /// <returns>分配器名称</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetAllocatorName(Allocator allocator)
        {
            return allocator switch
            {
                Allocator.None => "None",
                Allocator.Temp => "Temp",
                Allocator.TempJob => "TempJob",
                Allocator.Persistent => "Persistent",
                _ => $"Unknown({(int)allocator})"
            };
        }

        /// <summary>
        /// 获取分配器的描述
        /// </summary>
        /// <param name="allocator">分配器</param>
        /// <returns>分配器描述</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetAllocatorDescription(Allocator allocator)
        {
            return allocator switch
            {
                Allocator.None => "无分配器",
                Allocator.Temp => "临时分配器（当前帧有效）",
                Allocator.TempJob => "临时Job分配器（Job完成后自动释放）",
                Allocator.Persistent => "持久分配器（需要手动释放）",
                _ => $"未知分配器({(int)allocator})"
            };
        }

        #endregion

        #region 辅助类型

        /// <summary>
        /// 分配器生命周期类型
        /// </summary>
        public enum AllocatorLifetime
        {
            /// <summary>临时（当前帧）</summary>
            Temp,
            /// <summary>临时Job（Job完成后）</summary>
            TempJob,
            /// <summary>持久（手动释放）</summary>
            Persistent
        }

        #endregion
    }
}

