using Player;
using UnityEngine;

namespace PowerUps
{
    public class ManaRecoveryBoost: PowerUp
    {
        [SerializeField] private float manaRecoveryFactor = 1.25f;
        
        public override void ApplyEffect()
        {
            PlayerComponents.Controller.Modifiers.ManaRecovery *= manaRecoveryFactor;
        }
    }
}