// ==========================================================
// 文件名：EntryPointRunner.cs
// 命名空间: AFramework.DI
// 依赖: UnityEngine, Cysharp.Threading.Tasks
// ==========================================================

using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace AFramework.DI
{
    /// <summary>
    /// 入口点运行器
    /// <para>Unity MonoBehaviour 组件，负责驱动入口点的生命周期执行</para>
    /// </summary>
    /// <remarks>
    /// 该组件会自动挂载到 LifetimeScope 所在的 GameObject 上，
    /// 并在 Unity 生命周期中调用相应的入口点方法。
    /// </remarks>
    [AddComponentMenu("")] // 隐藏在 AddComponent 菜单中
    [DisallowMultipleComponent]
    public sealed class EntryPointRunner : MonoBehaviour
    {
        #region 字段

        private EntryPointDispatcher _dispatcher;
        private EntryPointRegistry _registry;
        private CancellationTokenSource _cts;

        private bool _isInitialized;
        private bool _isStarted;
        private bool _autoStartOnAwake = true;

        #endregion

        #region 属性

        /// <summary>
        /// 入口点调度器
        /// </summary>
        public EntryPointDispatcher Dispatcher => _dispatcher;

        /// <summary>
        /// 入口点注册表
        /// </summary>
        public EntryPointRegistry Registry => _registry;

        /// <summary>
        /// 是否已初始化
        /// </summary>
        public bool IsInitialized => _isInitialized;

        /// <summary>
        /// 是否已启动
        /// </summary>
        public bool IsStarted => _isStarted;

        /// <summary>
        /// 是否在 Awake 时自动启动
        /// <para>默认为 true</para>
        /// </summary>
        public bool AutoStartOnAwake
        {
            get => _autoStartOnAwake;
            set => _autoStartOnAwake = value;
        }

        /// <summary>
        /// 取消令牌
        /// </summary>
        public CancellationToken CancellationToken => _cts?.Token ?? CancellationToken.None;

        #endregion

        #region 事件

        /// <summary>
        /// 初始化完成事件
        /// </summary>
        public event Action OnInitialized;

        /// <summary>
        /// 启动完成事件
        /// </summary>
        public event Action OnStarted;

        #endregion

        #region 公共方法

        /// <summary>
        /// 配置入口点运行器
        /// </summary>
        /// <param name="registry">入口点注册表</param>
        /// <param name="exceptionHandler">异常处理器（可选）</param>
        public void Configure(
            EntryPointRegistry registry,
            EntryPointExceptionHandler exceptionHandler = null)
        {
            if (_isInitialized)
            {
                Debug.LogWarning("[AFramework.DI] EntryPointRunner 已初始化，无法重新配置");
                return;
            }

            _registry = registry ?? throw new ArgumentNullException(nameof(registry));
            _dispatcher = new EntryPointDispatcher(registry, exceptionHandler);
            _cts = new CancellationTokenSource();
        }

        /// <summary>
        /// 手动执行初始化阶段
        /// </summary>
        public void Initialize()
        {
            if (_isInitialized) return;
            if (_dispatcher == null)
            {
                Debug.LogError("[AFramework.DI] EntryPointRunner 未配置，请先调用 Configure 方法");
                return;
            }

            _dispatcher.Initialize();
            _isInitialized = true;

            try
            {
                OnInitialized?.Invoke();
            }
            catch (Exception ex)
            {
                Debug.LogError($"[AFramework.DI] OnInitialized 事件处理异常: {ex}");
            }
        }

        /// <summary>
        /// 异步执行启动阶段
        /// </summary>
        /// <param name="cancellation">取消令牌</param>
        public async UniTask StartAsync(CancellationToken cancellation = default)
        {
            if (_isStarted) return;
            if (_dispatcher == null)
            {
                Debug.LogError("[AFramework.DI] EntryPointRunner 未配置，请先调用 Configure 方法");
                return;
            }

            // 确保已初始化
            if (!_isInitialized)
            {
                Initialize();
            }

            // 合并取消令牌
            using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(
                _cts.Token, cancellation);

            await _dispatcher.StartAsync(linkedCts.Token);
            _isStarted = true;

            try
            {
                OnStarted?.Invoke();
            }
            catch (Exception ex)
            {
                Debug.LogError($"[AFramework.DI] OnStarted 事件处理异常: {ex}");
            }
        }

        /// <summary>
        /// 同步执行启动阶段（不包含异步启动）
        /// </summary>
        public void StartSync()
        {
            if (_isStarted) return;
            if (_dispatcher == null)
            {
                Debug.LogError("[AFramework.DI] EntryPointRunner 未配置，请先调用 Configure 方法");
                return;
            }

            // 确保已初始化
            if (!_isInitialized)
            {
                Initialize();
            }

            _dispatcher.Start();
            _isStarted = true;

            try
            {
                OnStarted?.Invoke();
            }
            catch (Exception ex)
            {
                Debug.LogError($"[AFramework.DI] OnStarted 事件处理异常: {ex}");
            }
        }

        /// <summary>
        /// 注册入口点实例
        /// </summary>
        /// <param name="instance">要注册的实例</param>
        public void Register(object instance)
        {
            _registry?.Register(instance);
        }

        /// <summary>
        /// 注销入口点实例
        /// </summary>
        /// <param name="instance">要注销的实例</param>
        public void Unregister(object instance)
        {
            _registry?.Unregister(instance);
        }

        #endregion

        #region Unity 生命周期

        private void Update()
        {
            if (!_isStarted) return;
            _dispatcher?.Tick();
        }

        private void FixedUpdate()
        {
            if (!_isStarted) return;
            _dispatcher?.FixedTick();
        }

        private void LateUpdate()
        {
            if (!_isStarted) return;
            _dispatcher?.LateTick();
        }

        private void OnDestroy()
        {
            Cleanup();
        }

        private void OnApplicationQuit()
        {
            Cleanup();
        }

        #endregion

        #region 私有方法

        private void Cleanup()
        {
            _cts?.Cancel();
            _cts?.Dispose();
            _cts = null;

            _dispatcher?.Dispose();
            _dispatcher = null;

            _registry?.Dispose();
            _registry = null;

            OnInitialized = null;
            OnStarted = null;
        }

        #endregion

        #region 静态工厂方法

        /// <summary>
        /// 创建入口点运行器
        /// </summary>
        /// <param name="gameObject">要挂载的 GameObject</param>
        /// <param name="registry">入口点注册表</param>
        /// <param name="exceptionHandler">异常处理器（可选）</param>
        /// <returns>入口点运行器实例</returns>
        public static EntryPointRunner Create(
            GameObject gameObject,
            EntryPointRegistry registry,
            EntryPointExceptionHandler exceptionHandler = null)
        {
            if (gameObject == null)
                throw new ArgumentNullException(nameof(gameObject));

            var runner = gameObject.GetComponent<EntryPointRunner>();
            if (runner == null)
            {
                runner = gameObject.AddComponent<EntryPointRunner>();
            }

            runner.Configure(registry, exceptionHandler);
            return runner;
        }

        /// <summary>
        /// 创建独立的入口点运行器 GameObject
        /// </summary>
        /// <param name="name">GameObject 名称</param>
        /// <param name="registry">入口点注册表</param>
        /// <param name="exceptionHandler">异常处理器（可选）</param>
        /// <param name="dontDestroyOnLoad">是否跨场景保留</param>
        /// <returns>入口点运行器实例</returns>
        public static EntryPointRunner CreateStandalone(
            string name,
            EntryPointRegistry registry,
            EntryPointExceptionHandler exceptionHandler = null,
            bool dontDestroyOnLoad = false)
        {
            var go = new GameObject(name ?? "[EntryPointRunner]");

            if (dontDestroyOnLoad)
            {
                DontDestroyOnLoad(go);
            }

            return Create(go, registry, exceptionHandler);
        }

        #endregion
    }
}
