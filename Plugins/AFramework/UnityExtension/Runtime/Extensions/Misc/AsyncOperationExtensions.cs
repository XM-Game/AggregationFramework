// ==========================================================
// 文件名：AsyncOperationExtensions.cs
// 命名空间: AFramework.UnityExtension
// 依赖: UnityEngine, System
// ==========================================================

using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace AFramework.UnityExtension
{
    /// <summary>
    /// AsyncOperation 扩展方法
    /// <para>提供 AsyncOperation 的状态查询和实用功能扩展</para>
    /// </summary>
    public static class AsyncOperationExtensions
    {
        #region 状态查询

        /// <summary>
        /// 检查是否完成
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsCompleted(this AsyncOperation operation)
        {
            return operation != null && operation.isDone;
        }

        /// <summary>
        /// 检查是否正在进行
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsInProgress(this AsyncOperation operation)
        {
            return operation != null && !operation.isDone;
        }

        /// <summary>
        /// 获取进度百分比 (0-100)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float GetProgressPercent(this AsyncOperation operation)
        {
            return operation != null ? operation.progress * 100f : 0f;
        }

        /// <summary>
        /// 获取进度 (0-1)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float GetProgress(this AsyncOperation operation)
        {
            return operation?.progress ?? 0f;
        }

        /// <summary>
        /// 获取显示用进度 (场景加载时 0.9 = 100%)
        /// </summary>
        public static float GetDisplayProgress(this AsyncOperation operation)
        {
            if (operation == null) return 0f;

            // Unity 场景加载在 allowSceneActivation = false 时会停在 0.9
            if (operation.progress >= 0.9f)
                return 1f;

            return operation.progress / 0.9f;
        }

        /// <summary>
        /// 获取显示用进度百分比
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float GetDisplayProgressPercent(this AsyncOperation operation)
        {
            return operation.GetDisplayProgress() * 100f;
        }

        #endregion

        #region 控制

        /// <summary>
        /// 允许场景激活
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AllowActivation(this AsyncOperation operation)
        {
            if (operation != null)
            {
                operation.allowSceneActivation = true;
            }
        }

        /// <summary>
        /// 禁止场景激活
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void PreventActivation(this AsyncOperation operation)
        {
            if (operation != null)
            {
                operation.allowSceneActivation = false;
            }
        }

        /// <summary>
        /// 设置优先级
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetPriority(this AsyncOperation operation, int priority)
        {
            if (operation != null)
            {
                operation.priority = priority;
            }
        }

        /// <summary>
        /// 设置为高优先级
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetHighPriority(this AsyncOperation operation)
        {
            operation?.SetPriority(0);
        }

        /// <summary>
        /// 设置为低优先级
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetLowPriority(this AsyncOperation operation)
        {
            operation?.SetPriority(1000);
        }

        #endregion

        #region 回调

        /// <summary>
        /// 添加完成回调
        /// </summary>
        public static AsyncOperation OnComplete(this AsyncOperation operation, Action callback)
        {
            if (operation == null || callback == null) return operation;

            if (operation.isDone)
            {
                callback();
            }
            else
            {
                operation.completed += _ => callback();
            }

            return operation;
        }

        /// <summary>
        /// 添加完成回调 (带操作参数)
        /// </summary>
        public static AsyncOperation OnComplete(this AsyncOperation operation, Action<AsyncOperation> callback)
        {
            if (operation == null || callback == null) return operation;

            if (operation.isDone)
            {
                callback(operation);
            }
            else
            {
                operation.completed += callback;
            }

            return operation;
        }

        #endregion

        #region 链式配置

        /// <summary>
        /// 配置允许场景激活
        /// </summary>
        public static AsyncOperation WithAllowSceneActivation(this AsyncOperation operation, bool allow)
        {
            if (operation != null)
            {
                operation.allowSceneActivation = allow;
            }
            return operation;
        }

        /// <summary>
        /// 配置优先级
        /// </summary>
        public static AsyncOperation WithPriority(this AsyncOperation operation, int priority)
        {
            if (operation != null)
            {
                operation.priority = priority;
            }
            return operation;
        }

        #endregion
    }

    /// <summary>
    /// ResourceRequest 扩展方法
    /// </summary>
    public static class ResourceRequestExtensions
    {
        /// <summary>
        /// 获取加载的资源
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T GetAsset<T>(this ResourceRequest request) where T : UnityEngine.Object
        {
            return request?.asset as T;
        }

        /// <summary>
        /// 检查资源是否加载成功
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsSuccessful(this ResourceRequest request)
        {
            return request != null && request.isDone && request.asset != null;
        }

        /// <summary>
        /// 添加完成回调
        /// </summary>
        public static ResourceRequest OnComplete<T>(this ResourceRequest request, Action<T> callback) where T : UnityEngine.Object
        {
            if (request == null || callback == null) return request;

            if (request.isDone)
            {
                callback(request.asset as T);
            }
            else
            {
                request.completed += _ => callback(request.asset as T);
            }

            return request;
        }
    }

    /// <summary>
    /// AssetBundleRequest 扩展方法
    /// </summary>
    public static class AssetBundleRequestExtensions
    {
        /// <summary>
        /// 获取加载的资源
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T GetAsset<T>(this AssetBundleRequest request) where T : UnityEngine.Object
        {
            return request?.asset as T;
        }

        /// <summary>
        /// 获取加载的所有资源
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T[] GetAllAssets<T>(this AssetBundleRequest request) where T : UnityEngine.Object
        {
            if (request?.allAssets == null) return Array.Empty<T>();

            var result = new System.Collections.Generic.List<T>();
            foreach (var asset in request.allAssets)
            {
                if (asset is T typedAsset)
                {
                    result.Add(typedAsset);
                }
            }
            return result.ToArray();
        }

        /// <summary>
        /// 检查资源是否加载成功
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsSuccessful(this AssetBundleRequest request)
        {
            return request != null && request.isDone && request.asset != null;
        }

        /// <summary>
        /// 添加完成回调
        /// </summary>
        public static AssetBundleRequest OnComplete<T>(this AssetBundleRequest request, Action<T> callback) where T : UnityEngine.Object
        {
            if (request == null || callback == null) return request;

            if (request.isDone)
            {
                callback(request.asset as T);
            }
            else
            {
                request.completed += _ => callback(request.asset as T);
            }

            return request;
        }
    }

    /// <summary>
    /// AssetBundleCreateRequest 扩展方法
    /// </summary>
    public static class AssetBundleCreateRequestExtensions
    {
        /// <summary>
        /// 获取加载的 AssetBundle
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static AssetBundle GetAssetBundle(this AssetBundleCreateRequest request)
        {
            return request?.assetBundle;
        }

        /// <summary>
        /// 检查是否加载成功
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsSuccessful(this AssetBundleCreateRequest request)
        {
            return request != null && request.isDone && request.assetBundle != null;
        }

        /// <summary>
        /// 添加完成回调
        /// </summary>
        public static AssetBundleCreateRequest OnComplete(this AssetBundleCreateRequest request, Action<AssetBundle> callback)
        {
            if (request == null || callback == null) return request;

            if (request.isDone)
            {
                callback(request.assetBundle);
            }
            else
            {
                request.completed += _ => callback(request.assetBundle);
            }

            return request;
        }
    }
}
