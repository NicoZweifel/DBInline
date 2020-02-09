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
    internal class Command<T> : Command, IQueryBuilder<Command<T>,T>
    {
        public Command(string commandText, Transaction transaction) : base(commandText, transaction)
        {
        }
        public new T Scalar()
        {
            var res = ExecuteScalar();
            if (res == DBNull.Value)
            {
                return default;
            }
            return (T)res;
        }
        public new async Task<T> ScalarAsync()
        {
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

        public IQuery<T> Set(string text)
        {
            ClauseBuilder.CommandText = text;
            return this;
        }
        

        IQueryBuilder<Command<T>,T> IQueryBuilder<Command<T>, T>.Where(string fieldName, object value)
        {
            WhereInternal(fieldName, value);
            return this;
        }

        IQueryBuilder<Command<T>,T> IQueryBuilder<Command<T>, T>.Where(string clause)
        {
            ClauseBuilder.AddWhere(clause);
            return this;
        }
        IQuery<T> ICommandCommon<IQuery<T>>.Order(string clause)
        {
            ClauseBuilder.OrderClause = clause;
            return this;
        }
        IQuery<T> ICommandCommon<IQuery<T>>.Limit(int limit)
        {
            ClauseBuilder.Limit = limit;
            return this;
        }

        IQuery<T> ICommandCommon<IQuery<T>>.Rollback(Action action)
        {
             Rollback(action);
             return this;
        }

        IQuery<T> ICommandCommon<IQuery<T>>.Param(string name, object value)
        {
            Param(name, value);
            return this;
        }

        IQuery<T> ICommandCommon<IQuery<T>>.Param(IDbDataParameter parameter)
        {
            Param(parameter.ParameterName,parameter.Value);
            return this;
        }

        IQuery<T> ICommandCommon<IQuery<T>>.Param(SimpleParameter parameter)
        {
            Param(parameter.Name,parameter.Value);
            return this;
        }

        IQuery<T> ICommandCommon<IQuery<T>>.Parameters(IEnumerable<IDbDataParameter> paramArray)
        {
            Parameters(paramArray);
            return this;
        }

        IQueryBuilder<Command<T>, T> IQueryBuilder<Command<T>,T>.Or(string clause)
        {
            ClauseBuilder.AddOr(clause);
            return this;
        }

        IQueryBuilder<Command<T>, T> IQueryBuilder<Command<T>,T>.Or(string fieldName, object value)
        {
            OrInternal(fieldName,value);
            return this;
        }
    }
    public class Command : DbCommand, ITokenHolder,IQueryBuilder<Command>
    {
        private readonly bool _isolated;
        internal readonly DbCommand DbCommand;
        private int _generatedParamId;
        public new DatabaseConnection Connection { get; }

        // ReSharper disable once MemberCanBePrivate.Global
        public new Transaction Transaction { get; }

        internal readonly ClauseBuilder ClauseBuilder = new ClauseBuilder();
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
            var res = (T) ExecuteScalar();
            return res;
        }

        public async Task<T> ScalarAsync<T>()
        {
            var res =(T) await ExecuteScalarAsync(Token);
            return res;
        }

        public object Scalar()
        {
            return ExecuteScalar();
        }

        public async Task<object> ScalarAsync()
        {
            var res =await ExecuteScalarAsync(Token);
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
            base.Parameters.Add(param.DbParameter);
            return param;
        }


        IQuery IQuery.Set(string text)
        {
            ClauseBuilder.CommandText = text;
            return this;
        }

        public int Run()
        {
            var res = ExecuteNonQuery();
            return res;
        }
        public object RunScalar()
        {
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
        
        public override Task<object> ExecuteScalarAsync(CancellationToken cancellationToken)
        {
            ClauseBuilder.BuildClauses(this);
            return base.ExecuteScalarAsync(cancellationToken);
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
        
        public IQuery Rollback(Action action)
        {
            Transaction.Rollback(action);
            return this;
        }
        IQuery ICommandCommon<IQuery>.Param(string name, object value)
        {
            CreateParameter(name,value);
            return this;
        }

        public IQuery Param(IDbDataParameter parameter)
        {
            CreateParameter(parameter.ParameterName,parameter.Value);
            return this;
        }

        public IQuery Param(SimpleParameter parameter)
        {
            CreateParameter(parameter.Name,parameter.Value);
            return this;
        }

        IQuery ICommandCommon<IQuery>.Parameters(IEnumerable<IDbDataParameter> paramArray)
        {
            foreach (var dbDataParameter in paramArray)
            {
                CreateParameter(dbDataParameter.ParameterName,dbDataParameter.Value);
            }
            return this;
        }

        public IQuery Order(string clause)
        {
            ClauseBuilder.AddOr(clause);
            return this;
        }

        public IQuery Limit(int limit)
        {
            ClauseBuilder.Limit = limit;
            return this;
        }
        
        
        IQueryBuilder<Command> IQueryBuilder<Command>.Where(string clause)
        {
            return Where(clause);
        }

        IQueryBuilder<Command> IQueryBuilder<Command>.Where(string fieldName, object value)
        {
            WhereInternal(fieldName,value);
            return this;
        }
        
        private string GenerateParam(string fieldName, object value)
        {
            Interlocked.Increment(ref _generatedParamId);
            var name = $"@{fieldName}_{_generatedParamId}";
            Param((name, value));
            return name;
        }


        protected void WhereInternal(string fieldName, object value)
        {
            var name =GenerateParam(fieldName,value);
            Where($"{fieldName}={name}");
        }


        private IQueryBuilder<Command> Where(string whereString)
        {
            ClauseBuilder.AddWhere(whereString);
            return this;
        }
        

        IQuery ICommandCommon<IQuery>.Limit(int limit)
        {
            ClauseBuilder.Limit = limit;
            return this;
        }

        IQuery ICommandCommon<IQuery>.Order(string clause)
        {
            ClauseBuilder.OrderClause = clause;
            return this;
        }

 
        IQuery ICommandCommon<IQuery>.Param(IDbDataParameter parameter)
        {
            CreateParameter(parameter.ParameterName,parameter.Value);
            return this;
        }

        IQuery ICommandCommon<IQuery>.Param(SimpleParameter parameter)
        {
            CreateParameter(parameter.Name,parameter.Value);
            return this;
        }

        IAddParameter IAddParameter.Param(string name, object value)
        {
            CreateParameter(name,value);
            return this;
        }
        
        IAddParameter IAddParameter.Parameters(IEnumerable<IDbDataParameter> paramArray)
        {
           return Parameters(paramArray);
        }

        IAddRollBack IAddRollBack.Rollback(Action action)
        {
             Rollback(action);
             return this;
        }


        public IQuery Param(string name, object value)
        {
            CreateParameter(name,value);
            return this;
        }

        public IQuery Parameters(IEnumerable<IDbDataParameter> paramArray)
        {
            foreach (var dbDataParameter in paramArray)
            {
                CreateParameter(dbDataParameter.ParameterName,dbDataParameter.Value);
            }
            return this;
        }

        // ReSharper disable once UnusedMethodReturnValue.Local
        private IDbDataParameter Param((string name, object value) valueTuple)
        {
            var (name, value) = valueTuple;
            return CreateParameter(name,value);
        }
        public ICommandCommon<IQueryCommon> Common => this;
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

        public IQueryBuilder<Command> Or(string clause)
        {
            ClauseBuilder.AddOr(clause);
            return this;
        }
        public IQueryBuilder<Command> Or(string fieldName, object value)
        {
            OrInternal(fieldName, value);
            return this;
        }
        protected void OrInternal(string fieldName, object value)
        {
            var name = GenerateParam(fieldName, value);
            Or($"{fieldName}={name}");
        }
    }
}