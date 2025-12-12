using KadenZombie8.BIMOS.Audio;
using KadenZombie8.BIMOS.Sockets;
using UnityEngine;
using UnityEngine.Audio;

namespace KadenZombie8.BIMOS.Guns
{
    [RequireComponent(typeof(Socket))]
    public class SocketSoundPlayer : SoundPlayer
    {
        [SerializeField]
        private AudioResource
            _attachSound,
            _detachSound;

        private Socket _socket;

        protected override void Awake()
        {
            base.Awake();
            _socket = GetComponent<Socket>();
        }

        private void OnEnable()
        {
            _socket.OnAttachStart += Attached;
            _socket.OnDetachStart += Detached;
        }

        private void OnDisable()
        {
            _socket.OnAttachStart -= Attached;
            _socket.OnDetachStart -= Detached;
        }

        private void Attached(Plug _) => Play(_attachSound);

        private void Detached(Plug _) => Play(_detachSound);
    }
}
