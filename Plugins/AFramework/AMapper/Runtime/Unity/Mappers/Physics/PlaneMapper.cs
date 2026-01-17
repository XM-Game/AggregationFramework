// ==========================================================
// 文件名：PlaneMapper.cs
// 命名空间: AFramework.AMapper.Unity
// 依赖: UnityEngine, AFramework.AMapper
// 功能: Plane 类型映射器，支持 Plane 与数组的转换
// ==========================================================

using System;
using UnityEngine;

namespace AFramework.AMapper.Unity
{
    /// <summary>
    /// Plane 类型映射器
    /// <para>支持 Plane 与 float[]、Vector4 的双向转换</para>
    /// <para>Plane type mapper supporting bidirectional conversion with float[], Vector4</para>
    /// </summary>
    /// <remarks>
    /// 设计原则：
    /// <list type="bullet">
    /// <item>单一职责：仅负责 Plane 相关类型的映射</item>
    /// <item>开闭原则：通过 IsMatch 方法扩展支持的类型对</item>
    /// <item>性能优化：使用直接赋值，避免反射和装箱</item>
    /// </list>
    /// 
    /// 支持的映射类型：
    /// <list type="bullet">
    /// <item>Plane ↔ float[]（数组长度为 4，normal.xyz + distance）</item>
    /// <item>Plane ↔ Vector4（normal.xyz + distance）</item>
    /// </list>
    /// 
    /// 平面方程：ax + by + cz + d = 0
    /// <list type="bullet">
    /// <item>normal = (a, b, c)</item>
    /// <item>distance = d</item>
    /// </list>
    /// </remarks>
    public sealed class PlaneMapper : IObjectMapper
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
            System.Linq.Expressions.Expression destParam;
            if (destinationExpression != null)
            {
                destParam = System.Linq.Expressions.Expression.Convert(destinationExpression, typeof(object));
            }
            else
            {
                destParam = System.Linq.Expressions.Expression.Constant(null, typeof(object));
            }
            
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

            // Plane → 其他类型
            if (sourceType == typeof(Plane))
            {
                return destType == typeof(float[]) ||
                       destType == typeof(Vector4);
            }

            // 其他类型 → Plane
            if (destType == typeof(Plane))
            {
                return sourceType == typeof(float[]) ||
                       sourceType == typeof(Vector4);
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
                return GetDefaultValue(typeof(Plane));

            try
            {
                var sourceType = source.GetType();
                var destType = destination?.GetType();

                // Plane → 其他类型
                if (sourceType == typeof(Plane))
                {
                    var plane = (Plane)source;

                    if (destType == typeof(float[]) || destType == null)
                        return new float[]
                        {
                            plane.normal.x,
                            plane.normal.y,
                            plane.normal.z,
                            plane.distance
                        };

                    if (destType == typeof(Vector4))
                        return new Vector4(
                            plane.normal.x,
                            plane.normal.y,
                            plane.normal.z,
                            plane.distance);
                }

                // 其他类型 → Plane
                if (destType == typeof(Plane) || destType == null)
                {
                    if (sourceType == typeof(float[]))
                    {
                        var array = (float[])source;
                        if (array.Length < 4)
                            throw new MappingException($"数组长度不足，至少需要 4 个元素才能转换为 Plane / Array length insufficient, at least 4 elements required to convert to Plane");
                        return new Plane(
                            new Vector3(array[0], array[1], array[2]),
                            array[3]);
                    }

                    if (sourceType == typeof(Vector4))
                    {
                        var vector4 = (Vector4)source;
                        return new Plane(
                            new Vector3(vector4.x, vector4.y, vector4.z),
                            vector4.w);
                    }
                }

                throw new MappingException($"不支持的映射类型对：{sourceType.Name} → {destType?.Name ?? "unknown"} / Unsupported type pair: {sourceType.Name} → {destType?.Name ?? "unknown"}");
            }
            catch (Exception ex) when (!(ex is MappingException))
            {
                throw new MappingException($"Plane 映射失败 / Plane mapping failed: {ex.Message}", ex);
            }
        }

        #endregion

        #region 私有方法 / Private Methods

        /// <summary>
        /// 获取类型的默认值
        /// </summary>
        private object GetDefaultValue(Type type)
        {
            if (type == typeof(Plane))
                return new Plane(Vector3.up, 0f);
            if (type == typeof(float[]))
                return new float[] { 0, 1, 0, 0 };
            if (type == typeof(Vector4))
                return new Vector4(0, 1, 0, 0);

            return null;
        }

        #endregion
    }
}
