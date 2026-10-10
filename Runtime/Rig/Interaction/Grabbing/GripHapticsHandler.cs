using KadenZombie8.BIMOS.Rig.Grips;
using UnityEngine;

namespace KadenZombie8.BIMOS.Rig
{
    /// <summary>
    /// Sends haptic impulses to specified grabs relating to the grip
    /// </summary>
    public class GripHapticsHandler : MonoBehaviour
    {
        public Grip[] Grips;

        /// <summary>
        /// Sends haptic impulses to each of the defined grabs
        /// </summary>
        /// <param name="amplitude">The amplitude of the impulse</param>
        /// <param name="duration">The duration of the impulse</param>
        public void SendHapticImpulse(float amplitude, float duration)
        {
            foreach (Grip grip in Grips) {
                if (!grip) return;

                if (grip.LeftHand)
                    grip.LeftHand.SendHapticImpulse(amplitude, duration);

                if (grip.RightHand)
                    grip.RightHand.SendHapticImpulse(amplitude, duration);
            }
        }
    }
}
