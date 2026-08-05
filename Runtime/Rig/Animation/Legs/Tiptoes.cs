using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace KadenZombie8.BIMOS.Rig.Animation
{
    /// <summary>
    /// Angles the feet down when they lose contact with the ground.
    /// </summary>
    [DefaultExecutionOrder(2)]
    [RequireComponent(typeof(TwoBoneIKConstraint))]
    public class Tiptoes : MonoBehaviour
    {
        [SerializeField]
        private Transform _target;

        private Transform _foot;

        private float _footLength;

        private TwoBoneIKConstraint _twoBoneIKConstraint;

        private void Awake()
        {
            _twoBoneIKConstraint = GetComponent<TwoBoneIKConstraint>();

            _foot = _twoBoneIKConstraint.data.tip;
            var toes = _foot.GetChild(0);

            _footLength = toes ? Vector3.Distance(_foot.position, toes.position) : 0.1f;
        }

        private void LateUpdate()
        {
            var heelHeight = _foot.position.y - _target.position.y;
            if (heelHeight < 0f) return;

            var angle = Mathf.Asin(heelHeight / _footLength) * Mathf.Rad2Deg;
            if (float.IsNaN(angle)) return;

            _foot.localEulerAngles += Vector3.left * angle;
        }
    }
}
