using System.Threading;

namespace DBInline.Interfaces
{
    public interface ITokenHolder
    {
        public CancellationToken Token { get; }
    }
}