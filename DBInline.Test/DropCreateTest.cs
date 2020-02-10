using System.Linq;
using System.Threading.Tasks;
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
                .Text(DropQuery)
                .Run();

            p.Query()
                .Text(CreateQuery)
                .Run();

            p.Commit(); //Test
            
            var selCount = p.Query()
                .Text(SelectQuery)
                .Get(x => x)
                .ToList()
                .Count;

            Assert.IsTrue(selCount == 0, "Table should be empty.");

            var insCount = p.Query()
                .Text(InsertQuery)
                .Run();

            Assert.IsTrue(insCount == 5, "Table should be filled.");

            selCount = p.Query()
                .Text(SelectQuery)
                .Get(x => x)
                .ToList()
                .Count;

            p.Commit();

            Assert.IsTrue(selCount == 5, "Table should be filled.");
            Assert.Pass();
        }

        [Test, NonParallelizable,Order(1)]
        public async Task DropCreateAsync()
        {
           await PoolAsync(p =>
           {
               
               p.Query()
                   .Text(DropQuery)
                   .Run();

               p.Query()
                   .Text(CreateQuery)
                   .Run();

               p.Commit(); //Test
               
               var selCount = p.Query()
                   .Text(SelectQuery)
                   .Get(x => x)
                   .ToList()
                   .Count;

               Assert.IsTrue(selCount == 0, "Table should be empty.");

               var insCount = p.Query()
                   .Text(InsertQuery)
                   .Run();

               Assert.IsTrue(insCount == 5, "Table should be filled.");

               p.Commit(); //Test

               selCount = p.Query()
                   .Text(SelectQuery)
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
