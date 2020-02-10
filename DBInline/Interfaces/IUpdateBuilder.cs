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
    public interface IUpdateQuery : IUpdateBuilder
    {
        public ICommand Run();
    }
    public interface IUpdateQuery<T> : IUpdateBuilder<T>
    {
       public ICommand<T> Run();
    }
}