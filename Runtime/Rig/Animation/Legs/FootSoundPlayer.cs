using KadenZombie8.BIMOS.Audio;
using UnityEngine;
using UnityEngine.Audio;

namespace KadenZombie8.BIMOS.Rig.Animation
{
    [RequireComponent(typeof(Foot))]
    public class FootSoundPlayer : SoundPlayer
    {
        [SerializeField]
        private AudioResource
            _walkSound,
            _runSound;

        private Foot _foot;

        protected override void Awake()
        {
            base.Awake();
            _foot = GetComponent<Foot>();
        }

        private void OnEnable() => _foot.OnStep += Stepped;

        private void OnDisable() => _foot.OnStep -= Stepped;

        private void Stepped(bool isRunning) => Play(isRunning ? _runSound : _walkSound);
    }
}
