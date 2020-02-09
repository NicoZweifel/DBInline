using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Input;
using DBInline.Classes;

namespace DBInline.Interfaces
{
    public interface ICommandBuilder<out T> : IAddRollBack, IAddParameter where T : ICommandBuilder<T>
    {
        public T Set(string text);
        public new T Rollback(Action action);
        public new T Param(string name, object value);
        public  T Param(IDbDataParameter parameter);
        public  T Param(SimpleParameter parameter);
        public new T Parameters(IEnumerable<IDbDataParameter> paramArray);
        public T Where(string clause);
        public IOrBuilder<T> Where(string fieldName, object value);
        public T Order(string clause);
        public T Limit(int limit);
    }

    public interface IOrBuilder<out T> : ICommandBuilder<T>  where T : ICommandBuilder<T>
    {
        public T Or(string clause);
        public T Or(string fieldName, object value);
    }
    
    public interface ICommandBuilder : IAddRollBack, IAddParameter
    {
    }
}