using Player;
using UnityEngine;

namespace PowerUps
{
    public class CheapMagic: PowerUp
    {
        [SerializeField] private float castManaConsumptionFactor = 0.75f;
        
        public override void ApplyEffect()
        {
            PlayerComponents.Controller.Modifiers.CastManaConsumption *= castManaConsumptionFactor;
        }
    }
}