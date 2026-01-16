// ==========================================================
// 文件名：ComponentProvider.cs
// 命名空间: AFramework.DI
// 依赖: System, System.Collections.Generic, UnityEngine
// 功能: Unity 组件实例提供者，支持 MonoBehaviour 组件的创建和查找
// ==========================================================

using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace AFramework.DI
{
    /// <summary>
    /// Unity 组件实例提供者
    /// <para>专门用于处理 Unity MonoBehaviour 组件的创建、查找和生命周期管理</para>
    /// <para>Unity component instance provider for MonoBehaviour creation, lookup and lifecycle management</para>
    /// </summary>
    /// <remarks>
    /// 特性：
    /// <list type="bullet">
    /// <item>组件创建：支持在指定 GameObject 上添加组件</item>
    /// <item>组件查找：支持从场景中查找现有组件</item>
    /// <item>依赖注入：创建后自动执行依赖注入</item>
    /// <item>生命周期：与 Unity 对象生命周期集成</item>
    /// </list>
    /// 
    /// 创建模式：
    /// <list type="bullet">
    /// <item>NewGameObject：创建新的 GameObject 并添加组件</item>
    /// <item>ExistingGameObject：在指定的 GameObject 上添加组件</item>
    /// <item>FindInScene：从场景中查找现有组件</item>
    /// <item>FindOrCreate：查找现有组件，不存在则创建</item>
    /// </list>
    /// </remarks>
    public sealed class ComponentProvider : IInstanceProvider
    {
        #region 枚举 / Enums

        /// <summary>
        /// 组件创建模式
        /// <para>Component creation mode</para>
        /// </summary>
        public enum CreationMode
        {
            /// <summary>
            /// 创建新的 GameObject 并添加组件
            /// <para>Create new GameObject and add component</para>
            /// </summary>
            NewGameObject,

            /// <summary>
            /// 在指定的 GameObject 上添加组件
            /// <para>Add component to specified GameObject</para>
            /// </summary>
            ExistingGameObject,

            /// <summary>
            /// 从场景中查找现有组件
            /// <para>Find existing component in scene</para>
            /// </summary>
            FindInScene,

            /// <summary>
            /// 查找现有组件，不存在则创建
            /// <para>Find existing component or create if not found</para>
            /// </summary>
            FindOrCreate
        }

        #endregion

        #region 字段 / Fields

        private readonly Type _componentType;
        private readonly Lifetime _lifetime;
        private readonly CreationMode _creationMode;
        private readonly Func<GameObject> _gameObjectProvider;
        private readonly string _gameObjectName;
        private readonly bool _dontDestroyOnLoad;
        private readonly IInjector _injector;
        private readonly IReadOnlyList<Action<object>> _onActivatedCallbacks;
        private readonly object _syncRoot = new object();

        // 缓存
        private Component _cachedInstance;
        private volatile bool _isCreated;

        #endregion

        #region 属性 / Properties

        /// <inheritdoc/>
        public Lifetime Lifetime => _lifetime;

        /// <inheritdoc/>
        public Type InstanceType => _componentType;

        /// <inheritdoc/>
        public bool HasInstance => _isCreated && _cachedInstance != null;

        /// <summary>
        /// 获取创建模式
        /// <para>Get the creation mode</para>
        /// </summary>
        public CreationMode Mode => _creationMode;

        #endregion

        #region 构造函数 / Constructor

        /// <summary>
        /// 创建组件提供者
        /// </summary>
        /// <param name="componentType">组件类型（必须继承自 Component）/ Component type</param>
        /// <param name="lifetime">生命周期 / Lifetime</param>
        /// <param name="creationMode">创建模式 / Creation mode</param>
        /// <param name="gameObjectProvider">GameObject 提供函数 / GameObject provider function</param>
        /// <param name="gameObjectName">新建 GameObject 的名称 / Name for new GameObject</param>
        /// <param name="dontDestroyOnLoad">是否设置 DontDestroyOnLoad / Whether to set DontDestroyOnLoad</param>
        /// <param name="injector">注入器 / Injector</param>
        /// <param name="onActivatedCallbacks">激活回调 / Activation callbacks</param>
        public ComponentProvider(
            Type componentType,
            Lifetime lifetime = Lifetime.Singleton,
            CreationMode creationMode = CreationMode.NewGameObject,
            Func<GameObject> gameObjectProvider = null,
            string gameObjectName = null,
            bool dontDestroyOnLoad = false,
            IInjector injector = null,
            IReadOnlyList<Action<object>> onActivatedCallbacks = null)
        {
            // 验证组件类型
            if (componentType == null)
                throw new ArgumentNullException(nameof(componentType));
            
            if (!typeof(Component).IsAssignableFrom(componentType))
            {
                throw new ArgumentException(
                    $"类型 {componentType.Name} 不是有效的 Unity 组件类型。\n" +
                    $"Type {componentType.Name} is not a valid Unity Component type.",
                    nameof(componentType));
            }

            _componentType = componentType;
            _lifetime = lifetime;
            _creationMode = creationMode;
            _gameObjectProvider = gameObjectProvider;
            _gameObjectName = gameObjectName ?? $"[DI] {componentType.Name}";
            _dontDestroyOnLoad = dontDestroyOnLoad;
            _injector = injector ?? Injector.Instance;
            _onActivatedCallbacks = onActivatedCallbacks ?? Array.Empty<Action<object>>();
        }

        #endregion

        #region IInstanceProvider 实现 / IInstanceProvider Implementation

        /// <inheritdoc/>
        public object GetInstance(IObjectResolver resolver)
        {
            return GetInstance(resolver, null);
        }

        /// <inheritdoc/>
        public object GetInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
        {
            if (resolver == null)
                throw new ArgumentNullException(nameof(resolver));

            // 瞬态：每次创建新实例
            if (_lifetime == Lifetime.Transient)
            {
                return CreateComponent(resolver, parameters);
            }

            // 检查缓存的实例是否仍然有效
            if (_isCreated)
            {
                // Unity 对象可能被销毁，需要检查
                if (_cachedInstance != null)
                {
                    return _cachedInstance;
                }
                // 实例已被销毁，重置状态
                _isCreated = false;
            }

            lock (_syncRoot)
            {
                if (_isCreated && _cachedInstance != null)
                {
                    return _cachedInstance;
                }

                _cachedInstance = CreateComponent(resolver, parameters);
                Thread.MemoryBarrier();
                _isCreated = true;

                return _cachedInstance;
            }
        }

        /// <inheritdoc/>
        public void DisposeInstance(object instance)
        {
            if (instance is Component component && component != null)
            {
                try
                {
                    // 对于 MonoBehaviour，销毁 GameObject 或组件
                    if (Application.isPlaying)
                    {
                        UnityEngine.Object.Destroy(component.gameObject);
                    }
                    else
                    {
                        UnityEngine.Object.DestroyImmediate(component.gameObject);
                    }
                }
                catch (Exception ex)
                {
                    UnityEngine.Debug.LogWarning(
                        $"[AFramework.DI] 销毁组件时发生异常: {ex.Message}\n" +
                        $"Exception destroying component: {ex.Message}");
                }
            }

            lock (_syncRoot)
            {
                if (ReferenceEquals(_cachedInstance, instance))
                {
                    _cachedInstance = null;
                    _isCreated = false;
                }
            }
        }

        /// <inheritdoc/>
        public string GetDiagnosticInfo()
        {
            var instanceInfo = _cachedInstance != null 
                ? $", GameObject={_cachedInstance.gameObject.name}" 
                : "";
            return $"ComponentProvider[Type={_componentType.Name}, Mode={_creationMode}, Lifetime={_lifetime}{instanceInfo}]";
        }

        #endregion

        #region 私有方法 / Private Methods

        /// <summary>
        /// 创建组件实例
        /// </summary>
        private Component CreateComponent(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
        {
            Component component = _creationMode switch
            {
                CreationMode.NewGameObject => CreateOnNewGameObject(),
                CreationMode.ExistingGameObject => CreateOnExistingGameObject(),
                CreationMode.FindInScene => FindComponentInScene(),
                CreationMode.FindOrCreate => FindOrCreateComponent(),
                _ => throw new InvalidOperationException($"未知的创建模式: {_creationMode}")
            };

            if (component == null)
            {
                throw new ResolutionException(
                    $"无法创建或查找组件 {_componentType.Name}。\n" +
                    $"Failed to create or find component {_componentType.Name}.");
            }

            // 执行依赖注入
            _injector.Inject(component, resolver, parameters);

            // 执行激活回调
            InvokeActivationCallbacks(component);

            return component;
        }

        /// <summary>
        /// 在新 GameObject 上创建组件
        /// </summary>
        private Component CreateOnNewGameObject()
        {
            var go = new GameObject(_gameObjectName);
            
            if (_dontDestroyOnLoad)
            {
                UnityEngine.Object.DontDestroyOnLoad(go);
            }

            return go.AddComponent(_componentType);
        }

        /// <summary>
        /// 在现有 GameObject 上创建组件
        /// </summary>
        private Component CreateOnExistingGameObject()
        {
            if (_gameObjectProvider == null)
            {
                throw new InvalidOperationException(
                    "使用 ExistingGameObject 模式时必须提供 gameObjectProvider。\n" +
                    "gameObjectProvider is required when using ExistingGameObject mode.");
            }

            var go = _gameObjectProvider();
            if (go == null)
            {
                throw new InvalidOperationException(
                    "gameObjectProvider 返回了 null。\n" +
                    "gameObjectProvider returned null.");
            }

            return go.AddComponent(_componentType);
        }

        /// <summary>
        /// 从场景中查找组件
        /// </summary>
        private Component FindComponentInScene()
        {
#if UNITY_2023_1_OR_NEWER
            return UnityEngine.Object.FindFirstObjectByType(_componentType) as Component;
#else
            return UnityEngine.Object.FindObjectOfType(_componentType) as Component;
#endif
        }

        /// <summary>
        /// 查找或创建组件
        /// </summary>
        private Component FindOrCreateComponent()
        {
            var existing = FindComponentInScene();
            if (existing != null)
            {
                return existing;
            }

            return CreateOnNewGameObject();
        }

        /// <summary>
        /// 执行激活回调
        /// </summary>
        private void InvokeActivationCallbacks(Component component)
        {
            if (_onActivatedCallbacks == null || _onActivatedCallbacks.Count == 0)
                return;

            foreach (var callback in _onActivatedCallbacks)
            {
                try
                {
                    callback?.Invoke(component);
                }
                catch (Exception ex)
                {
                    UnityEngine.Debug.LogError(
                        $"[AFramework.DI] 执行组件激活回调时发生异常: {ex.Message}\n" +
                        $"Exception in component activation callback: {ex.Message}");
                    throw;
                }
            }
        }

        #endregion
    }
}
