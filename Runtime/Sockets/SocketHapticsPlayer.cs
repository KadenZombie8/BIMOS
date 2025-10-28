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
            _socket.OnAttach += PlaySocketHaptics;
            _socket.OnDetach += PlaySocketHaptics;
        }

        private void OnDisable()
        {
            _socket.OnAttach -= PlaySocketHaptics;
            _socket.OnDetach -= PlaySocketHaptics;
        }

        private void PlaySocketHaptics()
        {
            HapticSettings.Duration = _socket.InsertTime;
            Play();
        }
    }
}
