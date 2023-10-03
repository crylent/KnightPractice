using System;
using System.Collections;
using UnityEngine;

namespace Enemies
{
    public class Slime : Enemy
    {
        [SerializeField] private float speed = 3f;
        [SerializeField] private float attackRange = 13f;
        [SerializeField] private float spurtRange = 25f;
        [SerializeField] private float spurtCooldown = 5f;
        [SerializeField] private float freezeTimeAfterSpurt = 1f;

        private readonly ActionCooldown _allBehaviorCooldown;
        private readonly ActionCooldown _spurtCooldown;
        
        private static readonly int XMovement = Animator.StringToHash("x-movement");
        private static readonly int SpurtTrigger = Animator.StringToHash("onSpurt");

        public Slime()
        {
            _spurtCooldown = new ActionCooldown(this, spurtCooldown);
            _allBehaviorCooldown = new ActionCooldown(this, freezeTimeAfterSpurt);
        }

        protected override void BehaviorUpdate()
        {
            if (!_allBehaviorCooldown.CanPerform) return;
            Movement = GetDirectionToPlayer() * speed; // chase player
            Animator.SetInteger(XMovement, Math.Sign(Movement.x));
            AttackBehavior();
        }

        protected virtual void AttackBehavior()
        {
            var distance = GetDistanceToPlayer();
            if (distance < spurtRange && _spurtCooldown.CanPerform)
            {
                Attack(SpurtTrigger);
                _spurtCooldown.Cooldown();
            }
            else if (distance < attackRange)
            {
                Attack(AttackTrigger);
            }
        }

        public override void StartAttack(AttackCollider attackCollider)
        {
            if (!IsAlive) return;
            if (attackCollider.AttackName == "Spurt")
            {
                StartCoroutine(Spurt());
            }
        }

        private IEnumerator Spurt()
        {
            var direction = GetDirectionToPlayer();
            var spurtTime = Animator.GetCurrentAnimatorClipInfo(0)[0].clip.length;
            var spurtSpeed = (Math.Min(GetDistanceToPlayer(), spurtRange) - 5f) / spurtTime;
            Rigidbody.velocity = Vector3.zero;
            Animator.SetInteger(XMovement, 0);
            _allBehaviorCooldown.Cooldown();
            while (IsAttacking)
            {
                transform.position +=  spurtSpeed * Time.fixedDeltaTime * direction;
                yield return new WaitForFixedUpdate();
            }
        }

        private void FixedUpdate()
        {
            if (BehaviorEnabled && !IsAttacking && _allBehaviorCooldown.CanPerform &&
                Animator.GetCurrentAnimatorStateInfo(0).normalizedTime % 1f > 0.5f)
            {
                Rigidbody.velocity = Movement;
            }
        }
    }
}
