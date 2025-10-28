using System;
using UnityEngine;

namespace KadenZombie8.BIMOS.ImpactProperties
{
    public class MaterialGiver : MonoBehaviour
    {
        public Material Material;
    }

    [Serializable]
    public struct ImpulseRange
    {
        public float Minimum;
        public float Maximum;
    }
}
