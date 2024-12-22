using UnityEngine;

namespace BIMOS
{
    [AddComponentMenu("BIMOS/Grabs/Grab (Offhand)")]
    public class OffhandGrab : SnapGrab
    {
        public override void AlignHand(Hand hand)
        {
            base.AlignHand(hand);
            Transform otherPhysicsHand = hand.otherHand.PhysicsHandTransform;
            hand.PhysicsHand.Target = hand.otherHand.PhysicsHand.Target;

            if (!hand.otherHand.CurrentGrab)
                return;

            Vector3 targetPosition = transform.TransformPoint(hand.PalmTransform.InverseTransformPoint(hand.PhysicsHandTransform.position));
            Quaternion targetRotation = transform.rotation * Quaternion.Inverse(hand.PalmTransform.rotation) * hand.PhysicsHandTransform.rotation;

            hand.PhysicsHand.TargetOffsetPosition = otherPhysicsHand.transform.InverseTransformPoint(targetPosition);
            hand.PhysicsHand.TargetOffsetRotation = Quaternion.Inverse(otherPhysicsHand.transform.rotation) * targetRotation;
        }

        public override void DestroyGrabJoint(Hand hand)
        {
            base.DestroyGrabJoint(hand);
            hand.PhysicsHand.Target = hand.PhysicsHand.Controller;
            hand.PhysicsHand.TargetOffsetPosition = Vector3.zero;
            hand.PhysicsHand.TargetOffsetRotation = Quaternion.identity;
        }
    }
}
