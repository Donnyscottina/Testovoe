using System;
using ClickerTest.Config;
using UniRx;

namespace ClickerTest.Clicker.Model
{
    public enum CollectSource
    {
        Manual,
        Auto
    }

    public readonly struct CollectResult
    {
        public CollectResult(CollectSource source, int reward)
        {
            Source = source;
            Reward = reward;
        }

        public CollectSource Source { get; }
        public int Reward { get; }
    }

    public sealed class ClickerModel : IDisposable
    {
        private readonly ClickerConfig _config;
        private readonly CurrencyModel _currency;
        private readonly EnergyModel _energy;
        private readonly Subject<CollectResult> _collected = new Subject<CollectResult>();
        private readonly Subject<CollectSource> _rejected = new Subject<CollectSource>();

        public ClickerModel(ClickerConfig config, CurrencyModel currency, EnergyModel energy)
        {
            _config = config;
            _currency = currency;
            _energy = energy;
        }

        public IObservable<CollectResult> Collected => _collected;

        public IObservable<CollectSource> Rejected => _rejected;

        public bool TryCollect(CollectSource source)
        {
            var isManual = source == CollectSource.Manual;
            var cost = isManual ? _config.ClickEnergyCost : _config.AutoCollectEnergyCost;
            var reward = isManual ? _config.ClickReward : _config.AutoCollectReward;

            if (!_energy.TrySpend(cost))
            {
                _rejected.OnNext(source);
                return false;
            }

            _currency.Add(reward);
            _collected.OnNext(new CollectResult(source, reward));
            return true;
        }

        public void Dispose()
        {
            _collected.Dispose();
            _rejected.Dispose();
        }
    }
}
