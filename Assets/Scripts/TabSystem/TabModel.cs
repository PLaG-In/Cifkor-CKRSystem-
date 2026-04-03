using System;
using UniRx;

namespace TabSystem
{
    public enum TabType
    {
        Clicker = 0,
        Weather = 1,
        Dogs = 2
    }

    public class TabModel
    {
        public IReadOnlyReactiveProperty<TabType> ActiveTab => _activeTab;
        private readonly ReactiveProperty<TabType> _activeTab = new(TabType.Clicker);

        public void SwitchTab(TabType tab)
        {
            if (_activeTab.Value == tab) return;
            _activeTab.Value = tab;
        }
    }
}
