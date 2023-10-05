using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public abstract class FillBar : MonoBehaviour
    {
        [SerializeField] protected Image filled;
        [SerializeField] private float minFillAmount;
        [SerializeField] private float maxFillAmount = 1f;

        protected float NewFillAmount;
        protected abstract float GetMaxValue();

        protected FillBar()
        {
            NewFillAmount = maxFillAmount;
        }

        public virtual void OnValueChanged(float value)
        {
            NewFillAmount = ValueToFillAmount(value);
            filled.fillAmount = NewFillAmount;
        }

        private float ValueToFillAmount(float value)
        {
            var quotient = value / GetMaxValue();
            return minFillAmount + (maxFillAmount - minFillAmount) * quotient;
        }
    }
}