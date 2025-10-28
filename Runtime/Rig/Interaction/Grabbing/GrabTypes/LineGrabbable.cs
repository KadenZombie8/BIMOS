using UnityEngine;

namespace KadenZombie8.BIMOS.Rig
{
    [AddComponentMenu("BIMOS/Grabbables/Grabbable (Line)")]
    public class LineGrabbable : SnapGrabbable
    {
        [Header("Line Properties")]
        public Transform Origin;
        public float Length = 1f;

        protected void AlignHandBase(Hand hand, out Vector3 position, out Quaternion rotation)
        {
            base.AlignHand(hand, out position, out rotation);
        }

        public override void AlignHand(Hand hand, out Vector3 position, out Quaternion rotation)
        {
            var point = GetNearestPoint(hand.PalmTransform.position);
            base.AlignHand(hand, out position, out rotation);
            position += point - Origin.position;
        }

        public override void CreateCollider()
        {
            GameObject colliderObject = new("GrabCollider");
            colliderObject.transform.parent = transform;
            CapsuleCollider collider = colliderObject.AddComponent<CapsuleCollider>();
            collider.isTrigger = true;
            colliderObject.transform.SetPositionAndRotation(transform.position, Origin.rotation);
            collider.radius = 0.01f;
            collider.height = Length;
            Collider = collider;
        }

        protected Vector3 GetNearestPoint(Vector3 palmPosition)
        {
            var lineDirection = Origin.up;

            var v = palmPosition - Origin.position;
            var d = Vector3.Dot(v, lineDirection);
            d = Mathf.Clamp(d, -Length / 2f, Length / 2f);
            return Origin.position + lineDirection * d;
        }
    }
}
