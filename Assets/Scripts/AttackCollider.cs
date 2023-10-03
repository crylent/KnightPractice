using System.Collections.Generic;
using Enemies;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class AttackCollider : MonoBehaviour
{
    [SerializeField] private string attackName;
    public string AttackName => attackName;
    
    [SerializeField] private int damage = 1;
    public int Damage => damage;
    
    public bool PlayerIsInside { get; private set; }
    public HashSet<Enemy> Enemies { get; } = new();

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Enemies.Add(other.GetComponent<Enemy>());
        }
        else if (other.CompareTag("Player"))
        {
            PlayerIsInside = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Enemies.Remove(other.GetComponent<Enemy>());
        }
        else if (other.CompareTag("Player"))
        {
            PlayerIsInside = false;
        }
    }
}
