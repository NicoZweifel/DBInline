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
        /// Place credentials in Environment.CurrentDirectory + "creds\\data.json")
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
            //Replace
            ContextController.AddContext("postgres", credentials.First().Type, credentials.First().GetConnectionString(), true);
            ContextController.AddContext("MsSql", credentials.Last().Type, credentials.Last().GetConnectionString());
            
        }

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
        /// Drops test Table.
        /// </summary>
        protected const string DropQuery = "DROP TABLE IF EXISTS \"dbinline_generated_table\";";
        
        
        /// <summary>
        ///Simple Select Test query, First Result should be string.
        /// Tests pass if Any string Results return.
        /// </summary>
        protected const string ExampleQuery3 = "SELECT * FROM details;";

        /// <summary>
        /// Queries that get roll backed.
        /// </summary>
        protected const string RollbackQuery = "DELETE FROM test WHERE name = @name;";
        protected const string RollbackQuery2 = "DELETE FROM  [BigDota].[dbo].[MatchDetails] WHERE MatchId = @id;";

        /// <summary>
        /// Simple Select Test query, First Result should be string. Tests check for "86"
        /// </summary>
        protected const string ExampleQuery1 = "INSERT INTO test(name)VALUES('86'); SELECT name FROM test WHERE name = \'86\' limit 1;";
        protected readonly SimpleParameter Param1 =new SimpleParameter("@name", "86");

        /// <summary>
        /// Test query.
        /// For testing run on separate Database-Server Tests check for long: 5155081264.
        /// </summary>
        protected const string ExampleQuery2 = "INSERT INTO [BigDota].[dbo].[test](name)VALUES('86') SELECT TOP 1 * FROM [BigDota].[dbo].[MatchDetails] WHERE MatchID = @id;";
        protected readonly SimpleParameter Param2 =new SimpleParameter("@id", 5156572898);
        protected const string Database2 = "MsSql"; // 2nd database


       
    }
}