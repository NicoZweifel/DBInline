using System;
using System.Threading;
using System.Linq;
using DBInline.Classes.Transactions;

namespace DBInline.Interfaces
{
    public interface IPool : IConnectionSource, IAddRollBack , IConnectionPool
    {
        public void Commit();
        // ReSharper disable once UnusedMemberInSuper.Global
        internal void Rollback();
        public void Cancel();
        // ReSharper disable once UnusedMemberInSuper.Global
        public IPool AddRollBack(Action a);
        
        public CancellationToken Token { get; }
    }
}