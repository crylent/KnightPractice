using Player;

namespace PowerUps
{
    public class EliteDodge: PowerUp
    {
        public override void ApplyEffect()
        {
            PlayerComponents.Controller.Modifiers.InvulnerableWhenDodging = true;
        }
    }
}