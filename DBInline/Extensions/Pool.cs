using System;
using System.Threading;
using System.Threading.Tasks;
using DBInline.Classes;
using DBInline.Interfaces;

namespace DBInline
{
    public static partial class Extensions
    {
        /// <summary>
        /// Groups Transactions.
        /// </summary>
        /// <param name="this">Connection Source</param>
        /// <param name="body">Execution body.</param>
        public static void Pool(this IConnectionSource @this, Action<IPool> body)
        {
            @this.Pool(p=>
            {
                body(p);
                return 0;
            });
        }
        /// <summary>
        /// Groups Transactions.
        /// </summary>
        public static IPool Pool()
        {
            return new Pool();
        }
        /// <summary>
        /// Groups Transactions.
        /// </summary>
        /// <param name="body">Execution body."></param>
        public static void Pool(Action<IPool> body)
        {
             ContextController.DefaultContext.Pool(p=>
             {
                  body(p);
                  return 0;
             });
        }

        /// <summary>
        /// Groups Transactions.
        /// </summary>
        /// <param name="this">Connection Source</param>
        /// <param name="body">Execution body.</param>
        public static T Pool<T>(this IConnectionSource  @this, Func<IPool, T> body)
        {
            using var pool = new Pool(@this);
            try
            {
                var res = body(pool);
                return res;
            }
            catch (Exception ex)
            {
                if (ex.GetType() != typeof(Pool.PoolCanceledException))
                {
                    throw new AggregateException("Failed to run Pool.", ex);
                }
                else
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// Groups Transactions.
        /// </summary>
        /// <param name="body">Execution body."></param>
        public static T Pool<T>(Func<IPool, T> body)
        {
            return Pool(null,body);
        }

        /// <summary>
        /// Groups Transactions.
        /// </summary>
        /// <param name="this">Connection Source</param>
        /// <param name="body">Execution body.</param>
        public static Task<T> PoolAsync<T>(this IConnectionSource @this, Func<IPool, T> body)
        {
            var source = new CancellationTokenSource();
            var token = source.Token;
            return Task.Run(() =>
            {
                using var pool = new Pool(@this)
                {
                    TokenSource = source
                };
                try
                {
                    var res = body(pool);
                    return res;
                }
                catch (Exception ex)
                {
                    if ( ex.GetType() != typeof (Pool.PoolCanceledException))
                    {
                        throw new AggregateException("Failed to run Pool.", ex);
                    }
                    else
                    {
                        throw;
                    }
                }
            }, token);
        }

        /// <summary>
        /// Groups Transactions.
        /// </summary>
        /// <param name="this">Connection Source</param>
        /// <param name="body">Execution body.</param>
        public static Task<T> PoolAsync<T>(this IConnectionSource @this, Func<IPool, Task<T>> body)
        {
            var source = new CancellationTokenSource();
            var token = source.Token;
            return Task.Run(async () =>
            {
                using var pool = new Pool(@this)
                {
                    TokenSource = source
                };
                try
                {
                    var res =await body(pool);
                    return res;
                }
                catch (Exception ex)
                {
                    if ( ex.GetType() != typeof (Pool.PoolCanceledException))
                    {
                        throw new AggregateException("Failed to run Pool.", ex);
                    }
                    else
                    {
                        throw;
                    }
                }
            }, token);
        }
        /// <summary>
        /// Groups Transactions.
        /// </summary>
        /// <param name="body">Execution body."></param>
        public static Task<T> PoolAsync<T>(Func<IPool, T> body)
        {
            return PoolAsync(null, body);
        }
        /// <summary>
        /// Groups Transactions.
        /// </summary>
        /// <param name="body">Execution body."></param>
        public static  Task<T> PoolAsync<T>(Func<IPool, Task<T>> body)
        {
            return PoolAsync(null, body);
        }
    }
}
