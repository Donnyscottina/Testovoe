using UnityEngine;

namespace ClickerTest.Weather
{
    public sealed class WeatherForecast
    {
        public WeatherForecast(int temperature, string temperatureUnit, string shortForecast, Texture2D icon)
        {
            Temperature = temperature;
            TemperatureUnit = temperatureUnit;
            ShortForecast = shortForecast;
            Icon = icon;
        }

        public int Temperature { get; }
        public string TemperatureUnit { get; }
        public string ShortForecast { get; }

        public Texture2D Icon { get; }
    }
}
