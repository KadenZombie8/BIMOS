using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KadenZombie8.BIMOS.Ragdoll.Editor
{
	/// <summary>
	/// Abstract class. Represents ragdoll part. For example: legUpper, head, pelvis or something else
	/// </summary>
	abstract class RagdollPartBase
	{
		public readonly Transform transform;
		public Rigidbody rigidbody;
		public CharacterJoint joint;

		protected RagdollPartBase(Animator physicsAnimator, Animator virtualAnimator, HumanBodyBones bone)
		{
			transform = physicsAnimator.GetBoneTransform(bone);
			var physicsLimb = transform.gameObject.AddComponent<PhysicsLimb>();
            physicsLimb.virtualLimb = virtualAnimator.GetBoneTransform(bone);
		}
	}
	/// <summary>
	/// Ragoll part for Box collider
	/// </summary>
	sealed class RagdollPartBox : RagdollPartBase
	{
		public BoxCollider collider;

		public RagdollPartBox(Animator physicsAnimator, Animator virtualAnimator, HumanBodyBones bone) : base(physicsAnimator, virtualAnimator, bone) { }
	}
	/// <summary>
	/// Ragoll part for Capsule collider
	/// </summary>
	sealed class RagdollPartCapsule : RagdollPartBase
	{
		public CapsuleCollider collider;

		public RagdollPartCapsule(Animator physicsAnimator, Animator virtualAnimator, HumanBodyBones bone) : base(physicsAnimator, virtualAnimator, bone) { }
    }
	/// <summary>
	/// Ragoll part for Sphere collider
	/// </summary>
	sealed class RagdollPartSphere : RagdollPartBase
	{
		public SphereCollider collider;

		public RagdollPartSphere(Animator physicsAnimator, Animator virtualAnimator, HumanBodyBones bone) : base(physicsAnimator, virtualAnimator, bone) { }
    }
}