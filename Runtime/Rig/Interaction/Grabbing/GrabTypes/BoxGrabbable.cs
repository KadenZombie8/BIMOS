using System;
using UnityEngine;
using UnityEngine.XR;

namespace KadenZombie8.BIMOS.Rig
{
    public class BoxGrabbable : AutoGrabbable
    {
        [Flags]
        public enum BoxFaces
        {
            Front = 1,
            Back = 2,
            Left = 4,
            Right = 8,
            Top = 16,
            Bottom = 32
        }

        public BoxFaces EnabledFaces =
            BoxFaces.Front |
            BoxFaces.Back |
            BoxFaces.Left |
            BoxFaces.Right |
            BoxFaces.Top |
            BoxFaces.Bottom;

        private bool RoughlyEqual(Vector3 a, Vector3 b) => Vector3.Dot(a, b) > 0.9f;

        private bool IsValid(Hand hand, Vector3 handToTargetDirection)
        {
            var hit = RaycastHit(hand, handToTargetDirection);
            var normal = hit.normal;
            return
                EnabledFaces.HasFlag(BoxFaces.Front) && RoughlyEqual(normal, transform.forward) ||
                EnabledFaces.HasFlag(BoxFaces.Back) && RoughlyEqual(normal, -transform.forward) ||
                EnabledFaces.HasFlag(BoxFaces.Left) && RoughlyEqual(normal, -transform.right) ||
                EnabledFaces.HasFlag(BoxFaces.Right) && RoughlyEqual(normal, transform.right) ||
                EnabledFaces.HasFlag(BoxFaces.Top) && RoughlyEqual(normal, transform.up) ||
                EnabledFaces.HasFlag(BoxFaces.Bottom) && RoughlyEqual(normal, -transform.up);
        }

        private void AltAlignHand(Hand hand, out Vector3 position, out Quaternion rotation)
        {
            position = hand.PalmTransform.position;
            rotation = hand.PalmTransform.rotation;

            var hit = RaycastHit(hand, hand.PalmTransform.forward);
            if (hit.collider)
            {
                var projected = Vector3.ProjectOnPlane(-hand.PalmTransform.up, hit.normal).normalized;
                position = hit.point;
                var crossed = Vector3.Cross(hit.normal, projected).normalized;
                rotation = Quaternion.LookRotation(-crossed, -projected);
                rotation *= Quaternion.Euler(180f, 90f, 180f);
                position += hit.normal * 0.02f; // Moves hand out of collider
            }

            rotation = rotation * Quaternion.Inverse(hand.PalmTransform.rotation) * hand.PhysicsHandTransform.rotation;
            position += rotation * hand.PalmTransform.InverseTransformPoint(hand.PhysicsHandTransform.position);
        }

        public override void AlignHand(Hand hand, out Vector3 position, out Quaternion rotation)
        {
            var handTargetPosition = Collider.ClosestPoint(hand.PalmTransform.position);
            var handToTargetDirection = handTargetPosition - hand.PalmTransform.position;

            if (IsValid(hand, handToTargetDirection))
            {
                base.AlignHand(hand, out position, out rotation);
            }
            else if (IsValid(hand, hand.PalmTransform.forward))
            {
                AltAlignHand(hand, out position, out rotation);
            }
            else
            {
                position = hand.PalmTransform.position;
                rotation = hand.PalmTransform.rotation;
            }
        }
    }
}
