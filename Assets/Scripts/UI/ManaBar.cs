using System;
using System.Collections;
using Player;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class ManaBar : MonoBehaviour
    {
        [SerializeField] private Image filled;
        [SerializeField] private Image consumed;
        [SerializeField] private float minFillAmount = 0.227f;
        [SerializeField] private float maxFillAmount = 0.928f;
        [SerializeField] private float consumptionAnimationSpeed = 0.002f;
        [SerializeField] private float recoveryAnimationSpeed = 0.006f;

        private float _newFillAmount;
        private static float MaxMana => PlayerComponents.Controller.MaxMana;

        public ManaBar()
        {
            _newFillAmount = maxFillAmount;
        }


        private void FixedUpdate()
        {
            if (consumed.fillAmount > _newFillAmount)
            {
                consumed.fillAmount -= consumptionAnimationSpeed;
            }
            else if (filled.fillAmount < _newFillAmount)
            {
                filled.fillAmount += recoveryAnimationSpeed;
            }
        }

        public void OnManaChanged(float mana)
        {
            _newFillAmount = ManaToFillAmount(mana);
            filled.fillAmount = _newFillAmount;
        }

        private float ManaToFillAmount(float mana)
        {
            var quotient = mana / MaxMana;
            return minFillAmount + (maxFillAmount - minFillAmount) * quotient;
        }
    }
}
