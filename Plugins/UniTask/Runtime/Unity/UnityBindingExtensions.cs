using System;
using System.Threading;
using UnityEngine;
#if !UNITY_2019_1_OR_NEWER || UNITASK_UGUI_SUPPORT
using UnityEngine.UI;
#endif

namespace Cysharp.Threading.Tasks
{
    /// <summary>
    /// UnityBindingExtensions 类
    /// 提供将 IUniTaskAsyncEnumerable 绑定到 UnityEngine.UI.Text、Selectable、MonoBehaviour 的扩展方法
    /// </summary>
    public static class UnityBindingExtensions
    {
#if !UNITY_2019_1_OR_NEWER || UNITASK_UGUI_SUPPORT
        // <string> -> Text
        /// <summary>
        /// 将字符串异步序列绑定到 Text，默认在目标销毁时自动取消，并在出错时尝试重绑。
        /// </summary>
        public static void BindTo(this IUniTaskAsyncEnumerable<string> source, UnityEngine.UI.Text text, bool rebindOnError = true)
        {
            BindToCore(source, text, text.GetCancellationTokenOnDestroy(), rebindOnError).Forget();
        }

        /// <summary>
        /// 将字符串异步序列绑定到 Text，可自定义取消令牌与错误重绑行为。
        /// </summary>
        public static void BindTo(this IUniTaskAsyncEnumerable<string> source, UnityEngine.UI.Text text, CancellationToken cancellationToken, bool rebindOnError = true)
        {
            BindToCore(source, text, cancellationToken, rebindOnError).Forget();
        }

        /// <summary>
        /// 具体的绑定实现：消费异步序列，将每次 Current 写入 text.text；支持在异常时按需重绑。
        /// </summary>
        static async UniTaskVoid BindToCore(IUniTaskAsyncEnumerable<string> source, UnityEngine.UI.Text text, CancellationToken cancellationToken, bool rebindOnError)
        {
            var repeat = false; // 是否重复绑定
            BIND_AGAIN:
            var e = source.GetAsyncEnumerator(cancellationToken); // 获取异步枚举器
            try
            {
                while (true)
                {
                    bool moveNext; // 是否移动到下一个
                    try
                    {
                        moveNext = await e.MoveNextAsync();
                        repeat = false; // 不重复绑定
                    }
                    catch (Exception ex)
                    {
                        if (ex is OperationCanceledException) return; // 如果操作已取消，则返回

                        if (rebindOnError && !repeat)
                        {
                            repeat = true; // 重复绑定
                            goto BIND_AGAIN;
                        }
                        else
                        {
                            throw; // 抛出异常
                        }
                    }

                    if (!moveNext) return; // 如果移动到下一个失败，则返回

                    text.text = e.Current; // 将当前值写入 text.text
                }
            }
            finally
            {
                if (e != null) // 如果异步枚举器不为空
                {
                    await e.DisposeAsync();
                }
            }
        }

        // <T> -> Text

        /// <summary>
        /// 将泛型异步序列绑定到 Text，默认在目标销毁时自动取消，并在出错时尝试重绑。
        /// </summary>
        public static void BindTo<T>(this IUniTaskAsyncEnumerable<T> source, UnityEngine.UI.Text text, bool rebindOnError = true)
        {
            BindToCore(source, text, text.GetCancellationTokenOnDestroy(), rebindOnError).Forget();
        }

        /// <summary>
        /// 将泛型异步序列绑定到 Text，可自定义取消令牌与错误重绑行为。
        /// </summary>
        public static void BindTo<T>(this IUniTaskAsyncEnumerable<T> source, UnityEngine.UI.Text text, CancellationToken cancellationToken, bool rebindOnError = true)
        {
            BindToCore(source, text, cancellationToken, rebindOnError).Forget();
        }

        /// <summary>
        /// 将响应式属性绑定到 Text，默认在目标销毁时自动取消，并在出错时尝试重绑。
        /// </summary>
        public static void BindTo<T>(this AsyncReactiveProperty<T> source, UnityEngine.UI.Text text, bool rebindOnError = true)
        {
            BindToCore(source, text, text.GetCancellationTokenOnDestroy(), rebindOnError).Forget();
        }

        /// <summary>
        /// 具体的绑定实现：消费泛型异步序列，将每次 Current 写入 text.text；支持在异常时按需重绑。
        /// </summary>
        static async UniTaskVoid BindToCore<T>(IUniTaskAsyncEnumerable<T> source, UnityEngine.UI.Text text, CancellationToken cancellationToken, bool rebindOnError)
        {
            var repeat = false; // 是否重复绑定
            BIND_AGAIN:
            var e = source.GetAsyncEnumerator(cancellationToken); // 获取异步枚举器
            try
            {
                while (true)
                {
                    bool moveNext; // 是否移动到下一个
                    try
                    {
                        moveNext = await e.MoveNextAsync();
                        repeat = false; // 不重复绑定
                    }
                    catch (Exception ex)
                    {
                        if (ex is OperationCanceledException) return;

                        if (rebindOnError && !repeat)
                        {
                            repeat = true;
                            goto BIND_AGAIN;
                        }
                        else
                        {
                            throw;
                        }
                    }

                    if (!moveNext) return;

                    text.text = e.Current.ToString();
                }
            }
            finally
            {
                if (e != null)
                {
                    await e.DisposeAsync();
                }
            }
        }

        // <bool> -> Selectable

        /// <summary>
        /// 将布尔异步序列绑定到 Selectable，默认在目标销毁时自动取消，并在出错时尝试重绑。
        /// </summary>
        public static void BindTo(this IUniTaskAsyncEnumerable<bool> source, Selectable selectable, bool rebindOnError = true)
        {
            BindToCore(source, selectable, selectable.GetCancellationTokenOnDestroy(), rebindOnError).Forget();
        }

        /// <summary>
        /// 将布尔异步序列绑定到 Selectable，可自定义取消令牌与错误重绑行为。
        /// </summary>
        public static void BindTo(this IUniTaskAsyncEnumerable<bool> source, Selectable selectable, CancellationToken cancellationToken, bool rebindOnError = true)
        {
            BindToCore(source, selectable, cancellationToken, rebindOnError).Forget();
        }

        /// <summary>
        /// 具体的绑定实现：消费布尔异步序列，将每次 Current 写入 selectable.interactable；支持在异常时按需重绑。
        /// </summary>
        static async UniTaskVoid BindToCore(IUniTaskAsyncEnumerable<bool> source, Selectable selectable, CancellationToken cancellationToken, bool rebindOnError)
        {
            var repeat = false;
            BIND_AGAIN:
            var e = source.GetAsyncEnumerator(cancellationToken);
            try
            {
                while (true)
                {
                    bool moveNext;
                    try
                    {
                        moveNext = await e.MoveNextAsync();
                        repeat = false;
                    }
                    catch (Exception ex)
                    {
                        if (ex is OperationCanceledException) return;

                        if (rebindOnError && !repeat)
                        {
                            repeat = true;
                            goto BIND_AGAIN;
                        }
                        else
                        {
                            throw;
                        }
                    }

                    if (!moveNext) return;


                    selectable.interactable = e.Current;
                }
            }
            finally
            {
                if (e != null)
                {
                    await e.DisposeAsync();
                }
            }
        }
#endif

        // <T> -> Action

        /// <summary>
        /// 将泛型异步序列绑定到 MonoBehaviour，默认在目标销毁时自动取消，并在出错时尝试重绑。
        /// </summary>
        public static void BindTo<TSource, TObject>(this IUniTaskAsyncEnumerable<TSource> source, TObject monoBehaviour, Action<TObject, TSource> bindAction, bool rebindOnError = true)
            where TObject : MonoBehaviour
        {
            BindToCore(source, monoBehaviour, bindAction, monoBehaviour.GetCancellationTokenOnDestroy(), rebindOnError).Forget();
        }

        /// <summary>
        /// 将泛型异步序列绑定到 MonoBehaviour，可自定义取消令牌与错误重绑行为。
        /// </summary>
        public static void BindTo<TSource, TObject>(this IUniTaskAsyncEnumerable<TSource> source, TObject bindTarget, Action<TObject, TSource> bindAction, CancellationToken cancellationToken, bool rebindOnError = true)
        {
            BindToCore(source, bindTarget, bindAction, cancellationToken, rebindOnError).Forget();
        }
        /// <summary>
        /// 具体的绑定实现：消费泛型异步序列，将每次 Current 写入 bindTarget；支持在异常时按需重绑。
        /// </summary>
        static async UniTaskVoid BindToCore<TSource, TObject>(IUniTaskAsyncEnumerable<TSource> source, TObject bindTarget, Action<TObject, TSource> bindAction, CancellationToken cancellationToken, bool rebindOnError)
        {
            var repeat = false; // 是否重复绑定
            BIND_AGAIN:
            var e = source.GetAsyncEnumerator(cancellationToken); // 获取异步枚举器
            try
            {
                while (true)
                {
                    bool moveNext; // 是否移动到下一个
                    try
                    {
                        moveNext = await e.MoveNextAsync();
                        repeat = false;
                    }
                    catch (Exception ex)
                    {
                        if (ex is OperationCanceledException) return;

                        if (rebindOnError && !repeat)
                        {
                            repeat = true;
                            goto BIND_AGAIN;
                        }
                        else
                        {
                            throw;
                        }
                    }

                    if (!moveNext) return;

                    bindAction(bindTarget, e.Current);
                }
            }
            finally
            {
                if (e != null)
                {
                    await e.DisposeAsync();
                }
            }
        }
    }
}
