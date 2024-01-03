using System;
using System.Collections;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;

namespace Enemies
{
    public class Slime : Enemy
    {
        [SerializeField] private float attackRange = 13f;
        [SerializeField] private float spurtRange = 25f;
        [SerializeField] private float spurtCooldown = 5f;
        [SerializeField] private float freezeTimeAfterSpurt = 1f;

        private ActionCooldown _spurtCooldown;
        [CanBeNull] private GameObject _spurtArrow;
        
        private static readonly int XMovement = Animator.StringToHash("x-movement");
        private static readonly int SpurtTrigger = Animator.StringToHash("onSpurt");

        protected override void Start()
        {
            base.Start();
            _spurtCooldown = new ActionCooldown(this, spurtCooldown);
            AllBehaviorCooldown = new ActionCooldown(this, freezeTimeAfterSpurt);
        }

        private void LateUpdate()
        {
            if (!_spurtArrow.IsUnityNull()) _spurtArrow!.transform.rotation = GetRotationToPlayer();
        }

        protected override void BehaviorUpdate()
        {
            if (!AllBehaviorCooldown.CanPerform) return;
            Movement = GetDirectionToPlayer() * Speed; // chase player
            var moveDirection = Math.Sign(Movement.x);
            IsWatchingRight = moveDirection switch
            {
                -1 => false,
                1 => true,
                _ => IsWatchingRight
            };
            Animator.SetInteger(XMovement, moveDirection);
            AttackBehavior();
        }

        protected virtual void AttackBehavior()
        {
            if (IsAttacking) return;
            var distance = GetDistanceToPlayer();
            if (distance < spurtRange && _spurtCooldown.CanPerform)
            {
                _spurtArrow = Instantiate(
                    threatArrow, 
                    transform.position + threatArrow.transform.position,
                    GetRotationToPlayer());
                Attack(SpurtTrigger);
                _spurtCooldown.Cooldown();
            }
            else if (distance < attackRange)
            {
                Attack(AttackTrigger);
            }
        }

        public override void StartAttack(string attackName, 
            AttackCollider attackCollider, ParticleSystem attackEffect)
        {
            if (!IsAlive) return;
            if (attackName != "Spurt") return;
            _spurtArrow = null;
            StartCoroutine(Spurt());
        }

        private IEnumerator Spurt()
        {
            var direction = GetDirectionToPlayer();
            var spurtTime = Animator.GetCurrentAnimatorClipInfo(0)[0].clip.length;
            var spurtSpeed = (Math.Min(GetDistanceToPlayer(), spurtRange) - 5f) / spurtTime;
            Rigidbody.velocity = Vector3.zero;
            Animator.SetInteger(XMovement, 0);
            AllBehaviorCooldown.Cooldown();
            while (IsAttacking)
            {
                transform.position +=  spurtSpeed * Time.fixedDeltaTime * direction;
                yield return new WaitForFixedUpdate();
            }
        }

        private void FixedUpdate()
        {
            if (BehaviorEnabled && !IsAttacking && AllBehaviorCooldown.CanPerform &&
                Animator.GetCurrentAnimatorStateInfo(0).normalizedTime % 1f > 0.5f)
            {
                Rigidbody.velocity = Movement;
            }
        }
    }
}
