namespace DBInline.Interfaces
{
    public interface IConditionQuery : IQuery
    {
        public IConditionQuery Or(string clause);
        public IConditionQuery Or(string fieldName, object value);
    }
    
    public interface IConditionQuery<T> : IQuery<T>
    {
        public IConditionQuery Or(string clause);
        public IConditionQuery Or(string fieldName, object value);
    }
}