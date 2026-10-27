using System;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace ClickerTest.Navigation
{
    public sealed class TabButtonView : MonoBehaviour
    {
        [SerializeField] private TabId _id;
        [SerializeField] private Button _button;
        [SerializeField] private Image _highlight;
        [SerializeField] private Image _icon;
        [SerializeField] private TMP_Text _label;
        [SerializeField] private Color _selectedColor = new Color(1f, 0.72f, 0.18f);
        [SerializeField] private Color _normalColor = new Color(0.6f, 0.64f, 0.75f);

        public TabId Id => _id;
        public IObservable<Unit> Clicked => _button.OnClickAsObservable();

        public void SetSelected(bool selected)
        {
            var color = selected ? _selectedColor : _normalColor;
            _icon.color = color;
            _label.color = color;
            _highlight.enabled = selected;
        }
    }
}
