using System.Data;
using System.Linq;
using System.Threading.Tasks;
using MySqlX.XDevAPI.Relational;
using NUnit.Framework;
using static DBInline.Extensions;

namespace DBInline.Test
{
    [NonParallelizable, Order(0)]
    public class DropCreateTest : TestBase
    {
        // Using Multiple Pools to check if everything is persistent.
        [Test, NonParallelizable, Order(0)]
        public void DropCreate()
        {
            using var p = Pool();

            p.Query()
                .Drop(tableName)
                .IfExists()
                .Run();

            p.Query()
                .Create(tableName)
                .Add("dbid", SqlDbType.Int)
                .Add("name", SqlDbType.VarChar, 50)
                .Run();

            p.Commit(); //Test

            var selCount = p.Query()
                .Select("*")
                .From(tableName)
                .Get(x => (int) x[0])
                .ToList()
                .Count;

            Assert.IsTrue(selCount == 0, "Table should be empty.");

            var insCount = p.Query()
                .Insert(tableName)
                .Add("id")
                .Add("name")
                .Values()
                .Row().Add(1).Add("John Doe")
                .Row().Add(2).Add("James Smith")
                .Row().Add(3).Add("Jack Williams")
                .Row().Add(3).Add("Peter Brown")
                .Row().Add(3).Add("Hans Mueller")
                .Run();

            Assert.IsTrue(insCount == 5, "Table should be filled.");

            selCount = p.Query()
                .Select("*")
                .From(tableName)
                .Get(x => x)
                .ToList()
                .Count;

            p.Commit();

            Assert.IsTrue(selCount == 5, "Table should be filled.");
            Assert.Pass();
        }

        [Test, NonParallelizable, Order(1)]
        public async Task DropCreateAsync()
        {
            await PoolAsync(p =>
            {

                p.Query()
                    .Drop(tableName)
                    .IfExists()
                    .Run();

                p.Query()
                    .Create(tableName)
                    .Add("dbid", SqlDbType.Int)
                    .Add("name", SqlDbType.VarChar, 50)
                    .Run();

                p.Commit(); //Test

                var selCount = p.Query()
                    .Select("*")
                    .From(tableName)
                    .Get(x => (int) x[0])
                    .ToList()
                    .Count;

                Assert.IsTrue(selCount == 0, "Table should be empty.");

                var insCount = p.Query()
                    .Insert(tableName)
                    .Add("id")
                    .Add("name")
                    .Values()
                    .Row().Add(1).Add("John Doe")
                    .Row().Add(2).Add("James Smith")
                    .Row().Add(3).Add("Jack Williams")
                    .Row().Add(3).Add("Peter Brown")
                    .Row().Add(3).Add("Hans Mueller")
                    .Run();

                Assert.IsTrue(insCount == 5, "Table should be filled.");

                p.Commit(); //Test

                selCount = p.Query()
                    .Select("*")
                    .From(tableName)
                    .Get(x => x)
                    .ToList()
                    .Count;

                Assert.IsTrue(selCount == 5, "Table should be filled.");

                Assert.Pass();

                p.Commit();

            }).ConfigureAwait(false);
        }
    }
}
