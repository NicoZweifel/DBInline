using System.Windows.Input;

namespace DBInline.Interfaces
{
    public interface IColumnsBuilder
    {
        public IColumnsBuilder Add(string columnName);
        public ISelectBuilder Select();
        public ISelectBuilder Select(params string[] fields);
        public IValuesBuilder Values();
        public IInsertQuery Values(params string[] values);
    }

    public interface IColumnsBuilder<T>
    {
        public IColumnsBuilder<T> Add(string columnName);
        public ISelectBuilder<T> Select();
        public ISelectBuilder<T> Select(params string[] fields);
        public IValuesBuilder Values();
        public IInsertQuery<T> Values(params string[] values);
    }
}
