using Player;
using UnityEngine;

namespace PowerUps
{
    public class CriticalHits: PowerUp
    {
        [SerializeField] private float criticalChance = 0.1f;
        
        public override void ApplyEffect()
        {
            PlayerComponents.Controller.Modifiers.CriticalHitChance += criticalChance;
        }
    }
}