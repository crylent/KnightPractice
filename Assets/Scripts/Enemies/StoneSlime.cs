using UnityEngine;

namespace Enemies
{
    public class StoneSlime : Slime
    {
        [SerializeField] private float jumpAttackRange = 20f;
        [SerializeField] private float jumpAttackCooldown = 5f;

        private readonly ActionCooldown _jumpAttackCooldown;
            
        private static readonly int JumpAttackTrigger = Animator.StringToHash("onJump");

        public StoneSlime()
        {
            _jumpAttackCooldown = new ActionCooldown(this, jumpAttackCooldown);
        }

        protected override void AttackBehavior()
        {
            if (GetDistanceToPlayer() < jumpAttackRange && _jumpAttackCooldown.CanPerform)
            {
                Collider.enabled = false; // disable collider while not on the ground
                Attack(JumpAttackTrigger);
                _jumpAttackCooldown.Cooldown();
            }
            else
            {
                base.AttackBehavior();
            }
        }

        public override void MakeDamage(string attackName, AttackCollider attackCollider, float damageDelay)
        {
            base.MakeDamage(attackName, attackCollider, damageDelay);
            Collider.enabled = true;
        }
    }
}
