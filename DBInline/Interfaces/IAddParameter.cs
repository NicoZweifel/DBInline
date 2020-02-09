using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Policy;
using System.Windows.Input;
using DBInline.Classes.Transactions;

namespace DBInline.Interfaces
{
    public interface  IAddRollBack
    {
        public IAddRollBack AddRollback(Action action);
    }
    public delegate void RollbackAdded(Action a);
    public interface  IAddParameter 
    {
        public  IAddParameter Param(string name, object value);
        public IAddParameter AddParameters(IEnumerable<IDbDataParameter> paramArray);
    }
}