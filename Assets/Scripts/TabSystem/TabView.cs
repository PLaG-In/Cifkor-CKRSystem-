using System;
using UnityEngine;
using UnityEngine.UI;

namespace TabSystem
{
    public class TabView : MonoBehaviour
    {
        [Header("Tab Panels")]
        [SerializeField] private GameObject clickerPanel;
        [SerializeField] private GameObject weatherPanel;
        [SerializeField] private GameObject dogsPanel;

        [Header("Nav Buttons")]
        [SerializeField] private Button clickerButton;
        [SerializeField] private Button weatherButton;
        [SerializeField] private Button dogsButton;

        [Header("Button Highlight Colors")]
        [SerializeField] private Color activeColor = Color.white;
        [SerializeField] private Color inactiveColor = Color.black;

        private Button[] _buttons;
        private GameObject[] _panels;

        public event Action<TabType> OnTabClicked;

        private void Awake()
        {
            _buttons = new[] { clickerButton, weatherButton, dogsButton };
            _panels = new[] { clickerPanel, weatherPanel, dogsPanel };

            clickerButton.onClick.AddListener(() => OnTabClicked?.Invoke(TabType.Clicker));
            weatherButton.onClick.AddListener(() => OnTabClicked?.Invoke(TabType.Weather));
            dogsButton.onClick.AddListener(() => OnTabClicked?.Invoke(TabType.Dogs));
        }

        public void ShowTab(TabType tab)
        {
            int index = (int)tab;
            for (int i = 0; i < _panels.Length; i++)
            {
                _panels[i].SetActive(i == index);
                var img = _buttons[i].GetComponent<Image>();
                if (img != null)
                    img.color = i == index ? activeColor : inactiveColor;
            }
        }
    }
}
