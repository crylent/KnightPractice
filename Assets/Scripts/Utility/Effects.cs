using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
// ReSharper disable UnusedMethodReturnValue.Global

namespace Utility
{
    public class Effects: MonoBehaviour
    {
        private static Effects _instance;

        private void Start()
        {
            _instance = this;
        }
        
        public static ParticleSystem PlayEffectForSeconds(ParticleSystem effect, float seconds, Vector3 position)
        {
            var transform = new GameObject().transform;
            transform.position = position;
            return PlayEffect(effect, transform, false, seconds);
        }

        public static ParticleSystem PlayEffectForSeconds(ParticleSystem effect, float seconds, Transform parent, bool isAttached = false)
        {
            return PlayEffect(effect, parent, isAttached, seconds);
        }

        public static ParticleSystem PlayEffectOnce(ParticleSystem effect, Transform parent, bool isAttached = false)
        {
            return PlayEffect(effect, parent, isAttached);
        }

        private static ParticleSystem PlayEffect(ParticleSystem effect, Transform parent, bool isAttached, float? seconds = null)
        {
            var transform = effect.transform;
            var system = isAttached ?
                Instantiate(effect, parent) : 
                Instantiate(effect, parent.position + transform.position, transform.rotation);
            
            _instance.StartCoroutine(PlayAndDestroy(system, seconds));
            return system;
        }

        private static IEnumerator PlayAndDestroy(ParticleSystem system, float? seconds = null)
        {
            
            if (seconds == null) yield break;
            yield return new WaitForSeconds((float) seconds);
            if (system.IsDestroyed()) yield break;
            system.Stop();
        }
    }
}
