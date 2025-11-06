using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace KadenZombie8.BIMOS.Rig.Movement
{
    /// <summary>
    /// Handles the movement from the player moving their headset
    /// in the real world.
    /// </summary>
    public class RoomscaleMovement : MonoBehaviour
    {
        private BIMOSRig _rig;
        private Vector3 _lastForwardDirection;

        private void Start() => _rig = BIMOSRig.Instance;

        private void FixedUpdate()
        {
            // Position
            var deltaCameraPosition = _rig.ControllerRig.Transforms.HeadCameraOffset.position - _rig.ControllerRig.transform.position;

            var deltaCameraPositionFlattened = deltaCameraPosition;
            deltaCameraPositionFlattened.y = 0f;

            _rig.ControllerRig.Transforms.RoomscaleOffset.position -= deltaCameraPosition;

            _rig.PhysicsRig.Rigidbodies.LocomotionSphere.position += deltaCameraPositionFlattened;
            _rig.PhysicsRig.Rigidbodies.Knee.position += deltaCameraPositionFlattened;
            _rig.PhysicsRig.Rigidbodies.Pelvis.position += deltaCameraPosition;
            _rig.PhysicsRig.Rigidbodies.Head.position += deltaCameraPosition;

            _rig.PhysicsRig.Crouching.TargetLegHeight += deltaCameraPosition.y;

            // Rotation
            var currentForwardDirection = _rig.ControllerRig.HeadForwardDirection;

            var deltaCameraRotation = Quaternion.FromToRotation(_lastForwardDirection, currentForwardDirection);
            _rig.PhysicsRig.Rigidbodies.Pelvis.rotation *= deltaCameraRotation;
            _rig.ControllerRig.transform.rotation *= Quaternion.Inverse(deltaCameraRotation);

            _lastForwardDirection = currentForwardDirection;
        }
    }
}