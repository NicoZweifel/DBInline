namespace DBInline.Interfaces
{
    public interface IInsertBuilder 
    {
        public IValueCollectionBuilder Into(string[] columns);
    }
    
    public interface IInsertBuilder<T> 
    {
        public IValueCollectionBuilder<T> Into(string[] columns);
    }
}