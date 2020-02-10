namespace DBInline.Interfaces
{
    public interface IConditionBuilder<out TBuilder> where TBuilder : IQueryCommon
    {
        public TBuilder Or(string clause);
        public TBuilder Or(string fieldName, object value);
    }
}