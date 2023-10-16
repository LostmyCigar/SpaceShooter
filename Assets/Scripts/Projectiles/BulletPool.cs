using Player;
using UnityEngine;
using UnityEngine.Pool;

public class BulletPool : MonoBehaviour
{
    [SerializeField] private Bullet bulletPrefab;
    [SerializeField] private PlayerStats bulletStats;
    public ObjectPool<Bullet> pool;

    [SerializeField] private int ActiveBullets;
    [SerializeField] private int InActiveBullets;


    private void Awake()
    {
        DontDestroyOnLoad(transform.root);

        pool = new ObjectPool<Bullet>(CreateBullet, OnTakeFromPool, OnReturnToPool);
    }

    private void Update()
    {
        ActiveBullets = pool.CountActive;
        InActiveBullets = pool.CountInactive;
    }
    private Bullet CreateBullet()
    {
        var bullet = Instantiate(bulletPrefab);
        bullet.SetPool(pool);
        bullet.SetStats(bulletStats);
        return bullet;
    }

    private void OnTakeFromPool(Bullet bullet)
    {
        bullet.gameObject.SetActive(true);
    }

    private void OnReturnToPool(Bullet bullet)
    {
        bullet.gameObject.SetActive(false);
    }
}