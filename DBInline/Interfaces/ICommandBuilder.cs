namespace DBInline.Interfaces
{
    public interface ICommandBuilder<out TBuilder> : ICommandBuilderCommon<TBuilder> where TBuilder : IQueryCommon
    { 
        public IConditionBuilder<TBuilder> Where(string clause);
        public IConditionBuilder<TBuilder> Where(string fieldName, object value);
    }
}