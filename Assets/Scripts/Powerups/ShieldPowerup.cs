using Player;
using UnityEngine;

namespace PowerUps
{
    public class ShieldPowerUp: PowerUp
    {
        [SerializeField] private float recoveryTimeFactor = 0.75f;
        
        public override void ApplyEffect()
        {
            PlayerComponents.Controller.Modifiers.ShieldStabilityRecoverTime *= recoveryTimeFactor;
        }
    }
}