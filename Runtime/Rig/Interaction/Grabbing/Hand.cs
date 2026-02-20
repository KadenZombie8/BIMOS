using System.Collections;
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

        private Coroutine _hapticCoroutine;

        public void SendHapticImpulse(float amplitude, float duration)
        {
            // XR
            var device = Handedness == Handedness.Left ? InputDevices.GetDeviceAtXRNode(XRNode.LeftHand) : InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
            if (device.isValid)
                device.SendHapticImpulse(0, amplitude, duration);

            // Gamepad
            var gamepad = Gamepad.current;
            if (gamepad != null)
            {
                gamepad.SetMotorSpeeds(amplitude, amplitude);

                if (_hapticCoroutine != null)
                    StopCoroutine(_hapticCoroutine);

                _hapticCoroutine = StartCoroutine(StopHaptics(duration, gamepad));
            }
        }

        private IEnumerator StopHaptics(float duration, Gamepad gamepad)
        {
            yield return new WaitForSeconds(duration);
            gamepad?.SetMotorSpeeds(0f, 0f);
            _hapticCoroutine = null;
        }
    }
}
