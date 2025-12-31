// ==========================================================
// 文件名：DelegateExtensions.cs
// 命名空间: AFramework.CSharpExtension
// 依赖: System, System.Linq
// ==========================================================

using System;
using System.Linq;
using System.Runtime.CompilerServices;

namespace AFramework.CSharpExtension
{
    /// <summary>
    /// Delegate 扩展方法
    /// <para>提供委托的常用操作扩展，包括安全调用、组合、异步执行等功能</para>
    /// </summary>
    public static class DelegateExtensions
    {
        #region 安全调用

        /// <summary>
        /// 安全调用 Action（忽略异常）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SafeInvoke(this Action action)
        {
            try { action?.Invoke(); }
            catch { /* 忽略异常 */ }
        }

        /// <summary>
        /// 安全调用 Action&lt;T&gt;（忽略异常）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SafeInvoke<T>(this Action<T> action, T arg)
        {
            try { action?.Invoke(arg); }
            catch { /* 忽略异常 */ }
        }

        /// <summary>
        /// 安全调用 Action&lt;T1, T2&gt;（忽略异常）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SafeInvoke<T1, T2>(this Action<T1, T2> action, T1 arg1, T2 arg2)
        {
            try { action?.Invoke(arg1, arg2); }
            catch { /* 忽略异常 */ }
        }

        /// <summary>
        /// 安全调用 Func&lt;TResult&gt;（异常时返回默认值）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TResult SafeInvoke<TResult>(this Func<TResult> func, TResult defaultValue = default)
        {
            try { return func != null ? func() : defaultValue; }
            catch { return defaultValue; }
        }

        /// <summary>
        /// 安全调用 Func&lt;T, TResult&gt;（异常时返回默认值）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TResult SafeInvoke<T, TResult>(this Func<T, TResult> func, T arg, TResult defaultValue = default)
        {
            try { return func != null ? func(arg) : defaultValue; }
            catch { return defaultValue; }
        }

        #endregion

        #region 带异常处理的调用

        /// <summary>
        /// 调用 Action 并捕获异常
        /// </summary>
        public static Exception TryInvoke(this Action action)
        {
            if (action == null) return null;
            
            try
            {
                action();
                return null;
            }
            catch (Exception ex)
            {
                return ex;
            }
        }

        /// <summary>
        /// 调用 Action&lt;T&gt; 并捕获异常
        /// </summary>
        public static Exception TryInvoke<T>(this Action<T> action, T arg)
        {
            if (action == null) return null;
            
            try
            {
                action(arg);
                return null;
            }
            catch (Exception ex)
            {
                return ex;
            }
        }

        /// <summary>
        /// 调用 Func&lt;TResult&gt; 并返回结果
        /// </summary>
        public static Result<TResult> TryInvoke<TResult>(this Func<TResult> func)
        {
            if (func == null)
                return Result<TResult>.Failure("Delegate is null.");
            
            try
            {
                return Result<TResult>.Success(func());
            }
            catch (Exception ex)
            {
                return Result<TResult>.FromException(ex);
            }
        }

        #endregion

        #region 委托组合

        /// <summary>
        /// 组合两个 Action
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Action Combine(this Action first, Action second)
        {
            return (Action)Delegate.Combine(first, second);
        }

        /// <summary>
        /// 组合两个 Action&lt;T&gt;
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Action<T> Combine<T>(this Action<T> first, Action<T> second)
        {
            return (Action<T>)Delegate.Combine(first, second);
        }

        /// <summary>
        /// 从委托中移除指定委托
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Action Remove(this Action source, Action toRemove)
        {
            return (Action)Delegate.Remove(source, toRemove);
        }

        /// <summary>
        /// 从委托中移除指定委托
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Action<T> Remove<T>(this Action<T> source, Action<T> toRemove)
        {
            return (Action<T>)Delegate.Remove(source, toRemove);
        }

        #endregion

        #region 委托信息

        /// <summary>
        /// 获取委托的调用列表长度
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetInvocationCount(this Delegate del)
        {
            return del?.GetInvocationList().Length ?? 0;
        }

        /// <summary>
        /// 检查委托是否为空或无订阅者
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNullOrEmpty(this Delegate del)
        {
            return del == null || del.GetInvocationList().Length == 0;
        }

        /// <summary>
        /// 检查委托是否有订阅者
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasSubscribers(this Delegate del)
        {
            return del != null && del.GetInvocationList().Length > 0;
        }

        #endregion

        #region 条件执行

        /// <summary>
        /// 条件执行 Action
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void InvokeIf(this Action action, bool condition)
        {
            if (condition) action?.Invoke();
        }

        /// <summary>
        /// 条件执行 Action
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void InvokeIf(this Action action, Func<bool> predicate)
        {
            if (predicate != null && predicate()) action?.Invoke();
        }

        /// <summary>
        /// 条件执行 Action&lt;T&gt;
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void InvokeIf<T>(this Action<T> action, T arg, bool condition)
        {
            if (condition) action?.Invoke(arg);
        }

        /// <summary>
        /// 条件执行 Action&lt;T&gt;
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void InvokeIf<T>(this Action<T> action, T arg, Func<T, bool> predicate)
        {
            if (predicate != null && predicate(arg)) action?.Invoke(arg);
        }

        #endregion

        #region 重试执行

        /// <summary>
        /// 重试执行 Action
        /// </summary>
        public static bool InvokeWithRetry(this Action action, int maxRetries, int delayMs = 0)
        {
            if (action == null) return false;
            
            for (int i = 0; i <= maxRetries; i++)
            {
                try
                {
                    action();
                    return true;
                }
                catch
                {
                    if (i == maxRetries) return false;
                    if (delayMs > 0) System.Threading.Thread.Sleep(delayMs);
                }
            }
            return false;
        }

        /// <summary>
        /// 重试执行 Func&lt;TResult&gt;
        /// </summary>
        public static Result<TResult> InvokeWithRetry<TResult>(this Func<TResult> func, int maxRetries, int delayMs = 0)
        {
            if (func == null)
                return Result<TResult>.Failure("Delegate is null.");
            
            Exception lastException = null;
            for (int i = 0; i <= maxRetries; i++)
            {
                try
                {
                    return Result<TResult>.Success(func());
                }
                catch (Exception ex)
                {
                    lastException = ex;
                    if (i < maxRetries && delayMs > 0)
                        System.Threading.Thread.Sleep(delayMs);
                }
            }
            return Result<TResult>.FromException(lastException);
        }

        #endregion

        #region 广播调用

        /// <summary>
        /// 广播调用所有订阅者（即使某个抛出异常也继续执行）
        /// </summary>
        public static void BroadcastInvoke(this Action action)
        {
            if (action == null) return;
            
            foreach (var del in action.GetInvocationList())
            {
                try { ((Action)del)(); }
                catch { /* 继续执行其他订阅者 */ }
            }
        }

        /// <summary>
        /// 广播调用所有订阅者（即使某个抛出异常也继续执行）
        /// </summary>
        public static void BroadcastInvoke<T>(this Action<T> action, T arg)
        {
            if (action == null) return;
            
            foreach (var del in action.GetInvocationList())
            {
                try { ((Action<T>)del)(arg); }
                catch { /* 继续执行其他订阅者 */ }
            }
        }

        /// <summary>
        /// 广播调用并收集所有异常
        /// </summary>
        public static Exception[] BroadcastInvokeWithExceptions(this Action action)
        {
            if (action == null) return Array.Empty<Exception>();
            
            var exceptions = new System.Collections.Generic.List<Exception>();
            foreach (var del in action.GetInvocationList())
            {
                try { ((Action)del)(); }
                catch (Exception ex) { exceptions.Add(ex); }
            }
            return exceptions.ToArray();
        }

        #endregion

        #region 函数组合

        /// <summary>
        /// 函数组合：先执行 first，再执行 second
        /// </summary>
        public static Func<T, TResult2> Then<T, TResult1, TResult2>(
            this Func<T, TResult1> first, 
            Func<TResult1, TResult2> second)
        {
            if (first == null || second == null) return null;
            return x => second(first(x));
        }

        /// <summary>
        /// 函数组合：先执行 second，再执行 first
        /// </summary>
        public static Func<T, TResult2> Compose<T, TResult1, TResult2>(
            this Func<TResult1, TResult2> first, 
            Func<T, TResult1> second)
        {
            if (first == null || second == null) return null;
            return x => first(second(x));
        }

        #endregion

        #region 柯里化

        /// <summary>
        /// 柯里化：将 Func&lt;T1, T2, TResult&gt; 转换为 Func&lt;T1, Func&lt;T2, TResult&gt;&gt;
        /// </summary>
        public static Func<T1, Func<T2, TResult>> Curry<T1, T2, TResult>(this Func<T1, T2, TResult> func)
        {
            if (func == null) return null;
            return x => y => func(x, y);
        }

        /// <summary>
        /// 柯里化：将 Func&lt;T1, T2, T3, TResult&gt; 转换为嵌套函数
        /// </summary>
        public static Func<T1, Func<T2, Func<T3, TResult>>> Curry<T1, T2, T3, TResult>(this Func<T1, T2, T3, TResult> func)
        {
            if (func == null) return null;
            return x => y => z => func(x, y, z);
        }

        /// <summary>
        /// 部分应用：固定第一个参数
        /// </summary>
        public static Func<T2, TResult> Partial<T1, T2, TResult>(this Func<T1, T2, TResult> func, T1 arg1)
        {
            if (func == null) return null;
            return arg2 => func(arg1, arg2);
        }

        #endregion

        #region 记忆化

        /// <summary>
        /// 记忆化：缓存函数结果
        /// </summary>
        public static Func<T, TResult> Memoize<T, TResult>(this Func<T, TResult> func)
        {
            if (func == null) return null;
            
            var cache = new System.Collections.Concurrent.ConcurrentDictionary<T, TResult>();
            return arg => cache.GetOrAdd(arg, func);
        }

        /// <summary>
        /// 记忆化：缓存无参函数结果
        /// </summary>
        public static Func<TResult> Memoize<TResult>(this Func<TResult> func)
        {
            if (func == null) return null;
            
            var hasValue = false;
            var cachedValue = default(TResult);
            var lockObj = new object();
            
            return () =>
            {
                if (hasValue) return cachedValue;
                
                lock (lockObj)
                {
                    if (!hasValue)
                    {
                        cachedValue = func();
                        hasValue = true;
                    }
                }
                return cachedValue;
            };
        }

        #endregion
    }
}
