using Player;

namespace UI
{
    public class ManaBar : AnimatedFillBar
    {
        protected override float GetMaxValue() => PlayerComponents.Controller.MaxMana;
    }
}
