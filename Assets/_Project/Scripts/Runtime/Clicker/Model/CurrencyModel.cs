using System;
using ClickerTest.Config;
using UniRx;

namespace ClickerTest.Clicker.Model
{
    public sealed class CurrencyModel
    {
        private readonly ReactiveProperty<int> _balance;

        public CurrencyModel(ClickerConfig config)
        {
            _balance = new ReactiveProperty<int>(config.StartCurrency);
        }

        public IReadOnlyReactiveProperty<int> Balance => _balance;

        public void Add(int amount)
        {
            if (amount < 0)
                throw new ArgumentOutOfRangeException(nameof(amount), amount, "Amount must be non-negative.");

            _balance.Value += amount;
        }
    }
}
