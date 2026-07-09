using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace KadenZombie8.BIMOS.Rig.Animation
{
    /// <summary>
    /// Limits target distance of two bone IK constraint, preventing snapping from pole length becoming 0
    /// </summary>
    [DefaultExecutionOrder(2)]
    [RequireComponent(typeof(TwoBoneIKConstraint))]
    public class TwoBoneIKLimiter : MonoBehaviour
    {
        [Tooltip("The direct unlimited transform of the target")]
        public Transform UnlimitedTarget;

        [Tooltip("The indirect limited transform of the target")]
        public Transform LimitedTarget;

        private TwoBoneIKConstraint _twoBoneIKConstraint;
        private float _chainLength;

        private Transform _root;
        private Transform _mid;
        private Transform _tip;

        [SerializeField]
        private float _margin = 0.01f;

        private void Awake()
        {
            _twoBoneIKConstraint = GetComponent<TwoBoneIKConstraint>();

            _root = _twoBoneIKConstraint.data.root;
            _mid = _twoBoneIKConstraint.data.mid;
            _tip = _twoBoneIKConstraint.data.tip;

            var rootMidLength = Vector3.Distance(_root.position, _mid.position);
            var midTipLength = Vector3.Distance(_mid.position, _tip.position);
            _chainLength = rootMidLength + midTipLength;
        }

        private void Update()
        {
            var displacement = UnlimitedTarget.position - _root.position;
            var chainEpsilon = _chainLength - _margin;

            LimitedTarget.SetPositionAndRotation(
                _root.position + Vector3.ClampMagnitude(displacement, chainEpsilon),
                UnlimitedTarget.rotation
            );
        }
    }
}