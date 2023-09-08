using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class GlobalSpawner : MonoBehaviour
{
    [SerializeField] private Rect worldSize;
    [SerializeField] private Vector2Int chunks;
    [SerializeField] private GameObject[] enemies;
    [SerializeField] private int enemiesInChunkRequired;
    [SerializeField] private float worldUpdateRate;

    private readonly List<GameObject> _spawnedEnemies = new();
    
    private void Start()
    {
        InvokeRepeating(nameof(WorldUpdate), 0, worldUpdateRate);
    }

    private void WorldUpdate()
    {
        var playerChunk = GetPlayerChunk();

        // clear the list of no longer existing enemies
        _spawnedEnemies.RemoveAll(enemy => enemy.IsDestroyed());

        // initialize counter array
        var enemiesInChunkCounter = new int[chunks.y][];
        for (var i = 0; i < enemiesInChunkCounter.Length; i++)
        {
            enemiesInChunkCounter[i] = new int[chunks.x];
        }

        // for all existing enemies
        foreach (var enemy in _spawnedEnemies)
        {
            var chunk = GetChunk(enemy.transform.position);
            if (Math.Abs(chunk.x - playerChunk.x) > 1 || Math.Abs(chunk.y - playerChunk.y) > 1)
            {
                Destroy(enemy.gameObject); // clear enemies in remote chunks
            }
            else
            {
                enemiesInChunkCounter[chunk.y][chunk.x] += 1; // count enemies in the chunk
            }
        }

        // for all neighbour chunks
        for (var chunkX = Math.Max(0, playerChunk.x - 1);
             chunkX <= Math.Min(playerChunk.x + 1, chunks.x - 1);
             chunkX++)
        {
            for (var chunkY = Math.Max(0, playerChunk.y - 1);
                 chunkY <= Math.Min(playerChunk.y + 1, chunks.y - 1);
                 chunkY++)
            {
                if (chunkX == playerChunk.x && chunkY == playerChunk.y) continue; // skip player chunk
                
                for (var i = 0; i < enemiesInChunkRequired - enemiesInChunkCounter[chunkY][chunkX]; i++)
                {
                    SpawnEnemy(chunkX, chunkY);
                }
            }
        }
    }

    private Vector2Int GetPlayerChunk()
    {
        return GetChunk(PlayerComponents.Transform.position);
    }

    private Vector2Int GetChunk(Vector3 pos)
    {
        return new Vector2Int(
            Math.Min((int) Math.Floor((pos.x - worldSize.xMin) / worldSize.width * chunks.x), chunks.x - 1),
            Math.Min((int) Math.Floor((pos.z - worldSize.yMin) / worldSize.height * chunks.y), chunks.y - 1)
        );
    }

    private void SpawnEnemy(int chunkX, int chunkY)
    {
        var location = new Vector3(
             worldSize.xMin + worldSize.width * chunkX / chunks.x + Random.Range(0, worldSize.width / chunks.x),
             0,
             worldSize.yMin + worldSize.height * chunkY / chunks.y + Random.Range(0, worldSize.height / chunks.y)
        );
        var enemyIndex = Random.Range(0, enemies.Length);
        var enemyToSpawn = enemies[enemyIndex];
        _spawnedEnemies.Add(
            Instantiate(enemyToSpawn, location + enemyToSpawn.transform.localPosition, enemyToSpawn.transform.rotation)
            );
    }
}
