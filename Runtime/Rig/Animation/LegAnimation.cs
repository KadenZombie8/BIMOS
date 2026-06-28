using System;
using System.Collections;
using KadenZombie8.BIMOS.Rig.Movement;
using UnityEngine;

namespace KadenZombie8.BIMOS.Rig.Animation
{
    public class LegAnimation : MonoBehaviour
    {
        private Foot _currentFoot;

        [SerializeField]
        private Foot _leftFoot;

        [SerializeField]
        private Foot _rightFoot;

        [SerializeField]
        private LocomotionSphere _locomotionSphere;

        [SerializeField]
        private Transform _hips;

        [SerializeField]
        private Transform _character;

        [SerializeField]
        private Rigidbody _locomotionSphereRigidbody;

        [SerializeField]
        private SmoothLocomotion _smoothLocomotion;

        private Vector3 _velocity, _groundVelocity;
        private bool _isMoving, _isStepping;
        private float _stepTime = 0.1f, _stepLength = 0.1f, _stepHeight = 0.1f;

        private void Start() => _currentFoot = _rightFoot;

        private void Update()
        {
            _velocity = Vector3.ProjectOnPlane(_locomotionSphereRigidbody.linearVelocity - _groundVelocity, Vector3.up);
            UpdateTarget(_leftFoot);
            UpdateTarget(_rightFoot);
            if (!_locomotionSphere.IsGrounded) //Take air pose if off ground
            {
                SnapFootToTarget(_leftFoot);
                SnapFootToTarget(_rightFoot);
            }
            else
            {
                if ((_currentFoot.Anchor.position - _currentFoot.Target.position).magnitude > _stepLength) //Step if foot far enough from target
                {
                    StartCoroutine(Step());
                }
                if (_velocity.magnitude < 0.1f)
                {
                    if (_isMoving) //Readjust the feet if only just stopped moving
                    {
                        StartCoroutine(Step(true));
                        StartCoroutine(Step(true));
                    }
                    _isMoving = false;
                }
                else
                {
                    _isMoving = true;
                }
            }
        }

        private void UpdateTarget(Foot foot)
        {
            foot.Target.position = _hips.position + _velocity * _stepTime + _character.right * foot.Offset;
            if (Physics.Raycast(
                foot.Target.position,
                Vector3.down,
                out RaycastHit hit,
                1.25f,
                Physics.DefaultRaycastLayers,
                QueryTriggerInteraction.Ignore)
                && _locomotionSphere.IsGrounded)
            {
                foot.IsGrounded = true;
                foot.Target.SetPositionAndRotation(
                    hit.point,
                    Quaternion.LookRotation(Vector3.ProjectOnPlane(_character.forward, hit.normal), hit.normal)
                );
            }
            else
            {
                var target = Vector3.ProjectOnPlane(_hips.position, Vector3.up);
                target += Vector3.up * (_locomotionSphereRigidbody.position.y - 0.2f);
                target += _velocity * _stepTime + _character.right * foot.Offset;
                foot.Target.SetPositionAndRotation(target, Quaternion.LookRotation(_character.forward));
            }
        }

        private IEnumerator Step(bool isMandatory = false)
        {
            if (!isMandatory && _isStepping || !_currentFoot.IsGrounded)
            {
                yield break;
            }
            while (_isStepping)
            {
                yield return null;
            }
            _isStepping = true;

            _currentFoot.Anchor.GetPositionAndRotation(out var startPoint, out var startRot);
            float timeElapsed = 0;
            do
            {
                if (!isMandatory)
                {
                    _stepTime = Mathf.Clamp(-2f / 30f * _velocity.magnitude + 0.3f, 0.1f, 0.2f);
                    _stepHeight = Mathf.Clamp(1f / 6f * _velocity.magnitude, 0f, 0.5f);
                }
                else
                {
                    _stepTime = 0.1f;
                    _stepHeight = 0f;
                }

                startPoint += _groundVelocity * Time.deltaTime;

                _currentFoot.Target.GetPositionAndRotation(out var endPoint, out var endRot);
                var centerPoint = (startPoint + endPoint) / 2f;
                centerPoint += Vector3.up * _stepHeight;

                timeElapsed += Time.deltaTime;
                float normalizedTime = timeElapsed / (_stepTime * 2f);

                _currentFoot.Anchor.SetPositionAndRotation(Vector3.Lerp(
                    Vector3.Lerp(startPoint, centerPoint, normalizedTime),
                    Vector3.Lerp(centerPoint, endPoint, normalizedTime),
                    normalizedTime), Quaternion.Slerp(startRot, endRot, normalizedTime));

                yield return null;
            } while (timeElapsed < _stepTime * 2f);

            if (!isMandatory)
                _currentFoot.Step(_smoothLocomotion.IsRunning);

            if (_currentFoot.Equals(_leftFoot))
                _currentFoot = _rightFoot;
            else
                _currentFoot = _leftFoot;

            _isStepping = false;
        }

        private void SnapFootToTarget(Foot foot)
        {
            foot.Anchor.SetPositionAndRotation(
                foot.Target.position,
                foot.Target.rotation
            );
        }

        public void TeleportFeet()
        {
            _velocity = Vector3.zero;
            UpdateTarget(_leftFoot);
            UpdateTarget(_rightFoot);
            SnapFootToTarget(_leftFoot);
            SnapFootToTarget(_rightFoot);
        }
    }
}