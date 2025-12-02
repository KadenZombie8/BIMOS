using UnityEngine;
using UnityEngine.InputSystem;

namespace KadenZombie8.BIMOS.Rig.Movement
{
    /// <summary>
    /// Draws the shapes of the hexabody configuration
    /// for easier debugging.
    /// </summary>
    public class DebugShapes : MonoBehaviour
    {
        public InputAction Action;

        [SerializeField]
        private Transform
            _locomotionSphere,
            _body,
            _head,
            _leftUpperArm,
            _leftLowerArm,
            _leftHand,
            _rightUpperArm,
            _rightLowerArm,
            _rightHand;

        [SerializeField]
        private bool _isVisible;

        private PhysicsRigColliders _colliders;

        private void Start() => _colliders = BIMOSRig.Instance.PhysicsRig.Colliders;

        private void OnEnable()
        {
            Action.Enable();
            Action.performed += ToggleDebugShapes;

            SetDebugShapesVisible(_isVisible);
        }

        private void OnDisable()
        {
            Action.performed -= ToggleDebugShapes;
        }

        private void ToggleDebugShapes(InputAction.CallbackContext context)
        {
            _isVisible = !_isVisible;
            SetDebugShapesVisible(_isVisible);
        }

        private void SetDebugShapesVisible(bool isVisible)
        {
            _locomotionSphere.gameObject.SetActive(isVisible);
            _body.gameObject.SetActive(isVisible);
            _head.gameObject.SetActive(isVisible);
            _leftUpperArm.gameObject.SetActive(isVisible);
            _leftLowerArm.gameObject.SetActive(isVisible);
            _leftHand.gameObject.SetActive(isVisible);
            _rightUpperArm.gameObject.SetActive(isVisible);
            _rightLowerArm.gameObject.SetActive(isVisible);
            _rightHand.gameObject.SetActive(isVisible);
        }

        private void LateUpdate()
        {
            _locomotionSphere.localScale = _colliders.LocomotionSphere.radius * 2f * Vector3.one;
            _body.localScale = new(_colliders.Body.radius * 2f, _colliders.Body.height / 2f, _colliders.Body.radius * 2f);
            _head.localScale = new(_colliders.Head.radius * 2f, _colliders.Head.height / 2f, _colliders.Head.radius * 2f);

            // Left arm
            _leftUpperArm.localScale = new(_colliders.LeftArm.UpperArm.radius * 2f, _colliders.LeftArm.UpperArm.height / 2f, _colliders.LeftArm.UpperArm.radius * 2f);
            _leftUpperArm.localPosition = _colliders.LeftArm.UpperArm.center;

            _leftLowerArm.localScale = new(_colliders.LeftArm.LowerArm.radius * 2f, _colliders.LeftArm.LowerArm.height / 2f, _colliders.LeftArm.LowerArm.radius * 2f);
            _leftLowerArm.localPosition = _colliders.LeftArm.LowerArm.center;

            _leftHand.localScale = _colliders.LeftArm.Hand.size;

            // Right arm
            _rightUpperArm.localScale = new(_colliders.RightArm.UpperArm.radius * 2f, _colliders.RightArm.UpperArm.height / 2f, _colliders.RightArm.UpperArm.radius * 2f);
            _rightUpperArm.localPosition = _colliders.RightArm.UpperArm.center;

            _rightLowerArm.localScale = new(_colliders.RightArm.LowerArm.radius * 2f, _colliders.RightArm.LowerArm.height / 2f, _colliders.RightArm.LowerArm.radius * 2f);
            _rightLowerArm.localPosition = _colliders.RightArm.LowerArm.center;

            _rightHand.localScale = _colliders.RightArm.Hand.size;
        }
    }
}