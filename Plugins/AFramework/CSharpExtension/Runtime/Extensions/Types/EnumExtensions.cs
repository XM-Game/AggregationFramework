// ==========================================================
// 文件名：EnumExtensions.cs
// 命名空间: AFramework.CSharpExtension
// 依赖: System, System.Linq
// ==========================================================

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace AFramework.CSharpExtension
{
    /// <summary>
    /// 枚举扩展方法
    /// <para>提供枚举的常用操作扩展，包括转换、验证、获取描述等功能</para>
    /// </summary>
    public static class EnumExtensions
    {
        #region 基础操作

        /// <summary>
        /// 获取枚举的整数值
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ToInt<TEnum>(this TEnum value) where TEnum : Enum
        {
            return Convert.ToInt32(value);
        }

        /// <summary>
        /// 获取枚举的长整数值
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long ToLong<TEnum>(this TEnum value) where TEnum : Enum
        {
            return Convert.ToInt64(value);
        }

        /// <summary>
        /// 获取枚举的字符串表示
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ToStringValue<TEnum>(this TEnum value) where TEnum : Enum
        {
            return value.ToString();
        }

        /// <summary>
        /// 从整数值创建枚举
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TEnum ToEnum<TEnum>(this int value) where TEnum : Enum
        {
            return (TEnum)Enum.ToObject(typeof(TEnum), value);
        }

        /// <summary>
        /// 从字符串创建枚举
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TEnum ToEnum<TEnum>(this string value, bool ignoreCase = true) where TEnum : struct, Enum
        {
            return Enum.TryParse<TEnum>(value, ignoreCase, out var result) ? result : default;
        }

        /// <summary>
        /// 尝试从字符串创建枚举
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryToEnum<TEnum>(this string value, out TEnum result, bool ignoreCase = true) where TEnum : struct, Enum
        {
            return Enum.TryParse(value, ignoreCase, out result);
        }

        #endregion

        #region 验证操作

        /// <summary>
        /// 检查枚举值是否已定义
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsDefined<TEnum>(this TEnum value) where TEnum : Enum
        {
            return Enum.IsDefined(typeof(TEnum), value);
        }

        /// <summary>
        /// 检查枚举值是否有效（已定义）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsValid<TEnum>(this TEnum value) where TEnum : Enum
        {
            return value.IsDefined();
        }

        #endregion

        #region 标志位操作

        /// <summary>
        /// 检查是否包含指定标志位
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasFlag<TEnum>(this TEnum value, TEnum flag) where TEnum : Enum
        {
            return value.HasFlag(flag);
        }

        /// <summary>
        /// 检查是否包含任意一个指定标志位
        /// </summary>
        public static bool HasAnyFlag<TEnum>(this TEnum value, params TEnum[] flags) where TEnum : Enum
        {
            if (flags == null || flags.Length == 0)
                return false;

            foreach (var flag in flags)
            {
                if (value.HasFlag(flag))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// 检查是否包含所有指定标志位
        /// </summary>
        public static bool HasAllFlags<TEnum>(this TEnum value, params TEnum[] flags) where TEnum : Enum
        {
            if (flags == null || flags.Length == 0)
                return false;

            foreach (var flag in flags)
            {
                if (!value.HasFlag(flag))
                    return false;
            }
            return true;
        }

        /// <summary>
        /// 添加标志位
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TEnum AddFlag<TEnum>(this TEnum value, TEnum flag) where TEnum : Enum
        {
            long valueAsLong = Convert.ToInt64(value);
            long flagAsLong = Convert.ToInt64(flag);
            return (TEnum)Enum.ToObject(typeof(TEnum), valueAsLong | flagAsLong);
        }

        /// <summary>
        /// 移除标志位
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TEnum RemoveFlag<TEnum>(this TEnum value, TEnum flag) where TEnum : Enum
        {
            long valueAsLong = Convert.ToInt64(value);
            long flagAsLong = Convert.ToInt64(flag);
            return (TEnum)Enum.ToObject(typeof(TEnum), valueAsLong & ~flagAsLong);
        }

        /// <summary>
        /// 切换标志位
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TEnum ToggleFlag<TEnum>(this TEnum value, TEnum flag) where TEnum : Enum
        {
            long valueAsLong = Convert.ToInt64(value);
            long flagAsLong = Convert.ToInt64(flag);
            return (TEnum)Enum.ToObject(typeof(TEnum), valueAsLong ^ flagAsLong);
        }

        /// <summary>
        /// 获取所有设置的标志位
        /// </summary>
        public static IEnumerable<TEnum> GetFlags<TEnum>(this TEnum value) where TEnum : Enum
        {
            foreach (TEnum flag in Enum.GetValues(typeof(TEnum)))
            {
                if (value.HasFlag(flag))
                    yield return flag;
            }
        }

        #endregion

        #region 描述和特性

        /// <summary>
        /// 获取枚举的 Description 特性值
        /// </summary>
        public static string GetDescription<TEnum>(this TEnum value) where TEnum : Enum
        {
            var field = value.GetType().GetField(value.ToString());
            if (field == null)
                return value.ToString();

            var attributes = field.GetCustomAttributes(typeof(DescriptionAttribute), false);
            if (attributes.Length > 0 && attributes[0] is DescriptionAttribute descAttr)
                return descAttr.Description;

            return value.ToString();
        }

        /// <summary>
        /// 获取枚举的自定义特性
        /// </summary>
        public static TAttribute GetAttribute<TEnum, TAttribute>(this TEnum value) 
            where TEnum : Enum 
            where TAttribute : Attribute
        {
            var field = value.GetType().GetField(value.ToString());
            if (field == null)
                return null;

            var attributes = field.GetCustomAttributes(typeof(TAttribute), false);
            return attributes.Length > 0 ? attributes[0] as TAttribute : null;
        }

        #endregion

        #region 集合操作

        /// <summary>
        /// 获取枚举的所有值
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TEnum[] GetValues<TEnum>() where TEnum : Enum
        {
            return (TEnum[])Enum.GetValues(typeof(TEnum));
        }

        /// <summary>
        /// 获取枚举的所有名称
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string[] GetNames<TEnum>() where TEnum : Enum
        {
            return Enum.GetNames(typeof(TEnum));
        }

        /// <summary>
        /// 获取枚举的值数量
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetCount<TEnum>() where TEnum : Enum
        {
            return Enum.GetValues(typeof(TEnum)).Length;
        }

        /// <summary>
        /// 获取枚举的所有值和描述的字典
        /// </summary>
        public static Dictionary<TEnum, string> GetValueDescriptionDictionary<TEnum>() where TEnum : Enum
        {
            var result = new Dictionary<TEnum, string>();
            foreach (TEnum value in Enum.GetValues(typeof(TEnum)))
            {
                result[value] = value.GetDescription();
            }
            return result;
        }

        #endregion

        #region 循环操作

        /// <summary>
        /// 获取下一个枚举值（循环）
        /// </summary>
        public static TEnum Next<TEnum>(this TEnum value) where TEnum : Enum
        {
            var values = GetValues<TEnum>();
            int index = Array.IndexOf(values, value);
            return values[(index + 1) % values.Length];
        }

        /// <summary>
        /// 获取上一个枚举值（循环）
        /// </summary>
        public static TEnum Previous<TEnum>(this TEnum value) where TEnum : Enum
        {
            var values = GetValues<TEnum>();
            int index = Array.IndexOf(values, value);
            return values[(index - 1 + values.Length) % values.Length];
        }

        #endregion

        #region 比较操作

        /// <summary>
        /// 检查枚举值是否在指定范围内
        /// </summary>
        public static bool InRange<TEnum>(this TEnum value, TEnum min, TEnum max) where TEnum : Enum
        {
            long valueAsLong = Convert.ToInt64(value);
            long minAsLong = Convert.ToInt64(min);
            long maxAsLong = Convert.ToInt64(max);
            return valueAsLong >= minAsLong && valueAsLong <= maxAsLong;
        }

        #endregion

        #region 随机操作

        private static readonly Random _random = new Random();

        /// <summary>
        /// 获取随机枚举值
        /// </summary>
        public static TEnum Random<TEnum>() where TEnum : Enum
        {
            var values = GetValues<TEnum>();
            return values[_random.Next(values.Length)];
        }

        /// <summary>
        /// 获取随机枚举值（排除指定值）
        /// </summary>
        public static TEnum RandomExcept<TEnum>(params TEnum[] except) where TEnum : Enum
        {
            var values = GetValues<TEnum>().Where(v => !except.Contains(v)).ToArray();
            if (values.Length == 0)
                throw new InvalidOperationException("No valid enum values available after exclusion.");
            return values[_random.Next(values.Length)];
        }

        #endregion

        #region 转换为集合

        /// <summary>
        /// 转换为 List
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static List<TEnum> ToList<TEnum>() where TEnum : Enum
        {
            return new List<TEnum>(GetValues<TEnum>());
        }

        /// <summary>
        /// 转换为字典（值 -> 名称）
        /// </summary>
        public static Dictionary<TEnum, string> ToDictionary<TEnum>() where TEnum : Enum
        {
            var result = new Dictionary<TEnum, string>();
            foreach (TEnum value in Enum.GetValues(typeof(TEnum)))
            {
                result[value] = value.ToString();
            }
            return result;
        }

        #endregion
    }
}
