using System;
using System.Linq;
using NUnit.Framework;
using static DBInline.Extensions;

namespace DBInline.Test
{
    [NonParallelizable]
    public class PoolTest : TestBase
    {
        /// <summary>
        /// First Query is slow.
        /// </summary>
        /// <returns></returns>
        [Test(ExpectedResult = "86_5156572898_True"), Order(0), NonParallelizable]
        public string FirstPool()
        {
            using var p = Pool();
            
            var values = p.Query<string>()
                .Set(ExampleQuery3)
                .Select(r => (string) r[0])
                .ToList();

            var res1 = p.Query<string>()
                .Set(ExampleQuery1)
                .Param(Param1)
                .Scalar();

            var res2 = p.Query<long>(Database2)
                .Set(ExampleQuery2)
                .Param(Param2.Name, Param2.Value)
                .Scalar();

            p.Commit();
            
            return $"{res1}_" +
                   $"{res2}_{values.Any()}";
        }

        [Test(ExpectedResult = true), Order(1), NonParallelizable]
        public bool UpdateTest()
        {
            var p = Pool();
            var i = p.Query()
                .Set(RollbackQuery)
                .Param(Param1)
                .Run();

            var res1 = !p.Query()
                .Set("SELECT name FROM test WHERE name = \'86\'")
                .Select(r => (string)r[0])
                .ToList()
                .Any();
            
            for (var counter = 1; counter< 10;counter++)
            {
                p.Query()
                    .Set("INSERT INTO test (name) VALUES (86)")
                    .Run();
            }
            p.Commit();

            using var p2 = Pool();
               var res2 = p2.Query<string>()
                .Set("SELECT name FROM test WHERE name like '86'")
                .Select(r => (string)r[0])
                .ToList()
                .Any();

            p2.Commit();

            return i > 0 && res1 && res2;
        }
        //TODO test ms db
        [Test(ExpectedResult = true), Order(2), NonParallelizable]
        public bool UpdateTest2()
        {
            using var p = Pool();
            var i = p.Query()
                .Set(RollbackQuery)
                .Param(Param1)
                .Run();

            var res1 = !p.Query()
                .Set("SELECT name FROM test WHERE name = \'86\'")
                .Select(r => (string)r[0])
                .ToList()
                .Any();
            
            for (var counter = 1; i< 10;i++)
            {
                p.Query()
                    .Set("INSERT INTO test (name) VALUES (86)")
                    .Run();
            }
            p.Commit();

            using var p2 = Pool();
            var res2 = p2.Query<string>()
                .Set("SELECT name FROM test WHERE name like '86'")
                .Select(r => (string)r[0])
                .ToList()
                .Any();

            p2.Commit();

            return i > 0 && res1 && res2;
        }
        
        [Test(ExpectedResult = "True86True"), Order(3), NonParallelizable]
        public string PoolSyntaxTest()
        {
            var t = PoolAsync(async p =>
            {
                var asyncIe =  p.Query<string>()
                    .Set(ExampleQuery3)
                    .SelectAsync(r=> (string)r[0]);

                var res = p.Query<string>()
                    .Set(ExampleQuery1)
                    .Param(Param1)
                    .AddRollback(() => { })
                    .ScalarAsync();

                var t2 = p.Query<long>(Database2)
                    .Set(ExampleQuery2)
                    .Param(Param2)
                    .ScalarAsync();

                var json = "";
                await foreach(var obj in asyncIe)
                {
                    json += obj;
                }

                return json.Any() + await res + (await t2 > 0);
            });
            t.Wait();
            return t.Result;
        }

        [Test(ExpectedResult = "86_5_5156572898"), Order(4), NonParallelizable]
        public string PoolTestRollback()
        {
            var rollbackTest = 1;
            try
            {
                using var p = Pool();
                // ReSharper disable once VariableHidesOuterVariable
                // ReSharper disable once RedundantAssignment
                var res = p.Query<int>()
                    .Set(RollbackQuery)
                    .Param(Param1)
                    .AddRollback(() => { rollbackTest += 1; })
                    .Run();

                // ReSharper disable once RedundantAssignment
                var res2 = p.Query<long>(Database2)
                    .Set(ExampleQuery2)
                    .Param(Param2)
                    .AddRollback( ()=> { rollbackTest += 3; })
                    .Scalar();

                throw new Exception("TEST");
                return res.ToString() + res2;

                
            }
            catch (Exception ex)
            {
                if (!ex.Message.Contains("TEST")) throw;
            }

            return Pool(p =>
            {
                var res1 = p.Query<string>()
                    .Set(ExampleQuery1)
                    .Param(Param1)
                    .Scalar();

                var res2 = p.Query<long>(Database2)
                    .Set(ExampleQuery2)
                    .Param(Param2.Name, Param2.Value)
                    .Scalar();

                return $"{res1}_{rollbackTest}_" +
                       $"{res2}";
            });
        }

        [Test(ExpectedResult = "86_3_5156572898"), Order(5), NonParallelizable]
        public string PoolTestRollbackAsync()
        {
            var rollbackTest = 1;
            try
            {
                int i;
                var t = PoolAsync(async s =>
                {
                    i = await s.Query<int>()
                        .Set(RollbackQuery)
                        .Param(Param1)
                        .AddRollback(() => rollbackTest += 1)
                        .RunAsync();

                    //2nd Query.
                    await s.Query<int>(Database2)
                        .Set(RollbackQuery2)
                        .Param(Param2)
                        .AddRollback(() => rollbackTest += 1)
                        .RunAsync();

                    //Crash to Trigger Rollback.
                    throw new Exception("TEST");
#pragma warning disable 162
                    return i;
#pragma warning restore 162
                });
                t.Wait();
            }
            catch (Exception ex)
            {
                if (!ex.Message.Contains("TEST")) throw;
            }

            return Pool(p =>
            {
                var res1 = p.Query()
                    .Set(ExampleQuery1)
                    .Param(Param1)
                    .Scalar<string>();

                var res2 = p.Query<long>(Database2)
                    .Set(ExampleQuery2)
                    .Param(Param2)
                    .Scalar();

                return $"{res1}_{rollbackTest}_" +
                       $"{res2}";
            });
        }


        [Test(ExpectedResult = true), Order(6), NonParallelizable]
        public bool CancelTest()
        {
            try
            {
                var t = PoolAsync(async p =>
                {

                    var jsons2 = p.Query<string>()
                        .Set(ExampleQuery3)
                        .TableAsync();

                    var res = p.Query<string>()
                        .Set(ExampleQuery1)
                        .Param(Param1)
                        .AddRollback(() => { })
                        .ScalarAsync();

                    var t2 = p.Query<long>(Database2)
                        .Set(ExampleQuery2)
                        .Param(Param2)
                        .ScalarAsync();


                    var table = (await p.Query<string>()
                            .Set(ExampleQuery3)
                            .TableAsync()) //Select as DataTable
                            .ToJson();
                    
                    p.Cancel();

                    return (await jsons2).ToJson().Any() + await res + (await t2 > 0);
                });
                t.Wait();
                return false;
            }
            catch (Exception)
            {
                return true;
            }
        }

    }
}