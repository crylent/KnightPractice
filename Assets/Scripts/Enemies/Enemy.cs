using Player;
using UnityEngine;

namespace Enemies
{
    public abstract class Enemy : MonoBehaviour, ICanAttack
    {
        protected Vector3 Movement = Vector3.zero;

        [SerializeField] protected float health;

        [SerializeField] private Collider hitbox;
        private Collider _hitbox;

        private OpacityController.OpacityController _opacityController;
        protected Animator animator;
        protected new Rigidbody rigidbody;
        private bool _isAttacking;
        
        private static readonly int AttackTrigger = Animator.StringToHash("onAttack");
        private static readonly int HitTrigger = Animator.StringToHash("onHit");
        private static readonly int DeathTrigger = Animator.StringToHash("onDeath");

        protected void Start()
        {
            animator = gameObject.GetComponent<Animator>();
            rigidbody = gameObject.GetComponent<Rigidbody>();
        }

        protected void Update()
        {
            BehaviorUpdate();
        }

        protected abstract void BehaviorUpdate();

        protected void Attack()
        {
            if (_isAttacking) return;
            
            _isAttacking = true;
            animator.SetTrigger(AttackTrigger);
            _hitbox = Instantiate(hitbox, gameObject.transform);
        }

        public void ApplyDamage()
        {
            if (_hitbox.bounds.Intersects(PlayerComponents.Collider.bounds))
            {
                PlayerComponents.Controller.OnHit(10);
            }
            Destroy(_hitbox.gameObject);
            _hitbox = null;
        }

        public void AfterAttack()
        {
            _isAttacking = false;
        }

        public void OnHit(float damageDone)
        {
            health -= damageDone;
            animator.SetTrigger(health <= 0 ? DeathTrigger : HitTrigger);
        }
    }
}
