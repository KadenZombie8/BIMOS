using UnityEngine;
using UnityEngine.XR;

namespace BIMOS
{
    [AddComponentMenu("BIMOS/Grabs/Grab (Basic)")]
    public class Grab : MonoBehaviour
    {
        [HideInInspector]
        public Hand LeftHand, RightHand;

        public HandPose HandPose;
        public delegate void Release();
        public event Release ReleaseEvent;
        public bool IsLeftHanded = true, IsRightHanded = true;
        public Grab[] EnableGrabs, DisableGrabs;

        private Rigidbody _rigidBody;
        private ArticulationBody _articulationBody;
        private Transform _body;

        [HideInInspector]
        public Collider Collider;

        private UnityEngine.XR.InputDevice _leftDevice;
        private UnityEngine.XR.InputDevice _rightDevice;

        private void OnEnable()
        {
            _body = Utilities.GetBody(transform, out _rigidBody, out _articulationBody);
            if (!_body)
            {
                Rigidbody rigidbody = gameObject.AddComponent<Rigidbody>();
                rigidbody.isKinematic = true;
                _rigidBody = rigidbody;
                _body = _rigidBody.transform;
            }

            Collider = GetComponent<Collider>();
            if (Collider)
                return;

            CreateCollider();

            // Initialize Input Devices for Haptic Feedback (controllers)
            _leftDevice = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
            _rightDevice = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
        }

        public virtual void CreateCollider()
        {
            SphereCollider collider = gameObject.AddComponent<SphereCollider>();
            collider.isTrigger = true;
            collider.radius = 0.01f;
            Collider = collider;
        }

        public virtual float CalculateRank(Transform handTransform)
        {
            if (Collider is MeshCollider)
                return 1f / 1000f;

            return 1f / Vector3.Distance(handTransform.position, Collider.ClosestPoint(handTransform.position));
        }

        public virtual void OnGrab(Hand hand)
        {
            hand.CurrentGrab = this;

            if (hand.IsLeftHand)
                LeftHand = hand;
            else
                RightHand = hand;

            hand.GrabHandler.ApplyGrabPose(HandPose);

            AlignHand(hand);
            CreateGrabJoint(hand);

            IgnoreCollision(hand, true);

            foreach (Grab grab in EnableGrabs)
            {
                if (grab)
                    grab.enabled = true;
            }
            foreach (Grab grab in DisableGrabs)
            {
                if (grab)
                    grab.enabled = false;
            }

            GetComponent<Interactable>()?.OnGrab();

            ProvideHapticFeedback(hand);
        }

        private void ProvideHapticFeedback(Hand hand)
        {
            UnityEngine.XR.InputDevice device = hand.IsLeftHand ? _leftDevice : _rightDevice;

            if (device.isValid)
            {
                device.SendHapticImpulse(0, 10f, 0.7f);
            }
        }

        public virtual void IgnoreCollision(Hand hand, bool ignore)
        {
            foreach (Collider collider in _body.GetComponentsInChildren<Collider>())
                Physics.IgnoreCollision(collider, hand.PhysicsHandCollider, ignore);
        }

        public virtual void AlignHand(Hand hand) { }

        private void CreateGrabJoint(Hand hand)
        {
            FixedJoint grabJoint = hand.PhysicsHandTransform.gameObject.AddComponent<FixedJoint>();
            grabJoint.enableCollision = true;
            if (_rigidBody)
                grabJoint.connectedBody = _rigidBody;
            if (_articulationBody)
                grabJoint.connectedArticulationBody = _articulationBody;
        }

        public void OnRelease(Hand hand, bool toggleGrabs)
        {
            if (!hand)
                return;

            DestroyGrabJoint(hand);

            if (toggleGrabs)
            {
                foreach (Grab grab in EnableGrabs)
                {
                    if (grab)
                        grab.enabled = false;
                }
                foreach (Grab grab in DisableGrabs)
                {
                    if (grab)
                        grab.enabled = true;
                }
            }

            GetComponent<Interactable>()?.OnRelease();

            hand.CurrentGrab = null;

            if (hand.IsLeftHand)
                LeftHand = null;
            else
                RightHand = null;

            ReleaseEvent?.Invoke();
        }

        public virtual void DestroyGrabJoint(Hand hand)
        {
            if (!hand)
                return;

            FixedJoint grabJoint = hand.PhysicsHandTransform.GetComponent<FixedJoint>();
            Destroy(grabJoint); 

            IgnoreCollision(hand, false);

            if (!gameObject.activeSelf)
                return;
        }

        private void OnDisable()
        {
            OnRelease(LeftHand, false);
            OnRelease(RightHand, false);
        }
    }
}
