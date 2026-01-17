// ==========================================================
// 文件名：Vector4Mapper.cs
// 命名空间: AFramework.AMapper.Unity
// 依赖: UnityEngine, AFramework.AMapper
// 功能: Vector4 类型映射器，支持 Vector4 与其他向量类型的转换
// ==========================================================

using System;
using UnityEngine;

namespace AFramework.AMapper.Unity
{
    /// <summary>
    /// Vector4 类型映射器
    /// <para>支持 Vector4 与 Vector2、Vector3、Color 等类型的双向转换</para>
    /// <para>Vector4 type mapper supporting bidirectional conversion with Vector2, Vector3, Color, etc.</para>
    /// </summary>
    /// <remarks>
    /// 设计原则：
    /// <list type="bullet">
    /// <item>单一职责：仅负责 Vector4 相关类型的映射</item>
    /// <item>开闭原则：通过 IsMatch 方法扩展支持的类型对</item>
    /// <item>性能优化：使用直接赋值，避免反射和装箱</item>
    /// </list>
    /// 
    /// 支持的映射类型：
    /// <list type="bullet">
    /// <item>Vector4 ↔ Vector2（丢弃或补充 z、w 分量）</item>
    /// <item>Vector4 ↔ Vector3（丢弃或补充 w 分量）</item>
    /// <item>Vector4 ↔ Color（RGBA 分量对应）</item>
    /// <item>Vector4 ↔ Quaternion（xyzw 分量对应）</item>
    /// <item>Vector4 ↔ float[]（数组长度至少为 4）</item>
    /// </list>
    /// </remarks>
    public sealed class Vector4Mapper : IObjectMapper
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

            // Vector4 → 其他类型
            if (sourceType == typeof(Vector4))
            {
                return destType == typeof(Vector2) ||
                       destType == typeof(Vector3) ||
                       destType == typeof(Color) ||
                       destType == typeof(Quaternion) ||
                       destType == typeof(float[]);
            }

            // 其他类型 → Vector4
            if (destType == typeof(Vector4))
            {
                return sourceType == typeof(Vector2) ||
                       sourceType == typeof(Vector3) ||
                       sourceType == typeof(Color) ||
                       sourceType == typeof(Quaternion) ||
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
                return GetDefaultValue(destination?.GetType() ?? typeof(Vector4));

            try
            {
                var sourceType = source.GetType();
                var destType = destination?.GetType() ?? typeof(Vector4);

                // Vector4 → 其他类型
                if (sourceType == typeof(Vector4))
                {
                    var vector4 = (Vector4)source;

                    if (destType == typeof(Vector2))
                        return new Vector2(vector4.x, vector4.y);

                    if (destType == typeof(Vector3))
                        return new Vector3(vector4.x, vector4.y, vector4.z);

                    if (destType == typeof(Color))
                        return new Color(vector4.x, vector4.y, vector4.z, vector4.w);

                    if (destType == typeof(Quaternion))
                        return new Quaternion(vector4.x, vector4.y, vector4.z, vector4.w);

                    if (destType == typeof(float[]))
                        return new float[] { vector4.x, vector4.y, vector4.z, vector4.w };
                }

                // 其他类型 → Vector4
                if (destType == typeof(Vector4))
                {
                    if (sourceType == typeof(Vector2))
                    {
                        var vector2 = (Vector2)source;
                        return new Vector4(vector2.x, vector2.y, 0f, 0f);
                    }

                    if (sourceType == typeof(Vector3))
                    {
                        var vector3 = (Vector3)source;
                        return new Vector4(vector3.x, vector3.y, vector3.z, 0f);
                    }

                    if (sourceType == typeof(Color))
                    {
                        var color = (Color)source;
                        return new Vector4(color.r, color.g, color.b, color.a);
                    }

                    if (sourceType == typeof(Quaternion))
                    {
                        var quaternion = (Quaternion)source;
                        return new Vector4(quaternion.x, quaternion.y, quaternion.z, quaternion.w);
                    }

                    if (sourceType == typeof(float[]))
                    {
                        var array = (float[])source;
                        if (array.Length < 4)
                            throw new MappingException($"数组长度不足，至少需要 4 个元素才能转换为 Vector4 / Array length insufficient, at least 4 elements required to convert to Vector4");
                        return new Vector4(array[0], array[1], array[2], array[3]);
                    }
                }

                throw new MappingException($"不支持的映射类型对：{sourceType.Name} → {destType.Name} / Unsupported type pair: {sourceType.Name} → {destType.Name}");
            }
            catch (Exception ex) when (!(ex is MappingException))
            {
                throw new MappingException($"Vector4 映射失败 / Vector4 mapping failed: {ex.Message}", ex);
            }
        }

        #endregion

        #region 私有方法 / Private Methods

        /// <summary>
        /// 获取类型的默认值
        /// </summary>
        private object GetDefaultValue(Type type)
        {
            if (type == typeof(Vector2))
                return Vector2.zero;
            if (type == typeof(Vector3))
                return Vector3.zero;
            if (type == typeof(Vector4))
                return Vector4.zero;
            if (type == typeof(Color))
                return Color.clear;
            if (type == typeof(Quaternion))
                return Quaternion.identity;
            if (type == typeof(float[]))
                return new float[4];

            return null;
        }

        #endregion
    }
}
