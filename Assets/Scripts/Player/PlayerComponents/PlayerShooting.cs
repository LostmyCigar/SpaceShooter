using UnityEngine;


namespace Player
{
    public class PlayerShooting : Player.PlayerComponent
    {

        public PlayerShooting(Player player, PlayerStats stats)
        {
            this.player = player;
            this.stats = stats;
        }

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

        //Optimize this with DOTS + Pooling
        private void Shoot(Vector3 aimDir)
        {
            var bullet = Object.Instantiate(stats.BulletPrefab, player.transform.position, Quaternion.identity);

            bullet.GetComponent<Bullet>().Init(stats, aimDir);


            FireRateTimer = stats.FireRate;
        }

        public override void StartComponent()
        {
            
        }

        public override void UpdateComponent()
        {
            FireRateTimer -= FireRateTimer <= 0 ? 0 : Time.deltaTime;
        }
    }

}
