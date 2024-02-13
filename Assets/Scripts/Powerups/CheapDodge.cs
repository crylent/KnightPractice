using Player;
using UnityEngine;

namespace PowerUps
{
    public class CheapDodge: PowerUp
    {
        [SerializeField] private float dodgeManaConsumptionFactor = 0.5f;
        
        public override void ApplyEffect()
        {
            PlayerComponents.Controller.Modifiers.DodgeManaConsumption *= dodgeManaConsumptionFactor;
        }
    }
}