// ==========================================================
// 文件名：RayMapper.cs
// 命名空间: AFramework.AMapper.Unity
// 依赖: UnityEngine, AFramework.AMapper
// 功能: Ray 类型映射器，支持 Ray 与其他类型的转换
// ==========================================================

using System;
using UnityEngine;

namespace AFramework.AMapper.Unity
{
    /// <summary>
    /// Ray 类型映射器
    /// <para>支持 Ray 与 Ray2D、float[] 等类型的双向转换</para>
    /// <para>Ray type mapper supporting bidirectional conversion with Ray2D, float[], etc.</para>
    /// </summary>
    /// <remarks>
    /// 设计原则：
    /// <list type="bullet">
    /// <item>单一职责：仅负责 Ray 相关类型的映射</item>
    /// <item>开闭原则：通过 IsMatch 方法扩展支持的类型对</item>
    /// <item>性能优化：使用直接赋值，避免反射和装箱</item>
    /// </list>
    /// 
    /// 支持的映射类型：
    /// <list type="bullet">
    /// <item>Ray ↔ Ray2D（3D 与 2D 射线转换，丢弃或补充 z 分量）</item>
    /// <item>Ray ↔ float[]（数组长度为 6，origin.xyz + direction.xyz）</item>
    /// </list>
    /// </remarks>
    public sealed class RayMapper : IObjectMapper
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

            // Ray → 其他类型
            if (sourceType == typeof(Ray))
            {
                return destType == typeof(Ray2D) ||
                       destType == typeof(float[]);
            }

            // 其他类型 → Ray
            if (destType == typeof(Ray))
            {
                return sourceType == typeof(Ray2D) ||
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
                return GetDefaultValue(typeof(Ray));

            try
            {
                var sourceType = source.GetType();
                var destType = destination?.GetType();

                // Ray → 其他类型
                if (sourceType == typeof(Ray))
                {
                    var ray = (Ray)source;

                    if (destType == typeof(Ray2D) || destType == null)
                        return new Ray2D(
                            new Vector2(ray.origin.x, ray.origin.y),
                            new Vector2(ray.direction.x, ray.direction.y));

                    if (destType == typeof(float[]))
                        return new float[]
                        {
                            ray.origin.x, ray.origin.y, ray.origin.z,
                            ray.direction.x, ray.direction.y, ray.direction.z
                        };
                }

                // 其他类型 → Ray
                if (destType == typeof(Ray) || destType == null)
                {
                    if (sourceType == typeof(Ray2D))
                    {
                        var ray2D = (Ray2D)source;
                        return new Ray(
                            new Vector3(ray2D.origin.x, ray2D.origin.y, 0f),
                            new Vector3(ray2D.direction.x, ray2D.direction.y, 0f));
                    }

                    if (sourceType == typeof(float[]))
                    {
                        var array = (float[])source;
                        if (array.Length < 6)
                            throw new MappingException($"数组长度不足，至少需要 6 个元素才能转换为 Ray / Array length insufficient, at least 6 elements required to convert to Ray");
                        return new Ray(
                            new Vector3(array[0], array[1], array[2]),
                            new Vector3(array[3], array[4], array[5]));
                    }
                }

                throw new MappingException($"不支持的映射类型对：{sourceType.Name} → {destType?.Name ?? "unknown"} / Unsupported type pair: {sourceType.Name} → {destType?.Name ?? "unknown"}");
            }
            catch (Exception ex) when (!(ex is MappingException))
            {
                throw new MappingException($"Ray 映射失败 / Ray mapping failed: {ex.Message}", ex);
            }
        }

        #endregion

        #region 私有方法 / Private Methods

        /// <summary>
        /// 获取类型的默认值
        /// </summary>
        private object GetDefaultValue(Type type)
        {
            if (type == typeof(Ray))
                return new Ray(Vector3.zero, Vector3.forward);
            if (type == typeof(Ray2D))
                return new Ray2D(Vector2.zero, Vector2.right);
            if (type == typeof(float[]))
                return new float[6];

            return null;
        }

        #endregion
    }
}
