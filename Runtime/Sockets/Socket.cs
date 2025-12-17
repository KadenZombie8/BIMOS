using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace KadenZombie8.BIMOS.Sockets
{
    public class Socket : MonoBehaviour
    {
        [Serializable]
        public struct SocketEvents
        {
            public SocketAnimationEvents Align;
            public SocketAnimationEvents Attach;
            public SocketAnimationEvents Detach;
        }

        [Serializable]
        public struct SocketAnimationEvents
        {
            public UnityEvent<Plug> OnStart;
            public UnityEvent<Plug> OnEnd;
        }

        public SocketEvents Events;

        public string[] Tags;

        public Transform AttachPoint, DetachPoint;

        public float InsertTime = 0.1f;

        [HideInInspector]
        public Plug Plug;

        [HideInInspector]
        public ConfigurableJoint AttachJoint;

        private bool _onCooldown;
        private readonly float _cooldownTime = 0.1f;
        private bool _waitingForDetach;
        private readonly float _maxAlignTime = 0.25f;
        private readonly float _maxPositionDifference = 0.1f;

        private Rigidbody _rigidBody;
        private ArticulationBody _articulationBody;
        private Transform _body;

        private void Awake() => _body = Utilities.GetBody(transform, out _rigidBody, out _articulationBody);

        private bool HasMatchingTag(Plug plug)
        {
            foreach (var socketTag in Tags)
                foreach (var plugTag in plug.Tags)
                    if (socketTag == plugTag)
                        return true;

            return false;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (Plug)
                return;

            var plug = other.GetComponent<Plug>();

            if (!plug)
                return;

            if (!HasMatchingTag(plug))
                return;

            if (plug.Socket)
                return;

            Attach(plug);
        }

        public void Attach(Plug plug)
        {
            if (_onCooldown)
                return;

            if (!plug.IsGrabbed())
                return;

            _onCooldown = true;

            Plug = plug;

            Plug.Socket = this;

            foreach (var plugCollider in Plug.Rigidbody.GetComponentsInChildren<Collider>())
                foreach (var socketCollider in _body.GetComponentsInChildren<Collider>())
                    Physics.IgnoreCollision(plugCollider, socketCollider, true);

            StartCoroutine(AttachCoroutine());
        }

        private IEnumerator AttachCoroutine()
        {
            var plug = Plug.transform;

            var rotation = Plug.Rigidbody.transform.rotation;
            Plug.Rigidbody.transform.rotation = DetachPoint.rotation * Quaternion.Inverse(plug.localRotation);

            AttachJoint = Plug.Rigidbody.gameObject.AddComponent<ConfigurableJoint>();
            AttachJoint.xMotion
               = AttachJoint.yMotion
               = AttachJoint.zMotion
               = ConfigurableJointMotion.Locked;
            AttachJoint.rotationDriveMode = RotationDriveMode.Slerp;
            AttachJoint.slerpDrive = new() { positionSpring = Mathf.Infinity, maximumForce = Mathf.Infinity };
            if (_rigidBody)
                AttachJoint.connectedBody = _rigidBody;
            if (_articulationBody)
                AttachJoint.connectedArticulationBody = _articulationBody;

            AttachJoint.autoConfigureConnectedAnchor = false;
            AttachJoint.anchor = plug.localPosition;

            Plug.Rigidbody.transform.rotation = rotation;

            plug.GetPositionAndRotation(out var initialPosition, out var initialRotation);
            var initialLocalPosition = _body.InverseTransformPoint(initialPosition);
            var initialLocalRotation = Quaternion.Inverse(_body.rotation) * initialRotation;

            var elapsedTime = 0f;
            var positionDifference = Mathf.Min(
                Vector3.Distance(initialPosition, DetachPoint.position), _maxPositionDifference)
                / _maxPositionDifference;
            var rotationDifference = Quaternion.Angle(plug.rotation, DetachPoint.rotation) / 180f;
            var averageDifference = Mathf.Min(positionDifference + rotationDifference, 1f);
            var alignTime = 0f;

            if (averageDifference > 0.1f)
                alignTime = _maxAlignTime * (averageDifference - 0.1f) / 0.9f;

            Events.Align.OnStart?.Invoke(Plug);
            Plug.Events.Align.OnStart?.Invoke(this);
            while (elapsedTime < alignTime)
            {
                if (!Plug || !Plug.Rigidbody || !Plug.Rigidbody.gameObject.activeInHierarchy)
                {
                    ForceReset();
                    yield break;
                }

                var initialWorldPosition = _body.TransformPoint(initialLocalPosition);
                var initialWorldRotation = _body.rotation * initialLocalRotation;
                var targetPosition = Vector3.Lerp(initialWorldPosition, DetachPoint.position, elapsedTime / alignTime);
                var targetRotation = Quaternion.Lerp(initialWorldRotation, DetachPoint.rotation, elapsedTime / alignTime);

                AttachJoint.connectedAnchor = _body.InverseTransformPoint(targetPosition);
                AttachJoint.targetRotation = Quaternion.Inverse(targetRotation) * DetachPoint.rotation;

                elapsedTime += Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }
            Events.Align.OnEnd?.Invoke(Plug);
            Plug.Events.Align.OnEnd?.Invoke(this);

            elapsedTime = 0f;
            Events.Attach.OnStart?.Invoke(Plug);
            Plug.Events.Attach.OnStart?.Invoke(this);
            while (elapsedTime < InsertTime)
            {
                if (!Plug || !Plug.Rigidbody || !Plug.Rigidbody.gameObject.activeInHierarchy)
                {
                    ForceReset();
                    yield break;
                }

                var targetPosition = Vector3.Lerp(DetachPoint.position, AttachPoint.position, elapsedTime / InsertTime);
                var targetRotation = Quaternion.Lerp(DetachPoint.rotation, AttachPoint.rotation, elapsedTime / InsertTime);

                AttachJoint.connectedAnchor = _body.InverseTransformPoint(targetPosition);
                AttachJoint.targetRotation = Quaternion.Inverse(targetRotation) * DetachPoint.rotation;

                elapsedTime += Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }

            Events.Attach.OnEnd?.Invoke(Plug);
            Plug.Events.Attach.OnEnd?.Invoke(this);

            AttachJoint.connectedAnchor = _body.InverseTransformPoint(AttachPoint.position);
            AttachJoint.targetRotation = Quaternion.Inverse(AttachPoint.rotation) * DetachPoint.rotation;

            _onCooldown = false;

            if (_waitingForDetach)
                Detach();
        }

        private void ForceReset()
        {
            Destroy(AttachJoint);
            Plug = null;
            _onCooldown = false;
            _waitingForDetach = false;
        }

        public void Detach()
        {
            if (!Plug)
                return;

            _waitingForDetach = true;

            if (_onCooldown)
                return;

            _waitingForDetach = false;

            _onCooldown = true;

            StartCoroutine(DetachCoroutine());
        }

        private IEnumerator DetachCoroutine()
        {
            float elapsedTime = 0f;
            var plug = Plug.transform;

            Events.Detach.OnStart?.Invoke(Plug);
            Plug.Events.Detach.OnStart?.Invoke(this);
            while (elapsedTime < InsertTime)
            {
                var targetPosition = Vector3.Lerp(AttachPoint.position, DetachPoint.position, elapsedTime / InsertTime);
                var targetRotation = Quaternion.Lerp(AttachPoint.rotation, DetachPoint.rotation, elapsedTime / InsertTime);

                AttachJoint.connectedAnchor = _body.InverseTransformPoint(targetPosition);
                AttachJoint.targetRotation = Quaternion.Inverse(targetRotation) * DetachPoint.rotation;

                elapsedTime += Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }
            Events.Detach.OnEnd?.Invoke(Plug);
            Plug.Events.Detach.OnEnd?.Invoke(this);

            AttachJoint.connectedAnchor = _body.InverseTransformPoint(DetachPoint.position);
            AttachJoint.targetRotation = Quaternion.Inverse(DetachPoint.rotation) * DetachPoint.rotation;

            Destroy(AttachJoint);

            foreach (var plugCollider in Plug.Rigidbody.GetComponentsInChildren<Collider>())
                foreach (var socketCollider in _body.GetComponentsInChildren<Collider>())
                    Physics.IgnoreCollision(plugCollider, socketCollider, false);

            Plug.Rigidbody.transform.SetPositionAndRotation(
                DetachPoint.position - DetachPoint.rotation * plug.localPosition,
                DetachPoint.rotation * Quaternion.Inverse(plug.localRotation)
            );

            Plug.Rigidbody.linearVelocity += (DetachPoint.position - AttachPoint.position) / InsertTime;

            Plug.Socket = null;
            Plug = null;

            yield return new WaitForSeconds(_cooldownTime);

            _onCooldown = false;
        }
    }
}
