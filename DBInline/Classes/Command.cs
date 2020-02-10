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
    internal class Command<T> : Command,IQuery<T>,  IConditionBuilder<Command<T>>
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

            return (T) res;
        }

        public new async Task<T> ScalarAsync()
        {
            var res = await ExecuteScalarAsync(Token).ConfigureAwait(false);
            if (res == DBNull.Value)
            {
                return default;
            }

            return (T) res;
        }
        
        IQuery<T> IQuery<T>.Set(string text)
        {
            CommandBuilder.CommandText = text;
            return this;
        }

        IConditionBuilder<IQuery<T>> ICommandBuilder<IQuery<T>>.Where(string fieldName, object value)
        {
            WhereInternal(fieldName, value);
            return this;
        }

        IConditionBuilder<IQuery<T>> ICommandBuilder<IQuery<T>>.Where(string clause)
        {
            CommandBuilder.AddWhere(clause);
            return this;
        }

        IQuery<T> ICommandBuilderCommon<IQuery<T>>.Order(string clause)
        {
            CommandBuilder.OrderClause = clause;
            return this;
        }

        IQuery<T> ICommandBuilderCommon<IQuery<T>>.Limit(int limit)
        {
            CommandBuilder.Limit = limit;
            return this;
        }

        IQuery<T> ICommandBuilderCommon<IQuery<T>>.Rollback(Action action)
        {
            Transaction.Rollback(action);
            return this;
        }

        IQuery<T> ICommandBuilderCommon<IQuery<T>>.Param(string name, object value)
        {
            Param(name, value);
            return this;
        }

        IQuery<T> ICommandBuilderCommon<IQuery<T>>.Param(IDbDataParameter parameter)
        {
            Param(parameter.ParameterName, parameter.Value);
            return this;
        }

        IQuery<T> ICommandBuilderCommon<IQuery<T>>.Param(SimpleParameter parameter)
        {
            Param(parameter.Name, parameter.Value);
            return this;
        }

        IQuery<T> ICommandBuilderCommon<IQuery<T>>.Parameters(IEnumerable<IDbDataParameter> paramArray)
        {
            Parameters(paramArray);
            return this;
        }

        Command<T> IConditionBuilder<Command<T>>.Or(string clause)
        {
            CommandBuilder.AddOr(clause);
            return this;
        }

        Command<T> IConditionBuilder<Command<T>>.Or(string fieldName, object value)
        {
            OrInternal(fieldName, value);
            return this;
        }
    }

    public class Command : DbCommand, ITokenHolder, IQuery, IConditionBuilder<Command>
    {
        public Command(string commandText, Transaction transaction, bool isolated = true)
        {
            _isolated = isolated;
            Connection = transaction.Connection;
            DbCommand = transaction.Connection.CreateCommand();
            DbCommand.Transaction = transaction.DbTransaction;
            CommandBuilder.CommandText = commandText;
            Transaction = transaction;
            Transaction.OnCommandCreated(this);
        }
        
        private readonly bool _isolated;

        internal readonly DbCommand DbCommand;

        private int _generatedParamId;
        public new DatabaseConnection Connection { get; }
        public new Transaction Transaction { get; }

        internal readonly CommandBuilder CommandBuilder = new CommandBuilder();

        protected override DbConnection DbConnection { get; set; }
        protected override DbParameterCollection DbParameterCollection => DbCommand.Parameters;

        protected override DbTransaction DbTransaction { get; set; }

        public override string CommandText
        {
            get => CommandBuilder.CommandText;
            set => CommandBuilder.CommandText = value;
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

        public override bool DesignTimeVisible
        {
            get => DbCommand.DesignTimeVisible;
            set => DbCommand.DesignTimeVisible = value;
        }
        public ICommandBuilderCommon<IQueryCommon> BuilderCommon => this;
        Command IWrapCommand.Command => this;
        public CancellationToken Token => Transaction.Token;

        public Task<int> RunAsync()
        {
            CommandBuilder.BuildClauses(this);
            return DbCommand.ExecuteNonQueryAsync(Token);
        }
        
        public DataSet DataSet()
        {
            CommandBuilder.BuildClauses(this);
            var dt = new DataSet();
            using var da = new DataAdapter(this);
            da.Fill(dt);
            return dt;
        }

        public async Task<DataSet> DataSetAsync()
        {
            CommandBuilder.BuildClauses(this);
            var dt = new DataSet();
            await using var da = new DataAdapter(this);
            await da.FillAsync(dt, Token).ConfigureAwait(false);
            return dt;
        }

        public DbDataReader Reader()
        {
            CommandBuilder.BuildClauses(this);
            return ExecuteReader();
        }

        public DataTable Table()
        {
            CommandBuilder.BuildClauses(this);
            var dt = new DataTable();
            using var da = new DataAdapter(this);
            da.Fill(dt);
            return dt;
        }

        public async Task<DataTable> TableAsync()
        {
            CommandBuilder.BuildClauses(this);
            var dt = new DataTable();
            await using var da = new DataAdapter(this);
            await da.FillAsync(dt, Token).ConfigureAwait(false);
            return dt;
        }

        public Task<DbDataReader> ReaderAsync()
        {
            CommandBuilder.BuildClauses(this);
            return DbCommand.ExecuteReaderAsync(Token);
        }
        public override void Cancel()
        {
            DbCommand.Cancel();
        }

        public T Scalar<T>()
        {
            var res = (T) ExecuteScalar();
            return res;
        }

        public async Task<T> ScalarAsync<T>()
        {
            var res = (T) await ExecuteScalarAsync(Token);
            return res;
        }

        public object Scalar()
        {
            return ExecuteScalar();
        }

        public async Task<object> ScalarAsync()
        {
            var res = await ExecuteScalarAsync(Token);
            return res;
        }

        public IEnumerable<TOut> Select<TOut>(Func<IDataReader, TOut> transform)
        {
            CommandBuilder.BuildClauses(this);
            using var r = ExecuteReader();
            while (r.Read())
            {
                yield return transform(r);
            }
            r.Close();
        }

        public async Task<List<TOut>> SelectAsync<TOut>(Func<IDataReader, TOut> transform)
        {
            var res = new List<TOut>();
            await using var r = await ExecuteReaderAsync(Token);
            while (r.Read())
            {
                res.Add(transform(r));
            }
            r.Close();
            return res;
        }

        public async IAsyncEnumerable<TOut> SelectAsyncEnumerable<TOut>(Func<IDataReader, TOut> transform)
        {
            CommandBuilder.BuildClauses(this);
            await using var r = await ExecuteReaderAsync(Token);
            while (r.Read())
            {
                yield return transform(r);
            }
            await r.CloseAsync().ConfigureAwait(false);
        }

        private Parameter CreateParameter(string name, object value)
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
            CommandBuilder.BuildClauses(this);
            return DbCommand.ExecuteNonQuery();
        }

        public override object ExecuteScalar()
        {
            CommandBuilder.BuildClauses(this);
            return DbCommand.ExecuteScalar();
        }

        public override Task<object> ExecuteScalarAsync(CancellationToken cancellationToken)
        {
            CommandBuilder.BuildClauses(this);
            return base.ExecuteScalarAsync(cancellationToken);
        }

        public override void Prepare()
        {
            DbCommand.Prepare();
        }

        public new void Dispose()
        {
            DbCommand.Dispose();
            if (_isolated) Transaction.Dispose();
        }
        
        protected override DbParameter CreateDbParameter()
        {
            return DbCommand.CreateParameter();
        }

        protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior)
        {
            CommandBuilder.BuildClauses(this);
            return DbCommand.ExecuteReader();
        }

        IQuery IQuery.Set(string text)
        {
            CommandBuilder.CommandText = text;
            return this;
        }

        IQuery ICommandBuilderCommon<IQuery>.Rollback(Action action)
        {
            Transaction.Rollback(action);
            return this;
        }

        IQuery ICommandBuilderCommon<IQuery>.Param(string name, object value)
        {
            CreateParameter(name, value);
            return this;
        }

        public IQuery Param(IDbDataParameter parameter)
        {
            CreateParameter(parameter.ParameterName, parameter.Value);
            return this;
        }

        public IQuery Param(SimpleParameter parameter)
        {
            CreateParameter(parameter.Name, parameter.Value);
            return this;
        }

        IQuery ICommandBuilderCommon<IQuery>.Parameters(IEnumerable<IDbDataParameter> paramArray)
        {
            foreach (var dbDataParameter in paramArray)
            {
                CreateParameter(dbDataParameter.ParameterName, dbDataParameter.Value);
            }

            return this;
        }

        public IQuery Order(string clause)
        {
            CommandBuilder.AddOr(clause);
            return this;
        }

        public IQuery Limit(int limit)
        {
            CommandBuilder.Limit = limit;
            return this;
        }
        
        IConditionBuilder<IQuery> ICommandBuilder<IQuery>.Where(string clause)
        {
            return Where(clause);
        }

        IConditionBuilder<IQuery> ICommandBuilder<IQuery>.Where(string fieldName, object value)
        {
            WhereInternal(fieldName, value);
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
            var name = GenerateParam(fieldName, value);
            Where($"{fieldName}={name}");
        }
        
        private IConditionBuilder<Command> Where(string whereString)
        {
            CommandBuilder.AddWhere(whereString);
            return this;
        }
        
        IQuery ICommandBuilderCommon<IQuery>.Limit(int limit)
        {
            CommandBuilder.Limit = limit;
            return this;
        }

        IQuery ICommandBuilderCommon<IQuery>.Order(string clause)
        {
            CommandBuilder.OrderClause = clause;
            return this;
        }
        
        IQuery ICommandBuilderCommon<IQuery>.Param(IDbDataParameter parameter)
        {
            CreateParameter(parameter.ParameterName, parameter.Value);
            return this;
        }

        IQuery ICommandBuilderCommon<IQuery>.Param(SimpleParameter parameter)
        {
            CreateParameter(parameter.Name, parameter.Value);
            return this;
        }

        IAddParameter IAddParameter.Param(string name, object value)
        {
            CreateParameter(name, value);
            return this;
        }

        IAddParameter IAddParameter.Parameters(IEnumerable<IDbDataParameter> paramArray)
        {
            return Parameters(paramArray);
        }

        IAddRollBack IAddRollBack.Rollback(Action action)
        {
            Transaction.Rollback(action);
            return this;
        }
        
        public IQuery Param(string name, object value)
        {
            CreateParameter(name, value);
            return this;
        }

        public IQuery Parameters(IEnumerable<IDbDataParameter> paramArray)
        {
            foreach (var dbDataParameter in paramArray)
            {
                CreateParameter(dbDataParameter.ParameterName, dbDataParameter.Value);
            }

            return this;
        }

        // ReSharper disable once UnusedMethodReturnValue.Local
        private IDbDataParameter Param((string name, object value) valueTuple)
        {
            var (name, value) = valueTuple;
            return CreateParameter(name, value);
        }
        

        Command IConditionBuilder<Command>.Or(string clause)
        {
            CommandBuilder.AddOr(clause);
            return this;
        }

        Command IConditionBuilder<Command>.Or(string fieldName, object value)
        {
            OrInternal(fieldName, value);
            return this;
        }

        protected void OrInternal(string fieldName, object value)
        {
            var name = GenerateParam(fieldName, value);
            CommandBuilder.AddOr($"{fieldName}={name}");
        }
    }
}