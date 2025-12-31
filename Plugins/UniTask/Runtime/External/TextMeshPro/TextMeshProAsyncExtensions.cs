#if UNITASK_TEXTMESHPRO_SUPPORT

using System;
using System.Threading;
using TMPro;
using UnityEngine.Events;

namespace Cysharp.Threading.Tasks
{
    /// <summary>
    /// TextMeshPro 相关的异步扩展。
    /// 提供将 IUniTaskAsyncEnumerable 绑定到 TMP_Text 的快捷方法，支持取消与错误自动重绑。
    /// </summary>
    public static partial class TextMeshProAsyncExtensions
    {
        // <string> -> Text
        /// <summary>
        /// 将字符串异步序列绑定到 TMP_Text，默认在目标销毁时自动取消，并在出错时尝试重绑。
        /// </summary>
        public static void BindTo(this IUniTaskAsyncEnumerable<string> source, TMP_Text text, bool rebindOnError = true)
        {
            BindToCore(source, text, text.GetCancellationTokenOnDestroy(), rebindOnError).Forget();
        }

        /// <summary>
        /// 将字符串异步序列绑定到 TMP_Text，可自定义取消令牌与错误重绑行为。
        /// </summary>
        public static void BindTo(this IUniTaskAsyncEnumerable<string> source, TMP_Text text, CancellationToken cancellationToken, bool rebindOnError = true)
        {
            BindToCore(source, text, cancellationToken, rebindOnError).Forget();
        }

        /// <summary>
        /// 具体的绑定实现：消费异步序列，将每次 Current 写入 text.text；支持在异常时按需重绑。
        /// </summary>
        static async UniTaskVoid BindToCore(IUniTaskAsyncEnumerable<string> source, TMP_Text text, CancellationToken cancellationToken, bool rebindOnError)
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

                    text.text = e.Current; // 将当前值写入 TMP_Text
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

        // <T> -> Text

        /// <summary>
        /// 将泛型异步序列绑定到 TMP_Text，使用 ToString 显示；默认随目标销毁自动取消。
        /// </summary>
        public static void BindTo<T>(this IUniTaskAsyncEnumerable<T> source, TMP_Text text, bool rebindOnError = true)
        {
            BindToCore(source, text, text.GetCancellationTokenOnDestroy(), rebindOnError).Forget();
        }

        /// <summary>
        /// 将泛型异步序列绑定到 TMP_Text，可自定义取消令牌与重绑策略。
        /// </summary>
        public static void BindTo<T>(this IUniTaskAsyncEnumerable<T> source, TMP_Text text, CancellationToken cancellationToken, bool rebindOnError = true)
        {
            BindToCore(source, text, cancellationToken, rebindOnError).Forget();
        }

        /// <summary>
        /// 将 ReactiveProperty 绑定到 TMP_Text，便于与 UniRx/AsyncReactiveProperty 联动。
        /// </summary>
        public static void BindTo<T>(this AsyncReactiveProperty<T> source, TMP_Text text, bool rebindOnError = true)
        {
            BindToCore(source, text, text.GetCancellationTokenOnDestroy(), rebindOnError).Forget();
        }

        /// <summary>
        /// 具体绑定实现：消费泛型异步序列，将 ToString 结果写入 TMP_Text；支持异常重绑。
        /// </summary>
        static async UniTaskVoid BindToCore<T>(IUniTaskAsyncEnumerable<T> source, TMP_Text text, CancellationToken cancellationToken, bool rebindOnError)
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

                    text.text = e.Current.ToString(); // 使用 ToString 写入
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

#endif