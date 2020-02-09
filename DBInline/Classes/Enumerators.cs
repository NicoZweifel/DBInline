using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using DBInline.Classes.Iterators;
using DBInline.Classes.Transactions;

namespace DBInline.Classes
{
    namespace Enumerators
    {
        internal interface IDbDataEnumerator<out T> : IEnumerable<T>,  IAsyncEnumerable<T>,IDisposable 
        {
        }
        internal class DbDataEnumerator : IDbDataEnumerator<IDataReader>
        {
            private readonly DbDataIterator _iterator;
            public DbDataEnumerator(Transaction tran, string commandText, IEnumerable<IDbDataParameter> parameters)
            {
                _iterator = new DbDataIterator(tran, commandText, parameters);
            }

            public IAsyncEnumerator<IDataReader> GetAsyncEnumerator(CancellationToken cancellationToken = default)
            {
                return _iterator;
            }

            public IEnumerator<IDataReader> GetEnumerator()
            {
                return _iterator;
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return _iterator;
            }

            public void Dispose()
            {
                _iterator?.Dispose();
            }

            //TODO IQueryable
            // public Type ElementType { get; }
            // public Expression Expression { get; }
            // public IQueryProvider Provider { get; }
        }
        internal class DbDataEnumerator<T> : IDbDataEnumerator<T>
        {
            private readonly DbDataIterator<T> _iterator;
            public DbDataEnumerator(Transaction tran, string commandText, IEnumerable<IDbDataParameter> parameters, Func<IDataReader, T> ctor)
            {
                _iterator = new DbDataIterator<T>(tran, commandText, parameters, ctor);
            }

            public void Dispose()
            {
                _iterator.Dispose();
            }

            public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
            {
                return _iterator;
            }

            public IEnumerator<T> GetEnumerator()
            {
                return _iterator;
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return _iterator;
            }
        }
    }
}
