using System;
using System.Collections;
using UnityEngine;

namespace KadenZombie8.BIMOS.Rig
{
    public class Feet : MonoBehaviour
    {
        public event Action OnStep;

        private BIMOSRig _player;

        private Foot _currentFoot, _leftFoot, _rightFoot;

        private Rigidbody _pelvisRigidbody;
        private Vector3 _pelvisVelocity, _groundVelocity;
        private bool _isMoving, _isStepping;
        private float _stepTime = 0.1f, _stepLength = 0.1f, _stepHeight = 0.1f;

        private LayerMask _mask;

        private void Start()
        {
            _player = GetComponentInParent<BIMOSRig>();
            _leftFoot = new Foot(_player.AnimationRig.Transforms.LeftFootAnchor, _player.AnimationRig.Transforms.LeftFootTarget, -0.08f);
            _rightFoot = new Foot(_player.AnimationRig.Transforms.RightFootAnchor, _player.AnimationRig.Transforms.RightFootTarget, 0.08f);
            _currentFoot = _rightFoot;
            _pelvisRigidbody = _player.PhysicsRig.Rigidbodies.Pelvis;
            _mask = ~LayerMask.GetMask("BIMOSRig");
        }

        private void Update()
        {
            _pelvisVelocity = Vector3.ProjectOnPlane(_pelvisRigidbody.linearVelocity - _groundVelocity, Vector3.up);
            UpdateTarget(_leftFoot);
            UpdateTarget(_rightFoot);
            if (!_player.PhysicsRig.LocomotionSphere.IsGrounded) //Take air pose if off ground
            {
                SnapFootToTarget(_leftFoot);
                SnapFootToTarget(_rightFoot);
            }
            else
            {
                if ((_currentFoot.Transform.position - _currentFoot.Target.position).magnitude > _stepLength) //Step if foot far enough from target
                {
                    StartCoroutine(Step());
                }
                if (_pelvisVelocity.magnitude < 0.1f)
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
            foot.Target.position = _player.AnimationRig.Transforms.Hips.position + _pelvisVelocity * _stepTime + _player.AnimationRig.Transforms.Character.right * foot.Offset;
            if (Physics.Raycast(foot.Target.position, Vector3.down, out RaycastHit hit, 1.25f, _mask, QueryTriggerInteraction.Ignore) && _player.PhysicsRig.LocomotionSphere.IsGrounded)
            {
                foot.IsGrounded = true;
                foot.Target.position = hit.point;
                foot.Target.rotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(_player.AnimationRig.Transforms.Character.forward, hit.normal), hit.normal);
            }
            else
            {
                Vector3 target = Vector3.ProjectOnPlane(_player.AnimationRig.Transforms.Hips.position, Vector3.up);
                target += Vector3.up * (_player.PhysicsRig.Rigidbodies.LocomotionSphere.position.y - 0.2f);
                target += _pelvisVelocity * _stepTime + _player.AnimationRig.Transforms.Character.right * foot.Offset;
                foot.Target.SetPositionAndRotation(target, Quaternion.LookRotation(_player.AnimationRig.Transforms.Character.forward));
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

            Quaternion startRot = _currentFoot.Transform.rotation;
            Vector3 startPoint = _currentFoot.Transform.position;

            float timeElapsed = 0;
            do
            {
                if (!isMandatory)
                {
                    _stepTime = Mathf.Clamp(-2 / 30f * _pelvisVelocity.magnitude + 0.3f, 0.1f, 0.2f);
                    _stepHeight = Mathf.Clamp(1 / 6f * _pelvisVelocity.magnitude, 0f, 0.5f);
                }
                else
                {
                    _stepTime = 0.1f;
                    _stepHeight = 0f;
                }

                startPoint += _groundVelocity * Time.deltaTime;

                Quaternion endRot = _currentFoot.Target.rotation;
                Vector3 endPoint = _currentFoot.Target.position;
                Vector3 centerPoint = (startPoint + endPoint) / 2;
                centerPoint += Vector3.up * _stepHeight;

                timeElapsed += Time.deltaTime;
                float normalizedTime = timeElapsed / (_stepTime * 2);
                _currentFoot.Transform.position = Vector3.Lerp(
                    Vector3.Lerp(startPoint, centerPoint, normalizedTime),
                    Vector3.Lerp(centerPoint, endPoint, normalizedTime),
                    normalizedTime);
                _currentFoot.Transform.rotation = Quaternion.Slerp(startRot, endRot, normalizedTime);
                yield return null;
            } while (timeElapsed < _stepTime * 2);

            if (!isMandatory)
                OnStep?.Invoke();

            if (_currentFoot == _leftFoot)
                _currentFoot = _rightFoot;
            else
                _currentFoot = _leftFoot;

            _isStepping = false;
        }

        private void SnapFootToTarget(Foot foot)
        {
            foot.Transform.SetPositionAndRotation(
                foot.Target.position,
                foot.Target.rotation
            );
        }

        public void TeleportFeet()
        {
            UpdateTarget(_leftFoot);
            UpdateTarget(_rightFoot);
            SnapFootToTarget(_leftFoot);
            SnapFootToTarget(_rightFoot);
        }
    }

    class Foot
    {
        public Transform Transform;
        public Transform Target;
        public float Offset;
        public bool IsGrounded;

        public Foot(Transform transform, Transform target, float offset, bool isGrounded = false)
        {
            Transform = transform;
            Target = target;
            Offset = offset;
            IsGrounded = isGrounded;
        }
    }
}