// ==========================================================
// 文件名：Registration.cs
// 命名空间: AFramework.DI
// 依赖: System, System.Collections.Generic
// 功能: 实现服务注册信息，描述服务的类型、生命周期和提供者

// ==========================================================

using System;
using System.Collections.Generic;

namespace AFramework.DI
{
    /// <summary>
    /// 注册信息实现类
    /// <para>描述一个服务注册的完整信息，包括服务类型、实现类型、生命周期和实例提供者</para>
    /// <para>Registration implementation that describes complete information of a service registration</para>
    /// </summary>
    /// <remarks>
    /// 注册信息是不可变的，一旦创建就不能修改。
    /// 这确保了容器构建后注册配置的稳定性。
    /// </remarks>
    public sealed class Registration : IRegistration
    {
        #region 字段 / Fields

        private readonly List<Type> _serviceTypes;
        private readonly List<IInjectParameter> _parameters;
        private readonly Dictionary<string, object> _metadata;

        #endregion

        #region 属性 / Properties

        /// <inheritdoc/>
        public Type ServiceType { get; }

        /// <inheritdoc/>
        public Type ImplementationType { get; }

        /// <inheritdoc/>
        public IReadOnlyList<Type> ServiceTypes => _serviceTypes;

        /// <inheritdoc/>
        public Lifetime Lifetime { get; }

        /// <inheritdoc/>
        public object Key { get; }

        /// <inheritdoc/>
        public bool HasKey => Key != null;

        /// <inheritdoc/>
        public IInstanceProvider Provider { get; internal set; }

        /// <inheritdoc/>
        public IReadOnlyList<IInjectParameter> Parameters => _parameters;

        /// <inheritdoc/>
        public IReadOnlyDictionary<string, object> Metadata => _metadata;

        /// <summary>
        /// 获取工厂函数（如果有）
        /// <para>Get the factory function if any</para>
        /// </summary>
        internal Func<IObjectResolver, object> Factory { get; }

        /// <summary>
        /// 获取现有实例（如果有）
        /// <para>Get the existing instance if any</para>
        /// </summary>
        internal object ExistingInstance { get; }

        /// <summary>
        /// 获取激活回调列表
        /// <para>Get the list of activation callbacks</para>
        /// </summary>
        internal IReadOnlyList<Action<object>> OnActivatedCallbacks { get; }

        #endregion

        #region 构造函数 / Constructor

        /// <summary>
        /// 创建注册信息实例
        /// </summary>
        internal Registration(
            Type serviceType,
            Type implementationType,
            Lifetime lifetime,
            object key,
            Func<IObjectResolver, object> factory,
            object existingInstance,
            IEnumerable<Type> serviceTypes,
            IEnumerable<IInjectParameter> parameters,
            IDictionary<string, object> metadata,
            IEnumerable<Action<object>> onActivatedCallbacks)
        {
            ServiceType = serviceType;
            ImplementationType = implementationType;
            Lifetime = lifetime;
            Key = key;
            Factory = factory;
            ExistingInstance = existingInstance;

            _serviceTypes = serviceTypes != null 
                ? new List<Type>(serviceTypes) 
                : new List<Type>();
            
            // 确保主服务类型在列表中
            if (serviceType != null && !_serviceTypes.Contains(serviceType))
            {
                _serviceTypes.Insert(0, serviceType);
            }

            _parameters = parameters != null 
                ? new List<IInjectParameter>(parameters) 
                : new List<IInjectParameter>();
            
            _metadata = metadata != null 
                ? new Dictionary<string, object>(metadata) 
                : new Dictionary<string, object>();

            OnActivatedCallbacks = onActivatedCallbacks != null
                ? new List<Action<object>>(onActivatedCallbacks)
                : Array.Empty<Action<object>>();
        }

        #endregion

        #region 元数据方法 / Metadata Methods

        /// <inheritdoc/>
        public T GetMetadata<T>(string key)
        {
            if (_metadata.TryGetValue(key, out var value))
            {
                return (T)value;
            }
            return default;
        }

        /// <inheritdoc/>
        public bool TryGetMetadata<T>(string key, out T value)
        {
            if (_metadata.TryGetValue(key, out var obj))
            {
                value = (T)obj;
                return true;
            }
            value = default;
            return false;
        }

        #endregion

        #region 诊断 / Diagnostics

        /// <summary>
        /// 获取注册信息的字符串表示
        /// </summary>
        public override string ToString()
        {
            var keyInfo = HasKey ? $", Key={Key}" : "";
            return $"Registration[{ServiceType?.Name} -> {ImplementationType?.Name}, {Lifetime}{keyInfo}]";
        }

        #endregion
    }
}
