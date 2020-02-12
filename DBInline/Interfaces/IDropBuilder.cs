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
}