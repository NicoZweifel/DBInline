using System.Security.Cryptography.X509Certificates;
using MySqlX.XDevAPI.Relational;

namespace DBInline.Interfaces
{
    public interface IDropBuilder :IDropQuery
    {
        public IDropQuery IfExists();
    }
    public interface IDropBuilder<T> :IDropQuery<T>
    {
        public IDropQuery<T> IfExists();
    }
    public interface IDropQuery : ICommand
    { 
        public int Run();
    }
    public interface IDropQuery<T> : ICommand<T>
    {
        public int Run();
    }
}