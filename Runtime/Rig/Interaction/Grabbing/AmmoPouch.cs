using System.Collections.Generic;
using KadenZombie8.BIMOS.Sockets;
using UnityEngine;

namespace KadenZombie8.BIMOS.Rig
{
    public class AmmoPouch : ItemSlot
    {
        public static AmmoPouch Instance;

        public GameObject AmmoPrefab { get; private set; }

        [SerializeField]
        private int _maxSpawnedMagazines = 5;

        private readonly List<GameObject> _spawnedMagazines = new();

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void Start() => SetItemSlotGrabbablesEnabled(false);

        public void SetAmmoPrefab(GameObject ammoPrefab)
        {
            SetItemSlotGrabbablesEnabled(true);
            AmmoPrefab = ammoPrefab;
        }

        public override void StoreItem(Storable storable)
        {
            base.StoreItem(storable);
            _spawnedMagazines.Remove(storable.gameObject);
            DestroyStoredItem();
        }

        public override void RetrieveItem(SnapGrabbable grabbable)
        {
            if (AmmoPrefab == null)
                return;

            var magazine = Instantiate(AmmoPrefab);
            _spawnedMagazines.Add(magazine);
            StoredStorable = magazine.GetComponent<Storable>();

            List<GameObject> ejectedMagazines = new();
            foreach (var spawnedMagazine in _spawnedMagazines)
            {
                var plug = spawnedMagazine.GetComponentInChildren<Plug>();
                if (!plug) continue;
                if (!plug.Socket) ejectedMagazines.Add(spawnedMagazine);
            }

            while (ejectedMagazines.Count > _maxSpawnedMagazines)
            {
                var magazineToRemove = ejectedMagazines[0];
                ejectedMagazines.Remove(magazineToRemove);

                _spawnedMagazines.Remove(magazineToRemove);
                Destroy(magazineToRemove);
            }

            base.RetrieveItem(grabbable);
        }
    }
}
