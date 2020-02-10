namespace DBInline.Interfaces
{
    public interface ICommandBuilder<out TBuilder, TOut> : ICommandBuilderCommon<TBuilder> where TBuilder : IQueryCommon
    { 
        public IConditionBuilder<TBuilder, TOut> Where(string clause);
        public IConditionBuilder<TBuilder, TOut> Where(string fieldName, object value);
    }

    public interface ICommandBuilder<out TBuilder> : ICommandBuilderCommon<TBuilder> where TBuilder : IQueryCommon
    {
        public IConditionBuilder<TBuilder> Where(string clause);
        public IConditionBuilder<TBuilder> Where(string fieldName, object value);
    }
}