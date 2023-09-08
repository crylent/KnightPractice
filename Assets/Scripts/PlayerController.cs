using System;
using System.Collections.Generic;
using Enemies;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float health = 100;
    [SerializeField] private float speed = 25;
    private Vector3 _deltaMove;
    private AttackPhase _attackPhase; // true if animation (attack or other) should block other animations until completed
    private Animator _animator;
    private Rigidbody _rigidbody;
    
    [SerializeField] private Collider attackHitbox;
    private Collider _attackHitbox;
    private HashSet<Enemy> _enemiesBeingAttacked; // enemies inside the attack hitbox
    
    private static readonly int Hit = Animator.StringToHash("onHit");
    private static readonly int Attack = Animator.StringToHash("onAttack");
    private static readonly int IsRunning = Animator.StringToHash("isRunning");

    // Start is called before the first frame update
    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _animator = transform.GetChild(0).GetComponent<Animator>();
        PlayerComponents.Init(gameObject);
    }

    // Update is called once per frame
    private void Update()
    {
        var state = _animator.GetCurrentAnimatorStateInfo(0);
        var normalizedTime = state.normalizedTime;
        switch (_attackPhase)
        {
            case AttackPhase.BeforeHit when state.IsName("Character_Attack") && normalizedTime >= 0.5: // make damage
                Strike(); break;
            case AttackPhase.AfterHit when !state.IsName("Character_Attack"): // attack animation finished
                OnAttackCompleted(); break;
            case AttackPhase.NotAttacking:
            default:
                break;
        }
    }

    private void FixedUpdate()
    {
        var isMoving = _deltaMove.magnitude != 0 && _attackPhase == AttackPhase.NotAttacking;
        _animator.SetBool(IsRunning, isMoving);

        _rigidbody.velocity = isMoving ? speed * _deltaMove : Vector3.zero;
        
        if (isMoving && _deltaMove.x != 0)
        {
            transform.localScale = new Vector3(_deltaMove.x > 0 ? 1 : -1, 1, 1);
        }
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
        _attackHitbox = Instantiate(attackHitbox, gameObject.transform);
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
