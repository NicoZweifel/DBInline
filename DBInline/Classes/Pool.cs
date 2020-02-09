using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using DBInline.Classes.Transactions;
using DBInline.Interfaces;

namespace DBInline.Classes
{
    internal class Pool : IPool
    {
        private readonly IConnectionSource _connectionSource;

        //private readonly TransactionScope _scope;
        
        // ReSharper disable once MemberCanBeMadeStatic.Global
        public  void Cancel()
        {
            throw new PoolCanceledException();
        }

        internal class PoolCanceledException : Exception
        {
        }
        
        public Pool()
        {
            _connectionSource = ContextController.DefaultContext;
            _connectionSource.CommandCreated += OnCommandCreated;
            //_scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        }

        public Pool(IConnectionSource connectionSource)
        {
            //_scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            switch (connectionSource)
            {
                case Transaction t:
                    _transactions.Add(t.Connection.Context.Name,t);
                    break;
            }
            _connectionSource = connectionSource ?? ContextController.DefaultContext;
            _connectionSource.CommandCreated += OnCommandCreated;
        }
        
        private readonly Dictionary<string, DatabaseConnection> _connections =
            new Dictionary<string, DatabaseConnection>();
        private readonly Dictionary<string,Transaction> _transactions = new Dictionary<string, Transaction>();
        private readonly List<Transaction> _wrappedTransactions = new List<Transaction>();
        private readonly List<Command> _commands = new List<Command>();
        public IReadOnlyList<DatabaseConnection> Connections => _connections.Values.ToList();
        public IReadOnlyList<Transaction> Transactions => _transactions.Values.ToList();
        private readonly List<Action> _rollbackActions = new List<Action>();
        public void Close()
        {
            foreach (var keyValuePair in _connections)
            {
                keyValuePair.Value.Close();
            }
        }
        public void Commit()
        {
            foreach (var t in _transactions.Values.Union(_wrappedTransactions))
            {
                t.Commit();
            }
            Clean();
        }

        void IPool.Rollback()
        {  
            var exceptions = new List<Exception>();
            foreach (var command in _commands)
            {
                try
                {
                    command.Cancel();
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                }
            }
            try
            {
                TokenSource.Cancel();
            }
            catch (Exception ex)
            {
                exceptions.Add(ex);
            }

            foreach (var transaction in _transactions)
            {
                try
                {
                    transaction.Value.Rollback();
                }
                catch(Exception ex)
                {
                    exceptions.Add(ex);
                }
            }
            foreach (var rollbackAction in _rollbackActions.Union(_wrappedTransactions.SelectMany(t=> t.RollbackActions)))
            {
                try
                {
                    rollbackAction.DynamicInvoke();
                }
                catch(Exception ex)
                {
                    exceptions.Add(ex);
                }
            }
            if (exceptions.Any())
            {
                throw new AggregateException("A Rollback Action in the pool has failed!",exceptions);
            }
        }
        
        public void Dispose()
        {
            ((IPool)this).Rollback();
            Clean();
        }
        private void Clean()
        {             
            foreach (var command in _commands)
            {
                command.Dispose();
            }
            _rollbackActions.Clear();
            foreach (var transaction in _transactions)
            {
                transaction.Value.Dispose();
            }
            foreach (var transaction in _wrappedTransactions)
            {
                transaction.Dispose();
            }
            _transactions.Clear();
            _wrappedTransactions.Clear();
            Close();
            foreach (var databaseConnection in _connections)
            {
                databaseConnection.Value.Dispose();
            }      foreach (var databaseConnection in _connections)
            {
                databaseConnection.Value.Dispose();
            }   
        }
        public Database DefaultDbType => ContextController.DefaultContext.Type;
        
        public DatabaseConnection Connection()
        {
            if (_connections.Count != 0) return _connections.First().Value;
            var conn = _connectionSource.Connection();
            OnConnectionCreated(conn);
            return conn;
        }

        public DatabaseConnection Connection(string contextName)
        {
            DatabaseConnection conn;
            if (_connections.ContainsKey(contextName))
            {
                conn = _connections[contextName];
            }
            else
            {
                conn = _connectionSource.Connection(contextName);
                OnConnectionCreated(conn);
            }
            return conn;
        }

        DatabaseConnection IConnectionSource.Connection()
        {
            return Connection();
        }


        public void OnConnectionCreated(DatabaseConnection connection)
        {
            _connections.Add(connection.Context.Name, connection);
            var handler = ConnectionCreated;
            handler?.Invoke(connection);
        }

        public event ConnectionCreated ConnectionCreated;
        public void OnCommandCreated(Command command)
        {
            _commands.Add(command);
            var handler = CommandCreated;
            handler?.Invoke(command);
        }

        public event CommandCreated CommandCreated;

        public void OnTransactionCreated(Transaction transaction)
        {
            if (_transactions.ContainsKey(transaction.Connection.Context.Name))
            {
                transaction.DbTransaction = _transactions[transaction.Connection.Context.Name].DbTransaction;
                transaction.OnTransactionWrapped(transaction);
                return;
            }
            transaction.DbTransaction = transaction.Connection.BeginTransaction();
            ((IConnectionSource)transaction).CommandCreated += OnCommandCreated;
            transaction.Token = Token;
            _transactions.Add(transaction.Connection.Context.Name, transaction);
            var handler = TransactionCreated;
            handler?.Invoke(transaction);
        }

        public event TransactionCreated TransactionCreated;
        public void OnTransactionWrapped(Transaction transaction)
        {
            ((IConnectionSource)transaction).CommandCreated += OnCommandCreated;
            transaction.Token = Token;
            _wrappedTransactions.Add(transaction);
            var handler = TransactionWrapped;
            handler?.Invoke(transaction);
        }

        public event TransactionWrapped TransactionWrapped;

        public Transaction Transaction()
        {
            if (_transactions.ContainsKey(ContextController.DefaultContext.Name))
                return _transactions[ContextController.DefaultContext.Name];
            if(_connections.TryGetValue(ContextController.DefaultContext.Name,out _))
            {
                return new ManagedTransaction(this);
            }
            var t = new ManagedTransaction(_connectionSource ?? ContextController.DefaultContext) { Token = Token };
            _connections.Add(t.Connection.Context.Name,t.Connection);
            _transactions.Add(t.Connection.Context.Name,t);
            return t;
        }

        public Transaction Transaction(string contextName)
        {
            if (_transactions.ContainsKey(contextName))
                return _transactions[contextName];
            var t = new ManagedTransaction(this,contextName){ Token = Token };
            _transactions.Add(t.Connection.Context.Name, t);
            return t;
        }

        public CancellationToken Token => TokenSource.Token;

        internal CancellationTokenSource TokenSource = new CancellationTokenSource();
        public IPool AddRollBack(Action action)
        {
           _rollbackActions.Add(action);
            return this;
        }

        public IAddRollBack AddRollback(Action action)
        {
            _rollbackActions.Add(action);
            return this;
        }
    }
}
