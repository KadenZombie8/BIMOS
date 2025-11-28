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
            _leftHand,
            _rightHand;

        [SerializeField]
        private bool _isVisible;

        private PhysicsRigColliders _colliders;

        private void Start() => _colliders = GetComponentInParent<BIMOSRig>().PhysicsRig.Colliders;

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
            _leftHand.gameObject.SetActive(isVisible);
            _rightHand.gameObject.SetActive(isVisible);
        }

        private void LateUpdate()
        {
            _locomotionSphere.localScale = _colliders.LocomotionSphere.radius * 2f * Vector3.one;
            _body.localScale = new(_colliders.Body.radius * 2f, _colliders.Body.height / 2f, _colliders.Body.radius * 2f);
            _head.localScale = new(_colliders.Head.radius * 2f, _colliders.Head.height / 2f, _colliders.Head.radius * 2f);
            _leftHand.localScale = _colliders.LeftHand.size;
            _rightHand.localScale = _colliders.RightHand.size;
        }
    }
}