using UnityEngine;

namespace KadenZombie8.BIMOS.Rig
{
    [AddComponentMenu("BIMOS/Grabbables/Grabbable (Snap)")]
    public class SnapGrabbable : Grabbable
    {
        public Handedness Handedness;

        public override float CalculateRank(Hand hand) => base.CalculateRank(hand) * 3f;

        public override void AlignHand(Hand hand, out Vector3 position, out Quaternion rotation)
        {
            position = transform.TransformPoint(hand.PalmTransform.InverseTransformPoint(hand.PhysicsHandTransform.position));
            rotation = transform.rotation * Quaternion.Inverse(hand.PalmTransform.rotation) * hand.PhysicsHandTransform.rotation;
        }
    }
}