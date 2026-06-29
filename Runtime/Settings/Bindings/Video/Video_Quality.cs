using UnityEngine;

namespace KadenZombie8.BIMOS.Settings.Bindings
{
    public class Video_Quality : SettingBinding<int>
    {
        protected override void SettingUpdated(int value) => QualitySettings.SetQualityLevel(value);
    }
}
