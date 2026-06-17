using KadenZombie8.BIMOS.Rig;
using KadenZombie8.BIMOS.Rig.Movement;
using KadenZombie8.BIMOS.UI.Options;
using UnityEngine;
using UnityEngine.InputSystem;

namespace KadenZombie8.BIMOS.UI
{
    /// <summary>
    /// Toggles the VR menu when the menu button is pressed
    /// </summary>
    public class MenuToggleVR : MonoBehaviour
    {
        [SerializeField]
        private InputActionReference _menuButtonReference;

        [SerializeField]
        private GripChecker _gripChecker;

        [SerializeField]
        private GameObject _menuCanvas;

        [SerializeField]
        private GameObject _optionsMenu;

        [SerializeField]
        private GameObject _discardPopup;

        [SerializeField]
        private BackButton _back;

        [SerializeField]
        private Vector3 _canvasOffset = new(0f, -0.1f, 0.4f);

        private Transform _localCamera;

        private void Awake() => _menuButtonReference.action.Enable();

        private void Start() => _localCamera = BIMOSUtils.LocalRig.ControllerRig.Transforms.Camera;

        private void OnEnable() => _menuButtonReference.action.performed += ToggleMenuButton;

        private void OnDisable() => _menuButtonReference.action.performed -= ToggleMenuButton;

        public void ToggleMenuButton(InputAction.CallbackContext _)
        {
            if (!_gripChecker.IsGripping) ToggleMenu();
        }

        public void ToggleMenu()
        {
            var canvasRotation = Quaternion.LookRotation(Vector3.Cross(_localCamera.right, Vector3.up));

            _menuCanvas.transform.SetPositionAndRotation(
                _localCamera.position + canvasRotation * _canvasOffset,
                canvasRotation
            );

            if (_optionsMenu.activeSelf)
            {
                if (_discardPopup.activeSelf)
                    _discardPopup.SetActive(false);
                else
                    _back.Pressed();
                return;
            }

            var showMenu = !_menuCanvas.activeSelf;
            _menuCanvas.SetActive(showMenu);
        }
    }
}
