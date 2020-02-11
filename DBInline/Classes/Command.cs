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
        IValuesBuilder<T>,
        IDeleteQuery<T>
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
            var res = new List<T>();
            using var r =  ExecuteReader();
            while (r.Read())
            {
                res.Add(transform(r));
            }

            r.Close();
            return res;
        }

        async Task<List<T>> IQuery<T>.GetAsync(Func<IDataReader, T> transform)
        {
            var res = new List<T>();
            await using var r = await ExecuteReaderAsync(Token);
            while (r.Read())
            {
                res.Add(transform(r));
            }

            r.Close();
            return res;
        }

        async IAsyncEnumerable<T> IQuery<T>.GetAsyncEnumerable(Func<IDataReader, T> transform)
        {
            await using var r = await ExecuteReaderAsync(Token);
            while (r.Read())
            {
                yield return transform(r);
            }

            await r.CloseAsync().ConfigureAwait(false);
        }

        IConditionBuilder<T> ICommandBuilder<T>.Where(string fieldName, object value)
        {
            WhereInternal(fieldName, "=",value);
            return this;
        }

        IConditionBuilder<T> ICommandBuilder<T>.WhereNot(string column, object value)
        {
            WhereInternal(column,"!=",value);
            return this;
        }

        IConditionBuilder<T> ICommandBuilder<T>.Where(string clause)
        {
            CommandTextBuilder.AddWhere(clause);
            return this;
        }
        
        IConditionBuilder<T> IConditionBuilder<T>.Or(string clause)
        {
            CommandTextBuilder.AddOr(clause);
            return this;
        }

        IConditionBuilder<T> IConditionBuilder<T>.Or(string fieldName, object value)
        {
            OrInternal(fieldName, value);
            return this;
        }

        IQuery<T> IInsertBuilder<T>.From(string table)
        {
            CommandTextBuilder.AddFrom(table);
            return this;
        }
        ISelectBuilder<T> ISelectBuilder<T>.Add(params string[] columns)
        {
            CommandTextBuilder.AddColumns(columns);
            return this;
        }

        IQuery<T> ISelectBuilder<T>.From(string table)
        {
            CommandTextBuilder.AddFrom(table);
            return this;
        }

        IInsertBuilder<T> IInsertBuilder<T>.Add(params string[] columns)
        {
            CommandTextBuilder.AddColumns(columns);
            return this;
        }

        IValuesBuilder<T> IInsertBuilder<T>.Values()
        {
            return this;
        }

        IColumnsBuilder<T> IColumnsBuilder<T>.Add(params string[] columns)
        {
            CommandTextBuilder.AddColumns(columns);
            return this;
        }

        ISelectBuilder<T> IColumnsBuilder<T>.Select()
        {
            CommandTextBuilder.AddSelect();
            return this;
        }

        ISelectBuilder<T> IColumnsBuilder<T>.Select(params string[] columns)
        {
            CommandTextBuilder.AddSelect();
            CommandTextBuilder.AddColumns(columns);
            return this;
        }

        IValuesBuilder IColumnsBuilder<T>.Values()
        {
            return this;
        }

        IInsertQuery<T> IColumnsBuilder<T>.Values(params string[] values)
        {
            CommandTextBuilder.AddValues(values);
            return this;
        }
        
        ISelectBuilder<T> ICommand<T>.Select(params string[] columns)
        {
            CommandTextBuilder.AddSelect();
            return this;
        }

        IInsertBuilder<T> ICommand<T>.Insert(string tableName)
        {
            CommandTextBuilder.AddInsert(tableName);
            return this;
        }

        IUpdateBuilder<T> ICommand<T>.Update(string tableName)
        {
            CommandTextBuilder.AddUpdate(tableName);
            return this;
        }

        IDropBuilder<T> ICommand<T>.Drop(string tableName)
        {
            CommandTextBuilder.AddDrop(tableName);
            return this;
        }

        ICreateBuilder<T> ICommand<T>.Create(string tableName)
        {
            CommandTextBuilder.AddCreate(tableName);
            return this;
        }

        IDeleteQuery<T> ICommand<T>.Delete(string tableName)
        {
            CommandTextBuilder.AddDelete(tableName);
            return this;
        }

        ICreateQuery<T> ICreateBuilder<T>.Add(string column, SqlDbType type, int charCount)
        {
            CommandTextBuilder.AddColumnDefinition(column,type,charCount);
            return this;
        }

        IDropQuery<T> IDropBuilder<T>.IfExists()
        {
            CommandTextBuilder.AddIfExists();
            return this;
        }

        IInsertFromQuery<T> IInsertFromBuilder<T>.Select(params string[] columns)
        {
            CommandTextBuilder.AddInsertFromColumns(columns ?? new string[]{});
            return this;
        }

        ICommandBuilder<T> IInsertFromQuery<T>.From(string table)
        {
            CommandTextBuilder.AddFrom(table);
            return this;
        }

        IRowBuilder<T> IRowBuilder<T>.Row()
        {
            CommandTextBuilder.AddRow();
            return this;
        }

        IRowBuilder<T> IRowBuilder<T>.Add<TIn>(TIn value)
        {
            CommandTextBuilder.AddToRow(value);
            return this;
        }

        IUpdateQuery<T> IUpdateBuilder<T>.Set<TParam>(string columnName, TParam value)
        {
            var name = GenerateParam(columnName, value);
            CommandTextBuilder.AddUpdateValue(columnName,name);
            return this;
        }

        IRowBuilder<T> IValuesBuilder<T>.Row()
        {
           CommandTextBuilder.AddRow();
           return this;
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
        IValuesBuilder,
        IDeleteQuery
    {
        public Command(string commandText, Transaction transaction, bool isolated = true)
        {
            _isolated = isolated;
            Connection = transaction.Connection;
            DbCommand = transaction.Connection.CreateCommand();
            DbCommand.Transaction = transaction.DbTransaction;
            Transaction = transaction;
            Transaction.OnCommandCreated(this);
        }
        
        private readonly bool _isolated;

        internal readonly DbCommand DbCommand;

        private int _generatedParamId;
        public new DatabaseConnection Connection { get; }
        public new Transaction Transaction { get; }

        internal readonly CommandTextBuilder CommandTextBuilder = new CommandTextBuilder();

        protected override DbConnection DbConnection { get; set; }
        protected override DbParameterCollection DbParameterCollection => DbCommand.Parameters;

        protected override DbTransaction DbTransaction { get; set; }

        public override string CommandText
        {
            get => DbCommand.CommandText;
            set => DbCommand.CommandText = value;
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
            return DbCommand.ExecuteNonQueryAsync(Token);
        }

        public IDropQuery IfExists()
        {
            CommandTextBuilder.AddIfExists();
            return this;
        }

        public DataSet DataSet()
        {
            var dt = new DataSet();
            using var da = new DataAdapter(this);
            da.Fill(dt);
            return dt;
        }

        public async Task<DataSet> DataSetAsync()
        {
            var dt = new DataSet();
            await using var da = new DataAdapter(this);
            await da.FillAsync(dt, Token).ConfigureAwait(false);
            return dt;
        }

        public DbDataReader Reader()
        {
            return ExecuteReader();
        }

        public DataTable Table()
        {
            var dt = new DataTable();
            using var da = new DataAdapter(this);
            da.Fill(dt);
            return dt;
        }

        async Task<DataTable> IQuery.TableAsync()
        {
            var dt = new DataTable();
            await using var da = new DataAdapter(this);
            await da.FillAsync(dt, Token).ConfigureAwait(false);
            return dt;
        }

        Task<DbDataReader> IQuery.ReaderAsync()
        {
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
            await using var r = await ExecuteReaderAsync(Token);
            while (r.Read())
            {
                yield return transform(r);
            }
            await r.CloseAsync().ConfigureAwait(false);
        }

        private Parameter CreateParameter<T>(string name, T value)
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
            var res = ExecuteNonQuery();
            return res;
        }

        ICommandBuilder IInsertFromQuery.From(string tableName)
        {
            CommandTextBuilder.AddTableName(tableName);
            return this;
        }
        
        ISelectBuilder ISelectBuilder.Add(params string[] columnNames)
        {
            CommandTextBuilder.AddColumns(columnNames);
            return this;
        }

        public ISelectBuilder Select()
        {
            CommandTextBuilder.AddSelect();
            return this;
        }

        IQuery ISelectBuilder.From(string tableName)
        {
           CommandTextBuilder.AddFrom(tableName);
           return this;
        }
        
        IInsertBuilder IInsertBuilder.Add(params string[] columnNames)
        {
            CommandTextBuilder.AddColumns(columnNames);
            return this;
        }

        IValuesBuilder IInsertBuilder.Values()
        {
            return this;
        }

        private object RunScalar()
        {
            var res = ExecuteScalar();
            return res;
        }

        public override int ExecuteNonQuery()
        {
            CommandTextBuilder.SetCommandText(this);
            return DbCommand.ExecuteNonQuery();
        }

        public override object ExecuteScalar()
        {
            CommandTextBuilder.SetCommandText(this);
            return DbCommand.ExecuteScalar();
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
            CommandTextBuilder.SetCommandText(this);
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
            CommandTextBuilder.AddOr(clause);
            return this;
        }

        IQuery ICommandBuilder.Limit(int limit)
        {
            CommandTextBuilder.Limit = limit;
            return this;
        }
        
        IConditionBuilder ICommandBuilder.Where(string clause)
        {
            return Where(clause);
        }

        IConditionBuilder ICommandBuilder.Where(string columnName, object value)
        {
            WhereInternal(columnName, "=",value);
            return this;
        }

        IConditionBuilder ICommandBuilder.WhereNot(string columnName, object value)
        {
            WhereInternal(columnName, "!=",value);
            return this;
        }

        protected string GenerateParam<T>(string columnName, T value)
        {
            Interlocked.Increment(ref _generatedParamId);
            var name = $"@{columnName}_{_generatedParamId}";
            Param((name, value));
            return name;
        }
        
        protected void WhereInternal<T>(string columnName, string op,T value )
        {
            var name = GenerateParam(columnName, value);
            Where($"{columnName}{op}{name}");
        }
        
        private IConditionBuilder Where(string whereString)
        {
            CommandTextBuilder.AddWhere(whereString);
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
        internal IDbDataParameter Param<T>((string name, T value) valueTuple)
        {
            var (name, value) = valueTuple;
            return CreateParameter(name, value);
        }
        
        IConditionBuilder IConditionBuilder.Or(string clause)
        {
            CommandTextBuilder.AddOr(clause);
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
            CommandTextBuilder.AddOr($"{fieldName}={name}");
        }


        IQuery IInsertBuilder.From(string tableName)
        {
            CommandTextBuilder.AddFrom(tableName);
            return this;
        }

        IColumnsBuilder IColumnsBuilder.Add(params string[] columnName)
        {
            CommandTextBuilder.AddColumns(columnName);
            return this;
        }
        
        ISelectBuilder IColumnsBuilder.Select(params string[] columns)
        {
            CommandTextBuilder.AddSelect();
            CommandTextBuilder.AddColumns(columns ??  new string []{});
            return this;
        }

        public IValuesBuilder Values()
        {
            return this;
        }

        IInsertQuery IColumnsBuilder.Values(params string[] values)
        {
            CommandTextBuilder.AddSelect();
            CommandTextBuilder.AddValues(values ??  new string []{});
            return this;
        }

        ISelectBuilder ICommand.Select(params string[] columns)
        {
            CommandTextBuilder.AddSelect();
            CommandTextBuilder.AddColumns(columns ??  new string []{});
            return this;
        }

        IInsertBuilder ICommand.Insert(string tableName)
        {
            CommandTextBuilder.AddInsert(tableName);
            return this;
        }

        IUpdateBuilder ICommand.Update(string tableName)
        {
            CommandTextBuilder.AddUpdate(tableName);
            return this;
        }

        IDropBuilder ICommand.Drop(string tableName)
        {
            CommandTextBuilder.AddDrop(tableName);
            return this;
        }

        ICreateBuilder ICommand.Create(string tableName)
        {
            CommandTextBuilder.AddCreate(tableName);
            return this;
        }

        IDeleteQuery ICommand.Delete(string tableName)
        {
            CommandTextBuilder.AddDelete(tableName);
            return this;
        }

        IRowBuilder IValuesBuilder.Row()
        {
            CommandTextBuilder.AddRow();
            return this;
        }

        IInsertQuery IRowBuilder.Add<TIn>(TIn value)
        {
            CommandTextBuilder.AddToRow(value);
            return this;
        }

        ICreateQuery ICreateBuilder.Add(string column, SqlDbType type, int charCount)
        {
            CommandTextBuilder.AddColumnDefinition(column,type,charCount);
            return this;
        }

        IInsertFromQuery IInsertFromBuilder.Select(params string[] columns)
        {
            CommandTextBuilder.AddInsertFromColumns(columns ?? new string[]{});
            return this;
        }

        IRowBuilder IRowBuilder.Row()
        {
            CommandTextBuilder.AddRow();
            return this;
        }

        IUpdateQuery IUpdateBuilder.Set<TParam>(string columnName, TParam value)
        {
            var name = GenerateParam(columnName,value);
            CommandTextBuilder.AddUpdateValue(columnName,name);
            return this;
        }
    }
}