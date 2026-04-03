using UnityEngine;

namespace Clicker
{
    [CreateAssetMenu(fileName = "ClickerConfig", menuName = "Config/ClickerConfig")]
    public class ClickerConfig : ScriptableObject
    {
        [Header("Currency")]
        public int currencyPerClick = 1;
        public int autoCollectAmount = 1;
        public float autoCollectInterval = 3f;

        [Header("Energy")]
        public int maxEnergy = 1000;
        public int startEnergy = 1000;
        public int energyCostPerClick = 1;
        public int energyCostPerAutoCollect = 1;
        public int energyRechargeAmount = 10;
        public float energyRechargeInterval = 10f;

        [Header("VFX")]
        public float currencyFlyDuration = 0.8f;
        public float currencyFlyHeight = 300f;
        public int particleCount = 8;
    }
}
