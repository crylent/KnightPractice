using System;
using System.Collections.Generic;
using Enemies;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private float health = 100;
        [SerializeField] private float speed = 25;
        private Vector3 _deltaMove;
        private bool _watchingRight;
        private AttackPhase _attackPhase; // true if animation (attack or other) should block other animations until completed
        private Animator _animator;
        private Rigidbody _rigidbody;
    
        [SerializeField] private Collider attackHitbox;
        private Collider _attackHitbox;
        private HashSet<Enemy> _enemiesBeingAttacked; // enemies inside the attack hitbox
    
        private static readonly int Hit = Animator.StringToHash("onHit");
        private static readonly int Attack = Animator.StringToHash("onAttack");
        private static readonly int Movement = Animator.StringToHash("movement");

        // Start is called before the first frame update
        private void Start()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _animator = GetComponent<Animator>();
            PlayerComponents.Init(gameObject);
        }

        // Update is called once per frame
        private void Update()
        {
            var normalizedTime = GetAnimatorState().normalizedTime;
            switch (_attackPhase)
            {
                case AttackPhase.BeforeHit when IsPerformingAttackAnimation() && normalizedTime >= 0.5: // make damage
                    Strike(); break;
                case AttackPhase.AfterHit when !IsPerformingAttackAnimation(): // attack animation finished
                    OnAttackCompleted(); break;
                case AttackPhase.NotAttacking:
                default:
                    break;
            }
        }

        private AnimatorStateInfo GetAnimatorState()
        {
            return _animator.GetCurrentAnimatorStateInfo(0);
        }

        private bool IsPerformingAttackAnimation()
        {
            var state = GetAnimatorState();
            return state.IsName("Cat_Attack") || state.IsName("Cat_AttackRight");
        }

        private void FixedUpdate()
        {
            var movement = (_attackPhase == AttackPhase.NotAttacking) ? _deltaMove : Vector3.zero; // can't move when attacking
            
            var deltaX = Math.Sign(movement.x);
            var movementAnimation = (deltaX != 0) ? deltaX : Math.Sign(movement.z);
            _animator.SetInteger(Movement, movementAnimation);
            _watchingRight = movementAnimation switch
            {
                > 0 => true,
                < 0 => false,
                _ => _watchingRight
            };

            _rigidbody.velocity = speed * movement;
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            var v = context.ReadValue<Vector2>();
            _deltaMove = new Vector3(v[0], 0, v[1]);
        }

        public void OnAttack(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            if (_attackPhase != AttackPhase.NotAttacking) return;
        
            _attackPhase = AttackPhase.BeforeHit;
            _animator.SetTrigger(Attack);
            _attackHitbox = Instantiate(attackHitbox, transform);
            if (!_watchingRight) // rotate hitbox to the left
            {
                var hitboxTransform = _attackHitbox.transform;
                var hitboxRot = hitboxTransform.rotation.eulerAngles;
                hitboxRot.y -= 180;
                hitboxTransform.rotation = Quaternion.Euler(hitboxRot);
            }
            _enemiesBeingAttacked = _attackHitbox.GetComponent<ColliderController>().Enemies;
        }

        public void OnHit(float damageDone)
        {
            health -= damageDone;
            _animator.SetTrigger(Hit);
            _attackPhase = AttackPhase.NotAttacking;
        }

        private void Strike()
        {
            foreach (var other in _enemiesBeingAttacked)
            {
                other.OnHit(10);
            }

            _attackPhase = AttackPhase.AfterHit;
            Destroy(_attackHitbox.gameObject);
        }

        private void OnAttackCompleted()
        {
            _attackPhase = AttackPhase.NotAttacking;
        }
    }
}
