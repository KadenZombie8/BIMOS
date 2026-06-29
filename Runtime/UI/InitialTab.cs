using UnityEngine;
using UnityEngine.UI;

namespace KadenZombie8.BIMOS.UI
{
    /// <summary>
    /// Selects a specified tab's toggle on awake.
    /// </summary>
    public class InitialTab : MonoBehaviour
    {
        [SerializeField]
        private Toggle _initialToggle;

        private void Start() => _initialToggle.Select();
    }
}
