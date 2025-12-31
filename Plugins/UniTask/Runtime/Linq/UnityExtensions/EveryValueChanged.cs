using Cysharp.Threading.Tasks.Internal;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Cysharp.Threading.Tasks.Linq
{
    /// <summary>
    /// UniTaskAsyncEnumerable 类
    /// </summary>
    public static partial class UniTaskAsyncEnumerable
    {   
        /// <summary>
        /// 每隔一段时间执行一次
        /// </summary>
        /// <typeparam name="TTarget">目标类型</typeparam>
        /// <typeparam name="TProperty">属性类型</typeparam>
        /// <param name="target">目标</param>
        /// <param name="propertySelector">属性选择器</param>
        /// <param name="monitorTiming">监视时机</param>
        /// <param name="equalityComparer">相等比较器</param>
        /// <param name="cancelImmediately">是否立即取消</param>
        /// <returns>UniTaskAsyncEnumerable</returns>
        public static IUniTaskAsyncEnumerable<TProperty> EveryValueChanged<TTarget, TProperty>(TTarget target, Func<TTarget, TProperty> propertySelector, PlayerLoopTiming monitorTiming = PlayerLoopTiming.Update, IEqualityComparer<TProperty> equalityComparer = null, bool cancelImmediately = false)
            where TTarget : class
        {
            var unityObject = target as UnityEngine.Object;
            var isUnityObject = target is UnityEngine.Object; // don't use (unityObject == null)

            if (isUnityObject)
            {
                return new EveryValueChangedUnityObject<TTarget, TProperty>(target, propertySelector, equalityComparer ?? UnityEqualityComparer.GetDefault<TProperty>(), monitorTiming, cancelImmediately);
            }
            else
            {
                return new EveryValueChangedStandardObject<TTarget, TProperty>(target, propertySelector, equalityComparer ?? UnityEqualityComparer.GetDefault<TProperty>(), monitorTiming, cancelImmediately);
            }
        }
    }

    /// <summary>
    /// EveryValueChangedUnityObject 类
    /// </summary>
    internal sealed class EveryValueChangedUnityObject<TTarget, TProperty> : IUniTaskAsyncEnumerable<TProperty>
    {
        readonly TTarget target;
        readonly Func<TTarget, TProperty> propertySelector;
        readonly IEqualityComparer<TProperty> equalityComparer;
        readonly PlayerLoopTiming monitorTiming;
        readonly bool cancelImmediately;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="target">目标</param>
        /// <param name="propertySelector">属性选择器</param>
        /// <param name="equalityComparer">相等比较器</param>
        /// <param name="monitorTiming">监视时机</param>
        /// <param name="cancelImmediately">是否立即取消</param>
        public EveryValueChangedUnityObject(TTarget target, Func<TTarget, TProperty> propertySelector, IEqualityComparer<TProperty> equalityComparer, PlayerLoopTiming monitorTiming, bool cancelImmediately)
        {
            this.target = target;
            this.propertySelector = propertySelector;
            this.equalityComparer = equalityComparer;
            this.monitorTiming = monitorTiming;
            this.cancelImmediately = cancelImmediately;
        }

        /// <summary>
        /// 获取异步枚举器
        /// </summary>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>异步枚举器</returns>
        public IUniTaskAsyncEnumerator<TProperty> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            return new _EveryValueChanged(target, propertySelector, equalityComparer, monitorTiming, cancellationToken, cancelImmediately);
        }

        /// <summary>
        /// _EveryValueChanged 类
        /// </summary>
        sealed class _EveryValueChanged : MoveNextSource, IUniTaskAsyncEnumerator<TProperty>, IPlayerLoopItem
        {
            readonly TTarget target;               // 目标
            readonly UnityEngine.Object targetAsUnityObject; // 目标的 Unity 对象
            readonly IEqualityComparer<TProperty> equalityComparer; // 相等比较器
            readonly Func<TTarget, TProperty> propertySelector; // 属性选择器
            readonly CancellationToken cancellationToken; // 取消令牌
            readonly CancellationTokenRegistration cancellationTokenRegistration; // 取消令牌注册

            bool first; // 是否是第一次
            TProperty currentValue; // 当前值
            bool disposed; // 是否已释放
            /// <summary>
            /// 构造函数
            /// </summary>
            /// <param name="target">目标</param>
            /// <param name="propertySelector">属性选择器</param>
            /// <param name="equalityComparer">相等比较器</param>
            /// <param name="monitorTiming">监视时机</param>
            /// <param name="cancellationToken">取消令牌</param>
            /// <param name="cancelImmediately">是否立即取消</param>
            public _EveryValueChanged(TTarget target, Func<TTarget, TProperty> propertySelector, IEqualityComparer<TProperty> equalityComparer, PlayerLoopTiming monitorTiming, CancellationToken cancellationToken, bool cancelImmediately)
            {
                this.target = target;
                this.targetAsUnityObject = target as UnityEngine.Object;
                this.propertySelector = propertySelector;
                this.equalityComparer = equalityComparer;
                this.cancellationToken = cancellationToken;
                this.first = true;
                
                if (cancelImmediately && cancellationToken.CanBeCanceled)
                {
                    cancellationTokenRegistration = cancellationToken.RegisterWithoutCaptureExecutionContext(state =>
                    {
                        var source = (_EveryValueChanged)state;
                        source.completionSource.TrySetCanceled(source.cancellationToken);
                    }, this);
                }
                
                TaskTracker.TrackActiveTask(this, 2);
                PlayerLoopHelper.AddAction(monitorTiming, this);
            }

            /// <summary>
            /// 当前值
            /// </summary>
            public TProperty Current => currentValue;
            /// <summary>
            /// 移动到下一个值
            /// </summary>
            /// <returns>是否成功</returns>
            public UniTask<bool> MoveNextAsync()
            {
                if (disposed) return CompletedTasks.False;

                completionSource.Reset();
                
                if (cancellationToken.IsCancellationRequested)
                {
                    completionSource.TrySetCanceled(cancellationToken);
                    return new UniTask<bool>(this, completionSource.Version);
                }

                if (first)
                {
                    first = false;
                    if (targetAsUnityObject == null)
                    {
                        return CompletedTasks.False;
                    }
                    this.currentValue = propertySelector(target);
                    return CompletedTasks.True;
                }

                return new UniTask<bool>(this, completionSource.Version);
            }

            /// <summary>
            /// 释放异步枚举器
            /// </summary>
            /// <returns>UniTask</returns>
            public UniTask DisposeAsync()
            {
                if (!disposed)
                {
                    cancellationTokenRegistration.Dispose();
                    disposed = true;
                    TaskTracker.RemoveTracking(this);
                }
                return default;
            }

            /// <summary>
            /// 移动到下一个值
            /// </summary>
            /// <returns>是否成功</returns>
            public bool MoveNext()
            {
                if (disposed || targetAsUnityObject == null) 
                {
                    completionSource.TrySetResult(false);
                    DisposeAsync().Forget();
                    return false;
                }
                
                if (cancellationToken.IsCancellationRequested)
                {
                    completionSource.TrySetCanceled(cancellationToken);
                    return false;
                }
                TProperty nextValue = default(TProperty);
                try
                {
                    nextValue = propertySelector(target);
                    if (equalityComparer.Equals(currentValue, nextValue))
                    {
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    completionSource.TrySetException(ex);
                    DisposeAsync().Forget();
                    return false;
                }

                currentValue = nextValue;
                completionSource.TrySetResult(true);
                return true;
            }
        }
    }

    /// <summary>
    /// EveryValueChangedStandardObject 类
    /// </summary>
    internal sealed class EveryValueChangedStandardObject<TTarget, TProperty> : IUniTaskAsyncEnumerable<TProperty>
        where TTarget : class
    {
        readonly WeakReference<TTarget> target; // 目标
        readonly Func<TTarget, TProperty> propertySelector; // 属性选择器
        readonly IEqualityComparer<TProperty> equalityComparer; // 相等比较器
        readonly PlayerLoopTiming monitorTiming; // 监视时机
        readonly bool cancelImmediately; // 是否立即取消

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="target">目标</param>
        /// <param name="propertySelector">属性选择器</param>
        /// <param name="equalityComparer">相等比较器</param>
        /// <param name="monitorTiming">监视时机</param>
        /// <param name="cancelImmediately">是否立即取消</param>
        public EveryValueChangedStandardObject(TTarget target, Func<TTarget, TProperty> propertySelector, IEqualityComparer<TProperty> equalityComparer, PlayerLoopTiming monitorTiming, bool cancelImmediately)
        {
            this.target = new WeakReference<TTarget>(target, false);
            this.propertySelector = propertySelector;
            this.equalityComparer = equalityComparer;
            this.monitorTiming = monitorTiming;
            this.cancelImmediately = cancelImmediately;
        }
        /// <summary>
        /// 获取异步枚举器
        /// </summary>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>异步枚举器</returns>
        public IUniTaskAsyncEnumerator<TProperty> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            return new _EveryValueChanged(target, propertySelector, equalityComparer, monitorTiming, cancellationToken, cancelImmediately);
        }
        /// <summary>
        /// _EveryValueChanged 类
        /// </summary>
        sealed class _EveryValueChanged : MoveNextSource, IUniTaskAsyncEnumerator<TProperty>, IPlayerLoopItem
        {
            readonly WeakReference<TTarget> target; // 目标
            readonly IEqualityComparer<TProperty> equalityComparer; // 相等比较器
            readonly Func<TTarget, TProperty> propertySelector; // 属性选择器
            readonly CancellationToken cancellationToken; // 取消令牌
            readonly CancellationTokenRegistration cancellationTokenRegistration; // 取消令牌注册

            bool first; // 是否是第一次
            TProperty currentValue; // 当前值
            bool disposed; // 是否已释放
            /// <summary>
            /// 构造函数
            /// </summary>
            /// <param name="target">目标</param>
            /// <param name="propertySelector">属性选择器</param>
            /// <param name="equalityComparer">相等比较器</param>
            /// <param name="monitorTiming">监视时机</param>
            /// <param name="cancellationToken">取消令牌</param>
            /// <param name="cancelImmediately">是否立即取消</param>
            public _EveryValueChanged(WeakReference<TTarget> target, Func<TTarget, TProperty> propertySelector, IEqualityComparer<TProperty> equalityComparer, PlayerLoopTiming monitorTiming, CancellationToken cancellationToken, bool cancelImmediately)
            {
                this.target = target;
                this.propertySelector = propertySelector;
                this.equalityComparer = equalityComparer;
                this.cancellationToken = cancellationToken;
                this.first = true;
                
                if (cancelImmediately && cancellationToken.CanBeCanceled)
                {
                    cancellationTokenRegistration = cancellationToken.RegisterWithoutCaptureExecutionContext(state =>
                    {
                        var source = (_EveryValueChanged)state;
                        source.completionSource.TrySetCanceled(source.cancellationToken);
                    }, this);
                }
                
                TaskTracker.TrackActiveTask(this, 2);
                PlayerLoopHelper.AddAction(monitorTiming, this);
            }

            /// <summary>
            /// 当前值
            /// </summary>
            public TProperty Current => currentValue;
            /// <summary>
            /// 移动到下一个值
            /// </summary>
            /// <returns>是否成功</returns>

            public UniTask<bool> MoveNextAsync()
            {
                if (disposed) return CompletedTasks.False;

                completionSource.Reset();
                
                if (cancellationToken.IsCancellationRequested)
                {
                    completionSource.TrySetCanceled(cancellationToken);
                    return new UniTask<bool>(this, completionSource.Version);
                }
                
                if (first)
                {
                    first = false;
                    if (!target.TryGetTarget(out var t))
                    {
                        return CompletedTasks.False;
                    }
                    this.currentValue = propertySelector(t);
                    return CompletedTasks.True;
                }

                return new UniTask<bool>(this, completionSource.Version);
            }

            /// <summary>
            /// 释放异步枚举器
            /// </summary>
            /// <returns>UniTask</returns>
            public UniTask DisposeAsync()
            {
                if (!disposed)
                {
                    cancellationTokenRegistration.Dispose();
                    disposed = true;
                    TaskTracker.RemoveTracking(this);
                }
                return default;
            }

            /// <summary>
            /// 移动到下一个值
            /// </summary>
            /// <returns>是否成功</returns>
            public bool MoveNext()
            {
                if (disposed || !target.TryGetTarget(out var t))
                {
                    completionSource.TrySetResult(false);
                    DisposeAsync().Forget();
                    return false;
                }

                if (cancellationToken.IsCancellationRequested)
                {
                    completionSource.TrySetCanceled(cancellationToken);
                    return false;
                }

                TProperty nextValue = default(TProperty);
                try
                {
                    nextValue = propertySelector(t);
                    if (equalityComparer.Equals(currentValue, nextValue))
                    {
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    completionSource.TrySetException(ex);
                    DisposeAsync().Forget();
                    return false;
                }

                currentValue = nextValue;
                completionSource.TrySetResult(true);
                return true;
            }
        }
    }
}