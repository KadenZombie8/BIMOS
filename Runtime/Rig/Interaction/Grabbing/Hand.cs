using KadenZombie8.BIMOS.Rig.Movement;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;

namespace KadenZombie8.BIMOS.Rig
{
    public enum Handedness { Left, Right };

    public class Hand : MonoBehaviour
    {
        public HandAnimator HandAnimator;
        public Grabbable CurrentGrab;
        public HandInputReader HandInputReader;
        public Transform PalmTransform;
        public PhysicsArm PhysicsArm;
        public Transform PhysicsHandTransform;
        public GrabHandler GrabHandler;
        public Handedness Handedness;
        public Hand OtherHand;
        public ArmColliders ArmColliders;
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
