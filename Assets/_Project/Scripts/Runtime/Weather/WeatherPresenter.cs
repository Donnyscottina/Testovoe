using System;
using System.Threading;
using ClickerTest.Config;
using ClickerTest.Core.Network;
using ClickerTest.Navigation;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace ClickerTest.Weather
{
    public sealed class WeatherPresenter : ITabPresenter, IDisposable
    {
        private readonly WeatherView _view;
        private readonly WeatherService _service;
        private readonly NetworkConfig _config;
        private CancellationTokenSource _visibleCts;
        private RequestHandle<WeatherForecast> _lastRequest;
        private bool _hasForecast;

        public WeatherPresenter(WeatherView view, WeatherService service, NetworkConfig config)
        {
            _view = view;
            _service = service;
            _config = config;
        }

        public TabId Id => TabId.Weather;

        public void Show()
        {
            _view.SetVisible(true);
            if (!_hasForecast)
                _view.ShowLoading();

            _visibleCts = new CancellationTokenSource();
            PollAsync(_visibleCts.Token).Forget();
        }

        public void Hide()
        {
            StopPolling();
            _service.CancelRequests();
            _view.SetVisible(false);
        }

        public void Dispose() => StopPolling();

        private async UniTaskVoid PollAsync(CancellationToken token)
        {
            var interval = TimeSpan.FromSeconds(_config.WeatherPollIntervalSeconds);
            while (!token.IsCancellationRequested)
            {
                if (_lastRequest == null ||
                    (_lastRequest.Status != RequestStatus.Pending && _lastRequest.Status != RequestStatus.Running))
                {
                    _lastRequest = _service.RequestForecast();
                    ShowResultAsync(_lastRequest, token).Forget();
                }

                if (await UniTask.Delay(interval, cancellationToken: token).SuppressCancellationThrow())
                    return;
            }
        }

        private async UniTaskVoid ShowResultAsync(RequestHandle<WeatherForecast> request, CancellationToken token)
        {
            try
            {
                var (isCanceled, forecast) = await request.Result.SuppressCancellationThrow();
                if (isCanceled || token.IsCancellationRequested)
                    return;

                _hasForecast = true;
                _view.ShowForecast(forecast.Icon, $"Сегодня - {forecast.Temperature}{forecast.TemperatureUnit}",
                    forecast.ShortForecast, DateTime.Now);
            }
            catch (Exception exception)
            {
                if (token.IsCancellationRequested)
                    return;

                Debug.LogWarning($"Weather request failed: {exception.Message}");
                _view.ShowError("Не удалось загрузить погоду.\nПовторим через несколько секунд.");
            }
        }

        private void StopPolling()
        {
            if (_visibleCts == null)
                return;

            _visibleCts.Cancel();
            _visibleCts.Dispose();
            _visibleCts = null;
        }
    }
}
