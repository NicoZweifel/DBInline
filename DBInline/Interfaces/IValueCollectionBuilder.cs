using System.Windows.Input;

namespace DBInline.Interfaces
{
    public interface IValueCollectionBuilder
    {
        public ICommandBuilder Values(string[] fields);

        public ITableBuilder Select(string[] fields);
    }
    public interface IValueCollectionBuilder<T>
    {
        public int ExecuteNonQuery();

        public ITableBuilder<T> Select(string[] fields);
    }
    public interface ITableBuilder
    {
        public ICommandBuilder From(string tableName);
    }
    public interface ITableBuilder<T>
    {
        public ICommandBuilder<T> From(string tableName);
        
    }
}
