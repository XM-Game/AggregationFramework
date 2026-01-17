// ==========================================================
// 文件名：MapperRegistry.cs
// 命名空间: AFramework.AMapper
// 依赖: System, System.Collections.Generic
// 功能: 映射器注册表，管理内置类型映射器
// ==========================================================

using System;
using System.Collections.Generic;
using System.Linq;

namespace AFramework.AMapper
{
    /// <summary>
    /// 映射器注册表
    /// <para>管理和查找内置类型映射器</para>
    /// <para>Mapper registry for managing and finding built-in type mappers</para>
    /// </summary>
    /// <remarks>
    /// 设计原则：
    /// <list type="bullet">
    /// <item>单一职责：仅负责映射器的注册和查找</item>
    /// <item>开闭原则：支持注册自定义映射器扩展</item>
    /// <item>责任链模式：按优先级顺序查找匹配的映射器</item>
    /// </list>
    /// 
    /// 内置映射器优先级（从高到低）：
    /// 1. 可赋值类型映射器（AssignableMapper）
    /// 2. 可空类型映射器（NullableSourceMapper、NullableDestinationMapper）
    /// 3. 枚举映射器（EnumToEnumMapper、StringToEnumMapper、EnumToStringMapper）
    /// 4. 集合映射器（ArrayMapper、ListMapper、DictionaryMapper、HashSetMapper、CollectionMapper）
    /// 5. 构造函数映射器（ConstructorMapper、ConversionOperatorMapper）
    /// 6. 动态类型映射器（DictionaryToObjectMapper、ObjectToDictionaryMapper）
    /// 7. 基元类型映射器（StringMapper、ParseStringMapper、ConvertMapper）
    /// </remarks>
    public sealed class MapperRegistry
    {
        #region 私有字段 / Private Fields

        private readonly List<IObjectMapper> _mappers;
        private readonly object _lock = new object();

        #endregion

        #region 单例 / Singleton

        private static readonly Lazy<MapperRegistry> _instance = new Lazy<MapperRegistry>(() => new MapperRegistry());

        /// <summary>
        /// 获取默认实例
        /// <para>Get default instance</para>
        /// </summary>
        public static MapperRegistry Default => _instance.Value;

        #endregion

        #region 构造函数 / Constructor

        /// <summary>
        /// 创建映射器注册表
        /// </summary>
        public MapperRegistry()
        {
            _mappers = new List<IObjectMapper>();
            RegisterBuiltinMappers();
        }

        #endregion

        #region 公共方法 / Public Methods

        /// <summary>
        /// 注册映射器
        /// <para>Register mapper</para>
        /// </summary>
        /// <param name="mapper">映射器 / Mapper</param>
        /// <exception cref="ArgumentNullException">当 mapper 为 null 时抛出</exception>
        public void Register(IObjectMapper mapper)
        {
            if (mapper == null)
                throw new ArgumentNullException(nameof(mapper));

            lock (_lock)
            {
                _mappers.Add(mapper);
            }
        }

        /// <summary>
        /// 在指定位置插入映射器
        /// <para>Insert mapper at specified index</para>
        /// </summary>
        /// <param name="index">索引 / Index</param>
        /// <param name="mapper">映射器 / Mapper</param>
        /// <exception cref="ArgumentNullException">当 mapper 为 null 时抛出</exception>
        /// <exception cref="ArgumentOutOfRangeException">当 index 超出范围时抛出</exception>
        public void Insert(int index, IObjectMapper mapper)
        {
            if (mapper == null)
                throw new ArgumentNullException(nameof(mapper));

            lock (_lock)
            {
                _mappers.Insert(index, mapper);
            }
        }

        /// <summary>
        /// 查找适用的映射器
        /// <para>Find applicable mapper</para>
        /// </summary>
        /// <param name="typePair">类型对 / Type pair</param>
        /// <returns>映射器或 null / Mapper or null</returns>
        public IObjectMapper FindMapper(TypePair typePair)
        {
            lock (_lock)
            {
                foreach (var mapper in _mappers)
                {
                    if (mapper.IsMatch(typePair))
                    {
                        return mapper;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// 查找适用的映射器
        /// <para>Find applicable mapper</para>
        /// </summary>
        /// <typeparam name="TSource">源类型 / Source type</typeparam>
        /// <typeparam name="TDestination">目标类型 / Destination type</typeparam>
        /// <returns>映射器或 null / Mapper or null</returns>
        public IObjectMapper FindMapper<TSource, TDestination>()
        {
            return FindMapper(TypePair.Create<TSource, TDestination>());
        }

        /// <summary>
        /// 获取所有映射器
        /// <para>Get all mappers</para>
        /// </summary>
        /// <returns>映射器列表 / Mapper list</returns>
        public IReadOnlyList<IObjectMapper> GetMappers()
        {
            lock (_lock)
            {
                return _mappers.ToArray();
            }
        }

        /// <summary>
        /// 清除所有映射器
        /// <para>Clear all mappers</para>
        /// </summary>
        public void Clear()
        {
            lock (_lock)
            {
                _mappers.Clear();
            }
        }

        /// <summary>
        /// 重置为默认映射器
        /// <para>Reset to default mappers</para>
        /// </summary>
        public void Reset()
        {
            lock (_lock)
            {
                _mappers.Clear();
                RegisterBuiltinMappers();
            }
        }

        #endregion

        #region 私有方法 / Private Methods

        /// <summary>
        /// 注册内置映射器（按优先级顺序）
        /// </summary>
        private void RegisterBuiltinMappers()
        {
            // 1. 可赋值类型映射器（最高优先级）
            _mappers.Add(new Primitive.AssignableMapper());

            // 2. 可空类型映射器
            _mappers.Add(new Nullable.NullableSourceMapper());
            _mappers.Add(new Nullable.NullableDestinationMapper());

            // 3. 枚举映射器
            _mappers.Add(new Enum.EnumToEnumMapper());
            _mappers.Add(new Enum.StringToEnumMapper());
            _mappers.Add(new Enum.EnumToStringMapper());

            // 4. 集合映射器
            _mappers.Add(new Collection.ArrayMapper());
            _mappers.Add(new Collection.ListMapper());
            _mappers.Add(new Collection.DictionaryMapper());
            _mappers.Add(new Collection.HashSetMapper());
            _mappers.Add(new Collection.CollectionMapper());

            // 5. 构造函数映射器
            _mappers.Add(new Constructor.ConversionOperatorMapper());
            _mappers.Add(new Constructor.ConstructorMapper());

            // 6. 动态类型映射器
            _mappers.Add(new Dynamic.DictionaryToObjectMapper());
            _mappers.Add(new Dynamic.ObjectToDictionaryMapper());

            // 7. 基元类型映射器（最低优先级）
            _mappers.Add(new Primitive.StringMapper());
            _mappers.Add(new Primitive.ParseStringMapper());
            _mappers.Add(new Primitive.ConvertMapper());
        }

        #endregion
    }
}
