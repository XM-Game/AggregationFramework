// ==========================================================
// 文件名：Validator.cs
// 命名空间: AFramework.CSharpExtension
// 依赖: System, System.Collections.Generic
// ==========================================================

using System;
using System.Collections.Generic;

namespace AFramework.CSharpExtension
{
    /// <summary>
    /// 验证器基类
    /// <para>提供流式验证 API，支持链式调用</para>
    /// </summary>
    /// <typeparam name="T">要验证的类型</typeparam>
    public class Validator<T>
    {
        #region 字段

        private readonly T _value;
        private readonly List<string> _errors;
        private readonly string _name;

        #endregion

        #region 构造函数

        /// <summary>
        /// 创建验证器
        /// </summary>
        /// <param name="value">要验证的值</param>
        /// <param name="name">值的名称（用于错误消息）</param>
        public Validator(T value, string name = null)
        {
            _value = value;
            _name = name ?? "值";
            _errors = new List<string>();
        }

        #endregion

        #region 属性

        /// <summary>
        /// 要验证的值
        /// </summary>
        public T Value => _value;

        /// <summary>
        /// 验证是否通过
        /// </summary>
        public bool IsValid => _errors.Count == 0;

        /// <summary>
        /// 错误列表
        /// </summary>
        public IReadOnlyList<string> Errors => _errors;

        #endregion

        #region 验证方法

        /// <summary>
        /// 添加自定义验证规则
        /// </summary>
        public Validator<T> Must(Func<T, bool> predicate, string errorMessage)
        {
            if (!predicate(_value))
                _errors.Add(errorMessage);
            return this;
        }

        /// <summary>
        /// 添加自定义验证规则（带值名称）
        /// </summary>
        public Validator<T> Must(Func<T, bool> predicate, Func<string, string> errorMessageFactory)
        {
            if (!predicate(_value))
                _errors.Add(errorMessageFactory(_name));
            return this;
        }

        /// <summary>
        /// 条件验证
        /// </summary>
        public Validator<T> When(bool condition, Action<Validator<T>> validation)
        {
            if (condition)
                validation(this);
            return this;
        }

        /// <summary>
        /// 条件验证
        /// </summary>
        public Validator<T> When(Func<T, bool> condition, Action<Validator<T>> validation)
        {
            if (condition(_value))
                validation(this);
            return this;
        }

        #endregion

        #region 结果方法

        /// <summary>
        /// 获取验证结果
        /// </summary>
        public ValidationResult GetResult()
        {
            return _errors.Count == 0 ? ValidationResult.Success() : ValidationResult.Failure(_errors);
        }

        /// <summary>
        /// 获取带值的验证结果
        /// </summary>
        public ValidationResult<T> GetResultWithValue()
        {
            return _errors.Count == 0 
                ? ValidationResult<T>.Success(_value) 
                : ValidationResult<T>.Failure(_errors);
        }

        /// <summary>
        /// 如果验证失败则抛出异常
        /// </summary>
        public T ThrowIfInvalid()
        {
            if (_errors.Count > 0)
                throw new ValidationException(string.Join(Environment.NewLine, _errors), _errors);
            return _value;
        }

        #endregion

        #region 静态工厂方法

        /// <summary>
        /// 创建验证器
        /// </summary>
        public static Validator<T> For(T value, string name = null)
        {
            return new Validator<T>(value, name);
        }

        #endregion
    }

    /// <summary>
    /// 验证器扩展方法
    /// </summary>
    public static class ValidatorExtensions
    {
        #region 通用验证

        /// <summary>
        /// 验证不为 null
        /// </summary>
        public static Validator<T> NotNull<T>(this Validator<T> validator) where T : class
        {
            return validator.Must(v => v != null, name => $"{name} 不能为 null");
        }

        /// <summary>
        /// 验证相等
        /// </summary>
        public static Validator<T> Equal<T>(this Validator<T> validator, T expected) where T : IEquatable<T>
        {
            return validator.Must(v => v != null && v.Equals(expected), name => $"{name} 必须等于 {expected}");
        }

        /// <summary>
        /// 验证不相等
        /// </summary>
        public static Validator<T> NotEqual<T>(this Validator<T> validator, T unexpected) where T : IEquatable<T>
        {
            return validator.Must(v => v == null || !v.Equals(unexpected), name => $"{name} 不能等于 {unexpected}");
        }

        #endregion

        #region 字符串验证

        /// <summary>
        /// 验证字符串不为空
        /// </summary>
        public static Validator<string> NotEmpty(this Validator<string> validator)
        {
            return validator.Must(v => !string.IsNullOrEmpty(v), name => $"{name} 不能为空");
        }

        /// <summary>
        /// 验证字符串不为空白
        /// </summary>
        public static Validator<string> NotWhiteSpace(this Validator<string> validator)
        {
            return validator.Must(v => !string.IsNullOrWhiteSpace(v), name => $"{name} 不能为空白");
        }

        /// <summary>
        /// 验证字符串长度
        /// </summary>
        public static Validator<string> Length(this Validator<string> validator, int minLength, int maxLength)
        {
            return validator.Must(
                v => v != null && v.Length >= minLength && v.Length <= maxLength,
                name => $"{name} 长度必须在 {minLength} 到 {maxLength} 之间");
        }

        /// <summary>
        /// 验证字符串最小长度
        /// </summary>
        public static Validator<string> MinLength(this Validator<string> validator, int minLength)
        {
            return validator.Must(
                v => v != null && v.Length >= minLength,
                name => $"{name} 长度不能小于 {minLength}");
        }

        /// <summary>
        /// 验证字符串最大长度
        /// </summary>
        public static Validator<string> MaxLength(this Validator<string> validator, int maxLength)
        {
            return validator.Must(
                v => v == null || v.Length <= maxLength,
                name => $"{name} 长度不能大于 {maxLength}");
        }

        /// <summary>
        /// 验证字符串匹配正则表达式
        /// </summary>
        public static Validator<string> Matches(this Validator<string> validator, string pattern, string patternName = "指定格式")
        {
            return validator.Must(
                v => v != null && System.Text.RegularExpressions.Regex.IsMatch(v, pattern),
                name => $"{name} 必须匹配{patternName}");
        }

        #endregion

        #region 数值验证

        /// <summary>
        /// 验证值在范围内
        /// </summary>
        public static Validator<int> InRange(this Validator<int> validator, int min, int max)
        {
            return validator.Must(v => v >= min && v <= max, name => $"{name} 必须在 {min} 到 {max} 之间");
        }

        /// <summary>
        /// 验证值在范围内
        /// </summary>
        public static Validator<float> InRange(this Validator<float> validator, float min, float max)
        {
            return validator.Must(v => v >= min && v <= max, name => $"{name} 必须在 {min} 到 {max} 之间");
        }

        /// <summary>
        /// 验证值为正数
        /// </summary>
        public static Validator<int> Positive(this Validator<int> validator)
        {
            return validator.Must(v => v > 0, name => $"{name} 必须为正数");
        }

        /// <summary>
        /// 验证值为正数
        /// </summary>
        public static Validator<float> Positive(this Validator<float> validator)
        {
            return validator.Must(v => v > 0f, name => $"{name} 必须为正数");
        }

        /// <summary>
        /// 验证值为非负数
        /// </summary>
        public static Validator<int> NonNegative(this Validator<int> validator)
        {
            return validator.Must(v => v >= 0, name => $"{name} 不能为负数");
        }

        /// <summary>
        /// 验证值为非负数
        /// </summary>
        public static Validator<float> NonNegative(this Validator<float> validator)
        {
            return validator.Must(v => v >= 0f, name => $"{name} 不能为负数");
        }

        /// <summary>
        /// 验证值大于指定值
        /// </summary>
        public static Validator<T> GreaterThan<T>(this Validator<T> validator, T threshold) where T : IComparable<T>
        {
            return validator.Must(v => v != null && v.CompareTo(threshold) > 0, name => $"{name} 必须大于 {threshold}");
        }

        /// <summary>
        /// 验证值小于指定值
        /// </summary>
        public static Validator<T> LessThan<T>(this Validator<T> validator, T threshold) where T : IComparable<T>
        {
            return validator.Must(v => v != null && v.CompareTo(threshold) < 0, name => $"{name} 必须小于 {threshold}");
        }

        #endregion

        #region 集合验证

        /// <summary>
        /// 验证集合不为空
        /// </summary>
        public static Validator<T[]> NotEmpty<T>(this Validator<T[]> validator)
        {
            return validator.Must(v => v != null && v.Length > 0, name => $"{name} 不能为空");
        }

        /// <summary>
        /// 验证集合不为空
        /// </summary>
        public static Validator<ICollection<T>> NotEmpty<T>(this Validator<ICollection<T>> validator)
        {
            return validator.Must(v => v != null && v.Count > 0, name => $"{name} 不能为空");
        }

        /// <summary>
        /// 验证集合元素数量
        /// </summary>
        public static Validator<T[]> Count<T>(this Validator<T[]> validator, int min, int max)
        {
            return validator.Must(
                v => v != null && v.Length >= min && v.Length <= max,
                name => $"{name} 元素数量必须在 {min} 到 {max} 之间");
        }

        #endregion
    }

    /// <summary>
    /// 验证工具类
    /// </summary>
    public static class Validate
    {
        /// <summary>
        /// 创建验证器
        /// </summary>
        public static Validator<T> That<T>(T value, string name = null)
        {
            return new Validator<T>(value, name);
        }
    }
}
