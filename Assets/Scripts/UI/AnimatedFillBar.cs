using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public abstract class AnimatedFillBar: FillBar
    {
        [SerializeField] private Image consumed;
        [SerializeField] private float consumptionAnimationSpeed = 0.002f;
        [SerializeField] private float recoveryAnimationSpeed = 0.006f;

        private void FixedUpdate()
        {
            if (consumed.fillAmount > NewFillAmount)
            {
                consumed.fillAmount -= consumptionAnimationSpeed;
            }
            else if (filled.fillAmount < NewFillAmount)
            {
                filled.fillAmount += recoveryAnimationSpeed;
            }
        }
    }
}