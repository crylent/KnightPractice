using System;
using Player;
using UnityEngine;

namespace Enemies
{
    public class Slime : Enemy
    {
        [SerializeField] private float speed = 3f;
        [SerializeField] private float attackRange = 5f;
        private static readonly int XMovement = Animator.StringToHash("x-movement");

        protected override void BehaviorUpdate()
        {
            var posDiff = PlayerComponents.Transform.position - gameObject.transform.position; // positions difference
            posDiff.y = 0; // don't consider the height
            Movement = posDiff.normalized * speed; // chase player
            
            animator.SetInteger(XMovement, Math.Sign(Movement.x));

            if (posDiff.magnitude < attackRange)
            {
                Attack();
            }
        }

        private void FixedUpdate()
        {
            if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime % 1f > 0.5f)
                rigidbody.velocity = Movement;
        }
    }
}
