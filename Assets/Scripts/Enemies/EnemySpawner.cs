using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] Enemy _enemyPrefab;

    [SerializeField] private int _startSpawnCount;

    [SerializeField] private int _perFrameSpawnCount;

    [SerializeField] private int _timedIntervalSpawnCount;
    [SerializeField] private int _spawnTime;

    [SerializeField] private float _spawnRadius;


    [SerializeField] private EnemyPool _enemyPool;

    float spawnTimer = 0;

    private void Start()
    {
        for (int i = 0; i < _startSpawnCount; i++)
        {
            SpawnEnemy();
        }

        StartCoroutine(SpawnWithInterval(_spawnTime));
    }

    private void Update()
    {
        for (int i = 0; i < _perFrameSpawnCount; i++)
        {
            SpawnEnemy();
        }
    }

    private Enemy GetEnemy()
    {
        return _enemyPool.pool.Get();
    }

    private IEnumerator SpawnWithInterval(int interval)
    {
        for (int i = 0; i < _timedIntervalSpawnCount; i++)
        {
            SpawnEnemy();
        }

        yield return new WaitForSeconds(interval);

        StartCoroutine(SpawnWithInterval(interval));
    }

    private Vector3 GetRandomSpawnPoint()
    {
        float x = Random.Range(-1f, 1f);
        float z = Random.Range(-1f, 1f);
        Vector3 randomPoint = new(x, 0, z);
        randomPoint.Normalize();
        return randomPoint * _spawnRadius;
    }

    void SpawnEnemy() {
        var spawnPoint = GetRandomSpawnPoint();
        var aimDir = spawnPoint.normalized * -1;

        var enemy = GetEnemy();
        enemy.transform.position = spawnPoint;
        enemy.Init(aimDir);
    }
}
