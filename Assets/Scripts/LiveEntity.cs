using System.Collections;
using System.Collections.Generic;
using Player;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(Animator), typeof(Rigidbody))]
public abstract class LiveEntity : MonoBehaviour
{
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

    [SerializeField] private bool takesFireDamage = true;
    [SerializeField] private float fireResistance = 1f; // affects how frequently entity is damaged by fire

    protected virtual void Start()
    {
        Health = maxHealth;
        Animator = gameObject.GetComponent<Animator>();
        Rigidbody = gameObject.GetComponent<Rigidbody>();
        Collider = gameObject.GetComponent<Collider>();
    }

    public virtual void StartAttack(AttackCollider attackCollider) {}
    public abstract void MakeDamage(AttackCollider attackCollider);
    public virtual void AfterAttack()
    {
        IsAttacking = false;
    }

    public virtual void TakeDamage(LiveEntity producer = null, int damage = 1)
    {
        Health -= damage;
        Animator.SetTrigger(IsAlive ? HitTrigger : DeathTrigger);
    }
    
    // FIRE DAMAGE CONTROLLER
    private readonly List<GameObject> _fireInstancesColliding = new();

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Fire") || !takesFireDamage) return;
        _fireInstancesColliding.Add(other.gameObject);
        StartCoroutine(RemoveAfterDestroying(other.gameObject));
        if (_fireInstancesColliding.Count == 1) // avoid double damaging
        {
            StartCoroutine(StartApplyingFireDamage());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Fire") || !takesFireDamage) return;
        _fireInstancesColliding.Remove(other.gameObject);
    }

    private IEnumerator StartApplyingFireDamage()
    {
        while (_fireInstancesColliding.Count > 0)
        {
            Debug.Log(_fireInstancesColliding.Count);
            PlayerComponents.Controller.TakeDamage();
            yield return new WaitForSeconds(fireResistance);
        }
    }

    private IEnumerator RemoveAfterDestroying(GameObject fire)
    {
        yield return new WaitUntil(fire.IsDestroyed);
        if (_fireInstancesColliding.Contains(fire))
        {
            _fireInstancesColliding.Remove(fire);
        }
    }
}
