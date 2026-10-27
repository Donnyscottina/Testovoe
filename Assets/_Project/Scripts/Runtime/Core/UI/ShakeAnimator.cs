using System.Threading;
using ClickerTest.Core.Tweening;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace ClickerTest.Core.UI
{
    [RequireComponent(typeof(RectTransform))]
    public sealed class ShakeAnimator : MonoBehaviour
    {
        [SerializeField] private float _amplitude = 18f;
        [SerializeField] private float _frequency = 28f;
        [SerializeField] private float _duration = 0.35f;

        private RectTransform _rectTransform;
        private Vector2 _basePosition;
        private CancellationTokenSource _cts;

        private void Awake()
        {
            _rectTransform = (RectTransform)transform;
        }

        private void OnDisable()
        {
            if (_cts == null)
                return;

            Stop();
            _rectTransform.anchoredPosition = _basePosition;
        }

        public void Play()
        {
            if (_cts == null)
                _basePosition = _rectTransform.anchoredPosition;

            Stop();
            _cts = new CancellationTokenSource();
            Tween.To(_duration, Apply, Ease.Linear, _cts.Token).SuppressCancellationThrow().Forget();
        }

        private void Apply(float t)
        {
            var offset = Mathf.Sin(t * _duration * _frequency) * _amplitude * (1f - t);
            _rectTransform.anchoredPosition = _basePosition + new Vector2(offset, 0f);
        }

        private void Stop()
        {
            if (_cts == null)
                return;

            _cts.Cancel();
            _cts.Dispose();
            _cts = null;
        }
    }
}
