using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using DBInline.Interfaces;

namespace DBInline.Classes
{
    public class Transaction : DbTransaction, ICommandBuilder, ITokenHolder, IConnectionSource
    {
        protected IConnectionSource ConnectionSource { get; set; }
        internal DbTransaction DbTransaction { get; set; }

        internal readonly List<Action> RollbackActions = new List<Action>();
        public CancellationToken Token { get; set; }
        internal new DatabaseConnection Connection { get;  set; }
        protected override DbConnection DbConnection => Connection;
        public override IsolationLevel IsolationLevel => DbTransaction.IsolationLevel;

        // ReSharper disable once MemberCanBePrivate.Global
        // ReSharper disable once CollectionNeverQueried.Global
        [NotNull] public readonly List<IDbDataParameter> ParameterCollection = new List<IDbDataParameter>();

        // ReSharper disable once CollectionNeverQueried.Local
        private readonly List<Command> _commands = new List<Command>();

        internal Transaction(IConnectionSource connection)
        {
            Connection = connection.Connection();
            ConnectionSource = connection;
            CommandCreated += connection.OnCommandCreated;
        }
       internal Transaction( DatabaseConnection conn)
        {
            Connection = conn;
            ConnectionSource = conn.Context;
        }
            internal Transaction(IConnectionSource connection, string databaseName)
        {
            Connection = connection.Connection(databaseName);
        }


        public override void Commit()
        {
            DbTransaction.Commit();
        }

        public override void Rollback()
        {
            DbTransaction.Rollback();
            RollbackActions.ForEach(x => x.DynamicInvoke());
        }

        public IAddRollBack Rollback(Action action)
        {
            RollbackActions.Add(action);
            return this;
        }


        public IDbDataParameter AddParam(string name, object value)
        {
            var p = new SimpleParameter(name, value).ToDbParameter(Connection.DbType);
            ParameterCollection.Add(p);
            return p;
        }

        public IAddParameter Parameters(IEnumerable<IDbDataParameter> paramArray)
        {
            foreach (var p in paramArray)
            {
                AddParam(p.ParameterName, p.Value);
            }

            return this;
        }

        IAddParameter IAddParameter.Param(string name, object value)
        {
            AddParam(name, value);
            return this;
        }

        public IDbDataParameter AddParam((string name, object value) valueTuple)
        {
            return AddParam(valueTuple.name, valueTuple.value);
        }

        public IAddParameter AddParam(SimpleParameter parameter)
        {
            var p = new SimpleParameter(parameter.Name, parameter.Value);
            ParameterCollection.Add(p.ToDbParameter(Connection.DbType));
            return this;
        }

        public IAddParameter AddParam(Parameter parameter)
        {
            var p = new SimpleParameter(parameter.Name, parameter.Value);
            ParameterCollection.Add(p.ToDbParameter(Connection.DbType));
            return this;
        }

        public IAddParameter AddParameters(IEnumerable<(string name, object value)> paramArray)
        {
            foreach (var (name, value) in paramArray)
            {
                AddParam(name, value);
            }

            return this;
        }

        public IAddParameter AddParameters(IEnumerable<SimpleParameter> paramArray)
        {
            foreach (var p in paramArray)
            {
                AddParam(p.Name, p.Value);
            }

            return this;
        }

        public IAddParameter AddParameters(IEnumerable<Parameter> paramArray)
        {
            foreach (var p in paramArray)
            {
                AddParam(p.Name, p.Value);
            }

            return this;
        }

        public void OnRollBackAdded(Action a)
        {
            RollbackActions.Add(a);
            var handler = RollbackAdded;
            handler?.Invoke(a);
        }

        public event RollbackAdded RollbackAdded;
        public Database DefaultDbType => Connection.DbType;



        DatabaseConnection IConnectionSource.Connection()
        {
            return Connection;
        }

        DatabaseConnection IConnectionSource.Connection(string contextName)
        {
            return Connection.Context.Connection(contextName);
        }

        public void OnTransactionCreated(Transaction transaction)
        {
            var handler = TransactionCreated;
            handler?.Invoke(transaction);
        }

        public event TransactionCreated TransactionCreated;

        public void OnTransactionWrapped(Transaction transaction)
        {
            var handler = TransactionWrapped;
            handler?.Invoke(transaction);
        }

        public  event TransactionWrapped TransactionWrapped;


        public void OnCommandCreated(Command command)
        {
            _commands.Add(command);
            var handler = CommandCreated;
            handler?.Invoke(command);
        }

        Transaction IConnectionSource.Transaction()
        {
            return this;
        }

        Transaction IConnectionSource.Transaction(string databaseName)
        {
            return ConnectionSource.Transaction(databaseName);
        }


        public event CommandCreated CommandCreated; 

    
    }
}

