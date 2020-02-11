using System.Threading.Tasks;

namespace DBInline.Interfaces
{
    public interface IUpdateQuery : IUpdateBuilder, ICommand, ICommandBuilder
    {
        public int Run();
        public Task<int> RunAsync();
    }

    public interface IUpdateQuery<T> : IUpdateBuilder<T>, ICommand<T>, ICommandBuilder<T>
    {
        public int Run();
        public Task<int> RunAsync();
    }
}
