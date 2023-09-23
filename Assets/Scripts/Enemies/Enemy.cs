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

        protected void Update()
        {
            BehaviorUpdate();
        }

        protected abstract void BehaviorUpdate();

        protected void Attack()
        {
            if (IsAttacking) return;
            
            IsAttacking = true;
            Animator.SetTrigger(AttackTrigger);
            _hitbox = Instantiate(hitbox, gameObject.transform);
        }

        public override void MakeDamage()
        {
            if (_hitbox.bounds.Intersects(PlayerComponents.Collider.bounds))
            {
                PlayerComponents.Controller.TakeDamage(this);
            }
            Destroy(_hitbox.gameObject);
            _hitbox = null;
        }
    }
}
