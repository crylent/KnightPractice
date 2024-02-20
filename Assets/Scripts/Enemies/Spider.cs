using UnityEngine;

namespace Enemies
{
    public class Spider : Enemy
    {
        [SerializeField] private float jumpCooldown = 5f;
        
        private static readonly int JumpTrigger = Animator.StringToHash("onJump");
        private ActionCooldown _jumpCooldown;

        protected override void Start()
        {
            base.Start();
            _jumpCooldown = new ActionCooldown(this, jumpCooldown);
        }
        
        protected override void BehaviorUpdate()
        {
            AttackBehavior();
        }

        private void AttackBehavior()
        {
            if (IsAttacking || !_jumpCooldown.CanPerform) return;
            Attack(JumpTrigger);
        }

        public override void StartAttack(string attackName, ParticleSystem attackEffect)
        {
            if (!IsAlive) return;
            var direction = GetDirectionToPlayer();
            Movement = direction.normalized * Speed;
            Collider.enabled = false;
        }

        public override void AfterAttack()
        {
            base.AfterAttack();
            Movement = Vector3.zero;
            _jumpCooldown.Cooldown();
            Collider.enabled = true;
        }

        private void FixedUpdate()
        {
            if (BehaviorEnabled)
            {
                Rigidbody.velocity = Movement;
            }
        }
    }
}
