using UnityEngine;

namespace KadenZombie8.BIMOS.Rig.Animation
{
    [DefaultExecutionOrder(-1)]
    public class Head : MonoBehaviour
    {
        [SerializeField]
        private Transform _character;

        [SerializeField]
        private Transform _headCameraOffset;

        private void Update() => UpdateCharacter();

        public void UpdateCharacter()
        {
            _character.position = _headCameraOffset.position - Vector3.up * 1.65f;
            var targetRotation = Quaternion.LookRotation(Vector3.Cross(_headCameraOffset.right, Vector3.up));
            _character.rotation = Quaternion.Lerp(_character.rotation, targetRotation, Time.deltaTime * 5f);
        }
    }
}
