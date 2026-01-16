using System;
using UnityEngine;

namespace KadenZombie8.BIMOS.Rig
{
    public class ControllerRig : MonoBehaviour
    {
        private BIMOSRig _player;
        public ControllerRigTransforms Transforms;
        public float HeadsetStandingHeight = 1.65f;

        public Quaternion HeadForwardRotation => Quaternion.LookRotation(Vector3.Cross(Transforms.Camera.right, Vector3.up));

        public Vector3 HeadForwardDirection => HeadForwardRotation * Vector3.forward;

        public void Start()
        {
            _player = BIMOSRig.Instance;

            Transforms.Camera.GetComponent<Camera>().cullingMask = ~LayerMask.GetMask("BIMOSMenu");
            Transforms.MenuCamera.GetComponent<Camera>().cullingMask = LayerMask.GetMask("BIMOSMenu");

            #region Preferences
            HeadsetStandingHeight = PlayerPrefs.GetFloat("HeadsetStandingHeight", 1.65f);
            HeadsetStandingHeight = Mathf.Clamp(HeadsetStandingHeight, 1f, 3f);

            //SmoothTurnSpeed = PlayerPrefs.GetFloat("SmoothTurnSpeed", 10f);
            //SnapTurnIncrement = PlayerPrefs.GetFloat("SnapTurnIncrement", 45f);
            #endregion

            ScaleCharacter();
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
