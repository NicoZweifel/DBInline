using System.Threading.Tasks;

namespace DBInline.Interfaces
{
    public interface ICreateQuery : ICreateBuilder, ICommand
    {
        public int Run();
        public Task<int> RunAsync();
    }

    public interface ICreateQuery<T> : ICreateBuilder, ICommand<T>
    {
        public int Run();
        public Task<int> RunAsync();
    }
}