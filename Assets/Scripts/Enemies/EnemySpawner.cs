using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] GameObject _player;
    [SerializeField] Enemy _enemyPrefab;
    [SerializeField] List<Transform> _spawnPoints;

    [SerializeField] int EnemySpawnCounter;

    float spawnTimer = 0;

    static float worldSpaceScreenBounds;

    private void Start()
    {
        var cam = Camera.main;
        worldSpaceScreenBounds = Camera.main.orthographicSize;
    }

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

        float x = Random.Range(-1f, 0);
        float y = Random.Range(-1f, 0);
        Vector3 randomPoint = new(x, y);

        randomPoint.z = 10f;
        Vector3 worldPoint = Camera.main.ViewportToWorldPoint(randomPoint);


        for (int i = 0; i < _spawnPoints.Count; i++)
        {
            var enemy = Instantiate<Enemy>(_enemyPrefab, worldPoint, Quaternion.identity);

    


          //  enemy.Init(_player.transform);
            EnemySpawnCounter++;
        }
    }
}
