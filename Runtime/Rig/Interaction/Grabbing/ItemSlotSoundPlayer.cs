using KadenZombie8.BIMOS.Audio;
using UnityEngine;
using UnityEngine.Audio;

namespace KadenZombie8.BIMOS.Rig
{
    [RequireComponent(typeof(ItemSlot))]
    public class ItemSlotSoundPlayer : SoundPlayer
    {
        [SerializeField]
        private AudioResource
            _storeSound,
            _retrieveSound;

        private ItemSlot _itemSlot;

        protected override void Awake()
        {
            base.Awake();
            _itemSlot = GetComponent<ItemSlot>();
        }

        private void OnEnable()
        {
            _itemSlot.OnStore += Stored;
            _itemSlot.OnRetrieve += Retrieved;
        }

        private void OnDisable()
        {
            _itemSlot.OnStore -= Stored;
            _itemSlot.OnRetrieve -= Retrieved;
        }

        private void Stored() => Play(_storeSound);

        private void Retrieved() => Play(_retrieveSound);
    }
}
