using System.Collections.Generic;
using static DBInline.Extensions;
using NUnit.Framework;
using System.Linq;


namespace DBInline.Test
{
    [NonParallelizable]
    internal class Tests : TestBase
    {
        /// <summary>
        /// First Query is slow.
        /// </summary>
        [Test(ExpectedResult = "86"), Order(0)]
        public string FirstRead()
        {
            // ReSharper disable once AssignmentIsFullyDiscarded
            return CmdScalar<string>(ExampleQuery1);
        }
        [Test(ExpectedResult = "86"), Order(5)]
        public string SelectTest()
        {
            return QueryRun<List<string>>(ExampleQuery1, cmd =>
            {
                cmd.Param(Param1);
                var res = new List<string>();
                using var r = cmd.Reader();
                while (r.Read())
                {
                    res.Add((string)r[0]);
                }
                return res;
            }).First();
        }
        [Test(ExpectedResult =true), Order(8)]
        public bool PoolTest()
        {
            return Pool(p => p.Query<long>(Database2)
                    .Set(ExampleQuery2)
                    .Param(Param2)
                    .Select(r => (long)r[0])
                    .ToList())
                    .Any();
        }
        [Test(ExpectedResult = 5158976131), Order(9)]
        public long CmdScalarTest()
        {
            // ReSharper disable once AssignmentIsFullyDiscarded
            return Pool(p => 
                p.Query<long>(Database2)
                .Set(ExampleQuery2)
                .Param(Param2)
                .Scalar());
        }
      
        [Test(ExpectedResult = 5158976131), Order(13)]
        public long ChainTest()
        {
            return Pool(s => s.Query<long>(Database2)
                .Set(ExampleQuery2)
                .Param(Param2)
                .Scalar());
        }
    }
}
