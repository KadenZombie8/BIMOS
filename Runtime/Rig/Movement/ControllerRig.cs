using System;
using UnityEngine;

namespace KadenZombie8.BIMOS.Rig
{
    [DefaultExecutionOrder(-10)]
    public class ControllerRig : MonoBehaviour
    {
        [SerializeField]
        private BIMOSRig _rig;

        public ControllerRigTransforms Transforms;

        private readonly float _eyeToCrownHeight = 0.1f;

        public Quaternion HeadForwardRotation => Quaternion.LookRotation(Vector3.Cross(Transforms.Camera.right, Vector3.up));

        public Vector3 HeadForwardDirection => HeadForwardRotation * Vector3.forward;

        public void Start()
        {
            _rig = GetComponentInParent<BIMOSRig>();
            Transforms.Camera.GetComponent<Camera>().cullingMask = ~LayerMask.GetMask("UI");
        }

        public void UpdateRealHeight(float realHeight)
        {
            realHeight /= 100f;
            realHeight = Mathf.Clamp(realHeight, 0.9f, 2.7f);
            var eyeHeight = realHeight - _eyeToCrownHeight;
            float scaleFactor = _rig.AnimationRig.AvatarEyeHeight / eyeHeight;
            transform.localScale = Vector3.one * scaleFactor;
        }

        private void FixedUpdate()
        {
            transform.position = _rig.PhysicsRig.Rigidbodies.Pelvis.position;
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
