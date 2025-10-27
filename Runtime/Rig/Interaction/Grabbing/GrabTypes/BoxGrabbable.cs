using UnityEngine;
using System;

namespace KadenZombie8.BIMOS.Rig
{
    public class BoxGrabbable : AutoGrabbable
    {
        [Serializable]
        public struct BoxFaces
        {
            public bool Front;
            public bool Back;
            public bool Left;
            public bool Right;
            public bool Top;
            public bool Bottom;
        }

        public BoxFaces EnabledFaces = new()
        {
            Front = true,
            Back = true,
            Left = true,
            Right = true,
            Top = true,
            Bottom = true
        };
    }
}
