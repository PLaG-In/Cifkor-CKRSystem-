using DG.Tweening;
using TMPro;
using UnityEngine;
using Zenject;

namespace Clicker
{
    [RequireComponent(typeof(CanvasGroup))]
    public class CurrencyFlyItem : MonoBehaviour
    {
        private CanvasGroup _canvasGroup;
        private RectTransform _rectTransform;
        private CurrencyFlyPool _pool;
        private Sequence _sequence;

        [Inject]
        public void Construct(CurrencyFlyPool pool)
        {
            _pool = pool;
        }

        private void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            _rectTransform = GetComponent<RectTransform>();
        }
        
        public void Launch(Vector2 startAnchoredPos, ClickerConfig config)
        {
            _rectTransform.anchoredPosition = startAnchoredPos;
            _canvasGroup.alpha = 1f;
            
            _sequence?.Kill();

            float flyDuration = config.currencyFlyDuration;
            float targetY = startAnchoredPos.y + config.currencyFlyHeight;

            _sequence = DOTween.Sequence();

            _sequence.Join(
                _rectTransform
                    .DOAnchorPosY(targetY, flyDuration)
                    .SetEase(Ease.OutCubic)
            );

            _sequence.Join(
                _canvasGroup
                    .DOFade(0f, flyDuration * 0.5f)
                    .SetDelay(flyDuration * 0.5f)
                    .SetEase(Ease.InQuad)
            );

            _sequence.OnComplete(ReturnToPool);
        }

        private void ReturnToPool()
        {
            _pool.Despawn(this);
        }
        
        public void OnDespawned()
        {
            _sequence?.Kill();
            _sequence = null;
        }
    }
}
