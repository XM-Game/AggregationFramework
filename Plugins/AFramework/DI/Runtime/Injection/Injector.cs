// ==========================================================
// 文件名：Injector.cs
// 命名空间: AFramework.DI
// 依赖: System, System.Collections.Generic
// 功能: 实现注入器接口，提供完整的依赖注入能力

// ==========================================================

using System;
using System.Collections.Generic;

namespace AFramework.DI
{
    /// <summary>
    /// 注入器实现类
    /// <para>提供完整的依赖注入能力，包括构造函数、字段、属性和方法注入</para>
    /// <para>Injector implementation that provides complete dependency injection capabilities</para>
    /// </summary>
    /// <remarks>
    /// 注入执行顺序：
    /// <list type="number">
    /// <item>构造函数注入（创建实例时）</item>
    /// <item>字段注入</item>
    /// <item>属性注入</item>
    /// <item>方法注入（按 Order 排序）</item>
    /// </list>
    /// </remarks>
    public sealed class Injector : IInjector
    {
        #region 单例 / Singleton

        private static readonly Lazy<Injector> _instance = 
            new Lazy<Injector>(() => new Injector());

        /// <summary>
        /// 获取注入器实例
        /// <para>Get the injector instance</para>
        /// </summary>
        public static Injector Instance => _instance.Value;

        #endregion

        #region 字段 / Fields

        private readonly InjectionInfoCache _infoCache;
        private readonly ConstructorInjector _constructorInjector;
        private readonly FieldInjector _fieldInjector;
        private readonly PropertyInjector _propertyInjector;
        private readonly MethodInjector _methodInjector;

        #endregion

        #region 构造函数 / Constructor

        private Injector()
        {
            _infoCache = InjectionInfoCache.Instance;
            _constructorInjector = ConstructorInjector.Instance;
            _fieldInjector = FieldInjector.Instance;
            _propertyInjector = PropertyInjector.Instance;
            _methodInjector = MethodInjector.Instance;
        }

        #endregion

        #region IInjector 实现 / IInjector Implementation

        /// <inheritdoc/>
        public object CreateInstance(Type type, IObjectResolver resolver)
        {
            return CreateInstance(type, resolver, null);
        }

        /// <inheritdoc/>
        public object CreateInstance(Type type, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));
            if (resolver == null)
                throw new ArgumentNullException(nameof(resolver));

            // 验证类型可实例化
            if (type.IsAbstract || type.IsInterface)
            {
                throw ResolutionException.CannotInstantiateAbstract(type);
            }

            // 获取注入信息
            var injectionInfo = _infoCache.GetOrCreate(type);

            // 构造函数注入
            object instance;
            if (injectionInfo.Constructor != null)
            {
                instance = _constructorInjector.CreateInstance(
                    injectionInfo.Constructor, 
                    resolver, 
                    parameters);
            }
            else
            {
                // 无参构造
                try
                {
                    instance = Activator.CreateInstance(type, true);
                }
                catch (Exception ex)
                {
                    throw new ResolutionException(
                        $"创建类型 {type.Name} 的实例失败，没有可用的构造函数。\n" +
                        $"Failed to create instance of type {type.Name}, no available constructor.",
                        ex);
                }
            }

            // 成员注入
            InjectMembers(instance, injectionInfo, resolver, parameters);

            return instance;
        }

        /// <inheritdoc/>
        public void Inject(object instance, IObjectResolver resolver)
        {
            Inject(instance, resolver, null);
        }

        /// <inheritdoc/>
        public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
        {
            if (instance == null)
                throw new ArgumentNullException(nameof(instance));
            if (resolver == null)
                throw new ArgumentNullException(nameof(resolver));

            var type = instance.GetType();
            var injectionInfo = _infoCache.GetOrCreate(type);

            InjectMembers(instance, injectionInfo, resolver, parameters);
        }

        /// <inheritdoc/>
        public InjectionInfo GetInjectionInfo(Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            return _infoCache.GetOrCreate(type);
        }

        /// <inheritdoc/>
        public bool RequiresInjection(Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            var info = _infoCache.GetOrCreate(type);
            return info.HasInjectionPoints;
        }

        #endregion

        #region 私有方法 / Private Methods

        /// <summary>
        /// 注入成员（字段、属性、方法）
        /// </summary>
        private void InjectMembers(
            object instance,
            InjectionInfo injectionInfo,
            IObjectResolver resolver,
            IReadOnlyList<IInjectParameter> parameters)
        {
            // 字段注入
            if (injectionInfo.Fields.Count > 0)
            {
                _fieldInjector.Inject(instance, injectionInfo.Fields, resolver, parameters);
            }

            // 属性注入
            if (injectionInfo.Properties.Count > 0)
            {
                _propertyInjector.Inject(instance, injectionInfo.Properties, resolver, parameters);
            }

            // 方法注入
            if (injectionInfo.Methods.Count > 0)
            {
                _methodInjector.Inject(instance, injectionInfo.Methods, resolver, parameters);
            }
        }

        #endregion

        #region 静态辅助方法 / Static Helper Methods

        /// <summary>
        /// 创建实例（静态便捷方法）
        /// <para>Create instance (static convenience method)</para>
        /// </summary>
        public static T Create<T>(IObjectResolver resolver)
        {
            return (T)Instance.CreateInstance(typeof(T), resolver);
        }

        /// <summary>
        /// 创建实例（静态便捷方法）
        /// <para>Create instance (static convenience method)</para>
        /// </summary>
        public static T Create<T>(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
        {
            return (T)Instance.CreateInstance(typeof(T), resolver, parameters);
        }

        /// <summary>
        /// 注入依赖（静态便捷方法）
        /// <para>Inject dependencies (static convenience method)</para>
        /// </summary>
        public static void InjectInto(object instance, IObjectResolver resolver)
        {
            Instance.Inject(instance, resolver);
        }

        /// <summary>
        /// 注入依赖（静态便捷方法）
        /// <para>Inject dependencies (static convenience method)</para>
        /// </summary>
        public static void InjectInto(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
        {
            Instance.Inject(instance, resolver, parameters);
        }

        /// <summary>
        /// 清除注入信息缓存
        /// <para>Clear injection info cache</para>
        /// </summary>
        public static void ClearCache()
        {
            InjectionInfoCache.Instance.Clear();
        }

        #endregion
    }
}
