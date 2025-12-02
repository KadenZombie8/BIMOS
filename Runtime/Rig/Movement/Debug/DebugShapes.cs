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
            // Body
            _locomotionSphere.localScale = _colliders.LocomotionSphere.radius * 2f * Vector3.one;
            UpdateCapsuleShape(_body, _colliders.Body);
            UpdateCapsuleShape(_head, _colliders.Head);

            // Left arm
            UpdateCapsuleShape(_leftUpperArm, _colliders.LeftArm.UpperArm);
            UpdateCapsuleShape(_leftLowerArm, _colliders.LeftArm.LowerArm);
            _leftHand.localScale = _colliders.LeftArm.Hand.size;

            // Right arm
            UpdateCapsuleShape(_rightUpperArm, _colliders.RightArm.UpperArm);
            UpdateCapsuleShape(_rightLowerArm, _colliders.RightArm.LowerArm);
            _rightHand.localScale = _colliders.RightArm.Hand.size;
        }

        private void UpdateCapsuleShape(Transform visual, CapsuleCollider collider)
        {
            visual.localScale = new(collider.radius * 2f, collider.height / 2f, collider.radius * 2f);
            visual.localPosition = collider.center;
        }
    }
}