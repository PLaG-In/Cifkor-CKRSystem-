using UniRx;

namespace Weather
{
    public class WeatherModel
    {
        public IReadOnlyReactiveProperty<WeatherPeriod> CurrentWeather => _currentWeather;
        public IReadOnlyReactiveProperty<bool> IsLoading => _isLoading;
        public IReadOnlyReactiveProperty<string> Error => _error;

        private readonly ReactiveProperty<WeatherPeriod> _currentWeather = new();
        private readonly ReactiveProperty<bool> _isLoading = new(false);
        private readonly ReactiveProperty<string> _error = new();

        public void SetLoading(bool loading) => _isLoading.Value = loading;
        public void SetWeather(WeatherPeriod period)
        {
            _currentWeather.Value = period;
            _isLoading.Value = false;
            _error.Value = null;
        }
        public void SetError(string error)
        {
            _error.Value = error;
            _isLoading.Value = false;
        }
    }
}
