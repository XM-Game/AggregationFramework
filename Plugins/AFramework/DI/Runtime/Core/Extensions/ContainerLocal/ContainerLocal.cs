// ==========================================================
// 文件名：ContainerLocal.cs
// 命名空间: AFramework.DI
// 依赖: System
// 功能: 容器本地值，每个作用域容器拥有独立的实例
// ==========================================================

using System;

namespace AFramework.DI
{
    /// <summary>
    /// 容器本地值
    /// <para>每个作用域容器拥有独立的实例，类似于 ThreadLocal 但基于容器作用域</para>
    /// <para>Container-local value where each scope has its own independent instance</para>
    /// </summary>
    /// <typeparam name="T">值类型 / Value type</typeparam>
    /// <remarks>
    /// 使用场景：
    /// <list type="bullet">
    /// <item>多租户系统中每个租户有独立的配置</item>
    /// <item>多场景系统中每个场景有独立的状态</item>
    /// <item>请求作用域中每个请求有独立的上下文</item>
    /// </list>
    /// 
    /// 使用示例：
    /// <code>
    /// // 注册 ContainerLocal
    /// builder.RegisterContainerLocal&lt;UserContext&gt;();
    /// 
    /// // 在不同作用域中使用
    /// var local = container.Resolve&lt;ContainerLocal&lt;UserContext&gt;&gt;();
    /// local.Value = new UserContext { UserId = 123 };
    /// 
    /// // 子作用域有独立的值
    /// using var scope = container.CreateScope();
    /// var scopedLocal = scope.Resolve&lt;ContainerLocal&lt;UserContext&gt;&gt;();
    /// scopedLocal.Value = new UserContext { UserId = 456 }; // 不影响父作用域
    /// </code>
    /// </remarks>
    public sealed class ContainerLocal<T>
    {
        #region 字段 / Fields

        private T _value;
        private bool _hasValue;
        private readonly Func<T> _factory;

        #endregion

        #region 构造函数 / Constructor

        /// <summary>
        /// 创建容器本地值（无初始值）
        /// </summary>
        public ContainerLocal()
        {
            _value = default;
            _hasValue = false;
            _factory = null;
        }

        /// <summary>
        /// 创建容器本地值（带工厂函数）
        /// <para>Create container local with factory function</para>
        /// </summary>
        /// <param name="factory">值工厂函数 / Value factory function</param>
        public ContainerLocal(Func<T> factory)
        {
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
            _value = default;
            _hasValue = false;
        }

        /// <summary>
        /// 创建容器本地值（带初始值）
        /// <para>Create container local with initial value</para>
        /// </summary>
        /// <param name="initialValue">初始值 / Initial value</param>
        public ContainerLocal(T initialValue)
        {
            _value = initialValue;
            _hasValue = true;
            _factory = null;
        }

        #endregion

        #region 属性 / Properties

        /// <summary>
        /// 获取或设置本地值
        /// <para>Get or set the local value</para>
        /// </summary>
        /// <remarks>
        /// 如果值未设置且有工厂函数，首次访问时会自动创建值
        /// </remarks>
        public T Value
        {
            get
            {
                if (!_hasValue && _factory != null)
                {
                    _value = _factory();
                    _hasValue = true;
                }
                return _value;
            }
            set
            {
                _value = value;
                _hasValue = true;
            }
        }

        /// <summary>
        /// 检查是否已设置值
        /// <para>Check if value has been set</para>
        /// </summary>
        public bool HasValue => _hasValue;

        #endregion

        #region 方法 / Methods

        /// <summary>
        /// 尝试获取值
        /// <para>Try to get the value</para>
        /// </summary>
        /// <param name="value">输出值 / Output value</param>
        /// <returns>是否有值 / Whether has value</returns>
        public bool TryGetValue(out T value)
        {
            if (_hasValue)
            {
                value = _value;
                return true;
            }

            if (_factory != null)
            {
                _value = _factory();
                _hasValue = true;
                value = _value;
                return true;
            }

            value = default;
            return false;
        }

        /// <summary>
        /// 获取值或默认值
        /// <para>Get value or default</para>
        /// </summary>
        /// <param name="defaultValue">默认值 / Default value</param>
        /// <returns>值或默认值 / Value or default</returns>
        public T GetValueOrDefault(T defaultValue = default)
        {
            return TryGetValue(out var value) ? value : defaultValue;
        }

        /// <summary>
        /// 清除值
        /// <para>Clear the value</para>
        /// </summary>
        public void Clear()
        {
            _value = default;
            _hasValue = false;
        }

        /// <summary>
        /// 重置为初始状态（如果有工厂函数，下次访问会重新创建）
        /// <para>Reset to initial state</para>
        /// </summary>
        public void Reset()
        {
            Clear();
        }

        #endregion

        #region 隐式转换 / Implicit Conversion

        /// <summary>
        /// 隐式转换为值类型
        /// </summary>
        public static implicit operator T(ContainerLocal<T> local)
        {
            return local.Value;
        }

        #endregion

        #region ToString

        /// <inheritdoc/>
        public override string ToString()
        {
            return _hasValue ? $"ContainerLocal<{typeof(T).Name}>({_value})" : $"ContainerLocal<{typeof(T).Name}>(empty)";
        }

        #endregion
    }
}
