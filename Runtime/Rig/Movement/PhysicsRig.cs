using System;
using UnityEngine;

namespace KadenZombie8.BIMOS.Rig.Movement
{
    /// <summary>
    /// Reference bin for physics rig components
    /// </summary>
    public class PhysicsRig : MonoBehaviour
    {
        public PhysicsRigRigidbodies Rigidbodies;
        public PhysicsRigColliders Colliders;
        public PhysicsRigJoints Joints;
        public PhysicsRigJointDrives JointDrives;
        public PhysicsRigGrabHandlers GrabHandlers;

        [HideInInspector]
        public LayerMask PlayerLayerMask;

        [HideInInspector]
        public SmoothLocomotion Movement;

        [HideInInspector]
        public Crouching Crouching;

        [HideInInspector]
        public LocomotionSphere LocomotionSphere;

        private void Awake()
        {
            PlayerLayerMask = LayerMask.GetMask("Player");
            Crouching = GetComponent<Crouching>();
            Movement = GetComponent<SmoothLocomotion>();
            LocomotionSphere = Rigidbodies.LocomotionSphere.GetComponent<LocomotionSphere>();
            InitializeJointDrives();
        }

        private void InitializeJointDrives()
        {
            // Joint drives
            JointDrives.Pelvis = Joints.Pelvis.xDrive;
            JointDrives.Head = Joints.Head.slerpDrive;
            JointDrives.LeftHand = Joints.LeftHand.xDrive;
            JointDrives.RightHand = Joints.RightHand.xDrive;

            var fullHeight = Crouching.MaxStandingLegHeight - Crouching.CrawlingLegHeight;

            // Anchors
            Joints.Pelvis.connectedAnchor = Vector3.up * fullHeight / 2f;

            // Limits
            Joints.Pelvis.linearLimit = new SoftJointLimit
            {
                limit = fullHeight / 2f
            };

            // Fix for Unity's bad joint implementation
            Joints.Pelvis.connectedBody = Rigidbodies.Knee;
        }
    }

    [Serializable]
    public struct PhysicsRigRigidbodies
    {
        public Rigidbody LocomotionSphere;
        public Rigidbody Knee;
        public Rigidbody Pelvis;
        public Rigidbody Head;
        public ArmRigidbodies LeftArm;
        public ArmRigidbodies RightArm;
    }

    [Serializable]
    public struct ArmRigidbodies
    {
        public Rigidbody UpperArm;
        public Rigidbody LowerArm;
        public Rigidbody Hand;
    }

    [Serializable]
    public struct PhysicsRigColliders
    {
        public SphereCollider LocomotionSphere;
        public CapsuleCollider Body;
        public CapsuleCollider Head;
        public ArmColliders LeftArm;
        public ArmColliders RightArm;
    }

    [Serializable]
    public struct ArmColliders
    {
        public CapsuleCollider UpperArm;
        public CapsuleCollider LowerArm;
        public BoxCollider Hand;
    }

    [Serializable]
    public struct PhysicsRigJoints
    {
        public ConfigurableJoint Knee;
        public ConfigurableJoint Pelvis;
        public ConfigurableJoint Head;
        public ConfigurableJoint LeftHand;
        public ConfigurableJoint RightHand;
    }

    [Serializable]
    public struct PhysicsRigJointDrives
    {
        public JointDrive Pelvis;
        public JointDrive Head;
        public JointDrive LeftHand;
        public JointDrive RightHand;
    }

    [Serializable]
    public struct PhysicsRigGrabHandlers
    {
        public GrabHandler Left;
        public GrabHandler Right;
    }
}