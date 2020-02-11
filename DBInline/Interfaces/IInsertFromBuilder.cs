namespace DBInline.Interfaces
{
    public interface IInsertFromBuilder
    {
        public IInsertFromQuery Select(params string[] fields);
        
    }
    public interface IInsertFromBuilder<T>
    {
        public IInsertFromQuery<T> Select(params string[] columns);
        
    }
}