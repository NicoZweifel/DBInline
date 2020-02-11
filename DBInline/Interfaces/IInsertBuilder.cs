namespace DBInline.Interfaces
{
    public interface IInsertBuilder
    {
        public IQuery From(string tableName);
        public IInsertBuilder Add(params string[] columnNames);
        public IValuesBuilder Values();
    }

    public interface IInsertBuilder<T>
    {
        public IQuery<T> From(string tableName);
        public IInsertBuilder<T> Add(params string[] columnNames);
        public IValuesBuilder<T> Values();
    }
}