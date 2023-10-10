using Player;
using UnityEngine;

public class Bullet : MonoBehaviour
{

    float speed;
    float damage;

    Vector3 velocity;

    private void FixedUpdate()
    {
        transform.position += velocity;
    }


    public void Init(PlayerStats stats, Vector3 aimDir)
    {
        velocity = stats.BulletSpeed * aimDir;
    }


    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Hit something");
    }
}
