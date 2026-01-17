// ==========================================================
// 文件名：RectMapper.cs
// 命名空间: AFramework.AMapper.Unity
// 依赖: UnityEngine, AFramework.AMapper
// 功能: Rect 类型映射器，支持 Rect 与 RectInt、Vector4 的转换
// ==========================================================

using System;
using UnityEngine;

namespace AFramework.AMapper.Unity
{
    /// <summary>
    /// Rect 类型映射器
    /// <para>支持 Rect 与 RectInt、Vector4、float[] 等类型的双向转换</para>
    /// <para>Rect type mapper supporting bidirectional conversion with RectInt, Vector4, float[], etc.</para>
    /// </summary>
    /// <remarks>
    /// 设计原则：
    /// <list type="bullet">
    /// <item>单一职责：仅负责 Rect 相关类型的映射</item>
    /// <item>开闭原则：通过 IsMatch 方法扩展支持的类型对</item>
    /// <item>性能优化：使用直接赋值，避免反射和装箱</item>
    /// </list>
    /// 
    /// 支持的映射类型：
    /// <list type="bullet">
    /// <item>Rect ↔ RectInt（浮点数与整数转换）</item>
    /// <item>Rect ↔ Vector4（x, y, width, height 对应）</item>
    /// <item>Rect ↔ float[]（数组长度为 4，x, y, width, height 顺序）</item>
    /// </list>
    /// </remarks>
    public sealed class RectMapper : IObjectMapper
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

            // Rect → 其他类型
            if (sourceType == typeof(Rect))
            {
                return destType == typeof(RectInt) ||
                       destType == typeof(Vector4) ||
                       destType == typeof(float[]);
            }

            // 其他类型 → Rect
            if (destType == typeof(Rect))
            {
                return sourceType == typeof(RectInt) ||
                       sourceType == typeof(Vector4) ||
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
                return GetDefaultValue(destination?.GetType() ?? typeof(Rect));

            try
            {
                var sourceType = source.GetType();
                var destType = destination?.GetType() ?? typeof(Rect);

                // Rect → 其他类型
                if (sourceType == typeof(Rect))
                {
                    var rect = (Rect)source;

                    if (destType == typeof(RectInt))
                        return new RectInt(
                            Mathf.RoundToInt(rect.x),
                            Mathf.RoundToInt(rect.y),
                            Mathf.RoundToInt(rect.width),
                            Mathf.RoundToInt(rect.height));

                    if (destType == typeof(Vector4))
                        return new Vector4(rect.x, rect.y, rect.width, rect.height);

                    if (destType == typeof(float[]))
                        return new float[] { rect.x, rect.y, rect.width, rect.height };
                }

                // 其他类型 → Rect
                if (destType == typeof(Rect))
                {
                    if (sourceType == typeof(RectInt))
                    {
                        var rectInt = (RectInt)source;
                        return new Rect(rectInt.x, rectInt.y, rectInt.width, rectInt.height);
                    }

                    if (sourceType == typeof(Vector4))
                    {
                        var vector4 = (Vector4)source;
                        return new Rect(vector4.x, vector4.y, vector4.z, vector4.w);
                    }

                    if (sourceType == typeof(float[]))
                    {
                        var array = (float[])source;
                        if (array.Length < 4)
                            throw new MappingException($"数组长度不足，至少需要 4 个元素才能转换为 Rect / Array length insufficient, at least 4 elements required to convert to Rect");
                        return new Rect(array[0], array[1], array[2], array[3]);
                    }
                }

                throw new MappingException($"不支持的映射类型对：{sourceType.Name} → {destType.Name} / Unsupported type pair: {sourceType.Name} → {destType.Name}");
            }
            catch (Exception ex) when (!(ex is MappingException))
            {
                throw new MappingException($"Rect 映射失败 / Rect mapping failed: {ex.Message}", ex);
            }
        }

        #endregion

        #region 私有方法 / Private Methods

        /// <summary>
        /// 获取类型的默认值
        /// </summary>
        private object GetDefaultValue(Type type)
        {
            if (type == typeof(Rect))
                return new Rect(0, 0, 0, 0);
            if (type == typeof(RectInt))
                return new RectInt(0, 0, 0, 0);
            if (type == typeof(Vector4))
                return Vector4.zero;
            if (type == typeof(float[]))
                return new float[4];

            return null;
        }

        #endregion
    }
}
