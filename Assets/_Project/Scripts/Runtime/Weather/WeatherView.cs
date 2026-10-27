using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ClickerTest.Weather
{
    public sealed class WeatherView : MonoBehaviour
    {
        [SerializeField] private GameObject _forecastRoot;
        [SerializeField] private RawImage _icon;
        [SerializeField] private TMP_Text _temperatureText;
        [SerializeField] private TMP_Text _descriptionText;
        [SerializeField] private TMP_Text _updatedText;
        [SerializeField] private GameObject _loader;
        [SerializeField] private TMP_Text _errorText;

        public void SetVisible(bool visible) => gameObject.SetActive(visible);

        public void ShowLoading()
        {
            _forecastRoot.SetActive(false);
            _errorText.gameObject.SetActive(false);
            _loader.SetActive(true);
        }

        public void ShowForecast(Texture2D icon, string temperature, string description, DateTime updatedAt)
        {
            _loader.SetActive(false);
            _errorText.gameObject.SetActive(false);
            _forecastRoot.SetActive(true);

            _icon.texture = icon;
            _icon.enabled = icon != null;
            _temperatureText.text = temperature;
            _descriptionText.text = description;
            _updatedText.text = $"Обновлено в {updatedAt:HH:mm:ss}";
        }

        public void ShowError(string message)
        {
            _loader.SetActive(false);
            _errorText.text = message;
            _errorText.gameObject.SetActive(true);
        }
    }
}
