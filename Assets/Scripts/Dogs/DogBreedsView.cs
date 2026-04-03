using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Dogs
{
    public class DogBreedsView : MonoBehaviour
    {
        [Header("List")]
        [SerializeField] private Transform listContainer;
        [SerializeField] private BreedItemView breedItemPrefab;
        [SerializeField] private GameObject listLoadingIndicator;

        [Header("Detail Loading")]
        [SerializeField] private GameObject detailLoadingIndicator;

        [Header("Error")]
        [SerializeField] private GameObject errorPanel;
        [SerializeField] private TMPro.TextMeshProUGUI errorText;

        [Header("Popup")]
        [SerializeField] private BreedPopupView popup;

        public event Action<BreedData> OnBreedSelected;

        private readonly List<BreedItemView> _spawnedItems = new();

        public void ShowListLoading()
        {
            listLoadingIndicator.SetActive(true);
            listContainer.gameObject.SetActive(false);
            errorPanel.SetActive(false);
        }

        public void ShowBreeds(List<BreedData> breeds)
        {
            listLoadingIndicator.SetActive(false);
            errorPanel.SetActive(false);
            listContainer.gameObject.SetActive(true);
            
            foreach (var item in _spawnedItems)
            {
                Destroy(item.gameObject);
            }

            _spawnedItems.Clear();
            
            int count = Mathf.Min(breeds.Count, 10);
            for (int i = 0; i < count; i++)
            {
                var item = Instantiate(breedItemPrefab, listContainer);
                item.Setup(i + 1, breeds[i]);
                item.OnBreedClicked += breed => OnBreedSelected?.Invoke(breed);
                _spawnedItems.Add(item);
            }
        }

        public void ShowError(string message)
        {
            listLoadingIndicator.SetActive(false);
            listContainer.gameObject.SetActive(false);
            errorPanel.SetActive(true);
            errorText.text = message;
        }

        public void ShowDetailLoading()
        {
            detailLoadingIndicator.SetActive(true);
        }

        public void HideDetailLoading()
        {
            detailLoadingIndicator.SetActive(false);
        }
        
        public void ShowBreedPopup(BreedData breed)
        {
            HideDetailLoading();
            popup.Show(breed);
        }
    }
}
