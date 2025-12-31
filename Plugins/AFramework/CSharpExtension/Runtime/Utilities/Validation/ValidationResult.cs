// ==========================================================
// 文件名：ValidationResult.cs
// 命名空间: AFramework.CSharpExtension
// 依赖: System, System.Collections.Generic
// ==========================================================

using System;
using System.Collections.Generic;

namespace AFramework.CSharpExtension
{
    /// <summary>
    /// 验证结果
    /// <para>表示验证操作的结果，包含成功/失败状态和错误信息</para>
    /// </summary>
    public readonly struct ValidationResult
    {
        #region 字段

        private readonly List<string> _errors;

        #endregion

        #region 属性

        /// <summary>
        /// 验证是否成功
        /// </summary>
        public bool IsValid => _errors == null || _errors.Count == 0;

        /// <summary>
        /// 验证是否失败
        /// </summary>
        public bool IsInvalid => !IsValid;

        /// <summary>
        /// 错误消息列表
        /// </summary>
        public IReadOnlyList<string> Errors => _errors ?? (IReadOnlyList<string>)Array.Empty<string>();

        /// <summary>
        /// 错误数量
        /// </summary>
        public int ErrorCount => _errors?.Count ?? 0;

        /// <summary>
        /// 第一个错误消息
        /// </summary>
        public string FirstError => _errors != null && _errors.Count > 0 ? _errors[0] : null;

        /// <summary>
        /// 所有错误消息（用换行符连接）
        /// </summary>
        public string AllErrors => _errors != null ? string.Join(Environment.NewLine, _errors) : string.Empty;

        #endregion

        #region 构造函数

        private ValidationResult(List<string> errors)
        {
            _errors = errors;
        }

        #endregion

        #region 静态工厂方法

        /// <summary>
        /// 创建成功的验证结果
        /// </summary>
        public static ValidationResult Success() => new ValidationResult(null);

        /// <summary>
        /// 创建失败的验证结果
        /// </summary>
        public static ValidationResult Failure(string error)
        {
            return new ValidationResult(new List<string> { error });
        }

        /// <summary>
        /// 创建失败的验证结果（多个错误）
        /// </summary>
        public static ValidationResult Failure(IEnumerable<string> errors)
        {
            return new ValidationResult(new List<string>(errors));
        }

        /// <summary>
        /// 创建失败的验证结果（多个错误）
        /// </summary>
        public static ValidationResult Failure(params string[] errors)
        {
            return new ValidationResult(new List<string>(errors));
        }

        #endregion

        #region 操作方法

        /// <summary>
        /// 如果验证失败则抛出异常
        /// </summary>
        public void ThrowIfInvalid()
        {
            if (IsInvalid)
                throw new ValidationException(AllErrors, _errors);
        }

        /// <summary>
        /// 如果验证失败则抛出指定类型的异常
        /// </summary>
        public void ThrowIfInvalid<TException>() where TException : Exception
        {
            if (IsInvalid)
            {
                var exception = (TException)Activator.CreateInstance(typeof(TException), AllErrors);
                throw exception;
            }
        }

        #endregion

        #region 组合操作

        /// <summary>
        /// 合并两个验证结果
        /// </summary>
        public ValidationResult Merge(ValidationResult other)
        {
            if (IsValid && other.IsValid)
                return Success();

            var errors = new List<string>();
            if (_errors != null) errors.AddRange(_errors);
            if (other._errors != null) errors.AddRange(other._errors);
            return new ValidationResult(errors);
        }

        /// <summary>
        /// 合并多个验证结果
        /// </summary>
        public static ValidationResult Merge(params ValidationResult[] results)
        {
            var errors = new List<string>();
            foreach (var result in results)
            {
                if (result._errors != null)
                    errors.AddRange(result._errors);
            }
            return errors.Count == 0 ? Success() : new ValidationResult(errors);
        }

        /// <summary>
        /// 合并多个验证结果
        /// </summary>
        public static ValidationResult Merge(IEnumerable<ValidationResult> results)
        {
            var errors = new List<string>();
            foreach (var result in results)
            {
                if (result._errors != null)
                    errors.AddRange(result._errors);
            }
            return errors.Count == 0 ? Success() : new ValidationResult(errors);
        }

        #endregion

        #region 隐式转换

        /// <summary>
        /// 从布尔值隐式转换
        /// </summary>
        public static implicit operator bool(ValidationResult result) => result.IsValid;

        /// <summary>
        /// 从字符串隐式转换（创建失败结果）
        /// </summary>
        public static implicit operator ValidationResult(string error) => Failure(error);

        #endregion

        #region 重写方法

        public override string ToString()
        {
            return IsValid ? "验证成功" : $"验证失败: {AllErrors}";
        }

        #endregion
    }

    /// <summary>
    /// 带值的验证结果
    /// </summary>
    public readonly struct ValidationResult<T>
    {
        #region 字段

        private readonly T _value;
        private readonly List<string> _errors;

        #endregion

        #region 属性

        /// <summary>
        /// 验证是否成功
        /// </summary>
        public bool IsValid => _errors == null || _errors.Count == 0;

        /// <summary>
        /// 验证是否失败
        /// </summary>
        public bool IsInvalid => !IsValid;

        /// <summary>
        /// 验证通过时的值
        /// </summary>
        public T Value
        {
            get
            {
                if (IsInvalid)
                    throw new InvalidOperationException("验证失败，无法获取值");
                return _value;
            }
        }

        /// <summary>
        /// 错误消息列表
        /// </summary>
        public IReadOnlyList<string> Errors => _errors ?? (IReadOnlyList<string>)Array.Empty<string>();

        /// <summary>
        /// 第一个错误消息
        /// </summary>
        public string FirstError => _errors != null && _errors.Count > 0 ? _errors[0] : null;

        #endregion

        #region 构造函数

        private ValidationResult(T value, List<string> errors)
        {
            _value = value;
            _errors = errors;
        }

        #endregion

        #region 静态工厂方法

        /// <summary>
        /// 创建成功的验证结果
        /// </summary>
        public static ValidationResult<T> Success(T value) => new ValidationResult<T>(value, null);

        /// <summary>
        /// 创建失败的验证结果
        /// </summary>
        public static ValidationResult<T> Failure(string error)
        {
            return new ValidationResult<T>(default, new List<string> { error });
        }

        /// <summary>
        /// 创建失败的验证结果（多个错误）
        /// </summary>
        public static ValidationResult<T> Failure(IEnumerable<string> errors)
        {
            return new ValidationResult<T>(default, new List<string>(errors));
        }

        #endregion

        #region 操作方法

        /// <summary>
        /// 获取值或默认值
        /// </summary>
        public T GetValueOrDefault(T defaultValue = default)
        {
            return IsValid ? _value : defaultValue;
        }

        /// <summary>
        /// 如果验证失败则抛出异常
        /// </summary>
        public T GetValueOrThrow()
        {
            if (IsInvalid)
                throw new ValidationException(string.Join(Environment.NewLine, _errors), _errors);
            return _value;
        }

        /// <summary>
        /// 转换为无值的验证结果
        /// </summary>
        public ValidationResult ToResult()
        {
            return IsValid ? ValidationResult.Success() : ValidationResult.Failure(_errors);
        }

        #endregion

        #region 隐式转换

        public static implicit operator bool(ValidationResult<T> result) => result.IsValid;

        #endregion
    }

    /// <summary>
    /// 验证异常
    /// </summary>
    public class ValidationException : Exception
    {
        /// <summary>
        /// 错误列表
        /// </summary>
        public IReadOnlyList<string> Errors { get; }

        public ValidationException(string message) : base(message)
        {
            Errors = new[] { message };
        }

        public ValidationException(string message, IEnumerable<string> errors) : base(message)
        {
            Errors = new List<string>(errors);
        }
    }
}
