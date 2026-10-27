using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ClickerTest.Core.Network;
using ClickerTest.Navigation;
using ClickerTest.Popups;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using Zenject;

namespace ClickerTest.Breeds
{
    public sealed class BreedsPresenter : ITabPresenter, IInitializable, IDisposable
    {
        private readonly BreedsView _view;
        private readonly DogApiService _api;
        private readonly IInfoPopup _popup;
        private readonly CompositeDisposable _disposables = new CompositeDisposable();
        private CancellationTokenSource _visibleCts;
        private IReadOnlyList<BreedSummary> _breeds = Array.Empty<BreedSummary>();
        private RequestHandle<BreedDetails> _detailsRequest;
        private int _loadingIndex = -1;

        public BreedsPresenter(BreedsView view, DogApiService api, IInfoPopup popup)
        {
            _view = view;
            _api = api;
            _popup = popup;
        }

        public TabId Id => TabId.Breeds;

        public void Initialize()
        {
            _view.BreedClicked
                .Subscribe(OnBreedClicked)
                .AddTo(_disposables);
        }

        public void Show()
        {
            _view.SetVisible(true);
            _visibleCts = new CancellationTokenSource();
            LoadBreedsAsync(_visibleCts.Token).Forget();
        }

        public void Hide()
        {
            StopVisibleWork();
            _api.CancelAll();
            ResetDetailsLoading();
            _popup.Hide();
            _view.SetVisible(false);
        }

        public void Dispose()
        {
            StopVisibleWork();
            _disposables.Dispose();
        }

        private async UniTaskVoid LoadBreedsAsync(CancellationToken token)
        {
            _breeds = Array.Empty<BreedSummary>();
            _view.ShowListLoading();

            try
            {
                var (isCanceled, breeds) = await _api.RequestBreeds().Result.SuppressCancellationThrow();
                if (isCanceled || token.IsCancellationRequested)
                    return;

                _breeds = breeds;
                _view.ShowBreeds(breeds.Select(breed => breed.Name).ToList());
            }
            catch (Exception exception)
            {
                if (token.IsCancellationRequested)
                    return;

                Debug.LogWarning($"Breeds request failed: {exception.Message}");
                _view.ShowListError("Не удалось загрузить породы.\nОткройте вкладку ещё раз.");
            }
        }

        private void OnBreedClicked(int index)
        {
            if (index < 0 || index >= _breeds.Count || index == _loadingIndex)
                return;

            _api.CancelBreedDetails();
            ResetDetailsLoading();

            _loadingIndex = index;
            _view.SetBreedLoading(index, true);

            var request = _api.RequestBreedDetails(_breeds[index].Id);
            _detailsRequest = request;
            ShowDetailsAsync(request).Forget();
        }

        private async UniTaskVoid ShowDetailsAsync(RequestHandle<BreedDetails> request)
        {
            try
            {
                var (isCanceled, details) = await request.Result.SuppressCancellationThrow();
                if (isCanceled || request != _detailsRequest)
                    return;

                ResetDetailsLoading();
                _popup.Show(details.Name, details.Description);
            }
            catch (Exception exception)
            {
                if (request != _detailsRequest)
                    return;

                Debug.LogWarning($"Breed details request failed: {exception.Message}");
                ResetDetailsLoading();
                _popup.Show("Ошибка", "Не удалось загрузить информацию о породе. Попробуйте ещё раз.");
            }
        }

        private void ResetDetailsLoading()
        {
            if (_loadingIndex >= 0)
                _view.SetBreedLoading(_loadingIndex, false);

            _loadingIndex = -1;
            _detailsRequest = null;
        }

        private void StopVisibleWork()
        {
            if (_visibleCts == null)
                return;

            _visibleCts.Cancel();
            _visibleCts.Dispose();
            _visibleCts = null;
        }
    }
}
