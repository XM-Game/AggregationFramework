// ==========================================================
// 文件名：ScriptableObjectInstaller.cs
// 命名空间: AFramework.DI
// 依赖: UnityEngine
// ==========================================================

using UnityEngine;

namespace AFramework.DI
{
    /// <summary>
    /// ScriptableObject 安装器基类
    /// <para>可作为资产文件复用的安装器</para>
    /// </summary>
    /// <remarks>
    /// 使用场景：
    /// - 需要跨场景复用的安装器配置
    /// - 需要版本控制的安装器配置
    /// - 需要在多个 LifetimeScope 中共享的配置
    /// 
    /// 使用方式：
    /// 1. 创建继承 ScriptableObjectInstaller 的类
    /// 2. 通过 CreateAssetMenu 创建资产文件
    /// 3. 在 LifetimeScope 的 Inspector 中引用该资产
    /// </remarks>
    /// <example>
    /// <code>
    /// [CreateAssetMenu(fileName = "GameSettings", menuName = "AFramework/Installers/GameSettings")]
    /// public class GameSettingsInstaller : ScriptableObjectInstaller
    /// {
    ///     [SerializeField] private int _maxPlayers = 4;
    ///     [SerializeField] private float _gameSpeed = 1.0f;
    ///     
    ///     public override void Install(IContainerBuilder builder)
    ///     {
    ///         var settings = new GameSettings(_maxPlayers, _gameSpeed);
    ///         builder.RegisterInstance(settings);
    ///     }
    /// }
    /// </code>
    /// </example>
    public abstract class ScriptableObjectInstaller : ScriptableObject, IInstaller
    {
        /// <summary>
        /// 安装服务到容器构建器
        /// </summary>
        /// <param name="builder">容器构建器</param>
        public abstract void Install(IContainerBuilder builder);
    }

    /// <summary>
    /// 泛型 ScriptableObject 安装器基类
    /// <para>支持类型安全的配置数据</para>
    /// </summary>
    /// <typeparam name="TConfig">配置数据类型</typeparam>
    public abstract class ScriptableObjectInstaller<TConfig> : ScriptableObjectInstaller
        where TConfig : class
    {
        [SerializeField]
        [Tooltip("安装器配置数据")]
        private TConfig _config;

        /// <summary>
        /// 获取配置数据
        /// </summary>
        protected TConfig Config => _config;
    }
}
