using UnityEngine;

namespace KadenZombie8.BIMOS.Rig
{
    [AddComponentMenu("BIMOS/Grabbables/Grabbable (Auto)")]
    public class AutoGrabbable : Grabbable
    {
        public override void AlignHand(Hand hand, out Vector3 position, out Quaternion rotation)
        {
            Vector3 handTargetPosition = Collider.ClosestPoint(hand.PalmTransform.position);
            Vector3 handToTargetDirection = handTargetPosition - hand.PalmTransform.position;

            position = hand.PalmTransform.position;
            rotation = hand.PalmTransform.rotation;

            if (handToTargetDirection.sqrMagnitude == 0f)
                return;

            handToTargetDirection.Normalize();

            Ray ray = new(hand.PalmTransform.position, handToTargetDirection);

            if (Collider.Raycast(ray, out var hit, 10f))
            {
                Vector3 projected = Vector3.ProjectOnPlane(-hand.PalmTransform.up, hit.normal).normalized;
                position = handTargetPosition;
                Vector3 crossed = Vector3.Cross(hit.normal, projected).normalized;
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