namespace DBInline.Interfaces
{
    public interface IValuesBuilder : ICommand
    { 
        public IRowBuilder AddRow();
    }

    public interface IValuesBuilder<T> : ICommand<T>
    {
        public IRowBuilder<T> Row();
    }
}
