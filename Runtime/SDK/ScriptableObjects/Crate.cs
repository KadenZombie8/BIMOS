using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace KadenZombie8.BIMOS.SDK
{
    public abstract class Crate : Scannable
    {
        public string[] Tags {
            get => _tags; set => _tags = value;
        }
        public string[] _tags;
        public Pallet _pallet;

        public Pallet Pallet {
            get => _pallet; set => _pallet = value;
        }

        public abstract Asset BaseAsset {
            get;set;
        }
    }

    public abstract class Asset {
        public abstract Object BaseAsset {
            get; set;
        }
        public Guid Guid {
            get => _guid; set => _guid = value;
        }
        public Guid _guid;     
    }

    public class Asset<T> : Asset where T : Object {
        public override Object BaseAsset {
            get => MainAsset; set => MainAsset = (T)value;
        }
        public T MainAsset {
            get => _mainAsset; set => _mainAsset = value;
        }
        public T _mainAsset;
    }

    public abstract class Crate<T> : Crate where T : Object {
        public Asset<T> _mainAsset;

        public Asset<T> MainAsset {
            get => _mainAsset; set => _mainAsset = value;
        }
        public override Asset BaseAsset {
            get => MainAsset;
            set => MainAsset = value as Asset<T>;
        }
    }
}
