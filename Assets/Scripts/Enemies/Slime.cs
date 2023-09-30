using System;
using UnityEngine;

namespace Enemies
{
    public class Slime : Enemy
    {
        [SerializeField] private float speed = 3f;
        [SerializeField] protected float attackRange = 13f;
        private static readonly int XMovement = Animator.StringToHash("x-movement");

        protected override void BehaviorUpdate()
        {
            Movement = GetDirectionToPlayer() * speed; // chase player
            Animator.SetInteger(XMovement, Math.Sign(Movement.x));
            AttackBehavior();
        }

        protected virtual void AttackBehavior()
        {
            if (GetDistanceToPlayer() < attackRange)
            {
                Attack(AttackTrigger);
            }
        }

        private void FixedUpdate()
        {
            if (Animator.GetCurrentAnimatorStateInfo(0).normalizedTime % 1f > 0.5f)
                Rigidbody.velocity = Movement;
        }
    }
}
