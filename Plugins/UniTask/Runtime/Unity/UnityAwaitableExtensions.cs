#if UNITY_2023_1_OR_NEWER
/// <summary>
/// UnityAwaitableExtensions 类
/// 提供将 UnityEngine.Awaitable 转换为 UniTask 的扩展方法
/// </summary>
namespace Cysharp.Threading.Tasks
{
    /// <summary>
    /// UnityAwaitableExtensions 类
    /// 提供将 UnityEngine.Awaitable 转换为 UniTask 的扩展方法
    /// </summary>
    public static class UnityAwaitableExtensions
    {
        /// <summary>
        /// 将 UnityEngine.Awaitable 转换为 UniTask
        /// </summary>
        public static async UniTask AsUniTask(this UnityEngine.Awaitable awaitable)
        {
            await awaitable;
        }
        
        /// <summary>
        /// 将 UnityEngine.Awaitable<T> 转换为 UniTask<T>
        /// </summary>
        public static async UniTask<T> AsUniTask<T>(this UnityEngine.Awaitable<T> awaitable)
        {
            return await awaitable;
        }
    }
}
#endif
