using System;
using System.Collections.Generic;
using System.Data;
using DBInline.Classes;

namespace DBInline.Interfaces
{
    public interface ICommandBuilder : IAddParameter, IAddRollBack, IWrapCommand
    {
        public IQuery Order(string clause);
        public IQuery Limit(int limit);
        public IQuery Param(IDbDataParameter parameter);
        public IQuery Param(SimpleParameter parameter);
        public IConditionBuilder Where(string clause);
        public IConditionBuilder Where(string fieldName, object value);
        public IConditionBuilder WhereNot(string fieldName, object value);
    }
    public interface ICommandBuilder<T> : ICommandBuilder
    {
        public new IConditionBuilder<T> Where(string clause);
        public new IConditionBuilder<T> Where(string fieldName, object value);
        public new IConditionBuilder<T> WhereNot(string fieldName, object value);
    }

}