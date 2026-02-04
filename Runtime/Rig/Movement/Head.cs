using UnityEngine;

namespace KadenZombie8.BIMOS.Rig.Movement
{
    /// <summary>
    /// Aligns the head rigidbody to the camera
    /// </summary>
    public class Head : MonoBehaviour
    {
        [SerializeField]
        private Transform _pelvis;

        [SerializeField]
        private ConfigurableJoint _headJoint;

        [SerializeField]
        private Transform _headCameraOffset;

        private void FixedUpdate()
        {
            _headJoint.targetRotation = Quaternion.Inverse(_headCameraOffset.rotation) * _pelvis.rotation;
        }
    }
}