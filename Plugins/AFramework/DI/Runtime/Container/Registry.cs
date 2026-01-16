// ==========================================================
// 文件名：Registry.cs
// 命名空间: AFramework.DI
// 依赖: System, System.Collections.Generic, System.Runtime.CompilerServices
// 功能: 实现注册表，存储和管理所有服务注册信息
// 优化: 使用高性能哈希表优化类型查找，热路径内联优化
// 增强: 支持开放泛型注册和内置集合解析
// ==========================================================

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace AFramework.DI
{
    /// <summary>
    /// 注册表
    /// <para>存储和管理所有服务注册信息，提供高效的类型查找能力</para>
    /// <para>Registry that stores and manages all service registrations</para>
    /// </summary>
    /// <remarks>
    /// 性能优化：
    /// <list type="bullet">
    /// <item>构建完成后使用 TypeKeyHashtable 进行只读查找</item>
    /// <item>热路径方法使用 AggressiveInlining 优化</item>
    /// <item>支持构建时和运行时两种模式</item>
    /// <item>支持开放泛型类型注册和运行时构造</item>
    /// </list>
    /// </remarks>
    internal sealed class Registry
    {
        #region 字段 / Fields

        // 构建阶段：按服务类型索引的注册信息（支持多个注册）
        private Dictionary<Type, List<IRegistration>> _registrationsByType;
        
        // 构建阶段：按键值索引的注册信息
        private Dictionary<(Type, object), IRegistration> _keyedRegistrations;
        
        // 所有注册信息列表
        private readonly List<IRegistration> _allRegistrations;

        // 运行时阶段：高性能类型查找哈希表
        private TypeKeyHashtable<IRegistration> _typeHashtable;
        
        // 运行时阶段：高性能键值查找哈希表
        private TypeKeyHashtable<IRegistration> _keyedHashtable;

        // 开放泛型提供者列表
        private readonly List<OpenGenericProvider> _openGenericProviders;

        // 是否已冻结（构建完成）
        private bool _isFrozen;

        #endregion

        #region 构造函数 / Constructor

        /// <summary>
        /// 创建注册表实例
        /// </summary>
        public Registry()
        {
            _registrationsByType = new Dictionary<Type, List<IRegistration>>();
            _keyedRegistrations = new Dictionary<(Type, object), IRegistration>();
            _allRegistrations = new List<IRegistration>();
            _openGenericProviders = new List<OpenGenericProvider>();
            _isFrozen = false;
        }

        /// <summary>
        /// 从注册信息集合创建注册表
        /// </summary>
        public Registry(IEnumerable<IRegistration> registrations) : this()
        {
            if (registrations == null) return;

            foreach (var registration in registrations)
            {
                Add(registration);
            }
        }

        #endregion

        #region 添加注册 / Add Registration

        /// <summary>
        /// 添加注册信息
        /// </summary>
        public void Add(IRegistration registration)
        {
            if (registration == null)
                throw new ArgumentNullException(nameof(registration));

            if (_isFrozen)
                throw new InvalidOperationException(
                    "注册表已冻结，无法添加新注册。\n" +
                    "Registry is frozen, cannot add new registrations.");

            _allRegistrations.Add(registration);

            // 为所有服务类型建立索引
            foreach (var serviceType in registration.ServiceTypes)
            {
                if (!_registrationsByType.TryGetValue(serviceType, out var list))
                {
                    list = new List<IRegistration>();
                    _registrationsByType[serviceType] = list;
                }
                list.Add(registration);

                // 如果有键值，建立键值索引
                if (registration.HasKey)
                {
                    var key = (serviceType, registration.Key);
                    _keyedRegistrations[key] = registration;
                }
            }
        }

        /// <summary>
        /// 添加开放泛型注册
        /// <para>Add open generic registration</para>
        /// </summary>
        /// <param name="openServiceType">开放泛型服务类型 / Open generic service type</param>
        /// <param name="openImplementationType">开放泛型实现类型 / Open generic implementation type</param>
        /// <param name="lifetime">生命周期 / Lifetime</param>
        public void AddOpenGeneric(Type openServiceType, Type openImplementationType, Lifetime lifetime)
        {
            if (_isFrozen)
                throw new InvalidOperationException(
                    "注册表已冻结，无法添加新注册。\n" +
                    "Registry is frozen, cannot add new registrations.");

            var provider = new OpenGenericProvider(openServiceType, openImplementationType, lifetime);
            _openGenericProviders.Add(provider);
        }

        /// <summary>
        /// 冻结注册表，构建高性能查找结构
        /// <para>Freeze the registry and build high-performance lookup structures</para>
        /// </summary>
        public void Freeze()
        {
            if (_isFrozen) return;

            // 构建类型查找哈希表
            var typeEntries = new List<KeyValuePair<(Type, object), IRegistration>>();
            foreach (var kvp in _registrationsByType)
            {
                var list = kvp.Value;
                if (list.Count > 0)
                {
                    // 使用最后一个注册（后注册覆盖先注册）
                    typeEntries.Add(new KeyValuePair<(Type, object), IRegistration>(
                        (kvp.Key, null), list[list.Count - 1]));
                }
            }
            _typeHashtable = new TypeKeyHashtable<IRegistration>(typeEntries);

            // 构建键值查找哈希表
            var keyedEntries = new List<KeyValuePair<(Type, object), IRegistration>>();
            foreach (var kvp in _keyedRegistrations)
            {
                keyedEntries.Add(new KeyValuePair<(Type, object), IRegistration>(kvp.Key, kvp.Value));
            }
            _keyedHashtable = new TypeKeyHashtable<IRegistration>(keyedEntries);

            // 清理构建阶段的数据结构，释放内存
            _registrationsByType = null;
            _keyedRegistrations = null;

            _isFrozen = true;
        }

        #endregion

        #region 查找注册 / Find Registration

        /// <summary>
        /// 获取指定类型的注册信息
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IRegistration Get(Type serviceType)
        {
            if (_isFrozen)
            {
                // 运行时：使用高性能哈希表
                return _typeHashtable.TryGet(serviceType, out var registration) ? registration : null;
            }
            
            // 构建阶段：使用 Dictionary
            if (_registrationsByType.TryGetValue(serviceType, out var list) && list.Count > 0)
            {
                return list[list.Count - 1];
            }
            return null;
        }

        /// <summary>
        /// 获取指定类型的所有注册信息
        /// </summary>
        public IReadOnlyList<IRegistration> GetAll(Type serviceType)
        {
            if (_isFrozen)
            {
                // 运行时：遍历所有注册查找匹配项
                var results = new List<IRegistration>();
                foreach (var reg in _allRegistrations)
                {
                    foreach (var type in reg.ServiceTypes)
                    {
                        if (type == serviceType)
                        {
                            results.Add(reg);
                            break;
                        }
                    }
                }
                return results;
            }

            // 构建阶段：使用 Dictionary
            if (_registrationsByType.TryGetValue(serviceType, out var list))
            {
                return list;
            }
            return Array.Empty<IRegistration>();
        }

        /// <summary>
        /// 获取指定键值的注册信息
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IRegistration GetKeyed(Type serviceType, object key)
        {
            if (_isFrozen)
            {
                // 运行时：使用高性能哈希表
                return _keyedHashtable.TryGet(serviceType, key, out var registration) ? registration : null;
            }

            // 构建阶段：使用 Dictionary
            if (_keyedRegistrations.TryGetValue((serviceType, key), out var reg))
            {
                return reg;
            }
            return null;
        }

        /// <summary>
        /// 尝试获取指定类型的注册信息
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryGet(Type serviceType, out IRegistration registration)
        {
            registration = Get(serviceType);
            return registration != null;
        }

        /// <summary>
        /// 尝试获取指定键值的注册信息
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryGetKeyed(Type serviceType, object key, out IRegistration registration)
        {
            registration = GetKeyed(serviceType, key);
            return registration != null;
        }

        #endregion

        #region 检查注册 / Check Registration

        /// <summary>
        /// 检查指定类型是否已注册
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains(Type serviceType)
        {
            if (_isFrozen)
            {
                return _typeHashtable.Contains(serviceType);
            }
            return _registrationsByType.ContainsKey(serviceType);
        }

        /// <summary>
        /// 检查指定键值是否已注册
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ContainsKeyed(Type serviceType, object key)
        {
            if (_isFrozen)
            {
                return _keyedHashtable.Contains(serviceType, key);
            }
            return _keyedRegistrations.ContainsKey((serviceType, key));
        }

        #endregion

        #region 开放泛型查找 / Open Generic Lookup

        /// <summary>
        /// 尝试获取开放泛型注册
        /// <para>Try to get open generic registration for a closed generic type</para>
        /// </summary>
        /// <param name="closedServiceType">封闭泛型服务类型 / Closed generic service type</param>
        /// <param name="registration">输出的注册信息 / Output registration</param>
        /// <returns>是否找到匹配的开放泛型注册 / Whether found matching open generic registration</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryGetOpenGeneric(Type closedServiceType, out IRegistration registration)
        {
            registration = null;

            // 检查是否是泛型类型
            if (!closedServiceType.IsGenericType)
                return false;

            var typeArguments = closedServiceType.GetGenericArguments();

            // 遍历开放泛型提供者查找匹配项
            foreach (var provider in _openGenericProviders)
            {
                if (provider.CanHandle(closedServiceType))
                {
                    registration = provider.GetClosedRegistration(closedServiceType, typeArguments);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 检查是否有匹配的开放泛型注册
        /// <para>Check if there is a matching open generic registration</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ContainsOpenGeneric(Type closedServiceType)
        {
            if (!closedServiceType.IsGenericType)
                return false;

            foreach (var provider in _openGenericProviders)
            {
                if (provider.CanHandle(closedServiceType))
                    return true;
            }

            return false;
        }

        #endregion

        #region 属性 / Properties

        /// <summary>
        /// 获取所有注册信息
        /// </summary>
        public IReadOnlyList<IRegistration> AllRegistrations => _allRegistrations;

        /// <summary>
        /// 获取注册数量
        /// </summary>
        public int Count => _allRegistrations.Count;

        /// <summary>
        /// 是否已冻结
        /// </summary>
        public bool IsFrozen => _isFrozen;

        /// <summary>
        /// 获取开放泛型提供者数量
        /// </summary>
        public int OpenGenericCount => _openGenericProviders.Count;

        #endregion
    }
}
