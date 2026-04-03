using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Dogs
{
    public class BreedPopupView : MonoBehaviour
    {
        [SerializeField] private GameObject root;
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI descriptionText;
        [SerializeField] private Button okButton;

        private void Awake()
        {
            okButton.onClick.AddListener(Hide);
            root.SetActive(false);
        }

        public void Show(BreedData breed)
        {
            titleText.text = breed.attributes.name;
            descriptionText.text = string.IsNullOrEmpty(breed.attributes.description)
                ? "Описание отсутствует."
                : breed.attributes.description;

            root.SetActive(true);
            
            root.transform.localScale = Vector3.one * 0.8f;
            root.transform.DOScale(1f, 0.25f).SetEase(Ease.OutBack);
        }

        public void Hide()
        {
            root.transform.DOScale(0.8f, 0.15f)
                .SetEase(Ease.InBack)
                .OnComplete(() => root.SetActive(false));
        }
    }
}
