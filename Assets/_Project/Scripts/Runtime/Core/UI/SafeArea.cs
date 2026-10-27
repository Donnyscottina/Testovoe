using UnityEngine;

namespace ClickerTest.Core.UI
{
    [RequireComponent(typeof(RectTransform))]
    public sealed class SafeArea : MonoBehaviour
    {
        private RectTransform _rectTransform;
        private Rect _appliedArea;
        private Vector2Int _appliedScreen;

        private void Awake()
        {
            _rectTransform = (RectTransform)transform;
            Apply();
        }

        private void Update()
        {
            if (_appliedArea != Screen.safeArea || _appliedScreen.x != Screen.width || _appliedScreen.y != Screen.height)
                Apply();
        }

        private void Apply()
        {
            var area = Screen.safeArea;
            _appliedArea = area;
            _appliedScreen = new Vector2Int(Screen.width, Screen.height);

            if (Screen.width <= 0 || Screen.height <= 0)
                return;

            _rectTransform.anchorMin = new Vector2(area.xMin / Screen.width, area.yMin / Screen.height);
            _rectTransform.anchorMax = new Vector2(area.xMax / Screen.width, area.yMax / Screen.height);
            _rectTransform.offsetMin = Vector2.zero;
            _rectTransform.offsetMax = Vector2.zero;
        }
    }
}
