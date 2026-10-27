using System;
using UnityEngine;
using Zenject;

namespace ClickerTest.Clicker.View
{
    [RequireComponent(typeof(ParticleSystem))]
    public sealed class ClickBurstVfx : MonoBehaviour
    {
        [SerializeField] private ParticleSystem _particles;

        private Action<ClickBurstVfx> _onFinished;

        public void Play(Vector3 worldPosition, Action<ClickBurstVfx> onFinished)
        {
            _onFinished = onFinished;
            transform.position = worldPosition;
            _particles.Clear(true);
            _particles.Play(true);
        }

        private void OnParticleSystemStopped()
        {
            var onFinished = _onFinished;
            _onFinished = null;
            onFinished?.Invoke(this);
        }

        public sealed class Pool : MonoMemoryPool<ClickBurstVfx>
        {
            protected override void OnDespawned(ClickBurstVfx item)
            {
                item._onFinished = null;
                item._particles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                base.OnDespawned(item);
            }
        }
    }
}
