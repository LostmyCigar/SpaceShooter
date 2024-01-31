using UnityEngine;
using Player;
using UnityEngine.Pool;
using Unity.Mathematics;
using UnityEngine.UIElements;
using Unity.VisualScripting;
using Unity.Jobs;
using System.Threading;
using Unity.Burst;

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
        var handle = TheSleeping();

        //Check if we need this to be in lateupdate later
        handle.Complete();
    }
    public JobHandle TheSleeping()
    {
        var pieJob = new SleepJob();
        return pieJob.Schedule();
    }

    public void CalculatePiJOB()
    {
        var handle = ThePiStuff();
        handle.Complete();
    }

    public JobHandle ThePiStuff()
    {
        var pieJob = new PieJob();
        return pieJob.Schedule();
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


[BurstCompile]
public struct PieJob : IJob //could be private
{
    public void Execute()
    {
        float pi = 0.0f;
        for (int i = 0; i < 10000; i++)
        {
            pi += 4.0f * math.pow(-1, i) / (2 * i + 1);
        }
    }
}


[BurstCompile]
public struct SleepJob : IJob
{
    public void Execute()
    {
        Thread.Sleep(1);
    }
}