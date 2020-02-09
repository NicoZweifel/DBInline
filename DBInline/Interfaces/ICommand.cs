namespace DBInline.Interfaces
{
    public interface ICommand<out TBuilder, TOut> : ICommandCommon<TBuilder> where TBuilder : IQueryCommon
    { 
        public IQueryBuilder<TBuilder, TOut> Where(string clause);
        public IQueryBuilder<TBuilder, TOut> Where(string fieldName, object value);
    }

    public interface ICommand<out TBuilder> : ICommandCommon<TBuilder> where TBuilder : IQueryCommon
    {
        public IQueryBuilder<TBuilder> Where(string clause);
        public IQueryBuilder<TBuilder> Where(string fieldName, object value);
    }
}