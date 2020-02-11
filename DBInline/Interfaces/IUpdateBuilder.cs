namespace DBInline.Interfaces
{
   
    public interface IUpdateBuilder 
    {
        public IUpdateQuery Set<TParam>(string columnName, TParam value);
    }
    public interface IUpdateBuilder<T> 
    {
        public IUpdateQuery<T> Set<TParam>(string columnName,TParam value);
    }
}