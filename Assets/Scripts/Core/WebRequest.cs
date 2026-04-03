using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace Core
{
    public class WebRequest<T> : IRequest
    {
        public string Id { get; }
        public bool IsCancelled { get; private set; }

        private readonly string _url;
        private readonly Func<string, T> _deserializer;
        private readonly Action<T> _onSuccess;
        private readonly Action<string> _onError;
        private readonly CancellationTokenSource _internalCts;

        public WebRequest(
            string id,
            string url,
            Func<string, T> deserializer,
            Action<T> onSuccess,
            Action<string> onError = null)
        {
            Id = id;
            _url = url;
            _deserializer = deserializer;
            _onSuccess = onSuccess;
            _onError = onError;
            _internalCts = new CancellationTokenSource();
        }

        public void Cancel()
        {
            IsCancelled = true;
            _internalCts?.Cancel();
        }

        public async UniTask ExecuteAsync(CancellationToken externalToken)
        {
            if (IsCancelled) return;

            using var linked = CancellationTokenSource.CreateLinkedTokenSource(
                _internalCts.Token, externalToken);

            try
            {
                using var request = UnityWebRequest.Get(_url);
                await request.SendWebRequest().WithCancellation(linked.Token);

                if (IsCancelled) return;

                if (request.result == UnityWebRequest.Result.Success)
                {
                    var data = _deserializer(request.downloadHandler.text);
                    _onSuccess?.Invoke(data);
                }
                else
                {
                    _onError?.Invoke(request.error);
                }
            }
            catch (OperationCanceledException)
            {
                Debug.LogWarning($"[WebRequest] Request cancelled: {_url}");
            }
            catch (Exception e)
            {
                _onError?.Invoke(e.Message);
            }
        }
    }
}
