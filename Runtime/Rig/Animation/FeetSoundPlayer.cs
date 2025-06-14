using KadenZombie8.BIMOS.Audio;
using UnityEngine;
using UnityEngine.Audio;

namespace KadenZombie8.BIMOS.Rig
{
    public class FeetSoundPlayer : SoundPlayer
    {
        [SerializeField]
        private AudioResource
            _walkSound,
            _runSound;

        private Feet _feet;
        private SmoothLocomotion _smoothLocomotion;

        protected override void Setup()
        {
            _feet = BIMOSRig.Instance.AnimationRig.Feet;
            _smoothLocomotion = BIMOSRig.Instance.PhysicsRig.SmoothLocomotion;
        }

        private void OnEnable() => _feet.OnStep += Stepped;

        private void OnDisable() => _feet.OnStep -= Stepped;

        private void Stepped() => Play(_smoothLocomotion.IsRunning ? _runSound : _walkSound);
    }
}
