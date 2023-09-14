using Player;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Enemies
{
    public class SampleEnemy : Enemy
    {
        [SerializeField] private float movementTime = 3;
        [SerializeField] private float maxSpeed = 50;
        [SerializeField] private float attackRange = 5;
        
        // Start is called before the first frame update
        private new void Start()
        {
            base.Start();
            InvokeRepeating(nameof(decideOnDirection), 0, movementTime);
        }

        protected override void BehaviorUpdate()
        {
            var posDiff = PlayerComponents.Transform.position - gameObject.transform.position; // positions difference
            posDiff.y = 0; // don't consider the height
            Movement = posDiff.normalized * maxSpeed; // chase player

            if (posDiff.magnitude <= attackRange)
            {
                Attack();
            }
        }

        private void decideOnDirection()
        {
            if (Movement != Vector3.zero) // need rest
            {
                
                Movement = Vector3.zero;
            }
            else
            {
                var speed = Random.Range(0, maxSpeed);
                var angle = Random.Range(-180f, 180f);
                Movement = Quaternion.Euler(0, angle, 0) * Vector3.forward * speed;
            }
        }
    }
}
