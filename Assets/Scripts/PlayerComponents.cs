using UnityEngine;

public static class PlayerComponents
{
    internal static GameObject Player { get; private set; }
    internal static Transform Transform { get; private set; }
    internal static Collider Collider { get; private set; }
    internal static PlayerController Controller { get; private set; }
    internal static Camera MainCamera { get; private set; }
    
    
    public static void Init(GameObject player)
    {
        Player = player;
        Transform = player.transform;
        Collider = player.GetComponent<Collider>();
        Controller = player.GetComponent<PlayerController>();
        var cam = GameObject.FindWithTag("MainCamera");
        MainCamera = cam.GetComponent<Camera>();
    }
}
