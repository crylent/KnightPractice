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
using Random = UnityEngine.Random;

namespace Player
{
    public class PlayerController : LiveEntity
    {
        private Vector3 _deltaMove;
        
        [SerializeField] private float dodgeSpeed = 200f;
        [SerializeField] private float dodgeTime = 0.15f;
        [SerializeField] private float dodgeManaConsumption = 5f;
        [SerializeField] private ParticleSystem dodgeEffect;
        [SerializeField] private AudioClip dodgeSound;
        private bool _isDodging;

        [SerializeField] private Shield shield;
        [SerializeField] public float speedFactorWhenBlocking = 0.25f;

        [SerializeField] private float maxMana = 100f;
        [SerializeField] private float manaRecovery = 1f;
        public float MaxMana => maxMana;
        private float _mana;

        [SerializeField] private int criticalDamageFactor = 2;
        [SerializeField] private ParticleSystem criticalHitEffect;
        [SerializeField] private AudioClip criticalHitSound;

        private AttackCollider _attackHitbox;
        private HashSet<Enemy> _enemiesBeingAttacked; // enemies inside the attack hitbox
        
        private bool _isBeingPushed;

        private AudioSource _audio;
        
        [SerializeField] private UnityEvent<int> onHealthChanged;
        [SerializeField] private UnityEvent<float> onManaChanged;
        [SerializeField] private UnityEvent<float> onFreezeChanged;

        private static readonly int RunAnimSpeed = Animator.StringToHash("runAnimSpeed");
        private static readonly int AttackAnimSpeed = Animator.StringToHash("attackAnimSpeed");
        private static readonly int IsRunningBool = Animator.StringToHash("isRunning");
        private static readonly int IsWatchingRightBool = Animator.StringToHash("isWatchingRight");
        private static readonly int GoToSecondAttackBool = Animator.StringToHash("goToSecondAttack");
        private static readonly int HasDarknessEffectBool = Animator.StringToHash("hasDarknessEffect");
        private static readonly int OnEvasionTrigger = Animator.StringToHash("onEvasion");

        public class PlayerModifiers
        {
            public float ShieldStabilityRecoverTime = 1f;
            public float Speed = 1f;
            public float ManaRecovery = 1f;
            public float DodgeManaConsumption = 1f;
            public bool InvulnerableWhenDodging = false;
            public float CastManaConsumption = 1f;
            public float EvasionChance = 0f;
            public float CriticalHitChance = 0f;
            public float VampirismChance = 0f;
        }

        public PlayerModifiers Modifiers { get; private set; } = new();
        
        private float FinalSpeed => Speed * Modifiers.Speed;
        private float FinalManaRecovery => manaRecovery * Modifiers.ManaRecovery;
        private float FinalDodgeManaConsumption => dodgeManaConsumption * Modifiers.DodgeManaConsumption;

        // Start is called before the first frame update
        protected override void Start()
        {
            base.Start();
            _mana = maxMana;
            shield.Init(this);
            shield.AssignRecoverTimeModifier(() => Modifiers.ShieldStabilityRecoverTime);
            shield.Reset();
            _audio = GetComponent<AudioSource>();
            PlayerComponents.Init(gameObject);
        }

        protected override void Update()
        {
            base.Update();
            if (!IsFrozen) _mana = Math.Min(_mana + Time.deltaTime * FinalManaRecovery, maxMana); // recover mana
            onManaChanged.Invoke(_mana);
            onFreezeChanged.Invoke(Freeze);
        }

        private void FixedUpdate()
        {
            if (_isDodging) return; // ignore default movement when dodging
            var movement = !IsAttacking ? _deltaMove : Vector3.zero; // stop movement when attacking
            if (shield.IsBlocking) movement *= speedFactorWhenBlocking; // slow down movement when blocking
            Rigidbody.velocity = FinalSpeed * movement;
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            if (_isBeingPushed) return;
            var v = context.ReadValue<Vector2>();
            _deltaMove = new Vector3(v[0], 0, v[1]);
            
            // update animation
            var deltaX = Math.Sign(_deltaMove.x);
            var deltaZ = Math.Sign(_deltaMove.z);
            IsWatchingRight = deltaX > 0 || (deltaX == 0 && (deltaZ > 0 || (deltaZ == 0 && IsWatchingRight)));
            Animator.SetBool(IsWatchingRightBool, IsWatchingRight);
            Animator.SetBool(IsRunningBool, deltaX != 0 || deltaZ != 0);
            Animator.SetFloat(RunAnimSpeed, (shield.IsBlocking ? 0.5f : 1f) * (IsFrozen ? 0.75f : 1f));
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
            if (shield.IsBlocking) shield.SetBlock(false); // stop blocking
            Animator.SetTrigger(AttackTrigger);
            Animator.SetFloat(AttackAnimSpeed, AttackAnimationSpeed);
        }

        public void OnBlock(InputAction.CallbackContext context)
        {
            if (context.started && shield.Stability > 0) shield.SetBlock(true);
            else if (context.canceled) shield.SetBlock(false);
        }

        public void OnDodge(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            if (!_isDodging && _mana >= FinalDodgeManaConsumption) StartCoroutine(Dodge());
        }

        private IEnumerator Dodge()
        {
            _isDodging = true;
            
            yield return new WaitWhile(() => IsAttacking); // can't dodge while attacking, but will do it after finishing
            Effects.PlayEffectOnce(dodgeEffect, transform);
            _audio.PlayOneShot(dodgeSound);
            Rigidbody.velocity = _deltaMove * dodgeSpeed;
            
            // consume mana
            _mana -= FinalDodgeManaConsumption;
            onManaChanged.Invoke(_mana);
            
            yield return new WaitForSeconds(dodgeTime);
            _isDodging = false;
        }

        protected override void MakeDamageOnTarget(AttackCollider hitbox)
        {
            foreach (var other in hitbox.Enemies)
            {
                var damage = 1;
                if (Modifiers.CriticalHitChance > 0 && Random.Range(0f, 1f) < Modifiers.CriticalHitChance)
                {
                    damage *= criticalDamageFactor; // critical hit
                    Effects.PlayEffectOnce(criticalHitEffect, other.transform);
                    _audio.PlayOneShot(criticalHitSound);
                }
                other.TakeDamage(this, damage);
                if (!other.IsAlive && Modifiers.VampirismChance > 0 && Random.Range(0f, 1f) < Modifiers.VampirismChance)
                {
                    RecoverHealth(); // vampirism effect
                }
            }
        }

        public override void AfterAttack(ParticleSystem afterAttackEffect, bool effectIsAttached = true)
        {
            base.AfterAttack(afterAttackEffect, effectIsAttached);
            
            if (!_prepareOneMoreAttack) return;
            _prepareOneMoreAttack = false;
            Animator.SetBool(GoToSecondAttackBool, false);
        }

        public override void TakeDamage(LiveEntity producer = null, int damage = 1)
        {
            if (_isDodging && Modifiers.InvulnerableWhenDodging) return;
            if (Modifiers.EvasionChance > 0)
            {
                var rand = Random.Range(0f, 1f);
                if (rand < Modifiers.EvasionChance)
                {
                    Animator.SetTrigger(OnEvasionTrigger);
                    return;
                }
            }
            if (shield.IsBlocking && !producer.IsUnityNull() && 
                (IsWatchingRight && producer!.transform.position.x > transform.position.x ||
                 !IsWatchingRight && producer!.transform.position.x < transform.position.x)
               ) // can't block damage from environment or backstabs
            {
                shield.Hit(damage);
            }
            else
            {
                base.TakeDamage(producer, damage);
                if (producer is DarkGhost darkGhost)
                {
                    StartCoroutine(CastDarknessEffect(darkGhost.darknessEffectTime));
                }
                onHealthChanged.Invoke(Health);
                if (!IsAlive) FindObjectOfType<GameManager>().StopGame(true);
            }
        }

        public void GetPushed(Vector3 force)
        {
            _isBeingPushed = true;
            _deltaMove = force;
            StartCoroutine(StopBeingPushed());
        }

        private IEnumerator StopBeingPushed()
        {
            yield return new WaitForSeconds(0.4f);
            _isBeingPushed = false;
            _deltaMove = Vector3.zero;
        }

        public void RecoverHealth(int hp = 1)
        {
            Health = Math.Min(MaxHealth, Health + hp);
            onHealthChanged.Invoke(Health);
        }

        private int _darknessEffectCounter;
        
        private IEnumerator CastDarknessEffect(float time)
        {
            Animator.SetBool(HasDarknessEffectBool, true);
            _darknessEffectCounter++;
            yield return new WaitForSeconds(time);
            _darknessEffectCounter--;
            if (_darknessEffectCounter == 0) Animator.SetBool(HasDarknessEffectBool, false);
        }

        public void ResetPlayer()
        {
            Modifiers = new PlayerModifiers(); // reset effects
            Health = MaxHealth;
            _mana = MaxMana;
            Freeze = 0;
            shield.Reset();
            onHealthChanged.Invoke(Health);
            onFreezeChanged.Invoke(Freeze);
            transform.position = Vector3.zero;
            Collider.enabled = true;
        }

        // consume mana particles
        protected override void OnTriggerEnter(Collider col)
        {
            base.OnTriggerEnter(col);
            if (!col.TryGetComponent<ManaParticle>(out var manaParticle)) return;
            _mana += manaParticle.manaValue * Modifiers.ManaRecovery;
            onManaChanged.Invoke(_mana);
            Destroy(col.gameObject);
        }
    }
}
