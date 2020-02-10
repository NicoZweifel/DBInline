using System.Transactions;

namespace DBInline.Interfaces
{
    public interface ISelectBuilder
    {
        public IQuery From(string tableName);
        public ISelectBuilder Add(string columnName);
        public ISelectBuilder Add(string[] columnNames);
    }

    public interface ISelectBuilder<T>
    {
        public IQuery<T> From(string tableName);
        public ISelectBuilder<T> Add(string columnName);
        public ISelectBuilder<T> Add(string[] columnNames);
    }

    public interface IInsertBuilder
    {
        public IQuery From(string tableName);
        public IInsertBuilder Add(string columnName);
        public IInsertBuilder Add(string[] columnNames);
        public IValuesBuilder Values();
    }

    public interface IInsertBuilder<T> 
    {
        public IQuery<T> From(string tableName);
        public IInsertBuilder<T> Add(string columnName);
        public IInsertBuilder<T> Add(string[] columnNames);
        public IValuesBuilder<T> Values();
    }
}