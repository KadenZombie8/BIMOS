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
            _plug.Events.Attach.OnStart?.AddListener(PlaySocketHaptics);
            _plug.Events.Detach.OnStart?.AddListener(PlaySocketHaptics);
        }

        private void OnDisable()
        {
            _plug.Events.Attach.OnStart?.RemoveListener(PlaySocketHaptics);
            _plug.Events.Detach.OnStart?.RemoveListener(PlaySocketHaptics);
        }

        private void PlaySocketHaptics(Socket _)
        {
            HapticSettings.Duration = _plug.Socket.InsertTime;
            Play();
        }
    }
}
