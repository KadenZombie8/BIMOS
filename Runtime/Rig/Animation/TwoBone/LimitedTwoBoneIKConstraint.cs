using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace KadenZombie8.BIMOS.AnimationRigging
{
    public class LimitedTwoBoneIKConstraint : RigConstraint<
        LimitedTwoBoneIKConstraintJob,
        LimitedTwoBoneIKConstraintData,
        LimitedTwoBoneIKConstraintJobBinder<LimitedTwoBoneIKConstraintData>
        >
    {
        /// <inheritdoc />
        protected override void OnValidate()
        {
            base.OnValidate();
            m_Data.hintWeight = Mathf.Clamp01(m_Data.hintWeight);
            m_Data.targetPositionWeight = Mathf.Clamp01(m_Data.targetPositionWeight);
            m_Data.targetRotationWeight = Mathf.Clamp01(m_Data.targetRotationWeight);
        }
    }
}
