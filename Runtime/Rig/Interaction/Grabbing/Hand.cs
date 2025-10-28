using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;
using UnityEngine.XR;
using UnityEngine.XR.OpenXR.Input;

namespace KadenZombie8.BIMOS.Rig
{
    public enum Handedness { Left, Right };

    public class Hand : MonoBehaviour
    {
        public HandAnimator HandAnimator;
        public Grabbable CurrentGrab;
        public HandInputReader HandInputReader;
        public Transform PalmTransform;
        public PhysicsHand PhysicsHand;
        public Transform PhysicsHandTransform;
        public GrabHandler GrabHandler;
        public Handedness Handedness;
        public Hand OtherHand;
        public Collider PhysicsHandCollider;
        public Joint GrabJoint;

        [SerializeField]
        private InputActionReference _hapticAction;

        public void SendHapticImpulse(float amplitude, float duration)
        {
            var device = Handedness == Handedness.Left ? InputDevices.GetDeviceAtXRNode(XRNode.LeftHand) : InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
            if (device.isValid)
                device.SendHapticImpulse(0, amplitude, duration);
        }
    }
}
