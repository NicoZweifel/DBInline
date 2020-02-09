using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using DBInline.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.Data.Sqlite;
using MySql.Data.MySqlClient;
using Npgsql;

namespace DBInline.Classes
{
    public class SimpleParameter
    {
        private readonly (string name, object value) _param;
        public string Name { get; }
        public object Value { get; }

        public SimpleParameter(string name, object value)
        {
            _param = (name, value);
            Name = name;
            Value = value;
        }

        // ReSharper disable once MemberCanBePrivate.Global
        public SimpleParameter((string name, object value) param)
        {
            _param = param;
            Name = param.name;
            Value = param.value;
        }

        public DbParameter ToDbParameter(Database type)
        {
            return type switch
            {
                Database.Mssql => new SqlParameter(_param.name, _param.value),
                Database.Postgres => new NpgsqlParameter(_param.name, _param.value),
                Database.Mysql => new MySqlParameter(_param.name, _param.value),
                Database.SqlLite => new SqliteParameter(_param.name, _param.value),
                _ => throw new NotImplementedException("Database not recognized.")
            };
        }
    }

    public class Parameter : DbParameter ,IAddParameter,IAddRollBack
    {
        private readonly Command _command;

        internal readonly DbParameter DbParameter;
        public Parameter(DbParameter parameter)
        {
            DbParameter = parameter;
        }
        internal Parameter(Command command, DbParameter parameter)
        {
            _command = command;
            DbParameter = parameter;
        }

        public override void ResetDbType()
        {
            DbParameter.ResetDbType();
        }

        public override DbType DbType
        {
            get => DbParameter.DbType;
            set => DbParameter.DbType = value;
        }

        public override ParameterDirection Direction
        {
            get => DbParameter.Direction;
            set => DbParameter.Direction = value;
        }

        public override bool IsNullable { get => DbParameter.IsNullable; set =>DbParameter.IsNullable=value; }
        public override string ParameterName 
        {
            get => DbParameter.ParameterName;
            set => DbParameter.ParameterName = value;
        }


        public  string Name
        {
            get => DbParameter.ParameterName;
            set => DbParameter.ParameterName = value;
        }

        public override string SourceColumn
        {
            get => DbParameter.SourceColumn;
            set => DbParameter.SourceColumn = value;
        }

        public override object Value
        {
            get => DbParameter.Value;
            set => DbParameter.Value = value;
        }

        public override bool SourceColumnNullMapping
        {
            get => DbParameter.SourceColumnNullMapping;
            set => DbParameter.SourceColumnNullMapping = value;
        }

        public override int Size
        {
            get => DbParameter.Size;
            set => DbParameter.Size = value;
        }
        // ReSharper disable once MemberCanBePrivate.Global
        public IAddRollBack Rollback(Action action)
        {
            _command.Transaction.Rollback(action);
            return this;
        }
        // ReSharper disable once MemberCanBePrivate.Global

        public IAddParameter Param(string name, object value)
        {
            return _command.Param(name, value);
        }

        public IAddParameter Parameters(IEnumerable<IDbDataParameter> paramArray)
        {
            foreach (var dbDataParameter in paramArray)
            {
                Param(dbDataParameter.ParameterName, dbDataParameter.Value);
            }
            return this;
        }

        public IDbDataParameter AddParam((string name, object value) valueTuple)
        {
            _command.Param(valueTuple.name,valueTuple.value);
            return this;
        }
    }
}
