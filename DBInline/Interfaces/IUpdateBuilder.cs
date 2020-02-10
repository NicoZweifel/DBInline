namespace DBInline.Interfaces
{
    public interface IUpdateBuilder<out TBuilder,TOut> : IQuery<TOut> where TBuilder : IQueryCommon
    {
        public IUpdateBuilder<TBuilder,TOut> Set<T>(string columnName,T value);
    }

    public interface IUpdateBuilder<out TBuilder> : IQuery where TBuilder : IQueryCommon
    {
        public IUpdateBuilder<TBuilder> Set<T>(string columnName, T value);
    }
}