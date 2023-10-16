using System.Collections.Generic;
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
        public int BulletDamage;
        public float BulletDestroyTime;
        public List<int> BulletSpread;

        public float BulletSpeedReduction;

        public GameObject BulletPrefab;
    }
}
