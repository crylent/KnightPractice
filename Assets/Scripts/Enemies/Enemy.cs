using Player;
using UnityEngine;

namespace Enemies
{
    public abstract class Enemy : LiveEntity
    {
        protected Vector3 Movement = Vector3.zero;

        [SerializeField] private Collider hitbox;
        private Collider _hitbox;

        private OpacityController.OpacityController _opacityController;
        private bool _isAttacking;

        protected void Update()
        {
            BehaviorUpdate();
        }

        protected abstract void BehaviorUpdate();

        protected void Attack()
        {
            if (_isAttacking) return;
            
            _isAttacking = true;
            Animator.SetTrigger(AttackTrigger);
            _hitbox = Instantiate(hitbox, gameObject.transform);
        }

        public override void MakeDamage()
        {
            if (_hitbox.bounds.Intersects(PlayerComponents.Collider.bounds))
            {
                PlayerComponents.Controller.TakeDamage();
            }
            Destroy(_hitbox.gameObject);
            _hitbox = null;
        }

        public override void AfterAttack()
        {
            _isAttacking = false;
        }
    }
}
