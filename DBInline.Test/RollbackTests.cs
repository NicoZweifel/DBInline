using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using static DBInline.Extensions;

namespace DBInline.Test
{
    [NonParallelizable, Order(2)]
    public class RollBackTests : TestBase
    {
        private const string ExMsg = "Rollback Test.";

        [Test, NonParallelizable, Order(0)]
        public void PoolRollbackTest()
        {
            try
            {
                using var p = Pool();
                
                p.Query()
                    .Set(DeleteQuery)
                    .Run();

                var l = p.Query()
                    .Set(SelectQuery)
                    .Get(x => x)
                    .ToList();

                Assert.IsTrue(!l.Any(), "Table should be empty.");

                throw new TestException();
                // ReSharper disable once HeuristicUnreachableCode
#pragma warning disable 162
                p.Commit();
#pragma warning restore 162
            }
            catch (Exception e)
            {
                Debug.Write(e.ToString());
                
                var l = Transaction(t =>
                {
                    return t.Query()
                        .Set(SelectQuery)
                        .Get(x => (int) x[0])
                        .ToList();
                });
                
                Assert.IsTrue(!l.Any(), "Rollback has failed!");
            }
            Assert.Pass();
        }


        [Test, NonParallelizable, Order(1)]
        public void PoolRollbackTestAsyncLambda()
        {
            try
            {
                Pool(async p =>
                {
                    await p.Query()
                        .Set(DeleteQuery)
                        .RunAsync()
                        .ConfigureAwait(false);

                    var l = p.Query()
                        .Set(SelectQuery)
                        .Get(x => x)
                        .ToList();

                    Assert.IsTrue(!l.Any(), "Table should be empty.");

                    throw new TestException();
                    // ReSharper disable once HeuristicUnreachableCode
#pragma warning disable 162
                    p.Commit();
#pragma warning restore 162

                });
            }
            catch (Exception e)
            {
                Debug.Write(e.ToString());
                var l = Transaction(t =>
                {
                    return t.Query()
                        .Set(SelectQuery)
                        .Get(x => (int) x[0])
                        .ToList();
                });
                Assert.IsTrue(!l.Any(), "Rollback has failed!");
            }
            Assert.Pass();
        }

        [Test, NonParallelizable, Order(2)]
        public async Task AsyncPoolRollbackTest()
        {
            try
            {
                await PoolAsync(p =>
                {
                    p.Query()
                        .Set(DeleteQuery)
                        .Run();

                    var selCount = p.Query()
                        .Set(SelectQuery)
                        .Get(x => x)
                        .ToList()
                        .Count;
                    
                    Assert.IsTrue(selCount == 0, "Table should be empty.");
                    
                    throw new TestException();
                    
                }).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                Debug.Write(e.ToString());
                
                var l = Transaction(t =>
                {
                    return t.Query()
                        .Set(SelectQuery)
                        .Get(x => (int) x[0])
                        .ToList();
                });
                
                Assert.IsTrue(!l.Any(), "Rollback has failed!");
            }
            Assert.Pass();
        }

        [Test, NonParallelizable, Order(3)]
        public async Task AsyncPoolRollbackTestAsyncLambda()
        {
            try
            {
                await PoolAsync(async p =>
                {
                    await p.Query()
                        .Set(DeleteQuery)
                        .RunAsync()
                        .ConfigureAwait(false);

                    var count = (await p.Query()
                            .Set(SelectQuery)
                            .GetAsync(x => x)
                            .ConfigureAwait(false))
                        .Count;

                    Assert.IsTrue(count == 0, "Table should be empty.");
                    
                    throw new TestException();
                    
                }).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                Debug.Write(e.ToString());
                
                var l = await TransactionAsync(t =>
                {
                    return t.Query()
                        .Set(SelectQuery)
                        .Get(x => (int) x[0])
                        .ToList();
                }).ConfigureAwait(false);
                
                Assert.IsTrue(!l.Any(), "Rollback has failed!");
            }
            Assert.Pass();
        }

        [Test, NonParallelizable, Order(4)]
        public void TransactionRollbackTest()
        {
            try
            {
                Transaction(t =>
                {
                    t.Query()
                        .Set(DeleteQuery)
                        .Run();
                    
                    throw new TestException();
                    
                });
            }
            catch (Exception e)
            {
                Debug.Write(e.ToString());
                
                var l = Transaction(t =>
                {
                    return t.Query()
                        .Set(SelectQuery)
                        .Get(x => (int) x[0])
                        .ToList();
                });
                
                Assert.IsTrue(!l.Any(), "Rollback has failed!");
            }
            Assert.Pass();
        }

        [Test, NonParallelizable, Order(5)]
        public async Task TransactionAsyncRollbackTest()
        {
            try
            {
                await TransactionAsync(t =>
                {
                    t.Query()
                        .Set(DeleteQuery)
                        .Run();
                    
                    throw new TestException();
                    
                }).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                Debug.Write(e.ToString());
                
                var l = await TransactionAsync(t =>
                {
                    return t.Query()
                        .Set(SelectQuery)
                        .Get(x => (int) x[0])
                        .ToList();
                }).ConfigureAwait(false);
                
                Assert.IsTrue(!l.Any(), "Rollback has failed!");
            }
            Assert.Pass();
        }

        private class TestException : Exception
        {
            public TestException() : base(ExMsg)
            {
            }
        }
    }
}




    
