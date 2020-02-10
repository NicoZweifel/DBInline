namespace DBInline.Interfaces
{
    public interface IConditionBuilder : IQuery
    {
        public IConditionBuilder Or(string clause);
        public IConditionBuilder Or(string fieldName, object value);
    }

    public interface IConditionBuilder<T> : IQuery<T> 
    {
        public IConditionBuilder<T> Or(string clause);
        public IConditionBuilder<T> Or(string fieldName, object value);
    }
}