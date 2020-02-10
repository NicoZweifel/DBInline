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
        [Test, NonParallelizable,Order(0)]
        public void DropCreate()
        {
            using var p = Pool();

            p.Query()
                .Set(DropQuery)
                .Run();

            p.Query()
                .Set(CreateQuery)
                .Run();

            p.Commit(); //Test if it saved
            
            var selCount = p.Query()
                .Set(SelectQuery)
                .Select(x => x)
                .ToList()
                .Count;

            Assert.IsTrue(selCount == 0, "Table should be empty.");

            var insCount = p.Query()
                .Set(InsertQuery)
                .Run();

            Assert.IsTrue(insCount == 5, "Table should be filled.");

            p.Commit();
            
             selCount = p.Query()
                .Set(SelectQuery)
                .Select(x => x)
                .ToList()
                .Count;
             
             Assert.IsTrue(selCount == 5, "Table should be filled.");
             p.Commit();
             Assert.Pass();
        }
        
        [Test, NonParallelizable,Order(1)]
        public async Task DropCreateAsync()
        {
           await PoolAsync(p =>
           {
               
               p.Query()
                   .Set(DropQuery)
                   .Run();

               p.Query()
                   .Set(CreateQuery)
                   .Run();

               p.Commit(); //Test if it saved
               
               var selCount = p.Query()
                   .Set(SelectQuery)
                   .Select(x => x)
                   .ToList()
                   .Count;

               Assert.IsTrue(selCount == 0, "Table should be empty.");

               var insCount = p.Query()
                   .Set(InsertQuery)
                   .Run();

               Assert.IsTrue(insCount == 5, "Table should be filled.");

               p.Commit();

               selCount = p.Query()
                   .Set(SelectQuery)
                   .Select(x => x)
                   .ToList()
                   .Count;
             
               Assert.IsTrue(selCount == 5, "Table should be filled.");
               
               Assert.Pass();
               
               p.Commit();
               
           }).ConfigureAwait(false);
        }
    }
}
