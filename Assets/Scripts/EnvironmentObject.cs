using UnityEngine;

public class EnvironmentObject : MonoBehaviour
{
    [SerializeField] private float opacity = 1.0f;
    private SpriteRenderer[] _sprites;
    private Collider[] _colliders;
    private Animator _animator;
    private bool _isHidden;
    
    private static readonly int FadeOut = Animator.StringToHash("fadeOut");
    private static readonly int FadeIn = Animator.StringToHash("fadeIn");

    private void Start()
    {
        _sprites = GetComponentsInChildren<SpriteRenderer>();
        _colliders = GetComponents<Collider>();
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        var ray = PlayerComponents.MainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        var hideObject = false;
        foreach (var c in _colliders)
        {
            hideObject = c.Raycast(ray, out var hit, float.PositiveInfinity);
            if (hideObject) break;
        }

        switch (_isHidden)
        {
            case false when hideObject: // visible, make it hidden
                _animator.SetTrigger(FadeOut);
                break;
            case true when !hideObject: // hidden, make it visible
                _animator.SetTrigger(FadeIn);
                break;
        }

        foreach (var sprite in _sprites)
        {
            var color = sprite.color;
            color.a = opacity;
            sprite.color = color;
        }
        _isHidden = hideObject;
    }
}
