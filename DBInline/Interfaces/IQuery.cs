using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace DBInline.Interfaces
{

    public interface IQuery : IQueryCommon, ICommand<IQuery>, IWrapCommand
    {
        public IQuery Set(string text);
        public T Scalar<T>();
        public Task<T> ScalarAsync<T>();
    }
    
    public interface IQuery<T> : IQueryCommon, ICommand<IQuery<T>,T>, IWrapCommand
    {
        public IQuery<T> Set(string text);
        public T Scalar();
        public Task<T> ScalarAsync();

    }

}

