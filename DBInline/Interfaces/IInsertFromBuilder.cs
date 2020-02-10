namespace DBInline.Interfaces
{
    public interface IInsertFromBuilder<out TBuilder> :  IQuery where TBuilder : IQueryCommon
    {
        public IInsertFromBuilder<TBuilder> From(string tableName);
        
    }
}