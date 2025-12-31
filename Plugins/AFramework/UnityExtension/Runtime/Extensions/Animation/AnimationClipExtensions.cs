// ==========================================================
// 文件名：AnimationClipExtensions.cs
// 命名空间: AFramework.UnityExtension
// 依赖: UnityEngine, System
// ==========================================================

using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace AFramework.UnityExtension
{
    /// <summary>
    /// AnimationClip 扩展方法
    /// <para>提供 AnimationClip 的查询和实用功能扩展</para>
    /// </summary>
    public static class AnimationClipExtensions
    {
        #region 属性查询

        /// <summary>
        /// 获取动画片段长度 (秒)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float GetLength(this AnimationClip clip)
        {
            return clip != null ? clip.length : 0f;
        }

        /// <summary>
        /// 获取动画片段帧率
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float GetFrameRate(this AnimationClip clip)
        {
            return clip != null ? clip.frameRate : 0f;
        }

        /// <summary>
        /// 获取动画片段总帧数
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetFrameCount(this AnimationClip clip)
        {
            return clip != null ? Mathf.RoundToInt(clip.length * clip.frameRate) : 0;
        }

        /// <summary>
        /// 检查是否为循环动画
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsLooping(this AnimationClip clip)
        {
            return clip != null && clip.isLooping;
        }

        /// <summary>
        /// 检查是否为人形动画
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsHumanMotion(this AnimationClip clip)
        {
            return clip != null && clip.isHumanMotion;
        }

        /// <summary>
        /// 检查是否为空动画
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsEmpty(this AnimationClip clip)
        {
            return clip == null || clip.empty;
        }

        /// <summary>
        /// 检查是否有效
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsValid(this AnimationClip clip)
        {
            return clip != null && !clip.empty && clip.length > 0f;
        }

        #endregion

        #region 时间转换

        /// <summary>
        /// 将归一化时间转换为实际时间
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float NormalizedToTime(this AnimationClip clip, float normalizedTime)
        {
            return clip != null ? normalizedTime * clip.length : 0f;
        }

        /// <summary>
        /// 将实际时间转换为归一化时间
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float TimeToNormalized(this AnimationClip clip, float time)
        {
            return clip != null && clip.length > 0f ? time / clip.length : 0f;
        }

        /// <summary>
        /// 将帧数转换为时间
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float FrameToTime(this AnimationClip clip, int frame)
        {
            return clip != null && clip.frameRate > 0f ? frame / clip.frameRate : 0f;
        }

        /// <summary>
        /// 将时间转换为帧数
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int TimeToFrame(this AnimationClip clip, float time)
        {
            return clip != null ? Mathf.RoundToInt(time * clip.frameRate) : 0;
        }

        #endregion

        #region 事件操作

        /// <summary>
        /// 获取所有动画事件
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static AnimationEvent[] GetEvents(this AnimationClip clip)
        {
            return clip?.events ?? Array.Empty<AnimationEvent>();
        }

        /// <summary>
        /// 获取动画事件数量
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetEventCount(this AnimationClip clip)
        {
            return clip?.events?.Length ?? 0;
        }

        /// <summary>
        /// 检查是否有指定名称的事件
        /// </summary>
        public static bool HasEvent(this AnimationClip clip, string functionName)
        {
            if (clip == null || clip.events == null) return false;

            foreach (var evt in clip.events)
            {
                if (evt.functionName == functionName)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// 添加动画事件
        /// </summary>
        public static void AddEvent(this AnimationClip clip, float time, string functionName)
        {
            if (clip == null) return;

            var evt = new AnimationEvent
            {
                time = time,
                functionName = functionName
            };

            var events = clip.events;
            var newEvents = new AnimationEvent[events.Length + 1];
            events.CopyTo(newEvents, 0);
            newEvents[events.Length] = evt;
            clip.events = newEvents;
        }

        /// <summary>
        /// 添加动画事件 (带参数)
        /// </summary>
        public static void AddEvent(this AnimationClip clip, float time, string functionName, float floatParameter)
        {
            if (clip == null) return;

            var evt = new AnimationEvent
            {
                time = time,
                functionName = functionName,
                floatParameter = floatParameter
            };

            var events = clip.events;
            var newEvents = new AnimationEvent[events.Length + 1];
            events.CopyTo(newEvents, 0);
            newEvents[events.Length] = evt;
            clip.events = newEvents;
        }

        /// <summary>
        /// 添加动画事件 (带整数参数)
        /// </summary>
        public static void AddEventWithInt(this AnimationClip clip, float time, string functionName, int intParameter)
        {
            if (clip == null) return;

            var evt = new AnimationEvent
            {
                time = time,
                functionName = functionName,
                intParameter = intParameter
            };

            var events = clip.events;
            var newEvents = new AnimationEvent[events.Length + 1];
            events.CopyTo(newEvents, 0);
            newEvents[events.Length] = evt;
            clip.events = newEvents;
        }

        /// <summary>
        /// 添加动画事件 (带字符串参数)
        /// </summary>
        public static void AddEventWithString(this AnimationClip clip, float time, string functionName, string stringParameter)
        {
            if (clip == null) return;

            var evt = new AnimationEvent
            {
                time = time,
                functionName = functionName,
                stringParameter = stringParameter
            };

            var events = clip.events;
            var newEvents = new AnimationEvent[events.Length + 1];
            events.CopyTo(newEvents, 0);
            newEvents[events.Length] = evt;
            clip.events = newEvents;
        }

        /// <summary>
        /// 清除所有动画事件
        /// </summary>
        public static void ClearEvents(this AnimationClip clip)
        {
            if (clip != null)
            {
                clip.events = Array.Empty<AnimationEvent>();
            }
        }

        /// <summary>
        /// 移除指定名称的事件
        /// </summary>
        public static void RemoveEvents(this AnimationClip clip, string functionName)
        {
            if (clip == null || clip.events == null) return;

            var events = clip.events;
            int count = 0;

            foreach (var evt in events)
            {
                if (evt.functionName != functionName)
                    count++;
            }

            if (count == events.Length) return;

            var newEvents = new AnimationEvent[count];
            int index = 0;

            foreach (var evt in events)
            {
                if (evt.functionName != functionName)
                    newEvents[index++] = evt;
            }

            clip.events = newEvents;
        }

        #endregion

        #region 采样

        /// <summary>
        /// 在指定时间采样动画
        /// </summary>
        public static void SampleAt(this AnimationClip clip, GameObject target, float time)
        {
            if (clip == null || target == null) return;
            clip.SampleAnimation(target, time);
        }

        /// <summary>
        /// 在归一化时间采样动画
        /// </summary>
        public static void SampleAtNormalized(this AnimationClip clip, GameObject target, float normalizedTime)
        {
            if (clip == null || target == null) return;
            clip.SampleAnimation(target, normalizedTime * clip.length);
        }

        /// <summary>
        /// 在指定帧采样动画
        /// </summary>
        public static void SampleAtFrame(this AnimationClip clip, GameObject target, int frame)
        {
            if (clip == null || target == null) return;
            float time = clip.FrameToTime(frame);
            clip.SampleAnimation(target, time);
        }

        #endregion

        #region 边界

        /// <summary>
        /// 获取动画的本地边界
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Bounds GetLocalBounds(this AnimationClip clip)
        {
            return clip?.localBounds ?? new Bounds();
        }

        /// <summary>
        /// 检查是否有根运动曲线
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasRootCurves(this AnimationClip clip)
        {
            return clip != null && clip.hasRootCurves;
        }

        /// <summary>
        /// 检查是否有通用根变换
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasGenericRootTransform(this AnimationClip clip)
        {
            return clip != null && clip.hasGenericRootTransform;
        }

        /// <summary>
        /// 检查是否有运动浮点曲线
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasMotionFloatCurves(this AnimationClip clip)
        {
            return clip != null && clip.hasMotionFloatCurves;
        }

        /// <summary>
        /// 获取根运动的平均速度
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 GetAverageSpeed(this AnimationClip clip)
        {
            if (clip == null) return Vector3.zero;
            return clip.averageSpeed;
        }

        /// <summary>
        /// 获取根运动的平均角速度（弧度/秒）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float GetAverageAngularSpeed(this AnimationClip clip)
        {
            if (clip == null) return 0f;
            return clip.averageAngularSpeed;
        }

        #endregion
    }
}
