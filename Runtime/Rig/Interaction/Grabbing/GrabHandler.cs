using System;
using UnityEngine;

namespace KadenZombie8.BIMOS.Rig
{
    public class GrabHandler : MonoBehaviour
    {
        public event Action
            OnGrab,
            OnRelease;

        [SerializeField]
        private Hand _hand;

        [SerializeField]
        private Transform _grabBounds;

        [SerializeField]
        private HandPose _hoverHandPose, _defaultGrabHandPose;

        private Grabbable _chosenGrab;

        private void Update()
        {
            if (_hand.CurrentGrab) //If the hand isn't holding something
                return;

            _chosenGrab = GetChosenGrab(); //Get the grab the player is hovering over

            bool grabInRange = _chosenGrab && _hand.HandInputReader.Grip < 0.5f;
            _hand.HandAnimator.HandPose = grabInRange ? _hoverHandPose : _hand.HandAnimator.DefaultHandPose;
        }

        public void ApplyGrabPose(HandPose handPose)
        {
            if (!handPose)
                handPose = _defaultGrabHandPose;

            _hand.HandAnimator.HandPose = handPose;
        }

        private Grabbable GetChosenGrab()
        {
            Collider[] grabColliders = Physics.OverlapBox(_grabBounds.position, _grabBounds.localScale / 2, _grabBounds.rotation, Physics.AllLayers, QueryTriggerInteraction.Collide); //Get all grabs in the grab bounds
            float highestRank = 0;
            Grabbable highestRankGrab = null;

            foreach (Collider grabCollider in grabColliders) //Loop through found grab colliders to find grab with highest rank
            {
                Grabbable grabbable = grabCollider.GetComponent<Grabbable>();

                if (!grabbable)
                    grabbable = grabCollider.GetComponentInParent<Grabbable>();

                if (!grabbable)
                    continue;

                if (!grabbable.enabled)
                    continue;

                var snapGrabbable = grabbable as SnapGrabbable;
                if (snapGrabbable && snapGrabbable.Handedness != _hand.Handedness) //If grab exists and is for the appropriate hand
                    continue;

                float grabRank = grabbable.CalculateRank(_hand);

                if (grabRank <= highestRank || grabRank <= 0f)
                    continue;

                highestRank = grabRank;
                highestRankGrab = grabbable;
            }

            if (highestRank <= 0f)
                return null;

            return highestRankGrab; //Return the grab with the highest rank
        }

        public void AttemptGrab()
        {
            if (!_chosenGrab)
                return;

            _chosenGrab.Grab(_hand);
            OnGrab?.Invoke();
        }

        public void AttemptRelease()
        {
            if (!_hand.CurrentGrab)
                return;

            _hand.CurrentGrab.Release(_hand);
            OnRelease?.Invoke();
        }

        private void OnDisable() => AttemptRelease();
    }
}