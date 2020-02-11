using System.Threading.Tasks;

namespace DBInline.Interfaces
{
    public interface IInsertQuery : IRowBuilder,ICommand
    {
        public int Run();
        public Task<int> RunAsync();
    }

    public interface IInsertQuery<T> : IRowBuilder<T>, ICommand<T>
    {
        public int Run();
        public Task<int> RunAsync();
    }
}