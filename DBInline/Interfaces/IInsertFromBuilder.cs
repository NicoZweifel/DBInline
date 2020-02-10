namespace DBInline.Interfaces
{
    public interface IInsertFromBuilder<out TBuilder> where TBuilder : IQueryCommon
    {
        public ITableBuilder<TBuilder> Select(string[] fields);
        
    }
}