using KadenZombie8.BIMOS.Rig.Movement;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

#if UNITY_WEBGL
using WebXR;
#endif

namespace KadenZombie8.BIMOS.Rig
{
    public class SettingsMenu : MonoBehaviour
    {
//        private BIMOSRig _player;

//#if UNITY_WEBGL
//        [SerializeField]
//        private WebXRController _controller;
//#endif

//        [SerializeField]
//        private int _cameraMode;

//        [SerializeField]
//        private InputActionReference _menuButtonAction;

//        [SerializeField]
//        private InputAction _leftGripAction;

//        [SerializeField]
//        private Transform _cameraTransform, _cameraOffsetTransform;

//        [SerializeField]
//        private GameObject _spectatorCamera, _thirdPersonCamera;

//        [SerializeField]
//        private Collider _thirdPersonCameraCollider;

//        [SerializeField]
//        private GameObject
//            _visualLocomotionSphere,
//            _visualFender,
//            _visualLeg,
//            _visualPelvis,
//            _visualHead,
//            _visualLeftHand,
//            _visualRightHand;

//        [SerializeField]
//        private GameObject _settingsCanvas, _visualFloorOffset;

//        [SerializeField]
//        private Slider _smoothTurnSpeedSlider, _snapTurnIncrementSlider;

//        [SerializeField]
//        private Text
//            _heightDisplay,
//            _floorOffsetDisplay,
//            _smoothTurnSpeedDisplay,
//            _snapTurnIncrementDisplay,
//            _cameraModeDisplay,
//            _togglePhysicsDisplay,
//            _toggleAnimationDisplay;

//#if UNITY_WEBGL
//        private void Update()
//        {
//            if (_controller.GetButtonDown(WebXRController.ButtonTypes.ButtonB))
//                if (_controller.GetAxis(WebXRController.AxisTypes.Grip) < 0.5f)
//                    ToggleMenu(new InputAction.CallbackContext());
//        }
//#endif

//        private void SetLayerRecursive(GameObject gameObject, int layer)
//        {
//            gameObject.layer = layer;
//            foreach (Transform child in gameObject.transform)
//            {
//                child.gameObject.layer = layer;

//                if (!child.GetComponentInChildren<Transform>())
//                    continue;

//                SetLayerRecursive(child.gameObject, layer);
//            }
//        }

//        private void OnEnable()
//        {
//#if !UNITY_WEBGL
//            _menuButtonAction.action.Enable(); //Enables the menu button action
//            _menuButtonAction.action.performed += ToggleMenu;
//            _leftGripAction.Enable();
//#endif
//        }

//        private void OnDisable()
//        {
//            _menuButtonAction.action.performed -= ToggleMenu;
//        }

//        public void Start()
//        {
//            HideMenu();

//            _player = BIMOSRig.Instance;
//            SetLayerRecursive(gameObject, LayerMask.NameToLayer("BIMOSMenu"));
//            _visualFloorOffset.layer = LayerMask.NameToLayer("BIMOSMenu");

//            transform.SetParent(_cameraOffsetTransform); //Sets the canvas to follow the camera

//            //Initialise
//            _heightDisplay.text = "Height: " + (_player.ControllerRig.HeadsetStandingHeight * 100f).ToString("0") + "cm"; //Display the correct height with no decimals
//            _floorOffsetDisplay.text = "Floor offset: " + (_player.ControllerRig.FloorOffset * 100f).ToString("0") + "cm";
//#if !UNITY_ANDROID
//            _cameraMode = PlayerPrefs.GetInt("CameraMode", 0);
//            UpdateCameraMode();
//#endif

//            _smoothTurnSpeedSlider.value = PlayerPrefs.GetFloat("SmoothTurnSpeed", 10f); //Sets the smooth turn speed in the player
//            _smoothTurnSpeedDisplay.text = PlayerPrefs.GetFloat("SmoothTurnSpeed", 10f).ToString(); //Updates the display

//            int increment = 0;
//            switch (PlayerPrefs.GetFloat("SnapTurnIncrement", 45)) //Compare the value against the different values it can take
//            {
//                case 30:
//                    increment = 0;
//                    break;
//                case 45:
//                    increment = 1;
//                    break;
//                case 90:
//                    increment = 2;
//                    break;
//            }
//            _snapTurnIncrementSlider.value = increment;
//            _snapTurnIncrementDisplay.text = PlayerPrefs.GetFloat("SnapTurnIncrement", 45).ToString();

//            if (PlayerPrefs.GetInt("LocomotionType", 0) == 0)
//                SmoothLocomotion();
//            else
//                TeleportLocomotion();

//            switch (PlayerPrefs.GetInt("TurnType", 2))
//            {
//                case 0:
//                    NoTurn();
//                    break;
//                case 1:
//                    SmoothTurn();
//                    break;
//                case 2:
//                    SnapTurn();
//                    break;
//            }
//        }

//        //Canvas showing/hiding
//        private void ToggleMenu(InputAction.CallbackContext context)
//        {
//#if !UNITY_WEBGL
//            if (_leftGripAction.ReadValue<float>() > 0.5f)
//                return;
//#endif

//            if (_settingsCanvas.activeSelf)
//                HideMenu();
//            else
//                ShowMenu();
//        }

//        private void ShowMenu()
//        {
//            //Set the canvas to an appropriate rotation and position it 1m in front of the player
//            Quaternion canvasRotation = Quaternion.LookRotation(Vector3.Cross(_cameraTransform.right, Vector3.up));
//            _settingsCanvas.transform.rotation = canvasRotation;
//            _settingsCanvas.transform.position = _cameraTransform.position + canvasRotation * Vector3.forward * 1;

//            _settingsCanvas.SetActive(true); //Show the canvas
//            _visualFloorOffset.SetActive(true);
//        }

//        private void HideMenu()
//        {
//            _settingsCanvas.SetActive(false); //Hide the canvas
//            _visualFloorOffset.SetActive(false);
//        }

//        //Buttons
//        public void Resume()
//        {
//            HideMenu();
//        }
//        public void RecalculateHeight()
//        {
//            _player.ControllerRig.HeadsetStandingHeight = _player.ControllerRig.CameraTransform.localPosition.y;
//            UpdateHeight();
//        }
//        public void ResetHeight()
//        {
//            _player.ControllerRig.HeadsetStandingHeight = 1.65f;
//            UpdateHeight();
//        }
//        public void ChangeHeight(float value)
//        {
//            _player.ControllerRig.HeadsetStandingHeight += value;
//            UpdateHeight();
//        }
//        private void UpdateHeight()
//        {
//            _player.ControllerRig.HeadsetStandingHeight = Mathf.Clamp(_player.ControllerRig.HeadsetStandingHeight, 1f, 3f);
//            PlayerPrefs.SetFloat("HeadsetStandingHeight", _player.ControllerRig.HeadsetStandingHeight);
//            _player.ControllerRig.ScaleCharacter();
//            _heightDisplay.text = "Height: " + (_player.ControllerRig.HeadsetStandingHeight * 100f).ToString("0") + "cm";
//        }
//        public void RecalculateFloorHeight()
//        {
//            _player.ControllerRig.FloorOffset = _player.ControllerRig.HeadsetStandingHeight - _player.ControllerRig.CameraTransform.localPosition.y;
//            UpdateFloorHeight();
//        }
//        public void ResetFloorHeight()
//        {
//            _player.ControllerRig.FloorOffset = 0f;
//            UpdateFloorHeight();
//        }
//        public void ChangeFloorHeight(float value)
//        {
//            _player.ControllerRig.FloorOffset += value;
//            UpdateFloorHeight();
//        }
//        private void UpdateFloorHeight()
//        {
//            _player.ControllerRig.FloorOffset = Mathf.Clamp(_player.ControllerRig.FloorOffset, -1.35f, 0.65f);
//            PlayerPrefs.SetFloat("FloorOffset", _player.ControllerRig.FloorOffset);
//            _floorOffsetDisplay.text = "Floor offset: " + (_player.ControllerRig.FloorOffset * 100f).ToString("0") + "cm";
//        }
//        public void ChangeCameraMode()
//        {
//            _cameraMode++;
//            if (_cameraMode > 3)
//                _cameraMode = 0;
//            UpdateCameraMode();
//        }
//        private void UpdateCameraMode()
//        {
//#if UNITY_ANDROID || UNITY_WEBGL
//        _cameraModeDisplay.text = "Not available on platform";
//        return;
//#else

//            switch (_cameraMode)
//            {
//                case 0:
//                    _cameraModeDisplay.text = "Headset";
//                    _thirdPersonCamera.SetActive(false);
//                    _thirdPersonCameraCollider.enabled = true;
//                    _thirdPersonCamera.transform.parent = _player.PhysicsRig.PelvisRigidbody.transform;
//                    PlayerPrefs.SetInt("CameraMode", 0);
//                    break;
//                case 1:
//                    _cameraModeDisplay.text = "Spectator";
//                    _spectatorCamera.SetActive(true);
//                    PlayerPrefs.SetInt("CameraMode", 1);
//                    break;
//                case 2:
//                    _cameraModeDisplay.text = "Third-person";
//                    _spectatorCamera.SetActive(false);
//                    _thirdPersonCamera.transform.position = _cameraTransform.position + _cameraTransform.forward;
//                    _thirdPersonCamera.transform.LookAt(_cameraTransform);
//                    _thirdPersonCamera.GetComponent<Rigidbody>().isKinematic = false;
//                    _thirdPersonCamera.SetActive(true);
//                    break;
//                case 3:
//                    _cameraModeDisplay.text = "Third-person follow";

//                    _thirdPersonCamera.GetComponent<Rigidbody>().isKinematic = true;
//                    _thirdPersonCameraCollider.enabled = false;
//                    _thirdPersonCamera.transform.parent = _cameraOffsetTransform;
//                    break;
//            }
//#endif
//        }
//        public void TogglePhysics()
//        {
//            bool visible = !_visualLocomotionSphere.activeSelf;

//            _visualLocomotionSphere.SetActive(visible);
//            _visualFender.SetActive(visible);
//            _visualLeg.SetActive(visible);
//            _visualPelvis.SetActive(visible);
//            _visualHead.SetActive(visible);
//            _visualLeftHand.SetActive(visible);
//            _visualRightHand.SetActive(visible);

//            _togglePhysicsDisplay.text = visible ? "Hide physics rig" : "Show physics rig";
//        }
//        public void ToggleAnimation()
//        {
//            bool visible = !_player._animationRig.Transforms.Character.GetComponentInChildren<SkinnedMeshRenderer>().enabled;

//            foreach (var renderer in _player._animationRig.Transforms.Character.GetComponentsInChildren<SkinnedMeshRenderer>())
//                renderer.enabled = visible;

//            _toggleAnimationDisplay.text = visible ? "Hide animation rig" : "Show animation rig";
//        }
//        public void SmoothLocomotion()
//        {
//            _player.PhysicsRig.GetComponent<TeleportLocomotion>().enabled = false; //Disable teleport
//            _player.PhysicsRig.GetComponent<SmoothLocomotion>().enabled = true; //Enable smooth
//            PlayerPrefs.SetInt("LocomotionType", 0);
//        }
//        public void TeleportLocomotion()
//        {
//            _player.PhysicsRig.GetComponent<SmoothLocomotion>().enabled = false; //Disable smooth
//            _player.PhysicsRig.GetComponent<TeleportLocomotion>().enabled = true; //Enable teleport
//            PlayerPrefs.SetInt("LocomotionType", 1);
//        }
//        public void NoTurn()
//        {
//            _player.PhysicsRig.GetComponent<SmoothTurn>().enabled = false; //Disable smooth
//            _player.PhysicsRig.GetComponent<SnapTurn>().enabled = false; //Disable snap
//            PlayerPrefs.SetInt("TurnType", 0);
//        }
//        public void SmoothTurn()
//        {
//            _player.PhysicsRig.GetComponent<SnapTurn>().enabled = false; //Disable snap
//            _player.PhysicsRig.GetComponent<SmoothTurn>().enabled = true; //Enable smooth
//            PlayerPrefs.SetInt("TurnType", 1);
//        }
//        public void SnapTurn()
//        {
//            _player.PhysicsRig.GetComponent<SmoothTurn>().enabled = false; //Disable smooth
//            _player.PhysicsRig.GetComponent<SnapTurn>().enabled = true; //Enable snap
//            PlayerPrefs.SetInt("TurnType", 2);
//        }
//        public void UpdateSmoothTurnSpeed()
//        {
//            _player.ControllerRig.SmoothTurnSpeed = _smoothTurnSpeedSlider.value; //Sets the smooth turn speed in the player
//            _smoothTurnSpeedDisplay.text = _smoothTurnSpeedSlider.value.ToString(); //Updates the display
//            PlayerPrefs.SetFloat("SmoothTurnSpeed", _smoothTurnSpeedSlider.value);
//        }
//        public void UpdateSnapTurnIncrement()
//        {
//            int value = (int)_snapTurnIncrementSlider.value; //Value is given as a float, even when constrained to int values
//            int increment = 0; //The increment we shall use and change when processing the value

//            switch (value) //Compare the value against the different values it can take
//            {
//                case 0:
//                    increment = 30;
//                    break;
//                case 1:
//                    increment = 45;
//                    break;
//                case 2:
//                    increment = 90;
//                    break;
//            }

//            _player.ControllerRig.SnapTurnIncrement = increment; //Sets the snap turn increment in the player
//            PlayerPrefs.SetFloat("SnapTurnIncrement", increment);

//            _snapTurnIncrementDisplay.text = increment.ToString(); //Updates the display
//        }
//        public void ResetScene()
//        {
//            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); //Loads the current scene again
//        }
//        public void MainMenu()
//        {
//            for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
//                if (NameFromIndex(i) == "MainMenu")
//                {
//                    SceneManager.LoadScene(i);
//                    return;
//                }

//            Debug.LogError("Must have a scene called \"MainMenu\" in your build profile!");
//        }
//        private string NameFromIndex(int BuildIndex)
//        {
//            string path = SceneUtility.GetScenePathByBuildIndex(BuildIndex);
//            int slash = path.LastIndexOf('/');
//            string name = path.Substring(slash + 1);
//            int dot = name.LastIndexOf('.');
//            return name.Substring(0, dot);
//        }
    }
}
