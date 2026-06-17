using UnityEngine;

namespace KadenZombie8.BIMOS.Rig
{
    public class GripChecker : MonoBehaviour
    {
        public Hand Hand;
        public bool IsGripping { get; private set; }

        private void Update()
        {
            bool wasGripping = IsGripping;

            IsGripping = Hand.HandInputReader.Grip >= 0.5f;

            if (!wasGripping && IsGripping)
                Hand.GrabHandler.AttemptGrab();

            if (wasGripping && !IsGripping)
                Hand.GrabHandler.AttemptRelease();
        }
    }
}