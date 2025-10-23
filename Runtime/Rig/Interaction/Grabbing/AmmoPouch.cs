using KadenZombie8.BIMOS.Rig;
using KadenZombie8.BIMOS.Sockets;
using System.Collections.Generic;
using UnityEngine;

namespace KadenZombie8.BIMOS.Guns
{
    public class AmmoPouch : Grabbable
    {
        public static AmmoPouch Instance;

        public GameObject AmmoPrefab;
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

        public override void Grab(Hand hand) //Triggered when player grabs the grab
        {
            if (AmmoPrefab == null)
                return;

            Release(hand);
            GameObject magazine = Instantiate(AmmoPrefab);
            magazine.transform.SetPositionAndRotation(hand.PhysicsHandTransform.position, hand.PhysicsHandTransform.rotation);

            foreach (var grabbable in magazine.GetComponentsInChildren<SnapGrabbable>())
                if (grabbable.Handedness == hand.Handedness)
                {
                    grabbable.Grab(hand);
                    break;
                }

            _spawnedMagazines.Add(magazine);

            int ejectedMagazineCount = 0;
            foreach (GameObject spawnedMagazine in _spawnedMagazines)
                if (!spawnedMagazine.GetComponentInChildren<Attacher>()?.Socket)
                    ejectedMagazineCount++;

            if (ejectedMagazineCount > 5)
                foreach (GameObject spawnedMagazine in _spawnedMagazines)
                    if (!spawnedMagazine.GetComponentInChildren<Attacher>().Socket)
                    {
                        _spawnedMagazines.Remove(spawnedMagazine);
                        Destroy(spawnedMagazine);
                        break;
                    }
        }
    }
}
