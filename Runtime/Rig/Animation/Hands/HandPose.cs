using System;
using UnityEngine;

namespace KadenZombie8.BIMOS.Rig
{
    [CreateAssetMenu(fileName = "HandPose", menuName = "BIMOS/Hand Pose")]
    public class HandPose : ScriptableObject
    {
        public ThumbPoses Thumb = new();
        public IndexPoses Index = new();
        public FingerPoses Middle = new();
        public FingerPoses Ring = new();
        public FingerPoses Little = new();
    }

    [Serializable]
    public struct FingerPose
    {
        public Quaternion RootBone, MiddleBone, TipBone;

        public static FingerPose Default()
        {
            FingerPose fingerPose = new()
            {
                RootBone = Quaternion.identity,
                MiddleBone = Quaternion.identity,
                TipBone = Quaternion.identity
            };
            return fingerPose;
        }

        public FingerPose(FingerPose pose)
        {
            RootBone = pose.RootBone;
            MiddleBone = pose.MiddleBone;
            TipBone = pose.TipBone;
        }

        public readonly FingerPose Mirrored()
        {
            FingerPose mirroredFingerPose = new(this);

            mirroredFingerPose.RootBone.x *= -1f;
            mirroredFingerPose.MiddleBone.x *= -1f;
            mirroredFingerPose.TipBone.x *= -1f;

            mirroredFingerPose.RootBone.w *= -1f;
            mirroredFingerPose.MiddleBone.w *= -1f;
            mirroredFingerPose.TipBone.w *= -1f;

            return mirroredFingerPose;
        }
    }

    [Serializable]
    public class ThumbPoses
    {
        public FingerPose
            Idle = FingerPose.Default(),
            ThumbrestTouched = FingerPose.Default(),
            PrimaryTouched = FingerPose.Default(),
            PrimaryButton = FingerPose.Default(),
            SecondaryTouched = FingerPose.Default(),
            SecondaryButton = FingerPose.Default(),
            ThumbstickTouched = FingerPose.Default();
    }

    [Serializable]
    public class FingerPoses
    {
        public FingerPose
            Open = FingerPose.Default(),
            Closed = FingerPose.Default();
    }

    [Serializable]
    public class IndexPoses : FingerPoses
    {
        public FingerPose TriggerTouched = FingerPose.Default();
    }
}
