using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Policy;
using System.Windows.Input;
using DBInline.Classes;
using DBInline.Classes.Transactions;

namespace DBInline.Interfaces
{
    public interface ICommandBehaviour<out T> : IAddRollBack, IAddParameter where T : ICommandBehaviour<T>
    {
        public T Set(string text);
        public new T AddRollback(Action action);
        public new T Param(string name, object value);
        public  T Param(IDbDataParameter parameter);
        public  T Param(SimpleParameter parameter);
        public new T AddParameters(IEnumerable<IDbDataParameter> paramArray);
    }
    public interface ICommandBehaviour : IAddRollBack, IAddParameter
    {
    }
    
    
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