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
        public ITableBuilder<T> Select(string[] fields);
    }
    public interface ITableBuilder
    {
        public int Run();
        public ICommandBuilder From(string tableName);
    }
    public interface ITableBuilder<T> 
    {
        public int Run();
        public ICommandBuilder<T> From(string tableName);
    }
}
