// ==========================================================
// 文件名：Matrix4x4Mapper.cs
// 命名空间: AFramework.AMapper.Unity
// 依赖: UnityEngine, AFramework.AMapper
// 功能: Matrix4x4 类型映射器，支持 Matrix4x4 与数组的转换
// ==========================================================

using System;
using UnityEngine;

namespace AFramework.AMapper.Unity
{
    /// <summary>
    /// Matrix4x4 类型映射器
    /// <para>支持 Matrix4x4 与 float[] 的双向转换</para>
    /// <para>Matrix4x4 type mapper supporting bidirectional conversion with float[]</para>
    /// </summary>
    /// <remarks>
    /// 设计原则：
    /// <list type="bullet">
    /// <item>单一职责：仅负责 Matrix4x4 相关类型的映射</item>
    /// <item>开闭原则：通过 IsMatch 方法扩展支持的类型对</item>
    /// <item>性能优化：使用直接赋值，避免反射和装箱</item>
    /// </list>
    /// 
    /// 支持的映射类型：
    /// <list type="bullet">
    /// <item>Matrix4x4 ↔ float[]（数组长度为 16，按行主序存储）</item>
    /// </list>
    /// 
    /// 数组布局（行主序）：
    /// <code>
    /// [m00, m01, m02, m03,
    ///  m10, m11, m12, m13,
    ///  m20, m21, m22, m23,
    ///  m30, m31, m32, m33]
    /// </code>
    /// </remarks>
    public sealed class Matrix4x4Mapper : IObjectMapper
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

            // Matrix4x4 ↔ float[]
            return (sourceType == typeof(Matrix4x4) && destType == typeof(float[])) ||
                   (sourceType == typeof(float[]) && destType == typeof(Matrix4x4));
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
                return GetDefaultValue(destination?.GetType() ?? typeof(Matrix4x4));

            try
            {
                var sourceType = source.GetType();
                var destType = destination?.GetType() ?? typeof(Matrix4x4);

                // Matrix4x4 → float[]
                if (sourceType == typeof(Matrix4x4) && destType == typeof(float[]))
                {
                    var matrix = (Matrix4x4)source;
                    return new float[]
                    {
                        matrix.m00, matrix.m01, matrix.m02, matrix.m03,
                        matrix.m10, matrix.m11, matrix.m12, matrix.m13,
                        matrix.m20, matrix.m21, matrix.m22, matrix.m23,
                        matrix.m30, matrix.m31, matrix.m32, matrix.m33
                    };
                }

                // float[] → Matrix4x4
                if (sourceType == typeof(float[]) && destType == typeof(Matrix4x4))
                {
                    var array = (float[])source;
                    if (array.Length < 16)
                        throw new MappingException($"数组长度不足，需要 16 个元素才能转换为 Matrix4x4 / Array length insufficient, 16 elements required to convert to Matrix4x4");

                    var matrix = new Matrix4x4();
                    matrix.m00 = array[0];  matrix.m01 = array[1];  matrix.m02 = array[2];  matrix.m03 = array[3];
                    matrix.m10 = array[4];  matrix.m11 = array[5];  matrix.m12 = array[6];  matrix.m13 = array[7];
                    matrix.m20 = array[8];  matrix.m21 = array[9];  matrix.m22 = array[10]; matrix.m23 = array[11];
                    matrix.m30 = array[12]; matrix.m31 = array[13]; matrix.m32 = array[14]; matrix.m33 = array[15];
                    return matrix;
                }

                throw new MappingException($"不支持的映射类型对：{sourceType.Name} → {destType.Name} / Unsupported type pair: {sourceType.Name} → {destType.Name}");
            }
            catch (Exception ex) when (!(ex is MappingException))
            {
                throw new MappingException($"Matrix4x4 映射失败 / Matrix4x4 mapping failed: {ex.Message}", ex);
            }
        }

        #endregion

        #region 私有方法 / Private Methods

        /// <summary>
        /// 获取类型的默认值
        /// </summary>
        private object GetDefaultValue(Type type)
        {
            if (type == typeof(Matrix4x4))
                return Matrix4x4.identity;
            if (type == typeof(float[]))
            {
                // 返回单位矩阵的数组表示
                return new float[]
                {
                    1, 0, 0, 0,
                    0, 1, 0, 0,
                    0, 0, 1, 0,
                    0, 0, 0, 1
                };
            }

            return null;
        }

        #endregion
    }
}
