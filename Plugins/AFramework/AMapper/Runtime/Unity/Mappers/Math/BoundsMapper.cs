// ==========================================================
// 文件名：BoundsMapper.cs
// 命名空间: AFramework.AMapper.Unity
// 依赖: UnityEngine, AFramework.AMapper
// 功能: Bounds 类型映射器，支持 Bounds 与 BoundsInt 的转换
// ==========================================================

using System;
using UnityEngine;

namespace AFramework.AMapper.Unity
{
    /// <summary>
    /// Bounds 类型映射器
    /// <para>支持 Bounds 与 BoundsInt 的双向转换</para>
    /// <para>Bounds type mapper supporting bidirectional conversion with BoundsInt</para>
    /// </summary>
    /// <remarks>
    /// 设计原则：
    /// <list type="bullet">
    /// <item>单一职责：仅负责 Bounds 相关类型的映射</item>
    /// <item>开闭原则：通过 IsMatch 方法扩展支持的类型对</item>
    /// <item>性能优化：使用直接赋值，避免反射和装箱</item>
    /// </list>
    /// 
    /// 支持的映射类型：
    /// <list type="bullet">
    /// <item>Bounds ↔ BoundsInt（浮点数与整数转换）</item>
    /// </list>
    /// 
    /// 转换规则：
    /// <list type="bullet">
    /// <item>Bounds → BoundsInt：center 和 size 四舍五入到整数</item>
    /// <item>BoundsInt → Bounds：position 和 size 直接转换为浮点数</item>
    /// </list>
    /// </remarks>
    public sealed class BoundsMapper : IObjectMapper
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

            // Bounds ↔ BoundsInt
            return (sourceType == typeof(Bounds) && destType == typeof(BoundsInt)) ||
                   (sourceType == typeof(BoundsInt) && destType == typeof(Bounds));
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
                return GetDefaultValue(destination?.GetType() ?? typeof(Bounds));

            try
            {
                var sourceType = source.GetType();
                var destType = destination?.GetType() ?? typeof(Bounds);

                // Bounds → BoundsInt
                if (sourceType == typeof(Bounds) && destType == typeof(BoundsInt))
                {
                    var bounds = (Bounds)source;
                    var center = new Vector3Int(
                        Mathf.RoundToInt(bounds.center.x),
                        Mathf.RoundToInt(bounds.center.y),
                        Mathf.RoundToInt(bounds.center.z));
                    var size = new Vector3Int(
                        Mathf.RoundToInt(bounds.size.x),
                        Mathf.RoundToInt(bounds.size.y),
                        Mathf.RoundToInt(bounds.size.z));
                    
                    // BoundsInt 使用 position 和 size，需要从 center 计算 position
                    var position = center - size / 2;
                    return new BoundsInt(position, size);
                }

                // BoundsInt → Bounds
                if (sourceType == typeof(BoundsInt) && destType == typeof(Bounds))
                {
                    var boundsInt = (BoundsInt)source;
                    var center = new Vector3(
                        boundsInt.position.x + boundsInt.size.x * 0.5f,
                        boundsInt.position.y + boundsInt.size.y * 0.5f,
                        boundsInt.position.z + boundsInt.size.z * 0.5f);
                    var size = new Vector3(boundsInt.size.x, boundsInt.size.y, boundsInt.size.z);
                    return new Bounds(center, size);
                }

                throw new MappingException($"不支持的映射类型对：{sourceType.Name} → {destType.Name} / Unsupported type pair: {sourceType.Name} → {destType.Name}");
            }
            catch (Exception ex) when (!(ex is MappingException))
            {
                throw new MappingException($"Bounds 映射失败 / Bounds mapping failed: {ex.Message}", ex);
            }
        }

        #endregion

        #region 私有方法 / Private Methods

        /// <summary>
        /// 获取类型的默认值
        /// </summary>
        private object GetDefaultValue(Type type)
        {
            if (type == typeof(Bounds))
                return new Bounds(Vector3.zero, Vector3.zero);
            if (type == typeof(BoundsInt))
                return new BoundsInt(Vector3Int.zero, Vector3Int.zero);

            return null;
        }

        #endregion
    }
}
