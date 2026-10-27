using UnityEngine;

namespace ClickerTest.Config
{
    [CreateAssetMenu(menuName = "Clicker Test/Network Config", fileName = "NetworkConfig")]
    public sealed class NetworkConfig : ScriptableObject
    {
        [Header("HTTP")]
        [SerializeField] private string _userAgent = "(ClickerTest, test-task@example.com)";
        [SerializeField, Min(1)] private int _timeoutSeconds = 15;

        [Header("Weather")]
        [SerializeField] private string _weatherForecastUrl = "https://api.weather.gov/gridpoints/TOP/32,81/forecast";
        [SerializeField, Min(0.5f)] private float _weatherPollIntervalSeconds = 5f;

        [Header("Dog API")]
        [SerializeField] private string _dogApiBaseUrl = "https://dogapi.dog/api/v2";
        [SerializeField, Range(1, 30)] private int _breedsCount = 10;

        public string UserAgent => _userAgent;
        public int TimeoutSeconds => _timeoutSeconds;
        public string WeatherForecastUrl => _weatherForecastUrl;
        public float WeatherPollIntervalSeconds => _weatherPollIntervalSeconds;
        public string DogApiBaseUrl => _dogApiBaseUrl.TrimEnd('/');
        public int BreedsCount => _breedsCount;
    }
}
