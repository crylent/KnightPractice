using System.Collections;
using Unity.VisualScripting;
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
            
            if (seconds == null) yield break;
            yield return new WaitForSeconds((float) seconds);
            if (system.IsDestroyed()) yield break;
            system.Stop();
        }
    }
}
