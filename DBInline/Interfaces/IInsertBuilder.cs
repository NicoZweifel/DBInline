namespace DBInline.Interfaces
{
    public interface IInsertBuilder<out TBuilder> : IQuery where TBuilder : IQueryCommon
    {
        public IValueCollectionBuilder<TBuilder> Into(string[] columns);
    }
}