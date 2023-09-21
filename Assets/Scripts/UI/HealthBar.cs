using Player;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class HealthBar : MonoBehaviour
    {
        [SerializeField] private Image life;
        [SerializeField] private Image noLife;
        
        private static int Health => PlayerComponents.Controller.Health;
        private static int MaxHealth => PlayerComponents.Controller.MaxHealth;

        private void ClearBar()
        {
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }
        }

        public void OnHealthChanged()
        {
            ClearBar();
            Debug.Log(Health);
            for (var lives = 0; lives < Health; lives++)
            {
                Instantiate(life, transform);
            }

            for (var wastedLives = 0; wastedLives < MaxHealth - Health; wastedLives++)
            {
                Instantiate(noLife, transform);
            }
        }
    }
}
