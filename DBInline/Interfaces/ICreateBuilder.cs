using System.Data;

namespace DBInline.Interfaces
{
    public interface ICreateBuilder
    { 
        public ICreateQuery Add(string column, SqlDbType type ,int charCount =0);
    }

    public interface ICreateBuilder<T>
    {
        public ICreateQuery<T> Add(string column, SqlDbType type, int charCount = 0);
    }
}