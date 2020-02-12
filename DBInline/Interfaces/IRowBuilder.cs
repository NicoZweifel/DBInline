namespace DBInline.Interfaces
{
    public interface IRowBuilder
    {
        public IRowBuilder AddRow();
        public IInsertQuery AddValue<TIn>(TIn value);
    }

    public interface IRowBuilder<T>
    {
        public IRowBuilder<T> Row();

        public IRowBuilder<T> Add<TIn>(TIn value);
    }
}
