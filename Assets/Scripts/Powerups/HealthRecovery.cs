using Player;
using UnityEngine;

namespace PowerUps
{
    public class HealthRecovery: PowerUp
    {
        [SerializeField] private int health = 2;
        
        public override void ApplyEffect()
        {
            PlayerComponents.Controller.RecoverHealth(health);
        }
    }
}