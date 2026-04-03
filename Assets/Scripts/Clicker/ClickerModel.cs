using UniRx;

namespace Clicker
{
    public class ClickerModel
    {
        // ── Reactive state ─────────────────────────────────────────────────────
        public IReadOnlyReactiveProperty<int> Currency => _currency;
        public IReadOnlyReactiveProperty<int> Energy => _energy;

        private readonly ReactiveProperty<int> _currency = new(0);
        private readonly ReactiveProperty<int> _energy;

        private readonly ClickerConfig _config;

        public ClickerModel(ClickerConfig config)
        {
            _config = config;
            _energy = new ReactiveProperty<int>(config.startEnergy);
        }

        // ── Public API ─────────────────────────────────────────────────────────

        /// <returns>true если клик прошёл (есть энергия)</returns>
        public bool TryClick()
        {
            if (_energy.Value < _config.energyCostPerClick) return false;

            _energy.Value -= _config.energyCostPerClick;
            _currency.Value += _config.currencyPerClick;
            return true;
        }

        /// <returns>true если авто-сбор прошёл (есть энергия)</returns>
        public bool TryAutoCollect()
        {
            if (_energy.Value < _config.energyCostPerAutoCollect) return false;

            _energy.Value -= _config.energyCostPerAutoCollect;
            _currency.Value += _config.autoCollectAmount;
            return true;
        }

        public void RechargeEnergy()
        {
            _energy.Value = System.Math.Min(
                _energy.Value + _config.energyRechargeAmount,
                _config.maxEnergy);
        }
    }
}
