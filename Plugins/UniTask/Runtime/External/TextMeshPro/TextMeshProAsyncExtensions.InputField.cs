#if UNITASK_TEXTMESHPRO_SUPPORT

using System;
using System.Threading;
using TMPro;

namespace Cysharp.Threading.Tasks
{
    /// <summary>
    /// TMP_InputField 的异步扩展：将 UnityEvent 包装为 UniTask / IUniTaskAsyncEnumerable。
    /// 提供即时 await（OnInvokeAsync）与流式消费两种模式，均支持取消令牌与自动销毁取消。
    /// </summary>
    public static partial class TextMeshProAsyncExtensions
    {
        /// <summary>获取 onValueChanged 的异步事件处理器，销毁时自动取消。</summary>
        public static IAsyncValueChangedEventHandler<string> GetAsyncValueChangedEventHandler(this TMP_InputField inputField)
        {
            return new AsyncUnityEventHandler<string>(inputField.onValueChanged, inputField.GetCancellationTokenOnDestroy(), false);
        }

        /// <summary>获取 onValueChanged 的异步事件处理器，可自定义取消令牌。</summary>
        public static IAsyncValueChangedEventHandler<string> GetAsyncValueChangedEventHandler(this TMP_InputField inputField, CancellationToken cancellationToken)
        {
            return new AsyncUnityEventHandler<string>(inputField.onValueChanged, cancellationToken, false);
        }

        /// <summary>等待下一次 onValueChanged 触发，销毁时自动取消。</summary>
        public static UniTask<string> OnValueChangedAsync(this TMP_InputField inputField)
        {
            return new AsyncUnityEventHandler<string>(inputField.onValueChanged, inputField.GetCancellationTokenOnDestroy(), true).OnInvokeAsync();
        }

        /// <summary>等待下一次 onValueChanged 触发，可自定义取消令牌。</summary>
        public static UniTask<string> OnValueChangedAsync(this TMP_InputField inputField, CancellationToken cancellationToken)
        {
            return new AsyncUnityEventHandler<string>(inputField.onValueChanged, cancellationToken, true).OnInvokeAsync();
        }

        /// <summary>将 onValueChanged 作为异步可枚举流（自动销毁取消）。</summary>
        public static IUniTaskAsyncEnumerable<string> OnValueChangedAsAsyncEnumerable(this TMP_InputField inputField)
        {
            return new UnityEventHandlerAsyncEnumerable<string>(inputField.onValueChanged, inputField.GetCancellationTokenOnDestroy());
        }

        /// <summary>将 onValueChanged 作为异步可枚举流，可自定义取消令牌。</summary>
        public static IUniTaskAsyncEnumerable<string> OnValueChangedAsAsyncEnumerable(this TMP_InputField inputField, CancellationToken cancellationToken)
        {
            return new UnityEventHandlerAsyncEnumerable<string>(inputField.onValueChanged, cancellationToken);
        }

        /// <summary>获取 onEndEdit 的异步事件处理器，销毁时自动取消。</summary>
        public static IAsyncEndEditEventHandler<string> GetAsyncEndEditEventHandler(this TMP_InputField inputField)
        {
            return new AsyncUnityEventHandler<string>(inputField.onEndEdit, inputField.GetCancellationTokenOnDestroy(), false);
        }

        /// <summary>获取 onEndEdit 的异步事件处理器，可自定义取消令牌。</summary>
        public static IAsyncEndEditEventHandler<string> GetAsyncEndEditEventHandler(this TMP_InputField inputField, CancellationToken cancellationToken)
        {
            return new AsyncUnityEventHandler<string>(inputField.onEndEdit, cancellationToken, false);
        }

        /// <summary>等待下一次 onEndEdit 触发，销毁时自动取消。</summary>
        public static UniTask<string> OnEndEditAsync(this TMP_InputField inputField)
        {
            return new AsyncUnityEventHandler<string>(inputField.onEndEdit, inputField.GetCancellationTokenOnDestroy(), true).OnInvokeAsync();
        }

        /// <summary>等待下一次 onEndEdit 触发，可自定义取消令牌。</summary>
        public static UniTask<string> OnEndEditAsync(this TMP_InputField inputField, CancellationToken cancellationToken)
        {
            return new AsyncUnityEventHandler<string>(inputField.onEndEdit, cancellationToken, true).OnInvokeAsync();
        }

        /// <summary>将 onEndEdit 作为异步可枚举流（自动销毁取消）。</summary>
        public static IUniTaskAsyncEnumerable<string> OnEndEditAsAsyncEnumerable(this TMP_InputField inputField)
        {
            return new UnityEventHandlerAsyncEnumerable<string>(inputField.onEndEdit, inputField.GetCancellationTokenOnDestroy());
        }

        /// <summary>将 onEndEdit 作为异步可枚举流，可自定义取消令牌。</summary>
        public static IUniTaskAsyncEnumerable<string> OnEndEditAsAsyncEnumerable(this TMP_InputField inputField, CancellationToken cancellationToken)
        {
            return new UnityEventHandlerAsyncEnumerable<string>(inputField.onEndEdit, cancellationToken);
        }

        /// <summary>获取 onEndTextSelection 的异步事件处理器（text,start,end 元组），销毁时自动取消。</summary>
        public static IAsyncEndTextSelectionEventHandler<(string, int, int)> GetAsyncEndTextSelectionEventHandler(this TMP_InputField inputField)
        {
            return new AsyncUnityEventHandler<(string, int, int)>(new TextSelectionEventConverter(inputField.onEndTextSelection), inputField.GetCancellationTokenOnDestroy(), false);
        }

        /// <summary>获取 onEndTextSelection 的异步事件处理器，可自定义取消令牌。</summary>
        public static IAsyncEndTextSelectionEventHandler<(string, int, int)> GetAsyncEndTextSelectionEventHandler(this TMP_InputField inputField, CancellationToken cancellationToken)
        {
            return new AsyncUnityEventHandler<(string, int, int)>(new TextSelectionEventConverter(inputField.onEndTextSelection), cancellationToken, false);
        }

        /// <summary>等待下一次 onEndTextSelection 触发（text,start,end 元组），销毁时自动取消。</summary>
        public static UniTask<(string, int, int)> OnEndTextSelectionAsync(this TMP_InputField inputField)
        {
            return new AsyncUnityEventHandler<(string, int, int)>(new TextSelectionEventConverter(inputField.onEndTextSelection), inputField.GetCancellationTokenOnDestroy(), true).OnInvokeAsync();
        }

        /// <summary>等待下一次 onEndTextSelection 触发，可自定义取消令牌。</summary>
        public static UniTask<(string, int, int)> OnEndTextSelectionAsync(this TMP_InputField inputField, CancellationToken cancellationToken)
        {
            return new AsyncUnityEventHandler<(string, int, int)>(new TextSelectionEventConverter(inputField.onEndTextSelection), cancellationToken, true).OnInvokeAsync();
        }

        /// <summary>将 onEndTextSelection 作为异步可枚举流（自动销毁取消）。</summary>
        public static IUniTaskAsyncEnumerable<(string, int, int)> OnEndTextSelectionAsAsyncEnumerable(this TMP_InputField inputField)
        {
            return new UnityEventHandlerAsyncEnumerable<(string, int, int)>(new TextSelectionEventConverter(inputField.onEndTextSelection), inputField.GetCancellationTokenOnDestroy());
        }

        /// <summary>将 onEndTextSelection 作为异步可枚举流，可自定义取消令牌。</summary>
        public static IUniTaskAsyncEnumerable<(string, int, int)> OnEndTextSelectionAsAsyncEnumerable(this TMP_InputField inputField, CancellationToken cancellationToken)
        {
            return new UnityEventHandlerAsyncEnumerable<(string, int, int)>(new TextSelectionEventConverter(inputField.onEndTextSelection), cancellationToken);
        }

        /// <summary>获取 onTextSelection 的异步事件处理器（text,start,end 元组），销毁时自动取消。</summary>
        public static IAsyncTextSelectionEventHandler<(string, int, int)> GetAsyncTextSelectionEventHandler(this TMP_InputField inputField)
        {
            return new AsyncUnityEventHandler<(string, int, int)>(new TextSelectionEventConverter(inputField.onTextSelection), inputField.GetCancellationTokenOnDestroy(), false);
        }

        /// <summary>获取 onTextSelection 的异步事件处理器，可自定义取消令牌。</summary>
        public static IAsyncTextSelectionEventHandler<(string, int, int)> GetAsyncTextSelectionEventHandler(this TMP_InputField inputField, CancellationToken cancellationToken)
        {
            return new AsyncUnityEventHandler<(string, int, int)>(new TextSelectionEventConverter(inputField.onTextSelection), cancellationToken, false);
        }

        /// <summary>等待下一次 onTextSelection 触发，销毁时自动取消。</summary>
        public static UniTask<(string, int, int)> OnTextSelectionAsync(this TMP_InputField inputField)
        {
            return new AsyncUnityEventHandler<(string, int, int)>(new TextSelectionEventConverter(inputField.onTextSelection), inputField.GetCancellationTokenOnDestroy(), true).OnInvokeAsync();
        }

        /// <summary>等待下一次 onTextSelection 触发，可自定义取消令牌。</summary>
        public static UniTask<(string, int, int)> OnTextSelectionAsync(this TMP_InputField inputField, CancellationToken cancellationToken)
        {
            return new AsyncUnityEventHandler<(string, int, int)>(new TextSelectionEventConverter(inputField.onTextSelection), cancellationToken, true).OnInvokeAsync();
        }

        /// <summary>将 onTextSelection 作为异步可枚举流（自动销毁取消）。</summary>
        public static IUniTaskAsyncEnumerable<(string, int, int)> OnTextSelectionAsAsyncEnumerable(this TMP_InputField inputField)
        {
            return new UnityEventHandlerAsyncEnumerable<(string, int, int)>(new TextSelectionEventConverter(inputField.onTextSelection), inputField.GetCancellationTokenOnDestroy());
        }

        /// <summary>将 onTextSelection 作为异步可枚举流，可自定义取消令牌。</summary>
        public static IUniTaskAsyncEnumerable<(string, int, int)> OnTextSelectionAsAsyncEnumerable(this TMP_InputField inputField, CancellationToken cancellationToken)
        {
            return new UnityEventHandlerAsyncEnumerable<(string, int, int)>(new TextSelectionEventConverter(inputField.onTextSelection), cancellationToken);
        }

        /// <summary>获取 onDeselect 的异步事件处理器，销毁时自动取消。</summary>
        public static IAsyncDeselectEventHandler<string> GetAsyncDeselectEventHandler(this TMP_InputField inputField)
        {
            return new AsyncUnityEventHandler<string>(inputField.onDeselect, inputField.GetCancellationTokenOnDestroy(), false);
        }

        /// <summary>获取 onDeselect 的异步事件处理器，可自定义取消令牌。</summary>
        public static IAsyncDeselectEventHandler<string> GetAsyncDeselectEventHandler(this TMP_InputField inputField, CancellationToken cancellationToken)
        {
            return new AsyncUnityEventHandler<string>(inputField.onDeselect, cancellationToken, false);
        }

        /// <summary>等待下一次 onDeselect 触发，销毁时自动取消。</summary>
        public static UniTask<string> OnDeselectAsync(this TMP_InputField inputField)
        {
            return new AsyncUnityEventHandler<string>(inputField.onDeselect, inputField.GetCancellationTokenOnDestroy(), true).OnInvokeAsync();
        }

        /// <summary>等待下一次 onDeselect 触发，可自定义取消令牌。</summary>
        public static UniTask<string> OnDeselectAsync(this TMP_InputField inputField, CancellationToken cancellationToken)
        {
            return new AsyncUnityEventHandler<string>(inputField.onDeselect, cancellationToken, true).OnInvokeAsync();
        }

        /// <summary>将 onDeselect 作为异步可枚举流（自动销毁取消）。</summary>
        public static IUniTaskAsyncEnumerable<string> OnDeselectAsAsyncEnumerable(this TMP_InputField inputField)
        {
            return new UnityEventHandlerAsyncEnumerable<string>(inputField.onDeselect, inputField.GetCancellationTokenOnDestroy());
        }

        /// <summary>将 onDeselect 作为异步可枚举流，可自定义取消令牌。</summary>
        public static IUniTaskAsyncEnumerable<string> OnDeselectAsAsyncEnumerable(this TMP_InputField inputField, CancellationToken cancellationToken)
        {
            return new UnityEventHandlerAsyncEnumerable<string>(inputField.onDeselect, cancellationToken);
        }

        /// <summary>获取 onSelect 的异步事件处理器，销毁时自动取消。</summary>
        public static IAsyncSelectEventHandler<string> GetAsyncSelectEventHandler(this TMP_InputField inputField)
        {
            return new AsyncUnityEventHandler<string>(inputField.onSelect, inputField.GetCancellationTokenOnDestroy(), false);
        }

        /// <summary>获取 onSelect 的异步事件处理器，可自定义取消令牌。</summary>
        public static IAsyncSelectEventHandler<string> GetAsyncSelectEventHandler(this TMP_InputField inputField, CancellationToken cancellationToken)
        {
            return new AsyncUnityEventHandler<string>(inputField.onSelect, cancellationToken, false);
        }

        /// <summary>等待下一次 onSelect 触发，销毁时自动取消。</summary>
        public static UniTask<string> OnSelectAsync(this TMP_InputField inputField)
        {
            return new AsyncUnityEventHandler<string>(inputField.onSelect, inputField.GetCancellationTokenOnDestroy(), true).OnInvokeAsync();
        }

        /// <summary>等待下一次 onSelect 触发，可自定义取消令牌。</summary>
        public static UniTask<string> OnSelectAsync(this TMP_InputField inputField, CancellationToken cancellationToken)
        {
            return new AsyncUnityEventHandler<string>(inputField.onSelect, cancellationToken, true).OnInvokeAsync();
        }

        /// <summary>将 onSelect 作为异步可枚举流（自动销毁取消）。</summary>
        public static IUniTaskAsyncEnumerable<string> OnSelectAsAsyncEnumerable(this TMP_InputField inputField)
        {
            return new UnityEventHandlerAsyncEnumerable<string>(inputField.onSelect, inputField.GetCancellationTokenOnDestroy());
        }

        /// <summary>将 onSelect 作为异步可枚举流，可自定义取消令牌。</summary>
        public static IUniTaskAsyncEnumerable<string> OnSelectAsAsyncEnumerable(this TMP_InputField inputField, CancellationToken cancellationToken)
        {
            return new UnityEventHandlerAsyncEnumerable<string>(inputField.onSelect, cancellationToken);
        }

        /// <summary>获取 onSubmit 的异步事件处理器，销毁时自动取消。</summary>
        public static IAsyncSubmitEventHandler<string> GetAsyncSubmitEventHandler(this TMP_InputField inputField)
        {
            return new AsyncUnityEventHandler<string>(inputField.onSubmit, inputField.GetCancellationTokenOnDestroy(), false);
        }

        /// <summary>获取 onSubmit 的异步事件处理器，可自定义取消令牌。</summary>
        public static IAsyncSubmitEventHandler<string> GetAsyncSubmitEventHandler(this TMP_InputField inputField, CancellationToken cancellationToken)
        {
            return new AsyncUnityEventHandler<string>(inputField.onSubmit, cancellationToken, false);
        }

        /// <summary>等待下一次 onSubmit 触发，销毁时自动取消。</summary>
        public static UniTask<string> OnSubmitAsync(this TMP_InputField inputField)
        {
            return new AsyncUnityEventHandler<string>(inputField.onSubmit, inputField.GetCancellationTokenOnDestroy(), true).OnInvokeAsync();
        }

        /// <summary>等待下一次 onSubmit 触发，可自定义取消令牌。</summary>
        public static UniTask<string> OnSubmitAsync(this TMP_InputField inputField, CancellationToken cancellationToken)
        {
            return new AsyncUnityEventHandler<string>(inputField.onSubmit, cancellationToken, true).OnInvokeAsync();
        }

        /// <summary>将 onSubmit 作为异步可枚举流（自动销毁取消）。</summary>
        public static IUniTaskAsyncEnumerable<string> OnSubmitAsAsyncEnumerable(this TMP_InputField inputField)
        {
            return new UnityEventHandlerAsyncEnumerable<string>(inputField.onSubmit, inputField.GetCancellationTokenOnDestroy());
        }

        /// <summary>将 onSubmit 作为异步可枚举流，可自定义取消令牌。</summary>
        public static IUniTaskAsyncEnumerable<string> OnSubmitAsAsyncEnumerable(this TMP_InputField inputField, CancellationToken cancellationToken)
        {
            return new UnityEventHandlerAsyncEnumerable<string>(inputField.onSubmit, cancellationToken);
        }

    }
}

#endif