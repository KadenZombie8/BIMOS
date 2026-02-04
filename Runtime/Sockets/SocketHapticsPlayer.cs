using KadenZombie8.BIMOS.Sockets;
using UnityEngine;

namespace KadenZombie8.BIMOS
{
    [RequireComponent(typeof(Socket))]
    public class SocketHapticsPlayer : HapticsPlayer
    {
        private Socket _socket;

        private void Awake() => _socket = GetComponent<Socket>();

        private void OnEnable()
        {
            _socket.Events.Attach.OnStart?.AddListener(PlaySocketHaptics);
            _socket.Events.Detach.OnStart?.AddListener(PlaySocketHaptics);
        }

        private void OnDisable()
        {
            _socket.Events.Attach.OnStart?.RemoveListener(PlaySocketHaptics);
            _socket.Events.Detach.OnStart?.RemoveListener(PlaySocketHaptics);
        }

        private void PlaySocketHaptics(Plug _)
        {
            HapticSettings.Duration = _socket.InsertTime;
            Play();
        }
    }
}
