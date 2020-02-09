using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DBInline.Classes;
using DBInline.Interfaces;

namespace DBInline
{

    // ReSharper disable once UnusedType.Global
    /// <summary>
    /// Various Methods to Create DbCommands and Transaction for Databases.
    /// </summary>
    public static partial class Extensions
    {
        #region "cmd"

        /// <summary>
        /// Converts ValueTuple array of SimpleParameter to parameter array.
        /// </summary>
        /// <param name="this"></param>
        /// <param name="database"></param>
        /// <returns></returns>
        public static IEnumerable<IDbDataParameter> ToParameterArray(this IEnumerable<SimpleParameter> @this,
            Database database)
        {
            return @this.Select(x => x.ToDbParameter(database)).ToArray();
        }

        /// <summary>
        /// Executes SqlCommand.
        /// </summary>
        /// <param name="command">CommandText.</param>
        public static void CmdRun(string command)
        {
            Transaction(transaction => transaction.CmdRun(command,null));
        }

        /// <summary>
        /// Executes SqlCommand.
        /// </summary>
        /// <param name="body">Use the command.</param>
        public static T QueryRun<T>(Func<IQuery<T>, T> body)
        {
            return QueryRun("",  body);
        }

        /// <summary>
        /// Executes SqlCommand.
        /// </summary>
        /// <param name="command">CommandText.</param>
        /// <param name="parameters">ParameterCollection.</param>
        public static void CmdRun(string command, IEnumerable<IDbDataParameter> parameters)
        {
             Transaction(transaction => transaction.CmdRun(command,parameters));
        }

        /// <summary>
        /// Executes SqlCommand on desired Database context.
        /// </summary>
        /// <param name="this">Database context.</param>
        /// <param name="command">CommandText.</param>
        /// <param name="parameters">ParameterCollection.</param>
        // ReSharper disable once UnusedMethodReturnValue.Global
        // ReSharper disable once MemberCanBePrivate.Global
        public static T CmdRun<T>(this T @this, string command, IEnumerable<IDbDataParameter> parameters)
            where T : IConnectionSource
        {
            return @this.Transaction(tran =>
            {
                using var cmd = new Command<T>(command, tran);
                cmd.AddParameters(parameters ?? new IDbDataParameter[] { });
                cmd.ExecuteNonQuery();
                return @this;
            });
        }
        
        // ReSharper disable once MemberCanBePrivate.Global
        /// <summary>
        /// Executes SqlCommand.
        /// </summary>
        /// <param name="body">Customize and execute Command.</param>
        /// <param name="database">Desired Database</param>
        /// <param name="command">CommandText.</param>
        public static void CmdRun(string database,string command,Action<IQueryBuilder> body)
        {
           ContextController.ContextByName[database].Transaction(tran =>
            {
                using var cmd = new Command<int>(command, tran);
                body(cmd);
                cmd.ExecuteNonQuery();
            });
        }

        #endregion

        #region "Query<T>"

        // ReSharper disable once MemberCanBePrivate.Global  
        // ReSharper disable once MemberCanBePrivate.Global  
        /// <summary>
        /// Executes SqlCommand and returns Result.
        /// </summary>
        /// <param name="body">Use to Customize and to execute Command.</param>
        /// <param name="command">CommandText.</param>
        /// <typeparam name="T">Generic Type T</typeparam>
        /// <returns>Database Query Result of Type T.</returns>
        public static T QueryRun<T>(string command, Func<IQuery<T>, T> body)
        {
            return Transaction(tran =>
            {
                using var cmd = new Command<T>(command, tran);
                var res = body(cmd);
                return res;
            });
        }
        // ReSharper disable once MemberCanBePrivate.Global  
        // ReSharper disable once MemberCanBePrivate.Global  
        /// <summary>
        /// Executes SqlCommand and returns Result.
        /// </summary>
        /// <param name="body">Use to Customize and to execute Command.</param>
        /// <param name="command">CommandText.</param>
        /// <param name="database">Desired database.</param>
        /// <typeparam name="T">Generic Type T</typeparam>
        /// <returns>Database Query Result of Type T.</returns>
        public static T QueryRun<T>(string database,string command ,  Func<IQuery<T>, T> body)
        {
            return ContextController.ContextByName[database].Transaction(tran =>
            {
                using var cmd = new Command<T>(command, tran);
                var res = body(cmd);
                return res;
            });
        }



        // ReSharper disable once MemberCanBePrivate.Global  
        // ReSharper disable once MemberCanBePrivate.Global  
        /// <summary>
        /// Executes SqlCommand and returns Result.
        /// </summary>
        /// <param name="body">Use to Customize and to execute Command.</param>
        /// <param name="command">CommandText.</param>
        /// <param name="token">Cancellation token.</param>
        /// <typeparam name="T">Generic Type T</typeparam>
        /// <returns>Database Query Result of Type T.</returns>
        public static Task<T> QueryAsync<T>(string command, Func<IQuery<T>, T> body, CancellationToken token)
        {
            return Task.Run(() => QueryRun(null,command ,body), token);
        }

        // ReSharper disable once MemberCanBePrivate.Global  
        // ReSharper disable once MemberCanBePrivate.Global  
        /// <summary>
        /// Executes SqlCommand and returns Result.
        /// </summary>
        /// <param name="body">Use to Customize and to execute Command.</param>
        /// <param name="this">Desired database context.</param>
        /// <typeparam name="T">Generic Type T.</typeparam>
        /// <returns>Database Query Result of Type T.</returns>
        public static async Task<T> QueryAsync<T>(this IConnectionSource @this, Func<IQuery<T>, T> body)
        {
            return await @this.QueryAsync("", body, CancellationToken.None).ConfigureAwait(false);
        }
        

        // ReSharper disable once MemberCanBePrivate.Global  
        // ReSharper disable once MemberCanBePrivate.Global  
        /// <summary>
        /// Executes SqlCommand and returns Result.
        /// </summary>
        /// <param name="command">Command Text</param>
        /// <param name="body">Use to Customize and to execute Command.</param>
        /// <param name="this">Desired database context.</param>
        /// <typeparam name="T">Generic Type T.</typeparam>
        /// <returns>Database Query Result of Type T.</returns>
        public static async Task<T> QueryAsync<T>(this IConnectionSource @this, string command, Func<IQuery<T>, T> body)
        {
            return await @this.QueryAsync(command, body, CancellationToken.None).ConfigureAwait(false);
        }
        
        // ReSharper disable once MemberCanBePrivate.Global  
        // ReSharper disable once MemberCanBePrivate.Global  
        /// <summary>
        /// Executes SqlCommand and returns Result.
        /// </summary>
        /// <param name="body">Use to Customize and to execute Command.</param>
        /// <param name="token">Cancellation token.</param>
        /// <param name="this">Desired database context.</param>
        /// <typeparam name="T">Generic Type T</typeparam>
        /// <returns>Database Query Result of Type T.</returns>
        public static async Task<T> QueryAsync<T>(this IConnectionSource @this, Func<IQuery<T>, T> body,
            CancellationToken token)
        {
            return await @this.QueryAsync("", body, token).ConfigureAwait(false);
        }

        // ReSharper disable once MemberCanBePrivate.Global  
        // ReSharper disable once MemberCanBePrivate.Global  
        /// <summary>
        /// Executes SqlCommand and returns Result.
        /// </summary>
        /// <param name="body">Use to Customize and to execute Command.</param>
        /// <param name="command">CommandText.</param>
        /// <param name="token">Cancellation token.</param>
        /// <param name="this">Desired database context.</param>
        /// <typeparam name="T">Generic Type T</typeparam>
        /// <returns>Database Query Result of Type T.</returns>
        public static Task<T> QueryAsync<T>(this IConnectionSource @this, string command, Func<IQuery<T>, T> body,
            CancellationToken token)
        {
            return @this.TransactionAsync(tran =>
            {

                using var cmd = new Command<T>(command, tran);
                var res = body(cmd);
                return res;
            }, token);
        }
        
        // ReSharper disable once MemberCanBePrivate.Global
        /// <summary>
        /// Executes SqlCommand and returns Result.
        /// </summary>
        /// <param name="command">Command text.</param>
        /// <returns>Database Query Result of Type T.</returns>
        public static string Query(string command = "")
        {
            return Transaction(null,tran =>
            {
                using var cmd = new Command<string>(command, tran);
                return cmd.Table().ToJson();
            });
        }

        // ReSharper disable once MemberCanBePrivate.Global
        /// <summary>
        /// Executes SqlCommand and returns Result.
        /// </summary>
        /// <param name="this">Desired IConnectionSource.</param>
        /// <param name="database">Name of the database.</param>
        /// <typeparam name="T">Generic Type T.</typeparam>
        /// <returns>Database Query Result of Type T.</returns>
        public static IQuery<T> Query<T>(this IConnectionSource @this,string database)
        {
            return @this.Transaction(database).Query<T>();
        }

        // ReSharper disable once MemberCanBePrivate.Global
        /// <summary>
        /// Executes SqlCommand and returns Result.
        /// </summary>
        /// <param name="this">Desired IConnectionSource.</param>
        /// <param name="database">Name of the database.</param>
        /// <returns>Database Query Result of Type T.</returns>
        public static IQuery Query(this IConnectionSource @this, string database)
        {
            return @this.Transaction(database).Query();
        }

        // ReSharper disable once MemberCanBePrivate.Global
        /// <summary>
        /// Executes SqlCommand and returns Result.
        /// </summary>
        /// <param name="this">Desired IConnectionSource.</param>
        /// <typeparam name="T">Generic Type T.</typeparam>
        /// <returns>Database Query Result of Type T.</returns>
        public static IQuery<T> Query<T>(this IConnectionSource @this)
        {
            return @this.Transaction(tran => new Command<T>("",tran));
        }
        // ReSharper disable once MemberCanBePrivate.Global
        /// <summary>
        /// Executes SqlCommand and returns Result.
        /// </summary>
        /// <param name="this">Desired IConnectionSource.</param>
        /// <returns>Database Query Result of Type T.</returns>
        public static IQuery Query(this IConnectionSource @this)
        {
            return @this.Transaction(tran => new Command("", tran));
        }



        // ReSharper disable once MemberCanBePrivate.Global
        /// <summary>
        /// Executes SqlCommand and returns Result.
        /// </summary>
        /// <param name="body">Use to Customize and to execute Command.</param>
        /// <typeparam name="T">Generic Type T.</typeparam>
        /// <returns>Database Query Result of Type T.</returns>
        public static Task<T> QueryAsync<T>(Func<IQuery<T>, T> body)
        {
            return QueryAsync("", body);
        }

        // ReSharper disable once MemberCanBePrivate.Global

        // ReSharper disable once MemberCanBePrivate.Global
        /// <summary>
        /// Executes SqlCommand and returns Result.
        /// </summary>
        /// <param name="body">Use to Customize and to execute Command.</param>
        /// <param name="command">CommandText.</param>
        /// <typeparam name="T">Generic Type T.</typeparam>
        /// <returns>Database Query Result of Type T.</returns>
        public static Task<T> QueryAsync<T>(string command, Func<IQuery<T>, T> body)
        {
            return TransactionAsync(tran =>
            {
                using var cmd = new Command<T>(command, tran);
                var res = body(cmd);
                return res;
            });
        }

        #endregion

        #region "CmdScalar"

        /// <summary>
        /// Executes Command and returns first result of the first row.
        /// </summary>
        /// <param name="command">CommandText.</param>
        /// <typeparam name="T">Generic Type T.</typeparam>
        /// <returns>Database Query Result of Type T.</returns>
        public static T CmdScalar<T>(string command)
        {
            return QueryRun<T>(command, cmd => cmd.Scalar());
        }

        
        /// <summary>
        /// Executes Command and returns first result of the first row.
        /// </summary>
        /// <param name="command">CommandText.</param>
        /// <param name="parameterBuilder">Add parameters.</param>
        /// <typeparam name="T">Generic Type T.</typeparam>
        /// <returns>Database Query Result of Type T.</returns>
        public static T CmdScalar<T>(string command, Action<IQueryBuilder> parameterBuilder)
        {
            return QueryRun<T>(command, cmd =>
            {
                parameterBuilder(cmd.Builder);
                return cmd.Scalar();
            });
        }

        /// <summary>
        /// Executes Command and returns first result of the first row.
        /// </summary>
        /// <param name="command">CommandText.</param>
        /// <param name="parameters">ParameterCollection.</param>
        /// <typeparam name="T">Generic Type T.</typeparam>
        /// <returns>Database Query Result of Type T.</returns>
        // ReSharper disable once UnusedParameter.Global
        public static T CmdScalar<T>(string command, IEnumerable<IDbDataParameter> parameters)
        {
            return QueryRun<T>(command, cmd => cmd.Scalar());
        }
        
        #endregion
    }
}

