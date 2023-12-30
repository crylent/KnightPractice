using Player;
using UnityEngine;

namespace VFX
{
    public class ManaParticle : MonoBehaviour
    {
        public float manaValue = 1f;
        [SerializeField] private float speed = 0.5f;
        
        private static Vector3 PlayerPosition => PlayerComponents.Transform.position;

        private void Update()
        {
            var delta = PlayerPosition - transform.position;
            delta.y = 0;
            transform.Translate(speed * Time.deltaTime * delta);
        }
    }
}
