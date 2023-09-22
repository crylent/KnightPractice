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
        [SerializeField] private float consumptionAnimationSpeed = 0.001f;

        private static float Mana => PlayerComponents.Controller.Mana;
        private static float MaxMana => PlayerComponents.Controller.MaxMana;

        public void OnManaChanged()
        {
            StartCoroutine(AnimateManaChange());
        }

        private IEnumerator AnimateManaChange()
        {
            var newFillAmount = ManaToFillAmount(Mana);
            filled.fillAmount = newFillAmount;
            while (consumed.fillAmount > newFillAmount)
            {
                yield return new WaitForFixedUpdate();
                consumed.fillAmount -= consumptionAnimationSpeed;
            }
        }

        private float ManaToFillAmount(float mana)
        {
            var quotient = mana / MaxMana;
            return minFillAmount + (maxFillAmount - minFillAmount) * quotient;
        }
    }
}
