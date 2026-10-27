using System;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace ClickerTest.Breeds
{
    public sealed class BreedsView : MonoBehaviour
    {
        [SerializeField] private ScrollRect _scrollRect;
        [SerializeField] private GameObject _listLoader;
        [SerializeField] private TMP_Text _statusText;

        private readonly List<BreedItemView> _items = new List<BreedItemView>();
        private readonly Subject<int> _breedClicked = new Subject<int>();
        private BreedItemView.Pool _itemPool;

        [Inject]
        public void Construct(BreedItemView.Pool itemPool)
        {
            _itemPool = itemPool;
        }

        public IObservable<int> BreedClicked => _breedClicked;

        public void SetVisible(bool visible) => gameObject.SetActive(visible);

        public void ShowListLoading()
        {
            ClearBreeds();
            _statusText.gameObject.SetActive(false);
            _listLoader.SetActive(true);
        }

        public void ShowBreeds(IReadOnlyList<string> names)
        {
            ClearBreeds();
            _listLoader.SetActive(false);
            _statusText.gameObject.SetActive(false);

            for (var i = 0; i < names.Count; i++)
            {
                var item = _itemPool.Spawn();
                item.transform.SetAsLastSibling();
                item.Setup(i, $"{i + 1} - {names[i]}", _breedClicked.OnNext);
                _items.Add(item);
            }

            _scrollRect.verticalNormalizedPosition = 1f;
        }

        public void ShowListError(string message)
        {
            ClearBreeds();
            _listLoader.SetActive(false);
            _statusText.text = message;
            _statusText.gameObject.SetActive(true);
        }

        public void SetBreedLoading(int index, bool isLoading)
        {
            if (index >= 0 && index < _items.Count)
                _items[index].SetLoading(isLoading);
        }

        private void ClearBreeds()
        {
            foreach (var item in _items)
                _itemPool.Despawn(item);
            _items.Clear();
        }

        private void OnDestroy() => _breedClicked.Dispose();
    }
}
