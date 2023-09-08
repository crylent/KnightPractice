using UnityEngine;

namespace Enemies
{
    public abstract class Enemy : MonoBehaviour
    {
        protected bool PlayerDetected;

        protected Vector3 Movement = Vector3.zero;

        [SerializeField] protected float health;
        [SerializeField] protected float detectionRange = 100;
        [SerializeField] protected float chaseRange = 100;

        [SerializeField] private Collider hitbox;
        private Collider _hitbox;

        private Animator _animator;
        private AttackPhase _attackPhase;

        protected void Start()
        {
            _animator = gameObject.GetComponentInChildren<Animator>();
        }

        protected void Update()
        {
            var normalizedTime = _animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
            switch (_attackPhase)
            {
                case AttackPhase.BeforeHit when normalizedTime >= 0.5: // make damage
                    if (_hitbox.bounds.Intersects(PlayerComponents.Collider.bounds))
                    {
                        PlayerComponents.Controller.OnHit(10);
                    }
                    _attackPhase = AttackPhase.AfterHit;
                    Destroy(_hitbox.gameObject);
                    break;
                case AttackPhase.AfterHit when normalizedTime >= 1: // attack animation finished
                    _attackPhase = AttackPhase.NotAttacking;
                    break;
                case AttackPhase.NotAttacking:
                default:
                    break;
            }
            
            if (Movement.x != 0)
            {
                transform.localScale = new Vector3(Movement.x < 0 ? 1 : -1, 1, 1);
            }

            if (_attackPhase == AttackPhase.NotAttacking)
            {
                transform.Translate(Movement * Time.deltaTime);
            }
        }

        protected void Attack()
        {
            if (_attackPhase != AttackPhase.NotAttacking) return;
            
            _attackPhase = AttackPhase.BeforeHit;
            _animator.Play("attack");
            _hitbox = Instantiate(hitbox, gameObject.transform);
        }

        public void OnHit(float damageDone)
        {
            health -= damageDone;
            if (health <= 0)
            {
                Destroy(gameObject);
            }
        }
    }
}
