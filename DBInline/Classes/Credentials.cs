using System;
using System.Linq;

namespace DBInline.Classes
{
    public class Credentials
    {
        // ReSharper disable once MemberCanBeProtected.Global
        public Credentials()
        {
            Username = "";
            Password = "";
        }

        // ReSharper disable once MemberCanBeProtected.Global
        public Credentials(string username, string password)
        {
            this.Username = username;
            this.Password = password;
        }

        // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
        // ReSharper disable once MemberCanBePrivate.Global
        // ReSharper disable once MemberCanBeProtected.Global
        public string Username { get; set; }
        // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
        // ReSharper disable once MemberCanBePrivate.Global
        // ReSharper disable once MemberCanBeProtected.Global
        public string Password { get; set; }
    }

    // ReSharper disable once ClassNeverInstantiated.Global
    public class DatabaseCredentials : Credentials
    {
        public DatabaseCredentials()
        {
            Username = "";
            Password = "";
        }

        public DatabaseCredentials(string dataBase, string server, Database type, int port)
        {
            Username = "";
            Password = "";
            DataBase = dataBase;
            Server = server;
            Type = type;
            Port = port;
        }

        public DatabaseCredentials(string username, string password, string dataBase, string server, Database type,
            int port) : base(username, password)
        {
            DataBase = dataBase;
            Server = server;
            Type = type;
            Port = port;
        }

        // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
        // ReSharper disable once MemberCanBePrivate.Global
        public string DataBase { get; set; }

        // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
        // ReSharper disable once MemberCanBePrivate.Global
        public string Server { get; set; }

        // ReSharper disable once MemberCanBePrivate.Global
        // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
        public Database Type { get; set; }

        // ReSharper disable once MemberCanBePrivate.Global
        // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
        public int Port { get; set; }

        public string GetConnectionString()
        {
            return Type switch
            {
                Database.Mssql =>
                $"Server={Server} ; {(Username.Any() ? "user id=" + Username + ";" : "")} {(Password.Any() ? "Password=" + Password + ";" : "")} Database={DataBase}; {(!Username.Any() ? "Trusted_Connection=True" : "")}; Enlist=true",
                Database.Postgres =>
                $"Server={Server} ; Port={(Port == 0 ? "" : Port.ToString())}; {(Username.Any() ? "user id=" + Username + ";" : "")} {(Password.Any() ? "Password=" + Password + ";" : "")} Database={DataBase}; {(!Username.Any() ? "Trusted_Connection=True" : "")}; Enlist=true",
                Database.Mysql => "",
                Database.SqlLite => "",
                _ => throw new NotImplementedException("Database not recognized.")
            };
        }
    }
}