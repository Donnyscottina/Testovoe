using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace ClickerTest.Breeds
{
    public sealed class BreedItemView : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private TMP_Text _label;
        [SerializeField] private GameObject _loader;

        private int _index;
        private Action<int> _onClick;

        private void Awake()
        {
            _button.onClick.AddListener(() => _onClick?.Invoke(_index));
        }

        public void Setup(int index, string text, Action<int> onClick)
        {
            _index = index;
            _label.text = text;
            _onClick = onClick;
            SetLoading(false);
        }

        public void SetLoading(bool isLoading) => _loader.SetActive(isLoading);

        public sealed class Pool : MonoMemoryPool<BreedItemView>
        {
            protected override void OnDespawned(BreedItemView item)
            {
                item._onClick = null;
                item.SetLoading(false);
                base.OnDespawned(item);
            }
        }
    }
}
