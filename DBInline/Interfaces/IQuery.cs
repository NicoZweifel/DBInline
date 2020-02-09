using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace DBInline.Interfaces
{

    public interface IQuery : IQueryCommon, ICommandBuilderCommon<IQuery>, IWrapCommand
    {
        public IQuery Set(string text);
        public T Scalar<T>();
        public Task<T> ScalarAsync<T>();
        public IEnumerable<T> Select<T>(Func<IDataReader, T> transform);
        public IAsyncEnumerable<T> SelectAsync<T>(Func<IDataReader, T> transform);
    }
    
    public interface IQuery<T> : IQueryCommon, ICommandBuilderCommon<IQuery<T>>, IWrapCommand
    {
        public IQuery<T> Set(string text);
        public T Scalar();
        public Task<T> ScalarAsync();
        public IEnumerable<TOut> Select<TOut>(Func<IDataReader, TOut> transform);
        public IAsyncEnumerable<TOut> SelectAsync<TOut>(Func<IDataReader, TOut> transform);
    }

}

