using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Enemies
{
    public class CatDemon: Enemy
    {
        [SerializeField] private float phaseTime = 10f;
        [SerializeField] private float getCloserTime = 1f;
        [SerializeField] private float secondAttackChance = 0.5f;

        [SerializeField] private Shield shield;

        [SerializeField] private float explosionCooldown = 10f;
        
        private enum BehaviourPhase
        {
            Melee, LongRange
        }

        private BehaviourPhase _behaviorPhase;
        private bool _behaviorIsBlocked;

        private ActionCooldown _explosionCooldown;

        private static readonly int IsWatchingRightBool = Animator.StringToHash("isWatchingRight");
        private static readonly int IsRunningBool = Animator.StringToHash("isRunning");
        private static readonly int GoToSecondAttackBool = Animator.StringToHash("goToSecondAttack");
        private static readonly int OnExplosionTrigger = Animator.StringToHash("onExplosion");
        private static readonly int OnShockWaveTrigger = Animator.StringToHash("onShockWave");

        protected override void Start()
        {
            base.Start();
            BehaviorEnabled = true;
            shield.Init(this);
            shield.Reset();
            Movement = Vector3.zero;
            _explosionCooldown = new ActionCooldown(this, explosionCooldown);
            StartCoroutine(PhaseRotation());
        }

        protected override void BehaviorUpdate()
        {
            if (_behaviorIsBlocked || IsAttacking) return;
            Movement = _behaviorPhase switch
            {
                BehaviourPhase.Melee => GetDirectionToPlayer() * Speed,
                BehaviourPhase.LongRange => Vector3.zero,
                _ => throw new ArgumentOutOfRangeException()
            };
            _behaviorIsBlocked = true;
            StartCoroutine(MeleeAttack());
            //StartCoroutine(ExplosionAttack());
        }
        
        private IEnumerator MeleeAttack()
        {
            yield return new WaitForSeconds(getCloserTime);
            if (IsAttacking) yield break;
            IsAttacking = true;
            Animator.SetTrigger(AttackTrigger);
            if (Random.Range(0f, 1f) < secondAttackChance) Animator.SetBool(GoToSecondAttackBool, true);
            _behaviorIsBlocked = false;
        }

        private void ExplosionAttack()
        {
            shield.SetBlock(true);
            IsAttacking = true;
            _behaviorIsBlocked = true;
            Animator.SetTrigger(OnExplosionTrigger);
            _explosionCooldown.Cooldown();
        }

        private void ShockWave()
        {
            IsAttacking = true;
            _behaviorIsBlocked = true;
            Animator.SetTrigger(OnShockWaveTrigger);
        }

        public bool CallExplosion()
        {
            if (_behaviorPhase != BehaviourPhase.Melee) return true;
            if (IsAttacking) return false;
            if (!_explosionCooldown.CanPerform) ShockWave();
            else ExplosionAttack();
            return true;
        }

        private IEnumerator PhaseRotation()
        {
            while (IsAlive)
            {
                yield return new WaitForSeconds(phaseTime);
                _behaviorPhase = _behaviorPhase switch
                {
                    BehaviourPhase.Melee => BehaviourPhase.LongRange,
                    BehaviourPhase.LongRange => BehaviourPhase.Melee,
                    _ => throw new ArgumentOutOfRangeException()
                };
            }
        }

        public override void AfterAttack(ParticleSystem afterAttackEffect, bool effectIsAttached = true)
        {
            base.AfterAttack(afterAttackEffect, effectIsAttached);
            shield.SetBlock(false);
            _behaviorIsBlocked = false;
            if (Animator.GetBool(GoToSecondAttackBool))
            {
                Animator.SetBool(GoToSecondAttackBool, false);
            }
        }

        public override void TakeDamage(LiveEntity producer = null, int damage = 1)
        {
            if (shield.IsBlocking)
            {
                shield.Hit(damage);
                return;
            }
            base.TakeDamage(producer, damage);
        }

        private void FixedUpdate()
        {
            Animator.SetBool(IsWatchingRightBool, GetDirectionToPlayer().x > 0);
            Animator.SetBool(IsRunningBool, Movement.magnitude > 0 && !IsAttacking);
            if (!BehaviorEnabled) return;
            
            var movement = !IsAttacking ? Movement : Vector3.zero; // stop movement when attacking
            Rigidbody.velocity = movement;
        }
    }
}