using System.Threading.Tasks;

namespace DBInline.Interfaces
{
    public interface IDeleteQuery : ICommandBuilder,ICommand
    { 
        public int Run();
        public Task<int> RunAsync();
    }

    public interface IDeleteQuery<T> : ICommandBuilder<T>, ICommand<T>
    {
        public int Run();
        public Task<int> RunAsync();
    }
}