using UnityEngine;

namespace KadenZombie8.BIMOS.Rig
{
    public class InventorySlot : MonoBehaviour
    {
        public string[] Tags = { "Light", "Heavy" };
        public Storable StoredItem;

        [SerializeField]
        private Rigidbody _connectedBody;

        public void StoreItem(Storable item)
        {
            if (!HasMatchingTag(item)) return;

            StoredItem.gameObject.SetActive(false);
            StoredItem = item;
        }

        private bool HasMatchingTag(Storable item)
        {
            foreach (var slotTag in Tags)
                foreach (var itemTag in item.Tags)
                    if (slotTag == itemTag)
                        return true;

            return false;
        }

        public void RetrieveItem()
        {
            StoredItem.gameObject.SetActive(true);
            StoredItem = null;
        }
    }
}
