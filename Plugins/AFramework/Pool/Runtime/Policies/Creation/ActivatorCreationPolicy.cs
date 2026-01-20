// ==========================================================
// 文件名：ActivatorCreationPolicy.cs
// 命名空间: AFramework.Pool
// 依赖: System
// 功能: Activator 创建策略，使用反射创建对象
// ==========================================================

using System;

namespace AFramework.Pool
{
    /// <summary>
    /// Activator 创建策略
    /// Activator Creation Policy
    /// 
    /// <para>使用 Activator.CreateInstance 反射创建对象的策略</para>
    /// <para>Policy that creates objects using Activator.CreateInstance reflection</para>
    /// </summary>
    /// <typeparam name="T">对象类型 / Object type</typeparam>
    /// <remarks>
    /// 使用场景：
    /// - 运行时动态类型创建
    /// - 泛型类型参数未知
    /// - 插件系统
    /// - 配置驱动的对象创建
    /// 
    /// 优势：
    /// - 支持任意类型
    /// - 无需泛型约束
    /// - 灵活的类型选择
    /// 
    /// 劣势：
    /// - 性能较低（反射开销）
    /// - 不适合热路径
    /// - 需要无参构造函数
    /// </remarks>
    public class ActivatorCreationPolicy<T> : PoolPolicyBase<T>, IPoolCreationPolicy<T>
    {
        #region Fields

        /// <summary>
        /// 对象类型
        /// Object type
        /// </summary>
        private readonly Type _type;

        /// <summary>
        /// 是否使用非公共构造函数
        /// Whether to use non-public constructor
        /// </summary>
        private readonly bool _nonPublic;

        #endregion

        #region Constructors

        /// <summary>
        /// 构造函数
        /// Constructor
        /// </summary>
        /// <param name="type">对象类型 / Object type</param>
        /// <param name="nonPublic">是否使用非公共构造函数 / Whether to use non-public constructor</param>
        /// <param name="name">策略名称 / Policy name</param>
        public ActivatorCreationPolicy(Type type = null, bool nonPublic = false, string name = null)
            : base(name ?? "ActivatorCreationPolicy")
        {
            _type = type ?? typeof(T);
            _nonPublic = nonPublic;

            // 验证类型
            // Validate type
            if (_type.IsAbstract || _type.IsInterface)
            {
                throw new ArgumentException($"Cannot create instance of abstract class or interface: {_type.Name}", nameof(type));
            }
        }

        /// <summary>
        /// 构造函数（默认）
        /// Constructor (default)
        /// </summary>
        public ActivatorCreationPolicy()
            : this(null, false, null)
        {
        }

        #endregion

        #region IPoolCreationPolicy Implementation

        /// <inheritdoc />
        public T Create()
        {
            ThrowIfDisposed();

            try
            {
                object obj = Activator.CreateInstance(_type, _nonPublic);

                if (obj == null)
                {
                    throw new PoolCreationException($"Activator.CreateInstance returned null for type {_type.Name}.");
                }

                if (obj is T typedObj)
                {
                    return typedObj;
                }
                else
                {
                    throw new PoolCreationException($"Created object is not of type {typeof(T).Name}. Actual type: {obj.GetType().Name}");
                }
            }
            catch (PoolCreationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new PoolCreationException($"Failed to create object using Activator for type {_type.Name}.", ex);
            }
        }

        /// <inheritdoc />
        public bool Validate()
        {
            // 验证类型是否可以实例化
            // Validate if type can be instantiated
            if (_type == null)
            {
                return false;
            }

            if (_type.IsAbstract || _type.IsInterface)
            {
                return false;
            }

            // 检查是否有无参构造函数
            // Check if has parameterless constructor
            var constructor = _type.GetConstructor(
                _nonPublic ? 
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance : 
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance,
                null,
                Type.EmptyTypes,
                null);

            return constructor != null;
        }

        #endregion
    }
}
