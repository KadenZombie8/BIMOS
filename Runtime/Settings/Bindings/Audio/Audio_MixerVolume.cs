using UnityEngine;
using UnityEngine.Audio;

namespace KadenZombie8.BIMOS.Settings.Bindings
{
    public class Audio_MixerVolume : SettingBinding<float>
    {
        [SerializeField]
        private AudioMixer _mixer;

        [SerializeField]
        private string _parameterName;

        private void Start() => SettingUpdated(Setting.Value);

        protected override void SettingUpdated(float value)
        {
            var logarithmicVolume = Mathf.Log10(value / 10f) * 20f;
            logarithmicVolume = Mathf.Max(logarithmicVolume, -80f);
            _mixer.SetFloat(_parameterName, logarithmicVolume);
        }
    }
}
