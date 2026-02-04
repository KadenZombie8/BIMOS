using UnityEngine;

namespace KadenZombie8.BIMOS.Rig
{
    /// <summary>
    /// Transfers a hand from one grabbable to another
    /// </summary>
    public class GrabbableTransfer : MonoBehaviour
    {
        [SerializeField]
        private LeftRightGrabbables _transferGrabbables;

        public void TransferGrabbable(Grabbable grabbable)
        {
            var leftHand = grabbable.LeftHand;
            var rightHand = grabbable.RightHand;
            if (leftHand && _transferGrabbables.Left.isActiveAndEnabled)
            {
                grabbable.Release(leftHand);
                _transferGrabbables.Left.Grab(leftHand);
            }
            if (rightHand && _transferGrabbables.Right.isActiveAndEnabled)
            {
                grabbable.Release(rightHand);
                _transferGrabbables.Right.Grab(rightHand);
            }
        }
    }
}
