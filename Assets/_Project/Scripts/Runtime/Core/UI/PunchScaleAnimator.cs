using System.Threading;
using ClickerTest.Core.Tweening;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace ClickerTest.Core.UI
{
    public sealed class PunchScaleAnimator : MonoBehaviour
    {
        [SerializeField] private float _pressedScale = 0.88f;
        [SerializeField] private float _duration = 0.28f;
        [SerializeField, Range(0.05f, 0.9f)] private float _squeezePart = 0.3f;

        private CancellationTokenSource _cts;
        private Vector3 _baseScale = Vector3.one;

        private void Awake()
        {
            _baseScale = transform.localScale;
        }

        private void OnDisable()
        {
            Stop();
            transform.localScale = _baseScale;
        }

        public void Play()
        {
            Stop();
            _cts = new CancellationTokenSource();
            PlayAsync(_cts.Token).Forget();
        }

        private void Stop()
        {
            if (_cts == null)
                return;

            _cts.Cancel();
            _cts.Dispose();
            _cts = null;
        }

        private async UniTaskVoid PlayAsync(CancellationToken token)
        {
            var squeezeDuration = _duration * _squeezePart;
            var cancelled = await Tween.To(squeezeDuration, t => SetScale(Mathf.Lerp(1f, _pressedScale, t)), Ease.OutQuad, token)
                .SuppressCancellationThrow();
            if (cancelled)
                return;

            await Tween.To(_duration - squeezeDuration, t => SetScale(Mathf.LerpUnclamped(_pressedScale, 1f, t)), Ease.OutBack, token)
                .SuppressCancellationThrow();
        }

        private void SetScale(float factor) => transform.localScale = _baseScale * factor;
    }
}
