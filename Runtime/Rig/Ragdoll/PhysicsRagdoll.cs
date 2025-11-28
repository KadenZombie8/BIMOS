using System.Collections.Generic;
using UnityEngine;

namespace KadenZombie8.BIMOS.Ragdoll
{
    [DisallowMultipleComponent]
    public class PhysicsRagdoll : MonoBehaviour
    {
        public static List<PhysicsRagdoll> Ragdolls = new();   
        public List<PhysicsLimb> Limbs {
            get; set;
        } = new(); 
        public Animator Animator {
            get; private set;
        }

        public float forceMultiplier = 1f;
        public float inertiaTensorResitance = 0.2f;
        public float proportionalGain = 10f;
        public float derivativeGain = 1f;
        public float maxStrength = 100f;

        private void OnEnable() {
            Animator = GetComponentInChildren<Animator>();
            Ragdolls.Add(this);
        }

        private void OnDisable() {
            Ragdolls.Remove(this);
        }
    }
}
