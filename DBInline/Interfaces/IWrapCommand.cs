using DBInline.Classes;

namespace DBInline.Interfaces
{
    public interface IWrapCommand
    { 
        internal Command Command { get; }
    }
}