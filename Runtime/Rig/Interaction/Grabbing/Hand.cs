using UnityEngine;
using UnityEngine.InputSystem;
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
            => OpenXRInput.SendHapticImpulse(_hapticAction, amplitude, duration);
    }
}
