using KadenZombie8.BIMOS.Rig;
using System;
using System.Linq;
using UnityEngine;

namespace KadenZombie8.BIMOS
{
    public class ForceGrabber : MonoBehaviour
    {
        public Hand hand;
        public LineRenderer lineRenderer;
        public float forceGrabDistance = 6f;
        public float forceGrabRadius = 1f;
        public ForceGrabbable CurrentGrab;
        public LayerMask InteractableLayer;
        void Update()
        {
            if(!hand.CurrentGrab)
                ForceGrabLogic();
            if(lineRenderer) LineRendererLogic();
        }

        private void ForceGrabLogic() {
            bool wasForceGripping = hand.HandInputReader.Grip > 0.5f && hand.HandInputReader.Trigger > 0.5f;
            bool isForceGripping = hand.HandInputReader.Grip + hand.HandInputReader.Trigger > 0.5f;

            if (wasForceGripping && !CurrentGrab) {
                CastForce(out CurrentGrab);
            }
            else if(!isForceGripping && CurrentGrab) {
                CurrentGrab = null;
            }

            if (CurrentGrab) {
                FollowLogic();
            }
        }

        private void OnDrawGizmos() {
            Gizmos.color = Color.green;
            bool hasHit;
            hasHit = CastForce(out ForceGrabbable grab);
            if (!hasHit) {
                Gizmos.DrawWireSphere(hand.PalmTransform.position, forceGrabRadius);
                return;
            }
            Gizmos.DrawLine(hand.PalmTransform.position, grab.Grabbable.transform.position);
            Gizmos.DrawWireSphere(grab.Grabbable.transform.position, forceGrabRadius);
        }

        private bool CastForce(out ForceGrabbable grabbable) {
            grabbable = null;
            bool hasHit = Physics.SphereCast(hand.PalmTransform.position, forceGrabRadius, hand.PalmTransform.up, out var hit, forceGrabDistance, InteractableLayer, QueryTriggerInteraction.Collide);
            if(hasHit) grabbable = hit.collider.GetComponent<ForceGrabbable>();
            if (grabbable) {
                var hasHand = grabbable.Grabbable.LeftHand || grabbable.Grabbable.RightHand;
                grabbable = null;
            }
            return grabbable != null;
        }

        private void FollowLogic() {
            var rigidbody = CurrentGrab.Grabbable.Collider.attachedRigidbody;
            if (!rigidbody) {
                Debug.LogWarning("ForceGrabber: No rigidbody found on grabbed object.");
                Debug.Log("If it uses an articulated body then im too lazy");
                return;
            }
            Vector3 distance = hand.PalmTransform.position - rigidbody.transform.position;
            CurrentGrab.Grabbable.Grab(hand);
            if (Vector3.Distance(hand.PalmTransform.position, rigidbody.transform.position) <= 0.2f) {
                CurrentGrab.Grabbable.Grab(hand);
                CurrentGrab = null;
                return;
            }
            rigidbody.linearVelocity = distance * CurrentGrab.speed;
            rigidbody.angularVelocity = Vector3.zero;
            rigidbody.rotation = hand.PalmTransform.rotation;
        }

        private void LineRendererLogic() {
            if (CurrentGrab) {
                lineRenderer.enabled = true;
                lineRenderer.SetPosition(0, hand.PalmTransform.position);
                lineRenderer.SetPosition(1, CurrentGrab.Grabbable.Collider.attachedRigidbody.transform.position);
            }
            else {
                lineRenderer.enabled = false;
            }
        }
    }
}
