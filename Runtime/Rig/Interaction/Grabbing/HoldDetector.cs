using System;
using System.Collections.Generic;
using System.Linq;
using KadenZombie8.BIMOS.Sockets;
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
        public UnityEvent OnFirstGrab;
        public UnityEvent OnLastRelease;

        private readonly HashSet<Grabbable> _grabbables = new();
        private Item _item;
        private bool _wasHolding;

        private void Awake() => _item = GetComponent<Item>();

        private void AddGrabbable(Grabbable grabbable)
        {
            _grabbables.Add(grabbable);
            grabbable.OnGrab.AddListener(CheckIsHolding);
            grabbable.OnRelease.AddListener(CheckIsHolding);
        }

        private void RemoveGrabbable(Grabbable grabbable)
        {
            _grabbables.Remove(grabbable);
            grabbable.OnGrab.RemoveListener(CheckIsHolding);
            grabbable.OnRelease.RemoveListener(CheckIsHolding);
        }

        private void OnEnable()
        {
            foreach (GameObject gameObject in _item.GameObjects)
            {
                foreach (var grabbable in gameObject.GetComponentsInChildren<Grabbable>())
                    AddGrabbable(grabbable);
            }

            _item.OnGameObjectAdded += GameObjectAdded;
            _item.OnGameObjectRemoved += GameObjectRemoved;
        }

        private void OnDisable()
        {
            foreach (var grabbable in _grabbables.ToArray())
                RemoveGrabbable(grabbable);

            _item.OnGameObjectAdded -= GameObjectAdded;
            _item.OnGameObjectRemoved -= GameObjectRemoved;
        }

        private void GameObjectAdded(GameObject gameObject)
        {
            foreach (var grabbable in gameObject.GetComponentsInChildren<Grabbable>())
                AddGrabbable(grabbable);
        }

        private void GameObjectRemoved(GameObject gameObject)
        {
            foreach (var grabbable in gameObject.GetComponentsInChildren<Grabbable>())
                RemoveGrabbable(grabbable);
        }

        private void CheckIsHolding()
        {
            var isHolding = IsHolding();
            if (isHolding == _wasHolding) return;

            (isHolding ? OnFirstGrab : OnLastRelease)?.Invoke();
            _wasHolding = isHolding;
        }

        private bool IsHolding()
        {
            foreach (var grabbable in _grabbables)
                if (grabbable.LeftHand != grabbable.RightHand) return true;

            return false;
        }
    }
}
