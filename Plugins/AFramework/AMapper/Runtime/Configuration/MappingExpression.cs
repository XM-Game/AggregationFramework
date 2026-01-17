// ==========================================================
// 文件名：MappingExpression.cs
// 命名空间: AFramework.AMapper
// 依赖: System, System.Linq.Expressions, System.Reflection
// 功能: 类型映射配置表达式实现，提供成员级别的映射配置
// ==========================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace AFramework.AMapper
{
    /// <summary>
    /// 类型映射配置表达式基类（非泛型）
    /// <para>Base class for type mapping configuration expression (non-generic)</para>
    /// </summary>
    internal class MappingExpressionBase : IMappingExpression
    {
        #region 私有字段 / Private Fields

        protected TypeMap _typeMap;
        protected readonly Func<TypeMap> _typeMapFactory;
        protected readonly List<Action<TypeMap>> _pendingActions;

        #endregion

        #region 属性 / Properties

        /// <inheritdoc/>
        public ITypeMap TypeMap => _typeMap;

        #endregion

        #region 构造函数 / Constructors

        /// <summary>
        /// 创建映射表达式实例（延迟绑定）
        /// </summary>
        internal MappingExpressionBase(Func<TypeMap> typeMapFactory)
        {
            _typeMapFactory = typeMapFactory;
            _pendingActions = new List<Action<TypeMap>>();
        }

        /// <summary>
        /// 创建映射表达式实例（立即绑定）
        /// </summary>
        internal MappingExpressionBase(TypeMap typeMap)
        {
            _typeMap = typeMap ?? throw new ArgumentNullException(nameof(typeMap));
            _pendingActions = new List<Action<TypeMap>>();
        }

        #endregion

        #region 内部方法 / Internal Methods

        /// <summary>
        /// 应用配置到 TypeMap
        /// </summary>
        internal void ApplyTo(TypeMap typeMap)
        {
            _typeMap = typeMap;
            foreach (var action in _pendingActions)
            {
                action(typeMap);
            }
        }

        /// <summary>
        /// 添加待执行的配置动作
        /// </summary>
        protected void AddAction(Action<TypeMap> action)
        {
            if (_typeMap != null)
            {
                action(_typeMap);
            }
            else
            {
                _pendingActions.Add(action);
            }
        }

        #endregion
    }

    /// <summary>
    /// 类型映射配置表达式实现
    /// <para>Type mapping configuration expression implementation</para>
    /// </summary>
    /// <typeparam name="TSource">源类型 / Source type</typeparam>
    /// <typeparam name="TDestination">目标类型 / Destination type</typeparam>
    internal sealed class MappingExpression<TSource, TDestination> : MappingExpressionBase, IMappingExpression<TSource, TDestination>
    {
        #region 构造函数 / Constructors

        internal MappingExpression(Func<TypeMap> typeMapFactory) : base(typeMapFactory) { }

        internal MappingExpression(TypeMap typeMap) : base(typeMap) { }

        #endregion

        #region 成员映射 / Member Mapping

        /// <inheritdoc/>
        public IMappingExpression<TSource, TDestination> ForMember<TMember>(
            Expression<Func<TDestination, TMember>> destinationMember,
            Action<IMemberConfigurationExpression<TSource, TDestination, TMember>> memberOptions)
        {
            var memberInfo = GetMemberInfo(destinationMember);
            
            AddAction(typeMap =>
            {
                var memberMap = typeMap.GetOrAddMemberMap(memberInfo);
                var expression = new MemberConfigurationExpression<TSource, TDestination, TMember>(memberMap);
                memberOptions(expression);
            });

            return this;
        }

        /// <inheritdoc/>
        public IMappingExpression<TSource, TDestination> ForAllMembers(
            Action<IMemberConfigurationExpression<TSource, TDestination, object>> memberOptions)
        {
            AddAction(typeMap =>
            {
                foreach (var memberMap in typeMap.MemberMaps.Cast<MemberMap>())
                {
                    var expression = new MemberConfigurationExpression<TSource, TDestination, object>(memberMap);
                    memberOptions(expression);
                }
            });

            return this;
        }

        /// <inheritdoc/>
        public IMappingExpression<TSource, TDestination> ForAllOtherMembers(
            Action<IMemberConfigurationExpression<TSource, TDestination, object>> memberOptions)
        {
            AddAction(typeMap =>
            {
                foreach (var memberMap in typeMap.MemberMaps.Cast<MemberMap>().Where(m => !m.IsMapped))
                {
                    var expression = new MemberConfigurationExpression<TSource, TDestination, object>(memberMap);
                    memberOptions(expression);
                }
            });

            return this;
        }

        #endregion

        #region 路径映射 / Path Mapping

        /// <inheritdoc/>
        public IMappingExpression<TSource, TDestination> ForPath<TMember>(
            Expression<Func<TDestination, TMember>> destinationPath,
            Action<IPathConfigurationExpression<TSource, TDestination, TMember>> pathOptions)
        {
            var path = GetMemberPath(destinationPath);
            
            AddAction(typeMap =>
            {
                var pathMap = typeMap.AddPathMap(path);
                var expression = new PathConfigurationExpression<TSource, TDestination, TMember>(pathMap);
                pathOptions(expression);
            });

            return this;
        }

        #endregion

        #region 构造函数配置 / Constructor Configuration

        /// <inheritdoc/>
        public IMappingExpression<TSource, TDestination> ForCtorParam(
            string parameterName,
            Action<ICtorParamConfigurationExpression<TSource>> parameterOptions)
        {
            AddAction(typeMap =>
            {
                var ctorMap = typeMap.ConstructorMap as ConstructorMap;
                if (ctorMap != null)
                {
                    var paramMap = ctorMap.FindParameterMap(parameterName);
                    if (paramMap != null)
                    {
                        var expression = new CtorParamConfigurationExpression<TSource>(paramMap);
                        parameterOptions(expression);
                    }
                }
            });

            return this;
        }

        /// <inheritdoc/>
        public IMappingExpression<TSource, TDestination> ConstructUsing(
            Expression<Func<TSource, TDestination>> ctor)
        {
            AddAction(typeMap => typeMap.SetCustomCtorExpression(ctor));
            return this;
        }

        /// <inheritdoc/>
        public IMappingExpression<TSource, TDestination> ConstructUsing(
            Func<TSource, ResolutionContext, TDestination> ctor)
        {
            AddAction(typeMap => typeMap.SetCustomCtorFunction(ctor));
            return this;
        }

        /// <inheritdoc/>
        public IMappingExpression<TSource, TDestination> DisableCtorValidation()
        {
            AddAction(typeMap => typeMap.DisableConstructorMappingInternal());
            return this;
        }

        #endregion

        #region 类型转换器 / Type Converter

        /// <inheritdoc/>
        public IMappingExpression<TSource, TDestination> ConvertUsing<TConverter>()
            where TConverter : ITypeConverter<TSource, TDestination>
        {
            AddAction(typeMap => typeMap.SetTypeConverter(typeof(TConverter)));
            return this;
        }

        /// <inheritdoc/>
        public IMappingExpression<TSource, TDestination> ConvertUsing(
            Expression<Func<TSource, TDestination>> converter)
        {
            AddAction(typeMap => typeMap.SetTypeConverterExpression(converter));
            return this;
        }

        /// <inheritdoc/>
        public IMappingExpression<TSource, TDestination> ConvertUsing(
            Func<TSource, TDestination> converter)
        {
            Expression<Func<TSource, TDestination>> expr = s => converter(s);
            AddAction(typeMap => typeMap.SetTypeConverterExpression(expr));
            return this;
        }

        /// <inheritdoc/>
        public IMappingExpression<TSource, TDestination> ConvertUsing(
            Func<TSource, TDestination, ResolutionContext, TDestination> converter)
        {
            AddAction(typeMap => typeMap.SetCustomCtorFunction(
                (Func<TSource, ResolutionContext, TDestination>)((s, ctx) => converter(s, default, ctx))));
            return this;
        }

        #endregion

        #region 映射动作 / Mapping Actions

        /// <inheritdoc/>
        public IMappingExpression<TSource, TDestination> BeforeMap(
            Action<TSource, TDestination> beforeFunction)
        {
            Expression<Action<TSource, TDestination>> expr = (s, d) => beforeFunction(s, d);
            AddAction(typeMap => typeMap.AddBeforeMapAction(expr));
            return this;
        }

        /// <inheritdoc/>
        public IMappingExpression<TSource, TDestination> BeforeMap(
            Action<TSource, TDestination, ResolutionContext> beforeFunction)
        {
            Expression<Action<TSource, TDestination, ResolutionContext>> expr = (s, d, c) => beforeFunction(s, d, c);
            AddAction(typeMap => typeMap.AddBeforeMapAction(expr));
            return this;
        }

        /// <inheritdoc/>
        public IMappingExpression<TSource, TDestination> BeforeMap<TMappingAction>()
            where TMappingAction : IMappingAction<TSource, TDestination>
        {
            // 存储类型信息，运行时创建实例
            AddAction(typeMap =>
            {
                Expression<Action<TSource, TDestination, ResolutionContext>> expr = (s, d, c) =>
                    ((IMappingAction<TSource, TDestination>)Activator.CreateInstance(typeof(TMappingAction))).Process(s, d, c);
                typeMap.AddBeforeMapAction(expr);
            });
            return this;
        }

        /// <inheritdoc/>
        public IMappingExpression<TSource, TDestination> AfterMap(
            Action<TSource, TDestination> afterFunction)
        {
            Expression<Action<TSource, TDestination>> expr = (s, d) => afterFunction(s, d);
            AddAction(typeMap => typeMap.AddAfterMapAction(expr));
            return this;
        }

        /// <inheritdoc/>
        public IMappingExpression<TSource, TDestination> AfterMap(
            Action<TSource, TDestination, ResolutionContext> afterFunction)
        {
            Expression<Action<TSource, TDestination, ResolutionContext>> expr = (s, d, c) => afterFunction(s, d, c);
            AddAction(typeMap => typeMap.AddAfterMapAction(expr));
            return this;
        }

        /// <inheritdoc/>
        public IMappingExpression<TSource, TDestination> AfterMap<TMappingAction>()
            where TMappingAction : IMappingAction<TSource, TDestination>
        {
            AddAction(typeMap =>
            {
                Expression<Action<TSource, TDestination, ResolutionContext>> expr = (s, d, c) =>
                    ((IMappingAction<TSource, TDestination>)Activator.CreateInstance(typeof(TMappingAction))).Process(s, d, c);
                typeMap.AddAfterMapAction(expr);
            });
            return this;
        }

        #endregion

        #region 继承映射 / Inheritance Mapping

        /// <inheritdoc/>
        public IMappingExpression<TSource, TDestination> Include<TDerivedSource, TDerivedDestination>()
            where TDerivedSource : TSource
            where TDerivedDestination : TDestination
        {
            AddAction(typeMap => typeMap.AddIncludedDerivedType(
                new TypePair(typeof(TDerivedSource), typeof(TDerivedDestination))));
            return this;
        }

        /// <inheritdoc/>
        public IMappingExpression<TSource, TDestination> IncludeBase<TBaseSource, TBaseDestination>()
        {
            AddAction(typeMap => typeMap.AddIncludedBaseType(
                new TypePair(typeof(TBaseSource), typeof(TBaseDestination))));
            return this;
        }

        /// <inheritdoc/>
        public IMappingExpression<TSource, TDestination> IncludeMembers(
            params Expression<Func<TSource, object>>[] memberExpressions)
        {
            AddAction(typeMap =>
            {
                foreach (var expr in memberExpressions)
                {
                    typeMap.AddIncludedMember(expr);
                }
            });
            return this;
        }

        #endregion

        #region 映射选项 / Mapping Options

        /// <inheritdoc/>
        public IMappingExpression<TSource, TDestination> MaxDepth(int depth)
        {
            AddAction(typeMap => typeMap.SetMaxDepth(depth));
            return this;
        }

        /// <inheritdoc/>
        public IMappingExpression<TSource, TDestination> PreserveReferences()
        {
            AddAction(typeMap => typeMap.EnablePreserveReferences());
            return this;
        }

        /// <inheritdoc/>
        public IMappingExpression<TSource, TDestination> Condition(
            Func<TSource, TDestination, bool> condition)
        {
            Expression<Func<TSource, TDestination, bool>> expr = (s, d) => condition(s, d);
            AddAction(typeMap => typeMap.SetCondition(expr));
            return this;
        }

        /// <inheritdoc/>
        public IMappingExpression<TSource, TDestination> AddTransform<TValue>(
            Expression<Func<TValue, TValue>> transformer)
        {
            AddAction(typeMap => typeMap.AddValueTransformer(
                new ValueTransformerConfiguration(typeof(TValue), transformer)));
            return this;
        }

        #endregion

        #region 反向映射 / Reverse Mapping

        /// <inheritdoc/>
        public IMappingExpression<TDestination, TSource> ReverseMap()
        {
            // 创建反向映射
            // 注意：这需要在 MapperConfiguration 中注册
            return new MappingExpression<TDestination, TSource>(() => null);
        }

        #endregion

        #region 私有方法 / Private Methods

        private static MemberInfo GetMemberInfo<TMember>(Expression<Func<TDestination, TMember>> expression)
        {
            if (expression.Body is MemberExpression memberExpr)
            {
                return memberExpr.Member;
            }

            if (expression.Body is UnaryExpression unaryExpr && unaryExpr.Operand is MemberExpression innerMemberExpr)
            {
                return innerMemberExpr.Member;
            }

            throw new ArgumentException(
                $"表达式必须是成员访问表达式 / Expression must be a member access expression: {expression}");
        }

        private static MemberInfo[] GetMemberPath<TMember>(Expression<Func<TDestination, TMember>> expression)
        {
            var path = new List<MemberInfo>();
            var current = expression.Body;

            while (current is MemberExpression memberExpr)
            {
                path.Insert(0, memberExpr.Member);
                current = memberExpr.Expression;
            }

            if (path.Count == 0)
            {
                throw new ArgumentException(
                    $"表达式必须是成员访问路径 / Expression must be a member access path: {expression}");
            }

            return path.ToArray();
        }

        #endregion
    }
}
