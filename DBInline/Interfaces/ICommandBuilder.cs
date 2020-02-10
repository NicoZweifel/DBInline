using System;
using System.Collections.Generic;
using System.Data;
using DBInline.Classes;

namespace DBInline.Interfaces
{
    public interface ICommandBuilder : IAddParameter, IAddRollBack, IWrapCommand
    {
        public IConditionBuilder Where(string clause);
        public IConditionBuilder Where(string fieldName, object value);
        public IQuery Param(IDbDataParameter parameter);
        public IQuery Param(SimpleParameter parameter);
        public IQuery Order(string clause);
        public IQuery Limit(int limit);
    }
    public interface ICommandBuilder<T> : ICommandBuilder
    {
        public new IConditionBuilder<T> Where(string clause);
        public new IConditionBuilder<T> Where(string fieldName, object value);
    }

}