using UnityEngine;

namespace KadenZombie8.BIMOS.Rig
{
    [AddComponentMenu("BIMOS/Grabbables/Grabbable (Auto)")]
    public class AutoGrabbable : Grabbable
    {
        protected RaycastHit RaycastHit(Hand hand, Vector3 handToTargetDirection)
        {
            if (handToTargetDirection.sqrMagnitude == 0f)
                return new();

            handToTargetDirection.Normalize();

            Ray ray = new(hand.PalmTransform.position, handToTargetDirection);

            if (Collider.Raycast(ray, out var hit, 10f)) return hit;
            return default;
        }

        public override void AlignHand(Hand hand, out Vector3 position, out Quaternion rotation)
        {
            position = hand.PalmTransform.position;
            rotation = hand.PalmTransform.rotation;

            var handTargetPosition = Collider.ClosestPoint(hand.PalmTransform.position);
            var handToTargetDirection = handTargetPosition - hand.PalmTransform.position;
            var hit = RaycastHit(hand, handToTargetDirection);
            if (hit.collider)
            {
                var projected = Vector3.ProjectOnPlane(-hand.PalmTransform.up, hit.normal).normalized;
                position = handTargetPosition;
                var crossed = Vector3.Cross(hit.normal, projected).normalized;
                rotation = Quaternion.LookRotation(-crossed, -projected);
                rotation *= Quaternion.Euler(180f, 90f, 180f);
                position += hit.normal * 0.02f; // Moves hand out of collider
            }

            rotation = rotation * Quaternion.Inverse(hand.PalmTransform.rotation) * hand.PhysicsHandTransform.rotation;
            position += rotation * hand.PalmTransform.InverseTransformPoint(hand.PhysicsHandTransform.position);
        }

        public override void IgnoreCollision(Hand hand, bool ignore) { }
    }
}
