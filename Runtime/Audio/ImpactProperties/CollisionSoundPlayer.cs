using System.Collections;
using KadenZombie8.BIMOS.ImpactProperties;
using UnityEngine;
using UnityEngine.Audio;

namespace KadenZombie8.BIMOS.Audio
{
    [RequireComponent(typeof(MaterialGiver))]
    public class CollisionSoundPlayer : MonoBehaviour
    {
        [SerializeField]
        private ImpulseRange _impulseRange = new()
        {
            Minimum = 0.2f,
            Maximum = 20f
        };

        private MaterialGiver _materialGiver;

        private void Awake() => _materialGiver = GetComponent<MaterialGiver>();

        private void OnCollisionEnter(Collision collision)
        {
            var material = _materialGiver.Material;
            if (!material) return;
            var collisionSound = material.CollisionSound;

            var impulseMagnitude = collision.impulse.magnitude;
            var volume = Mathf.InverseLerp(_impulseRange.Minimum, _impulseRange.Maximum, impulseMagnitude);
            PlayClipAtPoint(collisionSound, collision.GetContact(0).point, volume);
        }

        // Nasty, quick implementation
        private void PlayClipAtPoint(AudioResource resource, Vector3 position, float volume)
        {
            GameObject gameObject = new("One shot audio");
            gameObject.transform.position = position;
            AudioSource audioSource = (AudioSource)gameObject.AddComponent(typeof(AudioSource));
            audioSource.resource = resource;
            audioSource.spatialBlend = 1f;
            audioSource.volume = volume;
            audioSource.Play();
            StartCoroutine(DestroyOnEnd(gameObject, audioSource));
        }

        // Nastier
        private IEnumerator DestroyOnEnd(GameObject gameObject, AudioSource audioSource)
        {
            yield return new WaitWhile(() => audioSource.isPlaying);
            Destroy(gameObject);
        }
    }
}
