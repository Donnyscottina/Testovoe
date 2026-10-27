using System;
using ClickerTest.Config;
using UniRx;
using UnityEngine;

namespace ClickerTest.Clicker.Model
{
    public sealed class EnergyModel
    {
        private readonly ReactiveProperty<int> _current;

        public EnergyModel(ClickerConfig config)
        {
            Max = config.MaxEnergy;
            _current = new ReactiveProperty<int>(Mathf.Clamp(config.StartEnergy, 0, Max));
        }

        public int Max { get; }
        public IReadOnlyReactiveProperty<int> Current => _current;

        public bool TrySpend(int amount)
        {
            if (amount < 0)
                throw new ArgumentOutOfRangeException(nameof(amount), amount, "Amount must be non-negative.");
            if (_current.Value < amount)
                return false;

            _current.Value -= amount;
            return true;
        }

        public void Restore(int amount)
        {
            if (amount < 0)
                throw new ArgumentOutOfRangeException(nameof(amount), amount, "Amount must be non-negative.");

            _current.Value = Mathf.Min(Max, _current.Value + amount);
        }
    }
}
