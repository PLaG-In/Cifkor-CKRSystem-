using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using UnityEngine.Networking;

namespace Weather
{
    public class WeatherView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI weatherText;
        [SerializeField] private RawImage weatherIcon;
        [SerializeField] private GameObject loadingIndicator;
        [SerializeField] private GameObject errorPanel;
        [SerializeField] private TextMeshProUGUI errorText;

        private string _loadedIconUrl;
        
        private CancellationTokenSource _iconCts;
        
        public void ShowLoading()
        {
            loadingIndicator.SetActive(true);
            weatherText.gameObject.SetActive(false);
            errorPanel.SetActive(false);
        }

        public void ShowWeather(WeatherPeriod period)
        {
            loadingIndicator.SetActive(false);
            errorPanel.SetActive(false);
            weatherText.gameObject.SetActive(true);
            weatherText.text = $"Сегодня — {period.temperature}{period.temperatureUnit}";

            if (string.IsNullOrEmpty(period.icon))
            {
                weatherIcon.gameObject.SetActive(false);
                _loadedIconUrl = null;
                return;
            }
            
            if (period.icon == _loadedIconUrl)
                return;
            
            _iconCts?.Cancel();
            _iconCts?.Dispose();
            _iconCts = new CancellationTokenSource();

            LoadIconAsync(period.icon, _iconCts.Token).Forget();
        }

        public void ShowError(string message)
        {
            _iconCts?.Cancel();
            _loadedIconUrl = null;
            
            loadingIndicator.SetActive(false);
            weatherText.gameObject.SetActive(false);
            weatherIcon.gameObject.SetActive(false);
            errorPanel.SetActive(true);
            errorText.text = message;
        }
        
        private async UniTaskVoid LoadIconAsync(string url, CancellationToken token)
        {
            try
            {
                using var req = UnityWebRequestTexture.GetTexture(url);
                await req.SendWebRequest().WithCancellation(token);
                
                if (token.IsCancellationRequested) return;

                if (req.result == UnityWebRequest.Result.Success)
                {
                    _loadedIconUrl = url;
                    weatherIcon.texture = DownloadHandlerTexture.GetContent(req);
                    weatherIcon.gameObject.SetActive(true);
                }
                else
                {
                    weatherIcon.gameObject.SetActive(false);
                    _loadedIconUrl = null;
                }
            }
            catch (System.OperationCanceledException)
            {
                Debug.LogWarning($"[WeatherView] LoadIconAsync cancelled: {url}");
            }
        }
        
        private void OnDestroy()
        {
            _iconCts?.Cancel();
            _iconCts?.Dispose();
        }
    }
}
