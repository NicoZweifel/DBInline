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
                .Drop(Customers)
                .IfExists()
                .Run();

            p.Query()
                .Create(Customers)
                .Add("id", SqlDbType.Int)
                .Add("name", SqlDbType.VarChar, 50)
                .Run();

            p.Query()
                .Drop(Employees)
                .IfExists()
                .Run();

            p.Query()
                .Create(Employees)
                .Add("id", SqlDbType.Int)
                .Add("name", SqlDbType.VarChar, 50)
                .Run();
            
            p.Commit(); //Test

            var selCount = p.Query()
                .Select("*")
                .From(Customers)
                .Get(x => (int) x[0])
                .ToList()
                .Count;

            Assert.IsTrue(selCount == 0, "Table should be empty.");

            var insCount = p.Query()
                .Insert(Customers)
                .Add("id")
                .Add("name")
                .Values()
                .AddRow()
                .AddValue(1).AddValue("John Doe")
                .AddRow()
                .AddValue(2).AddValue("James Smith")
                .AddRow()
                .AddValue(3).AddValue("Jack Williams")
                .AddRow()
                .AddValue(4).AddValue("Peter Brown")
                .AddRow()
                .AddValue(5).AddValue("Hans Mueller")
                .Run();

            Assert.IsTrue(insCount == 5, "Table should be filled.");

            selCount = p.Query()
                .Insert(Employees)
                .Add("id")
                .Add("name")
                .Select()
                .Add("id")
                .Add("name")
                .From(Customers)
                .Run();
            
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
                    .Drop(Customers)
                    .IfExists()
                    .Run();

                p.Query()
                    .Create(Customers)
                    .Add("id", SqlDbType.Int)
                    .Add("name", SqlDbType.VarChar, 50)
                    .Run();

                p.Commit(); //Test

                var selCount = p.Query()
                    .Select("*")
                    .From(Customers)
                    .Get(x => (int) x[0])
                    .ToList()
                    .Count;

                Assert.IsTrue(selCount == 0, "Table should be empty.");

                var insCount = p.Query()
                    .Insert(Customers)
                    .Add("id")
                    .Add("name")
                    .Values()
                    .AddRow().AddValue(1).AddValue("John Doe")
                    .AddRow().AddValue(2).AddValue("James Smith")
                    .AddRow().AddValue(3).AddValue("Jack Williams")
                    .AddRow().AddValue(4).AddValue("Peter Brown")
                    .AddRow().AddValue(5).AddValue("Hans Mueller")
                    .Run();

                Assert.IsTrue(insCount == 5, "Table should be filled.");

                p.Commit(); //Test

                selCount = p.Query()
                    .Select("*")
                    .From(Customers)
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
