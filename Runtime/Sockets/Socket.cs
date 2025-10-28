using KadenZombie8.BIMOS.Rig;
using System;
using System.Collections;
using UnityEngine;

namespace KadenZombie8.BIMOS.Sockets
{
    public class Socket : MonoBehaviour
    {
        public event Action
            OnAttach,
            OnDetach;

        public string[] Tags;

        public Transform AttachPoint, DetachPoint;

        [SerializeField]
        private float _insertTime = 0.1f;

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
            foreach (string socketTag in Tags)
                foreach (string plugTag in plug.Tags)
                    if (socketTag == plugTag)
                        return true;

            return false;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (Plug)
                return;

            Plug plug = other.GetComponent<Plug>();

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

            foreach (Collider plugCollider in Plug.Rigidbody.GetComponentsInChildren<Collider>())
                foreach (Collider socketCollider in _body.GetComponentsInChildren<Collider>())
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
            while (elapsedTime < alignTime)
            {
                var initialWorldPosition = _body.TransformPoint(initialLocalPosition);
                var initialWorldRotation = _body.rotation * initialLocalRotation;
                var targetPosition = Vector3.Lerp(initialWorldPosition, DetachPoint.position, elapsedTime / alignTime);
                var targetRotation = Quaternion.Lerp(initialWorldRotation, DetachPoint.rotation, elapsedTime / alignTime);

                AttachJoint.connectedAnchor = _body.InverseTransformPoint(targetPosition);
                AttachJoint.targetRotation = Quaternion.Inverse(targetRotation) * DetachPoint.rotation;

                elapsedTime += Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }

            OnAttach?.Invoke();
            Plug.Attach();
            elapsedTime = 0f;
            while (elapsedTime < _insertTime)
            {
                var targetPosition = Vector3.Lerp(DetachPoint.position, AttachPoint.position, elapsedTime / _insertTime);
                var targetRotation = Quaternion.Lerp(DetachPoint.rotation, AttachPoint.rotation, elapsedTime / _insertTime);

                AttachJoint.connectedAnchor = _body.InverseTransformPoint(targetPosition);
                AttachJoint.targetRotation = Quaternion.Inverse(targetRotation) * DetachPoint.rotation;

                elapsedTime += Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }

            foreach (Grabbable grab in Plug.EnableGrabs)
                if (grab)
                    grab.enabled = true;

            foreach (Grabbable grab in Plug.DisableGrabs)
                if (grab)
                    grab.enabled = false;

            AttachJoint.connectedAnchor = _body.InverseTransformPoint(AttachPoint.position);
            AttachJoint.targetRotation = Quaternion.Inverse(AttachPoint.rotation) * DetachPoint.rotation;

            _onCooldown = false;

            if (_waitingForDetach)
                Detach();
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
            OnDetach?.Invoke();
            Plug.Detach();
        }

        private IEnumerator DetachCoroutine()
        {
            float elapsedTime = 0f;
            var plug = Plug.transform;

            while (elapsedTime < _insertTime)
            {
                var targetPosition = Vector3.Lerp(AttachPoint.position, DetachPoint.position, elapsedTime / _insertTime);
                var targetRotation = Quaternion.Lerp(AttachPoint.rotation, DetachPoint.rotation, elapsedTime / _insertTime);

                AttachJoint.connectedAnchor = _body.InverseTransformPoint(targetPosition);
                AttachJoint.targetRotation = Quaternion.Inverse(targetRotation) * DetachPoint.rotation;

                elapsedTime += Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }

            AttachJoint.connectedAnchor = _body.InverseTransformPoint(DetachPoint.position);
            AttachJoint.targetRotation = Quaternion.Inverse(DetachPoint.rotation) * DetachPoint.rotation;

            Destroy(AttachJoint);

            foreach (Collider plugCollider in Plug.Rigidbody.GetComponentsInChildren<Collider>())
                foreach (Collider socketCollider in _body.GetComponentsInChildren<Collider>())
                    Physics.IgnoreCollision(plugCollider, socketCollider, false);

            foreach (Grabbable grab in Plug.EnableGrabs)
                if (grab)
                    grab.enabled = false;

            foreach (Grabbable grab in Plug.DisableGrabs)
                if (grab)
                    grab.enabled = true;

            Plug.Rigidbody.transform.SetPositionAndRotation(
                DetachPoint.position - DetachPoint.rotation * plug.localPosition,
                DetachPoint.rotation * Quaternion.Inverse(plug.localRotation)
            );

            Plug.Rigidbody.linearVelocity += (DetachPoint.position - AttachPoint.position) / _insertTime;

            Plug.Socket = null;
            Plug = null;

            yield return new WaitForSeconds(_cooldownTime);

            _onCooldown = false;
        }
    }
}
