using UnityEngine;

namespace Player
{
    [CreateAssetMenu(fileName = "PlayerStats")]
    public class PlayerStats : ScriptableObject
    {
        public float MoveSpeed;


        [Header("Shooting")]


        public float FireRate;
        public float BulletSpeed;
        public float BulletDamage;

        public GameObject BulletPrefab;
    }
}
