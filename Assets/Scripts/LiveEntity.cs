using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Player;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using Utility;

[RequireComponent(
    typeof(Animator), 
    typeof(Rigidbody), 
    typeof(Collider)
    )]
[RequireComponent(typeof(AudioSource))]
public abstract class LiveEntity : GameEntity
{
    [SerializeField] private float speed = 50f;
    protected float Speed => !IsFrozen ? speed : speed * 0.5f; // slower when frozen
    
    protected bool IsWatchingRight;
    
    [FormerlySerializedAs("health")] [SerializeField] private int maxHealth;
    public int MaxHealth => maxHealth;

    protected int Health;
    public bool IsAlive => Health > 0;
    
    protected Animator Animator;
    protected Collider Collider;
    protected Rigidbody Rigidbody;
    protected bool IsAttacking;
    
    protected static readonly int AttackTrigger = Animator.StringToHash("onAttack");
    private static readonly int HitTrigger = Animator.StringToHash("onHit");
    private static readonly int DeathTrigger = Animator.StringToHash("onDeath");

    [SerializeField] private float fireResistance; // by default, fire makes 1 damage in 1 second; resistance increase this time
    [SerializeField] private float iceResistance; // by default, ice bar becomes full in 1 second; resistance increase this time

    private ParticleSystem _frozenEffect;
    private AddressableSingleHandler<ParticleSystem> _frozenEffectHandler;

    protected virtual void Start()
    {
        Health = maxHealth;
        Animator = gameObject.GetComponent<Animator>();
        Rigidbody = gameObject.GetComponent<Rigidbody>();
        Collider = gameObject.GetComponent<Collider>();

        _frozenEffectHandler = new AddressableSingleHandler<ParticleSystem>(this, "Frozen");
    }

    public virtual void StartAttack(string attackName, [CanBeNull] AttackCollider attackCollider, [CanBeNull] ParticleSystem attackEffect) {}

    public virtual void MakeDamage(string attackName, [CanBeNull] AttackCollider attackCollider)
    {
        if (attackCollider.IsUnityNull() || !IsAlive) return;
        StartCoroutine(MakeDamageCoroutine(attackCollider));
    }
    public virtual void AfterAttack()
    {
        IsAttacking = false;
    }

    private IEnumerator MakeDamageCoroutine(AttackCollider attackCollider)
    {
        var hitbox = Instantiate(attackCollider, gameObject.transform);
        hitbox.transform.parent = null; // detach from parent to ignore other attack colliders
        yield return new WaitForFixedUpdate(); // wait for OnTriggerEnter execution
        MakeDamageOnTarget(hitbox);
        var childSystem = hitbox.GetComponentInChildren<ParticleSystem>();
        if (!childSystem.IsUnityNull()) // if has child particle system, delay destroying
            yield return new WaitWhile(() => childSystem.IsAlive());
        Destroy(hitbox.gameObject);
    }

    protected abstract void MakeDamageOnTarget(AttackCollider hitbox);

    public virtual void TakeDamage(LiveEntity producer = null, int damage = 1)
    {
        Health = Math.Max(Health - damage, 0);
        Animator.SetTrigger(IsAlive ? HitTrigger : DeathTrigger);
        if (!IsAlive) Collider.enabled = false;
    }
    
    // PERIODIC DAMAGE CONTROLLER
    private readonly List<GameObject> _fireInstancesColliding = new();
    private readonly List<GameObject> _iceInstancesColliding = new();
    protected float Freeze;
    protected bool IsFrozen;

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Fire") && fireResistance < 1f)
        {
            _fireInstancesColliding.Add(other.gameObject);
            StartCoroutine(RemoveAfterDestroying(other.gameObject));
            if (_fireInstancesColliding.Count == 1) // avoid double damaging
            {
                StartCoroutine(StartApplyingFireDamage());
            }
        }
        else if (other.CompareTag("Ice") && iceResistance < 1f)
        {
            _iceInstancesColliding.Add(other.gameObject);
            StartCoroutine(RemoveAfterDestroying(other.gameObject));
        }
    }

    protected virtual void Update()
    {
        Freeze = _iceInstancesColliding.Count > 0 ?
            Math.Min(Freeze + 1f / (1f - iceResistance) * Time.deltaTime, 1f) :
            Math.Max(Freeze - 0.1f * Time.deltaTime, 0f);
        IsFrozen = Freeze switch
        {
            >= 1f when !IsFrozen => true,
            <= 0f when IsFrozen => false,
            _ => IsFrozen
        };

        // ReSharper disable once ConvertIfStatementToSwitchStatement
        if (!_frozenEffectHandler.HasInstance && IsFrozen) // add frozen effect
        {
            _frozenEffectHandler.Instantiate();
        }
        else if (_frozenEffectHandler.HasInstance && !IsFrozen) // remove frozen effect
        {
            Effects.StopEffect(_frozenEffectHandler.PopInstance());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Fire") && fireResistance < 1f) _fireInstancesColliding.Remove(other.gameObject);
        else if (other.CompareTag("Ice") && iceResistance < 1f) _iceInstancesColliding.Remove(other.gameObject);
    }

    private IEnumerator StartApplyingFireDamage()
    {
        while (_fireInstancesColliding.Count > 0)
        {
            TakeDamage();
            yield return new WaitForSeconds(1f / (1f - fireResistance));
        }
    }

    private IEnumerator RemoveAfterDestroying(GameObject obj)
    {
        yield return new WaitUntil(obj.IsDestroyed);
        if (_fireInstancesColliding.Contains(obj)) _fireInstancesColliding.Remove(obj);
        else if (_iceInstancesColliding.Contains(obj)) _iceInstancesColliding.Remove(obj);
    }
}
