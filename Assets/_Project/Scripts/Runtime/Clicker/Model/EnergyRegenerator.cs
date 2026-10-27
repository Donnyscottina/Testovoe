using System;
using ClickerTest.Config;
using UniRx;
using Zenject;

namespace ClickerTest.Clicker.Model
{
    public sealed class EnergyRegenerator : IInitializable, IDisposable
    {
        private readonly ClickerConfig _config;
        private readonly EnergyModel _energy;
        private IDisposable _timer;

        public EnergyRegenerator(ClickerConfig config, EnergyModel energy)
        {
            _config = config;
            _energy = energy;
        }

        public void Initialize()
        {
            _timer = Observable.Interval(TimeSpan.FromSeconds(_config.EnergyRegenIntervalSeconds))
                .Subscribe(_ => _energy.Restore(_config.EnergyRegenAmount));
        }

        public void Dispose() => _timer?.Dispose();
    }
}
