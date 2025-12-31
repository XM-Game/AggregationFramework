// ==========================================================
// 文件名：Curry.cs
// 命名空间: AFramework.CSharpExtension
// 依赖: System
// ==========================================================

using System;

namespace AFramework.CSharpExtension
{
    /// <summary>
    /// 柯里化工具类
    /// <para>提供函数柯里化和部分应用功能</para>
    /// </summary>
    public static class Curry
    {
        #region 双参数柯里化

        /// <summary>
        /// 柯里化双参数函数
        /// </summary>
        public static Func<T1, Func<T2, TResult>> Curried<T1, T2, TResult>(Func<T1, T2, TResult> func)
        {
            return t1 => t2 => func(t1, t2);
        }

        /// <summary>
        /// 反柯里化
        /// </summary>
        public static Func<T1, T2, TResult> Uncurried<T1, T2, TResult>(Func<T1, Func<T2, TResult>> func)
        {
            return (t1, t2) => func(t1)(t2);
        }

        #endregion

        #region 三参数柯里化

        /// <summary>
        /// 柯里化三参数函数
        /// </summary>
        public static Func<T1, Func<T2, Func<T3, TResult>>> Curried<T1, T2, T3, TResult>(Func<T1, T2, T3, TResult> func)
        {
            return t1 => t2 => t3 => func(t1, t2, t3);
        }

        /// <summary>
        /// 反柯里化
        /// </summary>
        public static Func<T1, T2, T3, TResult> Uncurried<T1, T2, T3, TResult>(Func<T1, Func<T2, Func<T3, TResult>>> func)
        {
            return (t1, t2, t3) => func(t1)(t2)(t3);
        }

        #endregion

        #region 四参数柯里化

        /// <summary>
        /// 柯里化四参数函数
        /// </summary>
        public static Func<T1, Func<T2, Func<T3, Func<T4, TResult>>>> Curried<T1, T2, T3, T4, TResult>(
            Func<T1, T2, T3, T4, TResult> func)
        {
            return t1 => t2 => t3 => t4 => func(t1, t2, t3, t4);
        }

        #endregion

        #region 部分应用 - 双参数

        /// <summary>
        /// 部分应用第一个参数
        /// </summary>
        public static Func<T2, TResult> Partial<T1, T2, TResult>(Func<T1, T2, TResult> func, T1 arg1)
        {
            return arg2 => func(arg1, arg2);
        }

        /// <summary>
        /// 部分应用第二个参数
        /// </summary>
        public static Func<T1, TResult> PartialRight<T1, T2, TResult>(Func<T1, T2, TResult> func, T2 arg2)
        {
            return arg1 => func(arg1, arg2);
        }

        #endregion

        #region 部分应用 - 三参数

        /// <summary>
        /// 部分应用第一个参数
        /// </summary>
        public static Func<T2, T3, TResult> Partial<T1, T2, T3, TResult>(Func<T1, T2, T3, TResult> func, T1 arg1)
        {
            return (arg2, arg3) => func(arg1, arg2, arg3);
        }

        /// <summary>
        /// 部分应用前两个参数
        /// </summary>
        public static Func<T3, TResult> Partial<T1, T2, T3, TResult>(Func<T1, T2, T3, TResult> func, T1 arg1, T2 arg2)
        {
            return arg3 => func(arg1, arg2, arg3);
        }

        /// <summary>
        /// 部分应用最后一个参数
        /// </summary>
        public static Func<T1, T2, TResult> PartialRight<T1, T2, T3, TResult>(Func<T1, T2, T3, TResult> func, T3 arg3)
        {
            return (arg1, arg2) => func(arg1, arg2, arg3);
        }

        #endregion

        #region 部分应用 - 四参数

        /// <summary>
        /// 部分应用第一个参数
        /// </summary>
        public static Func<T2, T3, T4, TResult> Partial<T1, T2, T3, T4, TResult>(
            Func<T1, T2, T3, T4, TResult> func, T1 arg1)
        {
            return (arg2, arg3, arg4) => func(arg1, arg2, arg3, arg4);
        }

        /// <summary>
        /// 部分应用前两个参数
        /// </summary>
        public static Func<T3, T4, TResult> Partial<T1, T2, T3, T4, TResult>(
            Func<T1, T2, T3, T4, TResult> func, T1 arg1, T2 arg2)
        {
            return (arg3, arg4) => func(arg1, arg2, arg3, arg4);
        }

        /// <summary>
        /// 部分应用前三个参数
        /// </summary>
        public static Func<T4, TResult> Partial<T1, T2, T3, T4, TResult>(
            Func<T1, T2, T3, T4, TResult> func, T1 arg1, T2 arg2, T3 arg3)
        {
            return arg4 => func(arg1, arg2, arg3, arg4);
        }

        #endregion

        #region Action 柯里化

        /// <summary>
        /// 柯里化双参数 Action
        /// </summary>
        public static Func<T1, Action<T2>> Curried<T1, T2>(Action<T1, T2> action)
        {
            return t1 => t2 => action(t1, t2);
        }

        /// <summary>
        /// 柯里化三参数 Action
        /// </summary>
        public static Func<T1, Func<T2, Action<T3>>> Curried<T1, T2, T3>(Action<T1, T2, T3> action)
        {
            return t1 => t2 => t3 => action(t1, t2, t3);
        }

        #endregion

        #region Action 部分应用

        /// <summary>
        /// 部分应用第一个参数
        /// </summary>
        public static Action<T2> Partial<T1, T2>(Action<T1, T2> action, T1 arg1)
        {
            return arg2 => action(arg1, arg2);
        }

        /// <summary>
        /// 部分应用第一个参数
        /// </summary>
        public static Action<T2, T3> Partial<T1, T2, T3>(Action<T1, T2, T3> action, T1 arg1)
        {
            return (arg2, arg3) => action(arg1, arg2, arg3);
        }

        /// <summary>
        /// 部分应用前两个参数
        /// </summary>
        public static Action<T3> Partial<T1, T2, T3>(Action<T1, T2, T3> action, T1 arg1, T2 arg2)
        {
            return arg3 => action(arg1, arg2, arg3);
        }

        #endregion
    }

    /// <summary>
    /// 柯里化扩展方法
    /// </summary>
    public static class CurryExtensions
    {
        #region Func 扩展

        /// <summary>
        /// 柯里化
        /// </summary>
        public static Func<T1, Func<T2, TResult>> Curried<T1, T2, TResult>(this Func<T1, T2, TResult> func)
        {
            return t1 => t2 => func(t1, t2);
        }

        /// <summary>
        /// 柯里化
        /// </summary>
        public static Func<T1, Func<T2, Func<T3, TResult>>> Curried<T1, T2, T3, TResult>(this Func<T1, T2, T3, TResult> func)
        {
            return t1 => t2 => t3 => func(t1, t2, t3);
        }

        /// <summary>
        /// 部分应用
        /// </summary>
        public static Func<T2, TResult> Apply<T1, T2, TResult>(this Func<T1, T2, TResult> func, T1 arg1)
        {
            return arg2 => func(arg1, arg2);
        }

        /// <summary>
        /// 部分应用
        /// </summary>
        public static Func<T2, T3, TResult> Apply<T1, T2, T3, TResult>(this Func<T1, T2, T3, TResult> func, T1 arg1)
        {
            return (arg2, arg3) => func(arg1, arg2, arg3);
        }

        /// <summary>
        /// 部分应用
        /// </summary>
        public static Func<T3, TResult> Apply<T1, T2, T3, TResult>(this Func<T1, T2, T3, TResult> func, T1 arg1, T2 arg2)
        {
            return arg3 => func(arg1, arg2, arg3);
        }

        #endregion

        #region Action 扩展

        /// <summary>
        /// 柯里化
        /// </summary>
        public static Func<T1, Action<T2>> Curried<T1, T2>(this Action<T1, T2> action)
        {
            return t1 => t2 => action(t1, t2);
        }

        /// <summary>
        /// 部分应用
        /// </summary>
        public static Action<T2> Apply<T1, T2>(this Action<T1, T2> action, T1 arg1)
        {
            return arg2 => action(arg1, arg2);
        }

        /// <summary>
        /// 部分应用
        /// </summary>
        public static Action<T2, T3> Apply<T1, T2, T3>(this Action<T1, T2, T3> action, T1 arg1)
        {
            return (arg2, arg3) => action(arg1, arg2, arg3);
        }

        #endregion
    }
}
