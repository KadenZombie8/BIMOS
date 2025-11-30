using UnityEngine;

namespace KadenZombie8.BIMOS.SDK
{
    public abstract class Scannable : ScriptableObject
    {
        public string Barcode {
            get => _barcode;
            set => _barcode = value;
        }
        public string Name {
            get => _name;
            set => _name = value;
        }


        public string _barcode;
        public string _name;
    }
}
