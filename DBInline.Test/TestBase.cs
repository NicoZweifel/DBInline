using NUnit.Framework;
using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using DBInline.Classes;


namespace DBInline.Test
{
    public class TestBase
    {
        /// <summary>
        /// Place credentials in Environment.CurrentDirectory + "creds\\data.json")
        /// </summary>
        [SetUp]
        public  void Setup()
        {
            // File.WriteAllText(Path.Combine(Environment.CurrentDirectory, "creds\\data.json"),JsonSerializer.Serialize(new List<DatabaseCredentials>(){  }));
                if (ContextController.Connected) return;
                var creds = JsonSerializer.Deserialize<DatabaseCredentials[]>(File.ReadAllText(Path.Combine(Environment.CurrentDirectory, "creds\\data.json")));
                ContextController.AddContext("postgres", creds.First().Type, creds.First().GetConnectionString(), true);
                ContextController.AddContext("MsSql", creds.Last().Type, creds.Last().GetConnectionString());
        }

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