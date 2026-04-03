using System.Threading;
using Cysharp.Threading.Tasks;

namespace Core
{
    public interface IRequest
    {
        string Id { get; }
        bool IsCancelled { get; }
        void Cancel();
        UniTask ExecuteAsync(CancellationToken token);
    }
}
