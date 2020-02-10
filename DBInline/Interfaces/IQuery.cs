using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace DBInline.Interfaces
{

    public interface IQuery : IQueryCommon, ICommandBuilder<IQuery>, IWrapCommand
    {
        public ISelectBuilder<IQuery> Select(string [] fields = null);
        public IInsertBuilder<IQuery> InsertInto(string tableName);
        public IUpdateBuilder<IQuery> Update(string tableName);
        public IQuery Set(string text);
        public T Scalar<T>();
        public Task<T> ScalarAsync<T>();
    }
    
    public interface IQuery<T> : IQueryCommon, ICommandBuilder<IQuery<T>,T>, IWrapCommand
    {
        public ISelectBuilder<IQuery<T>> Select(string [] fields = null);
        public IInsertBuilder<IQuery<T>> Insert(string tableName);
        public IUpdateBuilder<IQuery<T>> Update(string tableName);
        public IQuery<T> Set(string text);
        public T Scalar();
        public Task<T> ScalarAsync();

    }

}

