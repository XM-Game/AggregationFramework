#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#if !UNITY_2019_1_OR_NEWER || UNITASK_UGUI_SUPPORT
using System;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Cysharp.Threading.Tasks
{
    /// <summary>
    /// UnityAsyncExtensions 类
    /// 提供将 UnityEngine.UI.EventSystems 转换为 UniTask 的扩展方法 （用于 UGUI）
    /// </summary>
    public static partial class UnityAsyncExtensions
    {
        /// <summary>
        /// 将 UnityEngine.UI.EventSystems 转换为 UniTask
        /// </summary>
        public static AsyncUnityEventHandler GetAsyncEventHandler(this UnityEvent unityEvent, CancellationToken cancellationToken)
        {
            return new AsyncUnityEventHandler(unityEvent, cancellationToken, false);
        }
        /// <summary>
        /// 将 UnityEngine.UI.EventSystems 转换为 UniTask
        /// </summary>
        public static UniTask OnInvokeAsync(this UnityEvent unityEvent, CancellationToken cancellationToken)
        {
            return new AsyncUnityEventHandler(unityEvent, cancellationToken, true).OnInvokeAsync();
        }

        /// <summary>
        /// 将 UnityEngine.UI.EventSystems 转换为 UniTask （流式消费）
        /// </summary>
        public static IUniTaskAsyncEnumerable<AsyncUnit> OnInvokeAsAsyncEnumerable(this UnityEvent unityEvent, CancellationToken cancellationToken)
        {
            return new UnityEventHandlerAsyncEnumerable(unityEvent, cancellationToken);
        }

        /// <summary>
        /// 将 UnityEngine.UI.EventSystems 转换为 UniTask （流式消费）
        /// </summary>
        public static AsyncUnityEventHandler<T> GetAsyncEventHandler<T>(this UnityEvent<T> unityEvent, CancellationToken cancellationToken)
        {
            return new AsyncUnityEventHandler<T>(unityEvent, cancellationToken, false);
        }

        /// <summary>
        /// 将 UnityEngine.UI.EventSystems 转换为 UniTask （带取消令牌）
        /// </summary>
        public static UniTask<T> OnInvokeAsync<T>(this UnityEvent<T> unityEvent, CancellationToken cancellationToken)
        {
            return new AsyncUnityEventHandler<T>(unityEvent, cancellationToken, true).OnInvokeAsync();
        }

        /// <summary>
        /// 将 UnityEngine.UI.EventSystems 转换为 UniTask （流式消费）
        /// </summary>
        public static IUniTaskAsyncEnumerable<T> OnInvokeAsAsyncEnumerable<T>(this UnityEvent<T> unityEvent, CancellationToken cancellationToken)
        {
            return new UnityEventHandlerAsyncEnumerable<T>(unityEvent, cancellationToken);
        }

        /// <summary>
        /// 将 UnityEngine.UI.Button 转换为 UniTask （不使用 callOnce 模式）
        /// </summary>
        public static IAsyncClickEventHandler GetAsyncClickEventHandler(this Button button)
        {
            return new AsyncUnityEventHandler(button.onClick, button.GetCancellationTokenOnDestroy(), false);
        }

        /// <summary>
        /// 将 UnityEngine.UI.Button 转换为 UniTask （带取消令牌）
        /// </summary>
        public static IAsyncClickEventHandler GetAsyncClickEventHandler(this Button button, CancellationToken cancellationToken)
        {
            return new AsyncUnityEventHandler(button.onClick, cancellationToken, false);
        }

        /// <summary>
        /// 将 UnityEngine.UI.Button 转换为 UniTask （流式消费）
        /// </summary>
        public static UniTask OnClickAsync(this Button button)
        {
            return new AsyncUnityEventHandler(button.onClick, button.GetCancellationTokenOnDestroy(), true).OnInvokeAsync();
        }

        /// <summary>
        /// 将 UnityEngine.UI.Button 转换为 UniTask （带取消令牌）
        /// </summary>
        public static UniTask OnClickAsync(this Button button, CancellationToken cancellationToken)
        {
            return new AsyncUnityEventHandler(button.onClick, cancellationToken, true).OnInvokeAsync();
        }

        /// <summary>
        /// 将 UnityEngine.UI.Button 转换为 UniTask （流式消费）
        /// </summary>
        public static IUniTaskAsyncEnumerable<AsyncUnit> OnClickAsAsyncEnumerable(this Button button)
        {
            return new UnityEventHandlerAsyncEnumerable(button.onClick, button.GetCancellationTokenOnDestroy());
        }

        public static IUniTaskAsyncEnumerable<AsyncUnit> OnClickAsAsyncEnumerable(this Button button, CancellationToken cancellationToken)
        {
            return new UnityEventHandlerAsyncEnumerable(button.onClick, cancellationToken);
        }

        /// <summary>
        /// 将 UnityEngine.UI.Toggle 转换为 UniTask （不使用 callOnce 模式）
        /// </summary>
        public static IAsyncValueChangedEventHandler<bool> GetAsyncValueChangedEventHandler(this Toggle toggle)
        {
            return new AsyncUnityEventHandler<bool>(toggle.onValueChanged, toggle.GetCancellationTokenOnDestroy(), false);
        }

        /// <summary>
        /// 将 UnityEngine.UI.Toggle 转换为 UniTask （带取消令牌）
        /// </summary>
        public static IAsyncValueChangedEventHandler<bool> GetAsyncValueChangedEventHandler(this Toggle toggle, CancellationToken cancellationToken)
        {
            return new AsyncUnityEventHandler<bool>(toggle.onValueChanged, cancellationToken, false);
        }

        /// <summary>
        /// 将 UnityEngine.UI.Toggle 转换为 UniTask （流式消费）
        /// </summary>
        public static UniTask<bool> OnValueChangedAsync(this Toggle toggle)
        {
            return new AsyncUnityEventHandler<bool>(toggle.onValueChanged, toggle.GetCancellationTokenOnDestroy(), true).OnInvokeAsync();
        }

        /// <summary>
        /// 将 UnityEngine.UI.Toggle 转换为 UniTask （带取消令牌）
        /// </summary>
        public static UniTask<bool> OnValueChangedAsync(this Toggle toggle, CancellationToken cancellationToken)
        {
            return new AsyncUnityEventHandler<bool>(toggle.onValueChanged, cancellationToken, true).OnInvokeAsync();
        }

        /// <summary>
        /// 将 UnityEngine.UI.Toggle 转换为 UniTask （流式消费）
        /// </summary>
        public static IUniTaskAsyncEnumerable<bool> OnValueChangedAsAsyncEnumerable(this Toggle toggle)
        {
            return new UnityEventHandlerAsyncEnumerable<bool>(toggle.onValueChanged, toggle.GetCancellationTokenOnDestroy());
        }

        /// <summary>
        /// 将 UnityEngine.UI.Toggle 转换为 UniTask （流式消费）
        /// </summary>
        public static IUniTaskAsyncEnumerable<bool> OnValueChangedAsAsyncEnumerable(this Toggle toggle, CancellationToken cancellationToken)
        {
            return new UnityEventHandlerAsyncEnumerable<bool>(toggle.onValueChanged, cancellationToken);
        }

        /// <summary>
        /// 将 UnityEngine.UI.Scrollbar 转换为 UniTask （不使用 callOnce 模式）
        /// </summary>
        public static IAsyncValueChangedEventHandler<float> GetAsyncValueChangedEventHandler(this Scrollbar scrollbar)
        {
            return new AsyncUnityEventHandler<float>(scrollbar.onValueChanged, scrollbar.GetCancellationTokenOnDestroy(), false);
        }

        /// <summary>
        /// 将 UnityEngine.UI.Scrollbar 转换为 UniTask （带取消令牌）
        /// </summary>
        public static IAsyncValueChangedEventHandler<float> GetAsyncValueChangedEventHandler(this Scrollbar scrollbar, CancellationToken cancellationToken)
        {
            return new AsyncUnityEventHandler<float>(scrollbar.onValueChanged, cancellationToken, false);
        }

        /// <summary>
        /// 将 UnityEngine.UI.Scrollbar 转换为 UniTask （流式消费）
        /// </summary>
        public static UniTask<float> OnValueChangedAsync(this Scrollbar scrollbar)
        {
            return new AsyncUnityEventHandler<float>(scrollbar.onValueChanged, scrollbar.GetCancellationTokenOnDestroy(), true).OnInvokeAsync();
        }

        /// <summary>
        /// 将 UnityEngine.UI.Scrollbar 转换为 UniTask （带取消令牌）
        /// </summary>
        public static UniTask<float> OnValueChangedAsync(this Scrollbar scrollbar, CancellationToken cancellationToken)
        {
            return new AsyncUnityEventHandler<float>(scrollbar.onValueChanged, cancellationToken, true).OnInvokeAsync();
        }

        /// <summary>
        /// 将 UnityEngine.UI.Scrollbar 转换为 UniTask （流式消费）
        /// </summary>
        public static IUniTaskAsyncEnumerable<float> OnValueChangedAsAsyncEnumerable(this Scrollbar scrollbar)
        {
            return new UnityEventHandlerAsyncEnumerable<float>(scrollbar.onValueChanged, scrollbar.GetCancellationTokenOnDestroy());
        }

        /// <summary>
        /// 将 UnityEngine.UI.Scrollbar 转换为 UniTask （流式消费）
        /// </summary>
        public static IUniTaskAsyncEnumerable<float> OnValueChangedAsAsyncEnumerable(this Scrollbar scrollbar, CancellationToken cancellationToken)
        {
            return new UnityEventHandlerAsyncEnumerable<float>(scrollbar.onValueChanged, cancellationToken);
        }

        /// <summary>
        /// 将 UnityEngine.UI.ScrollRect 转换为 UniTask （不使用 callOnce 模式）
        /// </summary>
        public static IAsyncValueChangedEventHandler<Vector2> GetAsyncValueChangedEventHandler(this ScrollRect scrollRect)
        {
            return new AsyncUnityEventHandler<Vector2>(scrollRect.onValueChanged, scrollRect.GetCancellationTokenOnDestroy(), false);
        }

        /// <summary>
        /// 将 UnityEngine.UI.ScrollRect 转换为 UniTask （带取消令牌）
        /// </summary>
        public static IAsyncValueChangedEventHandler<Vector2> GetAsyncValueChangedEventHandler(this ScrollRect scrollRect, CancellationToken cancellationToken)
        {
            return new AsyncUnityEventHandler<Vector2>(scrollRect.onValueChanged, cancellationToken, false);
        }

        /// <summary>
        /// 将 UnityEngine.UI.ScrollRect 转换为 UniTask （流式消费）
        /// </summary>
        public static UniTask<Vector2> OnValueChangedAsync(this ScrollRect scrollRect)
        {
            return new AsyncUnityEventHandler<Vector2>(scrollRect.onValueChanged, scrollRect.GetCancellationTokenOnDestroy(), true).OnInvokeAsync();
        }

        /// <summary>
        /// 将 UnityEngine.UI.ScrollRect 转换为 UniTask （带取消令牌）
        /// </summary>
        public static UniTask<Vector2> OnValueChangedAsync(this ScrollRect scrollRect, CancellationToken cancellationToken)
        {
            return new AsyncUnityEventHandler<Vector2>(scrollRect.onValueChanged, cancellationToken, true).OnInvokeAsync();
        }

        /// <summary>
        /// 将 UnityEngine.UI.ScrollRect 转换为 UniTask （流式消费）
        /// </summary>
        public static IUniTaskAsyncEnumerable<Vector2> OnValueChangedAsAsyncEnumerable(this ScrollRect scrollRect)
        {
            return new UnityEventHandlerAsyncEnumerable<Vector2>(scrollRect.onValueChanged, scrollRect.GetCancellationTokenOnDestroy());
        }

        /// <summary>
        /// 将 UnityEngine.UI.ScrollRect 转换为 UniTask （流式消费）
        /// </summary>
        public static IUniTaskAsyncEnumerable<Vector2> OnValueChangedAsAsyncEnumerable(this ScrollRect scrollRect, CancellationToken cancellationToken)
        {
            return new UnityEventHandlerAsyncEnumerable<Vector2>(scrollRect.onValueChanged, cancellationToken);
        }

        /// <summary>
        /// 将 UnityEngine.UI.Slider 转换为 UniTask （不使用 callOnce 模式）
        /// </summary>
        public static IAsyncValueChangedEventHandler<float> GetAsyncValueChangedEventHandler(this Slider slider)
        {
            return new AsyncUnityEventHandler<float>(slider.onValueChanged, slider.GetCancellationTokenOnDestroy(), false);
        }

        /// <summary>
        /// 将 UnityEngine.UI.Slider 转换为 UniTask （带取消令牌）
        /// </summary>
        public static IAsyncValueChangedEventHandler<float> GetAsyncValueChangedEventHandler(this Slider slider, CancellationToken cancellationToken)
        {
            return new AsyncUnityEventHandler<float>(slider.onValueChanged, cancellationToken, false);
        }

        /// <summary>
        /// 将 UnityEngine.UI.Slider 转换为 UniTask （流式消费）
        /// </summary>
        public static UniTask<float> OnValueChangedAsync(this Slider slider)
        {
            return new AsyncUnityEventHandler<float>(slider.onValueChanged, slider.GetCancellationTokenOnDestroy(), true).OnInvokeAsync();
        }

        /// <summary>
        /// 将 UnityEngine.UI.Slider 转换为 UniTask （带取消令牌）
        /// </summary>
        public static UniTask<float> OnValueChangedAsync(this Slider slider, CancellationToken cancellationToken)
        {
            return new AsyncUnityEventHandler<float>(slider.onValueChanged, cancellationToken, true).OnInvokeAsync();
        }

        /// <summary>
        /// 将 UnityEngine.UI.Slider 转换为 UniTask （流式消费）
        /// </summary>
        public static IUniTaskAsyncEnumerable<float> OnValueChangedAsAsyncEnumerable(this Slider slider)
        {
            return new UnityEventHandlerAsyncEnumerable<float>(slider.onValueChanged, slider.GetCancellationTokenOnDestroy());
        }

        /// <summary>
        /// 将 UnityEngine.UI.Slider 转换为 UniTask （流式消费）
        /// </summary>
        public static IUniTaskAsyncEnumerable<float> OnValueChangedAsAsyncEnumerable(this Slider slider, CancellationToken cancellationToken)
        {
            return new UnityEventHandlerAsyncEnumerable<float>(slider.onValueChanged, cancellationToken);
        }

        /// <summary>
        /// 将 UnityEngine.UI.InputField 转换为 UniTask （不使用 callOnce 模式）
        /// </summary>
        public static IAsyncEndEditEventHandler<string> GetAsyncEndEditEventHandler(this InputField inputField)
        {
            return new AsyncUnityEventHandler<string>(inputField.onEndEdit, inputField.GetCancellationTokenOnDestroy(), false);
        }

        /// <summary>
        /// 将 UnityEngine.UI.InputField 转换为 UniTask （带取消令牌）
        /// </summary>
        public static IAsyncEndEditEventHandler<string> GetAsyncEndEditEventHandler(this InputField inputField, CancellationToken cancellationToken)
        {
            return new AsyncUnityEventHandler<string>(inputField.onEndEdit, cancellationToken, false);
        }

        /// <summary>
        /// 将 UnityEngine.UI.InputField 转换为 UniTask （流式消费）
        /// </summary>
        public static UniTask<string> OnEndEditAsync(this InputField inputField)
        {
            return new AsyncUnityEventHandler<string>(inputField.onEndEdit, inputField.GetCancellationTokenOnDestroy(), true).OnInvokeAsync();
        }

        /// <summary>
        /// 将 UnityEngine.UI.InputField 转换为 UniTask （带取消令牌）
        /// </summary>
        public static UniTask<string> OnEndEditAsync(this InputField inputField, CancellationToken cancellationToken)
        {
            return new AsyncUnityEventHandler<string>(inputField.onEndEdit, cancellationToken, true).OnInvokeAsync();
        }

        /// <summary>
        /// 将 UnityEngine.UI.InputField 转换为 UniTask （流式消费）
        /// </summary>
        public static IUniTaskAsyncEnumerable<string> OnEndEditAsAsyncEnumerable(this InputField inputField)
        {
            return new UnityEventHandlerAsyncEnumerable<string>(inputField.onEndEdit, inputField.GetCancellationTokenOnDestroy());
        }

        /// <summary>
        /// 将 UnityEngine.UI.InputField 转换为 UniTask （流式消费）
        /// </summary>
        public static IUniTaskAsyncEnumerable<string> OnEndEditAsAsyncEnumerable(this InputField inputField, CancellationToken cancellationToken)
        {
            return new UnityEventHandlerAsyncEnumerable<string>(inputField.onEndEdit, cancellationToken);
        }

        /// <summary>
        /// 将 UnityEngine.UI.InputField 转换为 UniTask （不使用 callOnce 模式）
        /// </summary>
        public static IAsyncValueChangedEventHandler<string> GetAsyncValueChangedEventHandler(this InputField inputField)
        {
            return new AsyncUnityEventHandler<string>(inputField.onValueChanged, inputField.GetCancellationTokenOnDestroy(), false);
        }

        /// <summary>
        /// 将 UnityEngine.UI.InputField 转换为 UniTask （带取消令牌）
        /// </summary>
        public static IAsyncValueChangedEventHandler<string> GetAsyncValueChangedEventHandler(this InputField inputField, CancellationToken cancellationToken)
        {
            return new AsyncUnityEventHandler<string>(inputField.onValueChanged, cancellationToken, false);
        }

        /// <summary>
        /// 将 UnityEngine.UI.InputField 转换为 UniTask （流式消费）
        /// </summary>
        public static UniTask<string> OnValueChangedAsync(this InputField inputField)
        {
            return new AsyncUnityEventHandler<string>(inputField.onValueChanged, inputField.GetCancellationTokenOnDestroy(), true).OnInvokeAsync();
        }

        /// <summary>
        /// 将 UnityEngine.UI.InputField 转换为 UniTask （带取消令牌）
        /// </summary>
        public static UniTask<string> OnValueChangedAsync(this InputField inputField, CancellationToken cancellationToken)
        {
            return new AsyncUnityEventHandler<string>(inputField.onValueChanged, cancellationToken, true).OnInvokeAsync();
        }

        /// <summary>
        /// 将 UnityEngine.UI.InputField 转换为 UniTask （流式消费）
        /// </summary>
        public static IUniTaskAsyncEnumerable<string> OnValueChangedAsAsyncEnumerable(this InputField inputField)
        {
            return new UnityEventHandlerAsyncEnumerable<string>(inputField.onValueChanged, inputField.GetCancellationTokenOnDestroy());
        }

        /// <summary>
        /// 将 UnityEngine.UI.InputField 转换为 UniTask （流式消费）
        /// </summary>
        public static IUniTaskAsyncEnumerable<string> OnValueChangedAsAsyncEnumerable(this InputField inputField, CancellationToken cancellationToken)
        {
            return new UnityEventHandlerAsyncEnumerable<string>(inputField.onValueChanged, cancellationToken);
        }

        /// <summary>
        /// 将 UnityEngine.UI.Dropdown 转换为 UniTask （不使用 callOnce 模式）
        /// </summary>
        public static IAsyncValueChangedEventHandler<int> GetAsyncValueChangedEventHandler(this Dropdown dropdown)
        {
            return new AsyncUnityEventHandler<int>(dropdown.onValueChanged, dropdown.GetCancellationTokenOnDestroy(), false);
        }

        /// <summary>
        /// 将 UnityEngine.UI.Dropdown 转换为 UniTask （带取消令牌）
        /// </summary>
        public static IAsyncValueChangedEventHandler<int> GetAsyncValueChangedEventHandler(this Dropdown dropdown, CancellationToken cancellationToken)
        {
            return new AsyncUnityEventHandler<int>(dropdown.onValueChanged, cancellationToken, false);
        }

        /// <summary>
        /// 将 UnityEngine.UI.Dropdown 转换为 UniTask （流式消费）
        /// </summary>
        public static UniTask<int> OnValueChangedAsync(this Dropdown dropdown)
        {
            return new AsyncUnityEventHandler<int>(dropdown.onValueChanged, dropdown.GetCancellationTokenOnDestroy(), true).OnInvokeAsync();
        }

        /// <summary>
        /// 将 UnityEngine.UI.Dropdown 转换为 UniTask （带取消令牌）
        /// </summary>
        public static UniTask<int> OnValueChangedAsync(this Dropdown dropdown, CancellationToken cancellationToken)
        {
            return new AsyncUnityEventHandler<int>(dropdown.onValueChanged, cancellationToken, true).OnInvokeAsync();
        }

        /// <summary>
        /// 将 UnityEngine.UI.Dropdown 转换为 UniTask （流式消费）
        /// </summary>
        public static IUniTaskAsyncEnumerable<int> OnValueChangedAsAsyncEnumerable(this Dropdown dropdown)
        {
            return new UnityEventHandlerAsyncEnumerable<int>(dropdown.onValueChanged, dropdown.GetCancellationTokenOnDestroy());
        }

        /// <summary>
        /// 将 UnityEngine.UI.Dropdown 转换为 UniTask （流式消费）
        /// </summary>
        public static IUniTaskAsyncEnumerable<int> OnValueChangedAsAsyncEnumerable(this Dropdown dropdown, CancellationToken cancellationToken)
        {
            return new UnityEventHandlerAsyncEnumerable<int>(dropdown.onValueChanged, cancellationToken);
        }
    }

    /// <summary>
    /// 异步点击事件处理器接口
    /// </summary>
    public interface IAsyncClickEventHandler : IDisposable
    {
        UniTask OnClickAsync(); // 等待下一次点击事件
    }

    /// <summary>
    /// 异步值改变事件处理器接口
    /// </summary>
    public interface IAsyncValueChangedEventHandler<T> : IDisposable
    {
        UniTask<T> OnValueChangedAsync(); // 等待下一次值改变事件
    }

    /// <summary>
    /// 异步结束编辑事件处理器接口
    /// </summary>
    public interface IAsyncEndEditEventHandler<T> : IDisposable
    {
        UniTask<T> OnEndEditAsync(); // 等待下一次结束编辑事件
    }

    // for TMP_PRO
    /// <summary>
    /// 异步结束文本选择事件处理器接口
    /// </summary>
    public interface IAsyncEndTextSelectionEventHandler<T> : IDisposable
    {
        UniTask<T> OnEndTextSelectionAsync(); // 等待下一次结束文本选择事件
    }

    /// <summary>
    /// 异步文本选择事件处理器接口
    /// </summary>
    public interface IAsyncTextSelectionEventHandler<T> : IDisposable
    {
        UniTask<T> OnTextSelectionAsync(); // 等待下一次文本选择事件
    }

    /// <summary>
    /// 异步取消选择事件处理器接口
    /// </summary>
    public interface IAsyncDeselectEventHandler<T> : IDisposable
    {
        UniTask<T> OnDeselectAsync(); // 等待下一次取消选择事件
    }

    /// <summary>
    /// 异步选择事件处理器接口
    /// </summary>
    public interface IAsyncSelectEventHandler<T> : IDisposable
    {
        UniTask<T> OnSelectAsync(); // 等待下一次选择事件
    }

    /// <summary>
    /// 异步提交事件处理器接口
    /// </summary>
    public interface IAsyncSubmitEventHandler<T> : IDisposable
    {
        UniTask<T> OnSubmitAsync(); // 等待下一次提交事件
    }

    /// <summary>
    /// 文本选择事件转换器
    /// </summary>
    internal class TextSelectionEventConverter : UnityEvent<(string, int, int)>, IDisposable
    {
        readonly UnityEvent<string, int, int> innerEvent;
        readonly UnityAction<string, int, int> invokeDelegate;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="unityEvent">UnityEvent</param>
        public TextSelectionEventConverter(UnityEvent<string, int, int> unityEvent)
        {
            this.innerEvent = unityEvent;
            this.invokeDelegate = InvokeCore;

            innerEvent.AddListener(invokeDelegate);
        }

        /// <summary>
        /// 调用核心
        /// </summary>
        /// <param name="item1">item1</param>
        /// <param name="item2">item2</param>
        /// <param name="item3">item3</param>
        void InvokeCore(string item1, int item2, int item3)
        {
            Invoke((item1, item2, item3));
        }

        /// <summary>
        /// 释放
        /// </summary>
        public void Dispose()
        {
            innerEvent.RemoveListener(invokeDelegate);
        }
    }

    /// <summary>
    /// 异步Unity事件处理器
    /// </summary>
    public class AsyncUnityEventHandler : IUniTaskSource, IDisposable, IAsyncClickEventHandler
    {
        static Action<object> cancellationCallback = CancellationCallback; // 取消回调

        readonly UnityAction action; // 动作
        readonly UnityEvent unityEvent; // UnityEvent

        CancellationToken cancellationToken;
        CancellationTokenRegistration registration; // 取消注册
        bool isDisposed;
        bool callOnce; // 是否一次性

        UniTaskCompletionSourceCore<AsyncUnit> core; // 核心

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="unityEvent">UnityEvent</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <param name="callOnce">是否一次性</param>
        public AsyncUnityEventHandler(UnityEvent unityEvent, CancellationToken cancellationToken, bool callOnce)
        {
            this.cancellationToken = cancellationToken;
            if (cancellationToken.IsCancellationRequested)
            {
                isDisposed = true;
                return;
            }

            this.action = Invoke;
            this.unityEvent = unityEvent;
            this.callOnce = callOnce;

            unityEvent.AddListener(action);

            if (cancellationToken.CanBeCanceled)
            {
                registration = cancellationToken.RegisterWithoutCaptureExecutionContext(cancellationCallback, this);
            }

            TaskTracker.TrackActiveTask(this, 3);
        }

        /// <summary>
        /// 等待下一次事件
        /// </summary>
        /// <returns>UniTask</returns>
        public UniTask OnInvokeAsync()
        {
            core.Reset();
            if (isDisposed)
            {
                core.TrySetCanceled(this.cancellationToken);
            }
            return new UniTask(this, core.Version);
        }

        /// <summary>
        /// 调用
        /// </summary>
        void Invoke()
        {
            core.TrySetResult(AsyncUnit.Default);
        }

        /// <summary>
        /// 取消回调
        /// </summary>
        /// <param name="state">状态</param>
        static void CancellationCallback(object state)
        {
            var self = (AsyncUnityEventHandler)state;
            self.Dispose();
        }

        /// <summary>
        /// 释放
        /// </summary>
        public void Dispose()
        {
            if (!isDisposed)
            {
                isDisposed = true;
                TaskTracker.RemoveTracking(this);
                registration.Dispose();
                if (unityEvent != null)
                {
                    unityEvent.RemoveListener(action);
                }
                core.TrySetCanceled(cancellationToken);
            }
        }

        /// <summary>
        /// 等待下一次点击事件
        /// </summary>
        /// <returns>UniTask</returns>
        UniTask IAsyncClickEventHandler.OnClickAsync()
        {
            return OnInvokeAsync();
        }

        /// <summary>
        /// 获取结果
        /// </summary>
        /// <param name="token">令牌</param>
        void IUniTaskSource.GetResult(short token)
        {
            try
            {
                core.GetResult(token);
            }
            finally
            {
                if (callOnce)
                {
                    Dispose();
                }
            }
        }

        /// <summary>
        /// 获取状态
        /// </summary>
        /// <param name="token">令牌</param>
        UniTaskStatus IUniTaskSource.GetStatus(short token)
        {
            return core.GetStatus(token);
        }

        /// <summary>
        /// 获取不安全状态
        /// </summary>
        /// <returns>UniTaskStatus</returns>
        UniTaskStatus IUniTaskSource.UnsafeGetStatus()
        {
            return core.UnsafeGetStatus();
        }

        /// <summary>
        /// 完成
        /// </summary>
        /// <param name="continuation">继续</param>
        /// <param name="state">状态</param>
        /// <param name="token">令牌</param>
        void IUniTaskSource.OnCompleted(Action<object> continuation, object state, short token)
        {
            core.OnCompleted(continuation, state, token);
        }
    }

    /// <summary>
    /// 异步Unity事件处理器 <T>
    /// </summary>
    public class AsyncUnityEventHandler<T> : IUniTaskSource<T>, IDisposable, IAsyncValueChangedEventHandler<T>, IAsyncEndEditEventHandler<T>
        , IAsyncEndTextSelectionEventHandler<T>, IAsyncTextSelectionEventHandler<T>, IAsyncDeselectEventHandler<T>, IAsyncSelectEventHandler<T>, IAsyncSubmitEventHandler<T>
    {
        static Action<object> cancellationCallback = CancellationCallback; // 取消回调

        readonly UnityAction<T> action; // 动作
        readonly UnityEvent<T> unityEvent; // UnityEvent <T>

        CancellationToken cancellationToken; // 取消令牌
        CancellationTokenRegistration registration; // 取消注册
        bool isDisposed; // 是否已销毁
        bool callOnce; // 是否一次性

        UniTaskCompletionSourceCore<T> core; // 核心

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="unityEvent">UnityEvent <T></param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <param name="callOnce">是否一次性</param>
        public AsyncUnityEventHandler(UnityEvent<T> unityEvent, CancellationToken cancellationToken, bool callOnce)
        {
            this.cancellationToken = cancellationToken;
            if (cancellationToken.IsCancellationRequested)
            {
                isDisposed = true;
                return;
            }

            this.action = Invoke;
            this.unityEvent = unityEvent;
            this.callOnce = callOnce;

            unityEvent.AddListener(action);

            if (cancellationToken.CanBeCanceled)
            {
                registration = cancellationToken.RegisterWithoutCaptureExecutionContext(cancellationCallback, this);
            }

            TaskTracker.TrackActiveTask(this, 3);
        }

        /// <summary>
        /// 等待下一次事件
        /// </summary>
        /// <returns>UniTask <T></returns>
        public UniTask<T> OnInvokeAsync()
        {
            core.Reset();
            if (isDisposed)
            {
                core.TrySetCanceled(this.cancellationToken);
            }
            return new UniTask<T>(this, core.Version);
        }

        /// <summary>
        /// 调用
        /// </summary>
        /// <param name="result">结果</param>
        void Invoke(T result)
        {
            core.TrySetResult(result);
        }

        /// <summary>
        /// 取消回调
        /// </summary>
        /// <param name="state">状态</param>
        static void CancellationCallback(object state)
        {
            var self = (AsyncUnityEventHandler<T>)state;
            self.Dispose();
        }

        /// <summary>
        /// 释放
        /// </summary>
        public void Dispose()
        {
            if (!isDisposed)
            {
                isDisposed = true;
                TaskTracker.RemoveTracking(this);
                registration.Dispose();
                if (unityEvent != null)
                {
                    // Dispose inner delegate for TextSelectionEventConverter
                    if (unityEvent is IDisposable disp)
                    {
                        disp.Dispose();
                    }

                    unityEvent.RemoveListener(action);
                }

                core.TrySetCanceled();
            }
        }

        /// <summary>
        /// 等待下一次值改变事件
        /// </summary>
        /// <returns>UniTask <T></returns>
        UniTask<T> IAsyncValueChangedEventHandler<T>.OnValueChangedAsync()
        {
            return OnInvokeAsync();
        }

        /// <summary>
        /// 等待下一次结束编辑事件
        /// </summary>
        /// <returns>UniTask <T></returns>
        UniTask<T> IAsyncEndEditEventHandler<T>.OnEndEditAsync()
        {
            return OnInvokeAsync();
        }

        /// <summary>
        /// 等待下一次结束文本选择事件
        /// </summary>
        /// <returns>UniTask <T></returns>
        UniTask<T> IAsyncEndTextSelectionEventHandler<T>.OnEndTextSelectionAsync()
        {
            return OnInvokeAsync();
        }

        /// <summary>
        /// 等待下一次文本选择事件
        /// </summary>
        /// <returns>UniTask <T></returns>
        UniTask<T> IAsyncTextSelectionEventHandler<T>.OnTextSelectionAsync()
        {
            return OnInvokeAsync();
        }

        /// <summary>
        /// 等待下一次取消选择事件
        /// </summary>
        /// <returns>UniTask <T></returns>
        UniTask<T> IAsyncDeselectEventHandler<T>.OnDeselectAsync()
        {
            return OnInvokeAsync();
        }

        /// <summary>
        /// 等待下一次选择事件
        /// </summary>
        /// <returns>UniTask <T></returns>
        UniTask<T> IAsyncSelectEventHandler<T>.OnSelectAsync()
        {
            return OnInvokeAsync();
        }

        /// <summary>
        /// 等待下一次提交事件
        /// </summary>
        /// <returns>UniTask <T></returns>
        UniTask<T> IAsyncSubmitEventHandler<T>.OnSubmitAsync()
        {
            return OnInvokeAsync();
        }

        /// <summary>
        /// 获取结果
        /// </summary>
        /// <param name="token">令牌</param>
        T IUniTaskSource<T>.GetResult(short token)
        {
            try
            {
                return core.GetResult(token);
            }
            finally
            {
                if (callOnce)
                {
                    Dispose();
                }
            }
        }

        /// <summary>
        /// 获取结果
        /// </summary>
        /// <param name="token">令牌</param>
        void IUniTaskSource.GetResult(short token)
        {
            ((IUniTaskSource<T>)this).GetResult(token);
        }

        /// <summary>
        /// 获取状态
        /// </summary>
        /// <param name="token">令牌</param>
        UniTaskStatus IUniTaskSource.GetStatus(short token)
        {
            return core.GetStatus(token);
        }

        /// <summary>
        /// 获取不安全状态
        /// </summary>
        /// <returns>UniTaskStatus</returns>
        UniTaskStatus IUniTaskSource.UnsafeGetStatus()
        {
            return core.UnsafeGetStatus();
        }

        /// <summary>
        /// 完成
        /// </summary>
        /// <param name="continuation">继续</param>
        /// <param name="state">状态</param>
        /// <param name="token">令牌</param>
        void IUniTaskSource.OnCompleted(Action<object> continuation, object state, short token)
        {
            core.OnCompleted(continuation, state, token);
        }
    }

    /// <summary>
    /// 异步Unity事件处理器异步枚举器
    /// </summary>
    public class UnityEventHandlerAsyncEnumerable : IUniTaskAsyncEnumerable<AsyncUnit>
    {
        readonly UnityEvent unityEvent;
        readonly CancellationToken cancellationToken1;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="unityEvent">UnityEvent</param>
        /// <param name="cancellationToken">取消令牌</param>
        public UnityEventHandlerAsyncEnumerable(UnityEvent unityEvent, CancellationToken cancellationToken)
        {
            this.unityEvent = unityEvent;
            this.cancellationToken1 = cancellationToken;
        }

        /// <summary>
        /// 获取异步枚举器
        /// </summary>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>IUniTaskAsyncEnumerator<AsyncUnit></returns>
        public IUniTaskAsyncEnumerator<AsyncUnit> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            if (this.cancellationToken1 == cancellationToken)
            {
                return new UnityEventHandlerAsyncEnumerator(unityEvent, this.cancellationToken1, CancellationToken.None);
            }
            else
            {
                return new UnityEventHandlerAsyncEnumerator(unityEvent, this.cancellationToken1, cancellationToken);
            }
        }

        /// <summary>
        /// 异步Unity事件处理器异步枚举器
        /// </summary>
        class UnityEventHandlerAsyncEnumerator : MoveNextSource, IUniTaskAsyncEnumerator<AsyncUnit>
        {
            static readonly Action<object> cancel1 = OnCanceled1; // 取消回调1
            static readonly Action<object> cancel2 = OnCanceled2; // 取消回调2

            readonly UnityEvent unityEvent; // UnityEvent
            CancellationToken cancellationToken1;
            CancellationToken cancellationToken2; // 取消令牌2

            UnityAction unityAction;
            CancellationTokenRegistration registration1; // 取消注册1
            CancellationTokenRegistration registration2; // 取消注册2
            bool isDisposed;

            /// <summary>
            /// 构造函数
            /// </summary>
            /// <param name="unityEvent">UnityEvent</param>
            /// <param name="cancellationToken1">取消令牌1</param>
            /// <param name="cancellationToken2">取消令牌2</param>
            public UnityEventHandlerAsyncEnumerator(UnityEvent unityEvent, CancellationToken cancellationToken1, CancellationToken cancellationToken2)
            {
                this.unityEvent = unityEvent;
                this.cancellationToken1 = cancellationToken1;
                this.cancellationToken2 = cancellationToken2;
            }

            /// <summary>
            /// 当前单元
            /// </summary>
            /// <returns>AsyncUnit</returns>
            public AsyncUnit Current => default;

            /// <summary>
            /// 移动到下一个
            /// </summary>
            /// <returns>UniTask<bool></returns>
            public UniTask<bool> MoveNextAsync()
            {
                cancellationToken1.ThrowIfCancellationRequested();
                cancellationToken2.ThrowIfCancellationRequested();
                completionSource.Reset();

                if (unityAction == null)
                {
                    unityAction = Invoke;

                    TaskTracker.TrackActiveTask(this, 3);
                    unityEvent.AddListener(unityAction);
                    if (cancellationToken1.CanBeCanceled)
                    {
                        registration1 = cancellationToken1.RegisterWithoutCaptureExecutionContext(cancel1, this);
                    }
                    if (cancellationToken2.CanBeCanceled)
                    {
                        registration2 = cancellationToken2.RegisterWithoutCaptureExecutionContext(cancel2, this);
                    }
                }

                return new UniTask<bool>(this, completionSource.Version);
            }

            /// <summary>
            /// 调用
            /// </summary>
            void Invoke()
            {
                completionSource.TrySetResult(true);
            }

            /// <summary>
            /// 取消回调1
            /// </summary>
            /// <param name="state">状态</param>
            static void OnCanceled1(object state)
            {
                var self = (UnityEventHandlerAsyncEnumerator)state;
                try
                {
                    self.completionSource.TrySetCanceled(self.cancellationToken1);
                }
                finally
                {
                    self.DisposeAsync().Forget();
                }
            }

            /// <summary>
            /// 取消回调2
            /// </summary>
            /// <param name="state">状态</param>
            static void OnCanceled2(object state)
            {
                var self = (UnityEventHandlerAsyncEnumerator)state;
                try
                {
                    self.completionSource.TrySetCanceled(self.cancellationToken2);
                }
                finally
                {
                    self.DisposeAsync().Forget();
                }
            }

            /// <summary>
            /// 释放
            /// </summary>
            /// <returns>UniTask</returns>
            public UniTask DisposeAsync()
            {
                if (!isDisposed)
                {
                    isDisposed = true;
                    TaskTracker.RemoveTracking(this);
                    registration1.Dispose();
                    registration2.Dispose();
                    unityEvent.RemoveListener(unityAction);

                    completionSource.TrySetCanceled();
                }

                return default;
            }
        }
    }

    /// <summary>
    /// 异步Unity事件处理器异步枚举器<T>
    /// </summary>
    public class UnityEventHandlerAsyncEnumerable<T> : IUniTaskAsyncEnumerable<T>
    {
        readonly UnityEvent<T> unityEvent;
        readonly CancellationToken cancellationToken1;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="unityEvent">UnityEvent <T></param>
        /// <param name="cancellationToken">取消令牌</param>
        public UnityEventHandlerAsyncEnumerable(UnityEvent<T> unityEvent, CancellationToken cancellationToken)
        {
            this.unityEvent = unityEvent;
            this.cancellationToken1 = cancellationToken;
        }

        /// <summary>
        /// 获取异步枚举器
        /// </summary>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>IUniTaskAsyncEnumerator<T></returns>
        public IUniTaskAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            if (this.cancellationToken1 == cancellationToken)
            {
                return new UnityEventHandlerAsyncEnumerator(unityEvent, this.cancellationToken1, CancellationToken.None);
            }
            else
            {
                return new UnityEventHandlerAsyncEnumerator(unityEvent, this.cancellationToken1, cancellationToken);
            }
        }

        /// <summary>
        /// 异步Unity事件处理器异步枚举器<T>
        /// </summary>
        class UnityEventHandlerAsyncEnumerator : MoveNextSource, IUniTaskAsyncEnumerator<T>
        {
            static readonly Action<object> cancel1 = OnCanceled1; // 取消回调1  
            static readonly Action<object> cancel2 = OnCanceled2; // 取消回调2

            readonly UnityEvent<T> unityEvent; // UnityEvent <T>
            CancellationToken cancellationToken1; // 取消令牌1
            CancellationToken cancellationToken2; // 取消令牌2

            UnityAction<T> unityAction; // UnityAction <T>
            CancellationTokenRegistration registration1; // 取消注册1
            CancellationTokenRegistration registration2; // 取消注册2
            bool isDisposed; // 是否已销毁

            /// <summary>
            /// 构造函数
            /// </summary>
            /// <param name="unityEvent">UnityEvent <T></param>
            /// <param name="cancellationToken1">取消令牌1</param>
            /// <param name="cancellationToken2">取消令牌2</param>
            public UnityEventHandlerAsyncEnumerator(UnityEvent<T> unityEvent, CancellationToken cancellationToken1, CancellationToken cancellationToken2)
            {
                this.unityEvent = unityEvent;
                this.cancellationToken1 = cancellationToken1;
                this.cancellationToken2 = cancellationToken2;
            }

            /// <summary>
            /// 当前单元
            /// </summary>
            /// <returns>T</returns>
            public T Current { get; private set; }

            /// <summary>
            /// 移动到下一个
            /// </summary>
            /// <returns>UniTask<bool></returns>
            public UniTask<bool> MoveNextAsync()
            {
                cancellationToken1.ThrowIfCancellationRequested();
                cancellationToken2.ThrowIfCancellationRequested();
                completionSource.Reset();

                if (unityAction == null)
                {
                    unityAction = Invoke;

                    TaskTracker.TrackActiveTask(this, 3);
                    unityEvent.AddListener(unityAction);
                    if (cancellationToken1.CanBeCanceled)
                    {
                        registration1 = cancellationToken1.RegisterWithoutCaptureExecutionContext(cancel1, this);
                    }
                    if (cancellationToken2.CanBeCanceled)
                    {
                        registration2 = cancellationToken2.RegisterWithoutCaptureExecutionContext(cancel2, this);
                    }
                }

                return new UniTask<bool>(this, completionSource.Version);
            }

            /// <summary>
            /// 调用
            /// </summary>
            /// <param name="value">值</param>
            void Invoke(T value)
            {
                Current = value;
                completionSource.TrySetResult(true);
            }

            /// <summary>
            /// 取消回调1
            /// </summary>
            /// <param name="state">状态</param>
            static void OnCanceled1(object state)
            {
                var self = (UnityEventHandlerAsyncEnumerator)state;
                try
                {
                    self.completionSource.TrySetCanceled(self.cancellationToken1);
                }
                finally
                {
                    self.DisposeAsync().Forget();
                }
            }

            /// <summary>
            /// 取消回调2
            /// </summary>
            /// <param name="state">状态</param>
            static void OnCanceled2(object state)
            {
                var self = (UnityEventHandlerAsyncEnumerator)state;
                try
                {
                    self.completionSource.TrySetCanceled(self.cancellationToken2);
                }
                finally
                {
                    self.DisposeAsync().Forget();
                }
            }

            /// <summary>
            /// 释放
            /// </summary>
            /// <returns>UniTask</returns>
            public UniTask DisposeAsync()
            {
                if (!isDisposed)
                {
                    isDisposed = true;
                    TaskTracker.RemoveTracking(this);
                    registration1.Dispose();
                    registration2.Dispose();
                    if (unityEvent is IDisposable disp)
                    {
                        disp.Dispose();
                    }
                    unityEvent.RemoveListener(unityAction);

                    completionSource.TrySetCanceled();
                }

                return default;
            }
        }
    }
}

#endif
