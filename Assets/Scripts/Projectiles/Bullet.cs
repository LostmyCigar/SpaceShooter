using Player;
using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

public class Bullet : MonoBehaviour
{
    [SerializeField] private Transform _transform;

    PlayerStats playerStats;
    Vector3 velocity;
    private IObjectPool<Bullet> pool;

    public void SetPool(IObjectPool<Bullet> pool) => this.pool = pool;
    public void SetStats(PlayerStats stats)
    {
        playerStats = stats;
    }

    public void Init(Vector3 aimDir, Vector3 position)
    {
        _transform.position = position;
        velocity = playerStats.BulletSpeed * aimDir;
        StartCoroutine(RemoveTimer(playerStats.BulletDestroyTime));
    }

    private void Remove()
    {
        pool.Release(this);
    }

    private IEnumerator RemoveTimer(float time)
    {
        yield return new WaitForSeconds(time);
        Remove();
    }


    private void FixedUpdate()
    {
        _transform.position += velocity;
    }

    private void Update()
    {
        velocity += velocity * (playerStats.BulletSpeedReduction * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        var damageable = other.GetComponent<IDamagable>();

        if (damageable != null)
        {
            damageable.TakeDamage(playerStats.BulletDamage);
            Remove();
        }
    }
}
