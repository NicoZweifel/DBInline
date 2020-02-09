using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Bson;
using NUnit.Framework;
using Renci.SshNet.Messages.Connection;
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

                var selCount = p.Query()
                    .Set(SelectQuery)
                    .Select(x => x)
                    .ToList()
                    .Count;

                Assert.IsTrue(selCount == 0, "Table should be empty.");

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
                        .Select(x => (int) x[0])
                        .ToList();
                });
                Assert.IsTrue(l.Count > 0, "Rollback has failed!");
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

                    var selCount = p.Query()
                        .Set(SelectQuery)
                        .Select(x => x)
                        .ToList()
                        .Count;

                    Assert.IsTrue(selCount == 0, "Table should be empty.");

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
                        .Select(x => (int) x[0])
                        .ToList();
                });
                Assert.IsTrue(l.Count > 0, "Rollback has failed!");
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
                        .Select(x => x)
                        .ToList()
                        .Count;
                    Assert.IsTrue(selCount == 0, "Table should be empty.");
                    throw new TestException();
                    // ReSharper disable once HeuristicUnreachableCode
#pragma warning disable 162
                    p.Commit();
#pragma warning restore 162
                }).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                Debug.Write(e.ToString());
                var l = Transaction(t =>
                {
                    return t.Query()
                        .Set(SelectQuery)
                        .Select(x => (int) x[0])
                        .ToList();
                });
                Assert.IsTrue(l.Count > 0, "Rollback has failed!");
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

                    var selCount = (await p.Query()
                            .Set(SelectQuery)
                            .SelectAsync(x => x)
                            .ConfigureAwait(false))
                        .Count;

                    Assert.IsTrue(selCount == 0, "Table should be empty.");
                    throw new TestException();
                    // ReSharper disable once HeuristicUnreachableCode
#pragma warning disable 162
                    p.Commit();
#pragma warning restore 162
                }).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                Debug.Write(e.ToString());
                var l = await TransactionAsync(t =>
                {
                    return t.Query()
                        .Set(SelectQuery)
                        .Select(x => (int) x[0])
                        .ToList();
                }).ConfigureAwait(false);
                Assert.IsTrue(l.Count > 0, "Rollback has failed!");
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
                        .Select(x => (int) x[0])
                        .ToList();
                });
                Assert.IsTrue(l.Count > 0, "Rollback has failed!");
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
                        .Select(x => (int) x[0])
                        .ToList();
                }).ConfigureAwait(false);
                Assert.IsTrue(l.Count > 0, "Rollback has failed!");
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




    
