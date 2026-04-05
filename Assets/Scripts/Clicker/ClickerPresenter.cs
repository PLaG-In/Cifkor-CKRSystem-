using System;
using TabSystem;
using UniRx;
using Zenject;

namespace Clicker
{
    public class ClickerPresenter : IInitializable, IDisposable
    {
        private readonly ClickerModel _model;
        private readonly ClickerView _view;
        private readonly ClickerConfig _config;
        private readonly TabModel _tabModel;
        private readonly CompositeDisposable _disposables = new();

        private bool _isActive;

        public ClickerPresenter(
            ClickerModel model,
            ClickerView view,
            ClickerConfig config,
            TabModel tabModel)
        {
            _model = model;
            _view = view;
            _config = config;
            _tabModel = tabModel;
        }

        public void Initialize()
        {
            _view.OnClickButtonPressed += OnClickPressed;

            _model.Currency
                .Subscribe(v => _view.SetCurrency(v))
                .AddTo(_disposables);

            _model.Energy
                .Subscribe(v => _view.SetEnergy(v, _config.maxEnergy))
                .AddTo(_disposables);
            
            _tabModel.ActiveTab
                .Subscribe(tab => _isActive = tab == TabType.Clicker)
                .AddTo(_disposables);
            
            Observable.Interval(TimeSpan.FromSeconds(_config.autoCollectInterval))
                .Subscribe(_ => { if (_isActive) OnAutoCollect(); })
                .AddTo(_disposables);
            
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
