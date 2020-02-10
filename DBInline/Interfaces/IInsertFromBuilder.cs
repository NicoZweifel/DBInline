namespace DBInline.Interfaces
{
    public interface IInsertFromBuilder
    {
        public IInsertFromQuery Select(string[] fields);
        
    }
    public interface IInsertFromBuilder<T>
    {
        public IColumnsFromQuery<T> Select(string[] columns);
        
    }
}