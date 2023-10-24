using Unity.Jobs;
using UnityEngine;
using Unity.Collections;
using Unity.Burst;

[BurstCompile]
public struct EnemyOnScreenJob : IJob
{
    private Vector2 _screenPoint;
    private Vector3 _direction;
    private int _screenWidth;
    private int _screenHeight;

    private NativeArray<bool> _onScreenResult;

    public EnemyOnScreenJob(Vector3 screenPoint, int screenWidth, int screenHeight, Vector3 direction, NativeArray<bool> onScreenResult)
    {
        _screenPoint = screenPoint;
        _screenWidth = screenWidth; 
        _screenHeight = screenHeight;
        _direction = direction;
        _onScreenResult = onScreenResult;
    }

    public void Execute()
    {

        if (_screenPoint.x > _screenWidth && _direction.x > 0 || _screenPoint.x < 0 && _direction.x < 0)
        {
            _onScreenResult[0] = false;
            return;
        }
            
        if (_screenPoint.y > _screenHeight && _direction.z > 0 || _screenPoint.y < 0 && _direction.z < 0)
        {
            _onScreenResult[0] = false;
            return;
        }

        _onScreenResult[0] = true;
    }
}
