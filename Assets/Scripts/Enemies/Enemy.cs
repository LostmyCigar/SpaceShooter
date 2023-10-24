using UnityEngine;
using Player;

public class Enemy : MonoBehaviour, IDamagable
{
    [SerializeField] private int _hp;
    [SerializeField] private float _minSpeed;
    [SerializeField] private float _maxSpeed;

    [SerializeField] private float _minSize;
    [SerializeField] private float _maxSize;

    [SerializeField] private Transform _transform;

    private Vector3 _Velocity;

    private void OnEnable()
    {
        Profiling.EnemyCounter++;
    }
 
    private void OnDisable()
    {
        Profiling.EnemyCounter--;
    }

    public void Init(Vector3 dir)
    {
        var speed = Random.Range(_minSpeed, _maxSpeed);
        var scale = transform.localScale * Random.Range(_minSize, _maxSize);

        _transform.localScale = scale;
        _Velocity  = dir.normalized * speed;
    }

    private void FixedUpdate()
    {
        _transform.position += _Velocity;
    }

    public void TakeDamage(int damage)
    {
        _hp -= damage;

        if (_hp < 0)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        var player = other.GetComponent<Player.Player>();
            
        if (player != null)
        {
            player.TakeDamage(1);
            Destroy(gameObject);
        }
    }
}
