using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using static DBInline.Extensions;


namespace DBInline.Test
{
    [NonParallelizable, Order(1)]
    public class PoolParameterTests : TestBase
    {
        [Test, NonParallelizable, Order(0)]
        public void PoolTest()
        {
            using var p = Pool();

            var list = p.Query()
                .Set(SelectQuery)
                .Select(x => (string) x[1])
                .ToList();

            Assert.IsTrue(list.Count == 5, "Count should be 5.");

            var johnJames = p.Query()
                .Set(SelectQuery)
                .Where("name", "John Doe")
                .Or("name", "James Smith")
                .Select(x => (string) x[1])
                .ToList();

            Assert.IsTrue(johnJames.Count == 2, "Count should be 2.");
            Assert.IsTrue(johnJames.Contains("James Smith"), "Name missing.");
            Assert.IsTrue(johnJames.Contains("John Doe"), "Name missing.");

            var peter = p.Query<int>()
                .Set(SelectQuery)
                .Where("name", "Peter Brown")
                .Scalar();

            Assert.IsTrue(peter == 4, "Name missing.");
            p.Commit();
            Assert.Pass();
        }


        [Test, NonParallelizable, Order(1)]
        public void PoolTestAsync()
        {
            PoolAsync(async p =>
            {
                var list = new List<string>();

                await foreach (var x in p.Query()
                    .Set(SelectQuery)
                    .SelectAsyncEnumerable(x => (string) x[1])
                    .ConfigureAwait(false))
                {
                    list.Add(x);
                }

                Assert.IsTrue(list.Count == 5, "Count should be 5.");


                var johnJames = (await p.Query()
                    .Set(SelectQuery)
                    .Where("name", "John Doe")
                    .Or("name", "James Smith")
                    .SelectAsync(x => (string) x[1])
                    .ConfigureAwait(false)).ToList();

                Assert.IsTrue(johnJames.Count == 2, "Count should be 2.");
                Assert.IsTrue(johnJames.Contains("James Smith"), "Name missing.");
                Assert.IsTrue(johnJames.Contains("John Doe"), "Name missing.");

                var peter = await p.Query<int>()
                    .Set(SelectQuery)
                    .ScalarAsync()
                    .ConfigureAwait(false);

                Assert.IsTrue(peter == 4, "Name missing.");
                Assert.Pass();
            });
        }
    }
}
