using System.Threading.Tasks;

namespace DBInline.Interfaces
{
    public interface IInsertQuery : IRowBuilder,ICommand
    {
        public int Run();
        public Task<int> RunAsync();
    }

    public interface IInsertQuery<T> : IRowBuilder<T>,ICommand<T>
    {
        public int Run();
        public Task<int> RunAsync();
    }
    
    public interface IValuesBuilder : ICommand
    { 
        public IRowBuilder Row();
    }

    public interface IValuesBuilder<T> : ICommand<T>
    {
        public IRowBuilder<T> Row();
    }

    public interface IRowBuilder
    {
         public IRowBuilder Row();
         public IInsertQuery Add<TIn>(TIn value);
    }
    
    public interface IRowBuilder<T>
    {
        public IRowBuilder<T> Row();
        
        public IRowBuilder<T>  Add<TIn>(TIn value);
    }
}