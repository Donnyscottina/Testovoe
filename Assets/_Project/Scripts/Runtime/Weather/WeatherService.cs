using System;
using System.Collections.Generic;
using System.Threading;
using ClickerTest.Config;
using ClickerTest.Core.Network;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ClickerTest.Weather
{
    public sealed class WeatherService : IDisposable
    {
        private readonly IRequestQueue _queue;
        private readonly IHttpClient _http;
        private readonly NetworkConfig _config;
        private readonly Dictionary<string, Texture2D> _iconCache = new Dictionary<string, Texture2D>();

        public WeatherService(IRequestQueue queue, IHttpClient http, NetworkConfig config)
        {
            _queue = queue;
            _http = http;
            _config = config;
        }

        public RequestHandle<WeatherForecast> RequestForecast() =>
            _queue.Enqueue<WeatherForecast>(RequestTags.Weather, LoadForecastAsync);

        public void CancelRequests() => _queue.Cancel(RequestTags.Weather);

        public void Dispose()
        {
            foreach (var icon in _iconCache.Values)
            {
                if (icon != null)
                    Object.Destroy(icon);
            }

            _iconCache.Clear();
        }

        private async UniTask<WeatherForecast> LoadForecastAsync(CancellationToken token)
        {
            var json = await _http.GetTextAsync(_config.WeatherForecastUrl, token);
            var response = JsonUtility.FromJson<WeatherResponseDto>(json);
            var periods = response?.properties?.periods;
            if (periods == null || periods.Length == 0)
                throw new FormatException("Weather response contains no forecast periods.");

            var today = periods[0];
            var icon = await LoadIconAsync(today.icon, token);
            return new WeatherForecast(today.temperature, today.temperatureUnit, today.shortForecast, icon);
        }

        private async UniTask<Texture2D> LoadIconAsync(string url, CancellationToken token)
        {
            if (string.IsNullOrEmpty(url))
                return null;
            if (_iconCache.TryGetValue(url, out var cached))
                return cached;

            try
            {
                var icon = await _http.GetTextureAsync(url, token);
                _iconCache[url] = icon;
                return icon;
            }
            catch (Exception exception) when (!(exception is OperationCanceledException))
            {
                Debug.LogWarning($"Weather icon was not loaded: {exception.Message}");
                return null;
            }
        }
    }
}
