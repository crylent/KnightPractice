using Player;
using UnityEngine;

namespace PowerUps
{
    public class Vampirism: PowerUp
    {
        [SerializeField] private float vampirismChance = 0.05f;
        
        public override void ApplyEffect()
        {
            PlayerComponents.Controller.Modifiers.VampirismChance += vampirismChance;
        }
    }
}