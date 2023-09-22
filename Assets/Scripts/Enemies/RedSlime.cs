using System.Collections;
using UnityEngine;

namespace Enemies
{
    public class RedSlime : Slime
    {
        [SerializeField] private GameObject fireEffect;
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
            yield return new WaitWhile(() => Animator.GetCurrentAnimatorStateInfo(0).IsName("Slime_FadeIn"));
            while (isActiveAndEnabled)
            {
                var fire = Instantiate(fireEffect, transform.position, fireEffect.transform.rotation);
                StartCoroutine(PutOutFire(fire.GetComponentInChildren<ParticleSystem>()));
                yield return new WaitForSeconds(fireCooldown);
            }
        }

        private IEnumerator PutOutFire(ParticleSystem fireSystem)
        {
            yield return new WaitForSeconds(fireLifetime);
            fireSystem.Stop();
            yield return new WaitWhile(fireSystem.IsAlive);
            Destroy(fireSystem.transform.parent.gameObject);
        }
    }
}
