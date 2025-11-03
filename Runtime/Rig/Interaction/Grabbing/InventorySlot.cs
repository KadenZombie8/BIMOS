using UnityEngine;

namespace KadenZombie8.BIMOS.Rig
{
    public class InventorySlot : MonoBehaviour
    {
        public Storable StoredItem;

        public void StoreItem(Storable item)
        {
            StoredItem.gameObject.SetActive(false);
            StoredItem = item;
        }

        public void RetrieveItem()
        {
            StoredItem.gameObject.SetActive(true);
            StoredItem = null;
        }
    }
}
