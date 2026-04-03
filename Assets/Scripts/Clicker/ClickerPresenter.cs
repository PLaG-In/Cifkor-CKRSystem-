using System;
using UniRx;
using Zenject;

namespace Clicker
{
    public class ClickerPresenter : IInitializable, IDisposable
    {
        private readonly ClickerModel _model;
        private readonly ClickerView _view;
        private readonly ClickerConfig _config;
        private readonly CompositeDisposable _disposables = new();

        public ClickerPresenter(ClickerModel model, ClickerView view, ClickerConfig config)
        {
            _model = model;
            _view = view;
            _config = config;
        }

        public void Initialize()
        {
            // Подписка на кнопку
            _view.OnClickButtonPressed += OnClickPressed;

            // Отображение начальных значений
            _model.Currency.Subscribe(v => _view.SetCurrency(v)).AddTo(_disposables);
            _model.Energy.Subscribe(v => _view.SetEnergy(v, _config.maxEnergy)).AddTo(_disposables);

            // Авто-сбор каждые N секунд
            Observable.Interval(TimeSpan.FromSeconds(_config.autoCollectInterval))
                .Subscribe(_ => OnAutoCollect())
                .AddTo(_disposables);

            // Восполнение энергии каждые N секунд
            Observable.Interval(TimeSpan.FromSeconds(_config.energyRechargeInterval))
                .Subscribe(_ => _model.RechargeEnergy())
                .AddTo(_disposables);
        }

        private void OnClickPressed()
        {
            if (_model.TryClick())
                _view.PlayClickVFX(_config);
        }

        private void OnAutoCollect()
        {
            if (_model.TryAutoCollect())
                _view.PlayClickVFX(_config);
        }

        public void Dispose()
        {
            _view.OnClickButtonPressed -= OnClickPressed;
            _disposables?.Dispose();
        }
    }
}
