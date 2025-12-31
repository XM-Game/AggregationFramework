using Cysharp.Threading.Tasks.Internal;
using System.Collections.Generic;
using System.Threading;

namespace Cysharp.Threading.Tasks.Linq
{   
    /// <summary>
    /// UniTaskAsyncEnumerable 类
    /// </summary>
    public static partial class UniTaskAsyncEnumerable
    {
        /// <summary>
        /// 将 UniTaskAsyncEnumerable 转换为 List
        /// </summary>
        /// <typeparam name="TSource">源类型</typeparam>
        /// <param name="source">源</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>List</returns>
        public static UniTask<List<TSource>> ToListAsync<TSource>(this IUniTaskAsyncEnumerable<TSource> source, CancellationToken cancellationToken = default)
        {
            Error.ThrowArgumentNullException(source, nameof(source));

            return Cysharp.Threading.Tasks.Linq.ToList.ToListAsync(source, cancellationToken);
        }
    }
    /// <summary>
    /// ToList 类
    /// </summary>
    internal static class ToList
    {
        internal static async UniTask<List<TSource>> ToListAsync<TSource>(IUniTaskAsyncEnumerable<TSource> source, CancellationToken cancellationToken)
        {
            var list = new List<TSource>();

            var e = source.GetAsyncEnumerator(cancellationToken);
            try
            {
                while (await e.MoveNextAsync())
                {
                    list.Add(e.Current);
                }
            }
            finally
            {
                if (e != null)
                {
                    await e.DisposeAsync();
                }
            }

            return list;
        }
    }
}