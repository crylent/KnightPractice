using System;
using System.Collections.Generic;
using Enemies;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class PlayerController : LiveEntity
    {
        [SerializeField] private float speed = 25;
        private Vector3 _deltaMove;
        private bool _watchingRight;
        private bool _isAttacking;
    
        [SerializeField] private Collider attackHitbox;
        private Collider _attackHitbox;
        private HashSet<Enemy> _enemiesBeingAttacked; // enemies inside the attack hitbox
    
        private static readonly int Movement = Animator.StringToHash("movement");

        // Start is called before the first frame update
        protected override void Start()
        {
            base.Start();
            PlayerComponents.Init(gameObject);
        }

        private void FixedUpdate()
        {
            var movement = !_isAttacking ? _deltaMove : Vector3.zero; // can't move when attacking
            Rigidbody.velocity = speed * movement;
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            var v = context.ReadValue<Vector2>();
            _deltaMove = new Vector3(v[0], 0, v[1]);
            
            // update animation
            var deltaX = Math.Sign(_deltaMove.x);
            var movementAnimation = (deltaX != 0) ? deltaX : Math.Sign(_deltaMove.z);
            Animator.SetInteger(Movement, movementAnimation);
            _watchingRight = movementAnimation switch
            {
                > 0 => true,
                < 0 => false,
                _ => _watchingRight
            };
        }

        public void OnAttack(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            if (_isAttacking) return;
            
            _isAttacking = true;
            Animator.SetTrigger(AttackTrigger);
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

        public override void MakeDamage()
        {
            foreach (var other in _enemiesBeingAttacked)
            {
                other.TakeDamage();
            }

            Destroy(_attackHitbox.gameObject);
        }

        public override void AfterAttack()
        {
            _isAttacking = false;
        }
    }
}
