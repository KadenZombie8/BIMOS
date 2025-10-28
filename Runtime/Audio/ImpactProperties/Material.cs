using UnityEngine;
using UnityEngine.Audio;

namespace KadenZombie8.BIMOS.ImpactProperties
{
    [CreateAssetMenu(fileName = "Material", menuName = "BIMOS/Material")]
    public class Material : ScriptableObject
    {
        public AudioResource CollisionSound;
    }
}
