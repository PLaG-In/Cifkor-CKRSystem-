using Zenject;

namespace Clicker
{
    public class CurrencyFlyPool : MonoMemoryPool<CurrencyFlyItem>
    {
        protected override void OnSpawned(CurrencyFlyItem item)
        {
            base.OnSpawned(item);
            item.gameObject.SetActive(true);
        }

        protected override void OnDespawned(CurrencyFlyItem item)
        {
            item.OnDespawned();
            item.gameObject.SetActive(false);
            base.OnDespawned(item);
        }
    }
}
