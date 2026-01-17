// ==========================================================
// 文件名：ExecutionPlanBuilder.cs
// 命名空间: AFramework.AMapper
// 依赖: System, System.Linq.Expressions, System.Reflection
// 功能: 执行计划构建器，生成映射执行表达式树
// ==========================================================

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace AFramework.AMapper
{
    /// <summary>
    /// 执行计划构建器
    /// <para>根据 TypeMap 配置生成映射执行表达式树</para>
    /// <para>Execution plan builder that generates mapping expression trees from TypeMap configuration</para>
    /// </summary>
    /// <remarks>
    /// ExecutionPlanBuilder 是 AMapper 的核心组件，负责：
    /// <list type="bullet">
    /// <item>根据 TypeMap 配置生成表达式树</item>
    /// <item>编译表达式树为可执行委托</item>
    /// <item>缓存编译后的执行计划</item>
    /// <item>处理复杂映射场景（嵌套、集合、循环引用等）</item>
    /// </list>
    /// </remarks>
    public sealed class ExecutionPlanBuilder
    {
        #region 私有字段 / Private Fields

        private readonly MapperConfiguration _configuration;
        private readonly ConcurrentDictionary<TypePair, LambdaExpression> _planCache;
        private readonly ConcurrentDictionary<TypePair, Delegate> _compiledCache;

        #endregion

        #region 构造函数 / Constructor

        /// <summary>
        /// 创建执行计划构建器
        /// </summary>
        /// <param name="configuration">映射配置 / Mapper configuration</param>
        public ExecutionPlanBuilder(MapperConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _planCache = new ConcurrentDictionary<TypePair, LambdaExpression>();
            _compiledCache = new ConcurrentDictionary<TypePair, Delegate>();
        }

        #endregion

        #region 公共方法 / Public Methods

        /// <summary>
        /// 构建执行计划
        /// <para>Build execution plan</para>
        /// </summary>
        /// <typeparam name="TSource">源类型 / Source type</typeparam>
        /// <typeparam name="TDestination">目标类型 / Destination type</typeparam>
        /// <returns>映射表达式 / Mapping expression</returns>
        public Expression<Func<TSource, TDestination, ResolutionContext, TDestination>> BuildPlan<TSource, TDestination>()
        {
            var typePair = TypePair.Create<TSource, TDestination>();
            return (Expression<Func<TSource, TDestination, ResolutionContext, TDestination>>)BuildPlan(typePair);
        }

        /// <summary>
        /// 构建执行计划（非泛型）
        /// <para>Build execution plan (non-generic)</para>
        /// </summary>
        /// <param name="typePair">类型对 / Type pair</param>
        /// <returns>映射表达式 / Mapping expression</returns>
        public LambdaExpression BuildPlan(TypePair typePair)
        {
            return _planCache.GetOrAdd(typePair, tp =>
            {
                var typeMap = _configuration.FindTypeMap(tp.SourceType, tp.DestinationType) as TypeMap;
                if (typeMap == null)
                {
                    throw new ConfigurationException($"未找到类型映射配置 / Type map not found: {tp}");
                }
                return BuildPlanCore(typeMap);
            });
        }

        /// <summary>
        /// 获取编译后的映射函数
        /// <para>Get compiled mapping function</para>
        /// </summary>
        /// <typeparam name="TSource">源类型 / Source type</typeparam>
        /// <typeparam name="TDestination">目标类型 / Destination type</typeparam>
        /// <returns>编译后的映射函数 / Compiled mapping function</returns>
        public Func<TSource, TDestination, ResolutionContext, TDestination> GetCompiledMapFunc<TSource, TDestination>()
        {
            var typePair = TypePair.Create<TSource, TDestination>();
            var compiled = _compiledCache.GetOrAdd(typePair, tp =>
            {
                var plan = BuildPlan<TSource, TDestination>();
                return plan.Compile();
            });
            return (Func<TSource, TDestination, ResolutionContext, TDestination>)compiled;
        }

        /// <summary>
        /// 预编译所有映射
        /// <para>Pre-compile all mappings</para>
        /// </summary>
        public void CompileAllMappings()
        {
            foreach (var typeMap in _configuration.GetAllTypeMaps())
            {
                var typePair = typeMap.TypePair;
                if (!_compiledCache.ContainsKey(typePair))
                {
                    var plan = BuildPlan(typePair);
                    _compiledCache.TryAdd(typePair, plan.Compile());
                }
            }
        }

        /// <summary>
        /// 清除缓存
        /// <para>Clear cache</para>
        /// </summary>
        public void ClearCache()
        {
            _planCache.Clear();
            _compiledCache.Clear();
        }

        /// <summary>
        /// 获取执行计划的字符串表示（用于调试）
        /// <para>Get string representation of execution plan (for debugging)</para>
        /// </summary>
        /// <typeparam name="TSource">源类型 / Source type</typeparam>
        /// <typeparam name="TDestination">目标类型 / Destination type</typeparam>
        /// <returns>执行计划字符串 / Execution plan string</returns>
        public string GetPlanString<TSource, TDestination>()
        {
            var plan = BuildPlan<TSource, TDestination>();
            return plan.ToString();
        }

        #endregion

        #region 核心构建方法 / Core Build Methods

        private LambdaExpression BuildPlanCore(TypeMap typeMap)
        {
            var sourceParam = Expression.Parameter(typeMap.SourceType, "source");
            var destParam = Expression.Parameter(typeMap.DestinationType, "destination");
            var contextParam = Expression.Parameter(typeof(ResolutionContext), "context");

            var statements = new List<Expression>();
            var variables = new List<ParameterExpression>();

            // 目标变量
            var destVar = Expression.Variable(typeMap.DestinationType, "dest");
            variables.Add(destVar);

            // 返回标签
            var returnLabel = Expression.Label(typeMap.DestinationType, "return");

            // 空值检查：如果源为 null，返回默认值或现有目标
            var nullCheck = BuildSourceNullCheck(sourceParam, destParam, typeMap.DestinationType, returnLabel);
            statements.Add(nullCheck);

            // 检查类型转换器
            if (typeMap.HasTypeConverter)
            {
                var converterExpr = BuildTypeConverterCall(typeMap, sourceParam, destParam, contextParam);
                statements.Add(Expression.Return(returnLabel, converterExpr));
            }
            else
            {
                // 创建或使用现有目标对象
                var createDest = BuildDestinationCreation(typeMap, sourceParam, destParam, contextParam, destVar);
                statements.Add(createDest);

                // 执行前置映射动作
                var beforeMapActions = BuildBeforeMapActions(typeMap, sourceParam, destVar, contextParam);
                statements.AddRange(beforeMapActions);

                // 成员映射
                var memberMappings = BuildMemberMappings(typeMap, sourceParam, destVar, contextParam);
                statements.AddRange(memberMappings);

                // 执行后置映射动作
                var afterMapActions = BuildAfterMapActions(typeMap, sourceParam, destVar, contextParam);
                statements.AddRange(afterMapActions);

                // 返回目标对象
                statements.Add(Expression.Return(returnLabel, destVar));
            }

            // 添加返回标签（默认返回 default）
            statements.Add(Expression.Label(returnLabel, Expression.Default(typeMap.DestinationType)));

            var body = Expression.Block(variables, statements);

            return Expression.Lambda(body, sourceParam, destParam, contextParam);
        }

        private Expression BuildSourceNullCheck(
            ParameterExpression sourceParam,
            ParameterExpression destParam,
            Type destinationType,
            LabelTarget returnLabel)
        {
            // 如果源类型是值类型且非可空，不需要空值检查
            if (sourceParam.Type.IsValueType && System.Nullable.GetUnderlyingType(sourceParam.Type) == null)
            {
                return Expression.Empty();
            }

            // if (source == null) return destination ?? default;
            var nullCheck = Expression.Equal(sourceParam, Expression.Constant(null, sourceParam.Type));

            Expression returnValue;
            if (destParam.Type.IsValueType)
            {
                returnValue = destParam;
            }
            else
            {
                // 如果目标也为 null，返回 default
                returnValue = Expression.Condition(
                    Expression.Equal(destParam, Expression.Constant(null, destParam.Type)),
                    Expression.Default(destinationType),
                    destParam
                );
            }

            return Expression.IfThen(nullCheck, Expression.Return(returnLabel, returnValue));
        }

        private Expression BuildTypeConverterCall(
            TypeMap typeMap,
            ParameterExpression sourceParam,
            ParameterExpression destParam,
            ParameterExpression contextParam)
        {
            // 使用类型转换表达式
            if (typeMap.TypeConverterExpression != null)
            {
                var paramCount = typeMap.TypeConverterExpression.Parameters.Count;
                return paramCount switch
                {
                    1 => Expression.Invoke(typeMap.TypeConverterExpression, sourceParam),
                    2 => Expression.Invoke(typeMap.TypeConverterExpression, sourceParam, destParam),
                    _ => Expression.Invoke(typeMap.TypeConverterExpression, sourceParam, destParam, contextParam)
                };
            }

            // 使用类型转换器类型
            if (typeMap.TypeConverterType != null)
            {
                // 创建转换器实例并调用 Convert 方法
                var converterInstance = Expression.New(typeMap.TypeConverterType);
                var convertMethod = typeMap.TypeConverterType.GetMethod("Convert");
                if (convertMethod != null)
                {
                    return Expression.Call(converterInstance, convertMethod, sourceParam, destParam, contextParam);
                }
            }

            throw new ConfigurationException($"无效的类型转换器配置 / Invalid type converter configuration: {typeMap.TypePair}");
        }

        private Expression BuildDestinationCreation(
            TypeMap typeMap,
            ParameterExpression sourceParam,
            ParameterExpression destParam,
            ParameterExpression contextParam,
            ParameterExpression destVar)
        {
            Expression createExpr;

            // 使用自定义构造表达式
            if (typeMap.CustomCtorExpression != null)
            {
                var paramCount = typeMap.CustomCtorExpression.Parameters.Count;
                createExpr = paramCount switch
                {
                    1 => Expression.Invoke(typeMap.CustomCtorExpression, sourceParam),
                    2 => Expression.Invoke(typeMap.CustomCtorExpression, sourceParam, contextParam),
                    _ => Expression.Invoke(typeMap.CustomCtorExpression, sourceParam)
                };
            }
            // 使用构造函数映射
            else if (typeMap.ConstructorMap != null && typeMap.ConstructorMap.CanResolve)
            {
                createExpr = BuildConstructorCall(typeMap.ConstructorMap, sourceParam, contextParam);
            }
            // 使用默认构造函数
            else
            {
                var defaultCtor = typeMap.DestinationType.GetConstructor(Type.EmptyTypes);
                if (defaultCtor != null)
                {
                    createExpr = Expression.New(defaultCtor);
                }
                else if (typeMap.DestinationType.IsValueType)
                {
                    createExpr = Expression.Default(typeMap.DestinationType);
                }
                else
                {
                    // 使用 Activator.CreateInstance
                    createExpr = Expression.Call(
                        typeof(Activator).GetMethod(nameof(Activator.CreateInstance), new[] { typeof(Type) }),
                        Expression.Constant(typeMap.DestinationType)
                    );
                    createExpr = Expression.Convert(createExpr, typeMap.DestinationType);
                }
            }

            // 如果传入了现有目标对象，使用它；否则创建新对象
            if (destParam.Type.IsValueType)
            {
                return Expression.Assign(destVar, createExpr);
            }

            return Expression.Assign(destVar,
                Expression.Condition(
                    Expression.Equal(destParam, Expression.Constant(null, typeMap.DestinationType)),
                    createExpr,
                    destParam
                )
            );
        }

        private Expression BuildConstructorCall(
            IConstructorMap constructorMap,
            ParameterExpression sourceParam,
            ParameterExpression contextParam)
        {
            var args = new List<Expression>();

            foreach (var paramMap in constructorMap.ParameterMaps)
            {
                Expression argExpr;

                if (paramMap.CustomMapExpression != null)
                {
                    argExpr = Expression.Invoke(paramMap.CustomMapExpression, sourceParam);
                }
                else if (paramMap.SourceMember != null)
                {
                    argExpr = ExpressionBuilder.BuildMemberAccess(sourceParam, paramMap.SourceMember);
                }
                else if (paramMap.HasDefaultValue)
                {
                    argExpr = Expression.Constant(paramMap.DefaultValue, paramMap.ParameterType);
                }
                else
                {
                    argExpr = Expression.Default(paramMap.ParameterType);
                }

                // 类型转换
                if (argExpr.Type != paramMap.ParameterType)
                {
                    argExpr = ExpressionBuilder.BuildTypeConversion(argExpr, paramMap.ParameterType);
                }

                args.Add(argExpr);
            }

            return Expression.New(constructorMap.Constructor, args);
        }

        private IEnumerable<Expression> BuildBeforeMapActions(
            TypeMap typeMap,
            ParameterExpression sourceParam,
            ParameterExpression destVar,
            ParameterExpression contextParam)
        {
            foreach (var action in typeMap.BeforeMapActions)
            {
                var paramCount = action.Parameters.Count;
                yield return paramCount switch
                {
                    2 => Expression.Invoke(action, sourceParam, destVar),
                    3 => Expression.Invoke(action, sourceParam, destVar, contextParam),
                    _ => Expression.Invoke(action, sourceParam, destVar)
                };
            }
        }

        private IEnumerable<Expression> BuildAfterMapActions(
            TypeMap typeMap,
            ParameterExpression sourceParam,
            ParameterExpression destVar,
            ParameterExpression contextParam)
        {
            foreach (var action in typeMap.AfterMapActions)
            {
                var paramCount = action.Parameters.Count;
                yield return paramCount switch
                {
                    2 => Expression.Invoke(action, sourceParam, destVar),
                    3 => Expression.Invoke(action, sourceParam, destVar, contextParam),
                    _ => Expression.Invoke(action, sourceParam, destVar)
                };
            }
        }

        private IEnumerable<Expression> BuildMemberMappings(
            TypeMap typeMap,
            ParameterExpression sourceParam,
            ParameterExpression destVar,
            ParameterExpression contextParam)
        {
            var mappings = new List<Expression>();

            // 按映射顺序排序
            var orderedMembers = typeMap.MemberMaps
                .Cast<MemberMap>()
                .Where(m => !m.IsIgnored)
                .OrderBy(m => m.MappingOrder);

            foreach (var memberMap in orderedMembers)
            {
                var mapping = BuildMemberMapping(memberMap, sourceParam, destVar, contextParam);
                if (mapping != null)
                {
                    mappings.Add(mapping);
                }
            }

            return mappings;
        }

        private Expression BuildMemberMapping(
            MemberMap memberMap,
            ParameterExpression sourceParam,
            ParameterExpression destVar,
            ParameterExpression contextParam)
        {
            // 构建前置条件检查
            Expression preConditionCheck = null;
            if (memberMap.PreConditionExpression != null)
            {
                preConditionCheck = Expression.Invoke(memberMap.PreConditionExpression, sourceParam);
            }

            // 获取源值表达式
            Expression sourceValue = BuildSourceValueExpression(memberMap, sourceParam, destVar, contextParam);
            if (sourceValue == null)
            {
                return null; // 无法映射
            }

            // 应用值转换器
            if (memberMap.HasValueConverter)
            {
                sourceValue = BuildValueConverterCall(memberMap, sourceValue, contextParam);
            }

            // 类型转换
            if (sourceValue.Type != memberMap.DestinationType)
            {
                sourceValue = ExpressionBuilder.BuildTypeConversion(sourceValue, memberMap.DestinationType);
            }

            // 空值替换
            if (memberMap.HasNullSubstitute && !memberMap.DestinationType.IsValueType)
            {
                sourceValue = Expression.Coalesce(
                    sourceValue,
                    Expression.Constant(memberMap.NullSubstitute, memberMap.DestinationType)
                );
            }

            // 构建赋值表达式
            Expression assignment = ExpressionBuilder.BuildMemberAssignment(destVar, memberMap.DestinationMember, sourceValue);

            // 条件映射
            if (memberMap.ConditionExpression != null)
            {
                var condition = Expression.Invoke(memberMap.ConditionExpression, sourceParam);
                assignment = Expression.IfThen(condition, assignment);
            }

            // 前置条件包装
            if (preConditionCheck != null)
            {
                assignment = Expression.IfThen(preConditionCheck, assignment);
            }

            return assignment;
        }

        private Expression BuildSourceValueExpression(
            MemberMap memberMap,
            ParameterExpression sourceParam,
            ParameterExpression destVar,
            ParameterExpression contextParam)
        {
            // 自定义映射表达式
            if (memberMap.CustomMapExpression != null)
            {
                return BuildCustomMapInvocation(memberMap.CustomMapExpression, sourceParam, destVar, contextParam);
            }

            // 值解析器
            if (memberMap.ValueResolverType != null)
            {
                return BuildValueResolverCall(memberMap, sourceParam, destVar, contextParam);
            }

            // 源成员链
            if (memberMap.SourceMembers != null && memberMap.SourceMembers.Length > 0)
            {
                return ExpressionBuilder.BuildMemberChainAccess(sourceParam, memberMap.SourceMembers);
            }

            return null;
        }

        #endregion

        #region 辅助方法 / Helper Methods

        private Expression BuildCustomMapInvocation(
            LambdaExpression customMap,
            ParameterExpression sourceParam,
            ParameterExpression destVar,
            ParameterExpression contextParam)
        {
            var paramCount = customMap.Parameters.Count;
            return paramCount switch
            {
                1 => Expression.Invoke(customMap, sourceParam),
                2 => Expression.Invoke(customMap, sourceParam, destVar),
                3 => Expression.Invoke(customMap, sourceParam, destVar, contextParam),
                _ => Expression.Invoke(customMap, sourceParam, destVar, Expression.Constant(null), contextParam)
            };
        }

        private Expression BuildValueResolverCall(
            MemberMap memberMap,
            ParameterExpression sourceParam,
            ParameterExpression destVar,
            ParameterExpression contextParam)
        {
            var resolverType = memberMap.ValueResolverType;

            // 创建解析器实例
            var resolverInstance = Expression.New(resolverType);

            // 查找 Resolve 方法
            var resolveMethod = resolverType.GetMethod("Resolve");
            if (resolveMethod == null)
            {
                throw new ConfigurationException(
                    $"值解析器缺少 Resolve 方法 / Value resolver missing Resolve method: {resolverType.Name}");
            }

            // 获取当前目标成员值
            var currentDestValue = ExpressionBuilder.BuildMemberAccess(destVar, memberMap.DestinationMember);

            // 调用 Resolve 方法
            return Expression.Call(resolverInstance, resolveMethod, sourceParam, destVar, currentDestValue, contextParam);
        }

        private Expression BuildValueConverterCall(
            MemberMap memberMap,
            Expression sourceValue,
            ParameterExpression contextParam)
        {
            // 使用值转换表达式
            if (memberMap.ValueConverterExpression != null)
            {
                var paramCount = memberMap.ValueConverterExpression.Parameters.Count;
                return paramCount switch
                {
                    1 => Expression.Invoke(memberMap.ValueConverterExpression, sourceValue),
                    2 => Expression.Invoke(memberMap.ValueConverterExpression, sourceValue, contextParam),
                    _ => Expression.Invoke(memberMap.ValueConverterExpression, sourceValue)
                };
            }

            // 使用值转换器类型
            if (memberMap.ValueConverterType != null)
            {
                var converterInstance = Expression.New(memberMap.ValueConverterType);
                var convertMethod = memberMap.ValueConverterType.GetMethod("Convert");
                if (convertMethod != null)
                {
                    return Expression.Call(converterInstance, convertMethod, sourceValue, contextParam);
                }
            }

            return sourceValue;
        }

        #endregion
    }
}
