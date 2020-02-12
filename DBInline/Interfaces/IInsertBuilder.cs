namespace DBInline.Interfaces
{
    public interface IInsertBuilder
    {
        public ISelectBuilder Select(params string[] columns);
        public IInsertBuilder Add(params string[] columnNames);
        public IValuesBuilder Values();
    }

    public interface IInsertBuilder<T>
    {
        public ISelectBuilder<T> Select(params string[] columns);
        public IInsertBuilder<T> Add(params string[] columnNames);
        public IValuesBuilder<T> Values();
    }
}