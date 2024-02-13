using System.Collections.Generic;
using PowerUps;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UI
{
    public class OptionsLayout : HorizontalLayoutGroup
    {
        [SerializeField] private PowerUpOption optionUI;
        
        public bool OptionSelected { get; private set; }
        
        public void Show(HashSet<PowerUp> options)
        {
            foreach (var powerUp in options)
            {
                var option = Instantiate(optionUI, transform);
                option.title.text = powerUp.powerUpName;
                option.description.text = powerUp.powerUpDesc;
                
                option.onClick.AddListener(() => powerUp.ApplyEffect());
                option.onClick.AddListener(() => OptionSelected = true);
            }
        }
    }
}
