using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace DBInline.Interfaces
{
    public interface IQueryCommon
    {
        public ISelectBuilder<IQuery> Select(string [] fields = null);
        public IInsertBuilder<IQuery> InsertInto(string tableName);
        public IUpdateBuilder<IQuery> Update(string tableName);
        public int Run();
        public Task<int> RunAsync();
        public ICommandBuilderCommon<IQueryCommon> BuilderCommon { get; }
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
}