using Player;
using UnityEngine;

namespace PowerUps
{
    public class SpeedBoost: PowerUp
    {
        [SerializeField] private float speedFactor = 1.25f;
        
        public override void ApplyEffect()
        {
            PlayerComponents.Controller.Modifiers.Speed *= speedFactor;
        }
    }
}