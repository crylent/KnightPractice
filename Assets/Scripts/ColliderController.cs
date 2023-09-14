using System.Collections.Generic;
using Enemies;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ColliderController : MonoBehaviour
{
    public HashSet<Enemy> Enemies { get; } = new();

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Enemies.Add(other.GetComponent<Enemy>());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Enemies.Remove(other.GetComponent<Enemy>());
        }
    }
}
