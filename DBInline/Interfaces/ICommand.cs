using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data.Common;
using System.Threading.Tasks;

namespace DBInline.Interfaces
{

    public interface ICommand : IAddParameter, IAddRollBack
    {
        public ISelectBuilder Select();
        public ISelectBuilder Select(params string[] fields);
        public IInsertBuilder Insert(string tableName);
        public IUpdateBuilder Update(string tableName);
        public IDropBuilder Drop(string tableName);
        public ICreateBuilder Create(string tableName);
        public IDeleteQuery Delete(string tableName);
    }
    
    public interface IDeleteQuery : ICommandBuilder,ICommand
    { 
        public int Run();
        public Task<int> RunAsync();
    }
    public interface IDeleteQuery<T> : ICommandBuilder<T>, ICommand<T>
    {
        public int Run();
        public Task<int> RunAsync();
    }

    public interface ICommand<T> :IAddParameter, IAddRollBack
    {
        public ISelectBuilder<T> Select();
        public ISelectBuilder<T> Select(params string [] fields);
        public IInsertBuilder<T> Insert(string tableName);
        public IUpdateBuilder<T> Update(string tableName);
        public IDropBuilder<T> Drop(string tableName);
        public ICreateBuilder<T> Create(string tableName);
        public IDeleteQuery<T> Delete(string tableName);
    }
}