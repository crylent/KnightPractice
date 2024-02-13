using Player;
using UnityEngine;

namespace PowerUps
{
    public class Evasions: PowerUp
    {
        [SerializeField] private float evasionChance = 0.2f;
        
        public override void ApplyEffect()
        {
            PlayerComponents.Controller.Modifiers.EvasionChance += evasionChance;
        }
    }
}