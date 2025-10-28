using UnityEngine;

namespace KadenZombie8.BIMOS.Rig
{
    /// <summary>
    /// Sends haptic impulses to specified grabs relating to the grabbable
    /// </summary>
    public class GrabbableHapticsHandler : MonoBehaviour
    {
        public Grabbable[] Grabbables;

        /// <summary>
        /// Sends haptic impulses to each of the defined grabs
        /// </summary>
        /// <param name="amplitude">The amplitude of the impulse</param>
        /// <param name="duration">The duration of the impulse</param>
        public void SendHapticImpulse(float amplitude, float duration)
        {
            foreach (Grabbable grabbable in Grabbables) {
                if (grabbable.LeftHand)
                    grabbable.LeftHand.SendHapticImpulse(amplitude, duration);
                if (grabbable.RightHand)
                    grabbable.RightHand.SendHapticImpulse(amplitude, duration);
            }
        }
    }
}
