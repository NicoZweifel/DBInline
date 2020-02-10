using System.Collections.Generic;
using System.Linq;
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
//doesnt work yet
            var test = p.Query<int>()
                .Select()
                .Add("id")
                .Add("name")
                .From(tableName)
                .Where("id",2)
                .Scalar();
//doesnt work yet
            var test2 = p.Query<int>()
                .Update(tableName)
                .Set("name", "John Doe2")
                .Set("id",6)
                .Where("id", 1)
                .Select()
                .Add("name")
                .From(tableName)
                .Where("id",6)
                .Scalar();
            
//the rest works
            var list = p.Query()
                .Set(SelectQuery)
                .Get(x => (string) x[1])
                .ToList();

            Assert.IsTrue(list.Count == 5, "Count should be 5.");

            var johnJames = p.Query()
                .Set(SelectQuery)
                .Where("name", "John Doe")
                .Or("name", "James Smith")
                .Get(x => (string) x[1])
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
                    .GetAsyncEnumerable(x => (string) x[1]))
                {
                    list.Add(x);
                }

                Assert.IsTrue(list.Count == 5, "Count should be 5.");


                var johnJames = (await p.Query()
                    .Set(SelectQuery)
                    .Where("name", "John Doe")
                    .Or("name", "James Smith")
                    .GetAsync(x => (string) x[1])
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
