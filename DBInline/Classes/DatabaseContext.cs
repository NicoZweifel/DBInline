using System.Collections.Generic;
using DBInline.Interfaces;

namespace DBInline.Classes
{
    public class DatabaseContext : IConnectionSource
    {
        public readonly string Name;
        private readonly string _connectionString;

        // ReSharper disable once MemberCanBePrivate.Global
        public readonly Database Type;
        // ReSharper disable once CollectionNeverQueried.Local
        private readonly List<Command> _commands = new List<Command>();

        public DatabaseContext(string name, string connectionString, Database type)
        {
            Name = name;
            _connectionString = connectionString;
            Type = type;
        }

        public Database DefaultDbType => Type;

        public DatabaseConnection Connection()
        {
            var conn = new DatabaseConnection(Type, _connectionString, this);
            return conn;
        }

        public DatabaseConnection Connection(string contextName)
        {
            var conn = ContextController.ContextByName[contextName].Connection();
            return conn;
        }

        public void OnTransactionCreated(Transaction transaction)
        {
            ((IConnectionSource) transaction).CommandCreated += OnCommandCreated;
            var handler = TransactionCreated;
            handler?.Invoke(transaction);
        }

        public event TransactionCreated TransactionCreated;

        public void OnTransactionWrapped(Transaction transaction)
        {
            ((IConnectionSource) transaction).CommandCreated += OnCommandCreated;
            var handler = TransactionWrapped;
            handler?.Invoke(transaction);
        }

        public event TransactionWrapped TransactionWrapped;

        public void OnCommandCreated(Command command)
        {
            _commands.Add(command);
            var handler = CommandCreated;
            handler?.Invoke(command);
        }

        public event CommandCreated CommandCreated;

        public Transaction Transaction()
        {
            return ContextController.DefaultContext.Transaction();
        }

        public Transaction Transaction(string databaseName)
        {
            return ContextController.ContextByName[databaseName].Transaction();
        }

        public void Dispose()
        {

        }
    }
}
