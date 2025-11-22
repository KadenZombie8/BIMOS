using System;
using UnityEngine;

namespace KadenZombie8.BIMOS.Rig
{
    [RequireComponent(typeof(Item))]
    public class Storable : MonoBehaviour
    {
        public event Action OnStored;
        public event Action OnRetrieved;

        public string[] Tags = { "Light" };
        public RetrieveGrabbablesStruct RetrieveGrabbables;

        [Serializable]
        public struct RetrieveGrabbablesStruct
        {
            public Grabbable Left;
            public Grabbable Right;
        }
    }
}
