using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace DBInline.Interfaces
{

    public interface IQuery : IQueryCommon, ICommandBuilder<IQuery>, IWrapCommand
    {
        public IQuery Set(string text);
        public T Scalar<T>();
        public Task<T> ScalarAsync<T>();
    }
    
    public interface IQuery<T> : IQueryCommon, ICommandBuilder<IQuery<T>,T>, IWrapCommand
    {
        public new ISelectBuilder<IQuery<T>> Select(string [] fields = null);
        public new IInsertBuilder<IQuery<T>> InsertInto(string tableName);
        public new IUpdateBuilder<IQuery<T>,T> Update(string tableName);
        public IQuery<T> Set(string text);
        public T Scalar();
        public Task<T> ScalarAsync();

    }
}

