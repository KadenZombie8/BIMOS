using UnityEngine;

namespace KadenZombie8.BIMOS.SDK
{
    [CreateAssetMenu(fileName = "Pallet", menuName = "BIMOS/SDK/Pallet")]
    public class Pallet : Scannable {
        public string Author {
            get => _author;
            set => _author = value;
        }
        public string _author;
    }
}
