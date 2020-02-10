using System.Windows.Input;

namespace DBInline.Interfaces
{
    public interface IValueCollectionBuilder<out TBuilder> where TBuilder : IQueryCommon
    {
        public ICommandBuilder<TBuilder> Values(string[] fields);

        public ITableBuilder<TBuilder> Select(string[] fields);
    }
    
    public interface ITableBuilder<out TBuilder> where TBuilder : IQueryCommon
    {
        public ICommandBuilder<TBuilder> From(string tableName);
        
    }
}
