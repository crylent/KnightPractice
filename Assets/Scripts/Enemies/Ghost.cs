using System.Collections;
using Player;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Enemies
{
    public class Ghost : Enemy
    {
        public float damageCooldown = 1f;
        public float changeDirectionCooldown = 1f;
        public float directionMaxRandom = 30f;
        public float speedupFactor = 2f;
        public float speedupCooldown = 5f;
        public float speedupTime = 3f;
        
        private ActionCooldown _damageCooldown;
        private ActionCooldown _changeDirectionCooldown;
        private ActionCooldown _speedupCooldown;
        
        private bool _speedup;
        private static readonly int SpeedupBool = Animator.StringToHash("speedup");

        protected override void Start()
        {
            base.Start();
            _damageCooldown = new ActionCooldown(this, damageCooldown);
            _changeDirectionCooldown = new ActionCooldown(this, changeDirectionCooldown);
            _speedupCooldown = new ActionCooldown(this, speedupCooldown, true);
        }

        protected override void BehaviorUpdate()
        {
            if (_changeDirectionCooldown.CanPerform || _speedup) // hunt the player when has speedup
            {
                float randomRotation = 0;
                if (!_speedup)
                {
                    randomRotation = Random.Range(-directionMaxRandom, directionMaxRandom);
                }
                var randomRotator = Quaternion.AngleAxis(randomRotation, Vector3.up);
                Movement = randomRotator * GetDirectionToPlayer() * Speed;
                _changeDirectionCooldown.Cooldown();
            }

            if (_speedupCooldown.CanPerform)
            {
                StartCoroutine(Speedup());
            }
        }

        private IEnumerator Speedup()
        {
            _speedup = true;
            Animator.SetBool(SpeedupBool, true);
            yield return new WaitForSeconds(speedupTime);
            _speedup = false;
            Animator.SetBool(SpeedupBool, false);
            _speedupCooldown.Cooldown();
        }

        private void OnTriggerStay(Collider other) // make damage every second
        {
            if (!other.CompareTag("Player") || !_damageCooldown.CanPerform) return;
            PlayerComponents.Controller.TakeDamage(this);
            _damageCooldown.Cooldown();
        }

        private void FixedUpdate()
        {
            if (BehaviorEnabled)
            {
                Rigidbody.velocity = Movement * (_speedup ? speedupFactor : 1);
            }
        }
    }
}
