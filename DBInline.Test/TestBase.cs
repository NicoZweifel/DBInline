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


        protected const string Customers = "dbinline_generated_table";
        
        protected const string Employees = "dbinline_generated_table2";
        
    }
}
