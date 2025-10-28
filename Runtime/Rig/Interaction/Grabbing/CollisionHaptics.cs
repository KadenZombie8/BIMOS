using KadenZombie8.BIMOS.ImpactProperties;
using UnityEngine;

namespace KadenZombie8.BIMOS.Rig
{
    public class CollisionHaptics : MonoBehaviour
    {
        [SerializeField]
        private GrabbableHapticsHandler _hapticsHandler;

        [SerializeField]
        private ImpulseRange _impulseRange = new()
        {
            Minimum = 0.2f,
            Maximum = 20f
        };

        private readonly float _collisionHapticDuration = 0.1f;

        private void OnCollisionEnter(Collision collision)
        {
            var impulseMagnitude = collision.impulse.magnitude;
            if (impulseMagnitude < _impulseRange.Minimum) return;
            var impulsePercent = Mathf.InverseLerp(_impulseRange.Minimum, _impulseRange.Maximum, impulseMagnitude);
            _hapticsHandler.SendHapticImpulse(impulsePercent, _collisionHapticDuration);
        }
    }
}
