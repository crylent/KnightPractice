using System.Collections;
using UnityEngine;

namespace Utility
{
    public class Effects: MonoBehaviour
    {
        private static Effects _instance;

        private void Start()
        {
            _instance = this;
        }

        public static void StopEffect(ParticleSystem system)
        {
            system.Stop();
            _instance.StartCoroutine(DestroyEffect(system));
        }
        
        public static void PlayEffectForSeconds(ParticleSystem effect, float seconds, Vector3 position)
        {
            var transform = new GameObject().transform;
            transform.position = position;
            _instance.StartCoroutine(PlayAndDestroy(effect, transform, false, seconds));
        }

        public static void PlayEffectForSeconds(ParticleSystem effect, float seconds, Transform parent, bool isAttached = false)
        {
            _instance.StartCoroutine(PlayAndDestroy(effect, parent, isAttached, seconds));
        }

        public static void PlayEffectOnce(ParticleSystem effect, Transform parent, bool isAttached = false)
        {
            _instance.StartCoroutine(PlayAndDestroy(effect, parent, isAttached));
        }

        private static IEnumerator PlayAndDestroy(ParticleSystem effect, Transform parent, bool isAttached, float? seconds = null)
        {
            var transform = effect.transform;
            var system = isAttached ?
                Instantiate(effect, parent) : 
                Instantiate(effect, parent.position + transform.position, transform.rotation);
            if (seconds != null)
            {
                yield return new WaitForSeconds((float) seconds);
                system.Stop();
            }

            _instance.StartCoroutine(DestroyEffect(system));
        }

        private static IEnumerator DestroyEffect(ParticleSystem system)
        {
            yield return new WaitWhile(system.IsAlive);
            Destroy(system.gameObject);
        }
    }
}
