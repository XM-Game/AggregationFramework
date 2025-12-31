using Cysharp.Threading.Tasks.Internal;
using System.Threading;

namespace Cysharp.Threading.Tasks.Linq
{
    public static partial class UniTaskAsyncEnumerable
    {
        /// <summary>
        /// 返回一个 UniTaskAsyncEnumerable
        /// </summary>
        /// <typeparam name="TValue">值类型</typeparam>
        /// <param name="value">值</param>
        /// <returns>UniTaskAsyncEnumerable</returns>
        public static IUniTaskAsyncEnumerable<TValue> Return<TValue>(TValue value)
        {
            return new Return<TValue>(value);
        }
    }

    /// <summary>
    /// Return 类
    /// </summary>
    internal class Return<TValue> : IUniTaskAsyncEnumerable<TValue>
    {
        readonly TValue value;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="value">值</param>
        public Return(TValue value)
        {
            this.value = value;
        }
        /// <summary>
        /// 获取异步枚举器
        /// </summary>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>异步枚举器</returns>
        public IUniTaskAsyncEnumerator<TValue> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            return new _Return(value, cancellationToken);
        }

        /// <summary>
        /// _Return 类
        /// </summary>
        class _Return : IUniTaskAsyncEnumerator<TValue>
        {
            readonly TValue value;
            CancellationToken cancellationToken;

            bool called;

            /// <summary>
            /// 构造函数
            /// </summary>
            /// <param name="value">值</param>
            /// <param name="cancellationToken">取消令牌</param>
            public _Return(TValue value, CancellationToken cancellationToken)
            {
                this.value = value;
                this.cancellationToken = cancellationToken;
                this.called = false;
            }

            /// <summary>
            /// 当前值
            /// </summary>
            public TValue Current => value;

            /// <summary>
            /// 移动到下一个值
            /// </summary>
            /// <returns>是否成功</returns>
            public UniTask<bool> MoveNextAsync()
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (!called)
                {
                    called = true;
                    return CompletedTasks.True;
                }

                return CompletedTasks.False;
            }

            /// <summary>
            /// 释放异步枚举器
            /// </summary>
            /// <returns>UniTask</returns>
            public UniTask DisposeAsync()
            {
                return default;
            }
        }
    }
}