using NUnit.Framework;
using static DBInline.Extensions;

namespace DBInline.Test
{
    [NonParallelizable, Order(5)]
    public class CleanupTests : TestBase
    {
        // Using Multiple Pools to check if everything is persistent.
        [Test, NonParallelizable, Order(0)]
        public void CleanupTest()
        {
            using var p = Pool();
            p.Query()
                .Drop(TableName)
                .IfExists()
                .Run();
            p.Commit();
            Assert.Pass();
        }
    }
}
