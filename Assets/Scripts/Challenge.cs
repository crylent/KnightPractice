using System;
using System.Collections.Generic;
using Enemies;
using UnityEngine;

public class Challenge : MonoBehaviour
{
    [Serializable]
    public class Wave
    {
        public List<Enemy> enemies;
    }
    
    [SerializeField] private List<Wave> waves;
    public List<Wave> Waves => waves;

    public int WavesCount => waves.Count;
}
