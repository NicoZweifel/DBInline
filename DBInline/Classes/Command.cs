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
    internal class Command<T> : Command,
        ICommand<T>,
        IColumnsBuilder<T>,
        ICommandBuilder<T>,
        IConditionBuilder<T>,
        ICreateBuilder<T>,
        ICreateQuery<T>,
        IDropBuilder<T>,
        IDropQuery<T>,
        IInsertBuilder<T>,
        IInsertFromBuilder<T>,
        IInsertFromQuery<T>,
        IInsertQuery<T>,
        IQuery<T>,
        IRowBuilder<T>,
        ISelectBuilder<T>,
        ITokenHolder,
        IUpdateBuilder<T>,
        IUpdateQuery<T>,
        IValuesBuilder<T>
    {
        public Command(string commandText, Transaction transaction) : base(commandText, transaction)
        {
        }

        T IQuery<T>.Scalar()
        {
            var res = ExecuteScalar();
            if (res == DBNull.Value)
            {
                return default;
            }

            return (T) res;
        }

        async Task<T> IQuery<T>.ScalarAsync()
        {
            var res = await ExecuteScalarAsync(Token).ConfigureAwait(false);
            if (res == DBNull.Value)
            {
                return default;
            }

            return (T) res;
        }

        IEnumerable<T> IQuery<T>.Get(Func<IDataReader, T> transform)
        {
            throw new NotImplementedException();
        }

        Task<List<T>> IQuery<T>.GetAsync(Func<IDataReader, T> transform)
        {
            throw new NotImplementedException();
        }

        IAsyncEnumerable<T> IQuery<T>.GetAsyncEnumerable(Func<IDataReader, T> transform)
        {
            throw new NotImplementedException();
        }

        IConditionBuilder<T> ICommandBuilder<T>.Where(string fieldName, object value)
        {
            WhereInternal(fieldName, value);
            return this;
        }

        IConditionBuilder<T> ICommandBuilder<T>.WhereNot(string fieldName, object value)
        {
            throw new NotImplementedException();
        }

        IConditionBuilder<T> ICommandBuilder<T>.Where(string clause)
        {
            CommandBuilder.AddWhere(clause);
            return this;
        }
        
        IConditionBuilder<T> IConditionBuilder<T>.Or(string clause)
        {
            CommandBuilder.AddOr(clause);
            return this;
        }

        IConditionBuilder<T> IConditionBuilder<T>.Or(string fieldName, object value)
        {
            OrInternal(fieldName, value);
            return this;
        }


        IQuery<T> IInsertBuilder<T>.From(string tableName)
        {
            throw new NotImplementedException();
        }
        ISelectBuilder<T> ISelectBuilder<T>.Add(params string[] columnNames)
        {
            throw new NotImplementedException();
        }

        IQuery<T> ISelectBuilder<T>.From(string tableName)
        {
            throw new NotImplementedException();
        }

        IInsertBuilder<T> IInsertBuilder<T>.Add(params string[] columnNames)
        {
            throw new NotImplementedException();
        }

        IValuesBuilder<T> IInsertBuilder<T>.Values()
        {
            throw new NotImplementedException();
        }

        IColumnsBuilder<T> IColumnsBuilder<T>.Add(params string[] columnName)
        {
            throw new NotImplementedException();
        }

        ISelectBuilder<T> IColumnsBuilder<T>.Select()
        {
            throw new NotImplementedException();
        }

        ISelectBuilder<T> IColumnsBuilder<T>.Select(params string[] columns)
        {
            throw new NotImplementedException();
        }

        IValuesBuilder IColumnsBuilder<T>.Values()
        {
            throw new NotImplementedException();
        }

        IInsertQuery<T> IColumnsBuilder<T>.Values(params string[] values)
        {
            throw new NotImplementedException();
        }
        
        ISelectBuilder<T> ICommand<T>.Select(params string[] columns)
        {
            throw new NotImplementedException();
        }

        IInsertBuilder<T> ICommand<T>.Insert(string tableName)
        {
            throw new NotImplementedException();
        }

        IUpdateBuilder<T> ICommand<T>.Update(string tableName)
        {
            throw new NotImplementedException();
        }

        IDropBuilder<T> ICommand<T>.Drop(string tableName)
        {
            throw new NotImplementedException();
        }

        ICreateBuilder<T> ICommand<T>.Create(string tableName)
        {
            throw new NotImplementedException();
        }

        IDeleteQuery<T> ICommand<T>.Delete(string tableName)
        {
            throw new NotImplementedException();
        }

        ICreateQuery<T> ICreateBuilder<T>.Add(string column, SqlDbType type, int charCount)
        {
            throw new NotImplementedException();
        }

        IDropQuery<T> IDropBuilder<T>.IfExists()
        {
            throw new NotImplementedException();
        }

        IInsertFromQuery<T> IInsertFromBuilder<T>.Select(params string[] columns)
        {
            throw new NotImplementedException();
        }

        ICommandBuilder<T> IInsertFromQuery<T>.From(string tableName)
        {
            throw new NotImplementedException();
        }

        IRowBuilder<T> IRowBuilder<T>.Row()
        {
            throw new NotImplementedException();
        }

        IRowBuilder<T> IRowBuilder<T>.Add<TIn>(TIn value)
        {
            throw new NotImplementedException();
        }

        IUpdateQuery<T> IUpdateBuilder<T>.Set<TParam>(string columnName, TParam value)
        {
            throw new NotImplementedException();
        }

        IRowBuilder<T> IValuesBuilder<T>.Row()
        {
            throw new NotImplementedException();
        }
    }

    public class Command : 
        DbCommand,
        ICommand,
        IColumnsBuilder,
        ICommandBuilder,
        IConditionBuilder,
        ICreateBuilder,
        ICreateQuery,
        IDropBuilder,
        IDropQuery,
        IInsertBuilder,
        IInsertFromBuilder,
        IInsertFromQuery,
        IInsertQuery,
        IQuery,
        IRowBuilder,
        ISelectBuilder,
        ITokenHolder,
        IUpdateBuilder,
        IUpdateQuery,
        IValuesBuilder
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
        Command IWrapCommand.Command => this;
        public CancellationToken Token => Transaction.Token;

        public Task<int> RunAsync()
        {
            CommandBuilder.BuildClauses(this);
            return DbCommand.ExecuteNonQueryAsync(Token);
        }

        public IDropQuery IfExists()
        {
            throw new NotImplementedException();
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

        async Task<DataTable> IQuery.TableAsync()
        {
            CommandBuilder.BuildClauses(this);
            var dt = new DataTable();
            await using var da = new DataAdapter(this);
            await da.FillAsync(dt, Token).ConfigureAwait(false);
            return dt;
        }

        Task<DbDataReader> IQuery.ReaderAsync()
        {
            CommandBuilder.BuildClauses(this);
            return DbCommand.ExecuteReaderAsync(Token);
        }
        public override void Cancel()
        {
            DbCommand.Cancel();
        }

        T IQuery.Scalar<T>()
        {
            var res = (T) ExecuteScalar();
            return res;
        }

        async Task<T> IQuery.ScalarAsync<T>()
        {
            var res = (T) await ExecuteScalarAsync(Token);
            return res;
        }

        private object Scalar()
        {
            return ExecuteScalar();
        }

        private async Task<object> ScalarAsync()
        {
            var res = await ExecuteScalarAsync(Token);
            return res;
        }

        IEnumerable<TOut> IQuery.Get<TOut>(Func<IDataReader, TOut> transform)
        {
            CommandBuilder.BuildClauses(this);
            using var r = ExecuteReader();
            while (r.Read())
            {
                yield return transform(r);
            }
            r.Close();
        }

        async Task<List<TOut>> IQuery.GetAsync<TOut>(Func<IDataReader, TOut> transform)
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

        async IAsyncEnumerable<TOut> IQuery.GetAsyncEnumerable<TOut>(Func<IDataReader, TOut> transform)
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

        ICommandBuilder IInsertFromQuery.From(string tableName)
        {
            throw new NotImplementedException();
        }
        
        ISelectBuilder ISelectBuilder.Add(params string[] columnNames)
        {
            throw new NotImplementedException();
        }

        IQuery ISelectBuilder.From(string tableName)
        {
            throw new NotImplementedException();
        }
        
        IInsertBuilder IInsertBuilder.Add(params string[] columnNames)
        {
            throw new NotImplementedException();
        }

        IValuesBuilder IInsertBuilder.Values()
        {
            throw new NotImplementedException();
        }

        private object RunScalar()
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
        
        IQuery ICommandBuilder.Param(IDbDataParameter parameter)
        {
            CreateParameter(parameter.ParameterName, parameter.Value);
            return this;
        }

        IQuery ICommandBuilder.Param(SimpleParameter parameter)
        {
            CreateParameter(parameter.Name, parameter.Value);
            return this;
        }
        
        IQuery ICommandBuilder.Order(string clause)
        {
            CommandBuilder.AddOr(clause);
            return this;
        }

        IQuery ICommandBuilder.Limit(int limit)
        {
            CommandBuilder.Limit = limit;
            return this;
        }
        
        IConditionBuilder ICommandBuilder.Where(string clause)
        {
            return Where(clause);
        }

        IConditionBuilder ICommandBuilder.Where(string fieldName, object value)
        {
            WhereInternal(fieldName, value);
            return this;
        }

        IConditionBuilder ICommandBuilder.WhereNot(string fieldName, object value)
        {
            throw new NotImplementedException();
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
        
        private IConditionBuilder Where(string whereString)
        {
            CommandBuilder.AddWhere(whereString);
            return this;
        }

        IAddParameter IAddParameter.Param(string name, object value)
        {
            CreateParameter(name, value);
            return this;
        }

        public IAddParameter Params(IEnumerable<IDbDataParameter> paramArray)
        {
            foreach (var dbDataParameter in paramArray)
            {
                CreateParameter(dbDataParameter.ParameterName, dbDataParameter.Value);
            }
            return this;
        }

        IAddRollBack IAddRollBack.Rollback(Action action)
        {
            Transaction.Rollback(action);
            return this;
        }
        
        internal IDbDataParameter Param(string name, object value)
        {
           return CreateParameter(name, value);
        }
        internal IDbDataParameter Param((string name, object value) valueTuple)
        {
            var (name, value) = valueTuple;
            return CreateParameter(name, value);
        }
        
        IConditionBuilder IConditionBuilder.Or(string clause)
        {
            CommandBuilder.AddOr(clause);
            return this;
        }

        IConditionBuilder IConditionBuilder.Or(string fieldName, object value)
        {
            OrInternal(fieldName, value);
            return this;
        }

        protected void OrInternal(string fieldName, object value)
        {
            var name = GenerateParam(fieldName, value);
            CommandBuilder.AddOr($"{fieldName}={name}");
        }


        IQuery IInsertBuilder.From(string tableName)
        {
            throw new NotImplementedException();
        }

        IColumnsBuilder IColumnsBuilder.Add(params string[] columnName)
        {
            throw new NotImplementedException();
        }

        ISelectBuilder IColumnsBuilder.Select()
        {
            throw new NotImplementedException();
        }

        ISelectBuilder IColumnsBuilder.Select(params string[] columns)
        {
            throw new NotImplementedException();
        }

        IValuesBuilder IColumnsBuilder.Values()
        {
            throw new NotImplementedException();
        }

        IInsertQuery IColumnsBuilder.Values(params string[] values)
        {
            throw new NotImplementedException();
        }

        ISelectBuilder ICommand.Select(params string[] columns)
        {
            throw new NotImplementedException();
        }

        IInsertBuilder ICommand.Insert(string tableName)
        {
            throw new NotImplementedException();
        }

        IUpdateBuilder ICommand.Update(string tableName)
        {
            throw new NotImplementedException();
        }

        IDropBuilder ICommand.Drop(string tableName)
        {
            throw new NotImplementedException();
        }

        ICreateBuilder ICommand.Create(string tableName)
        {
            throw new NotImplementedException();
        }

        IDeleteQuery ICommand.Delete(string tableName)
        {
            throw new NotImplementedException();
        }

        IRowBuilder IValuesBuilder.Row()
        {
            throw new NotImplementedException();
        }

        IInsertQuery IRowBuilder.Add<TIn>(TIn value)
        {
            throw new NotImplementedException();
        }

        ICreateQuery ICreateBuilder.Add(string column, SqlDbType type, int charCount)
        {
            throw new NotImplementedException();
        }

        IInsertFromQuery IInsertFromBuilder.Select(params string[] columns)
        {
            throw new NotImplementedException();
        }

        IRowBuilder IRowBuilder.Row()
        {
            throw new NotImplementedException();
        }

        IUpdateQuery IUpdateBuilder.Set<TParam>(string columnName, TParam value)
        {
            throw new NotImplementedException();
        }
    }
}