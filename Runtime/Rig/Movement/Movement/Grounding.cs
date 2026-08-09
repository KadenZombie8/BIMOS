using System;
using UnityEngine;

namespace KadenZombie8.BIMOS
{
    public class Grounding : MonoBehaviour
    {
        [SerializeField]
        [Range(0f, 89f)]
        [Tooltip("The maximum slope angle (in degrees) that can be stood on")]
        private float _maxSlopeAngle = 50f;

        private float _minSlopeDot;
        private readonly float _minFriction = 0.1f;

        private Rigidbody _rigidbody;

        public float MaxSlopeAngle
        {
            get => _maxSlopeAngle;
            set
            {
                _maxSlopeAngle = Mathf.Clamp(value, 0f, 89f);
                _minSlopeDot = Mathf.Cos((_maxSlopeAngle + 0.001f) * Mathf.Deg2Rad);
            }
        }

        public bool IsGrounded { get; private set; }

        public Vector3 GroundPoint { get; private set; }

        public Component GroundBody { get; private set; }

        private bool _isGrounded;

        public Vector3 GroundNormal { get; private set; }

        public bool IsSlipping { get; private set; }

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            MaxSlopeAngle = _maxSlopeAngle;
        }

        private void FixedUpdate()
        {
            if (!enabled) return;

            IsGrounded = _isGrounded;
            _isGrounded = false;
            IsSlipping = false;
        }

        private void OnCollisionStay(Collision collision)
        {
            if (!enabled || _isGrounded) return;

            IsSlipping = collision.collider.material.staticFriction < _minFriction;
            if (IsSlipping) return;

            var gravity = Physics.gravity;
            if (gravity.sqrMagnitude == 0f) return;

            var upDirection = -gravity.normalized;

            for (int i = 0; i < collision.contactCount; i++)
            {
                var contactPoint = collision.GetContact(i);
                var groundNormal = contactPoint.normal;
                var slopeDot = Vector3.Dot(groundNormal, upDirection);

                IsSlipping = slopeDot < _minSlopeDot;
                if (IsSlipping) continue;

                _isGrounded = true;
                GroundPoint = contactPoint.point;
                GroundNormal = groundNormal;
                GroundBody = collision.body;

                Vector3 alongPlaneVector = Vector3.Cross(groundNormal, upDirection);
                Vector3 upPlaneVector = Vector3.Cross(alongPlaneVector, groundNormal);

                var impulse = contactPoint.impulse;
                var counterImpulse = impulse.magnitude / slopeDot * upPlaneVector;

                _rigidbody.AddForce(counterImpulse, ForceMode.Impulse);
                BodyUtilities.AddForceAtPosition(collision.body, -counterImpulse, contactPoint.point, ForceMode.Impulse);

                return;
            }
        }
    }
}
