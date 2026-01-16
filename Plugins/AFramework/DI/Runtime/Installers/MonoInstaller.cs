// ==========================================================
// 文件名：MonoInstaller.cs
// 命名空间: AFramework.DI
// 依赖: UnityEngine
// ==========================================================

using UnityEngine;

namespace AFramework.DI
{
    /// <summary>
    /// MonoBehaviour 安装器基类
    /// <para>支持在 Inspector 中配置的安装器</para>
    /// </summary>
    /// <remarks>
    /// 使用场景：
    /// - 需要在 Inspector 中配置参数的安装器
    /// - 需要引用场景中 GameObject 的安装器
    /// - 需要序列化配置数据的安装器
    /// 
    /// 使用方式：
    /// 1. 创建继承 MonoInstaller 的类
    /// 2. 将组件挂载到 LifetimeScope 所在的 GameObject
    /// 3. 在 Inspector 中配置参数
    /// </remarks>
    /// <example>
    /// <code>
    /// public class UIInstaller : MonoInstaller
    /// {
    ///     [SerializeField] private Canvas _mainCanvas;
    ///     [SerializeField] private GameObject _loadingPrefab;
    ///     
    ///     public override void Install(IContainerBuilder builder)
    ///     {
    ///         builder.RegisterInstance(_mainCanvas);
    ///         builder.RegisterInstance(_loadingPrefab).Keyed("LoadingPrefab");
    ///         builder.Register&lt;IUIManager, UIManager&gt;().Singleton();
    ///     }
    /// }
    /// </code>
    /// </example>
    public abstract class MonoInstaller : MonoBehaviour, IInstaller
    {
        /// <summary>
        /// 安装服务到容器构建器
        /// </summary>
        /// <param name="builder">容器构建器</param>
        public abstract void Install(IContainerBuilder builder);
    }

    /// <summary>
    /// 泛型 MonoBehaviour 安装器基类
    /// <para>支持类型安全的配置数据</para>
    /// </summary>
    /// <typeparam name="TConfig">配置数据类型</typeparam>
    public abstract class MonoInstaller<TConfig> : MonoInstaller
        where TConfig : class
    {
        [SerializeField]
        [Tooltip("安装器配置数据")]
        private TConfig _config;

        /// <summary>
        /// 获取配置数据
        /// </summary>
        protected TConfig Config => _config;

        /// <summary>
        /// 设置配置数据（用于运行时配置）
        /// </summary>
        /// <param name="config">配置数据</param>
        public void SetConfig(TConfig config)
        {
            _config = config;
        }
    }
}
