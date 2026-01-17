// ==========================================================
// 文件名：TypeMap.cs
// 命名空间: AFramework.AMapper
// 依赖: System, System.Collections.Generic, System.Linq.Expressions, System.Reflection
// 功能: 类型映射配置实现，存储源-目标类型对的映射规则
// ==========================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace AFramework.AMapper
{
    /// <summary>
    /// 类型映射配置
    /// <para>存储单个源-目标类型对的完整映射配置</para>
    /// <para>Type map configuration storing complete mapping rules for a source-destination type pair</para>
    /// </summary>
    public sealed class TypeMap : ITypeMap
    {
        #region 私有字段 / Private Fields

        private readonly List<MemberMap> _memberMaps;
        private readonly List<MemberMap> _pathMaps;
        private readonly List<LambdaExpression> _beforeMapActions;
        private readonly List<LambdaExpression> _afterMapActions;
        private readonly List<TypePair> _includedDerivedTypes;
        private readonly List<TypePair> _includedBaseTypes;
        private readonly List<LambdaExpression> _includedMembers;
        private readonly List<ValueTransformerConfiguration> _valueTransformers;
        private ConstructorMap _constructorMap;
        private bool _isSealed;

        #endregion

        #region ITypeMap 实现 / ITypeMap Implementation

        /// <inheritdoc/>
        public Type SourceType { get; }

        /// <inheritdoc/>
        public Type DestinationType { get; }

        /// <inheritdoc/>
        public TypePair TypePair { get; }

        /// <inheritdoc/>
        public Type ProfileType { get; }

        /// <inheritdoc/>
        public IReadOnlyCollection<IMemberMap> MemberMaps => _memberMaps;

        /// <inheritdoc/>
        public IReadOnlyCollection<IMemberMap> PathMaps => _pathMaps;

        /// <inheritdoc/>
        public IConstructorMap ConstructorMap => _constructorMap;

        /// <inheritdoc/>
        public bool DisableConstructorMapping { get; private set; }

        /// <inheritdoc/>
        public LambdaExpression CustomCtorExpression { get; private set; }

        /// <inheritdoc/>
        public Delegate CustomCtorFunction { get; private set; }

        /// <inheritdoc/>
        public bool HasCustomConstruction => CustomCtorExpression != null || CustomCtorFunction != null;

        /// <inheritdoc/>
        public Type TypeConverterType { get; private set; }

        /// <inheritdoc/>
        public LambdaExpression TypeConverterExpression { get; private set; }

        /// <inheritdoc/>
        public bool HasTypeConverter => TypeConverterType != null || TypeConverterExpression != null;

        /// <inheritdoc/>
        public IReadOnlyList<LambdaExpression> BeforeMapActions => _beforeMapActions;

        /// <inheritdoc/>
        public IReadOnlyList<LambdaExpression> AfterMapActions => _afterMapActions;

        /// <inheritdoc/>
        public IReadOnlyList<TypePair> IncludedDerivedTypes => _includedDerivedTypes;

        /// <inheritdoc/>
        public IReadOnlyList<TypePair> IncludedBaseTypes => _includedBaseTypes;

        /// <inheritdoc/>
        public IReadOnlyList<LambdaExpression> IncludedMembers => _includedMembers;

        /// <inheritdoc/>
        public int? MaxDepth { get; private set; }

        /// <inheritdoc/>
        public bool PreserveReferences { get; private set; }

        /// <inheritdoc/>
        public LambdaExpression ConditionExpression { get; private set; }

        /// <inheritdoc/>
        public IReadOnlyList<ValueTransformerConfiguration> ValueTransformers => _valueTransformers;

        /// <inheritdoc/>
        public bool IsSealed => _isSealed;

        #endregion

        #region 构造函数 / Constructor

        /// <summary>
        /// 创建类型映射实例
        /// </summary>
        /// <param name="sourceType">源类型 / Source type</param>
        /// <param name="destinationType">目标类型 / Destination type</param>
        /// <param name="profileType">所属 Profile 类型 / Profile type</param>
        internal TypeMap(Type sourceType, Type destinationType, Type profileType = null)
        {
            SourceType = sourceType ?? throw new ArgumentNullException(nameof(sourceType));
            DestinationType = destinationType ?? throw new ArgumentNullException(nameof(destinationType));
            TypePair = new TypePair(sourceType, destinationType);
            ProfileType = profileType;

            _memberMaps = new List<MemberMap>();
            _pathMaps = new List<MemberMap>();
            _beforeMapActions = new List<LambdaExpression>();
            _afterMapActions = new List<LambdaExpression>();
            _includedDerivedTypes = new List<TypePair>();
            _includedBaseTypes = new List<TypePair>();
            _includedMembers = new List<LambdaExpression>();
            _valueTransformers = new List<ValueTransformerConfiguration>();

            // 自动发现目标成员
            DiscoverDestinationMembers();
        }

        #endregion

        #region 公共方法 / Public Methods

        /// <inheritdoc/>
        public IMemberMap FindMemberMap(string destinationMemberName)
        {
            return _memberMaps.FirstOrDefault(m => m.DestinationName == destinationMemberName);
        }

        /// <summary>
        /// 获取未映射的目标成员
        /// <para>Get unmapped destination members</para>
        /// </summary>
        public IEnumerable<string> GetUnmappedDestinationMembers()
        {
            return _memberMaps
                .Where(m => !m.IsMapped && !m.IsIgnored)
                .Select(m => m.DestinationName);
        }

        #endregion

        #region 内部配置方法 / Internal Configuration Methods

        /// <summary>
        /// 添加或获取成员映射
        /// </summary>
        internal MemberMap GetOrAddMemberMap(MemberInfo destinationMember)
        {
            ThrowIfSealed();

            var existing = _memberMaps.FirstOrDefault(m => m.DestinationMember == destinationMember);
            if (existing != null)
                return existing;

            var memberMap = new MemberMap(this, destinationMember);
            _memberMaps.Add(memberMap);
            return memberMap;
        }

        /// <summary>
        /// 添加路径映射
        /// </summary>
        internal MemberMap AddPathMap(MemberInfo[] path)
        {
            ThrowIfSealed();

            var pathMap = new MemberMap(this, path);
            _pathMaps.Add(pathMap);
            return pathMap;
        }

        /// <summary>
        /// 设置构造函数映射
        /// </summary>
        internal void SetConstructorMap(ConstructorMap constructorMap)
        {
            ThrowIfSealed();
            _constructorMap = constructorMap;
        }

        /// <summary>
        /// 禁用构造函数映射
        /// </summary>
        internal void DisableConstructorMappingInternal()
        {
            ThrowIfSealed();
            DisableConstructorMapping = true;
        }

        /// <summary>
        /// 设置自定义构造表达式
        /// </summary>
        internal void SetCustomCtorExpression(LambdaExpression expression)
        {
            ThrowIfSealed();
            CustomCtorExpression = expression;
        }

        /// <summary>
        /// 设置自定义构造函数
        /// </summary>
        internal void SetCustomCtorFunction(Delegate function)
        {
            ThrowIfSealed();
            CustomCtorFunction = function;
        }

        /// <summary>
        /// 设置类型转换器类型
        /// </summary>
        internal void SetTypeConverter(Type converterType)
        {
            ThrowIfSealed();
            TypeConverterType = converterType;
        }

        /// <summary>
        /// 设置类型转换表达式
        /// </summary>
        internal void SetTypeConverterExpression(LambdaExpression expression)
        {
            ThrowIfSealed();
            TypeConverterExpression = expression;
        }

        /// <summary>
        /// 添加前置映射动作
        /// </summary>
        internal void AddBeforeMapAction(LambdaExpression action)
        {
            ThrowIfSealed();
            _beforeMapActions.Add(action);
        }

        /// <summary>
        /// 添加后置映射动作
        /// </summary>
        internal void AddAfterMapAction(LambdaExpression action)
        {
            ThrowIfSealed();
            _afterMapActions.Add(action);
        }

        /// <summary>
        /// 添加派生类型映射
        /// </summary>
        internal void AddIncludedDerivedType(TypePair typePair)
        {
            ThrowIfSealed();
            _includedDerivedTypes.Add(typePair);
        }

        /// <summary>
        /// 添加基类型映射
        /// </summary>
        internal void AddIncludedBaseType(TypePair typePair)
        {
            ThrowIfSealed();
            _includedBaseTypes.Add(typePair);
        }

        /// <summary>
        /// 添加包含成员
        /// </summary>
        internal void AddIncludedMember(LambdaExpression memberExpression)
        {
            ThrowIfSealed();
            _includedMembers.Add(memberExpression);
        }

        /// <summary>
        /// 设置最大深度
        /// </summary>
        internal void SetMaxDepth(int depth)
        {
            ThrowIfSealed();
            MaxDepth = depth;
        }

        /// <summary>
        /// 启用引用保留
        /// </summary>
        internal void EnablePreserveReferences()
        {
            ThrowIfSealed();
            PreserveReferences = true;
        }

        /// <summary>
        /// 设置条件表达式
        /// </summary>
        internal void SetCondition(LambdaExpression condition)
        {
            ThrowIfSealed();
            ConditionExpression = condition;
        }

        /// <summary>
        /// 添加值转换器
        /// </summary>
        internal void AddValueTransformer(ValueTransformerConfiguration transformer)
        {
            ThrowIfSealed();
            _valueTransformers.Add(transformer);
        }

        /// <summary>
        /// 密封配置
        /// </summary>
        internal void Seal()
        {
            if (_isSealed) return;

            _isSealed = true;

            // 密封所有成员映射
            foreach (var memberMap in _memberMaps)
            {
                memberMap.Seal();
            }

            // 如果没有禁用构造函数映射且没有自定义构造，尝试发现构造函数
            if (!DisableConstructorMapping && !HasCustomConstruction && _constructorMap == null)
            {
                DiscoverConstructor();
            }
        }

        #endregion

        #region 私有方法 / Private Methods

        private void ThrowIfSealed()
        {
            if (_isSealed)
            {
                throw new InvalidOperationException(
                    $"类型映射已密封，无法修改 / TypeMap is sealed and cannot be modified: {TypePair}");
            }
        }

        private void DiscoverDestinationMembers()
        {
            // 获取目标类型的所有可写属性
            var properties = DestinationType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanWrite && p.GetSetMethod() != null);

            foreach (var property in properties)
            {
                var memberMap = new MemberMap(this, property);
                _memberMaps.Add(memberMap);
            }

            // 获取目标类型的所有公共字段
            var fields = DestinationType.GetFields(BindingFlags.Public | BindingFlags.Instance)
                .Where(f => !f.IsInitOnly);

            foreach (var field in fields)
            {
                var memberMap = new MemberMap(this, field);
                _memberMaps.Add(memberMap);
            }
        }

        private void DiscoverConstructor()
        {
            // 查找最佳构造函数
            var constructors = DestinationType.GetConstructors(BindingFlags.Public | BindingFlags.Instance)
                .OrderByDescending(c => c.GetParameters().Length);

            foreach (var ctor in constructors)
            {
                var parameters = ctor.GetParameters();
                if (parameters.Length == 0)
                {
                    // 默认构造函数，不需要 ConstructorMap
                    break;
                }

                // 检查是否所有参数都能从源类型解析
                var canResolve = true;
                var parameterMaps = new List<ConstructorParameterMap>();

                foreach (var param in parameters)
                {
                    var sourceMember = FindSourceMemberForParameter(param);
                    var paramMap = new ConstructorParameterMap(param, sourceMember);
                    parameterMaps.Add(paramMap);

                    if (sourceMember == null)
                    {
                        canResolve = false;
                    }
                }

                if (canResolve)
                {
                    _constructorMap = new ConstructorMap(ctor, parameterMaps);
                    break;
                }
            }
        }

        private MemberInfo FindSourceMemberForParameter(ParameterInfo parameter)
        {
            var paramName = parameter.Name;

            // 尝试按名称匹配属性
            var property = SourceType.GetProperty(paramName, 
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            if (property != null && parameter.ParameterType.IsAssignableFrom(property.PropertyType))
            {
                return property;
            }

            // 尝试按名称匹配字段
            var field = SourceType.GetField(paramName,
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            if (field != null && parameter.ParameterType.IsAssignableFrom(field.FieldType))
            {
                return field;
            }

            return null;
        }

        #endregion
    }
}
