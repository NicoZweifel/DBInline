using System;
using System.Collections.Generic;
using System.Data;
using DBInline.Classes;

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
        public T Where(string whereClause);
        public T Order(string orderClause);
    }
    public interface ICommandBehaviour : IAddRollBack, IAddParameter
    {
    }
}