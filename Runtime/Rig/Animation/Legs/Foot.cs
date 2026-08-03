using System;
using UnityEngine;

namespace KadenZombie8.BIMOS.Rig.Animation
{
    public class Foot : MonoBehaviour
    {
        public event Action<bool> OnStep;

        public Transform Anchor;
        public Transform Target;
        public float Offset;
        public bool IsGrounded { get; set; }

        public void Step(bool isRunning) => OnStep?.Invoke(isRunning);
    }
}
