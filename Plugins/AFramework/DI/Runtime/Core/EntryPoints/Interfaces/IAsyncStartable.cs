// ==========================================================
// 文件名：IAsyncStartable.cs
// 命名空间: AFramework.DI
// 依赖: Cysharp.Threading.Tasks (UniTask)
// ==========================================================

using System.Threading;
using Cysharp.Threading.Tasks;

namespace AFramework.DI
{
    /// <summary>
    /// 异步启动接口
    /// <para>支持异步初始化操作，如资源加载、网络请求等</para>
    /// <para>执行顺序：IPostInitializable → IAsyncStartable → IStartable</para>
    /// </summary>
    /// <remarks>
    /// 使用场景：
    /// - 异步加载资源
    /// - 网络初始化
    /// - 数据库连接
    /// - 需要等待的初始化操作
    /// </remarks>
    public interface IAsyncStartable
    {
        /// <summary>
        /// 异步启动方法
        /// </summary>
        /// <param name="cancellation">取消令牌，当生命周期作用域销毁时触发</param>
        /// <returns>异步任务</returns>
        UniTask StartAsync(CancellationToken cancellation);
    }
}
