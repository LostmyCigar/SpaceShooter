using UnityEngine;
using Player;

public class Enemy : MonoBehaviour, IDamagable
{
    [SerializeField] private int _hp;
    [SerializeField] private float _moveSpeed;
    [SerializeField] private Transform _transform;

    private Vector3 _Velocity;

    public void Init(Vector3 dir, float speed)
    {
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
