using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI
{
    [RequireComponent(typeof(Button))]
    public class PowerUpOption : UIBehaviour
    {
        public TextMeshProUGUI title;
        public TextMeshProUGUI description;
        [SerializeField] private AudioClip clickSound;

        private Button _button;

        protected override void Start() 
        {
            base.Start();
            _button = GetComponent<Button>();
            var soundController = FindObjectOfType<UISoundController>();
            AddListener(() => soundController.PlaySound(clickSound));
        }

        public void AddListener(UnityAction action)
        {
            _button.onClick.AddListener(action);
        }
    }
}
