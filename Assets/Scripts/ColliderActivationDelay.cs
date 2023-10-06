using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ColliderActivationDelay: MonoBehaviour
{
    [SerializeField] private float delay = 1f;
    private Collider _collider;
    
    private void Start()
    {
        _collider = GetComponent<Collider>();
        _collider.enabled = false;
        StartCoroutine(EnableCollider());
    }

    private IEnumerator EnableCollider()
    {
        yield return new WaitForSeconds(delay);
        _collider.enabled = true;
    }
}