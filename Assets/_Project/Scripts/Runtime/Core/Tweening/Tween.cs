using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace ClickerTest.Core.Tweening
{
    public static class Tween
    {
        public static async UniTask To(float duration, Action<float> onUpdate, Func<float, float> ease, CancellationToken cancellationToken)
        {
            ease ??= Ease.Linear;
            var elapsed = 0f;
            onUpdate(ease(0f));

            while (elapsed < duration)
            {
                await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);
                elapsed += Time.unscaledDeltaTime;
                onUpdate(ease(Mathf.Clamp01(elapsed / duration)));
            }
        }
    }

    public static class Ease
    {
        public static float Linear(float t) => t;
        public static float InQuad(float t) => t * t;
        public static float OutQuad(float t) => 1f - (1f - t) * (1f - t);
        public static float OutCubic(float t) => 1f - Mathf.Pow(1f - t, 3f);

        public static float OutBack(float t)
        {
            const float c1 = 1.70158f;
            const float c3 = c1 + 1f;
            return 1f + c3 * Mathf.Pow(t - 1f, 3f) + c1 * Mathf.Pow(t - 1f, 2f);
        }
    }
}
