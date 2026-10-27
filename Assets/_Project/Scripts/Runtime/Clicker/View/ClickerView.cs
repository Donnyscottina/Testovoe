using System;
using System.Collections.Generic;
using System.Threading;
using ClickerTest.Core.UI;
using Cysharp.Threading.Tasks;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace ClickerTest.Clicker.View
{
    public sealed class ClickerView : MonoBehaviour
    {
        [Header("Button")]
        [SerializeField] private Button _clickButton;
        [SerializeField] private PunchScaleAnimator _buttonPunch;
        [SerializeField] private RectTransform _vfxSpawnPoint;

        [Header("Counters")]
        [SerializeField] private TMP_Text _currencyText;
        [SerializeField] private PunchScaleAnimator _currencyPunch;
        [SerializeField] private TMP_Text _energyText;
        [SerializeField] private Image _energyFill;
        [SerializeField] private ShakeAnimator _energyShake;
        [SerializeField] private Color _energyTextColor = Color.white;
        [SerializeField] private Color _noEnergyColor = new Color(1f, 0.35f, 0.35f);

        [Header("Sound")]
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private AudioClip _clickClip;
        [SerializeField] private Vector2 _pitchRange = new Vector2(0.95f, 1.12f);

        private readonly List<ClickBurstVfx> _activeBursts = new List<ClickBurstVfx>();
        private CancellationTokenSource _effectsCts = new CancellationTokenSource();
        private FlyingCoinView.Pool _coinPool;
        private ClickBurstVfx.Pool _burstPool;
        private bool _isDestroyed;

        [Inject]
        public void Construct(FlyingCoinView.Pool coinPool, ClickBurstVfx.Pool burstPool)
        {
            _coinPool = coinPool;
            _burstPool = burstPool;
        }

        public IObservable<Unit> Clicked => _clickButton.OnClickAsObservable();

        public void SetVisible(bool visible)
        {
            if (!visible)
                StopEffects();

            gameObject.SetActive(visible);
        }

        public void SetCurrency(int amount) => _currencyText.text = amount.ToString();

        public void SetEnergy(int current, int max)
        {
            _energyText.text = $"{current} / {max}";
            _energyText.color = current > 0 ? _energyTextColor : _noEnergyColor;
            _energyFill.fillAmount = max > 0 ? (float)current / max : 0f;
        }

        public void PlayCollectFeedback(int reward)
        {
            if (!isActiveAndEnabled)
                return;

            _buttonPunch.Play();
            _currencyPunch.Play();
            SpawnBurst();
            FlyCoinAsync(reward, _effectsCts.Token).Forget();
            PlayClickSound();
        }

        public void PlayNoEnergyFeedback()
        {
            if (!isActiveAndEnabled)
                return;

            _energyShake.Play();
        }

        private void SpawnBurst()
        {
            var burst = _burstPool.Spawn();
            _activeBursts.Add(burst);
            burst.Play(_vfxSpawnPoint.position, finished =>
            {
                _activeBursts.Remove(finished);
                _burstPool.Despawn(finished);
            });
        }

        private async UniTaskVoid FlyCoinAsync(int reward, CancellationToken token)
        {
            var coin = _coinPool.Spawn();
            await coin.PlayAsync(_vfxSpawnPoint.position, reward, token).SuppressCancellationThrow();

            if (!_isDestroyed)
                _coinPool.Despawn(coin);
        }

        private void PlayClickSound()
        {
            if (_audioSource == null || _clickClip == null)
                return;

            _audioSource.pitch = UnityEngine.Random.Range(_pitchRange.x, _pitchRange.y);
            _audioSource.PlayOneShot(_clickClip);
        }

        private void StopEffects()
        {
            _effectsCts.Cancel();
            _effectsCts.Dispose();
            _effectsCts = new CancellationTokenSource();

            foreach (var burst in _activeBursts)
                _burstPool.Despawn(burst);
            _activeBursts.Clear();
        }

        private void OnDestroy()
        {
            _isDestroyed = true;
            _effectsCts.Cancel();
            _effectsCts.Dispose();
        }
    }
}
