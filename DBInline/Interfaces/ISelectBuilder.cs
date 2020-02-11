using System.Transactions;

namespace DBInline.Interfaces
{
    public interface ISelectBuilder
    {
        public IQuery From(string tableName);
        public ISelectBuilder Add(params string[] columnNames);
    }

    public interface ISelectBuilder<T>
    {
        public IQuery<T> From(string tableName);
        public ISelectBuilder Add(params string[] columnNames);
    }
}