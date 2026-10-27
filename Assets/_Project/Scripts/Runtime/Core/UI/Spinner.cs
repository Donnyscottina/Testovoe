using UnityEngine;

namespace ClickerTest.Core.UI
{
    public sealed class Spinner : MonoBehaviour
    {
        [SerializeField] private float _degreesPerSecond = 360f;

        private void Update()
        {
            transform.Rotate(0f, 0f, -_degreesPerSecond * Time.unscaledDeltaTime);
        }
    }
}
