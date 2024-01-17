using UnityEngine;
using Player;
using UnityEngine.Pool;
using UnityEngine.UIElements;
using Unity.VisualScripting;
using Unity.Jobs;
using System.Threading;

public class Enemy : MonoBehaviour, IDamagable
{
    [SerializeField] private int _hp;

    private EnemySpawner spawner;

    private Camera _camera;
    private IObjectPool<Enemy> _pool;

    public void SetPool(IObjectPool<Enemy> pool) => this._pool = pool;

    private void OnEnable()
    {
        Profiling.EnemyCounter++;
    }
    private void OnDisable()
    {
        spawner?.RemoveEnemy(this);
        Profiling.EnemyCounter--;
    }

    public void SetSpawner(EnemySpawner spawner)
    {
        this.spawner = spawner; 
    }

    public void UpdateEnemy()
    {
        Thread.Sleep(1);
    }



    public void TakeDamage(int damage)
    {
        _hp -= damage;

        if (_hp < 0)
        {
            Remove();
        }
    }
    public void Remove()
    {
        _pool.Release(this);
    }
}
