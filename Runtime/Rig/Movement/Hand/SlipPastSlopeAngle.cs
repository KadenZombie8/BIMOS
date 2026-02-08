using Unity.Collections;
using UnityEngine;

namespace KadenZombie8.BIMOS
{
    /// <summary>
    /// Uses contact modification to make objects lose friction on slopes greater than a specified angle
    /// </summary>
    public class SlipPastSlopeAngle : MonoBehaviour
    {
        [SerializeField]
        [Range(0f, 89f)]
        [Tooltip("The maximum slope angle (in degrees) that can be reached before slipping")]
        private float _maxSlopeAngle = 50f;

        private float _minSlopeDot;

        public float MaxSlopeAngle
        {
            get => _maxSlopeAngle;
            set
            {
                _maxSlopeAngle = Mathf.Clamp(value, 0f, 89f);
                _minSlopeDot = Mathf.Cos((_maxSlopeAngle + 0.001f) * Mathf.Deg2Rad);
            }
        }

        private Collider _collider;

        private int _colliderId;

        private void Awake()
        {
            _collider = GetComponent<Collider>();
            _collider.hasModifiableContacts = true;
            _colliderId = _collider.GetInstanceID();

            MaxSlopeAngle = _maxSlopeAngle;
            _minSlopeDot = Mathf.Cos((MaxSlopeAngle + 0.001f) * Mathf.Deg2Rad);
        }

        private void OnEnable() => Physics.ContactModifyEvent += OnContactModify;

        private void OnDisable() => Physics.ContactModifyEvent -= OnContactModify;

        private void OnContactModify(PhysicsScene scene, NativeArray<ModifiableContactPair> pairs)
        {
            var gravity = Physics.gravity;

            if (gravity.sqrMagnitude == 0f) return;

            var upDirection = -gravity.normalized;
            
            // Should really use a manager to avoid repeat checks for optimisation
            foreach (var pair in pairs)
            {
                if (pair.colliderInstanceID != _colliderId && pair.otherColliderInstanceID != _colliderId) continue;

                var slopeNormal = pair.GetNormal(0);
                if (pair.colliderInstanceID != _colliderId)
                    slopeNormal *= -1f;

                var slopeDot = Vector3.Dot(slopeNormal, upDirection);

                if (slopeDot > _minSlopeDot) continue;

                pair.SetDynamicFriction(0, 0f);
                pair.SetStaticFriction(0, 0f);
            }
        }
    }
}
