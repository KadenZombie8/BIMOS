using System;

namespace KadenZombie8.BIMOS.Rig
{
    public class BoxGrabbable : AutoGrabbable
    {
        [Flags]
        public enum BoxFaces
        {
            Front = 1,
            Back = 2,
            Left = 4,
            Right = 8,
            Top = 16,
            Bottom = 32
        }

        public BoxFaces EnabledFaces =
            BoxFaces.Front |
            BoxFaces.Back |
            BoxFaces.Left |
            BoxFaces.Right |
            BoxFaces.Top |
            BoxFaces.Bottom;
    }
}
