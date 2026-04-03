using System;
using UniRx;
using Zenject;

namespace TabSystem
{
    public class TabPresenter : IInitializable, IDisposable
    {
        private readonly TabModel _model;
        private readonly TabView _view;
        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        public TabPresenter(TabModel model, TabView view)
        {
            _model = model;
            _view = view;
        }

        public void Initialize()
        {
            _view.OnTabClicked += OnTabClicked;

            _model.ActiveTab
                .Subscribe(tab => _view.ShowTab(tab))
                .AddTo(_disposables);
            
            _view.ShowTab(_model.ActiveTab.Value);
        }

        private void OnTabClicked(TabType tab) => _model.SwitchTab(tab);

        public void Dispose()
        {
            _view.OnTabClicked -= OnTabClicked;
            _disposables?.Dispose();
        }
    }
}
