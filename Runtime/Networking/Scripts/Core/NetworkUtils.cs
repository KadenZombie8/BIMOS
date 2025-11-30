using UnityEngine;

namespace KadenZombie8.BIMOS.Networking {
    public static class NetworkUtils {
        public static void SetActive(this Behaviour obj, bool value) => obj.gameObject.SetActive(value);
    }
}
