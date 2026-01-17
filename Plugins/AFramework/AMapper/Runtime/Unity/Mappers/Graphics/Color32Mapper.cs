// ==========================================================
// 文件名：Color32Mapper.cs
// 命名空间: AFramework.AMapper.Unity
// 依赖: UnityEngine, AFramework.AMapper
// 功能: Color32 类型映射器，支持 Color32 与其他颜色类型的转换
// ==========================================================

using System;
using UnityEngine;

namespace AFramework.AMapper.Unity
{
    /// <summary>
    /// Color32 类型映射器
    /// <para>支持 Color32 与 Color、Vector4、byte[] 等类型的双向转换</para>
    /// <para>Color32 type mapper supporting bidirectional conversion with Color, Vector4, byte[], etc.</para>
    /// </summary>
    /// <remarks>
    /// 设计原则：
    /// <list type="bullet">
    /// <item>单一职责：仅负责 Color32 相关类型的映射</item>
    /// <item>开闭原则：通过 IsMatch 方法扩展支持的类型对</item>
    /// <item>性能优化：使用直接赋值，避免反射和装箱</item>
    /// </list>
    /// 
    /// 支持的映射类型：
    /// <list type="bullet">
    /// <item>Color32 ↔ Color（字节与浮点数转换）</item>
    /// <item>Color32 ↔ Vector4（字节归一化为 0-1 浮点数）</item>
    /// <item>Color32 ↔ byte[]（数组长度为 4，RGBA 顺序）</item>
    /// <item>Color32 ↔ uint（32 位整数，RGBA 打包）</item>
    /// </list>
    /// </remarks>
    public sealed class Color32Mapper : IObjectMapper
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

            // Color32 → 其他类型
            if (sourceType == typeof(Color32))
            {
                return destType == typeof(Color) ||
                       destType == typeof(Vector4) ||
                       destType == typeof(byte[]) ||
                       destType == typeof(uint);
            }

            // 其他类型 → Color32
            if (destType == typeof(Color32))
            {
                return sourceType == typeof(Color) ||
                       sourceType == typeof(Vector4) ||
                       sourceType == typeof(byte[]) ||
                       sourceType == typeof(uint);
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
                return GetDefaultValue(destination?.GetType() ?? typeof(Color32));

            try
            {
                var sourceType = source.GetType();
                var destType = destination?.GetType() ?? typeof(Color32);

                // Color32 → 其他类型
                if (sourceType == typeof(Color32))
                {
                    var color32 = (Color32)source;

                    if (destType == typeof(Color))
                        return (Color)color32;

                    if (destType == typeof(Vector4))
                        return new Vector4(color32.r / 255f, color32.g / 255f, color32.b / 255f, color32.a / 255f);

                    if (destType == typeof(byte[]))
                        return new byte[] { color32.r, color32.g, color32.b, color32.a };

                    if (destType == typeof(uint))
                        return PackToUInt(color32);
                }

                // 其他类型 → Color32
                if (destType == typeof(Color32))
                {
                    if (sourceType == typeof(Color))
                    {
                        var color = (Color)source;
                        return (Color32)color;
                    }

                    if (sourceType == typeof(Vector4))
                    {
                        var vector4 = (Vector4)source;
                        return new Color32(
                            (byte)Mathf.Clamp(vector4.x * 255f, 0, 255),
                            (byte)Mathf.Clamp(vector4.y * 255f, 0, 255),
                            (byte)Mathf.Clamp(vector4.z * 255f, 0, 255),
                            (byte)Mathf.Clamp(vector4.w * 255f, 0, 255));
                    }

                    if (sourceType == typeof(byte[]))
                    {
                        var array = (byte[])source;
                        if (array.Length < 4)
                            throw new MappingException($"数组长度不足，至少需要 4 个元素才能转换为 Color32 / Array length insufficient, at least 4 elements required to convert to Color32");
                        return new Color32(array[0], array[1], array[2], array[3]);
                    }

                    if (sourceType == typeof(uint))
                    {
                        var packed = (uint)source;
                        return UnpackFromUInt(packed);
                    }
                }

                throw new MappingException($"不支持的映射类型对：{sourceType.Name} → {destType.Name} / Unsupported type pair: {sourceType.Name} → {destType.Name}");
            }
            catch (Exception ex) when (!(ex is MappingException))
            {
                throw new MappingException($"Color32 映射失败 / Color32 mapping failed: {ex.Message}", ex);
            }
        }

        #endregion

        #region 私有方法 / Private Methods

        /// <summary>
        /// 将 Color32 打包为 uint（RGBA 格式）
        /// </summary>
        private uint PackToUInt(Color32 color)
        {
            return ((uint)color.r << 24) | ((uint)color.g << 16) | ((uint)color.b << 8) | color.a;
        }

        /// <summary>
        /// 从 uint 解包为 Color32（RGBA 格式）
        /// </summary>
        private Color32 UnpackFromUInt(uint packed)
        {
            return new Color32(
                (byte)((packed >> 24) & 0xFF),
                (byte)((packed >> 16) & 0xFF),
                (byte)((packed >> 8) & 0xFF),
                (byte)(packed & 0xFF));
        }

        /// <summary>
        /// 获取类型的默认值
        /// </summary>
        private object GetDefaultValue(Type type)
        {
            if (type == typeof(Color32))
                return new Color32(255, 255, 255, 255);
            if (type == typeof(Color))
                return Color.white;
            if (type == typeof(Vector4))
                return new Vector4(1, 1, 1, 1);
            if (type == typeof(byte[]))
                return new byte[] { 255, 255, 255, 255 };
            if (type == typeof(uint))
                return 0xFFFFFFFFu;

            return null;
        }

        #endregion
    }
}
