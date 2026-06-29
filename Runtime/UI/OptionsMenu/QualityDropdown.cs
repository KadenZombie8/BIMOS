using System.Linq;
using TMPro;
using UnityEngine;

namespace KadenZombie8.BIMOS
{
    [DefaultExecutionOrder(-1)]
    [RequireComponent(typeof(TMP_Dropdown))]
    public class QualityDropdown : MonoBehaviour
    {
        private void Awake()
        {
            var dropdown = GetComponent<TMP_Dropdown>();
            dropdown.ClearOptions();
            dropdown.AddOptions(QualitySettings.names.ToList());
        }
    }
}
