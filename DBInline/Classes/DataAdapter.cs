using System;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using MySql.Data.MySqlClient;
using Npgsql;


namespace DBInline.Classes
{
    public class DataAdapter : DbDataAdapter, IDbDataAdapter, IDisposable, IAsyncDisposable
    {
        private readonly DbDataAdapter _adapter;

        internal DataAdapter(Command command)
        {
            // ReSharper disable once SwitchExpressionHandlesSomeKnownEnumValuesWithExceptionInDefault
            _adapter = command.Connection.DbType switch
            {
                Database.Mssql => new SqlDataAdapter((SqlCommand) command.DbCommand),
                Database.Postgres => new NpgsqlDataAdapter((NpgsqlCommand) command.DbCommand),
                Database.Mysql => new MySqlDataAdapter((MySqlCommand) command.DbCommand),
                _ => throw new NotImplementedException("Database not recognized.")
            };
        }

        ValueTask IAsyncDisposable.DisposeAsync()
        {
            return DisposeAsync();
        }

        void IDisposable.Dispose()
        {
            Dispose();
        }

        public override ISite Site { get; set; }

        DataTable[] IDataAdapter.FillSchema(DataSet dataSet, SchemaType schemaType)
        {
            return FillSchema(dataSet, schemaType);
        }

        IDataParameter[] IDataAdapter.GetFillParameters()
        {
            return GetFillParameters();
        }

        int IDataAdapter.Update(DataSet dataSet)
        {
            return Update(dataSet);
        }

        MissingMappingAction IDataAdapter.MissingMappingAction
        {
            get => MissingMappingAction;
            set => MissingMappingAction = value;
        }

        MissingSchemaAction IDataAdapter.MissingSchemaAction
        {
            get => MissingSchemaAction;
            set => MissingSchemaAction = value;
        }

        ITableMappingCollection IDataAdapter.TableMappings => TableMappings;


        int IDataAdapter.Fill(DataSet dataSet)
        {
            return Fill(dataSet);
        }


        public override int Fill(DataSet dataSet)
        {
            return _adapter.Fill(dataSet);
        }
        public  Task<int> FillAsync(DataSet dataSet)
        {
            return Task.Run(() => _adapter.Fill(dataSet));
        }
        public  Task<int> FillAsync(DataSet dataSet,CancellationToken token)
        {
            return Task.Run(() => _adapter.Fill(dataSet),token);
        }
        public Task<int> FillAsync(DataTable dataTable)
        {
            return Task.Run(() => _adapter.Fill(dataTable));
        }
        public Task<int> FillAsync(DataTable dataTable, CancellationToken token)
        {
            return Task.Run(() => _adapter.Fill(dataTable),token);
        }
        public override DataTable[] FillSchema(DataSet dataSet, SchemaType schemaType)
        {
            return _adapter.FillSchema(dataSet, schemaType);
        }

        public override IDataParameter[] GetFillParameters()
        {
            return _adapter.GetFillParameters();
        }

        // ReSharper disable once MemberCanBePrivate.Global
        public new int Update(DataSet dataSet)
        {
            return _adapter.Update(dataSet);
        }


        public override bool ReturnProviderSpecificTypes { get; set; }

        // ReSharper disable once MemberCanBePrivate.Global
        public new ITableMappingCollection TableMappings => _adapter.TableMappings;

        // ReSharper disable once MemberCanBePrivate.Global
        public new IDbCommand DeleteCommand
        {
            get => _adapter.DeleteCommand;
            set => _adapter.DeleteCommand = value as DbCommand;
        }

        IDbCommand IDbDataAdapter.InsertCommand
        {
            get => InsertCommand;
            set => InsertCommand = value;
        }

        IDbCommand IDbDataAdapter.SelectCommand
        {
            get => SelectCommand;
            set => SelectCommand = value;
        }

        IDbCommand IDbDataAdapter.UpdateCommand
        {
            get => UpdateCommand;
            set => UpdateCommand = value;
        }

        IDbCommand IDbDataAdapter.DeleteCommand
        {
            get => DeleteCommand;
            set => DeleteCommand = value;
        }

        // ReSharper disable once MemberCanBePrivate.Global
        public new IDbCommand InsertCommand
        {
            get => _adapter.InsertCommand;
            set => _adapter.InsertCommand = value as DbCommand;
        }

        // ReSharper disable once MemberCanBePrivate.Global
        public new IDbCommand SelectCommand
        {
            get => _adapter.SelectCommand;
            set => _adapter.SelectCommand = value as DbCommand;
        }

        // ReSharper disable once MemberCanBePrivate.Global
        public new IDbCommand UpdateCommand
        {
            get => _adapter.UpdateCommand;
            set => _adapter.UpdateCommand = value as DbCommand;
        }

        public override int UpdateBatchSize { get; set; }

        public new void Dispose()
        {
            _adapter.Dispose();
        }

        // ReSharper disable once MemberCanBePrivate.Global
        public ValueTask DisposeAsync()
        {
            return new ValueTask(Task.Run(() => _adapter.Dispose()));
        }
    }
}
