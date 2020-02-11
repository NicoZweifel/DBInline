using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using DBInline.Classes;
using NUnit.Framework.Internal.Execution;


namespace DBInline.Test
{
    public class TestBase
    {
        /// <summary>
        /// Place credentials in Environment.CurrentDirectory + "credentials\\data.json")
        /// </summary>
        [SetUp]
        public void Setup()
        {
            if (ContextController.Connected) return;
            var path = Path.Combine(Environment.CurrentDirectory, "credentials\\data.json");
            if (!File.Exists(path))
            {
                File.WriteAllText(path,
                    JsonSerializer.Serialize(new List<DatabaseCredentials> {new DatabaseCredentials()}));
                throw new Exception($"No Database Credentials found, file has been created at {path}");
            }
            var credentials = JsonSerializer.Deserialize<DatabaseCredentials[]>(
                File.ReadAllText(Path.Combine(Environment.CurrentDirectory, "credentials\\data.json")));
            //TODO Move names to json
            ContextController.AddContext("postgres", credentials.First().Type, credentials.First().GetConnectionString(),true);
            ContextController.AddContext("MsSql", credentials.Last().Type, credentials.Last().GetConnectionString());
        }


        protected const string TableName = "dbinline_generated_table";
        
        /// <summary>
        /// Creates Table for tests.
        /// </summary>
        protected const string CreateQuery = "CREATE TABLE \"dbinline_generated_table\" (dbid int,name varchar(50));";

        /// <summary>
        /// Inserts some sample data.
        /// </summary>
        protected const string InsertQuery =
            "INSERT INTO dbinline_generated_table (dbid,name) VALUES (1,'John Doe'),(2,'James Smith'),(3,'Jack Williams'),(4,'Peter Brown'),(5,'Hans Mueller');";

        /// <summary>
        /// Select Query
        /// </summary>
        protected const string SelectQuery = "SELECT * FROM \"dbinline_generated_table\";";
        
                
        /// <summary>
        /// Deletes content in Table.
        /// </summary>
        protected const string DeleteQuery = "DELETE FROM \"dbinline_generated_table\";";
        
        /// <summary>
        /// Drops test Table.
        /// </summary>
        protected const string DropQuery = "DROP TABLE IF EXISTS \"dbinline_generated_table\";";
        
    }
}
