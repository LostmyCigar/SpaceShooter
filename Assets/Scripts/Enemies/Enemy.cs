using UnityEngine;
using Player;
using UnityEngine.Pool;
using Unity.Mathematics;
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

    public void SleepyThread()
    {
        Thread.Sleep(1);
    }

    public void CalculateCirclesOrSomething()
    {
        Profiling.CalculatePi(10000);
    }


    #region JOBS
    public void SleepOnTheJob()
    {

    }
    public void DoneSleepingOnTheJob()
    {

    }

    public void CalculatePieJOB()
    {

    }

    public void CompleteCalculatePieJOB()
    {

    }

    #endregion


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
        spawner.RemoveEnemy(this);
        _pool.Release(this);
    }
}
