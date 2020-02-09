using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using DBInline.Classes;

namespace DBInline.Interfaces
{


    public interface IQuery<T> : ICommandBuilder, ICommandBuilder<IQuery<T>>, IWrapCommand
    {
        public int Run();
        public Task<int> RunAsync();
        public T Scalar();
        public Task<T> ScalarAsync();
        public IEnumerable<TOut> Select<TOut>(Func<IDataReader, TOut> transform);
        public IAsyncEnumerable<TOut> SelectAsync<TOut>(Func<IDataReader, TOut> transform);
        public new IQuery<T> Set(string text);
        public IQueryBuilder Builder { get; }
        public DataTable Table();
        public Task<DataTable> TableAsync();
        public DataSet DataSet();
        public Task<DataSet> DataSetAsync();
        public DbDataReader Reader();
        public Task<DbDataReader> ReaderAsync();
    }

    public interface IQuery : ICommandBuilder, ICommandBuilder<IQuery>, IWrapCommand
    {
        public int Run();
        public Task<int> RunAsync();
        public Task<DataTable> TableAsync();
        public DataSet DataSet();
        public DataTable Table();
        public Task<DataSet> DataSetAsync();
        public DbDataReader Reader();
        public Task<DbDataReader> ReaderAsync();
        public new IQuery Set(string text);
        public T Scalar<T>();
        public Task<T> ScalarAsync<T>();
        public IEnumerable<TOut> Select<TOut>(Func<IDataReader, TOut> transform);
        public IAsyncEnumerable<TOut> SelectAsync<TOut>(Func<IDataReader, TOut> transform);
    }

    public interface IQueryBuilder : IQuery, IOrBuilder<IQuery>
    {
    }
    public interface IWrapCommand
    {
        internal Command Command { get; }
    }
}

