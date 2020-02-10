namespace DBInline.Interfaces
{
    public interface IUpdateBuilder<out TBuilder> : IQuery where TBuilder : IQueryCommon
    {
        public IUpdateBuilder<TBuilder> Add<T>(string columnName, T value);
        
    }
}