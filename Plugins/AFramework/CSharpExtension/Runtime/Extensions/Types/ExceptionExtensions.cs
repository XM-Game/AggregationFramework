// ==========================================================
// 文件名：ExceptionExtensions.cs
// 命名空间: AFramework.CSharpExtension
// 依赖: System, System.Text
// ==========================================================

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace AFramework.CSharpExtension
{
    /// <summary>
    /// Exception 扩展方法
    /// <para>提供异常的常用操作扩展，包括异常链遍历、格式化输出、重新抛出等功能</para>
    /// </summary>
    public static class ExceptionExtensions
    {
        #region 异常链操作

        /// <summary>
        /// 获取最内层异常
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Exception GetInnermostException(this Exception exception)
        {
            if (exception == null) return null;
            
            while (exception.InnerException != null)
                exception = exception.InnerException;
            
            return exception;
        }

        /// <summary>
        /// 获取异常链中的所有异常
        /// </summary>
        public static IEnumerable<Exception> GetAllExceptions(this Exception exception)
        {
            var current = exception;
            while (current != null)
            {
                yield return current;
                current = current.InnerException;
            }
        }

        /// <summary>
        /// 获取异常链的深度
        /// </summary>
        public static int GetExceptionDepth(this Exception exception)
        {
            int depth = 0;
            var current = exception;
            while (current != null)
            {
                depth++;
                current = current.InnerException;
            }
            return depth;
        }

        /// <summary>
        /// 检查异常链中是否包含指定类型的异常
        /// </summary>
        public static bool Contains<TException>(this Exception exception) where TException : Exception
        {
            var current = exception;
            while (current != null)
            {
                if (current is TException)
                    return true;
                current = current.InnerException;
            }
            return false;
        }

        /// <summary>
        /// 获取异常链中指定类型的异常
        /// </summary>
        public static TException Find<TException>(this Exception exception) where TException : Exception
        {
            var current = exception;
            while (current != null)
            {
                if (current is TException typed)
                    return typed;
                current = current.InnerException;
            }
            return null;
        }

        #endregion

        #region 格式化输出

        /// <summary>
        /// 获取完整的异常消息（包含所有内部异常）
        /// </summary>
        public static string GetFullMessage(this Exception exception)
        {
            if (exception == null) return string.Empty;
            
            var sb = new StringBuilder();
            var current = exception;
            int level = 0;
            
            while (current != null)
            {
                if (level > 0)
                    sb.Append(" ---> ");
                
                sb.Append($"[{current.GetType().Name}] {current.Message}");
                current = current.InnerException;
                level++;
            }
            
            return sb.ToString();
        }

        /// <summary>
        /// 获取格式化的异常详情
        /// </summary>
        public static string ToDetailedString(this Exception exception)
        {
            if (exception == null) return string.Empty;
            
            var sb = new StringBuilder();
            var current = exception;
            int level = 0;
            
            while (current != null)
            {
                if (level > 0)
                {
                    sb.AppendLine();
                    sb.AppendLine(new string('-', 50));
                    sb.AppendLine($"Inner Exception (Level {level}):");
                }
                
                sb.AppendLine($"Type: {current.GetType().FullName}");
                sb.AppendLine($"Message: {current.Message}");
                
                if (!string.IsNullOrEmpty(current.Source))
                    sb.AppendLine($"Source: {current.Source}");
                
                if (current.TargetSite != null)
                    sb.AppendLine($"TargetSite: {current.TargetSite}");
                
                if (!string.IsNullOrEmpty(current.StackTrace))
                {
                    sb.AppendLine("StackTrace:");
                    sb.AppendLine(current.StackTrace);
                }
                
                current = current.InnerException;
                level++;
            }
            
            return sb.ToString();
        }

        /// <summary>
        /// 获取简洁的异常摘要
        /// </summary>
        public static string ToSummary(this Exception exception)
        {
            if (exception == null) return string.Empty;
            
            var innermost = exception.GetInnermostException();
            return $"[{innermost.GetType().Name}] {innermost.Message}";
        }

        /// <summary>
        /// 获取异常的堆栈跟踪（安全获取）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetStackTraceSafe(this Exception exception)
        {
            return exception?.StackTrace ?? string.Empty;
        }

        #endregion

        #region 异常数据

        /// <summary>
        /// 添加数据到异常
        /// </summary>
        public static TException WithData<TException>(this TException exception, string key, object value) 
            where TException : Exception
        {
            if (exception != null && key != null)
                exception.Data[key] = value;
            return exception;
        }

        /// <summary>
        /// 添加多个数据到异常
        /// </summary>
        public static TException WithData<TException>(this TException exception, IDictionary<string, object> data) 
            where TException : Exception
        {
            if (exception != null && data != null)
            {
                foreach (var kvp in data)
                    exception.Data[kvp.Key] = kvp.Value;
            }
            return exception;
        }

        /// <summary>
        /// 获取异常数据
        /// </summary>
        public static T GetData<T>(this Exception exception, string key, T defaultValue = default)
        {
            if (exception == null || key == null || !exception.Data.Contains(key))
                return defaultValue;
            
            try
            {
                return (T)exception.Data[key];
            }
            catch
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// 检查异常是否包含指定数据
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasData(this Exception exception, string key)
        {
            return exception != null && key != null && exception.Data.Contains(key);
        }

        #endregion

        #region 异常类型检查

        /// <summary>
        /// 检查是否为致命异常（不应被捕获）
        /// </summary>
        public static bool IsFatal(this Exception exception)
        {
            return exception is OutOfMemoryException ||
                   exception is StackOverflowException ||
                   exception is AccessViolationException ||
                   exception is AppDomainUnloadedException ||
                   exception is System.Threading.ThreadAbortException;
        }

        /// <summary>
        /// 检查是否为操作取消异常
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsCancellation(this Exception exception)
        {
            return exception is OperationCanceledException ||
                   exception.Contains<OperationCanceledException>();
        }

        /// <summary>
        /// 检查是否为超时异常
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsTimeout(this Exception exception)
        {
            return exception is TimeoutException ||
                   exception.Contains<TimeoutException>();
        }

        /// <summary>
        /// 检查是否为参数异常
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsArgumentException(this Exception exception)
        {
            return exception is ArgumentException;
        }

        /// <summary>
        /// 检查是否为 IO 异常
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsIOException(this Exception exception)
        {
            return exception is System.IO.IOException ||
                   exception.Contains<System.IO.IOException>();
        }

        #endregion

        #region 重新抛出

        /// <summary>
        /// 重新抛出异常（保留原始堆栈跟踪）
        /// </summary>
        public static void Rethrow(this Exception exception)
        {
            if (exception == null) return;
            
            System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(exception).Throw();
        }

        /// <summary>
        /// 包装并抛出异常
        /// </summary>
        public static void ThrowWrapped(this Exception exception, string message)
        {
            if (exception == null) return;
            
            throw new Exception(message, exception);
        }

        /// <summary>
        /// 包装为指定类型的异常并抛出
        /// </summary>
        public static void ThrowAs<TException>(this Exception exception, string message = null) 
            where TException : Exception
        {
            if (exception == null) return;
            
            var wrappedException = (TException)Activator.CreateInstance(
                typeof(TException), 
                message ?? exception.Message, 
                exception);
            
            throw wrappedException;
        }

        #endregion

        #region 日志辅助

        /// <summary>
        /// 转换为日志友好的字符串
        /// </summary>
        public static string ToLogString(this Exception exception, bool includeStackTrace = true)
        {
            if (exception == null) return string.Empty;
            
            var sb = new StringBuilder();
            sb.Append($"[{exception.GetType().Name}] {exception.Message}");
            
            if (includeStackTrace && !string.IsNullOrEmpty(exception.StackTrace))
            {
                sb.AppendLine();
                sb.Append(exception.StackTrace);
            }
            
            if (exception.InnerException != null)
            {
                sb.AppendLine();
                sb.Append("Inner: ");
                sb.Append(exception.InnerException.ToLogString(includeStackTrace));
            }
            
            return sb.ToString();
        }

        /// <summary>
        /// 获取异常的简短描述（用于日志标题）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ToShortString(this Exception exception)
        {
            if (exception == null) return string.Empty;
            return $"{exception.GetType().Name}: {exception.Message}";
        }

        #endregion

        #region AggregateException 处理

        /// <summary>
        /// 展平 AggregateException
        /// </summary>
        public static IEnumerable<Exception> Flatten(this Exception exception)
        {
            if (exception == null) yield break;
            
            if (exception is AggregateException aggEx)
            {
                foreach (var inner in aggEx.Flatten().InnerExceptions)
                    yield return inner;
            }
            else
            {
                yield return exception;
            }
        }

        /// <summary>
        /// 获取 AggregateException 中的第一个异常
        /// </summary>
        public static Exception GetFirstException(this Exception exception)
        {
            if (exception is AggregateException aggEx && aggEx.InnerExceptions.Count > 0)
                return aggEx.InnerExceptions[0];
            
            return exception;
        }

        #endregion

        #region 条件处理

        /// <summary>
        /// 如果满足条件则处理异常
        /// </summary>
        public static void HandleIf<TException>(this Exception exception, Action<TException> handler) 
            where TException : Exception
        {
            if (exception is TException typed && handler != null)
                handler(typed);
        }

        /// <summary>
        /// 匹配异常类型并执行对应处理
        /// </summary>
        public static void Match(this Exception exception, 
            Action<ArgumentException> onArgument = null,
            Action<InvalidOperationException> onInvalidOperation = null,
            Action<NotSupportedException> onNotSupported = null,
            Action<Exception> onOther = null)
        {
            if (exception == null) return;
            
            switch (exception)
            {
                case ArgumentException argEx when onArgument != null:
                    onArgument(argEx);
                    break;
                case InvalidOperationException invOpEx when onInvalidOperation != null:
                    onInvalidOperation(invOpEx);
                    break;
                case NotSupportedException notSupEx when onNotSupported != null:
                    onNotSupported(notSupEx);
                    break;
                default:
                    onOther?.Invoke(exception);
                    break;
            }
        }

        #endregion
    }
}
