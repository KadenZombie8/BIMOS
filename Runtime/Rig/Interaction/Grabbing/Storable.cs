using System;
using UnityEngine;

namespace KadenZombie8.BIMOS.Rig
{
    public class Storable : MonoBehaviour
    {
        public string[] Tags = { "Light" };
        public GrabbablesStruct Grabbables;

        [Serializable]
        public struct GrabbablesStruct
        {
            public Grabbable Left;
            public Grabbable Right;
        }
    }
}
