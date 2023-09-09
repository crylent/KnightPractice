using System.Collections;
using System.Collections.Generic;
using Enemies;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Challenge challenge;
    [SerializeField] private Portal portalPrefab;

    private int _currentWave = -1;
    private long _enemiesAlive;
    //private readonly List<Enemy> _enemiesAlive = new();
    
    // Start is called before the first frame update
    private void Start()
    {
        StartCoroutine(LaunchChallenge());
    }

    private IEnumerator LaunchChallenge()
    {
        //yield return new WaitForEndOfFrame();

        while (_currentWave + 1 < challenge.WavesCount)
        {
            yield return new WaitWhile(() => _enemiesAlive > 0);
            CallNextWave();
        }
    }

    private void CallNextWave()
    {
        _currentWave += 1;
        var enemiesToSpawn = challenge.Waves[_currentWave].enemies;
        _enemiesAlive += enemiesToSpawn.Count;
        foreach (var enemy in enemiesToSpawn)
        {
            StartCoroutine(SpawnEnemy(enemy));
        }
    }

    private IEnumerator SpawnEnemy(Enemy enemy)
    {
        var portal = Instantiate(portalPrefab);
        portal.enemyToSpawn = enemy;
        //_enemiesAlive.Add(enemy);
        yield return new WaitUntil(() => portal.SpawnedEnemy);
        var spawnedEnemy = portal.SpawnedEnemy;

        yield return new WaitUntil(() => spawnedEnemy.IsDestroyed());
        _enemiesAlive -= 1;
    }
}
