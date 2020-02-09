using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace DBInline.Interfaces
{
    public interface IQueryCommon
    {
        public int Run();
        public Task<int> RunAsync();
        public ICommandCommon<IQueryCommon> Common { get; }
        public DataTable Table();
        public Task<DataTable> TableAsync();
        public DataSet DataSet();
        public Task<DataSet> DataSetAsync();
        public DbDataReader Reader();
        public Task<DbDataReader> ReaderAsync();
    }
}