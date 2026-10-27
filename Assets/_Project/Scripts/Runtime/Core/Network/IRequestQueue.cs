using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace ClickerTest.Core.Network
{
    public interface IRequestQueue
    {
        int PendingCount { get; }
        bool IsBusy { get; }

        RequestHandle<T> Enqueue<T>(string tag, Func<CancellationToken, UniTask<T>> operation);

        void Cancel(string tag);

        void CancelAll();
    }
}
