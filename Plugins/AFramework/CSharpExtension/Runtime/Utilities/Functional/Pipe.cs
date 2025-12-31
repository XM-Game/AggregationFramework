// ==========================================================
// 文件名：Pipe.cs
// 命名空间: AFramework.CSharpExtension
// 依赖: System
// ==========================================================

using System;

namespace AFramework.CSharpExtension
{
    /// <summary>
    /// 管道操作工具类
    /// <para>提供流式数据处理管道，支持链式调用</para>
    /// </summary>
    public static class Pipe
    {
        #region 创建管道

        /// <summary>
        /// 创建管道
        /// </summary>
        public static Pipeline<T> From<T>(T value)
        {
            return new Pipeline<T>(value);
        }

        /// <summary>
        /// 创建延迟管道
        /// </summary>
        public static Pipeline<T> FromLazy<T>(Func<T> factory)
        {
            return new Pipeline<T>(factory());
        }

        #endregion
    }

    /// <summary>
    /// 管道结构
    /// <para>支持链式调用的数据处理管道</para>
    /// </summary>
    public readonly struct Pipeline<T>
    {
        private readonly T _value;

        /// <summary>
        /// 当前值
        /// </summary>
        public T Value => _value;

        internal Pipeline(T value)
        {
            _value = value;
        }

        #region 转换操作

        /// <summary>
        /// 转换值
        /// </summary>
        public Pipeline<TResult> Map<TResult>(Func<T, TResult> transform)
        {
            return new Pipeline<TResult>(transform(_value));
        }

        /// <summary>
        /// 转换值（别名）
        /// </summary>
        public Pipeline<TResult> Then<TResult>(Func<T, TResult> transform)
        {
            return Map(transform);
        }

        /// <summary>
        /// 扁平化转换
        /// </summary>
        public Pipeline<TResult> FlatMap<TResult>(Func<T, Pipeline<TResult>> transform)
        {
            return transform(_value);
        }

        #endregion

        #region 条件操作

        /// <summary>
        /// 条件转换
        /// </summary>
        public Pipeline<T> MapIf(bool condition, Func<T, T> transform)
        {
            return condition ? Map(transform) : this;
        }

        /// <summary>
        /// 条件转换
        /// </summary>
        public Pipeline<T> MapIf(Func<T, bool> predicate, Func<T, T> transform)
        {
            return predicate(_value) ? Map(transform) : this;
        }

        /// <summary>
        /// 条件分支
        /// </summary>
        public Pipeline<TResult> Branch<TResult>(
            Func<T, bool> predicate,
            Func<T, TResult> ifTrue,
            Func<T, TResult> ifFalse)
        {
            return new Pipeline<TResult>(predicate(_value) ? ifTrue(_value) : ifFalse(_value));
        }

        #endregion

        #region 副作用操作

        /// <summary>
        /// 执行副作用（不改变值）
        /// </summary>
        public Pipeline<T> Do(Action<T> action)
        {
            action(_value);
            return this;
        }

        /// <summary>
        /// 条件执行副作用
        /// </summary>
        public Pipeline<T> DoIf(bool condition, Action<T> action)
        {
            if (condition) action(_value);
            return this;
        }

        /// <summary>
        /// 条件执行副作用
        /// </summary>
        public Pipeline<T> DoIf(Func<T, bool> predicate, Action<T> action)
        {
            if (predicate(_value)) action(_value);
            return this;
        }

        #endregion

        #region 验证操作

        /// <summary>
        /// 验证值
        /// </summary>
        public Pipeline<T> Validate(Func<T, bool> predicate, string errorMessage)
        {
            if (!predicate(_value))
                throw new InvalidOperationException(errorMessage);
            return this;
        }

        /// <summary>
        /// 验证值（返回验证结果）
        /// </summary>
        public ValidationResult<T> ValidateResult(Func<T, bool> predicate, string errorMessage)
        {
            return predicate(_value)
                ? ValidationResult<T>.Success(_value)
                : ValidationResult<T>.Failure(errorMessage);
        }

        #endregion

        #region 默认值操作

        /// <summary>
        /// 如果值为 null 则使用默认值
        /// <para>注意：对于值类型，此方法始终返回当前管道</para>
        /// </summary>
        public Pipeline<T> DefaultIfNull(T defaultValue)
        {
            return _value == null ? new Pipeline<T>(defaultValue) : this;
        }

        /// <summary>
        /// 如果值为 null 则使用工厂创建默认值
        /// <para>注意：对于值类型，此方法始终返回当前管道</para>
        /// </summary>
        public Pipeline<T> DefaultIfNull(Func<T> factory)
        {
            return _value == null ? new Pipeline<T>(factory()) : this;
        }

        #endregion

        #region 异常处理

        /// <summary>
        /// 尝试转换，失败返回默认值
        /// </summary>
        public Pipeline<TResult> TryMap<TResult>(Func<T, TResult> transform, TResult defaultValue = default)
        {
            try
            {
                return new Pipeline<TResult>(transform(_value));
            }
            catch
            {
                return new Pipeline<TResult>(defaultValue);
            }
        }

        /// <summary>
        /// 尝试转换，失败执行回退
        /// </summary>
        public Pipeline<TResult> TryMap<TResult>(Func<T, TResult> transform, Func<Exception, TResult> fallback)
        {
            try
            {
                return new Pipeline<TResult>(transform(_value));
            }
            catch (Exception ex)
            {
                return new Pipeline<TResult>(fallback(ex));
            }
        }

        #endregion

        #region 终结操作

        /// <summary>
        /// 获取值
        /// </summary>
        public T Run() => _value;

        /// <summary>
        /// 获取值或默认值
        /// </summary>
        public T RunOrDefault(T defaultValue = default)
        {
            return _value ?? defaultValue;
        }

        /// <summary>
        /// 执行最终操作
        /// </summary>
        public void RunWith(Action<T> action)
        {
            action(_value);
        }

        /// <summary>
        /// 转换为 Optional
        /// </summary>
        public Optional<T> ToOptional()
        {
            return _value != null ? Optional<T>.Some(_value) : Optional<T>.None;
        }

        #endregion

        #region 隐式转换

        public static implicit operator T(Pipeline<T> pipeline) => pipeline._value;

        #endregion

        #region 重写方法

        public override string ToString() => _value?.ToString() ?? "null";

        #endregion
    }

    /// <summary>
    /// 管道扩展方法
    /// </summary>
    public static class PipeExtensions
    {
        /// <summary>
        /// 将值转换为管道
        /// </summary>
        public static Pipeline<T> ToPipeline<T>(this T value)
        {
            return new Pipeline<T>(value);
        }

        /// <summary>
        /// 管道转换
        /// </summary>
        public static TResult PipeTo<T, TResult>(this T value, Func<T, TResult> transform)
        {
            return transform(value);
        }

        /// <summary>
        /// 链式管道转换
        /// </summary>
        public static TResult PipeTo<T, T1, TResult>(this T value, Func<T, T1> f1, Func<T1, TResult> f2)
        {
            return f2(f1(value));
        }

        /// <summary>
        /// 链式管道转换
        /// </summary>
        public static TResult PipeTo<T, T1, T2, TResult>(this T value, Func<T, T1> f1, Func<T1, T2> f2, Func<T2, TResult> f3)
        {
            return f3(f2(f1(value)));
        }

        /// <summary>
        /// 执行副作用并返回原值
        /// </summary>
        public static T Tap<T>(this T value, Action<T> action)
        {
            action(value);
            return value;
        }
    }
}
