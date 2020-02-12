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
//doesnt work yet
            var test = p.Query<int>()
                .Select()
                .Add("id")
                .Add("name")
                .From(TableName)
                .Where("id",2)
                .Scalar();
//doesnt work yet
            var test2 = p.Query<string>()
                .Update(TableName)
                .Set("name", "John Doe2")
                .Set("id",6)
                .Where("id", 1)
                .Select()
                .Add("name")
                .From(TableName)
                .Where("id",6)
                .Scalar();

            var test3 = p.Query<int>()
                .Update(TableName)
                .Set("name", "John Doe")
                .Set("id", 1)
                .Where("id", 6)
                .Scalar();
            
            
//the rest works
            var list = p.Query()
                .Select("*")
                .From(TableName)
                .Get(x => (string) x[1])
                .ToList();

            Assert.IsTrue(list.Count == 5, "Count should be 5.");

            var johnJames = p.Query()
                .Select("*")
                .From(TableName)
                .Where("name", "John Doe")
                .Or("name", "James Smith")
                .Get(x => (string) x[1])
                .ToList();

            Assert.IsTrue(johnJames.Count == 2, "Count should be 2.");
            Assert.IsTrue(johnJames.Contains("James Smith"), "Name missing.");
            Assert.IsTrue(johnJames.Contains("John Doe"), "Name missing.");

            var peter = p.Query<int>()
                .Select("*")
                .From(TableName)
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
                    .Select("*")
                    .From(TableName)
                    .GetAsyncEnumerable(x => (string) x[1])
                    .ConfigureAwait(false))
                {
                    list.Add(x);
                }

                Assert.IsTrue(list.Count == 5, "Count should be 5.");


                var johnJames = (await p.Query()
                    .Select("*")
                    .From(TableName)
                    .Where("name", "John Doe")
                    .Or("name", "James Smith")
                    .GetAsync(x => (string) x[1])
                    .ConfigureAwait(false)).ToList();

                Assert.IsTrue(johnJames.Count == 2, "Count should be 2.");
                Assert.IsTrue(johnJames.Contains("James Smith"), "Name missing.");
                Assert.IsTrue(johnJames.Contains("John Doe"), "Name missing.");

                var peter = await p.Query<int>()
                    .Select("id")
                    .From(TableName)
                    .Where("name","Peter Brown")
                    .ScalarAsync()
                    .ConfigureAwait(false);

                Assert.IsTrue(peter == 4, "Name missing.");
                Assert.Pass();
            });
        }
    }
}
