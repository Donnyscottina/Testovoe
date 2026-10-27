using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace ClickerTest.Core.Network
{
    internal interface IQueuedRequest
    {
        string Tag { get; }
        RequestStatus Status { get; }
        UniTask ExecuteAsync(CancellationToken lifetimeToken);
        void Cancel();
    }

    internal sealed class QueuedRequest<T> : IQueuedRequest
    {
        private readonly Func<CancellationToken, UniTask<T>> _operation;
        private readonly RequestQueue _owner;
        private readonly UniTaskCompletionSource<T> _completion = new UniTaskCompletionSource<T>();
        private CancellationTokenSource _runningCts;

        public QueuedRequest(string tag, Func<CancellationToken, UniTask<T>> operation, RequestQueue owner)
        {
            Tag = tag;
            _operation = operation;
            _owner = owner;
        }

        public string Tag { get; }
        public RequestStatus Status { get; private set; } = RequestStatus.Pending;
        public UniTask<T> Result => _completion.Task;

        public async UniTask ExecuteAsync(CancellationToken lifetimeToken)
        {
            if (Status != RequestStatus.Pending)
                return;

            Status = RequestStatus.Running;
            _runningCts = CancellationTokenSource.CreateLinkedTokenSource(lifetimeToken);
            try
            {
                var result = await _operation(_runningCts.Token);
                if (Status != RequestStatus.Running)
                    return;

                Status = RequestStatus.Completed;
                _completion.TrySetResult(result);
            }
            catch (OperationCanceledException)
            {
                MarkCancelled();
            }
            catch (Exception exception)
            {
                if (Status != RequestStatus.Running)
                    return;

                Status = RequestStatus.Failed;
                _completion.TrySetException(exception);
            }
            finally
            {
                _runningCts.Dispose();
                _runningCts = null;
            }
        }

        public void Cancel()
        {
            switch (Status)
            {
                case RequestStatus.Pending:
                    _owner.RemovePending(this);
                    MarkCancelled();
                    break;

                case RequestStatus.Running:
                    var cts = _runningCts;
                    MarkCancelled();
                    cts?.Cancel();
                    break;
            }
        }

        private void MarkCancelled()
        {
            if (Status == RequestStatus.Cancelled)
                return;

            Status = RequestStatus.Cancelled;
            _completion.TrySetCanceled();
        }
    }
}
