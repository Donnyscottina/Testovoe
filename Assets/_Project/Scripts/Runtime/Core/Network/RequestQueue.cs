using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace ClickerTest.Core.Network
{
    public sealed class RequestQueue : IRequestQueue, IDisposable
    {
        private readonly LinkedList<IQueuedRequest> _pending = new LinkedList<IQueuedRequest>();
        private readonly CancellationTokenSource _lifetimeCts = new CancellationTokenSource();
        private IQueuedRequest _running;
        private bool _isProcessing;
        private bool _isDisposed;

        public int PendingCount => _pending.Count;
        public bool IsBusy => _running != null;

        public RequestHandle<T> Enqueue<T>(string tag, Func<CancellationToken, UniTask<T>> operation)
        {
            if (operation == null)
                throw new ArgumentNullException(nameof(operation));
            if (_isDisposed)
                throw new ObjectDisposedException(nameof(RequestQueue));

            var request = new QueuedRequest<T>(tag, operation, this);
            _pending.AddLast(request);

            if (!_isProcessing)
                ProcessAsync().Forget();

            return new RequestHandle<T>(request);
        }

        public void Cancel(string tag) => CancelWhere(request => request.Tag == tag);

        public void CancelAll() => CancelWhere(_ => true);

        public void Dispose()
        {
            if (_isDisposed)
                return;

            _isDisposed = true;
            CancelAll();
            _lifetimeCts.Cancel();
            _lifetimeCts.Dispose();
        }

        internal void RemovePending(IQueuedRequest request) => _pending.Remove(request);

        private void CancelWhere(Predicate<IQueuedRequest> match)
        {
            var toCancel = new List<IQueuedRequest>();
            foreach (var request in _pending)
            {
                if (match(request))
                    toCancel.Add(request);
            }

            if (_running != null && match(_running))
                toCancel.Add(_running);

            foreach (var request in toCancel)
                request.Cancel();
        }

        private async UniTaskVoid ProcessAsync()
        {
            _isProcessing = true;
            try
            {
                while (_pending.Count > 0 && !_isDisposed)
                {
                    var request = _pending.First.Value;
                    _pending.RemoveFirst();

                    _running = request;
                    try
                    {
                        await request.ExecuteAsync(_lifetimeCts.Token);
                    }
                    finally
                    {
                        _running = null;
                    }
                }
            }
            finally
            {
                _isProcessing = false;
            }
        }
    }
}
