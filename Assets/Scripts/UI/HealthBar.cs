using System.Collections;
using System.Collections.Generic;
using Player;
using UnityEngine;

namespace UI
{
    public class HealthBar : MonoBehaviour
    {
        [SerializeField] private GameObject life;
        
        private static int MaxHealth => PlayerComponents.Controller.MaxHealth;
        private int _lastHealth;

        private readonly List<GameObject> _lives = new();
        private static readonly int IsWastedBoolean = Animator.StringToHash("isWasted");

        private void Start()
        {
            StartCoroutine(Initialize());
        }

        private IEnumerator Initialize()
        {
            yield return new WaitUntil(() => PlayerComponents.IsInitialized);
            for (var lives = 0; lives < MaxHealth; lives++)
            {
                _lives.Add(Instantiate(life, transform));
            }
            _lastHealth = MaxHealth;
        }

        public void OnHealthChanged(int health)
        {
            for (var i = _lastHealth; i > health; i--)
            {
                AnimateHealth(i - 1, true);
            }

            for (var i = _lastHealth; i < health; i++)
            {
                AnimateHealth(i, false);
            }
            _lastHealth = health;
        }

        private void AnimateHealth(int i, bool isWasted)
        {
            _lives[i].GetComponent<Animator>().SetBool(IsWastedBoolean, isWasted);
        }
    }
}
