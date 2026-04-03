using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Core
{
    public class RequestQueue : IDisposable
    {
        private readonly LinkedList<IRequest> _queue = new LinkedList<IRequest>();
        private readonly CancellationTokenSource _lifetimeCts = new CancellationTokenSource();
        private readonly object _lock = new object();

        private IRequest _current;
        private bool _isRunning;

        public RequestQueue()
        {
            ProcessLoopAsync().Forget();
        }

        // ── Public API ──────────────────────────────────────────────────────────

        public void Enqueue(IRequest request)
        {
            lock (_lock)
            {
                _queue.AddLast(request);
            }
        }
        
        public void CancelRequest(string requestId)
        {
            if (string.IsNullOrEmpty(requestId)) return;

            lock (_lock)
            {
                // Удаляем из очереди
                var node = _queue.First;
                while (node != null)
                {
                    var next = node.Next;
                    if (node.Value.Id == requestId)
                        _queue.Remove(node);
                    node = next;
                }
            }
            
            if (_current?.Id == requestId)
                _current.Cancel();
        }
        
        public void CancelByTag(string tag)
        {
            if (string.IsNullOrEmpty(tag)) return;

            lock (_lock)
            {
                var node = _queue.First;
                while (node != null)
                {
                    var next = node.Next;
                    if (node.Value.Id.Contains(tag))
                    {
                        node.Value.Cancel();
                        _queue.Remove(node);
                    }
                    node = next;
                }
            }

            if (_current?.Id?.Contains(tag) == true)
                _current.Cancel();
        }

        // ── Private ─────────────────────────────────────────────────────────────

        private async UniTaskVoid ProcessLoopAsync()
        {
            while (!_lifetimeCts.Token.IsCancellationRequested)
            {
                IRequest next = null;

                lock (_lock)
                {
                    if (_queue.Count > 0)
                    {
                        next = _queue.First.Value;
                        _queue.RemoveFirst();
                    }
                }

                if (next != null && !next.IsCancelled)
                {
                    _current = next;
                    try
                    {
                        await next.ExecuteAsync(_lifetimeCts.Token);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"[RequestQueue] Unhandled error: {e.Message}");
                    }
                    finally
                    {
                        _current = null;
                    }
                }
                else
                {
                    await UniTask.Yield(PlayerLoopTiming.Update, _lifetimeCts.Token);
                }
            }
        }

        public void Dispose()
        {
            _lifetimeCts?.Cancel();
            _lifetimeCts?.Dispose();
        }
    }
}
