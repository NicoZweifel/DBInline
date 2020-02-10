namespace DBInline.Interfaces
{
    public interface IInsertFromBuilder
    {
        public ITableBuilder Select(string[] fields);
        
    }
    public interface IInsertFromBuilder<T>
    {
        public ITableBuilder<T> Select(string[] fields);
        
    }
}