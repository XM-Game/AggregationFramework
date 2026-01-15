// ==========================================================
// 文件名：FormatterHelper.cs
// 命名空间: AFramework.Serialization
// 依赖: System, System.Reflection, System.Collections.Concurrent
// ==========================================================

using System;
using System.Collections.Concurrent;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace AFramework.Serialization
{
    /// <summary>
    /// 格式化器辅助工具类
    /// <para>提供格式化器通用功能</para>
    /// </summary>
    /// <remarks>
    /// 设计说明:
    /// 1. 提供属性复制功能（带缓存）
    /// 2. 提供类型检查工具
    /// 3. 避免重复反射调用
    /// </remarks>
    internal static class FormatterHelper
    {
        #region 字段

        /// <summary>属性信息缓存</summary>
        private static readonly ConcurrentDictionary<Type, PropertyInfo[]> _propertyCache
            = new ConcurrentDictionary<Type, PropertyInfo[]>();

        /// <summary>非托管类型缓存</summary>
        private static readonly ConcurrentDictionary<Type, bool> _unmanagedCache
            = new ConcurrentDictionary<Type, bool>();

        /// <summary>属性绑定标志</summary>
        private const BindingFlags PropertyBindingFlags =
            BindingFlags.Public | BindingFlags.Instance;

        #endregion

        #region 属性复制

        /// <summary>
        /// 复制对象属性
        /// </summary>
        /// <param name="source">源对象</param>
        /// <param name="target">目标对象</param>
        /// <param name="type">类型</param>
        public static void CopyProperties(object source, object target, Type type)
        {
            if (source == null || target == null)
                return;

            var properties = GetCachedProperties(type);

            foreach (var prop in properties)
            {
                if (!prop.CanRead || !prop.CanWrite)
                    continue;

                try
                {
                    var value = prop.GetValue(source);
                    prop.SetValue(target, value);
                }
                catch
                {
                    // 忽略无法复制的属性
                }
            }
        }

        /// <summary>
        /// 获取缓存的属性信息
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns>属性信息数组</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static PropertyInfo[] GetCachedProperties(Type type)
        {
            return _propertyCache.GetOrAdd(type, t => t.GetProperties(PropertyBindingFlags));
        }

        #endregion

        #region 类型检查

        /// <summary>
        /// 检查类型是否为非托管类型
        /// </summary>
        /// <param name="type">要检查的类型</param>
        /// <returns>如果是非托管类型返回 true</returns>
        public static bool IsUnmanagedType(Type type)
        {
            return _unmanagedCache.GetOrAdd(type, CheckUnmanagedInternal);
        }

        /// <summary>
        /// 检查类型是否为非托管类型 (内部实现)
        /// </summary>
        private static bool CheckUnmanagedInternal(Type type)
        {
            // 基础类型检查
            if (type.IsPrimitive || type.IsEnum)
                return true;

            // 引用类型不是非托管类型
            if (!type.IsValueType)
                return false;

            // 检查所有字段是否都是非托管类型
            var fields = type.GetFields(
                BindingFlags.Public |
                BindingFlags.NonPublic |
                BindingFlags.Instance);

            foreach (var field in fields)
            {
                var fieldType = field.FieldType;

                // 递归检查
                if (!fieldType.IsPrimitive && !fieldType.IsEnum)
                {
                    if (!fieldType.IsValueType || !IsUnmanagedType(fieldType))
                        return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 检查类型是否为值类型
        /// </summary>
        /// <typeparam name="T">要检查的类型</typeparam>
        /// <returns>如果是值类型返回 true</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsValueType<T>()
        {
            return typeof(T).IsValueType;
        }

        /// <summary>
        /// 检查类型是否支持 null
        /// </summary>
        /// <typeparam name="T">要检查的类型</typeparam>
        /// <returns>如果支持 null 返回 true</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SupportsNull<T>()
        {
            return !typeof(T).IsValueType;
        }

        #endregion

        #region VarInt 大小计算

        /// <summary>
        /// 获取 VarInt 编码大小
        /// </summary>
        /// <param name="value">值</param>
        /// <returns>编码后的字节数</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetVarIntSize(int value)
        {
            if (value < 0)
                return 5; // 负数需要 5 字节

            if (value < 0x80)
                return 1;
            if (value < 0x4000)
                return 2;
            if (value < 0x200000)
                return 3;
            if (value < 0x10000000)
                return 4;

            return 5;
        }

        /// <summary>
        /// 获取 VarInt 编码大小 (无符号)
        /// </summary>
        /// <param name="value">值</param>
        /// <returns>编码后的字节数</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetVarIntSize(uint value)
        {
            if (value < 0x80)
                return 1;
            if (value < 0x4000)
                return 2;
            if (value < 0x200000)
                return 3;
            if (value < 0x10000000)
                return 4;

            return 5;
        }

        /// <summary>
        /// 获取 VarLong 编码大小
        /// </summary>
        /// <param name="value">值</param>
        /// <returns>编码后的字节数</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetVarLongSize(long value)
        {
            if (value < 0)
                return 10; // 负数需要 10 字节

            return GetVarLongSize((ulong)value);
        }

        /// <summary>
        /// 获取 VarLong 编码大小 (无符号)
        /// </summary>
        /// <param name="value">值</param>
        /// <returns>编码后的字节数</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetVarLongSize(ulong value)
        {
            var size = 1;
            while (value >= 0x80)
            {
                value >>= 7;
                size++;
            }
            return size;
        }

        #endregion

        #region 缓存管理

        /// <summary>
        /// 清除所有缓存
        /// </summary>
        public static void ClearCache()
        {
            _propertyCache.Clear();
            _unmanagedCache.Clear();
        }

        /// <summary>
        /// 获取属性缓存数量
        /// </summary>
        public static int PropertyCacheCount => _propertyCache.Count;

        /// <summary>
        /// 获取非托管类型缓存数量
        /// </summary>
        public static int UnmanagedCacheCount => _unmanagedCache.Count;

        #endregion
    }
}
