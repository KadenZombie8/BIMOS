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
            _socket.Events.Attach.OnStart?.AddListener(Attached);
            _socket.Events.Detach.OnStart?.AddListener(Detached);
        }

        private void OnDisable()
        {
            _socket.Events.Attach.OnStart?.RemoveListener(Attached);
            _socket.Events.Detach.OnStart?.RemoveListener(Detached);
        }

        private void Attached(Plug _) => Play(_attachSound);

        private void Detached(Plug _) => Play(_detachSound);
    }
}
