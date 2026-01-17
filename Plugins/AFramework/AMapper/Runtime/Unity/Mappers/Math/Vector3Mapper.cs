// ==========================================================
// 文件名：Vector3Mapper.cs
// 命名空间: AFramework.AMapper.Unity
// 依赖: UnityEngine, AFramework.AMapper
// 功能: Vector3 类型映射器，支持 Vector3 与其他向量类型的转换
// ==========================================================

using System;
using UnityEngine;

namespace AFramework.AMapper.Unity
{
    /// <summary>
    /// Vector3 类型映射器
    /// <para>支持 Vector3 与 Vector2、Vector4、Vector3Int 等类型的双向转换</para>
    /// <para>Vector3 type mapper supporting bidirectional conversion with Vector2, Vector4, Vector3Int, etc.</para>
    /// </summary>
    /// <remarks>
    /// 设计原则：
    /// <list type="bullet">
    /// <item>单一职责：仅负责 Vector3 相关类型的映射</item>
    /// <item>开闭原则：通过 IsMatch 方法扩展支持的类型对</item>
    /// <item>性能优化：使用直接赋值，避免反射和装箱</item>
    /// </list>
    /// 
    /// 支持的映射类型：
    /// <list type="bullet">
    /// <item>Vector3 ↔ Vector2（丢弃或补充 z 分量）</item>
    /// <item>Vector3 ↔ Vector4（w 分量默认为 0）</item>
    /// <item>Vector3 ↔ Vector3Int（浮点数与整数转换）</item>
    /// <item>Vector3 ↔ float[]（数组长度至少为 3）</item>
    /// </list>
    /// </remarks>
    public sealed class Vector3Mapper : IObjectMapper
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

            // Vector3 → 其他类型
            if (sourceType == typeof(Vector3))
            {
                return destType == typeof(Vector2) ||
                       destType == typeof(Vector4) ||
                       destType == typeof(Vector3Int) ||
                       destType == typeof(float[]);
            }

            // 其他类型 → Vector3
            if (destType == typeof(Vector3))
            {
                return sourceType == typeof(Vector2) ||
                       sourceType == typeof(Vector4) ||
                       sourceType == typeof(Vector3Int) ||
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
                return GetDefaultValue(destination?.GetType() ?? typeof(Vector3));

            try
            {
                var sourceType = source.GetType();
                var destType = destination?.GetType() ?? typeof(Vector3);

                // Vector3 → 其他类型
                if (sourceType == typeof(Vector3))
                {
                    var vector3 = (Vector3)source;

                    if (destType == typeof(Vector2))
                        return new Vector2(vector3.x, vector3.y);

                    if (destType == typeof(Vector4))
                        return new Vector4(vector3.x, vector3.y, vector3.z, 0f);

                    if (destType == typeof(Vector3Int))
                        return new Vector3Int(
                            Mathf.RoundToInt(vector3.x),
                            Mathf.RoundToInt(vector3.y),
                            Mathf.RoundToInt(vector3.z));

                    if (destType == typeof(float[]))
                        return new float[] { vector3.x, vector3.y, vector3.z };
                }

                // 其他类型 → Vector3
                if (destType == typeof(Vector3))
                {
                    if (sourceType == typeof(Vector2))
                    {
                        var vector2 = (Vector2)source;
                        return new Vector3(vector2.x, vector2.y, 0f);
                    }

                    if (sourceType == typeof(Vector4))
                    {
                        var vector4 = (Vector4)source;
                        return new Vector3(vector4.x, vector4.y, vector4.z);
                    }

                    if (sourceType == typeof(Vector3Int))
                    {
                        var vector3Int = (Vector3Int)source;
                        return new Vector3(vector3Int.x, vector3Int.y, vector3Int.z);
                    }

                    if (sourceType == typeof(float[]))
                    {
                        var array = (float[])source;
                        if (array.Length < 3)
                            throw new MappingException($"数组长度不足，至少需要 3 个元素才能转换为 Vector3 / Array length insufficient, at least 3 elements required to convert to Vector3");
                        return new Vector3(array[0], array[1], array[2]);
                    }
                }

                throw new MappingException($"不支持的映射类型对：{sourceType.Name} → {destType.Name} / Unsupported type pair: {sourceType.Name} → {destType.Name}");
            }
            catch (Exception ex) when (!(ex is MappingException))
            {
                throw new MappingException($"Vector3 映射失败 / Vector3 mapping failed: {ex.Message}", ex);
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
            if (type == typeof(Vector3Int))
                return Vector3Int.zero;
            if (type == typeof(float[]))
                return new float[3];

            return null;
        }

        #endregion
    }
}
