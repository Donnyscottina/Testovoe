using System;
using ClickerTest.Clicker.Model;
using ClickerTest.Clicker.View;
using ClickerTest.Navigation;
using UniRx;
using Zenject;

namespace ClickerTest.Clicker
{
    public sealed class ClickerPresenter : ITabPresenter, IInitializable, IDisposable
    {
        private readonly ClickerView _view;
        private readonly ClickerModel _clicker;
        private readonly CurrencyModel _currency;
        private readonly EnergyModel _energy;
        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        public ClickerPresenter(ClickerView view, ClickerModel clicker, CurrencyModel currency, EnergyModel energy)
        {
            _view = view;
            _clicker = clicker;
            _currency = currency;
            _energy = energy;
        }

        public TabId Id => TabId.Clicker;

        public void Initialize()
        {
            _currency.Balance
                .Subscribe(_view.SetCurrency)
                .AddTo(_disposables);

            _energy.Current
                .Subscribe(current => _view.SetEnergy(current, _energy.Max))
                .AddTo(_disposables);

            _view.Clicked
                .Subscribe(_ => _clicker.TryCollect(CollectSource.Manual))
                .AddTo(_disposables);

            _clicker.Collected
                .Subscribe(result => _view.PlayCollectFeedback(result.Reward))
                .AddTo(_disposables);

            _clicker.Rejected
                .Where(source => source == CollectSource.Manual)
                .Subscribe(_ => _view.PlayNoEnergyFeedback())
                .AddTo(_disposables);
        }

        public void Show() => _view.SetVisible(true);

        public void Hide() => _view.SetVisible(false);

        public void Dispose() => _disposables.Dispose();
    }
}
