
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static DBInline.Extensions;

namespace DBInline.Test
{
    [NonParallelizable]
    internal class CmdSelectList : TestBase
    {
        /// <summary>
        /// First Query is slow.
        /// </summary>
        [Test(ExpectedResult = true), Order(0),NonParallelizable]
        public bool FirstCmd()
        {
            return QueryRun<List<string>>(ExampleQuery3, cmd =>
            {
                {
                    return cmd
                        .Select(r=>(string)r[0])
                        .ToList();
                }
            }).Any();
        }
        
        [Test(ExpectedResult = true), Order(1), NonParallelizable]
        public bool RunAsyncIEnumerable ()
        {
            var pool = PoolAsync(async p =>
            {
                var values = new List<string>();
                
                //AsyncEnumerable
                await foreach (var s in p.Query<string>()
                    .Set(ExampleQuery3)
                    .SelectAsync(r => (string) r[0]))
                {
                    values.Add(s);
                }
                return values.Any();
            });
            pool.Wait();
            return pool.Result;
        }

    
        [Test(ExpectedResult = true), Order(2), NonParallelizable]
        public bool CmdTest()
        {
            return QueryRun<List<string>>(q =>
            {
               
                var res = q.Set(ExampleQuery3)
                    .Select(r=>(string)r[0])
                    .ToList();
                return res;
            }).Any();
        }
        [Test(ExpectedResult = true), Order(3),NonParallelizable]
        public async Task<bool> CmdTestAsync()
        {
            return (await QueryAsync<List<string>>(cmd =>
            {
                {
                    cmd.Set(ExampleQuery3);
                    var res = new List<string>();
                    using var r = cmd.Reader();
                    while (r.Read())
                    {
                        res.Add((string)r[0]);
                    }
                    return res;
                }
            }).ConfigureAwait(false)).Any();
        }
        [Test(ExpectedResult = true), Order(4),NonParallelizable]
        public bool  RowTestAsync()
        {
            var t=   TransactionAsync(tran =>
            {
               return tran.Query<string>()
                    .Set(ExampleQuery3)
                    .Select(r => (string) r[0])
                    .ToList()
                    .Any();
            });
            t.Wait();
            return t.Result;
        }
    }
}
