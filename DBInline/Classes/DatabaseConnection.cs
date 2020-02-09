using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using DBInline.Classes.Transactions;
using DBInline.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.Data.Sqlite;
using MySql.Data.MySqlClient;
using Npgsql;

namespace DBInline.Classes
{
    public class DatabaseConnection : DbConnection 
    {
        // ReSharper disable once MemberCanBePrivate.Global
        public readonly Database DbType;
        private readonly DbConnection _innerConnection;
        public readonly DatabaseContext Context;
        private readonly List<Command> _commands = new List<Command>();
        public DatabaseConnection (Database type,string connectionString, DatabaseContext context)
        {
            DbType = type;
            Context = context;
            _innerConnection = DbType switch
            {
                DBInline.Database.Mssql => new SqlConnection(connectionString),
                DBInline.Database.Postgres => new NpgsqlConnection(connectionString),
                DBInline.Database.Mysql => new MySqlConnection(connectionString),
                DBInline.Database.SqlLite => new SqliteConnection(connectionString),
                _ => _innerConnection
            };
        }

        public sealed override string ConnectionString { get => _innerConnection.ConnectionString; set => _innerConnection.ConnectionString = value; }

        public override string Database => _innerConnection.Database;

        public override string DataSource => _innerConnection.DataSource;

        public override string ServerVersion => _innerConnection.ServerVersion;

        public override ConnectionState State => _innerConnection.State;

        public override void ChangeDatabase(string databaseName)
        {
            _innerConnection.ChangeDatabase(databaseName);
        }
        public override void Close()
        {
            _innerConnection.Close();
        }
        public override void Open()
        {
            _innerConnection.Open();
        }
        
        protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel)
        {
            return _innerConnection.BeginTransaction(isolationLevel);
        }
        protected override DbCommand CreateDbCommand()
        {
          return  _innerConnection.CreateCommand();
        }

    }
}