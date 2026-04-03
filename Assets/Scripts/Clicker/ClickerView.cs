using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Clicker
{
    public class ClickerView : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private Button clickButton;
        [SerializeField] private TextMeshProUGUI currencyText;
        [SerializeField] private TextMeshProUGUI energyText;

        [Header("VFX")]
        [SerializeField] private ParticleSystem clickParticles;

        [Header("Button Animation")]
        [SerializeField] private float clickScaleDown = 0.9f;
        [SerializeField] private float clickScaleDuration = 0.07f;

        [Header("Audio")]
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip clickSound;
        
        private CurrencyFlyPool _flyPool;

        public event Action OnClickButtonPressed;

        [Inject]
        public void Construct(CurrencyFlyPool flyPool)
        {
            _flyPool = flyPool;
        }

        private void Awake()
        {
            clickButton.onClick.AddListener(() => OnClickButtonPressed?.Invoke());
        }

        // ── Currency & Energy display ──────────────────────────────────────────

        public void SetCurrency(int value) =>
            currencyText.text = $"$ {value}";

        public void SetEnergy(int value, int max) =>
            energyText.text = $"E {value} / {max}";

        // ── VFX ───────────────────────────────────────────────────────────────

        public void PlayClickVFX(ClickerConfig config)
        {
            PlayButtonBounce();
            PlayParticles();
            PlayCurrencyFly(config);
            PlaySound();
        }

        private void PlayButtonBounce()
        {
            clickButton.transform
                .DOScale(clickScaleDown, clickScaleDuration)
                .SetEase(Ease.OutQuad)
                .OnComplete(() =>
                    clickButton.transform.DOScale(1f, clickScaleDuration).SetEase(Ease.OutBack));
        }

        private void PlayParticles()
        {
            if (clickParticles != null)
            {
                clickParticles.transform.position = clickButton.transform.position;
                clickParticles.Play();
            }
        }

        private void PlayCurrencyFly(ClickerConfig config)
        {
            if (_flyPool == null) return;
            
            var item = _flyPool.Spawn();
            
            var buttonRT = clickButton.GetComponent<RectTransform>();
            var canvasRT = item.transform.parent as RectTransform;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasRT,
                RectTransformUtility.WorldToScreenPoint(null, buttonRT.position),
                null,
                out var localPoint);

            item.Launch(localPoint, config);
        }

        private void PlaySound()
        {
            if (audioSource != null && clickSound != null)
                audioSource.PlayOneShot(clickSound);
        }
    }
}
