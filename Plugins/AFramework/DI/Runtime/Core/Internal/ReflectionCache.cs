// ==========================================================
// 文件名：ReflectionCache.cs
// 命名空间: AFramework.DI.Internal
// 依赖: System, System.Reflection, System.Collections.Generic
// ==========================================================

using System;
using System.Collections.Generic;
using System.Reflection;

namespace AFramework.DI.Internal
{
    /// <summary>
    /// 反射缓存
    /// <para>缓存反射信息以提高性能，避免重复的反射操作</para>
    /// </summary>
    internal static class ReflectionCache
    {
        #region 缓存实例

        /// <summary>
        /// 构造函数缓存
        /// </summary>
        private static readonly TypeCache<ConstructorInfo> ConstructorCache = new();

        /// <summary>
        /// 可注入字段缓存
        /// </summary>
        private static readonly TypeCache<List<FieldInfo>> InjectableFieldsCache = new();

        /// <summary>
        /// 可注入属性缓存
        /// </summary>
        private static readonly TypeCache<List<PropertyInfo>> InjectablePropertiesCache = new();

        /// <summary>
        /// 可注入方法缓存
        /// </summary>
        private static readonly TypeCache<List<MethodInfo>> InjectableMethodsCache = new();

        /// <summary>
        /// 类型注入信息缓存
        /// </summary>
        private static readonly TypeCache<TypeInjectionInfo> InjectionInfoCache = new();

        /// <summary>
        /// 参数类型缓存
        /// </summary>
        private static readonly ConcurrentCache<ConstructorInfo, Type[]> ParameterTypesCache = new();

        #endregion

        #region 构造函数缓存

        /// <summary>
        /// 获取缓存的最佳构造函数
        /// </summary>
        /// <param name="type">目标类型</param>
        /// <returns>最佳构造函数</returns>
        public static ConstructorInfo GetBestConstructor(Type type)
        {
            return ConstructorCache.GetOrAdd(type, ReflectionHelper.GetBestConstructor);
        }

        /// <summary>
        /// 获取缓存的构造函数参数类型
        /// </summary>
        /// <param name="constructor">构造函数</param>
        /// <returns>参数类型数组</returns>
        public static Type[] GetParameterTypes(ConstructorInfo constructor)
        {
            if (constructor == null) return Array.Empty<Type>();
            return ParameterTypesCache.GetOrAdd(constructor, ReflectionHelper.GetParameterTypes);
        }

        #endregion

        #region 可注入成员缓存

        /// <summary>
        /// 获取缓存的可注入字段
        /// </summary>
        /// <param name="type">目标类型</param>
        /// <returns>可注入字段列表</returns>
        public static List<FieldInfo> GetInjectableFields(Type type)
        {
            return InjectableFieldsCache.GetOrAdd(type, ReflectionHelper.GetInjectableFields);
        }

        /// <summary>
        /// 获取缓存的可注入属性
        /// </summary>
        /// <param name="type">目标类型</param>
        /// <returns>可注入属性列表</returns>
        public static List<PropertyInfo> GetInjectableProperties(Type type)
        {
            return InjectablePropertiesCache.GetOrAdd(type, ReflectionHelper.GetInjectableProperties);
        }

        /// <summary>
        /// 获取缓存的可注入方法
        /// </summary>
        /// <param name="type">目标类型</param>
        /// <returns>可注入方法列表</returns>
        public static List<MethodInfo> GetInjectableMethods(Type type)
        {
            return InjectableMethodsCache.GetOrAdd(type, ReflectionHelper.GetInjectableMethods);
        }

        #endregion

        #region 类型注入信息

        /// <summary>
        /// 获取类型的完整注入信息
        /// </summary>
        /// <param name="type">目标类型</param>
        /// <returns>类型注入信息</returns>
        public static TypeInjectionInfo GetInjectionInfo(Type type)
        {
            return InjectionInfoCache.GetOrAdd(type, CreateInjectionInfo);
        }

        /// <summary>
        /// 创建类型注入信息
        /// </summary>
        private static TypeInjectionInfo CreateInjectionInfo(Type type)
        {
            return new TypeInjectionInfo(
                type,
                ReflectionHelper.GetBestConstructor(type),
                ReflectionHelper.GetInjectableFields(type),
                ReflectionHelper.GetInjectableProperties(type),
                ReflectionHelper.GetInjectableMethods(type)
            );
        }

        #endregion

        #region 缓存管理

        /// <summary>
        /// 清空所有缓存
        /// </summary>
        public static void ClearAll()
        {
            ConstructorCache.Clear();
            InjectableFieldsCache.Clear();
            InjectablePropertiesCache.Clear();
            InjectableMethodsCache.Clear();
            InjectionInfoCache.Clear();
            ParameterTypesCache.Clear();
        }

        /// <summary>
        /// 清空指定类型的缓存
        /// </summary>
        /// <param name="type">目标类型</param>
        public static void ClearType(Type type)
        {
            // 由于 TypeCache 不支持单独移除，这里只能清空全部
            // 在实际使用中，通常不需要清空单个类型的缓存
            ClearAll();
        }

        /// <summary>
        /// 获取缓存统计信息
        /// </summary>
        /// <returns>缓存统计</returns>
        public static CacheStatistics GetStatistics()
        {
            return new CacheStatistics
            {
                ConstructorCacheCount = ConstructorCache.Count,
                FieldsCacheCount = InjectableFieldsCache.Count,
                PropertiesCacheCount = InjectablePropertiesCache.Count,
                MethodsCacheCount = InjectableMethodsCache.Count,
                InjectionInfoCacheCount = InjectionInfoCache.Count,
                ParameterTypesCacheCount = ParameterTypesCache.Count
            };
        }

        #endregion
    }

    #region 数据结构

    /// <summary>
    /// 类型注入信息
    /// <para>包含类型的所有注入相关信息</para>
    /// </summary>
    internal sealed class TypeInjectionInfo
    {
        /// <summary>
        /// 目标类型
        /// </summary>
        public Type Type { get; }

        /// <summary>
        /// 最佳构造函数
        /// </summary>
        public ConstructorInfo Constructor { get; }

        /// <summary>
        /// 构造函数参数
        /// </summary>
        public ParameterInfo[] ConstructorParameters { get; }

        /// <summary>
        /// 可注入字段
        /// </summary>
        public IReadOnlyList<FieldInfo> InjectableFields { get; }

        /// <summary>
        /// 可注入属性
        /// </summary>
        public IReadOnlyList<PropertyInfo> InjectableProperties { get; }

        /// <summary>
        /// 可注入方法
        /// </summary>
        public IReadOnlyList<MethodInfo> InjectableMethods { get; }

        /// <summary>
        /// 是否有可注入成员
        /// </summary>
        public bool HasInjectableMembers { get; }

        /// <summary>
        /// 是否需要后注入（字段、属性或方法注入）
        /// </summary>
        public bool RequiresPostInjection { get; }

        /// <summary>
        /// 创建类型注入信息
        /// </summary>
        public TypeInjectionInfo(
            Type type,
            ConstructorInfo constructor,
            List<FieldInfo> fields,
            List<PropertyInfo> properties,
            List<MethodInfo> methods)
        {
            Type = type;
            Constructor = constructor;
            ConstructorParameters = constructor?.GetParameters() ?? Array.Empty<ParameterInfo>();
            InjectableFields = fields ?? new List<FieldInfo>();
            InjectableProperties = properties ?? new List<PropertyInfo>();
            InjectableMethods = methods ?? new List<MethodInfo>();

            HasInjectableMembers = InjectableFields.Count > 0 ||
                                   InjectableProperties.Count > 0 ||
                                   InjectableMethods.Count > 0;

            RequiresPostInjection = HasInjectableMembers;
        }
    }

    /// <summary>
    /// 缓存统计信息
    /// </summary>
    internal struct CacheStatistics
    {
        /// <summary>
        /// 构造函数缓存数量
        /// </summary>
        public int ConstructorCacheCount;

        /// <summary>
        /// 字段缓存数量
        /// </summary>
        public int FieldsCacheCount;

        /// <summary>
        /// 属性缓存数量
        /// </summary>
        public int PropertiesCacheCount;

        /// <summary>
        /// 方法缓存数量
        /// </summary>
        public int MethodsCacheCount;

        /// <summary>
        /// 注入信息缓存数量
        /// </summary>
        public int InjectionInfoCacheCount;

        /// <summary>
        /// 参数类型缓存数量
        /// </summary>
        public int ParameterTypesCacheCount;

        /// <summary>
        /// 总缓存数量
        /// </summary>
        public readonly int TotalCount =>
            ConstructorCacheCount +
            FieldsCacheCount +
            PropertiesCacheCount +
            MethodsCacheCount +
            InjectionInfoCacheCount +
            ParameterTypesCacheCount;

        /// <summary>
        /// 转换为字符串
        /// </summary>
        public override readonly string ToString()
        {
            return $"ReflectionCache Statistics:\n" +
                   $"  Constructors: {ConstructorCacheCount}\n" +
                   $"  Fields: {FieldsCacheCount}\n" +
                   $"  Properties: {PropertiesCacheCount}\n" +
                   $"  Methods: {MethodsCacheCount}\n" +
                   $"  InjectionInfo: {InjectionInfoCacheCount}\n" +
                   $"  ParameterTypes: {ParameterTypesCacheCount}\n" +
                   $"  Total: {TotalCount}";
        }
    }

    #endregion
}
