using System.Threading.Tasks;

namespace DBInline.Interfaces
{
    public interface IConditionBuilder : ICommand
    {
        public int Run();
        public Task<int> RunAsync();
        public IConditionBuilder Or(string clause);
        public IConditionBuilder Or(string fieldName, object value);
    }

    public interface IConditionBuilder<T> : ICommand<T>
    {
        public int Run();
        public Task<int> RunAsync();
        public IConditionBuilder<T> Or(string clause);
        public IConditionBuilder<T> Or(string fieldName, object value);
    }
}