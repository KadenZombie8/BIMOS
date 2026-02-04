using UnityEngine;

namespace KadenZombie8.BIMOS.Rig
{
    public class Head : MonoBehaviour
    {
        private BIMOSRig _player;

        private void Start()
        {
            _player = BIMOSRig.Instance;
        }

        private void Update()
        {
            UpdateCharacter();
        }

        public void UpdateCharacter()
        {
            _player.AnimationRig.Transforms.Character.position = _player.ControllerRig.Transforms.HeadCameraOffset.position - Vector3.up * 1.65f;
            Quaternion targetRotation = Quaternion.LookRotation(Vector3.Cross(_player.ControllerRig.Transforms.Camera.right, Vector3.up));
            _player.AnimationRig.Transforms.Character.rotation = Quaternion.Lerp(_player.AnimationRig.Transforms.Character.rotation, targetRotation, Time.deltaTime * 5f);
        }
    }
}
