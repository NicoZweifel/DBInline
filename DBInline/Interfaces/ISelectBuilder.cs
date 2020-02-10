using System.Transactions;

namespace DBInline.Interfaces
{
    public interface ISelectBuilder<out TBuilder> : IQuery where TBuilder : IQueryCommon
    {
        public TBuilder From(string tableName);
        public ISelectBuilder<TBuilder> Add(string fieldName);
    }
}