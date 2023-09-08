using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class OpacityController : MonoBehaviour
{
    [SerializeField] private float opacity = 1.0f;
    private readonly List<SpriteRenderer> _sprites = new();
    private Animator _animator;

    public enum State
    {
        Visible, // opacity = 1
        Hidden, // opacity = 0.5
        Invisible // opacity = 0
    }
    
    private State _currentState;
    [DoNotSerialize] public State demandedState;

    private static readonly int FadeOutToZeroTrigger = Animator.StringToHash("fadeOutToZero");
    private static readonly int FadeOutToHalfTrigger = Animator.StringToHash("fadeOutToHalf");
    private static readonly int FadeInTrigger = Animator.StringToHash("fadeIn");
    private static readonly int FadeInFromZeroTrigger = Animator.StringToHash("fadeInFromZero");

    private void Start()
    {
        var rootSprite = GetComponent<SpriteRenderer>();
        if (rootSprite != null) _sprites.Add(rootSprite);

        _sprites.AddRange(GetComponentsInChildren<SpriteRenderer>());
        
        _animator = GetComponent<Animator>();
    }

    protected void Update()
    {
        if (demandedState != _currentState)
        {
            switch (demandedState)
            {
                case State.Visible:
                    _animator.SetTrigger(FadeInTrigger);
                    break;
                case State.Hidden:
                    _animator.SetTrigger(FadeOutToHalfTrigger);
                    break;
                case State.Invisible:
                    _animator.SetTrigger(FadeOutToZeroTrigger);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        _currentState = demandedState;

        foreach (var sprite in _sprites)
        {
            var color = sprite.color;
            color.a = opacity;
            sprite.color = color;
        }
    }

    public void FadeInFromZero()
    {
        StartCoroutine(FadeInFromZeroCoroutine());
    }

    private IEnumerator FadeInFromZeroCoroutine()
    {
        yield return new WaitForEndOfFrame(); // wait for animator initialization in Start() function
        _animator.SetTrigger(FadeInFromZeroTrigger);
        demandedState = State.Visible;
        _currentState = State.Visible;
    }
}
