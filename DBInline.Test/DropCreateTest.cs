using System.Linq;
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

            using var p2 = Pool();

            var selCount = p2.Query()
                .Set(SelectQuery)
                .Select(x => x)
                .ToList()
                .Count;

            Assert.IsTrue(selCount == 0, "Table should be empty.");

            var insCount = p2.Query()
                .Set(InsertQuery)
                .Run();

            Assert.IsTrue(insCount == 5, "Table should be filled.");

            p2.Commit();

            using var p3 = Pool();
            
             selCount = p3.Query()
                .Set(SelectQuery)
                .Select(x => x)
                .ToList()
                .Count;
             
             Assert.IsTrue(selCount == 5, "Table should be filled.");
             
             p3.Commit();
        }
    }
}