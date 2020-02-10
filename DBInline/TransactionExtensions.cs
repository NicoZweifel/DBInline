using System;
using System.Data;
using System.Reflection;
using System.Threading;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using DBInline.Classes;
using DBInline.Classes.Transactions;
using DBInline.Interfaces;


namespace DBInline
{
    public static partial class Extensions
    {
        // ReSharper disable once UnusedMember.Local
        /// <summary>
        /// Creates/Rollbacks/Disposes Transaction
        /// </summary>
        /// <param name="body">Use Transaction.</param>
        // ReSharper disable once MemberCanBePrivate.Global
        public static void Transaction(Action<Transaction> body)
        {
            Transaction(t =>
            {
                body(t);
                return 0;
            });
        }

        /// <summary>
        /// Creates/Rollbacks/Disposes Transaction
        /// </summary>
        /// <param name="body">Use Transaction.</param>
        /// <typeparam name="T">Generic Type T.</typeparam>
        /// <returns>QueryResult of Type T</returns>
        // ReSharper disable once MemberCanBePrivate.Global
        public static T Transaction<T>(Func<Transaction, T> body)
        {
            return Transaction(null,body);
        }
        /// <summary>
        /// Creates/Rollbacks/Disposes Transaction on desired database context.
        /// </summary>
        /// <param name="body">Use Transaction.</param>
        /// <typeparam name="T">Generic Type T.</typeparam>
        /// <returns>QueryResult of Type T</returns>
        // ReSharper disable once MemberCanBePrivate.Global
        public static Task<T> TransactionAsync<T>( Func<Transaction, T> body)
        {
            return Task.Run(() => Transaction(body));
        }  
        
        /// <summary>
        /// Creates/Rollbacks/Disposes Transaction on desired database context.
        /// </summary>
        /// <param name="body">Use Transaction.</param>
        /// <returns>Awaitable Task</returns>
        // ReSharper disable once MemberCanBePrivate.Global
        public static Task TransactionAsync(Action<Transaction> body)
        {
            return Task.Run(() => Transaction(body));
        }

        /// <summary>
        /// Creates/Rollbacks/Disposes Transaction on desired database context.
        /// </summary>
        /// <param name="this">Database context.</param>
        /// <param name="body">Use Transaction.</param>
        /// <typeparam name="T">Generic Type T.</typeparam>
        /// <returns>QueryResult of Type T</returns>
        // ReSharper disable once MemberCanBePrivate.Global
        public static Task<T> TransactionAsync<T>(this IConnectionSource @this, Func<Transaction, T> body)
        {
            return Task.Run(() => Transaction(@this,body));
        }

        /// <summary>
        /// Creates/Rollbacks/Disposes Transaction on desired database context.
        /// </summary>
        /// <param name="this">Database context.</param>
        /// <param name="body">Use Transaction.</param>
        /// <param name="token">Cancellation Token.</param>
        /// <typeparam name="T">Generic Type T.</typeparam>
        /// <returns>QueryResult of Type T</returns>
        // ReSharper disable once MemberCanBePrivate.Global
        public static Task<T> TransactionAsync<T>(this IConnectionSource @this, Func<Transaction, T> body,CancellationToken token)
        {
            return Task.Run(() => Transaction(@this,body,token),token);
        }

        /// <summary>
        /// Creates/Rollbacks/Disposes Transaction
        /// </summary>
        /// <param name="body">Use Transaction.</param>
        /// <param name="this">Desired connection source.</param>
        /// <typeparam name="T">Generic Type T.</typeparam>
        /// <returns>Query Results of Type T.</returns>
        // ReSharper disable once MemberCanBePrivate.Global
        public static T Transaction<T>(this IConnectionSource @this, Func<Transaction, T> body)
        {
            return @this.Transaction(body, CancellationToken.None);
        }
        
        /// <summary>
        /// Creates/Rollbacks/Disposes Transaction
        /// </summary>
        /// <param name="body">Use Transaction.</param>
        /// <param name="this">Desired connection source.</param>
        // ReSharper disable once MemberCanBePrivate.Global
        public static void Transaction(this IConnectionSource @this, Action<Transaction> body)
        {
             @this.Transaction(body, CancellationToken.None);
        }

        /// <summary>
        /// Creates/Rollbacks/Disposes Transaction
        /// </summary>
        /// <param name="body">Use Transaction.</param>
        /// <param name="this">Desired connection source.</param>
        /// <returns>Awaitable Task</returns>
        public static Task TransactionAsync(this IConnectionSource @this, Action<Transaction> body)
        {
            return Task.Run(()=>  @this.Transaction(body, CancellationToken.None));
        }

        /// <summary>
        /// Creates/Rollbacks/Disposes Transaction
        /// </summary>
        /// <param name="body">Use Transaction.</param>
        /// <param name="this">Desired connection source.</param>
        /// <param name="token">Cancellation Token.</param>
        /// <returns>Awaitable Task</returns>
        public static Task TransactionAsync(this IConnectionSource @this, Action<Transaction> body,
            CancellationToken token)
        {
           return Task.Run(()=> @this.Transaction(body,token),token);
        }

        /// <summary>
        /// Creates/Rollbacks/Disposes Transaction
        /// </summary>
        /// <param name="body">Use Transaction.</param>
        /// <param name="this">Desired connection source.</param>
        /// <param name="token">Cancellation Token.</param>
        private static void Transaction(this IConnectionSource @this, Action<Transaction> body,
            CancellationToken token)
        {
            @this.Transaction(tran =>
            {
                body(tran);
                return 0;
            }, token);
        }

        /// <summary>
        /// Creates/Rollbacks/Disposes Transaction
        /// </summary>
        /// <param name="body">Use Transaction.</param>
        /// <param name="this">Desired connection source.</param>
        /// <param name="token">Cancellation Token.</param>
        /// <typeparam name="T">Generic Type T.</typeparam>
        /// <returns>Query Results of Type T.</returns>
        private static T Transaction<T>( this IConnectionSource @this, Func<Transaction, T> body,CancellationToken token)
        {
            if(body.Method.GetCustomAttribute(typeof(AsyncStateMachineAttribute)) != null) 
                throw new InvalidOperationException("Async Lambdas not supported, please use a Pool.");
            var transaction = @this as ManagedTransaction;
            var ts = @this as IPool;
            var tran = transaction ?? @this?.Transaction() ?? new Transaction(ContextController.DefaultContext) {Token = token};
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            var handleTransaction = transaction == null && ts == null && @this == null;
            var conn = tran.Connection;
            if (conn.State == ConnectionState.Closed) conn.Open();
            tran.OnTransactionCreated(tran);
            if (tran.DbTransaction == null )
            {
                tran.DbTransaction = conn.BeginTransaction();
            }
            try
            {
                var res =  body(tran);
                if (handleTransaction)
                {
                    tran.Commit();
                }
                return res;
            }
            catch (Exception ex)
            {
                if (handleTransaction)
                {
                    tran.Rollback();
                }
                if (ex !is Pool.PoolCanceledException)
                {
                    throw new AggregateException("Failed to run Transaction.", ex);
                }
                else
                {
                    throw;
                }
            }
            finally
            {
                if (handleTransaction)
                {
                    tran.Dispose();
                    conn.Close();
                    conn.Dispose();
                }
            } 
        }
    }
}