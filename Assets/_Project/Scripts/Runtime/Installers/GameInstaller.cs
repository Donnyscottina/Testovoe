using ClickerTest.Breeds;
using ClickerTest.Clicker;
using ClickerTest.Clicker.Model;
using ClickerTest.Clicker.View;
using ClickerTest.Config;
using ClickerTest.Core.Network;
using ClickerTest.Navigation;
using ClickerTest.Popups;
using ClickerTest.Weather;
using UnityEngine;
using Zenject;

namespace ClickerTest.Installers
{
    public sealed class GameInstaller : MonoInstaller
    {
        [Header("Configs")]
        [SerializeField] private ClickerConfig _clickerConfig;
        [SerializeField] private NetworkConfig _networkConfig;

        [Header("Views")]
        [SerializeField] private TabBarView _tabBarView;
        [SerializeField] private ClickerView _clickerView;
        [SerializeField] private WeatherView _weatherView;
        [SerializeField] private BreedsView _breedsView;
        [SerializeField] private InfoPopupView _infoPopup;

        [Header("Pools")]
        [SerializeField] private FlyingCoinView _flyingCoinPrefab;
        [SerializeField] private ClickBurstVfx _clickBurstPrefab;
        [SerializeField] private RectTransform _clickerVfxRoot;
        [SerializeField] private BreedItemView _breedItemPrefab;
        [SerializeField] private RectTransform _breedListContent;

        public override void InstallBindings()
        {
            Container.BindInstance(_clickerConfig);
            Container.BindInstance(_networkConfig);

            BindNetwork();
            BindClicker();
            BindPools();
            BindViews();
            BindPresenters();
        }

        private void BindNetwork()
        {
            Container.Bind<IHttpClient>().To<UnityWebRequestHttpClient>().AsSingle();
            Container.BindInterfacesTo<RequestQueue>().AsSingle();
            Container.BindInterfacesAndSelfTo<WeatherService>().AsSingle();
            Container.Bind<DogApiService>().AsSingle();
        }

        private void BindClicker()
        {
            Container.Bind<CurrencyModel>().AsSingle();
            Container.Bind<EnergyModel>().AsSingle();
            Container.BindInterfacesAndSelfTo<ClickerModel>().AsSingle();
            Container.BindInterfacesTo<AutoCollector>().AsSingle();
            Container.BindInterfacesTo<EnergyRegenerator>().AsSingle();
        }

        private void BindPools()
        {
            Container.BindMemoryPool<FlyingCoinView, FlyingCoinView.Pool>()
                .WithInitialSize(8)
                .FromComponentInNewPrefab(_flyingCoinPrefab)
                .UnderTransform(_clickerVfxRoot);

            Container.BindMemoryPool<ClickBurstVfx, ClickBurstVfx.Pool>()
                .WithInitialSize(4)
                .FromComponentInNewPrefab(_clickBurstPrefab)
                .UnderTransform(_clickerVfxRoot);

            Container.BindMemoryPool<BreedItemView, BreedItemView.Pool>()
                .WithInitialSize(10)
                .FromComponentInNewPrefab(_breedItemPrefab)
                .UnderTransform(_breedListContent);
        }

        private void BindViews()
        {
            Container.BindInstance(_tabBarView);
            Container.BindInstance(_clickerView);
            Container.BindInstance(_weatherView);
            Container.BindInstance(_breedsView);
            Container.Bind<IInfoPopup>().FromInstance(_infoPopup);
        }

        private void BindPresenters()
        {
            Container.BindInterfacesTo<ClickerPresenter>().AsSingle();
            Container.BindInterfacesTo<WeatherPresenter>().AsSingle();
            Container.BindInterfacesTo<BreedsPresenter>().AsSingle();

            Container.BindInterfacesTo<TabsPresenter>().AsSingle();
            Container.BindExecutionOrder<TabsPresenter>(100);
        }
    }
}
