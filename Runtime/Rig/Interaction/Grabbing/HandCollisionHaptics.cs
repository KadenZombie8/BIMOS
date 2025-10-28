using UnityEngine;

namespace KadenZombie8.BIMOS.Rig
{
    public class HandCollisionHaptics : MonoBehaviour
    {
        private Hand _hand;
        private readonly float _collisionHapticDuration = 0.1f;
        private readonly float _minimumImpulseMagnitude = 0.2f;
        private readonly float _maximumImpulseMagnitude = 20f;

        private void Awake() => _hand = GetComponent<Hand>();

        private void OnCollisionEnter(Collision collision)
        {
            var impulseMagnitude = collision.impulse.magnitude;
            if (impulseMagnitude < _minimumImpulseMagnitude) return;
            var impulsePercent = Mathf.InverseLerp(_minimumImpulseMagnitude, _maximumImpulseMagnitude, impulseMagnitude);
            _hand.SendHapticImpulse(impulsePercent, _collisionHapticDuration);
        }
    }
}
