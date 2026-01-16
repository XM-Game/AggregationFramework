// ==========================================================
// 文件名：InjectAttribute.cs
// 命名空间: AFramework.DI
// 依赖: System
// 功能: 定义 [Inject] 特性，标记需要依赖注入的成员

// ==========================================================

using System;

namespace AFramework.DI
{
    /// <summary>
    /// 注入特性
    /// <para>标记需要依赖注入的构造函数、方法、属性或字段</para>
    /// <para>Inject attribute that marks members requiring dependency injection</para>
    /// </summary>
    /// <remarks>
    /// 使用场景：
    /// <list type="bullet">
    /// <item>构造函数：当有多个构造函数时，标记要使用的构造函数</item>
    /// <item>方法：标记需要在对象创建后调用的注入方法</item>
    /// <item>属性：标记需要注入的属性</item>
    /// <item>字段：标记需要注入的字段（常用于 MonoBehaviour）</item>
    /// </list>
    /// 
    /// 注入优先级：
    /// <list type="number">
    /// <item>构造函数注入（对象创建时）</item>
    /// <item>字段注入（对象创建后）</item>
    /// <item>属性注入（对象创建后）</item>
    /// <item>方法注入（对象创建后）</item>
    /// </list>
    /// </remarks>
    /// <example>
    /// <code>
    /// // 构造函数注入
    /// public class PlayerController
    /// {
    ///     private readonly IInputService _inputService;
    ///     
    ///     [Inject]
    ///     public PlayerController(IInputService inputService)
    ///     {
    ///         _inputService = inputService;
    ///     }
    /// }
    /// 
    /// // 方法注入
    /// public class GameManager : MonoBehaviour
    /// {
    ///     private IScoreService _scoreService;
    ///     
    ///     [Inject]
    ///     public void Construct(IScoreService scoreService)
    ///     {
    ///         _scoreService = scoreService;
    ///     }
    /// }
    /// 
    /// // 属性注入
    /// public class UIPanel
    /// {
    ///     [Inject]
    ///     public ILocalizationService Localization { get; set; }
    /// }
    /// 
    /// // 字段注入
    /// public class EnemySpawner : MonoBehaviour
    /// {
    ///     [Inject]
    ///     private IObjectPool _objectPool;
    /// }
    /// </code>
    /// </example>
    [AttributeUsage(
        AttributeTargets.Constructor | 
        AttributeTargets.Method | 
        AttributeTargets.Property | 
        AttributeTargets.Field,
        AllowMultiple = false,
        Inherited = true)]
    public sealed class InjectAttribute : Attribute
    {
        /// <summary>
        /// 创建注入特性实例
        /// </summary>
        public InjectAttribute()
        {
        }
    }
}
