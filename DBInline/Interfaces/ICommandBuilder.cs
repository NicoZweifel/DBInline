using System;
using System.Collections.Generic;
using System.Data;
using DBInline.Classes;

namespace DBInline.Interfaces
{
    public interface ICommandBuilder<out T> : IAddRollBack, IAddParameter where T : ICommandBuilder<T>
    {
        public T Set(string text);
        public new T AddRollback(Action action);
        public new T Param(string name, object value);
        public  T Param(IDbDataParameter parameter);
        public  T Param(SimpleParameter parameter);
        public new T AddParameters(IEnumerable<IDbDataParameter> paramArray);
        public T Where(string clause);
        public T Where(string fieldName, object value);
        public T Order(string clause);
        public T Limit(int limit);
    }

    public interface IClauseBuilder<out T> : ICommandBuilder<T>  where T : IClauseBuilder<T>
    {
        public T Or(string clause);
        public T Or(string fieldName, object value);
    }
    
    
    public interface ICommandBuilder : IAddRollBack, IAddParameter
    {
    }
}