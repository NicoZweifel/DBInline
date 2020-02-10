using DBInline.Interfaces;

// ReSharper disable once CheckNamespace
namespace DBInline.Classes.Transactions
{
    internal class ManagedTransaction : Transaction
    {
        public ManagedTransaction(Pool pool) : base(pool.Connection())
        {
            TransactionCreated += pool.OnTransactionCreated;
            CommandCreated += pool.OnCommandCreated;
            ConnectionSource = pool;
            OnTransactionCreated(this);
        }
        public ManagedTransaction(Pool pool, string dataBase) : base(pool.Connection(dataBase))
        {
            TransactionCreated += pool.OnTransactionCreated;
            CommandCreated += pool.OnCommandCreated;
            ConnectionSource = pool;
        }

        internal ManagedTransaction(IConnectionSource connection) : base(connection)
        {
            TransactionCreated += connection.OnTransactionCreated;
            CommandCreated += connection.OnCommandCreated;
            ConnectionSource = connection;
        }

        internal ManagedTransaction(IConnectionSource connection, string databaseName) : base(connection, databaseName)
        {
            TransactionCreated += connection.OnTransactionCreated;
            CommandCreated += connection.OnCommandCreated;
            ConnectionSource = connection;
        }
    }
}
