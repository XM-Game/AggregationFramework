// ==========================================================
// 文件名：IFormatterResolver.cs
// 命名空间: AFramework.Serialization
// 依赖: System
// ==========================================================

using System;

namespace AFramework.Serialization
{
    /// <summary>
    /// 格式化器解析器接口
    /// <para>负责根据类型查找和创建对应的格式化器</para>
    /// <para>支持格式化器缓存和链式解析</para>
    /// </summary>
    /// <remarks>
    /// 格式化器解析器是序列化系统的核心组件，负责：
    /// 1. 根据类型查找对应的格式化器
    /// 2. 缓存已创建的格式化器实例
    /// 3. 支持自定义格式化器注册
    /// 4. 支持链式解析（多个解析器组合）
    /// 
    /// 使用示例:
    /// <code>
    /// // 获取格式化器
    /// var formatter = resolver.GetFormatter&lt;Player&gt;();
    /// 
    /// // 注册自定义格式化器
    /// resolver.Register&lt;Player&gt;(new PlayerFormatter());
    /// 
    /// // 链式解析
    /// var compositeResolver = new CompositeResolver(
    ///     customResolver,
    ///     builtInResolver,
    ///     generatedResolver
    /// );
    /// </code>
    /// </remarks>
    public interface IFormatterResolver
    {
        /// <summary>
        /// 获取指定类型的格式化器
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <returns>格式化器实例，如果未找到返回 null</returns>
        IFormatter<T> GetFormatter<T>();

        /// <summary>
        /// 获取指定类型的格式化器 (非泛型版本)
        /// </summary>
        /// <param name="type">目标类型</param>
        /// <returns>格式化器实例，如果未找到返回 null</returns>
        IFormatter GetFormatter(Type type);

        /// <summary>
        /// 尝试获取指定类型的格式化器
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <param name="formatter">输出格式化器</param>
        /// <returns>如果找到返回 true</returns>
        bool TryGetFormatter<T>(out IFormatter<T> formatter);

        /// <summary>
        /// 尝试获取指定类型的格式化器 (非泛型版本)
        /// </summary>
        /// <param name="type">目标类型</param>
        /// <param name="formatter">输出格式化器</param>
        /// <returns>如果找到返回 true</returns>
        bool TryGetFormatter(Type type, out IFormatter formatter);

        /// <summary>
        /// 检查是否支持指定类型
        /// </summary>
        /// <param name="type">要检查的类型</param>
        /// <returns>如果支持返回 true</returns>
        bool CanResolve(Type type);
    }

    /// <summary>
    /// 可注册的格式化器解析器接口
    /// <para>支持动态注册和注销格式化器</para>
    /// </summary>
    public interface IRegisterableFormatterResolver : IFormatterResolver
    {
        /// <summary>
        /// 注册格式化器
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <param name="formatter">格式化器实例</param>
        void Register<T>(IFormatter<T> formatter);

        /// <summary>
        /// 注册格式化器 (非泛型版本)
        /// </summary>
        /// <param name="type">目标类型</param>
        /// <param name="formatter">格式化器实例</param>
        void Register(Type type, IFormatter formatter);

        /// <summary>
        /// 注销格式化器
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <returns>如果成功注销返回 true</returns>
        bool Unregister<T>();

        /// <summary>
        /// 注销格式化器 (非泛型版本)
        /// </summary>
        /// <param name="type">目标类型</param>
        /// <returns>如果成功注销返回 true</returns>
        bool Unregister(Type type);

        /// <summary>
        /// 清除所有注册的格式化器
        /// </summary>
        void Clear();

        /// <summary>
        /// 获取已注册的类型数量
        /// </summary>
        int RegisteredCount { get; }
    }

    /// <summary>
    /// 可组合的格式化器解析器接口
    /// <para>支持多个解析器链式组合</para>
    /// </summary>
    public interface ICompositeFormatterResolver : IFormatterResolver
    {
        /// <summary>
        /// 添加子解析器
        /// </summary>
        /// <param name="resolver">子解析器</param>
        void AddResolver(IFormatterResolver resolver);

        /// <summary>
        /// 移除子解析器
        /// </summary>
        /// <param name="resolver">子解析器</param>
        /// <returns>如果成功移除返回 true</returns>
        bool RemoveResolver(IFormatterResolver resolver);

        /// <summary>
        /// 获取所有子解析器
        /// </summary>
        /// <returns>子解析器数组</returns>
        IFormatterResolver[] GetResolvers();

        /// <summary>
        /// 获取子解析器数量
        /// </summary>
        int ResolverCount { get; }
    }

    /// <summary>
    /// 格式化器工厂接口
    /// <para>用于动态创建格式化器实例</para>
    /// </summary>
    public interface IFormatterFactory
    {
        /// <summary>
        /// 创建指定类型的格式化器
        /// </summary>
        /// <param name="type">目标类型</param>
        /// <returns>格式化器实例，如果无法创建返回 null</returns>
        IFormatter CreateFormatter(Type type);

        /// <summary>
        /// 检查是否可以为指定类型创建格式化器
        /// </summary>
        /// <param name="type">要检查的类型</param>
        /// <returns>如果可以创建返回 true</returns>
        bool CanCreate(Type type);

        /// <summary>
        /// 获取工厂优先级 (数值越小优先级越高)
        /// </summary>
        int Priority { get; }
    }
}
