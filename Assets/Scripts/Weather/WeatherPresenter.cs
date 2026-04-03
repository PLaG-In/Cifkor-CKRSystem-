using System;
using Core;
using TabSystem;
using UniRx;
using UnityEngine;
using Zenject;

namespace Weather
{
    public class WeatherPresenter : IInitializable, IDisposable
    {
        private const string API_URL = "https://api.weather.gov/gridpoints/TOP/32,81/forecast";
        private const string REQUEST_TAG = "weather";
        private const float POLL_INTERVAL = 5f;

        private readonly WeatherModel _model;
        private readonly WeatherView _view;
        private readonly RequestQueue _queue;
        private readonly TabModel _tabModel;
        private readonly CompositeDisposable _disposables = new();

        private IDisposable _pollTimer;
        private bool _isActive;

        public WeatherPresenter(
            WeatherModel model,
            WeatherView view,
            RequestQueue queue,
            TabModel tabModel)
        {
            _model = model;
            _view = view;
            _queue = queue;
            _tabModel = tabModel;
        }

        public void Initialize()
        {
            _tabModel.ActiveTab
                .Subscribe(OnTabChanged)
                .AddTo(_disposables);
            
            _model.IsLoading.Subscribe(loading =>
            {
                if (loading) _view.ShowLoading();
            }).AddTo(_disposables);

            _model.CurrentWeather.Subscribe(period =>
            {
                if (period != null) _view.ShowWeather(period);
            }).AddTo(_disposables);

            _model.Error.Subscribe(err =>
            {
                if (!string.IsNullOrEmpty(err)) _view.ShowError(err);
            }).AddTo(_disposables);
        }

        private void OnTabChanged(TabType tab)
        {
            if (tab == TabType.Weather)
            {
                _isActive = true;
                StartPolling();
            }
            else
            {
                _isActive = false;
                StopPolling();
                _queue.CancelByTag(REQUEST_TAG);
            }
        }

        private void StartPolling()
        {
            StopPolling();
            
            EnqueueWeatherRequest();
            
            _pollTimer = Observable.Interval(TimeSpan.FromSeconds(POLL_INTERVAL))
                .Subscribe(_ =>
                {
                    if (_isActive) EnqueueWeatherRequest();
                });
        }

        private void StopPolling()
        {
            _pollTimer?.Dispose();
            _pollTimer = null;
        }

        private void EnqueueWeatherRequest()
        {
            _model.SetLoading(true);

            var request = new WebRequest<WeatherResponse>(
                id: $"{REQUEST_TAG}_{DateTime.UtcNow.Ticks}",
                url: API_URL,
                deserializer: JsonUtility.FromJson<WeatherResponse>,
                onSuccess: data =>
                {
                    var period = data?.properties?.periods?[0];
                    if (period != null)
                    {
                        _model.SetWeather(period);
                    }
                    else
                    {
                        _model.SetError("Нет данных о погоде");
                    }
                },
                onError: err => _model.SetError(err)
            );

            _queue.Enqueue(request);
        }

        public void Dispose()
        {
            StopPolling();
            _disposables?.Dispose();
        }
    }
}
