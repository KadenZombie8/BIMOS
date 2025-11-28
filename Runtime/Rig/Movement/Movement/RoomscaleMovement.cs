using UnityEngine;

namespace KadenZombie8.BIMOS.Rig.Movement
{
    /// <summary>
    /// Handles the movement from the player moving their headset
    /// in the real world.
    /// </summary>
    public class RoomscaleMovement : MonoBehaviour
    {
        private BIMOSRig _rig;

        private void Start() => _rig = GetComponentInParent<BIMOSRig>();

        private void FixedUpdate()
        {
            var deltaCameraPosition = _rig.ControllerRig.Transforms.HeadCameraOffset.position - _rig.ControllerRig.transform.position;

            var deltaCameraPositionFlattened = deltaCameraPosition;
            deltaCameraPositionFlattened.y = 0f;

            _rig.ControllerRig.Transforms.RoomscaleOffset.position -= deltaCameraPosition;

            _rig.PhysicsRig.Rigidbodies.LocomotionSphere.position += deltaCameraPositionFlattened;
            _rig.PhysicsRig.Rigidbodies.Knee.position += deltaCameraPositionFlattened;
            _rig.PhysicsRig.Rigidbodies.Pelvis.position += deltaCameraPosition;
            _rig.PhysicsRig.Rigidbodies.Head.position += deltaCameraPosition;

            _rig.PhysicsRig.Crouching.TargetLegHeight += deltaCameraPosition.y;
        }
    }
}