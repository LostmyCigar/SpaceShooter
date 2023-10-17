using UnityEngine;


namespace Player
{
    public class PlayerShooting : Player.PlayerComponent
    {

        public PlayerShooting(Player player, PlayerStats stats, BulletPool bulletPool)
        {
            this.player = player;
            this.stats = stats;
            this.bulletPool = bulletPool;
        }


        private BulletPool bulletPool;
        private PlayerStats stats;
        private Player player;

        private float FireRateTimer;

        public void GetShootInput(Vector3 mousePositionInput)
        {
            var playerShootPoint = new Vector3(player.transform.position.x, 0, player.transform.position.z);
            var aimDir = (mousePositionInput - playerShootPoint).normalized;

            TryShoot(aimDir);
        }

        private void TryShoot(Vector3 aimDir)
        {
            if (FireRateTimer > 0)
                return;

            Shoot(aimDir);
        }

        private Bullet GetBullet()
        {
            return bulletPool.pool.Get();
        }

        //Optimize this with DOTS + Pooling
        private void Shoot(Vector3 aimDir)
        {
            for (int i = 0; i < 3; i++)
            {
                var finalAimDir = Quaternion.AngleAxis(stats.BulletSpread[i], Vector3.up) * aimDir;

                var bullet = GetBullet();
                bullet.Init(finalAimDir, player.transform.position);
            }

            FireRateTimer = stats.FireRate;
        }

        public override void StartComponent()
        {
            
        }

        public override void UpdateComponent()
        {
            FireRateTimer -= FireRateTimer <= 0 ? 0 : Time.deltaTime;
        }

        public override void FixedUpdateComponent()
        {
        }
    }

}
