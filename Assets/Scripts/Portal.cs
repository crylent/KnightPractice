using System.Collections;
using Enemies;
using UnityEngine;

public class Portal : GameEntity
{
    public Enemy enemyToSpawn;
    
    [SerializeField] private float spawnTime = 2.5f;
    [SerializeField] private float existenceTime = 5f;
    
    private ParticleSystem _particleSystem;
    public Enemy SpawnedEnemy { get; private set; }
    
    // Start is called before the first frame update
    private void Start()
    {
        _particleSystem = GetComponent<ParticleSystem>();
        StartCoroutine(SpawnEnemy());
        StartCoroutine(DestroyPortal());
    }

    private IEnumerator SpawnEnemy()
    {
        yield return new WaitForSeconds(spawnTime);
        var prefabTransform = enemyToSpawn.transform;
        SpawnedEnemy = Instantiate(enemyToSpawn, gameObject.transform.position + prefabTransform.position, prefabTransform.rotation);
    }

    private IEnumerator DestroyPortal()
    {
        yield return new WaitForSeconds(existenceTime);
        _particleSystem.Stop(true);
        
        yield return new WaitUntil(() => _particleSystem.isStopped);
        Destroy(gameObject);
    }
}
