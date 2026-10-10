using KadenZombie8.BIMOS.Rig.Grips;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace KadenZombie8.BIMOS.Rig
{
    /// <summary>
    /// Detects when an object (through multiple grabs) is first held or last released.
    /// </summary>
    [RequireComponent(typeof(Item))]
    public class HoldDetector : MonoBehaviour
    {
        public UnityEvent<Hand> OnFirstGrab;
        public UnityEvent<Hand> OnLastRelease;

        private readonly HashSet<Grip> _grips = new();
        private Item _item;
        private bool _wasHolding;

        private void Awake() => _item = GetComponent<Item>();

        private void AddGrip(Grip grip)
        {
            _grips.Add(grip);
            grip.OnGrab?.AddListener(CheckIsHolding);
            grip.OnRelease?.AddListener(CheckIsHolding);
        }

        private void RemoveGrip(Grip grip)
        {
            _grips.Remove(grip);
            grip.OnGrab?.RemoveListener(CheckIsHolding);
            grip.OnRelease?.RemoveListener(CheckIsHolding);
        }

        private void OnEnable()
        {
            foreach (var gameObject in _item.GameObjects)
            {
                foreach (var grip in gameObject.GetComponentsInChildren<Grip>())
                    AddGrip(grip);
            }

            _item.OnGameObjectAdded += GameObjectAdded;
            _item.OnGameObjectRemoved += GameObjectRemoved;
        }

        private void OnDisable()
        {
            foreach (var grip in _grips.ToArray())
                RemoveGrip(grip);

            _item.OnGameObjectAdded -= GameObjectAdded;
            _item.OnGameObjectRemoved -= GameObjectRemoved;
        }

        private void GameObjectAdded(GameObject gameObject)
        {
            foreach (var grip in gameObject.GetComponentsInChildren<Grip>())
                AddGrip(grip);
        }

        private void GameObjectRemoved(GameObject gameObject)
        {
            foreach (var grip in gameObject.GetComponentsInChildren<Grip>())
                RemoveGrip(grip);
        }

        private void CheckIsHolding(Hand hand)
        {
            var isHolding = IsHolding();
            if (isHolding == _wasHolding) return;

            (isHolding ? OnFirstGrab : OnLastRelease)?.Invoke(hand);
            _wasHolding = isHolding;
        }

        public bool IsHolding()
        {
            foreach (var grip in _grips)
                if (grip.LeftHand != grip.RightHand) return true;

            return false;
        }
    }
}
