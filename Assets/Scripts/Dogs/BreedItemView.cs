using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Dogs
{
    public class BreedItemView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI label;
        [SerializeField] private Button button;
        [SerializeField] private GameObject spinner;

        public event Action<BreedData> OnBreedClicked;

        private BreedData _data;

        public string BreedId => _data?.id;

        public void Setup(int index, BreedData data)
        {
            _data = data;
            label.text = $"{index} — {data.attributes.name}";

            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => OnBreedClicked?.Invoke(_data));
            
            SetLoading(false);
        }
        
        public void SetLoading(bool isLoading)
        {
            if (spinner != null)
                spinner.SetActive(isLoading);
            
            button.interactable = !isLoading;
        }
    }
}
