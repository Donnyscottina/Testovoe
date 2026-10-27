using System.Threading;
using ClickerTest.Core.Tweening;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using Zenject;

namespace ClickerTest.Clicker.View
{
    public sealed class FlyingCoinView : MonoBehaviour
    {
        [SerializeField] private RectTransform _rectTransform;
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private TMP_Text _amountText;
        [SerializeField] private float _flyDistance = 360f;
        [SerializeField] private float _horizontalSpread = 140f;
        [SerializeField] private float _duration = 0.85f;

        public async UniTask PlayAsync(Vector3 worldStart, int amount, CancellationToken token)
        {
            _amountText.text = $"+{amount}";
            _rectTransform.SetAsLastSibling();
            _rectTransform.position = worldStart;

            var start = _rectTransform.anchoredPosition;
            var end = start + new Vector2(Random.Range(-_horizontalSpread, _horizontalSpread), _flyDistance);

            await Tween.To(_duration, t =>
            {
                _rectTransform.anchoredPosition = Vector2.LerpUnclamped(start, end, Ease.OutCubic(t));
                _rectTransform.localScale = Vector3.one * Mathf.Lerp(0.6f, 1f, Ease.OutBack(Mathf.Clamp01(t * 3f)));
                _canvasGroup.alpha = 1f - Ease.InQuad(t);
            }, Ease.Linear, token);
        }

        public sealed class Pool : MonoMemoryPool<FlyingCoinView>
        {
        }
    }
}
