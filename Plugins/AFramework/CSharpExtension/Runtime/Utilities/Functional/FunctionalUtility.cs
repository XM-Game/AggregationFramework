// ==========================================================
// 文件名：FunctionalUtility.cs
// 命名空间: AFramework.CSharpExtension
// 依赖: System
// ==========================================================

using System;

namespace AFramework.CSharpExtension
{
    /// <summary>
    /// 函数式工具类
    /// <para>提供函数式编程常用工具，包括函数组合、条件执行等</para>
    /// </summary>
    public static class FunctionalUtility
    {
        #region 恒等函数

        /// <summary>
        /// 恒等函数，返回输入值本身
        /// </summary>
        public static T Identity<T>(T value) => value;

        /// <summary>
        /// 获取恒等函数
        /// </summary>
        public static Func<T, T> GetIdentity<T>() => Identity;

        #endregion

        #region 常量函数

        /// <summary>
        /// 创建返回常量值的函数
        /// </summary>
        public static Func<T> Constant<T>(T value) => () => value;

        /// <summary>
        /// 创建忽略输入并返回常量值的函数
        /// </summary>
        public static Func<TInput, TOutput> Constant<TInput, TOutput>(TOutput value) => _ => value;

        #endregion

        #region 函数组合

        /// <summary>
        /// 组合两个函数 (f . g)(x) = f(g(x))
        /// </summary>
        public static Func<T, TResult> Compose<T, TIntermediate, TResult>(
            Func<TIntermediate, TResult> f,
            Func<T, TIntermediate> g)
        {
            return x => f(g(x));
        }

        /// <summary>
        /// 组合三个函数
        /// </summary>
        public static Func<T, TResult> Compose<T, T1, T2, TResult>(
            Func<T2, TResult> f,
            Func<T1, T2> g,
            Func<T, T1> h)
        {
            return x => f(g(h(x)));
        }

        /// <summary>
        /// 管道组合（从左到右）
        /// </summary>
        public static Func<T, TResult> Pipe<T, TIntermediate, TResult>(
            Func<T, TIntermediate> f,
            Func<TIntermediate, TResult> g)
        {
            return x => g(f(x));
        }

        #endregion

        #region 条件执行

        /// <summary>
        /// 条件执行
        /// </summary>
        public static T If<T>(bool condition, Func<T> ifTrue, Func<T> ifFalse)
        {
            return condition ? ifTrue() : ifFalse();
        }

        /// <summary>
        /// 条件执行（带默认值）
        /// </summary>
        public static T IfOrDefault<T>(bool condition, Func<T> ifTrue, T defaultValue = default)
        {
            return condition ? ifTrue() : defaultValue;
        }

        /// <summary>
        /// 条件执行动作
        /// </summary>
        public static void When(bool condition, Action action)
        {
            if (condition) action();
        }

        /// <summary>
        /// 条件执行动作（带 else）
        /// </summary>
        public static void When(bool condition, Action ifTrue, Action ifFalse)
        {
            if (condition) ifTrue();
            else ifFalse();
        }

        #endregion

        #region 空值处理

        /// <summary>
        /// 如果值不为 null 则执行函数
        /// </summary>
        public static TResult IfNotNull<T, TResult>(T value, Func<T, TResult> func, TResult defaultValue = default) where T : class
        {
            return value != null ? func(value) : defaultValue;
        }

        /// <summary>
        /// 如果值不为 null 则执行动作
        /// </summary>
        public static void IfNotNull<T>(T value, Action<T> action) where T : class
        {
            if (value != null) action(value);
        }

        /// <summary>
        /// 合并空值
        /// </summary>
        public static T Coalesce<T>(params T[] values) where T : class
        {
            foreach (var value in values)
            {
                if (value != null) return value;
            }
            return null;
        }

        #endregion

        #region 异常处理

        /// <summary>
        /// 尝试执行函数，失败返回默认值
        /// </summary>
        public static T TryOrDefault<T>(Func<T> func, T defaultValue = default)
        {
            try
            {
                return func();
            }
            catch
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// 尝试执行函数，失败执行回退函数
        /// </summary>
        public static T TryOrFallback<T>(Func<T> func, Func<Exception, T> fallback)
        {
            try
            {
                return func();
            }
            catch (Exception ex)
            {
                return fallback(ex);
            }
        }

        /// <summary>
        /// 尝试执行动作，忽略异常
        /// </summary>
        public static void TryIgnore(Action action)
        {
            try
            {
                action();
            }
            catch
            {
                // 忽略异常
            }
        }

        /// <summary>
        /// 尝试执行动作，异常时执行回退
        /// </summary>
        public static void TryOrFallback(Action action, Action<Exception> fallback)
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                fallback(ex);
            }
        }

        #endregion

        #region 重试

        /// <summary>
        /// 重试执行函数
        /// </summary>
        public static T Retry<T>(Func<T> func, int maxRetries, int delayMs = 0)
        {
            Exception lastException = null;
            for (int i = 0; i <= maxRetries; i++)
            {
                try
                {
                    return func();
                }
                catch (Exception ex)
                {
                    lastException = ex;
                    if (i < maxRetries && delayMs > 0)
                        System.Threading.Thread.Sleep(delayMs);
                }
            }
            throw lastException;
        }

        /// <summary>
        /// 重试执行动作
        /// </summary>
        public static void Retry(Action action, int maxRetries, int delayMs = 0)
        {
            Exception lastException = null;
            for (int i = 0; i <= maxRetries; i++)
            {
                try
                {
                    action();
                    return;
                }
                catch (Exception ex)
                {
                    lastException = ex;
                    if (i < maxRetries && delayMs > 0)
                        System.Threading.Thread.Sleep(delayMs);
                }
            }
            throw lastException;
        }

        #endregion

        #region 函数适配

        /// <summary>
        /// 将 Action 转换为返回 Unit 的 Func
        /// </summary>
        public static Func<Unit> ToFunc(Action action)
        {
            return () => { action(); return Unit.Default; };
        }

        /// <summary>
        /// 将 Action&lt;T&gt; 转换为返回 Unit 的 Func
        /// </summary>
        public static Func<T, Unit> ToFunc<T>(Action<T> action)
        {
            return x => { action(x); return Unit.Default; };
        }

        /// <summary>
        /// 忽略函数返回值
        /// </summary>
        public static Action ToAction<T>(Func<T> func)
        {
            return () => func();
        }

        /// <summary>
        /// 忽略函数返回值
        /// </summary>
        public static Action<T> ToAction<T, TResult>(Func<T, TResult> func)
        {
            return x => func(x);
        }

        #endregion

        #region 延迟执行

        /// <summary>
        /// 创建延迟求值的值
        /// </summary>
        public static Lazy<T> Defer<T>(Func<T> factory)
        {
            return new Lazy<T>(factory);
        }

        /// <summary>
        /// 创建线程安全的延迟求值的值
        /// </summary>
        public static Lazy<T> DeferThreadSafe<T>(Func<T> factory)
        {
            return new Lazy<T>(factory, System.Threading.LazyThreadSafetyMode.ExecutionAndPublication);
        }

        #endregion

        #region 副作用

        /// <summary>
        /// 执行副作用并返回原值（用于调试）
        /// </summary>
        public static T Tap<T>(T value, Action<T> action)
        {
            action(value);
            return value;
        }

        /// <summary>
        /// 执行副作用并返回原值（无参数版本）
        /// </summary>
        public static T Tap<T>(T value, Action action)
        {
            action();
            return value;
        }

        #endregion

        #region 交换

        /// <summary>
        /// 交换函数参数顺序
        /// </summary>
        public static Func<T2, T1, TResult> Flip<T1, T2, TResult>(Func<T1, T2, TResult> func)
        {
            return (t2, t1) => func(t1, t2);
        }

        /// <summary>
        /// 交换动作参数顺序
        /// </summary>
        public static Action<T2, T1> Flip<T1, T2>(Action<T1, T2> action)
        {
            return (t2, t1) => action(t1, t2);
        }

        #endregion
    }
}
