using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using DBInline.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.Data.Sqlite;
using MySql.Data.MySqlClient;
using Npgsql;

namespace DBInline.Classes
{
    internal class Command<T> : Command, IQuery<T> , IClauseBuilder<Command<T>>
    {
        public Command(string commandText, Transaction transaction) : base(commandText, transaction)
        {
        }
        public new IQuery<T> Set(string text)
        {
            ClauseBuilder.CommandText = text;
            return this;
        }
        public T Scalar()
        {
            ClauseBuilder.BuildClauses(this);
            var res = ExecuteScalar();
            if (res == DBNull.Value)
            {
                return default;
            }
            return (T)res;
        }
        public async Task<T> ScalarAsync()
        { 
            ClauseBuilder.BuildClauses(this);
            var res = await ExecuteScalarAsync(Token).ConfigureAwait(false); 
            if (res == DBNull.Value)
            {
                return default;
            }
            return (T)res;
        }
        public new IEnumerable<TOut> Select<TOut>(Func<IDataReader, TOut> transform)
        {
            ClauseBuilder.BuildClauses(this);
            using var r = ExecuteReader();
            while (r.Read())
            {
               yield return transform(r);
            }
            r.Close();
        }
        public new async IAsyncEnumerable<TOut> SelectAsync<TOut>(Func<IDataReader, TOut> transform)
        {
            ClauseBuilder.BuildClauses(this);
            await using var r = ExecuteReader();
            while (await r.ReadAsync())
            {
                yield return transform(r);
            }
            await r.CloseAsync().ConfigureAwait(false);
        }
        Command<T> ICommandBuilder<Command<T>>.Set(string text)
        {
             Set(text);
             return this;
        }
        IQuery<T> ICommandBuilder<IQuery<T>>.AddRollback(Action action)
        {
             AddRollback(action);
             return this;
        }
        Command<T> ICommandBuilder<Command<T>>.Param(string name, object value)
        {
            Param(name, value);
            return this;
        }
        Command<T> ICommandBuilder<Command<T>>.Param(IDbDataParameter parameter)
        {
            Param(parameter.ParameterName,parameter.Value);
            return this;
        }
        Command<T> ICommandBuilder<Command<T>>.Param(SimpleParameter parameter)
        {
            Param(parameter.Name,parameter.Value);
            return this;
        }
        IQuery<T> ICommandBuilder<IQuery<T>>.Param(IDbDataParameter parameter)
        {
            Param(parameter.ParameterName,parameter.Value);
            return this;
        }
        IQuery<T> ICommandBuilder<IQuery<T>>.Param(SimpleParameter parameter)
        {
            Param(parameter.Name,parameter.Value);
            return this;
        }
        Command<T> ICommandBuilder<Command<T>>.AddParameters(IEnumerable<IDbDataParameter> paramArray)
        {
            AddParameters(paramArray);
            return this;
        }
        Command<T> ICommandBuilder<Command<T>>.Where(string whereString)
        {
            ClauseBuilder.AddWhere(whereString);
            return this;
        }

        Command<T> ICommandBuilder<Command<T>>.Where(string fieldName, object value)
        {
            base.Where(fieldName, value);
            return this;
        }

        IQuery<T> ICommandBuilder<IQuery<T>>.Where(string fieldName, object value)
        {
            base.Where(fieldName, value);
            return this;
        }

        IQuery<T> ICommandBuilder<IQuery<T>>.Where(string whereString)
        {
            ClauseBuilder.AddWhere(whereString);
            return this;
        }
        Command<T> ICommandBuilder<Command<T>>.Order(string orderClause)
        {
            ClauseBuilder.OrderClause = orderClause;
            return this;
        }
        IQuery<T> ICommandBuilder<IQuery<T>>.Order(string orderClause)
        {
            ClauseBuilder.OrderClause = orderClause;
            return this;
        }
        Command<T> ICommandBuilder<Command<T>>.Limit(int limit)
        {
            ClauseBuilder.Limit = limit;
            return this;
        }
        IQuery<T> ICommandBuilder<IQuery<T>>.Limit(int limit)
        {
            ClauseBuilder.Limit = limit;
            return this;
        }
        Command<T> ICommandBuilder<Command<T>>.AddRollback(Action action)
        {
            AddRollback(action);
            return this;
        }
        IQuery<T> ICommandBuilder<IQuery<T>>.Param(string name, object value)
        {
            Param(name, value);
            return this;
        }
        IQuery<T> ICommandBuilder<IQuery<T>>.AddParameters(IEnumerable<IDbDataParameter> paramArray)
        {
            AddParameters(paramArray);
            return this;
        }

        Command<T> IClauseBuilder<Command<T>>.Or(string clause)
        {
            ClauseBuilder.AddOr(clause);
            return this;
        }

        Command<T> IClauseBuilder<Command<T>>.Or(string fieldName, object value)
        {
            var name = GenerateParam(fieldName,value);
            ClauseBuilder.AddOr($"{fieldName}={name}");
            return this;
        }
    }
    public class Command : DbCommand, IQuery, IQueryBuilder, ITokenHolder
    {
        private readonly bool _isolated;
        internal readonly DbCommand DbCommand;
        public new DatabaseConnection Connection { get; }

        // ReSharper disable once MemberCanBePrivate.Global
        public new Transaction Transaction { get; }

        protected readonly ClauseBuilder ClauseBuilder = new ClauseBuilder();
        public Command(string commandText, Transaction transaction, bool isolated = true)
        {
            _isolated = isolated;
            Connection = transaction.Connection;
            DbCommand = transaction.Connection.CreateCommand();
            DbCommand.Transaction = transaction.DbTransaction;
            ClauseBuilder.CommandText = commandText;
            Transaction = transaction;
            Transaction.OnCommandCreated(this);
        }
        
        public override void Cancel()
        {
            DbCommand.Cancel();
        }
        public  T Scalar<T>()
        {
            ClauseBuilder.BuildClauses(this);
            var res = (T) ExecuteScalar();
            return res;
        }

        public async Task<T> ScalarAsync<T>()
        {
            ClauseBuilder.BuildClauses(this);
            var res =(T) await ExecuteScalarAsync(Token);
            return res;
        }

        public IEnumerable<TOut> Select<TOut>(Func<IDataReader, TOut> transform)
        {
            ClauseBuilder.BuildClauses(this);
            using var r = ExecuteReader();
            while (r.Read())
            {
                yield return transform(r);
            }
            r.Close();
        }

        public async IAsyncEnumerable<TOut> SelectAsync<TOut>(Func<IDataReader, TOut> transform)
        {
            ClauseBuilder.BuildClauses(this);
            await using var r = await ExecuteReaderAsync(Token);
            while (r.Read())
            {
                yield return transform(r);
            }
            await r.CloseAsync().ConfigureAwait(false);
        }

        // ReSharper disable once MemberCanBePrivate.Global
        internal Parameter CreateParameter(string name, object value)
        {
            var param = Connection.DbType switch
            {
                Database.Mssql => new Parameter(this, new SqlParameter(name, value)),
                Database.Postgres => new Parameter(this, new NpgsqlParameter(name, value)),
                Database.Mysql => new Parameter(this, new MySqlParameter(name, value)),
                Database.SqlLite => new Parameter(this, new SqliteParameter(name, value)),
                _ => new Parameter(DbCommand.CreateParameter())
            };
            Parameters.Add(param.DbParameter);
            return param;
        }
 
        
        public int Run()
        {
            ClauseBuilder.BuildClauses(this);
            var res = ExecuteNonQuery();
            return res;
        }
        public object RunScalar()
        {
            ClauseBuilder.BuildClauses(this);
            var res = ExecuteScalar();
            return res;
        }
        public override int ExecuteNonQuery()
        {
            ClauseBuilder.BuildClauses(this);
            return DbCommand.ExecuteNonQuery();
        }

        public override object ExecuteScalar()
        {
            ClauseBuilder.BuildClauses(this);
            return DbCommand.ExecuteScalar();
        }

        public override void Prepare()
        {
            DbCommand.Prepare();
        }
        
        public new void Dispose()
        {
            DbCommand.Dispose();
            if(_isolated) Transaction.Dispose();
        }
        
        public override string CommandText
        {
            get => ClauseBuilder.CommandText;
            set => ClauseBuilder.CommandText = value;
        }

        public override int CommandTimeout
        {
            get => DbCommand.CommandTimeout;
            set => DbCommand.CommandTimeout = value;
        }

        public override CommandType CommandType
        {
            get => DbCommand.CommandType;
            set => DbCommand.CommandType = value;
        }

        public override UpdateRowSource UpdatedRowSource
        {
            get => DbCommand.UpdatedRowSource;
            set => DbCommand.UpdatedRowSource = value;
        }

        protected override DbConnection DbConnection { get; set; }
        protected override DbParameterCollection DbParameterCollection => DbCommand.Parameters;
        protected override DbTransaction DbTransaction { get; set; }

        public override bool DesignTimeVisible
        {
            get => DbCommand.DesignTimeVisible;
            set => DbCommand.DesignTimeVisible = value;
        }

        protected override DbParameter CreateDbParameter()
        {
            return DbCommand.CreateParameter();
        }

        protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior)
        {
            ClauseBuilder.BuildClauses(this);
            return DbCommand.ExecuteReader();
        }

        public CancellationToken Token => Transaction.Token;

        public IQueryBuilder Set(string text)
        {
            ClauseBuilder.CommandText = text;
            return this;
        }

        IQuery ICommandBuilder<IQuery>.Set(string text)
        {
             Set(text);
             return this;
        }

        public IQuery AddRollback(Action action)
        {
            Transaction.AddRollback(action);
            return this;
        }

        IQuery ICommandBuilder<IQuery>.Param(string name, object value)
        {
            CreateParameter(name,value);
            return this;
        }

        IQuery ICommandBuilder<IQuery>.AddParameters(IEnumerable<IDbDataParameter> paramArray)
        {
            foreach (var dbDataParameter in paramArray)
            {
                CreateParameter(dbDataParameter.ParameterName,dbDataParameter.Value);
            }
            return this;
        }

        public IQueryBuilder Where(string whereClause)
        {
            ClauseBuilder.AddWhere(whereClause);
            return this;
        }

        private int _generatedParam;

        protected string GenerateParam(string fieldName, object value)
        {
            Interlocked.Increment(ref _generatedParam);
            var name = $"@{fieldName}_{_generatedParam}";
            AddParam((name, value));
            return name;
        }
        

        protected void Where(string fieldName, object value)
        {
            var name =GenerateParam(fieldName,value);
            Where($"{fieldName}={name}");
        }
        
        IQueryBuilder ICommandBuilder<IQueryBuilder>.Where(string fieldName, object value)
        {
            Where(fieldName, value);
            return this;
        }

        IQuery ICommandBuilder<IQuery>.Where(string fieldName, object value)
        {
            Where(fieldName, value);
            return this;
        }

        IQuery ICommandBuilder<IQuery>.Where(string whereString)
        {
            ClauseBuilder.AddWhere(whereString);
            return this;
        }

        public IQueryBuilder Order(string orderClause)
        {
            ClauseBuilder.OrderClause = orderClause;
            return this;
        }

        IQueryBuilder ICommandBuilder<IQueryBuilder>.Limit(int limit)
        {
            ClauseBuilder.Limit = limit;
            return this;
        }

        IQuery ICommandBuilder<IQuery>.Limit(int limit)
        {
            ClauseBuilder.Limit = limit;
            return this;
        }

        IQuery ICommandBuilder<IQuery>.Order(string orderClause)
        {
            ClauseBuilder.OrderClause = orderClause;
            return this;
        }

        IQueryBuilder IClauseBuilder<IQueryBuilder>.Or(string clause)
        {
            ClauseBuilder.AddOr(clause);
            return this;
        }

         IQueryBuilder IClauseBuilder<IQueryBuilder>.Or(string fieldName, object value)
        {
            var name = GenerateParam(fieldName,value);
            ClauseBuilder.AddOr($"{fieldName}={name}");
            return this;
        }

        IQuery ICommandBuilder<IQuery>.Param(IDbDataParameter parameter)
        {
            CreateParameter(parameter.ParameterName,parameter.Value);
            return this;
        }

        IQuery ICommandBuilder<IQuery>.Param(SimpleParameter parameter)
        {
            CreateParameter(parameter.Name,parameter.Value);
            return this;
        }

        IQueryBuilder ICommandBuilder<IQueryBuilder>.Param(string name, object value)
        {
            CreateParameter(name,value);
            return this;
        }

        public IQueryBuilder Param(IDbDataParameter parameter)
        {
            CreateParameter(parameter.ParameterName,parameter.Value);
            return this;
        }

        public IQueryBuilder Param(SimpleParameter parameter)
        {
            CreateParameter(parameter.Name,parameter.Value);
            return this;
        }

        IQueryBuilder ICommandBuilder<IQueryBuilder>.AddParameters(IEnumerable<IDbDataParameter> paramArray)
        {
            AddParameters(paramArray);
            return this;
        }

        IAddRollBack IAddRollBack.AddRollback(Action action)
        {
             AddRollback(action);
             return this;
        }

        IQueryBuilder ICommandBuilder<IQueryBuilder>.AddRollback(Action action)
        {
             AddRollback(action);
             return this;
        }

        public IAddParameter Param(string name, object value)
        {
            CreateParameter(name,value);
            return this;
        }

        public IAddParameter AddParameters(IEnumerable<IDbDataParameter> paramArray)
        {
            foreach (var dbDataParameter in paramArray)
            {
                CreateParameter(dbDataParameter.ParameterName,dbDataParameter.Value);
            }
            return this;
        }

        // ReSharper disable once UnusedMethodReturnValue.Local
        private IDbDataParameter AddParam((string name, object value) valueTuple)
        {
            var (name, value) = valueTuple;
            return CreateParameter(name,value);
        }
        public IQueryBuilder Builder => this;
        IQuery IQuery.Set(string text)
        {
            ClauseBuilder.CommandText = text;
            return this;
        }

        Command IWrapCommand.Command => this;
        

        public Task<int> RunAsync()
        {
            ClauseBuilder.BuildClauses(this);
            return DbCommand.ExecuteNonQueryAsync(Token);
        }
        

        public DataSet DataSet()
        {
            ClauseBuilder.BuildClauses(this);
            var dt = new DataSet();
             using var da= new DataAdapter(this);
            da.Fill(dt);
            return dt;
        }
        public async Task<DataSet> DataSetAsync()
        {
            ClauseBuilder.BuildClauses(this);
            var dt = new DataSet();
            await using var da= new DataAdapter(this);
            await da.FillAsync(dt,Token).ConfigureAwait(false);
            return dt;
        }

        public DbDataReader Reader()
        {
            ClauseBuilder.BuildClauses(this);
            return ExecuteReader();
        }

        public DataTable Table()
        {
            ClauseBuilder.BuildClauses(this);
            var dt = new DataTable();
            using var da= new DataAdapter(this);
            da.Fill(dt);
            return dt;
        }

        public async Task<DataTable> TableAsync()
        {
            ClauseBuilder.BuildClauses(this);
            var dt = new DataTable();
            await using var da= new DataAdapter(this);
            await da.FillAsync(dt,Token).ConfigureAwait(false);
            return dt;
        }
        
        public Task<DbDataReader> ReaderAsync()
        {
            ClauseBuilder.BuildClauses(this);
            return DbCommand.ExecuteReaderAsync(Token);
        }


    }
}