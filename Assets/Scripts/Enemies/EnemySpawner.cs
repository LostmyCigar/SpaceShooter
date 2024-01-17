using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class EnemySpawner : GenericSingleton<EnemySpawner>
{
    [SerializeField] Enemy _enemyPrefab;

    [SerializeField] private bool _shouldRunEnemies = true;

    [SerializeField] private int _SpawnCount;

    [SerializeField] private float _spawnRadius;
    [SerializeField] private EnemyPool _enemyPool;

    private List<Enemy> _enemies = new List<Enemy>();

    private Camera _camera;

    private void Awake()
    {
        _camera = Camera.main;
    }


    #region Handle Enemies

    private void Update()
    {
        if (!_shouldRunEnemies) return;

        foreach (Enemy enemy in _enemies)
        {
            enemy.UpdateEnemy();
        }
    }

    public void RemoveEnemy(Enemy enemy)
    {
        _enemies.Remove(enemy);
    }

    #endregion


    #region Spawn
    private Enemy GetEnemy() => _enemyPool.pool.Get();

    public void SpawnEnemies()
    {
        for (int i = 0; i < _SpawnCount; i++)
        {
            SpawnEnemy();
        }
    }

    void SpawnEnemy()
    {
        var enemy = GetEnemy();

        enemy.transform.position = GetRandomSpawnPoint();
        enemy.transform.localScale = transform.localScale * Random.Range(0.5f, 1.2f);

        _enemies.Add(enemy);
        enemy.SetSpawner(this);
    }

    private Vector3 GetRandomSpawnPoint()
    {
        var minPoint = _camera.ScreenToWorldPoint(new(0, 0));
        var maxPoint = _camera.ScreenToWorldPoint(new(Screen.width, Screen.height));

        float x = Random.Range(minPoint.x, maxPoint.x);
        float z = Random.Range(minPoint.z, maxPoint.z);

        Vector3 randomPoint = new(x, 0, z);
        return randomPoint; ;
    }

    #endregion
}
