using System.Threading.Tasks;

namespace DBInline.Interfaces
{
    public interface IDropQuery : ICommand
    {
        public int Run();

        public Task<int> RunAsync();
    }

    public interface IDropQuery<T> : ICommand<T>
    {
        public int Run();

        public Task<int> RunAsync();
    }
}
