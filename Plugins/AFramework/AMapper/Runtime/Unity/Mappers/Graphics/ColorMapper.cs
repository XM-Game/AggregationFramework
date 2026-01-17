// ==========================================================
// 文件名：ColorMapper.cs
// 命名空间: AFramework.AMapper.Unity
// 依赖: UnityEngine, AFramework.AMapper
// 功能: Color 类型映射器，支持 Color 与其他颜色类型的转换
// ==========================================================

using System;
using UnityEngine;

namespace AFramework.AMapper.Unity
{
    /// <summary>
    /// Color 类型映射器
    /// <para>支持 Color 与 Color32、Vector4、string（十六进制）等类型的双向转换</para>
    /// <para>Color type mapper supporting bidirectional conversion with Color32, Vector4, string (hex), etc.</para>
    /// </summary>
    /// <remarks>
    /// 设计原则：
    /// <list type="bullet">
    /// <item>单一职责：仅负责 Color 相关类型的映射</item>
    /// <item>开闭原则：通过 IsMatch 方法扩展支持的类型对</item>
    /// <item>性能优化：使用直接赋值，避免反射和装箱</item>
    /// </list>
    /// 
    /// 支持的映射类型：
    /// <list type="bullet">
    /// <item>Color ↔ Color32（浮点数与字节转换）</item>
    /// <item>Color ↔ Vector4（RGBA 分量对应）</item>
    /// <item>Color ↔ string（十六进制格式，如 "#RRGGBBAA"）</item>
    /// <item>Color ↔ float[]（数组长度为 4，RGBA 顺序）</item>
    /// </list>
    /// 
    /// 十六进制字符串格式：
    /// <list type="bullet">
    /// <item>#RGB（3 位，自动扩展为 #RRGGBB）</item>
    /// <item>#RRGGBB（6 位，Alpha 默认为 FF）</item>
    /// <item>#RRGGBBAA（8 位，完整 RGBA）</item>
    /// </list>
    /// </remarks>
    public sealed class ColorMapper : IObjectMapper
    {
        #region IObjectMapper 实现 / IObjectMapper Implementation

        /// <summary>
        /// 生成映射表达式
        /// <para>Generate mapping expression</para>
        /// </summary>
        public System.Linq.Expressions.Expression MapExpression(
            IMapperConfiguration configuration,
            TypePair typePair,
            System.Linq.Expressions.Expression sourceExpression,
            System.Linq.Expressions.Expression destinationExpression,
            System.Linq.Expressions.Expression contextExpression)
        {
            var mapMethod = GetType().GetMethod(nameof(Map));
            var sourceParam = System.Linq.Expressions.Expression.Convert(sourceExpression, typeof(object));
            System.Linq.Expressions.Expression destParam = destinationExpression != null 
                ? System.Linq.Expressions.Expression.Convert(destinationExpression, typeof(object))
                : System.Linq.Expressions.Expression.Constant(null, typeof(object));
            
            var mapCall = System.Linq.Expressions.Expression.Call(
                System.Linq.Expressions.Expression.Constant(this),
                mapMethod,
                sourceParam,
                destParam,
                contextExpression);
            
            return System.Linq.Expressions.Expression.Convert(mapCall, typePair.DestinationType);
        }

        /// <summary>
        /// 判断是否匹配指定的类型对
        /// <para>Check if the mapper matches the specified type pair</para>
        /// </summary>
        /// <param name="typePair">类型对 / Type pair</param>
        /// <returns>是否匹配 / Whether matches</returns>
        public bool IsMatch(TypePair typePair)
        {
            if (typePair == null)
                return false;

            var sourceType = typePair.SourceType;
            var destType = typePair.DestinationType;

            // Color → 其他类型
            if (sourceType == typeof(Color))
            {
                return destType == typeof(Color32) ||
                       destType == typeof(Vector4) ||
                       destType == typeof(string) ||
                       destType == typeof(float[]);
            }

            // 其他类型 → Color
            if (destType == typeof(Color))
            {
                return sourceType == typeof(Color32) ||
                       sourceType == typeof(Vector4) ||
                       sourceType == typeof(string) ||
                       sourceType == typeof(float[]);
            }

            return false;
        }

        /// <summary>
        /// 执行映射
        /// <para>Execute mapping</para>
        /// </summary>
        /// <param name="source">源对象 / Source object</param>
        /// <param name="destination">目标对象 / Destination object</param>
        /// <param name="context">解析上下文 / Resolution context</param>
        /// <returns>映射后的对象 / Mapped object</returns>
        /// <exception cref="MappingException">当映射失败时抛出 / Thrown when mapping fails</exception>
        public object Map(object source, object destination, ResolutionContext context)
        {
            if (source == null)
                return GetDefaultValue(destination?.GetType() ?? typeof(Color));

            try
            {
                var sourceType = source.GetType();
                var destType = destination?.GetType() ?? typeof(Color);

                // Color → 其他类型
                if (sourceType == typeof(Color))
                {
                    var color = (Color)source;

                    if (destType == typeof(Color32))
                        return (Color32)color;

                    if (destType == typeof(Vector4))
                        return new Vector4(color.r, color.g, color.b, color.a);

                    if (destType == typeof(string))
                        return ColorToHex(color);

                    if (destType == typeof(float[]))
                        return new float[] { color.r, color.g, color.b, color.a };
                }

                // 其他类型 → Color
                if (destType == typeof(Color))
                {
                    if (sourceType == typeof(Color32))
                    {
                        var color32 = (Color32)source;
                        return (Color)color32;
                    }

                    if (sourceType == typeof(Vector4))
                    {
                        var vector4 = (Vector4)source;
                        return new Color(vector4.x, vector4.y, vector4.z, vector4.w);
                    }

                    if (sourceType == typeof(string))
                    {
                        var hexString = (string)source;
                        if (!TryParseHexColor(hexString, out Color color))
                            throw new MappingException($"无效的十六进制颜色字符串：{hexString} / Invalid hex color string: {hexString}");
                        return color;
                    }

                    if (sourceType == typeof(float[]))
                    {
                        var array = (float[])source;
                        if (array.Length < 4)
                            throw new MappingException($"数组长度不足，至少需要 4 个元素才能转换为 Color / Array length insufficient, at least 4 elements required to convert to Color");
                        return new Color(array[0], array[1], array[2], array[3]);
                    }
                }

                throw new MappingException($"不支持的映射类型对：{sourceType.Name} → {destType.Name} / Unsupported type pair: {sourceType.Name} → {destType.Name}");
            }
            catch (Exception ex) when (!(ex is MappingException))
            {
                throw new MappingException($"Color 映射失败 / Color mapping failed: {ex.Message}", ex);
            }
        }

        #endregion

        #region 私有方法 / Private Methods

        /// <summary>
        /// 将 Color 转换为十六进制字符串
        /// </summary>
        private string ColorToHex(Color color)
        {
            Color32 color32 = color;
            return $"#{color32.r:X2}{color32.g:X2}{color32.b:X2}{color32.a:X2}";
        }

        /// <summary>
        /// 尝试解析十六进制颜色字符串
        /// </summary>
        private bool TryParseHexColor(string hex, out Color color)
        {
            color = Color.white;

            if (string.IsNullOrEmpty(hex))
                return false;

            // 移除 # 前缀
            hex = hex.TrimStart('#');

            // 支持 RGB、RRGGBB、RRGGBBAA 格式
            if (hex.Length == 3)
            {
                // RGB → RRGGBB
                hex = $"{hex[0]}{hex[0]}{hex[1]}{hex[1]}{hex[2]}{hex[2]}";
            }

            if (hex.Length == 6)
            {
                // RRGGBB → RRGGBBAA（Alpha 默认为 FF）
                hex += "FF";
            }

            if (hex.Length != 8)
                return false;

            try
            {
                byte r = Convert.ToByte(hex.Substring(0, 2), 16);
                byte g = Convert.ToByte(hex.Substring(2, 2), 16);
                byte b = Convert.ToByte(hex.Substring(4, 2), 16);
                byte a = Convert.ToByte(hex.Substring(6, 2), 16);

                color = new Color32(r, g, b, a);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 获取类型的默认值
        /// </summary>
        private object GetDefaultValue(Type type)
        {
            if (type == typeof(Color))
                return Color.white;
            if (type == typeof(Color32))
                return new Color32(255, 255, 255, 255);
            if (type == typeof(Vector4))
                return new Vector4(1, 1, 1, 1);
            if (type == typeof(string))
                return "#FFFFFFFF";
            if (type == typeof(float[]))
                return new float[] { 1, 1, 1, 1 };

            return null;
        }

        #endregion
    }
}
