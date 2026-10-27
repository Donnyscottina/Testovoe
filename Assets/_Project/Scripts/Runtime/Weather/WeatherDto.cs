using System;

namespace ClickerTest.Weather
{
    [Serializable]
    internal sealed class WeatherResponseDto
    {
        public WeatherPropertiesDto properties;
    }

    [Serializable]
    internal sealed class WeatherPropertiesDto
    {
        public WeatherPeriodDto[] periods;
    }

    [Serializable]
    internal sealed class WeatherPeriodDto
    {
        public string name;
        public int temperature;
        public string temperatureUnit;
        public string icon;
        public string shortForecast;
    }
}
