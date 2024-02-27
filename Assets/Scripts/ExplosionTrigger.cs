using Enemies;
using UnityEngine;

public class ExplosionTrigger : MonoBehaviour
{
    [SerializeField] private CatDemon owner;

    private bool _tryAgain;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) _tryAgain = true;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && _tryAgain)
        {
            _tryAgain = !owner.CallExplosion();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) _tryAgain = false;
    }
}
