using System.Collections.Generic;
using UnityEngine;

namespace KadenZombie8.BIMOS.Rig
{
    /// <summary>
    /// Sends haptic impulses to specified grabs relating to the grabbable
    /// </summary>
    public class GrabHapticsHandler : MonoBehaviour
    {
        private HashSet<Grabbable> _grabbables;

        private void Awake()
        {
            _grabbables = new(GetComponentsInChildren<Grabbable>());
            var parent = transform.parent;
            if (!parent) return;
            var parentArticulationBody = parent.GetComponentInParent<ArticulationBody>();
            if (!parentArticulationBody) return;
            foreach (var grabbable in parentArticulationBody.GetComponentsInChildren<Grabbable>())
                _grabbables.Add(grabbable);
        }

        /// <summary>
        /// Sends haptic impulses to each of the defined grabs
        /// </summary>
        /// <param name="amplitude">The amplitude of the impulse</param>
        /// <param name="duration">The duration of the impulse</param>
        public void SendHapticImpulse(float amplitude, float duration)
        {
            foreach (Grabbable grab in _grabbables) {
                if (grab.LeftHand)
                    grab.LeftHand.SendHapticImpulse(amplitude, duration);
                if (grab.RightHand)
                    grab.RightHand.SendHapticImpulse(amplitude, duration);
            }
        }
    }
}
