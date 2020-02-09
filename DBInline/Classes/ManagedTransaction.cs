using DBInline.Interfaces;

// ReSharper disable once CheckNamespace
namespace DBInline.Classes.Transactions
{
    internal class ManagedTransaction : Transaction
    {
        private readonly Pool _pool;
        public ManagedTransaction(Pool pool) : base(pool.Connection())
        {
            ConnectionSource = this;
            _pool = pool;
        }
        public ManagedTransaction(Pool pool, string dataBase) : base(pool.Connection(dataBase))
        {
            ConnectionSource = this;
            _pool = pool;
        }

        internal ManagedTransaction(IConnectionSource connection) : base(connection)
        {
        }

        internal ManagedTransaction(IConnectionSource connection, string databaseName) : base(connection, databaseName)
        {
        }

        internal Pool Pool => _pool;
    }
}
