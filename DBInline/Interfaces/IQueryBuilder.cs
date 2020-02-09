namespace DBInline.Interfaces
{
    public interface IQueryBuilder<out TBuilder> : IQuery where TBuilder : IQueryCommon
    {
        public IQueryBuilder<TBuilder> Or(string clause);
        public IQueryBuilder<TBuilder> Or(string fieldName, object value);
    }

    public interface IQueryBuilder<out TBuilder, TOut> : IQuery<TOut> where TBuilder : IQueryCommon
    {
        public IQueryBuilder<TBuilder, TOut> Or(string clause);
        public IQueryBuilder<TBuilder, TOut> Or(string fieldName, object value);
    }
}