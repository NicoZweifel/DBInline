using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace DBInline.Interfaces
{

    public interface ICommand : IAddParameter, IAddRollBack
    {
        public ISelectBuilder Select(string [] fields = null);
        public IInsertBuilder InsertInto(string tableName);
        public IUpdateBuilder Update(string tableName);
    }
    public interface ICommand<T> :IAddParameter, IAddRollBack
    {
        public ISelectBuilder<T> Select(string [] fields = null);
        public IInsertBuilder<T> InsertInto(string tableName);
        public IUpdateBuilder<T> Update(string tableName);
    }
}