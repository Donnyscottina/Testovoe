using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using Zenject;

namespace ClickerTest.Navigation
{
    public sealed class TabsPresenter : IInitializable, IDisposable
    {
        private const TabId StartTab = TabId.Clicker;

        private readonly TabBarView _view;
        private readonly Dictionary<TabId, ITabPresenter> _tabs;
        private readonly CompositeDisposable _disposables = new CompositeDisposable();
        private TabId? _current;

        public TabsPresenter(TabBarView view, List<ITabPresenter> tabs)
        {
            _view = view;
            _tabs = tabs.ToDictionary(tab => tab.Id);
        }

        public void Initialize()
        {
            foreach (var tab in _tabs.Values)
                tab.Hide();

            _view.TabSelected
                .Subscribe(Select)
                .AddTo(_disposables);

            Select(StartTab);
        }

        public void Dispose() => _disposables.Dispose();

        private void Select(TabId id)
        {
            if (_current == id)
                return;

            if (_current.HasValue)
                _tabs[_current.Value].Hide();

            _current = id;
            _tabs[id].Show();
            _view.SetSelected(id);
        }
    }
}
