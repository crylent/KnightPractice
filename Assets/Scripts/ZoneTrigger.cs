using UnityEngine;

public class ZoneTrigger: MonoBehaviour
{
    [SerializeField] private Rect zoneBounds;
    public Rect ZoneBounds => zoneBounds;
    [SerializeField] private Material open;
    [SerializeField] private Material closed;

    private Collider _collider;
    private Renderer _renderer;

    private void Start()
    {
        _collider = GetComponent<Collider>();
        _renderer = GetComponent<MeshRenderer>();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player") || other.transform.position.x < transform.position.x) return;
        Close();
    }

    public void Open()
    {
        _collider.isTrigger = true;
        _renderer.material = open;
    }

    private void Close()
    {
        _collider.isTrigger = false;
        _renderer.material = closed;
    }
}