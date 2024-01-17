using Unity.Jobs;
using UnityEngine;
using Unity.Collections;
using Unity.Burst;

[BurstCompile]
public struct EnemyMovementJob : IJob
{
    private Vector3 _position;
    private Vector3 _velocity;
    private float _deltaTime;

    private NativeArray<Vector3> _positionResult;

    public EnemyMovementJob(float speed, Vector3 direction, Vector3 position, float deltaTime, NativeArray<Vector3> positionResult)
    {
        _velocity = direction * speed;
        _position = position;
        _deltaTime = deltaTime;
        _positionResult = positionResult;
    }

    public void Move()
    {
        _position += _velocity * _deltaTime;
        _positionResult[0] = _position;
    }

    public void Execute()
    {
        Move();
    }
}
