using System;
using System.Collections;
using UnityEngine;
using Utility;

[Serializable]
public class Shield
{
    [SerializeField] public int defaultStability = 3;
    [SerializeField] public float defaultRecoverTime = 15f;
    [SerializeField] public ParticleSystem blockEffect;

    private Func<float> _recoverTimeModifier = () => 1f;
    private float RecoverTime => _recoverTimeModifier.Invoke() * defaultRecoverTime;

    public int Stability { get; private set; }
    public bool IsBlocking { get; private set; }

    private LiveEntity _owner;
    private Animator _animator;
    private static readonly int IsBlockingBool = Animator.StringToHash("isBlocking");
    private static readonly int ShieldDamageInt = Animator.StringToHash("shieldDamage");
    
    public void Init(LiveEntity owner)
    {
        _owner = owner;
        _animator = owner.GetComponent<Animator>();
    }

    public void AssignRecoverTimeModifier(Func<float> modifier)
    {
        _recoverTimeModifier = modifier;
    }

    public void SetBlock(bool blocking)
    {
        IsBlocking = blocking;
        _animator.SetBool(IsBlockingBool, blocking);
    }

    public void Reset()
    {
        Stability = defaultStability;
        ReflectShieldDamage();
    }

    public void Hit(int deltaStability)
    {
        _owner.StartCoroutine(DamageShield(deltaStability));
    }

    private IEnumerator DamageShield(int deltaStability)
    {
        Effects.PlayEffectOnce(blockEffect, _owner.transform);
        Stability -= deltaStability;
        ReflectShieldDamage();
        if (Stability <= 0) SetBlock(false);
            
        yield return new WaitForSeconds(RecoverTime);
        if (Stability >= defaultStability) yield break;
        Stability += deltaStability; // recover stability later
        ReflectShieldDamage();
    }

    private void ReflectShieldDamage()
    {
        _animator.SetInteger(ShieldDamageInt, defaultStability - Stability);
    }
}