using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] GameObject _player;
    [SerializeField] GameObject _enemyPrefab;
    [SerializeField] List<Transform> _spawnPoints;

    [SerializeField] int EnemySpawnCounter;

    float spawnTimer = 0;

    void Update()
    {
        spawnTimer -= Time.deltaTime;

        if (spawnTimer <= 0)
        {
            SpawnEnemies();
            spawnTimer = 1f;
        }
    }

    void SpawnEnemies() {

        for (int i = 0; i < _spawnPoints.Count; i++)
        {
            var enemy = Instantiate(_enemyPrefab, _spawnPoints[i]).GetComponent<Enemy>();
            enemy.Init(_player.transform);
            EnemySpawnCounter++;
        }
    }
}
