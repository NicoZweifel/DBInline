using System.Transactions;

namespace DBInline.Interfaces
{
    public interface ISelectBuilder
    {
        public IQuery From(string tableName);
        public ISelectBuilder Add(string columnName);
    }
    public interface ISelectBuilder<T>
    {
        public IQuery<T> From(string tableName);
        public ISelectBuilder<T> Add(string columnName);
    }
}