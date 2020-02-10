using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace DBInline.Interfaces
{

    public interface IQuery : IWrapCommand
    {
        public T Scalar<T>();
        public Task<T> ScalarAsync<T>();
        public int Run();
        public Task<int> RunAsync();
        public DataTable Table();
        public Task<DataTable> TableAsync();
        public DataSet DataSet();
        public Task<DataSet> DataSetAsync();
        public DbDataReader Reader();
        public Task<DbDataReader> ReaderAsync();
        public IEnumerable<TOut> Get<TOut>(Func<IDataReader, TOut> transform);
        public Task<List<TOut>> GetAsync<TOut>(Func<IDataReader, TOut> transform);
        public IAsyncEnumerable<TOut> GetAsyncEnumerable<TOut>(Func<IDataReader, TOut> transform);
    }
    
    public interface IQuery<T> : IQuery
    {
        public T Scalar();
        public Task<T> ScalarAsync();
        public IEnumerable<T> Get(Func<IDataReader, T> transform);
        public Task<List<T>> GetAsync(Func<IDataReader, T> transform);
        public IAsyncEnumerable<T> GetAsyncEnumerable(Func<IDataReader, T> transform);
    }
}

