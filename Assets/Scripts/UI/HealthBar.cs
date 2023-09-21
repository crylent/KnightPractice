using System;
using System.Collections;
using System.Collections.Generic;
using Player;
using UnityEngine;

namespace UI
{
    public class HealthBar : MonoBehaviour
    {
        [SerializeField] private GameObject life;
        
        private static int Health => PlayerComponents.Controller.Health;
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

        /*private void ClearBar()
        {
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }
        }*/

        public void OnHealthChanged()
        {
            var deltaSign = Math.Sign(Health - _lastHealth);
            for (var i = _lastHealth - 1; i != Health + deltaSign; i += deltaSign)
            {
                _lives[i].GetComponent<Animator>().SetBool(IsWastedBoolean,deltaSign < 0);
            }
            _lastHealth = Health;
        }
    }
}
