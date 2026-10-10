using KadenZombie8.BIMOS.Rig.Grips;
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

        public Transform GrabBounds;

        [SerializeField]
        private HandPose _hoverHandPose, _defaultGrabHandPose;

        private Grip _chosenGrab;
        private bool _wasGrabInRange = false;

        private const float _grabHapticDuration = 0.05f;

        private void Update()
        {
            if (_hand.CurrentGrip) //If the hand isn't holding something
                return;

            _chosenGrab = GetChosenGrab(); //Get the grab the player is hovering over

            bool isGrabInRange = _chosenGrab && _hand.HandInputReader.Grip < 0.5f;
            _hand.HandAnimator.HandPose = isGrabInRange ? _hoverHandPose : _hand.HandAnimator.DefaultHandPose;

            if (isGrabInRange && !_wasGrabInRange)
                _hand.SendHapticImpulse(0.025f, _grabHapticDuration);

            _wasGrabInRange = isGrabInRange;
        }

        public void ApplyGrabPose(HandPose handPose)
        {
            if (!handPose)
                handPose = _defaultGrabHandPose;

            _hand.HandAnimator.HandPose = handPose;
        }

        private Grip GetChosenGrab()
        {
            var grabColliders = Physics.OverlapBox(GrabBounds.position, GrabBounds.localScale / 2f, GrabBounds.rotation, Physics.AllLayers, QueryTriggerInteraction.Collide); //Get all grabs in the grab bounds
            float highestRank = 0;
            Grip highestRankGrab = null;

            foreach (var gripCollider in grabColliders) //Loop through found grab colliders to find grab with highest rank
            {
                var grip = gripCollider.GetComponent<Grip>();

                if (!grip)
                    grip = gripCollider.GetComponentInParent<Grip>();

                if (!grip)
                    continue;

                if (!grip.isActiveAndEnabled)
                    continue;

                var snapGrip = grip as TargetGrip;
                if (snapGrip && snapGrip.Handedness != _hand.Handedness) //If grab exists and is for the appropriate hand
                    continue;

                var grabRank = grip.CalculateRank(_hand);

                if (grabRank <= highestRank || grabRank <= 0f)
                    continue;

                highestRank = grabRank;
                highestRankGrab = grip;
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
            if (!_hand.CurrentGrip)
                return;

            OnRelease?.Invoke();
            _hand.SendHapticImpulse(0.1f, _grabHapticDuration);
            _hand.CurrentGrip.Release(_hand);
        }

        private void OnDisable() => AttemptRelease();
    }
}