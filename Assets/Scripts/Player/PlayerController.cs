using System;
using System.Collections;
using System.Collections.Generic;
using Enemies;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using Utility;
using VFX;

namespace Player
{
    public class PlayerController : LiveEntity
    {
        private Vector3 _deltaMove;
        
        [SerializeField] private float dodgeSpeed = 200f;
        [SerializeField] private float dodgeTime = 0.15f;
        [SerializeField] private float dodgeManaConsumption = 5f;
        [SerializeField] private ParticleSystem dodgeEffect;
        private bool _isDodging;

        [SerializeField] private int shieldDefaultStability = 3;
        [SerializeField] private float shieldStabilityRecoverTime = 15f;
        [SerializeField] private float speedFactorWhenBlocking = 0.25f;
        [SerializeField] private ParticleSystem blockEffect;
        private int _shieldStability;
        private bool _isBlocking;

        [SerializeField] private float maxMana = 100f;
        [SerializeField] private float manaRecovery = 1f;
        public float MaxMana => maxMana;
        private float _mana;

        private AttackCollider _attackHitbox;
        private HashSet<Enemy> _enemiesBeingAttacked; // enemies inside the attack hitbox
        
        [SerializeField] private UnityEvent<int> onHealthChanged;
        [SerializeField] private UnityEvent<float> onManaChanged;
        [SerializeField] private UnityEvent<float> onFreezeChanged;

        private static readonly int RunAnimSpeed = Animator.StringToHash("runAnimSpeed");
        private static readonly int AttackAnimSpeed = Animator.StringToHash("attackAnimSpeed");
        private static readonly int IsRunningBool = Animator.StringToHash("isRunning");
        private static readonly int IsWatchingRightBool = Animator.StringToHash("isWatchingRight");
        private static readonly int IsBlockingBool = Animator.StringToHash("isBlocking");
        private static readonly int ShieldDamageInt = Animator.StringToHash("shieldDamage");
        private static readonly int GoToSecondAttackBool = Animator.StringToHash("goToSecondAttack");

        // Start is called before the first frame update
        protected override void Start()
        {
            base.Start();
            _mana = maxMana;
            _shieldStability = shieldDefaultStability;
            PlayerComponents.Init(gameObject);
        }

        protected override void Update()
        {
            base.Update();
            if (!IsFrozen) _mana = Math.Min(_mana + Time.deltaTime * manaRecovery, maxMana); // recover mana
            onManaChanged.Invoke(_mana);
            onFreezeChanged.Invoke(Freeze);
        }

        private void FixedUpdate()
        {
            if (_isDodging) return; // ignore default movement when dodging
            var movement = !IsAttacking ? _deltaMove : Vector3.zero; // stop movement when attacking
            if (_isBlocking) movement *= speedFactorWhenBlocking; // slow down movement when blocking
            Rigidbody.velocity = Speed * movement;
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            var v = context.ReadValue<Vector2>();
            _deltaMove = new Vector3(v[0], 0, v[1]);
            
            // update animation
            var deltaX = Math.Sign(_deltaMove.x);
            var deltaZ = Math.Sign(_deltaMove.z);
            IsWatchingRight = deltaX > 0 || (deltaX == 0 && (deltaZ > 0 || (deltaZ == 0 && IsWatchingRight)));
            Animator.SetBool(IsWatchingRightBool, IsWatchingRight);
            Animator.SetBool(IsRunningBool, deltaX != 0 || deltaZ != 0);
            Animator.SetFloat(RunAnimSpeed, (_isBlocking ? 0.5f : 1f) * (IsFrozen ? 0.75f : 1f));
        }

        private bool _prepareOneMoreAttack;

        public void OnAttack(InputAction.CallbackContext context)
        {
            if (!context.performed || _isDodging) return;
            if (IsAttacking)
            {
                _prepareOneMoreAttack = true;
                Animator.SetBool(GoToSecondAttackBool, true);
                return;
            }
            Attack();
        }

        private void Attack()
        {
            IsAttacking = true;
            if (_isBlocking) SetBlock(false); // stop blocking
            Animator.SetTrigger(AttackTrigger);
            Animator.SetFloat(AttackAnimSpeed, AttackAnimationSpeed);
        }

        private float AttackAnimationSpeed => IsFrozen ? 0.5f : 1f;

        public void OnBlock(InputAction.CallbackContext context)
        {
            if (context.started && _shieldStability > 0) SetBlock(true);
            else if (context.canceled) SetBlock(false);
        }

        private void SetBlock(bool blocking)
        {
            _isBlocking = blocking;
            Animator.SetBool(IsBlockingBool, blocking);
        }

        public void OnDodge(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            if (!_isDodging && _mana >= dodgeManaConsumption) StartCoroutine(Dodge());
        }

        private IEnumerator Dodge()
        {
            _isDodging = true;
            
            yield return new WaitWhile(() => IsAttacking); // can't dodge while attacking, but will do it after finishing
            Effects.PlayEffectOnce(dodgeEffect, transform);
            Rigidbody.velocity = _deltaMove * dodgeSpeed;
            
            // consume mana
            _mana -= dodgeManaConsumption;
            onManaChanged.Invoke(_mana);
            
            yield return new WaitForSeconds(dodgeTime);
            _isDodging = false;
        }

        public override void StartAttack(string attackName,
            AttackCollider attackCollider, ParticleSystem attackEffect)
        {
            if (attackEffect.IsUnityNull()) return;
            foreach (var system in attackEffect!.GetComponentsInChildren<ParticleSystem>())
            {
                var main = system.main;
                main.simulationSpeed = AttackAnimationSpeed;
            }
            Effects.PlayEffectOnce(attackEffect, transform, true);
        }

        protected override void MakeDamageOnTarget(AttackCollider hitbox)
        {
            foreach (var other in hitbox.Enemies)
            {
                other.TakeDamage(this);
            }
        }

        public override void AfterAttack()
        {
            base.AfterAttack();
            
            if (!_prepareOneMoreAttack) return;
            _prepareOneMoreAttack = false;
            Animator.SetBool(GoToSecondAttackBool, false);
        }

        public override void TakeDamage(LiveEntity producer = null, int damage = 1)
        {
            if (_isBlocking && !producer.IsUnityNull() && 
                (IsWatchingRight && producer!.transform.position.x > transform.position.x ||
                !IsWatchingRight && producer!.transform.position.x < transform.position.x)
               ) // can't block damage from environment or backstabs
            {
                StartCoroutine(DamageShield(damage));
                Effects.PlayEffectOnce(blockEffect, transform);
            }
            else
            {
                base.TakeDamage(producer, damage);
                onHealthChanged.Invoke(Health);
                if (!IsAlive) FindObjectOfType<GameManager>().StopGame(true);
            }
        }

        private IEnumerator DamageShield(int deltaStability)
        {
            _shieldStability -= deltaStability;
            ReflectShieldDamage();
            if (_shieldStability <= 0) SetBlock(false);
            
            yield return new WaitForSeconds(shieldStabilityRecoverTime);
            if (_shieldStability >= shieldDefaultStability) yield break;
            _shieldStability += deltaStability; // recover stability later
            ReflectShieldDamage();
        }

        private void ReflectShieldDamage()
        {
            Animator.SetInteger(ShieldDamageInt, shieldDefaultStability - _shieldStability);
        }

        public void ResetPlayer()
        {
            Health = MaxHealth;
            _mana = MaxMana;
            Freeze = 0;
            _shieldStability = shieldDefaultStability;
            ReflectShieldDamage();
            onHealthChanged.Invoke(Health);
            onFreezeChanged.Invoke(Freeze);
            transform.position = Vector3.zero;
        }

        private void OnTriggerEnter(Collider col)
        {
            if (!col.TryGetComponent<ManaParticle>(out var manaParticle)) return;
            _mana += manaParticle.manaValue;
            onManaChanged.Invoke(_mana);
            Destroy(col.gameObject);
        }
    }
}
