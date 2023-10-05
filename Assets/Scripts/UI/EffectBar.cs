using UnityEngine;

namespace UI
{
    public abstract class EffectBar: FillBar
    {
        private Animator _animator;
        private static readonly int FillAmountFloat = Animator.StringToHash("fillAmount");

        private void Start()
        {
            _animator = GetComponent<Animator>();
        }

        public override void OnValueChanged(float value)
        {
            base.OnValueChanged(value);
            _animator.SetFloat(FillAmountFloat, NewFillAmount);
        }
    }
}