namespace DBInline.Interfaces
{
   
    public interface IUpdateBuilder 
    {
        public IUpdateBuilder Set<TParam>(string columnName, TParam value);
    }
    public interface IUpdateBuilder<T> 
    {
        public IUpdateBuilder<T> Set<TParam>(string columnName,TParam value);
    }
    public interface IUpdateQuery : IUpdateBuilder, IQuery
    {
    
    }
    public interface IUpdateQuery<T> : IUpdateBuilder<T>, IQuery<T>
    {
    
    }
}