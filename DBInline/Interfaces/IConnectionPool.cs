using DBInline.Classes;

namespace DBInline.Interfaces
{
    public interface IConnectionPool
    {
        // ReSharper disable once UnusedMemberInSuper.Global
        public void Close();
        // ReSharper disable once UnusedMemberInSuper.Global
        public void OnConnectionCreated(DatabaseConnection connection);
        // ReSharper disable once EventNeverSubscribedTo.Global
        public event ConnectionCreated ConnectionCreated;
        // ReSharper disable once EventNeverSubscribedTo.Global
    }
    public delegate void ConnectionCreated(DatabaseConnection transaction);
 
}