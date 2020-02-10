namespace DBInline.Interfaces
{
    public interface IValueCollectionBuilder<out TBuilder> : IQuery where TBuilder : IQueryCommon
    {
        public IFieldCollectionBuilder<TBuilder> Into(string[] fields);
    }
    
    public interface IFieldCollectionBuilder<out TBuilder> : IQuery where TBuilder : IQueryCommon
    {
        public IFieldCollectionBuilder<TBuilder> Into(string[] fields);
    }
}
