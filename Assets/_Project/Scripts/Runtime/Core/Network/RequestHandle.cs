using Cysharp.Threading.Tasks;

namespace ClickerTest.Core.Network
{
    public enum RequestStatus
    {
        Pending,
        Running,
        Completed,
        Failed,
        Cancelled
    }

    public sealed class RequestHandle<T>
    {
        private readonly QueuedRequest<T> _request;

        internal RequestHandle(QueuedRequest<T> request)
        {
            _request = request;
        }

        public string Tag => _request.Tag;
        public RequestStatus Status => _request.Status;

        public UniTask<T> Result => _request.Result;

        public void Cancel() => _request.Cancel();
    }
}
