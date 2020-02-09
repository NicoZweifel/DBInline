namespace DBInline.Interfaces
{
    public interface ICommandBuilder<out TBuilder, TOut> : ICommandBuilderCommon<TBuilder> where TBuilder : IQueryCommon
    { 
        public IQueryBuilder<TBuilder, TOut> Where(string clause);
        public IQueryBuilder<TBuilder, TOut> Where(string fieldName, object value);
    }

    public interface ICommandBuilder<out TBuilder> : ICommandBuilderCommon<TBuilder> where TBuilder : IQueryCommon
    {
        public IQueryBuilder<TBuilder> Where(string clause);
        public IQueryBuilder<TBuilder> Where(string fieldName, object value);
    }
}