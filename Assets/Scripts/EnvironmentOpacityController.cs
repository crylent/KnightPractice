using UnityEngine;

public class EnvironmentOpacityController : OpacityController
{
    private Collider[] _colliders;

    private void Start()
    {
        _colliders = GetComponents<Collider>();
    }
    
    private new void Update()
    {
        var ray = PlayerComponents.MainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        var hideObject = false;
        foreach (var c in _colliders)
        {
            hideObject = c.Raycast(ray, out _, float.PositiveInfinity);
            if (hideObject) break;
        }

        demandedState = hideObject ? State.Hidden : State.Visible;

        base.Update();
    }
}
