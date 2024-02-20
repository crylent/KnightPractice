using System.Collections;
using Player;
using UnityEngine;

namespace VFX
{
    public class ManaParticle : MonoBehaviour
    {
        public float manaValue = 1f;
        [SerializeField] private float speed = 0.5f;
        [SerializeField] private float lifetime = 15f;
        
        private static Vector3 PlayerPosition => PlayerComponents.Transform.position;
        private ParticleSystem _particleSystem;

        private void Start()
        {
            _particleSystem = GetComponentInChildren<ParticleSystem>();
            StartCoroutine(LifeTimer());
        }

        private void Update()
        {
            var delta = PlayerPosition - transform.position;
            delta.y = 0;
            transform.Translate(speed * Time.deltaTime * delta);
        }

        private IEnumerator LifeTimer()
        {
            yield return new WaitForSeconds(lifetime);
            _particleSystem.Stop();
            yield return new WaitWhile(_particleSystem.IsAlive);
            Destroy(gameObject);
        }
    }
}
