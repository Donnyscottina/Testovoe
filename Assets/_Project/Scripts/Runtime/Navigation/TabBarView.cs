using System;
using System.Linq;
using UniRx;
using UnityEngine;

namespace ClickerTest.Navigation
{
    public sealed class TabBarView : MonoBehaviour
    {
        [SerializeField] private TabButtonView[] _buttons;

        public IObservable<TabId> TabSelected =>
            _buttons.Select(button => button.Clicked.Select(_ => button.Id)).Merge();

        public void SetSelected(TabId id)
        {
            foreach (var button in _buttons)
                button.SetSelected(button.Id == id);
        }
    }
}
