namespace DBInline.Interfaces
{
    public interface IRowBuilder
    {
        public IRowBuilder Row();
        public IInsertQuery Add<TIn>(TIn value);
    }

    public interface IRowBuilder<T>
    {
        public IRowBuilder<T> Row();

        public IRowBuilder<T> Add<TIn>(TIn value);
    }
}
