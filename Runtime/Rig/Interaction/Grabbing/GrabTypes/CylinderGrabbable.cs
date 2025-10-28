using UnityEngine;

namespace KadenZombie8.BIMOS.Rig
{
    [AddComponentMenu("BIMOS/Grabbables/Grabbable (Cylinder)")]
    public class CylinderGrabbable : LineGrabbable
    {
        public override void AlignHand(Hand hand, out Vector3 position, out Quaternion rotation)
        {
            var palmForwardLocal = Origin.InverseTransformDirection(hand.PalmTransform.forward);
            palmForwardLocal.y = 0f;
            palmForwardLocal.Normalize();
            var grabForwardLocal = Origin.InverseTransformDirection(transform.forward);
            grabForwardLocal.y = 0f;
            grabForwardLocal.Normalize();

            var palmForwardWorld = Origin.TransformDirection(palmForwardLocal);
            var grabForwardWorld = Origin.TransformDirection(grabForwardLocal);

            var fromToRotation = Quaternion.FromToRotation(grabForwardWorld, palmForwardWorld);

            var point = GetNearestPoint(hand.PalmTransform.position);
            AlignHandBase(hand, out position, out rotation);
            rotation = fromToRotation * rotation;

            var basePosition = position + point - Origin.position;
            var offset = basePosition - point;
            position = point + fromToRotation * offset;
        }

        public override void CreateCollider()
        {
            base.CreateCollider();
            var collider = (CapsuleCollider)Collider;
            collider.radius = Vector3.Distance(Origin.position, transform.position) + 0.005f;
            Collider.transform.SetPositionAndRotation(Origin.position, Origin.rotation);
        }
    }
}
