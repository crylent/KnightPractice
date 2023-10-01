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

        public static void PlayEffectForSeconds(ParticleSystem effect, float seconds, MonoBehaviour parent, bool isAttached = false)
        {
            _instance.StartCoroutine(PlayAndDestroy(effect, parent.transform, isAttached, seconds));
        }

        public static void PlayEffectOnce(ParticleSystem effect, MonoBehaviour parent, bool isAttached = false)
        {
            _instance.StartCoroutine(PlayAndDestroy(effect, parent.transform, isAttached));
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
            yield return new WaitWhile(system.IsAlive);
            Destroy(system.gameObject);
        }
    }
}
