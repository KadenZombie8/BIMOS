using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Hands;
using UnityEngine.XR.Hands.Gestures;

namespace KadenZombie8.BIMOS.Rig
{
    public class HandAnimator : MonoBehaviour
    {
        public Handedness Handedness;

        [SerializeField]
        private Animator _animator;

        [SerializeField]
        private HandInputReader _handInputReader;

        [SerializeField]
        private Transform _handTarget;

        private Transform _hand;

        public HandPose DefaultHandPose, HandPose;

        [HideInInspector]
        public Transform[]
            Thumb,
            Index,
            Middle,
            Ring,
            Little;

        [HideInInspector]
        public Quaternion[]
            ThumbRot,
            IndexRot,
            MiddleRot,
            RingRot,
            LittleRot;

        public ThumbSubPoses ThumbPose = ThumbSubPoses.Idle;
        public enum ThumbSubPoses
        {
            Idle,
            ThumbrestTouched,
            PrimaryTouched,
            PrimaryButton,
            SecondaryTouched,
            SecondaryButton,
            ThumbstickTouched
        };
        public bool IsIndexOnTrigger;
        public float
            IndexCurl,
            MiddleCurl,
            RingCurl,
            LittleCurl;

        private readonly List<XRHandSubsystem> _handSubsystems = new();
        private XRHandSubsystem _handSubsystem;
        private readonly XRFingerShape[] _fingerShapes = new XRFingerShape[5];
        private XRHand _subsystemHand;

        private bool TryGetSubsystem(out XRHandSubsystem system)
        {
            system = null;

            if (_handSubsystems.Count == 0)
                SubsystemManager.GetSubsystems(_handSubsystems);

            if (_handSubsystems.Count > 0)
            {
                system = _handSubsystems[0];
                _subsystemHand = Handedness == Handedness.Left ? system.leftHand : system.rightHand;
                return true;
            }

            return false;
        }

        private void Awake()
        {
            Thumb = new Transform[3];
            Index = new Transform[3];
            Middle = new Transform[3];
            Ring = new Transform[3];
            Little = new Transform[3];
            ThumbRot = new Quaternion[3];
            IndexRot = new Quaternion[3];
            MiddleRot = new Quaternion[3];
            RingRot = new Quaternion[3];
            LittleRot = new Quaternion[3];
            if (Handedness == Handedness.Left)
            {
                _hand = _animator.GetBoneTransform(HumanBodyBones.LeftHand);
                Thumb[0] = _animator.GetBoneTransform(HumanBodyBones.LeftThumbProximal);
                Thumb[1] = _animator.GetBoneTransform(HumanBodyBones.LeftThumbIntermediate);
                Thumb[2] = _animator.GetBoneTransform(HumanBodyBones.LeftThumbDistal);
                Index[0] = _animator.GetBoneTransform(HumanBodyBones.LeftIndexProximal);
                Index[1] = _animator.GetBoneTransform(HumanBodyBones.LeftIndexIntermediate);
                Index[2] = _animator.GetBoneTransform(HumanBodyBones.LeftIndexDistal);
                Middle[0] = _animator.GetBoneTransform(HumanBodyBones.LeftMiddleProximal);
                Middle[1] = _animator.GetBoneTransform(HumanBodyBones.LeftMiddleIntermediate);
                Middle[2] = _animator.GetBoneTransform(HumanBodyBones.LeftMiddleDistal);
                Ring[0] = _animator.GetBoneTransform(HumanBodyBones.LeftRingProximal);
                Ring[1] = _animator.GetBoneTransform(HumanBodyBones.LeftRingIntermediate);
                Ring[2] = _animator.GetBoneTransform(HumanBodyBones.LeftRingDistal);
                Little[0] = _animator.GetBoneTransform(HumanBodyBones.LeftLittleProximal);
                Little[1] = _animator.GetBoneTransform(HumanBodyBones.LeftLittleIntermediate);
                Little[2] = _animator.GetBoneTransform(HumanBodyBones.LeftLittleDistal);
            }
            else
            {
                _hand = _animator.GetBoneTransform(HumanBodyBones.RightHand);
                Thumb[0] = _animator.GetBoneTransform(HumanBodyBones.RightThumbProximal);
                Thumb[1] = _animator.GetBoneTransform(HumanBodyBones.RightThumbIntermediate);
                Thumb[2] = _animator.GetBoneTransform(HumanBodyBones.RightThumbDistal);
                Index[0] = _animator.GetBoneTransform(HumanBodyBones.RightIndexProximal);
                Index[1] = _animator.GetBoneTransform(HumanBodyBones.RightIndexIntermediate);
                Index[2] = _animator.GetBoneTransform(HumanBodyBones.RightIndexDistal);
                Middle[0] = _animator.GetBoneTransform(HumanBodyBones.RightMiddleProximal);
                Middle[1] = _animator.GetBoneTransform(HumanBodyBones.RightMiddleIntermediate);
                Middle[2] = _animator.GetBoneTransform(HumanBodyBones.RightMiddleDistal);
                Ring[0] = _animator.GetBoneTransform(HumanBodyBones.RightRingProximal);
                Ring[1] = _animator.GetBoneTransform(HumanBodyBones.RightRingIntermediate);
                Ring[2] = _animator.GetBoneTransform(HumanBodyBones.RightRingDistal);
                Little[0] = _animator.GetBoneTransform(HumanBodyBones.RightLittleProximal);
                Little[1] = _animator.GetBoneTransform(HumanBodyBones.RightLittleIntermediate);
                Little[2] = _animator.GetBoneTransform(HumanBodyBones.RightLittleDistal);
            }

            ThumbRot[0] = Quaternion.identity;
            ThumbRot[1] = Quaternion.identity;
            ThumbRot[2] = Quaternion.identity;
            IndexRot[0] = Quaternion.identity;
            IndexRot[1] = Quaternion.identity;
            IndexRot[2] = Quaternion.identity;
            MiddleRot[0] = Quaternion.identity;
            MiddleRot[1] = Quaternion.identity;
            MiddleRot[2] = Quaternion.identity;
            RingRot[0] = Quaternion.identity;
            RingRot[1] = Quaternion.identity;
            RingRot[2] = Quaternion.identity;
            LittleRot[0] = Quaternion.identity;
            LittleRot[1] = Quaternion.identity;
            LittleRot[2] = Quaternion.identity;
        }

        private void LateUpdate()
        {
            if (!TryGetSubsystem(out _handSubsystem))
                return;

            _hand.SetPositionAndRotation(_handTarget.position, _handTarget.rotation);
            UpdateCurls();
            UpdateHand();
        }

        private void UpdateCurls()
        {
            if (_handInputReader.SecondaryButton)
                ThumbPose = ThumbSubPoses.SecondaryButton;
            else if (_handInputReader.PrimaryButton)
                ThumbPose = ThumbSubPoses.PrimaryButton;
            else if (_handInputReader.SecondaryTouched)
                ThumbPose = ThumbSubPoses.SecondaryTouched;
            else if (_handInputReader.PrimaryTouched)
                ThumbPose = ThumbSubPoses.PrimaryTouched;
            else if (_handInputReader.ThumbstickTouched)
                ThumbPose = ThumbSubPoses.ThumbstickTouched;
            else if (_handInputReader.ThumbrestTouched)
                ThumbPose = ThumbSubPoses.ThumbrestTouched;
            else
                ThumbPose = ThumbSubPoses.Idle;

            IndexCurl = _handInputReader.Trigger;
            IsIndexOnTrigger = _handInputReader.TriggerTouched;
            MiddleCurl = TryGetCurl(2, out var middleCurl) ? middleCurl : _handInputReader.Grip;
            RingCurl = TryGetCurl(3, out var ringCurl) ? ringCurl : _handInputReader.Grip;
            LittleCurl = TryGetCurl(4, out var littleCurl) ? littleCurl : _handInputReader.Grip;
        }

        private bool TryGetCurl(int fingerIndex, out float curl)
        {
            var fingerShape = _subsystemHand.CalculateFingerShape(
                    (XRHandFingerID)fingerIndex, XRFingerShapeTypes.All);

            return fingerShape.TryGetFullCurl(out curl);
        }

        private void UpdateHand()
        {
            switch (ThumbPose)
            {
                case ThumbSubPoses.Idle:
                    UpdateFinger(Thumb, ThumbRot, 1, HandPose.Thumb.Idle, HandPose.Thumb.Idle);
                    break;
                case ThumbSubPoses.ThumbrestTouched:
                    UpdateFinger(Thumb, ThumbRot, 1, HandPose.Thumb.ThumbrestTouched, HandPose.Thumb.ThumbrestTouched);
                    break;
                case ThumbSubPoses.PrimaryTouched:
                    UpdateFinger(Thumb, ThumbRot, 1, HandPose.Thumb.PrimaryTouched, HandPose.Thumb.PrimaryTouched);
                    break;
                case ThumbSubPoses.PrimaryButton:
                    UpdateFinger(Thumb, ThumbRot, 1, HandPose.Thumb.PrimaryButton, HandPose.Thumb.PrimaryButton);
                    break;
                case ThumbSubPoses.SecondaryTouched:
                    UpdateFinger(Thumb, ThumbRot, 1, HandPose.Thumb.SecondaryTouched, HandPose.Thumb.SecondaryTouched);
                    break;
                case ThumbSubPoses.SecondaryButton:
                    UpdateFinger(Thumb, ThumbRot, 1, HandPose.Thumb.SecondaryButton, HandPose.Thumb.SecondaryButton);
                    break;
                case ThumbSubPoses.ThumbstickTouched:
                    UpdateFinger(Thumb, ThumbRot, 1, HandPose.Thumb.ThumbstickTouched, HandPose.Thumb.ThumbstickTouched);
                    break;
            }

            if (IsIndexOnTrigger)
            {
                UpdateFinger(Index, IndexRot, IndexCurl, HandPose.Index.TriggerTouched, HandPose.Index.Closed);
            }
            else
            {
                UpdateFinger(Index, IndexRot, IndexCurl, HandPose.Index.Open, HandPose.Index.Closed);
            }

            UpdateFinger(Middle, MiddleRot, MiddleCurl, HandPose.Middle.Open, HandPose.Middle.Closed);
            UpdateFinger(Ring, RingRot, RingCurl, HandPose.Ring.Open, HandPose.Ring.Closed);
            UpdateFinger(Little, LittleRot, LittleCurl, HandPose.Little.Open, HandPose.Little.Closed);
        }

        private void UpdateFinger(Transform[] finger, Quaternion[] rots, float value, FingerPose open, FingerPose closed)
        {
            if (Handedness == Handedness.Left)
            {
                open = open.Mirrored();
                closed = closed.Mirrored();
            }

            //Sets dummy finger bone rotations to those in the fingerPose
            rots[0] = Quaternion.Slerp(rots[0], Quaternion.Slerp(open.RootBone, closed.RootBone, value), Time.deltaTime * 25f); ;
            rots[1] = Quaternion.Slerp(rots[1], Quaternion.Slerp(open.MiddleBone, closed.MiddleBone, value), Time.deltaTime * 25f); ;
            rots[2] = Quaternion.Slerp(rots[2], Quaternion.Slerp(open.TipBone, closed.TipBone, value), Time.deltaTime * 25f);

            if (finger[0])
                finger[0].localRotation *= rots[0];

            if (finger[1])
                finger[1].localRotation *= rots[1];

            if (finger[2])
                finger[2].localRotation *= rots[2];
        }
    }
}