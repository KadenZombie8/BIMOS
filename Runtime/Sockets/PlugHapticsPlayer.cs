using KadenZombie8.BIMOS.Sockets;
using UnityEngine;

namespace KadenZombie8.BIMOS
{
    [RequireComponent(typeof(Plug))]
    public class PlugHapticsPlayer : HapticsPlayer
    {
        private Plug _plug;

        private void Awake() => _plug = GetComponent<Plug>();

        private void OnEnable()
        {
            _plug.OnAttach.AddListener(PlaySocketHaptics);
            _plug.OnDetach.AddListener(PlaySocketHaptics);
        }

        private void OnDisable()
        {
            _plug.OnAttach.RemoveListener(PlaySocketHaptics);
            _plug.OnDetach.RemoveListener(PlaySocketHaptics);
        }

        private void PlaySocketHaptics()
        {
            HapticSettings.Duration = _plug.Socket.InsertTime;
            Play();
        }
    }
}
