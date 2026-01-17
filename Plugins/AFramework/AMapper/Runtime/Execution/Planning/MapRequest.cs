// ==========================================================
// 文件名：MapRequest.cs
// 命名空间: AFramework.AMapper
// 依赖: System
// 功能: 映射请求结构体，封装单次映射操作的请求信息
// ==========================================================

using System;

namespace AFramework.AMapper
{
    /// <summary>
    /// 映射请求
    /// <para>封装单次映射操作的请求信息</para>
    /// <para>Map request that encapsulates information for a single mapping operation</para>
    /// </summary>
    public readonly struct MapRequest : IEquatable<MapRequest>
    {
        #region 属性 / Properties

        /// <summary>
        /// 获取类型对
        /// <para>Get the type pair</para>
        /// </summary>
        public TypePair TypePair { get; }

        /// <summary>
        /// 获取源类型
        /// <para>Get the source type</para>
        /// </summary>
        public Type SourceType => TypePair.SourceType;

        /// <summary>
        /// 获取目标类型
        /// <para>Get the destination type</para>
        /// </summary>
        public Type DestinationType => TypePair.DestinationType;

        /// <summary>
        /// 获取运行时源类型
        /// <para>Get the runtime source type</para>
        /// </summary>
        /// <remarks>
        /// 当源对象的实际类型与声明类型不同时使用。
        /// 例如：声明为 object，实际为 Player。
        /// </remarks>
        public Type RuntimeSourceType { get; }

        /// <summary>
        /// 获取运行时目标类型
        /// <para>Get the runtime destination type</para>
        /// </summary>
        public Type RuntimeDestinationType { get; }

        /// <summary>
        /// 获取是否有运行时类型
        /// <para>Get whether has runtime types</para>
        /// </summary>
        public bool HasRuntimeTypes => RuntimeSourceType != null || RuntimeDestinationType != null;

        /// <summary>
        /// 获取成员映射（如果是成员映射请求）
        /// <para>Get the member map (if this is a member mapping request)</para>
        /// </summary>
        public IMemberMap MemberMap { get; }

        /// <summary>
        /// 获取是否是成员映射请求
        /// <para>Get whether this is a member mapping request</para>
        /// </summary>
        public bool IsMemberMapRequest => MemberMap != null;

        #endregion

        #region 构造函数 / Constructors

        /// <summary>
        /// 创建映射请求
        /// </summary>
        /// <param name="typePair">类型对 / Type pair</param>
        public MapRequest(TypePair typePair)
        {
            TypePair = typePair;
            RuntimeSourceType = null;
            RuntimeDestinationType = null;
            MemberMap = null;
        }

        /// <summary>
        /// 创建映射请求
        /// </summary>
        /// <param name="sourceType">源类型 / Source type</param>
        /// <param name="destinationType">目标类型 / Destination type</param>
        public MapRequest(Type sourceType, Type destinationType)
            : this(new TypePair(sourceType, destinationType))
        {
        }

        /// <summary>
        /// 创建映射请求（带运行时类型）
        /// </summary>
        /// <param name="typePair">类型对 / Type pair</param>
        /// <param name="runtimeSourceType">运行时源类型 / Runtime source type</param>
        /// <param name="runtimeDestinationType">运行时目标类型 / Runtime destination type</param>
        public MapRequest(TypePair typePair, Type runtimeSourceType, Type runtimeDestinationType)
        {
            TypePair = typePair;
            RuntimeSourceType = runtimeSourceType;
            RuntimeDestinationType = runtimeDestinationType;
            MemberMap = null;
        }

        /// <summary>
        /// 创建成员映射请求
        /// </summary>
        /// <param name="typePair">类型对 / Type pair</param>
        /// <param name="memberMap">成员映射 / Member map</param>
        public MapRequest(TypePair typePair, IMemberMap memberMap)
        {
            TypePair = typePair;
            RuntimeSourceType = null;
            RuntimeDestinationType = null;
            MemberMap = memberMap;
        }

        #endregion

        #region 工厂方法 / Factory Methods

        /// <summary>
        /// 创建泛型映射请求
        /// <para>Create generic map request</para>
        /// </summary>
        /// <typeparam name="TSource">源类型 / Source type</typeparam>
        /// <typeparam name="TDestination">目标类型 / Destination type</typeparam>
        /// <returns>映射请求 / Map request</returns>
        public static MapRequest Create<TSource, TDestination>()
        {
            return new MapRequest(TypePair.Create<TSource, TDestination>());
        }

        /// <summary>
        /// 创建带运行时类型的映射请求
        /// <para>Create map request with runtime types</para>
        /// </summary>
        /// <param name="source">源对象 / Source object</param>
        /// <param name="destinationType">目标类型 / Destination type</param>
        /// <returns>映射请求 / Map request</returns>
        public static MapRequest CreateFromRuntime(object source, Type destinationType)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            var sourceType = source.GetType();
            return new MapRequest(new TypePair(sourceType, destinationType));
        }

        #endregion

        #region IEquatable<MapRequest> 实现 / Implementation

        /// <inheritdoc/>
        public bool Equals(MapRequest other)
        {
            return TypePair.Equals(other.TypePair) &&
                   RuntimeSourceType == other.RuntimeSourceType &&
                   RuntimeDestinationType == other.RuntimeDestinationType &&
                   ReferenceEquals(MemberMap, other.MemberMap);
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return obj is MapRequest other && Equals(other);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            unchecked
            {
                var hash = TypePair.GetHashCode();
                hash = (hash * 397) ^ (RuntimeSourceType?.GetHashCode() ?? 0);
                hash = (hash * 397) ^ (RuntimeDestinationType?.GetHashCode() ?? 0);
                hash = (hash * 397) ^ (MemberMap?.GetHashCode() ?? 0);
                return hash;
            }
        }

        #endregion

        #region 运算符重载 / Operator Overloads

        /// <summary>
        /// 相等运算符
        /// </summary>
        public static bool operator ==(MapRequest left, MapRequest right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// 不等运算符
        /// </summary>
        public static bool operator !=(MapRequest left, MapRequest right)
        {
            return !left.Equals(right);
        }

        #endregion

        #region ToString

        /// <inheritdoc/>
        public override string ToString()
        {
            if (HasRuntimeTypes)
            {
                return $"MapRequest: {TypePair} (Runtime: {RuntimeSourceType?.Name ?? "?"} -> {RuntimeDestinationType?.Name ?? "?"})";
            }
            return $"MapRequest: {TypePair}";
        }

        #endregion
    }
}
