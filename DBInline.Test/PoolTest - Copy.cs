using System;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using static DBInline.Extensions;

namespace DBInline.Test
{
    [NonParallelizable]
    public class PoolTests : TestBase
    {
        /// <summary>
        /// First Query is slow.
        /// </summary>
        /// <returns></returns>
        [Test, NonParallelizable]
        public void PoolTest()
        {
            using var p = Pool();
            
            p.Query()
                .Set(CreateQuery)
                .Run();

            var countStart = p.Query()
                .Set(InsertQuery)
                .Run();
            
            Assert.IsTrue(countStart == 5,"Count should be 5.");

            var list = p.Query<string>()
                .Set(SelectQuery)
                .Select(x=>(string)x[1])
                .ToList();
            
            Assert.IsTrue(list.Count == 5,"Count should be 5.");
            
            
            
            // var johnJames = p.Query<string>()
            //     .Set(SelectQuery)
            //     .Where("name", "'John Doe'")
            //     .Or("name","'James Smith'")
            //     .Select(x=>(string)x[1])
            //     .ToList();
            //
            // Assert.IsTrue(johnJames.Count == 2,"Count should be 2.");
            // Assert.IsTrue(johnJames.Contains("James Smith"),"Name missing.");
            // Assert.IsTrue(johnJames.Contains("John Doe"),"Name missing.");
            
            // var drop = p.Query()
            //     .Set(DropQuery)
            //     .Run();
            //
            // Assert.IsTrue(drop == 5,"Count should be 5.");
            //
            // var countEnd = p.Query()
            //     .Set(SelectQuery)
            //     .Run();
            //
            // Assert.IsTrue(countEnd == 0,"Table should be empty.");
        }
    }
}