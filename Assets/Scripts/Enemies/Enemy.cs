using UnityEngine;
using Player;
using UnityEngine.Pool;
using UnityEngine.UIElements;
using Unity.VisualScripting;
using Unity.Jobs;

public class Enemy : MonoBehaviour, IDamagable
{
    [SerializeField] private int _hp;

    [SerializeField] private float _minSpeed;
    [SerializeField] private float _maxSpeed;

    [SerializeField] private float _minSize;
    [SerializeField] private float _maxSize;

    [SerializeField] private Transform _transform;

    private EnemyJobHandler _jobHandler;

    private Camera _camera;
    private IObjectPool<Enemy> _pool;

    public void SetPool(IObjectPool<Enemy> pool) => this._pool = pool;


    private void Awake()
    {
        _camera = Camera.main;
        _jobHandler = new EnemyJobHandler(_transform, _camera, Screen.width, Screen.height);

        _transform.localScale = transform.localScale * Random.Range(_minSize, _maxSize);
    }
    private void OnEnable()
    {
        _jobHandler.NotOnScreen += Remove;
        Profiling.EnemyCounter++;
    }
    private void OnDisable()
    {
        _jobHandler.NotOnScreen -= Remove;
        Profiling.EnemyCounter--;
    }

    private void Update()
    {
        _jobHandler.Update();
    }

    public void LateUpdate()
    {
        _jobHandler.LateUpdate();
    }

    private void OnDestroy()
    {
        _jobHandler.OnDestroy();
    }

    public void StartEnemy(Vector3 aimDir, Vector3 startPos)
    {
        var speed = Random.Range(_minSpeed, _maxSpeed);

        _jobHandler.SetMovementAndPosition(speed, aimDir, startPos);
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

    private void OnTriggerEnter(Collider other)
    {
        var player = other.GetComponent<Player.Player>();
            
        if (player != null)
        {
            player.TakeDamage(1);
            Remove();
        }
    }
}
