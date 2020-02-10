using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Input;
using DBInline.Classes;

namespace DBInline.Interfaces
{
    public interface ICommandBuilderCommon<out TBuilder> : IAddRollBack, IAddParameter where TBuilder : IQueryCommon
    {
        public new TBuilder Rollback(Action action);
        public new TBuilder Param(string name, object value);
        public TBuilder Param(IDbDataParameter parameter);
        public TBuilder Param(SimpleParameter parameter);
        public new TBuilder Parameters(IEnumerable<IDbDataParameter> paramArray);
        public TBuilder Order(string clause);
        public TBuilder Limit(int limit);
    }
}