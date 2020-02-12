using System;
using System.Collections.Generic;
using System.Data;

namespace DBInline.Interfaces
{
    public interface  IAddRollBack
    {
        public IAddRollBack Rollback(Action action);
    }
    public delegate void RollbackAdded(Action a);
    public interface  IAddParameter 
    {
        public  IAddParameter Param(string name, object value);
        public IAddParameter Params(IEnumerable<IDbDataParameter> paramArray);
    }
}