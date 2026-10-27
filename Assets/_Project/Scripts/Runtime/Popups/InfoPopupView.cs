using System.Threading;
using ClickerTest.Core.Tweening;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ClickerTest.Popups
{
    public sealed class InfoPopupView : MonoBehaviour, IInfoPopup
    {
        [SerializeField] private GameObject _root;
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private RectTransform _panel;
        [SerializeField] private TMP_Text _title;
        [SerializeField] private TMP_Text _body;
        [SerializeField] private ScrollRect _bodyScroll;
        [SerializeField] private LayoutElement _bodyLayout;
        [SerializeField] private Button _okButton;
        [SerializeField] private RectTransform _screenRect;
        [SerializeField, Range(0.1f, 0.9f)] private float _maxBodyHeightRatio = 0.55f;
        [SerializeField] private float _appearDuration = 0.22f;

        private CancellationTokenSource _animationCts;

        public bool IsOpen => _root.activeSelf;

        private void Awake()
        {
            _okButton.onClick.AddListener(Hide);
            _root.SetActive(false);
        }

        public void Show(string title, string body)
        {
            _title.text = title;
            _body.text = string.IsNullOrWhiteSpace(body) ? "Описание отсутствует." : body;

            _root.SetActive(true);
            FitBodyHeight();
            PlayAppear();
        }

        public void Hide()
        {
            StopAnimation();
            _root.SetActive(false);
        }

        private void FitBodyHeight()
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(_panel);
            var width = ((RectTransform)_bodyScroll.transform).rect.width;
            var textHeight = _body.GetPreferredValues(_body.text, width, 0f).y;
            var maxHeight = _screenRect.rect.height * _maxBodyHeightRatio;

            _bodyLayout.preferredHeight = Mathf.Min(textHeight, maxHeight);
            _bodyScroll.vertical = textHeight > maxHeight;
            LayoutRebuilder.ForceRebuildLayoutImmediate(_panel);
            _bodyScroll.verticalNormalizedPosition = 1f;
        }

        private void PlayAppear()
        {
            StopAnimation();
            _animationCts = new CancellationTokenSource();
            Tween.To(_appearDuration, t =>
                {
                    _canvasGroup.alpha = t;
                    _panel.localScale = Vector3.one * Mathf.LerpUnclamped(0.85f, 1f, Ease.OutBack(t));
                }, Ease.Linear, _animationCts.Token)
                .SuppressCancellationThrow()
                .Forget();
        }

        private void StopAnimation()
        {
            if (_animationCts == null)
                return;

            _animationCts.Cancel();
            _animationCts.Dispose();
            _animationCts = null;
            _canvasGroup.alpha = 1f;
            _panel.localScale = Vector3.one;
        }

        private void OnDestroy() => StopAnimation();
    }
}
