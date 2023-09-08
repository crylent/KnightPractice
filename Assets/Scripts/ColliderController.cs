using System.Collections.Generic;
using Enemies;
using UnityEngine;

public class ColliderController : MonoBehaviour
{
    //public HashSet<Collider> Colliders { get; } = new();
    public HashSet<Enemy> Enemies { get; } = new();

    private void OnTriggerEnter(Collider other)
    {
        //Colliders.Add(other);
        if (other.CompareTag("Enemy"))
        {
            Enemies.Add(other.GetComponent<Enemy>());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        //Colliders.Remove(other);
        if (other.CompareTag("Enemy"))
        {
            Enemies.Remove(other.GetComponent<Enemy>());
        }
    }
}
