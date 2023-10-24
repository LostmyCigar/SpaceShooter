using Player;
using UnityEngine.Pool;
using UnityEngine;

public class EnemyPool : MonoBehaviour
{
    [SerializeField] private Enemy enemyPrefab;

    public ObjectPool<Enemy> pool;

    [SerializeField] private int ActiveEnemies;
    [SerializeField] private int InactiveEnemies;


    private void Awake()
    {
        DontDestroyOnLoad(transform.root);

        pool = new ObjectPool<Enemy>(CreateEnemy, OnTakeFromPool, OnReturnToPool);

        for (int i = 0; i < 50000; i++)
        {
            var enemy = CreateEnemy();
            enemy.Remove();
        }
    }

    private void Update()
    {
        ActiveEnemies = pool.CountActive;
        InactiveEnemies = pool.CountInactive;
    }
    private Enemy CreateEnemy()
    {
        var enemy = Instantiate(enemyPrefab);
        enemy.SetPool(pool);
        return enemy;
    }

    private void OnTakeFromPool(Enemy enemy)
    {
        enemy.gameObject.SetActive(true);
    }

    private void OnReturnToPool(Enemy enemy)
    {
        enemy.gameObject.SetActive(false);
    }
}
