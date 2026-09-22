using System;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Animations.Rigging;

namespace KadenZombie8.BIMOS.AnimationRigging
{
    [Serializable]
    public struct LimitedTwoBoneIKConstraintData : IAnimationJobData, ITwoBoneIKConstraintData
    {
        [SerializeField]
        private Transform m_Root;

        [SerializeField]
        private Transform m_Mid;

        [SerializeField]
        private Transform m_Tip;

        [SyncSceneToStream]
        [SerializeField]
        private Transform m_Target;

        [SyncSceneToStream]
        [SerializeField]
        private Transform m_Hint;

        [SyncSceneToStream]
        [SerializeField]
        [Range(0f, 1f)]
        private float m_TargetPositionWeight;

        [SyncSceneToStream]
        [SerializeField]
        [Range(0f, 1f)]
        private float m_TargetRotationWeight;

        [SyncSceneToStream]
        [SerializeField]
        [Range(0f, 1f)]
        private float m_HintWeight;

        [NotKeyable]
        [SerializeField]
        private bool m_MaintainTargetPositionOffset;

        [NotKeyable]
        [SerializeField]
        private bool m_MaintainTargetRotationOffset;

        public Transform root
        {
            get
            {
                return m_Root;
            }
            set
            {
                m_Root = value;
            }
        }

        public Transform mid
        {
            get
            {
                return m_Mid;
            }
            set
            {
                m_Mid = value;
            }
        }

        public Transform tip
        {
            get
            {
                return m_Tip;
            }
            set
            {
                m_Tip = value;
            }
        }

        public Transform target
        {
            get
            {
                return m_Target;
            }
            set
            {
                m_Target = value;
            }
        }

        public Transform hint
        {
            get
            {
                return m_Hint;
            }
            set
            {
                m_Hint = value;
            }
        }

        public float targetPositionWeight
        {
            get
            {
                return m_TargetPositionWeight;
            }
            set
            {
                m_TargetPositionWeight = Mathf.Clamp01(value);
            }
        }

        public float targetRotationWeight
        {
            get
            {
                return m_TargetRotationWeight;
            }
            set
            {
                m_TargetRotationWeight = Mathf.Clamp01(value);
            }
        }

        public float hintWeight
        {
            get
            {
                return m_HintWeight;
            }
            set
            {
                m_HintWeight = Mathf.Clamp01(value);
            }
        }

        public bool maintainTargetPositionOffset
        {
            get
            {
                return m_MaintainTargetPositionOffset;
            }
            set
            {
                m_MaintainTargetPositionOffset = value;
            }
        }

        public bool maintainTargetRotationOffset
        {
            get
            {
                return m_MaintainTargetRotationOffset;
            }
            set
            {
                m_MaintainTargetRotationOffset = value;
            }
        }

        string ITwoBoneIKConstraintData.targetPositionWeightFloatProperty => ConstraintsUtils.ConstructConstraintDataPropertyName("m_TargetPositionWeight");

        string ITwoBoneIKConstraintData.targetRotationWeightFloatProperty => ConstraintsUtils.ConstructConstraintDataPropertyName("m_TargetRotationWeight");

        string ITwoBoneIKConstraintData.hintWeightFloatProperty => ConstraintsUtils.ConstructConstraintDataPropertyName("m_HintWeight");

        bool IAnimationJobData.IsValid()
        {
            if (m_Tip != null && m_Mid != null && m_Root != null && m_Target != null && m_Tip.IsChildOf(m_Mid))
            {
                return m_Mid.IsChildOf(m_Root);
            }

            return false;
        }

        void IAnimationJobData.SetDefaultValues()
        {
            m_Root = null;
            m_Mid = null;
            m_Tip = null;
            m_Target = null;
            m_Hint = null;
            m_TargetPositionWeight = 1f;
            m_TargetRotationWeight = 1f;
            m_HintWeight = 1f;
            m_MaintainTargetPositionOffset = false;
            m_MaintainTargetRotationOffset = false;
        }
    }
}