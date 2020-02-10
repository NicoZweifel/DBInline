using System.Threading.Tasks;

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
    public interface IUpdateQuery : IUpdateBuilder , ICommand, ICommandBuilder
    {
        public int Run();
        public Task<int> RunAsync();
        
    }
    public interface IUpdateQuery<T> : IUpdateBuilder<T> , ICommand<T>,ICommandBuilder<T>
    {
        public int Run();
        public Task<int> RunAsync();
    }
}