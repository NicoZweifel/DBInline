using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using DBInline.Classes.Transactions;

namespace DBInline.Classes
{
    namespace Iterators
    {
        internal class DbDataIterator<T> : DbDataIterator, IEnumerator<T>, IAsyncEnumerator<T> 
        {
            private readonly Func<IDataReader, T> _ctor;
            public DbDataIterator(Transaction tran, string commandText, IEnumerable<IDbDataParameter> parameters, Func<IDataReader, T> ctor) : base(tran, commandText, parameters)
            {
                this._ctor = ctor;
            }
            T IEnumerator<T>.Current => _ctor(base.Current);
            T IAsyncEnumerator<T>.Current => _ctor(base.Current);
        }
        internal class DbDataIterator : IEnumerator<DbDataReader>, IAsyncEnumerator<DbDataReader>
        {
            // ReSharper disable once MemberCanBePrivate.Global
            public DbCommand Command { get; private set; }
            public DbDataReader Current { get; private set; }
            // ReSharper disable once MemberCanBePrivate.Global
            public DbTransaction Tran { get; }
            // ReSharper disable once MemberCanBePrivate.Global
            public string CommandText { get; }
            object IEnumerator.Current => Current;
            public DbDataIterator(Transaction tran, string commandText, IEnumerable<IDbDataParameter> parameters)
            {
                CommandText = commandText;
                Tran = tran;
                Command = tran.Connection.CreateCommand();
                Command.Transaction = tran.DbTransaction;
                Command.CommandText = commandText;
                foreach (var p in parameters)
                {
                    Command.Parameters.Add(p);
                }
            }

            public void Dispose()
            {
                Current?.Dispose();
                Command.Dispose();
            }

            public bool MoveNext()
            {
                if (Current != null) return Current.Read();
                Current = Command.ExecuteReader();
                return Current != null && Current.Read();
            }

            public void Reset()
            {
                Dispose();
                Command = Tran.Connection.CreateCommand();
                Command.CommandText = CommandText;
                foreach (var p in Command.Parameters)
                {
                    Command.Parameters.Add(p);
                }
                Current = null;
            }

            public async ValueTask<bool> MoveNextAsync()
            {
                if (Current != null) return await Current.ReadAsync().ConfigureAwait(false);
                Current = await Command.ExecuteReaderAsync().ConfigureAwait(false);
                return Current != null && await Current.ReadAsync().ConfigureAwait(false);
            }

            public async ValueTask DisposeAsync()
            {
                if (Current != null) await Current.DisposeAsync().ConfigureAwait(false);
            }
        }
    }
}
