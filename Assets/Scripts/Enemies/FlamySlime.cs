using System.Collections;
using UnityEngine;

namespace Enemies
{
    public class FlamySlime : Slime
    {
        [SerializeField] private ParticleSystem fireEffect;
        [SerializeField] private float fireCooldown = 1f;
        [SerializeField] private float fireLifetime = 5f;
    
        // Start is called before the first frame update
        protected override void Start()
        {
            base.Start();
            StartCoroutine(SetGroundOnFire());
        }

        private IEnumerator SetGroundOnFire()
        {
            yield return new WaitUntil(() => BehaviorEnabled);
            while (isActiveAndEnabled)
            {
                Utility.Effects.PlayEffectForSeconds(fireEffect, fireLifetime, transform);
                yield return new WaitForSeconds(fireCooldown);
            }
        }
    }
}
