using Unity.Jobs;
using Unity.Collections;
using UnityEngine;
using System;

public class EnemyJobHandler
{
    public float _speed;
    public Vector3 _direction;
    private Transform _transform;
    private Camera _camera;
    private int _screenWidth;
    private int _screenHeight;

    private JobHandle _moveJobHandle;
    private NativeArray<Vector3> _positionResult;

    private JobHandle _onScreenJobHandle;
    private NativeArray<bool> _onScreenResult;

    public event Action NotOnScreen;

    public EnemyJobHandler(Transform transform, Camera camera, int screenWidth, int screenHeight)
    {
        _camera = camera;  
        _transform = transform;
        _screenWidth = screenWidth;
        _screenHeight = screenHeight;
        _positionResult = new NativeArray<Vector3>(1, Allocator.Persistent);
        _onScreenResult = new NativeArray<bool>(1, Allocator.Persistent);
    }

    public void SetMovementAndPosition(float speed, Vector3 direction, Vector3 position)
    {
        _transform.position = position;
        _speed = speed;
        _direction = direction;
    }

    public void Update()
    {
        var position = _transform.position;
        var screenPoint = _camera.WorldToScreenPoint(_transform.position);

        EnemyMovementJob moveJob = new EnemyMovementJob(_speed, _direction, position, Time.deltaTime, _positionResult);
        EnemyOnScreenJob onScreenJob = new EnemyOnScreenJob(screenPoint, _screenWidth, _screenHeight, _direction, _onScreenResult);

        _moveJobHandle = moveJob.Schedule();
        _onScreenJobHandle = onScreenJob.Schedule();

        _moveJobHandle.Complete();
        _onScreenJobHandle.Complete();

        _transform.position = _positionResult[0];

        if (!_onScreenResult[0])
            NotOnScreen.Invoke();
    }

    public void LateUpdate()
    {

    }

    public void OnDestroy()
    {
        _positionResult.Dispose();
        _onScreenResult.Dispose();
    }
}
