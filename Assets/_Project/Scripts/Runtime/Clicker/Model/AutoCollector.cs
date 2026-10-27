using System;
using ClickerTest.Config;
using UniRx;
using Zenject;

namespace ClickerTest.Clicker.Model
{
    public sealed class AutoCollector : IInitializable, IDisposable
    {
        private readonly ClickerConfig _config;
        private readonly ClickerModel _clicker;
        private IDisposable _timer;

        public AutoCollector(ClickerConfig config, ClickerModel clicker)
        {
            _config = config;
            _clicker = clicker;
        }

        public void Initialize()
        {
            _timer = Observable.Interval(TimeSpan.FromSeconds(_config.AutoCollectIntervalSeconds))
                .Subscribe(_ => _clicker.TryCollect(CollectSource.Auto));
        }

        public void Dispose() => _timer?.Dispose();
    }
}
