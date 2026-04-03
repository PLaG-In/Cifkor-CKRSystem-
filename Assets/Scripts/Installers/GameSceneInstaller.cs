using Clicker;
using Core;
using Dogs;
using TabSystem;
using UnityEngine;
using Weather;
using Zenject;

namespace Installers
{
    public class GameSceneInstaller : MonoInstaller
    {
        [Header("Config")]
        [SerializeField] private ClickerConfig clickerConfig;

        [Header("Views")]
        [SerializeField] private TabView tabView;
        [SerializeField] private ClickerView clickerView;
        [SerializeField] private WeatherView weatherView;
        [SerializeField] private DogBreedsView dogBreedsView;

        [Header("Clicker Pool")]
        [SerializeField] private CurrencyFlyItem currencyFlyPrefab;
        [SerializeField] private RectTransform currencyFlyPoolParent;
        [SerializeField] private int currencyFlyPoolInitialSize = 10;

        public override void InstallBindings()
        {
            // Core
            Container.Bind<RequestQueue>().AsSingle().NonLazy();

            // Tab system
            Container.Bind<TabModel>().AsSingle();
            Container.BindInstance(tabView).AsSingle();
            Container.BindInterfacesTo<TabPresenter>().AsSingle().NonLazy();

            // Clicker
            Container.BindInstance(clickerConfig).AsSingle();
            Container.Bind<ClickerModel>().AsSingle().WithArguments(clickerConfig);
            Container.BindInstance(clickerView).AsSingle();

            // Currency fly pool
            Container.BindMemoryPool<CurrencyFlyItem, CurrencyFlyPool>()
                .WithInitialSize(currencyFlyPoolInitialSize)
                .FromComponentInNewPrefab(currencyFlyPrefab)
                .UnderTransform(currencyFlyPoolParent);

            Container.BindInterfacesTo<ClickerPresenter>().AsSingle().NonLazy();

            // Weather
            Container.Bind<WeatherModel>().AsSingle();
            Container.BindInstance(weatherView).AsSingle();
            Container.BindInterfacesTo<WeatherPresenter>().AsSingle().NonLazy();

            // Dogs
            Container.Bind<DogBreedsModel>().AsSingle();
            Container.BindInstance(dogBreedsView).AsSingle();
            Container.BindInterfacesTo<DogBreedsPresenter>().AsSingle().NonLazy();
        }
    }
}
