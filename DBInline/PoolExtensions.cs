﻿using System;
using System.Threading;
using System.Threading.Tasks;
using DBInline.Classes;
using DBInline.Interfaces;

namespace DBInline
{
    public static partial class Extensions
    {
        /// <summary>
        /// Groups Transactions/Connections./Connections.
        /// </summary>
        /// <param name="this">Connection Source</param>
        /// <param name="body">Execution body.</param>
        public static void Pool(this IConnectionSource @this, Action<IPool> body)
        {
            @this.Pool(p =>
            {
                body(p);
                return 0;
            });
        }

        /// <summary>
        /// Groups Transactions/Connections./Connections.
        /// </summary>
        public static IPool Pool()
        {
            return new Pool();
        }

        /// <summary>
        /// Groups Transactions/Connections./Connections.
        /// </summary>
        /// <param name="body">Execution body."></param>
        public static void Pool(Action<IPool> body)
        {
            ContextController.DefaultContext.Pool(p =>
            {
                body(p);
                return 0;
            });
        }

        /// <summary>
        /// Groups Transactions/Connections./Connections.
        /// </summary>
        /// <param name="this">Connection Source</param>
        /// <param name="body">Execution body.</param>
        public static T Pool<T>(this IConnectionSource @this, Func<IPool, T> body)
        {
            using var pool = new Pool(@this);
            var res = body(pool);
            return res;
        }

        /// <summary>
        /// Groups Transactions/Connections.
        /// </summary>
        /// <param name="body">Execution body."></param>
        public static T Pool<T>(Func<IPool, T> body)
        {
            return Pool(null, body);
        }

        /// <summary>
        /// Groups Transactions/Connections.
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
                var res = body(pool);
                pool.Commit();
                return res;
            }, token);
        }

        /// <summary>
        /// Groups Transactions/Connections.
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
                var res = await body(pool);
                pool.Commit();
                return res;
            }, token);
        }

        /// <summary>
        /// Groups Transactions/Connections.
        /// </summary>
        /// <param name="body">Execution body."></param>
        public static Task<T> PoolAsync<T>(Func<IPool, T> body)
        {
            return PoolAsync(null, body);
        }

        /// <summary>
        /// Groups Transactions/Connections.
        /// </summary>
        /// <param name="body">Execution body."></param>
        public static Task<T> PoolAsync<T>(Func<IPool, Task<T>> body)
        {
            return PoolAsync(null, body);
        }

        /// <summary>
        /// Groups Transactions/Connections.
        /// </summary>
        /// <param name="body">Execution body."></param>
        public static async Task PoolAsync(Action<IPool> body)
        {
            await PoolAsync(null, p =>
            {
                body(p);
                return 0;
            }).ConfigureAwait(false);
        }
    }
}
