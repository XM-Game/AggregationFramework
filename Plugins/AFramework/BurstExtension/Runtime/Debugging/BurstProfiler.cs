// ==========================================================
// 文件名：BurstProfiler.cs
// 命名空间：AFramework.Burst
// 创建时间：2026-01-01
// 功能描述：Burst性能分析工具，提供轻量级的性能测量功能
// 依赖：Unity.Burst, Unity.Collections
// ==========================================================

using System.Diagnostics;
using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Collections;

namespace AFramework.Burst
{
    /// <summary>
    /// Burst性能分析工具
    /// 提供轻量级的性能测量和统计功能
    /// </summary>
    [BurstCompile]
    public static class BurstProfiler
    {
        #region 计时器结构

        /// <summary>
        /// 高精度计时器（基于Stopwatch Ticks）
        /// </summary>
        public struct Timer
        {
            private long _startTicks;
            private long _elapsedTicks;
            private bool _isRunning;

            /// <summary>
            /// 是否正在运行
            /// </summary>
            public bool IsRunning => _isRunning;

            /// <summary>
            /// 已经过的Ticks
            /// </summary>
            public long ElapsedTicks => _isRunning 
                ? _elapsedTicks + (Stopwatch.GetTimestamp() - _startTicks) 
                : _elapsedTicks;

            /// <summary>
            /// 已经过的毫秒数
            /// </summary>
            public double ElapsedMilliseconds => ElapsedTicks * 1000.0 / Stopwatch.Frequency;

            /// <summary>
            /// 已经过的微秒数
            /// </summary>
            public double ElapsedMicroseconds => ElapsedTicks * 1000000.0 / Stopwatch.Frequency;

            /// <summary>
            /// 已经过的秒数
            /// </summary>
            public double ElapsedSeconds => (double)ElapsedTicks / Stopwatch.Frequency;

            /// <summary>
            /// 开始计时
            /// </summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Start()
            {
                if (!_isRunning)
                {
                    _startTicks = Stopwatch.GetTimestamp();
                    _isRunning = true;
                }
            }

            /// <summary>
            /// 停止计时
            /// </summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Stop()
            {
                if (_isRunning)
                {
                    _elapsedTicks += Stopwatch.GetTimestamp() - _startTicks;
                    _isRunning = false;
                }
            }

            /// <summary>
            /// 重置计时器
            /// </summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Reset()
            {
                _elapsedTicks = 0;
                _isRunning = false;
            }

            /// <summary>
            /// 重置并开始计时
            /// </summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Restart()
            {
                _elapsedTicks = 0;
                _startTicks = Stopwatch.GetTimestamp();
                _isRunning = true;
            }

            /// <summary>
            /// 创建并启动计时器
            /// </summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static Timer StartNew()
            {
                var timer = new Timer();
                timer.Start();
                return timer;
            }
        }

        #endregion

        #region 性能统计结构

        /// <summary>
        /// 性能统计数据
        /// </summary>
        public struct ProfileStats
        {
            /// <summary>
            /// 采样次数
            /// </summary>
            public int SampleCount;

            /// <summary>
            /// 总时间（毫秒）
            /// </summary>
            public double TotalMs;

            /// <summary>
            /// 最小时间（毫秒）
            /// </summary>
            public double MinMs;

            /// <summary>
            /// 最大时间（毫秒）
            /// </summary>
            public double MaxMs;

            /// <summary>
            /// 平均时间（毫秒）
            /// </summary>
            public double AverageMs => SampleCount > 0 ? TotalMs / SampleCount : 0;

            /// <summary>
            /// 添加采样
            /// </summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void AddSample(double ms)
            {
                if (SampleCount == 0)
                {
                    MinMs = ms;
                    MaxMs = ms;
                }
                else
                {
                    if (ms < MinMs) MinMs = ms;
                    if (ms > MaxMs) MaxMs = ms;
                }
                TotalMs += ms;
                SampleCount++;
            }

            /// <summary>
            /// 重置统计
            /// </summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Reset()
            {
                SampleCount = 0;
                TotalMs = 0;
                MinMs = 0;
                MaxMs = 0;
            }
        }

        #endregion

        #region 作用域计时器

        /// <summary>
        /// 作用域计时器（用于using语句）
        /// </summary>
        public struct ScopedTimer : System.IDisposable
        {
            private Timer _timer;
            private FixedString64Bytes _name;

            /// <summary>
            /// 创建作用域计时器
            /// </summary>
            public ScopedTimer(in FixedString64Bytes name)
            {
                _name = name;
                _timer = Timer.StartNew();
            }

            /// <summary>
            /// 结束计时并输出结果
            /// </summary>
            public void Dispose()
            {
                _timer.Stop();
                LogTimerResult(_name, _timer.ElapsedMilliseconds);
            }

            [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
            [BurstDiscard]
            private static void LogTimerResult(in FixedString64Bytes name, double ms)
            {
                UnityEngine.Debug.Log($"[Profiler] {name}: {ms:F4}ms");
            }
        }

        /// <summary>
        /// 创建作用域计时器
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ScopedTimer Scope(in FixedString64Bytes name)
        {
            return new ScopedTimer(name);
        }

        #endregion

        #region 静态计时方法

        /// <summary>
        /// 获取当前时间戳（Ticks）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long GetTimestamp()
        {
            return Stopwatch.GetTimestamp();
        }

        /// <summary>
        /// 计算两个时间戳之间的毫秒数
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double TicksToMilliseconds(long ticks)
        {
            return ticks * 1000.0 / Stopwatch.Frequency;
        }

        /// <summary>
        /// 计算两个时间戳之间的微秒数
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double TicksToMicroseconds(long ticks)
        {
            return ticks * 1000000.0 / Stopwatch.Frequency;
        }

        /// <summary>
        /// 计算两个时间戳之间的时间差（毫秒）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double GetElapsedMilliseconds(long startTicks, long endTicks)
        {
            return TicksToMilliseconds(endTicks - startTicks);
        }

        #endregion

        #region 日志输出

        /// <summary>
        /// 输出性能统计信息
        /// </summary>
        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
        [BurstDiscard]
        public static void LogStats(in FixedString64Bytes name, in ProfileStats stats)
        {
            UnityEngine.Debug.Log(
                $"[Profiler] {name}: Samples={stats.SampleCount}, " +
                $"Avg={stats.AverageMs:F4}ms, Min={stats.MinMs:F4}ms, Max={stats.MaxMs:F4}ms, " +
                $"Total={stats.TotalMs:F4}ms");
        }

        /// <summary>
        /// 输出计时结果
        /// </summary>
        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
        [BurstDiscard]
        public static void LogTime(in FixedString64Bytes name, double milliseconds)
        {
            UnityEngine.Debug.Log($"[Profiler] {name}: {milliseconds:F4}ms");
        }

        /// <summary>
        /// 输出吞吐量信息
        /// </summary>
        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
        [BurstDiscard]
        public static void LogThroughput(in FixedString64Bytes name, int itemCount, double milliseconds)
        {
            double itemsPerSecond = itemCount / (milliseconds / 1000.0);
            UnityEngine.Debug.Log($"[Profiler] {name}: {itemCount} items in {milliseconds:F4}ms ({itemsPerSecond:F0} items/sec)");
        }

        /// <summary>
        /// 输出内存带宽信息
        /// </summary>
        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
        [BurstDiscard]
        public static void LogBandwidth(in FixedString64Bytes name, long bytes, double milliseconds)
        {
            double mbPerSecond = (bytes / (1024.0 * 1024.0)) / (milliseconds / 1000.0);
            UnityEngine.Debug.Log($"[Profiler] {name}: {bytes} bytes in {milliseconds:F4}ms ({mbPerSecond:F2} MB/s)");
        }

        #endregion

        #region 帧率计算

        /// <summary>
        /// 帧率计算器
        /// </summary>
        public struct FrameRateCalculator
        {
            private double _accumulatedTime;
            private int _frameCount;
            private double _currentFps;
            private double _updateInterval;

            /// <summary>
            /// 当前帧率
            /// </summary>
            public double CurrentFps => _currentFps;

            /// <summary>
            /// 当前帧时间（毫秒）
            /// </summary>
            public double CurrentFrameTimeMs => _currentFps > 0 ? 1000.0 / _currentFps : 0;

            /// <summary>
            /// 创建帧率计算器
            /// </summary>
            /// <param name="updateInterval">更新间隔（秒）</param>
            public FrameRateCalculator(double updateInterval = 0.5)
            {
                _accumulatedTime = 0;
                _frameCount = 0;
                _currentFps = 0;
                _updateInterval = updateInterval;
            }

            /// <summary>
            /// 更新帧率计算
            /// </summary>
            /// <param name="deltaTime">帧时间（秒）</param>
            /// <returns>如果帧率已更新返回true</returns>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool Update(double deltaTime)
            {
                _accumulatedTime += deltaTime;
                _frameCount++;

                if (_accumulatedTime >= _updateInterval)
                {
                    _currentFps = _frameCount / _accumulatedTime;
                    _accumulatedTime = 0;
                    _frameCount = 0;
                    return true;
                }
                return false;
            }
        }

        #endregion
    }
}
