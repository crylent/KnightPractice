using System.Collections;
using System.Collections.Generic;
using PowerUps;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI
{
    [RequireComponent(typeof(HorizontalLayoutGroup))]
    public class OptionsLayout : UIBehaviour
    {
        [SerializeField] private PowerUpOption optionUI;
        
        public bool OptionSelected { get; private set; }
        
        public void Show(HashSet<PowerUp> options)
        {
            OptionSelected = false;
            foreach (var powerUp in options)
            {
                var option = Instantiate(optionUI, transform);
                option.title.text = powerUp.powerUpName;
                option.description.text = powerUp.powerUpDesc;
                StartCoroutine(AddListeners(option, powerUp.ApplyEffect));
            }
        }

        private IEnumerator AddListeners(PowerUpOption button, UnityAction applyEffect)
        {
            yield return new WaitForEndOfFrame(); // button is null until next frame
            button.AddListener(applyEffect);
            button.AddListener(OnOptionSelected);
        }

        private void OnOptionSelected()
        {
            OptionSelected = true;
            foreach (Transform option in transform)
            {
                Destroy(option.gameObject);
            }
        }
    }
}
