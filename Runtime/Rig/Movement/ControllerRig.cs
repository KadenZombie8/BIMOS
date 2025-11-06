using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.XR;
using UnityEngine.XR;
using UnityEngine.XR.Management;

namespace KadenZombie8.BIMOS.Rig
{
    public class ControllerRig : MonoBehaviour
    {
        private BIMOSRig _player;
        public ControllerRigTransforms Transforms;
        public float HeadsetStandingHeight = 1.65f;

        public Quaternion HeadForwardRotation => Quaternion.LookRotation(Vector3.Cross(Transforms.Camera.right, Vector3.up));

        public Vector3 HeadForwardDirection => HeadForwardRotation * Vector3.forward;

        private TrackedPoseDriver
            _headsetDriver,
            _leftControllerDriver,
            _rightControllerDriver;

        public void Start()
        {
            _player = BIMOSRig.Instance;
            transform.parent = _player.PhysicsRig.Rigidbodies.Pelvis.transform;
            transform.localPosition = Vector3.zero;

            Transforms.Camera.GetComponent<Camera>().cullingMask = ~LayerMask.GetMask("BIMOSMenu");
            Transforms.MenuCamera.GetComponent<Camera>().cullingMask = LayerMask.GetMask("BIMOSMenu");

            #region Preferences
            HeadsetStandingHeight = PlayerPrefs.GetFloat("HeadsetStandingHeight", 1.65f);
            HeadsetStandingHeight = Mathf.Clamp(HeadsetStandingHeight, 1f, 3f);

            //SmoothTurnSpeed = PlayerPrefs.GetFloat("SmoothTurnSpeed", 10f);
            //SnapTurnIncrement = PlayerPrefs.GetFloat("SnapTurnIncrement", 45f);
            #endregion

            ScaleCharacter();
            //StartCoroutine(WaitForMotionControls());
        }

        private IEnumerator WaitForMotionControls()
        {
            _headsetDriver = Transforms.Camera.GetComponent<TrackedPoseDriver>();
            _leftControllerDriver = Transforms.LeftController.GetComponent<TrackedPoseDriver>();
            _rightControllerDriver = Transforms.RightController.GetComponent<TrackedPoseDriver>();

            _headsetDriver.enabled
                = _leftControllerDriver.enabled
                    = _rightControllerDriver.enabled
                        = false;

            var headsetActive = false;

            while (!headsetActive)
            {
                try
                {
                    var display = XRGeneralSettings.Instance.Manager.activeLoader.GetLoadedSubsystem<XRDisplaySubsystem>();
                    if (display.running)
                        headsetActive = true;
                }
                catch { }
                yield return null;
            }

            _headsetDriver.enabled
                = _leftControllerDriver.enabled
                    = _rightControllerDriver.enabled
                        = true;
        }

        public void ScaleCharacter()
        {
            float scaleFactor = _player.AnimationRig.AvatarEyeHeight / HeadsetStandingHeight;
            transform.localScale = Vector3.one * scaleFactor;
        }

        [Serializable]
        public struct ControllerRigTransforms
        {
            public Transform Camera;
            public Transform HeadCameraOffset;
            public Transform RoomscaleOffset;
            public Transform MenuCamera;
            public Transform LeftPalm;
            public Transform RightPalm;
            public Transform LeftController;
            public Transform RightController;
        }
    }
}
