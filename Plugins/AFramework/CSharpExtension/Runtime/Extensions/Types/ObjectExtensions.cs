// ==========================================================
// 文件名：ObjectExtensions.cs
// 命名空间: AFramework.CSharpExtension
// 依赖: System
// ==========================================================

using System;
using System.Runtime.CompilerServices;

namespace AFramework.CSharpExtension
{
    /// <summary>
    /// Object 扩展方法
    /// <para>提供对象的常用操作扩展，包括空值检查、类型转换、克隆等功能</para>
    /// </summary>
    public static class ObjectExtensions
    {
        #region 空值检查

        /// <summary>
        /// 检查对象是否为 null
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNull(this object obj)
        {
            return obj == null;
        }

        /// <summary>
        /// 检查对象是否不为 null
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNotNull(this object obj)
        {
            return obj != null;
        }

        /// <summary>
        /// 如果对象为 null，返回默认值
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T OrDefault<T>(this T obj, T defaultValue = default)
        {
            return obj != null ? obj : defaultValue;
        }

        /// <summary>
        /// 如果对象为 null，通过工厂方法创建默认值
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T OrDefault<T>(this T obj, Func<T> defaultFactory)
        {
            return obj != null ? obj : (defaultFactory != null ? defaultFactory() : default);
        }

        #endregion

        #region 类型检查和转换

        /// <summary>
        /// 检查对象是否为指定类型
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Is<T>(this object obj)
        {
            return obj is T;
        }

        /// <summary>
        /// 检查对象是否不是指定类型
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNot<T>(this object obj)
        {
            return !(obj is T);
        }

        /// <summary>
        /// 尝试转换为指定类型
        /// </summary>
        public static bool TryAs<T>(this object obj, out T result) where T : class
        {
            result = obj as T;
            return result != null;
        }

        /// <summary>
        /// 安全转换为指定类型（失败返回默认值）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T As<T>(this object obj, T defaultValue = default) where T : class
        {
            return obj as T ?? defaultValue;
        }

        /// <summary>
        /// 强制转换为指定类型
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Cast<T>(this object obj)
        {
            return (T)obj;
        }

        #endregion

        #region 条件执行

        /// <summary>
        /// 如果对象不为 null，执行操作
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T IfNotNull<T>(this T obj, Action<T> action) where T : class
        {
            if (obj != null && action != null)
                action(obj);
            return obj;
        }

        /// <summary>
        /// 如果对象不为 null，执行转换
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TResult IfNotNull<T, TResult>(this T obj, Func<T, TResult> func, TResult defaultValue = default) where T : class
        {
            return obj != null && func != null ? func(obj) : defaultValue;
        }

        #endregion

        #region 链式调用

        /// <summary>
        /// 对对象执行操作并返回自身（用于链式调用）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Do<T>(this T obj, Action<T> action)
        {
            action?.Invoke(obj);
            return obj;
        }

        /// <summary>
        /// 对对象执行转换
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TResult Map<T, TResult>(this T obj, Func<T, TResult> func)
        {
            return func != null ? func(obj) : default;
        }

        #endregion

        #region 相等性比较

        /// <summary>
        /// 检查对象是否在指定集合中
        /// </summary>
        public static bool In<T>(this T obj, params T[] values)
        {
            if (values == null || values.Length == 0)
                return false;

            foreach (var value in values)
            {
                if (Equals(obj, value))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// 检查对象是否不在指定集合中
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool NotIn<T>(this T obj, params T[] values)
        {
            return !obj.In(values);
        }

        #endregion

        #region 转换为 Optional

        /// <summary>
        /// 转换为 Optional
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Optional<T> ToOptional<T>(this T obj)
        {
            return Optional<T>.FromNullable(obj);
        }

        #endregion

        #region 深拷贝（需要序列化支持）

        /// <summary>
        /// 深拷贝对象（使用二进制序列化）
        /// </summary>
        /// <remarks>
        /// 注意：对象必须标记为 [Serializable]
        /// </remarks>
        public static T DeepClone<T>(this T obj) where T : class
        {
            if (obj == null)
                return null;

            try
            {
                using (var ms = new System.IO.MemoryStream())
                {
                    var formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                    formatter.Serialize(ms, obj);
                    ms.Position = 0;
                    return (T)formatter.Deserialize(ms);
                }
            }
            catch
            {
                return null;
            }
        }

        #endregion

        #region 调试辅助

        /// <summary>
        /// 获取对象的类型名称
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetTypeName(this object obj)
        {
            return obj?.GetType().Name ?? "null";
        }

        /// <summary>
        /// 获取对象的完整类型名称
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetFullTypeName(this object obj)
        {
            return obj?.GetType().FullName ?? "null";
        }

        /// <summary>
        /// 转换为调试字符串
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ToDebugString(this object obj)
        {
            if (obj == null)
                return "null";

            return $"[{obj.GetType().Name}] {obj}";
        }

        #endregion

        #region 哈希码

        /// <summary>
        /// 安全获取哈希码（null 返回 0）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetHashCodeSafe(this object obj)
        {
            return obj?.GetHashCode() ?? 0;
        }

        #endregion

        #region 异常处理

        /// <summary>
        /// 尝试执行操作，捕获异常并返回结果
        /// </summary>
        public static Result<TResult> Try<T, TResult>(this T obj, Func<T, TResult> func)
        {
            if (obj == null || func == null)
                return Result<TResult>.Failure("Object or function is null.");

            try
            {
                return Result<TResult>.Success(func(obj));
            }
            catch (Exception ex)
            {
                return Result<TResult>.FromException(ex);
            }
        }

        /// <summary>
        /// 尝试执行操作，捕获异常
        /// </summary>
        public static Result<T> Try<T>(this T obj, Action<T> action)
        {
            if (obj == null || action == null)
                return Result<T>.Failure("Object or action is null.");

            try
            {
                action(obj);
                return Result<T>.Success(obj);
            }
            catch (Exception ex)
            {
                return Result<T>.FromException(ex);
            }
        }

        #endregion
    }
}
