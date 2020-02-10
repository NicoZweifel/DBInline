namespace DBInline.Interfaces
{
    public interface IInsertFromQuery : IValuesBuilder
    {
        public ICommandBuilder From(string tableName);
    }
    public interface IColumnsFromQuery<T> :IColumnsBuilder<T>
    {
        public ICommandBuilder<T> From(string tableName);
    }
}