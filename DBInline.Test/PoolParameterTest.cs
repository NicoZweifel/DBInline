using System;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using static DBInline.Extensions;

namespace DBInline.Test
{
    [NonParallelizable,Order(1)]
    public class PoolParameterTests : TestBase
    {
        [Test, NonParallelizable]
        public void PoolTest()
        {
            using var p = Pool();

            var list = p.Query<string>()
                .Set(SelectQuery)
                .Select(x=>(string)x[1])
                .ToList();
            
            Assert.IsTrue(list.Count == 5,"Count should be 5.");
            
            var johnJames = p.Query<string>()
                .Set(SelectQuery)
                .Where("name", "'John Doe'")
                .Or("name","'James Smith'")
                .Select(x=>(string)x[1])
                .ToList();
            
            Assert.IsTrue(johnJames.Count == 2,"Count should be 2.");
            Assert.IsTrue(johnJames.Contains("James Smith"),"Name missing.");
            Assert.IsTrue(johnJames.Contains("John Doe"),"Name missing.");
            
            p.Commit();
        }
    }
}