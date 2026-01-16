// ==========================================================
// 文件名：IFactory.cs
// 命名空间: AFramework.DI
// 依赖: 无
// ==========================================================

namespace AFramework.DI
{
    /// <summary>
    /// 工厂接口
    /// <para>提供创建服务实例的能力</para>
    /// </summary>
    /// <typeparam name="T">服务类型</typeparam>
    public interface IFactory<out T>
    {
        /// <summary>
        /// 创建服务实例
        /// </summary>
        /// <returns>服务实例</returns>
        T Create();
    }

    /// <summary>
    /// 带参数的工厂接口
    /// </summary>
    /// <typeparam name="TParam">参数类型</typeparam>
    /// <typeparam name="T">服务类型</typeparam>
    public interface IFactory<in TParam, out T>
    {
        /// <summary>
        /// 使用参数创建服务实例
        /// </summary>
        /// <param name="param">创建参数</param>
        /// <returns>服务实例</returns>
        T Create(TParam param);
    }

    /// <summary>
    /// 带两个参数的工厂接口
    /// </summary>
    public interface IFactory<in TParam1, in TParam2, out T>
    {
        /// <summary>
        /// 使用参数创建服务实例
        /// </summary>
        T Create(TParam1 param1, TParam2 param2);
    }

    /// <summary>
    /// 带三个参数的工厂接口
    /// </summary>
    public interface IFactory<in TParam1, in TParam2, in TParam3, out T>
    {
        /// <summary>
        /// 使用参数创建服务实例
        /// </summary>
        T Create(TParam1 param1, TParam2 param2, TParam3 param3);
    }
}
