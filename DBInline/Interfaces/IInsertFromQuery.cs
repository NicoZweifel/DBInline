namespace DBInline.Interfaces
{
    public interface IInsertFromQuery : IValuesBuilder
    {
        public ICommandBuilder From(string tableName);
    }
    public interface IInsertFromQuery<T> :IColumnsBuilder<T>
    {
        public ICommandBuilder<T> From(string tableName);
    }
}