using UnityEngine;
using Player;

public class Enemy : MonoBehaviour, IDamagable
{
    [SerializeField] private int _hp;
    [SerializeField] private float _moveSpeed;
    [SerializeField] private Transform _transform;

    private Transform _playerTransform;
    private Vector3 _Velocity;

    public void Init(Transform playerTranform)
    {
        _playerTransform = playerTranform;
    }

    private void FixedUpdate()
    {
        _transform.position += _Velocity;
    }

    void Update()
    {
        var DirToPlayer = (_playerTransform.position - _transform.position).normalized;

        _Velocity = DirToPlayer * _moveSpeed * 0.01f;
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
