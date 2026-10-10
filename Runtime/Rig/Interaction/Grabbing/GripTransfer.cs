using KadenZombie8.BIMOS.Rig.Grips;
using UnityEngine;

namespace KadenZombie8.BIMOS.Rig
{
    /// <summary>
    /// Transfers a hand from one grip to another
    /// </summary>
    public class GripTransfer : MonoBehaviour
    {
        [SerializeField]
        private LeftRightGrips _transferGrips;

        public void TransferGrip(Grip grip)
        {
            var leftHand = grip.LeftHand;
            var rightHand = grip.RightHand;
            if (leftHand && _transferGrips.Left.isActiveAndEnabled)
            {
                grip.Release(leftHand);
                _transferGrips.Left.Grab(leftHand);
            }
            if (rightHand && _transferGrips.Right.isActiveAndEnabled)
            {
                grip.Release(rightHand);
                _transferGrips.Right.Grab(rightHand);
            }
        }
    }
}
