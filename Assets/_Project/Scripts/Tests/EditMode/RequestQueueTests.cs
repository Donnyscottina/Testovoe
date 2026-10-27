using System;
using System.Collections.Generic;
using System.Threading;
using ClickerTest.Core.Network;
using Cysharp.Threading.Tasks;
using NUnit.Framework;

namespace ClickerTest.Tests
{
    public sealed class RequestQueueTests
    {
        private RequestQueue _queue;
        private List<string> _started;

        [SetUp]
        public void SetUp()
        {
            _queue = new RequestQueue();
            _started = new List<string>();
        }

        [TearDown]
        public void TearDown() => _queue.Dispose();

        [Test]
        public void Requests_RunOneAfterAnother()
        {
            var first = new FakeRequest("A", _started);
            var second = new FakeRequest("B", _started);

            var firstHandle = _queue.Enqueue("tag", first.RunAsync);
            var secondHandle = _queue.Enqueue("tag", second.RunAsync);

            CollectionAssert.AreEqual(new[] { "A" }, _started);
            Assert.AreEqual(RequestStatus.Running, firstHandle.Status);
            Assert.AreEqual(RequestStatus.Pending, secondHandle.Status);
            Assert.AreEqual(1, _queue.PendingCount);

            first.Complete(1);

            CollectionAssert.AreEqual(new[] { "A", "B" }, _started);
            Assert.AreEqual(RequestStatus.Completed, firstHandle.Status);
            Assert.AreEqual(1, firstHandle.Result.GetAwaiter().GetResult());

            second.Complete(2);

            Assert.AreEqual(2, secondHandle.Result.GetAwaiter().GetResult());
            Assert.IsFalse(_queue.IsBusy);
        }

        [Test]
        public void Cancel_RemovesPendingRequestFromQueue()
        {
            var running = new FakeRequest("A", _started);
            var pending = new FakeRequest("B", _started);

            _queue.Enqueue("other", running.RunAsync);
            var pendingHandle = _queue.Enqueue("weather", pending.RunAsync);

            _queue.Cancel("weather");

            Assert.AreEqual(RequestStatus.Cancelled, pendingHandle.Status);
            Assert.AreEqual(0, _queue.PendingCount);
            Assert.IsTrue(pendingHandle.Result.SuppressCancellationThrow().GetAwaiter().GetResult().IsCanceled);

            running.Complete(1);

            CollectionAssert.AreEqual(new[] { "A" }, _started, "Cancelled request must never start.");
        }

        [Test]
        public void Cancel_AbortsRunningRequestAndStartsNext()
        {
            var running = new FakeRequest("A", _started);
            var next = new FakeRequest("B", _started);

            var runningHandle = _queue.Enqueue("weather", running.RunAsync);
            _queue.Enqueue("other", next.RunAsync);

            _queue.Cancel("weather");

            Assert.IsTrue(running.WasCancellationRequested);
            Assert.AreEqual(RequestStatus.Cancelled, runningHandle.Status);
            CollectionAssert.AreEqual(new[] { "A", "B" }, _started);
        }

        [Test]
        public void Cancel_OnlyAffectsMatchingTag()
        {
            var running = new FakeRequest("A", _started);
            var weather = new FakeRequest("B", _started);
            var breeds = new FakeRequest("C", _started);

            var runningHandle = _queue.Enqueue("breeds", running.RunAsync);
            _queue.Enqueue("weather", weather.RunAsync);
            var breedsHandle = _queue.Enqueue("breeds", breeds.RunAsync);

            _queue.Cancel("weather");

            Assert.AreEqual(RequestStatus.Running, runningHandle.Status);
            Assert.AreEqual(RequestStatus.Pending, breedsHandle.Status);
            Assert.AreEqual(1, _queue.PendingCount);
        }

        [Test]
        public void HandleCancel_CancelsSingleRequest()
        {
            var running = new FakeRequest("A", _started);
            var pendingA = new FakeRequest("B", _started);
            var pendingB = new FakeRequest("C", _started);

            _queue.Enqueue("tag", running.RunAsync);
            var handleA = _queue.Enqueue("tag", pendingA.RunAsync);
            var handleB = _queue.Enqueue("tag", pendingB.RunAsync);

            handleA.Cancel();
            running.Complete(0);

            Assert.AreEqual(RequestStatus.Cancelled, handleA.Status);
            Assert.AreEqual(RequestStatus.Running, handleB.Status);
            CollectionAssert.AreEqual(new[] { "A", "C" }, _started);
        }

        [Test]
        public void FailedRequest_ReportsErrorAndDoesNotBlockQueue()
        {
            var failing = new FakeRequest("A", _started);
            var next = new FakeRequest("B", _started);

            var failingHandle = _queue.Enqueue("tag", failing.RunAsync);
            _queue.Enqueue("tag", next.RunAsync);

            failing.Fail(new InvalidOperationException("boom"));

            Assert.AreEqual(RequestStatus.Failed, failingHandle.Status);
            Assert.Throws<InvalidOperationException>(() => failingHandle.Result.GetAwaiter().GetResult());
            CollectionAssert.AreEqual(new[] { "A", "B" }, _started);
        }

        [Test]
        public void ContinuationCanEnqueueNextRequest()
        {
            var first = new FakeRequest("A", _started);
            var second = new FakeRequest("B", _started);

            var handle = _queue.Enqueue("tag", first.RunAsync);
            handle.Result.ContinueWith(_ => { _queue.Enqueue("tag", second.RunAsync); }).Forget();

            first.Complete(1);

            CollectionAssert.AreEqual(new[] { "A", "B" }, _started);
            Assert.IsTrue(_queue.IsBusy);
        }

        private sealed class FakeRequest
        {
            private readonly string _name;
            private readonly List<string> _log;
            private readonly UniTaskCompletionSource<int> _source = new UniTaskCompletionSource<int>();
            private CancellationToken _token;

            public FakeRequest(string name, List<string> log)
            {
                _name = name;
                _log = log;
            }

            public bool WasCancellationRequested => _token.IsCancellationRequested;

            public UniTask<int> RunAsync(CancellationToken token)
            {
                _token = token;
                _log.Add(_name);
                token.Register(() => _source.TrySetCanceled(token));
                return _source.Task;
            }

            public void Complete(int value) => _source.TrySetResult(value);

            public void Fail(Exception exception) => _source.TrySetException(exception);
        }
    }
}
