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
        public IConditionQuery Where(string clause);
        public IConditionQuery Where(string fieldName, object value);
        public IConditionQuery WhereNot(string fieldName, object value);
    }
    public interface ICommandBuilder<T> : ICommandBuilder
    {
        public new IConditionQuery<T> Where(string clause);
        public new IConditionQuery<T> Where(string fieldName, object value);
        public new IConditionQuery<T> WhereNot(string fieldName, object value);
    }

}