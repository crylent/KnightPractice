using UnityEngine;

namespace Player
{
    public static class PlayerComponents
    {
        internal static bool IsInitialized { get; private set; }
        internal static GameObject Object { get; private set; }
        internal static Transform Transform { get; private set; }
        internal static Collider Collider { get; private set; }
        internal static PlayerController Controller { get; private set; }
        internal static Camera MainCamera { get; private set; }
    
    
        public static void Init(GameObject player)
        {
            IsInitialized = true;
            Object = player.gameObject;
            Transform = player.transform;
            Collider = player.GetComponent<Collider>();
            Controller = player.GetComponent<PlayerController>();
            var cam = GameObject.FindWithTag("MainCamera");
            MainCamera = cam.GetComponent<Camera>();
        }
    }
}
