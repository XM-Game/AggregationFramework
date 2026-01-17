// ==========================================================
// 文件名：MemberMap.cs
// 命名空间: AFramework.AMapper
// 依赖: System, System.Linq.Expressions, System.Reflection
// 功能: 成员映射配置实现，存储单个成员的映射规则
// ==========================================================

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace AFramework.AMapper
{
    /// <summary>
    /// 成员映射配置
    /// <para>存储单个目标成员的完整映射配置</para>
    /// <para>Member map configuration storing complete mapping rules for a single destination member</para>
    /// </summary>
    public sealed class MemberMap : IMemberMap
    {
        #region 私有字段 / Private Fields

        private MemberInfo[] _sourceMembers;
        private bool _isSealed;

        #endregion

        #region IMemberMap 实现 / IMemberMap Implementation

        /// <inheritdoc/>
        public MemberInfo DestinationMember { get; }

        /// <inheritdoc/>
        public string DestinationName { get; }

        /// <inheritdoc/>
        public Type DestinationType { get; }

        /// <inheritdoc/>
        public MemberInfo[] SourceMembers
        {
            get => _sourceMembers;
            private set => _sourceMembers = value;
        }

        /// <inheritdoc/>
        public string SourceMemberName
        {
            get
            {
                if (_sourceMembers == null || _sourceMembers.Length == 0)
                    return null;
                return string.Join(".", _sourceMembers.Select(m => m.Name));
            }
        }

        /// <inheritdoc/>
        public Type SourceType
        {
            get
            {
                if (_sourceMembers == null || _sourceMembers.Length == 0)
                    return null;
                var lastMember = _sourceMembers[_sourceMembers.Length - 1];
                return GetMemberType(lastMember);
            }
        }

        /// <inheritdoc/>
        public bool IsIgnored { get; private set; }

        /// <inheritdoc/>
        public bool IsMapped => IsIgnored || HasCustomResolver || _sourceMembers != null;

        /// <inheritdoc/>
        public bool UseDestinationValue { get; private set; }

        /// <inheritdoc/>
        public int MappingOrder { get; private set; }

        /// <inheritdoc/>
        public LambdaExpression CustomMapExpression { get; private set; }

        /// <inheritdoc/>
        public Type ValueResolverType { get; private set; }

        /// <inheritdoc/>
        public IValueResolverConfiguration ValueResolverConfig { get; private set; }

        /// <inheritdoc/>
        public bool HasCustomResolver => CustomMapExpression != null || ValueResolverType != null || ValueResolverConfig != null;

        /// <inheritdoc/>
        public Type ValueConverterType { get; private set; }

        /// <inheritdoc/>
        public LambdaExpression ValueConverterExpression { get; private set; }

        /// <inheritdoc/>
        public bool HasValueConverter => ValueConverterType != null || ValueConverterExpression != null;

        /// <inheritdoc/>
        public LambdaExpression ConditionExpression { get; private set; }

        /// <inheritdoc/>
        public LambdaExpression PreConditionExpression { get; private set; }

        /// <inheritdoc/>
        public bool HasCondition => ConditionExpression != null || PreConditionExpression != null;

        /// <inheritdoc/>
        public object NullSubstitute { get; private set; }

        /// <inheritdoc/>
        public bool HasNullSubstitute { get; private set; }

        /// <inheritdoc/>
        public bool? AllowNull { get; private set; }

        /// <inheritdoc/>
        public ITypeMap TypeMap { get; }

        #endregion

        #region 构造函数 / Constructors

        /// <summary>
        /// 创建成员映射实例
        /// </summary>
        /// <param name="typeMap">所属类型映射 / Parent type map</param>
        /// <param name="destinationMember">目标成员 / Destination member</param>
        internal MemberMap(TypeMap typeMap, MemberInfo destinationMember)
        {
            TypeMap = typeMap ?? throw new ArgumentNullException(nameof(typeMap));
            DestinationMember = destinationMember ?? throw new ArgumentNullException(nameof(destinationMember));
            DestinationName = destinationMember.Name;
            DestinationType = GetMemberType(destinationMember);

            // 尝试自动匹配源成员
            TryAutoMatch(typeMap.SourceType);
        }

        /// <summary>
        /// 创建路径映射实例
        /// </summary>
        /// <param name="typeMap">所属类型映射 / Parent type map</param>
        /// <param name="path">目标路径 / Destination path</param>
        internal MemberMap(TypeMap typeMap, MemberInfo[] path)
        {
            TypeMap = typeMap ?? throw new ArgumentNullException(nameof(typeMap));
            
            if (path == null || path.Length == 0)
                throw new ArgumentException("路径不能为空 / Path cannot be empty", nameof(path));

            DestinationMember = path[path.Length - 1];
            DestinationName = string.Join(".", path.Select(m => m.Name));
            DestinationType = GetMemberType(DestinationMember);
        }

        #endregion

        #region 内部配置方法 / Internal Configuration Methods

        /// <summary>
        /// 设置忽略
        /// </summary>
        internal void SetIgnored()
        {
            ThrowIfSealed();
            IsIgnored = true;
        }

        /// <summary>
        /// 设置源成员
        /// </summary>
        internal void SetSourceMembers(MemberInfo[] members)
        {
            ThrowIfSealed();
            _sourceMembers = members;
        }

        /// <summary>
        /// 设置自定义映射表达式
        /// </summary>
        internal void SetCustomMapExpression(LambdaExpression expression)
        {
            ThrowIfSealed();
            CustomMapExpression = expression;
        }

        /// <summary>
        /// 设置值解析器类型
        /// </summary>
        internal void SetValueResolverType(Type resolverType)
        {
            ThrowIfSealed();
            ValueResolverType = resolverType;
        }

        /// <summary>
        /// 设置值解析器配置
        /// </summary>
        internal void SetValueResolverConfig(IValueResolverConfiguration config)
        {
            ThrowIfSealed();
            ValueResolverConfig = config;
        }

        /// <summary>
        /// 设置值转换器类型
        /// </summary>
        internal void SetValueConverterType(Type converterType)
        {
            ThrowIfSealed();
            ValueConverterType = converterType;
        }

        /// <summary>
        /// 设置值转换表达式
        /// </summary>
        internal void SetValueConverterExpression(LambdaExpression expression)
        {
            ThrowIfSealed();
            ValueConverterExpression = expression;
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
        /// 设置前置条件表达式
        /// </summary>
        internal void SetPreCondition(LambdaExpression condition)
        {
            ThrowIfSealed();
            PreConditionExpression = condition;
        }

        /// <summary>
        /// 设置空值替换
        /// </summary>
        internal void SetNullSubstitute(object value)
        {
            ThrowIfSealed();
            NullSubstitute = value;
            HasNullSubstitute = true;
        }

        /// <summary>
        /// 设置允许空值
        /// </summary>
        internal void SetAllowNull(bool allow)
        {
            ThrowIfSealed();
            AllowNull = allow;
        }

        /// <summary>
        /// 设置使用目标值
        /// </summary>
        internal void SetUseDestinationValue()
        {
            ThrowIfSealed();
            UseDestinationValue = true;
        }

        /// <summary>
        /// 设置映射顺序
        /// </summary>
        internal void SetMappingOrder(int order)
        {
            ThrowIfSealed();
            MappingOrder = order;
        }

        /// <summary>
        /// 密封配置
        /// </summary>
        internal void Seal()
        {
            _isSealed = true;
        }

        #endregion

        #region 私有方法 / Private Methods

        private void ThrowIfSealed()
        {
            if (_isSealed)
            {
                throw new InvalidOperationException(
                    $"成员映射已密封，无法修改 / MemberMap is sealed and cannot be modified: {DestinationName}");
            }
        }

        private void TryAutoMatch(Type sourceType)
        {
            // 尝试按名称精确匹配
            var property = sourceType.GetProperty(DestinationName, 
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            if (property != null)
            {
                _sourceMembers = new[] { property };
                return;
            }

            var field = sourceType.GetField(DestinationName,
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            if (field != null)
            {
                _sourceMembers = new[] { field };
                return;
            }

            // 尝试扁平化匹配（如 CustomerName -> Customer.Name）
            TryFlattenMatch(sourceType);
        }

        private void TryFlattenMatch(Type sourceType)
        {
            // 简单的扁平化匹配实现
            // 例如：CustomerName 可能匹配 Customer.Name
            var destName = DestinationName;
            
            foreach (var prop in sourceType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (destName.StartsWith(prop.Name, StringComparison.OrdinalIgnoreCase) && 
                    destName.Length > prop.Name.Length)
                {
                    var remainingName = destName.Substring(prop.Name.Length);
                    var nestedMember = prop.PropertyType.GetProperty(remainingName,
                        BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                    
                    if (nestedMember != null)
                    {
                        _sourceMembers = new MemberInfo[] { prop, nestedMember };
                        return;
                    }
                }
            }
        }

        private static Type GetMemberType(MemberInfo member)
        {
            return member switch
            {
                PropertyInfo prop => prop.PropertyType,
                FieldInfo field => field.FieldType,
                _ => throw new ArgumentException($"不支持的成员类型 / Unsupported member type: {member.MemberType}")
            };
        }

        #endregion
    }

    /// <summary>
    /// 值解析器配置
    /// <para>Value resolver configuration</para>
    /// </summary>
    public sealed class ValueResolverConfiguration : IValueResolverConfiguration
    {
        /// <inheritdoc/>
        public Type ResolverType { get; }

        /// <inheritdoc/>
        public object ResolverInstance { get; }

        /// <inheritdoc/>
        public LambdaExpression SourceMemberExpression { get; }

        /// <inheritdoc/>
        public Type SourceMemberType { get; }

        /// <summary>
        /// 创建值解析器配置
        /// </summary>
        public ValueResolverConfiguration(Type resolverType, object resolverInstance = null,
            LambdaExpression sourceMemberExpression = null, Type sourceMemberType = null)
        {
            ResolverType = resolverType;
            ResolverInstance = resolverInstance;
            SourceMemberExpression = sourceMemberExpression;
            SourceMemberType = sourceMemberType;
        }
    }
}
