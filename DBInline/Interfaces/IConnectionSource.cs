using System;
using DBInline.Classes;

namespace DBInline.Interfaces
{
    public interface IConnectionSource : IDisposable
    {
        public Database DefaultDbType { get; }
        public DatabaseConnection Connection();
        public DatabaseConnection Connection(string contextName);
        // ReSharper disable once UnusedMemberInSuper.Global
        public void OnTransactionCreated(Transaction transaction);
        // ReSharper disable once EventNeverSubscribedTo.Global
        public event TransactionCreated TransactionCreated;
        // ReSharper disable once UnusedMemberInSuper.Global
        public void OnTransactionWrapped(Transaction transaction);
        // ReSharper disable once EventNeverSubscribedTo.Global
        public event TransactionWrapped TransactionWrapped;
        public void OnCommandCreated(Command command);
        public event CommandCreated CommandCreated;
        public Transaction Transaction();
        // ReSharper disable once UnusedMemberInSuper.Global
        public Transaction Transaction(string databaseName);
    }
    public delegate void TransactionCreated(Transaction transaction);
    public delegate void TransactionWrapped(Transaction transaction);
    public delegate void CommandCreated(Command command);
}