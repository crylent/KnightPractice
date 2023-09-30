using System.Collections;
using UnityEngine;

namespace Enemies
{
    public class StoneSlime : Slime
    {
        [SerializeField] private float jumpAttackRange = 20f;
        [SerializeField] private float jumpAttackCooldown = 5f; // cooldown after attack finished

        private bool _canPerformJumpAttack = true;
            
        private static readonly int JumpAttackTrigger = Animator.StringToHash("onJump");

        protected override void AttackBehavior()
        {
            var distance = GetDistanceToPlayer();
            if (distance < jumpAttackRange && _canPerformJumpAttack)
            {
                Collider.enabled = false; // disable collider while not on the ground
                Attack(JumpAttackTrigger);
                StartCoroutine(DisableJumpAttack());
            }
            else if (GetDistanceToPlayer() < attackRange)
            {
                Attack(AttackTrigger);
            }
        }

        public override void MakeDamage(AttackCollider attackCollider)
        {
            base.MakeDamage(attackCollider);
            Collider.enabled = true;
        }

        private IEnumerator DisableJumpAttack()
        {
            _canPerformJumpAttack = false;
            yield return new WaitWhile(() => IsAttacking);
            yield return new WaitForSeconds(jumpAttackCooldown);
            _canPerformJumpAttack = true;
        }
    }
}
